using Parser.Interfaces;

using ProjectData;

using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Text;

namespace YamlFileParser
{
    public class PipelineParser : IFileParser
    {
        #region Private Members

        private static readonly char[] variableSeparator = [':'];
        private const string v1TemplateRepo = "/AzureDevOps.Automation.Pipeline.Templates";
        private const string v2TemplateRepo = "/AzureDevOps.Automation.Pipeline.Templates.v2";

        #endregion

        #region IFileParser Implementation

        void IFileParser.Initialize(object data)
        {
            // no op
        }

        void IFileParser.Parse(FileItem file, string[] content)
        {
            var cleanContent = StripComments(content);

            if (CheckIfConfigFile(file))
            {
                ParseConfigFile(file, cleanContent);
            }
            else
            {
                ParsePipelineFile(file, cleanContent);
            }
        }

        #endregion

        #region Private Methods

        private static bool CheckIfConfigFile(FileItem file)
        {
            return file.Path.EndsWith("-config.yml", StringComparison.InvariantCultureIgnoreCase);
        }

        private static void ParseConfigFile(FileItem file, string[] cleanContent)
        {
            // skip if not in the same repository as the pipeline
            if (!file.Path.StartsWith("/deploy/"))
            {
                return;
            }

            var environments = ParseEnabledEnvironments(cleanContent);
            if (environments == null || environments.Count == 0)
            {
                return;
            }

            file.PipelineProperties.Add("IsConfigFile", "true");
            file.PipelineProperties.Add("Environments", string.Join('|', environments));
            ParseConfigProperties(file);
        }

        private static List<string> ParseEnabledEnvironments(string[] content)
        {
            var environments = new List<string>();

            var enabledEnvironments = new List<string>();


            for (var index = 0; index < content.Length; index++)
            {
                var line = content[index].Trim();
                if (line.Equals("variables:", StringComparison.OrdinalIgnoreCase))
                {
                    index++;

                    while (index < content.Length - 1)
                    {
                        var variable = ParseConfigEnabledVariable(content[index], content[index + 1].Trim());

                        if (variable != null && variable.Value.Item2)
                        {
                            enabledEnvironments.Add(variable.Value.Item1);
                        }

                        index += 2;
                    }

                    index = 1;
                    foreach (var enabledItem in enabledEnvironments)
                    {
                        var environmentNameAndIndex = ParseConfigEnvironmentName(enabledItem, index, content);
                        if (environmentNameAndIndex == null)
                        {
                            continue;
                        }

                        environments.Add(environmentNameAndIndex.Value.Item1);
                        index = environmentNameAndIndex.Value.Item2;
                    }
                }
            }

            return environments;
        }

        private static (string, bool)? ParseConfigEnabledVariable(string name, string value)
        {
            if (name.StartsWith("- name:") && value.StartsWith("value:"))
            {
                var nameValue = name[7..].Trim();

                if (nameValue.EndsWith("StageActive"))
                {
                    nameValue = nameValue.Replace("StageActive", string.Empty);

                    if (!bool.TryParse(value[6..].Trim(), out var boolValue))
                    {
                        boolValue = false;
                    }

                    return (nameValue, boolValue);
                }
            }

            return null;
        }

        private static (string, int)? ParseConfigEnvironmentName(string environment, int index, string[] content)
        {
            var parseIndex = index;
            while (parseIndex - 1 < content.Length)
            {
                if (content[parseIndex].Trim().StartsWith("- name:"))
                {
                    var name = content[parseIndex][7..].Trim();
                    var value = content[parseIndex + 1].Trim();
                    if (name.Equals($"{environment}StageName") && value.StartsWith("value:"))
                    {
                        return (value[6..].Trim().Replace("'", string.Empty), parseIndex);
                    }
                }

                parseIndex += 2;
            }

            return null;
        }

        private static void ParseConfigProperties(FileItem file)
        {
            // Get the portfolio and product from the file path
            var filename = Path.GetFileNameWithoutExtension(file.Path)
                               .Replace("-config", string.Empty);

            var parts = filename.Split('-', 2, StringSplitOptions.TrimEntries);

            if (parts.Length < 2)
            {
                parts = filename.Split('.', 2, StringSplitOptions.TrimEntries);
            }

            if (parts.Length >= 2 && !string.IsNullOrEmpty(parts[0]) && !string.IsNullOrEmpty(parts[1]))
            {
                file.PipelineProperties.Add("portfolio", parts[0]);
                file.PipelineProperties.Add("product", parts[1]);
            }
        }

        private static void ParsePipelineFile(FileItem file, string[] cleanContent)
        {
            var variables = ParseVariables(cleanContent);

            var templateType = GetTemplateType(cleanContent);
            file.PipelineProperties.Add("template", templateType.ToString());

            switch (templateType)
            {
                case PipelineTemplateType.V1:
                    ParseV1Template(cleanContent, variables, file);
                    // In some cases, a V1 template extends a template similar to a V2 template
                    if (file.PipelineProperties["portfolio"] == string.Empty && file.PipelineProperties["product"] == string.Empty)
                    {
                        ParseV2Template(cleanContent, variables, file);
                    }
                    break;

                case PipelineTemplateType.V2:
                    ParseV2Template(cleanContent, variables, file);

                    // Generic pipelines are structured like a V1 with respect to parsing the portfolio and product values.
                    if (file.PipelineProperties["portfolio"] == string.Empty && file.PipelineProperties["product"] == string.Empty)
                    {
                        ParseV1Template(cleanContent, variables, file);
                    }

                    if (!file.PipelineProperties.ContainsKey("blueprintType") && file.PipelineProperties.ContainsKey("blueprint"))
                    {
                        var genericType = GetGenericBlueprintType(file.PipelineProperties["blueprint"]);
                        if (genericType != BlueprintType.None)
                        {
                            file.PipelineProperties["blueprintType"] = genericType.ToString();
                        }
                    }

                    // If portfolio and product are still empty, see if any of the variables match.
                    if (file.PipelineProperties["portfolio"] == string.Empty && file.PipelineProperties["product"] == string.Empty)
                    {
                        if (variables.TryGetValue("portfolioName", out var portfolio))
                        {
                            file.PipelineProperties["portfolio"] = portfolio;
                        }

                        if (variables.TryGetValue("productName", out var product))
                        {
                            file.PipelineProperties["product"] = product;
                        }
                    }

                    // SuppressCD parameter defaults to false if not found
                    if (!file.PipelineProperties.ContainsKey("suppressCD") && !file.PipelineProperties["blueprintType"].StartsWith("Generic"))
                    {
                        file.PipelineProperties["suppressCD"] = "false";
                    }
                    break;
            }
        }

        private static string[] StripComments(string[] content)
        {
            List<string> lines = [];
            foreach (var line in content)
            {
                if (!IsComment(line) && !string.IsNullOrWhiteSpace(line))
                {
                    var cleanline = StripTrailingComments(line);
                    lines.Add(cleanline);
                }
            }

            return [.. lines];
        }

        private static PipelineTemplateType GetTemplateType(string[] content)
        {
            var templateType = PipelineTemplateType.Custom;
            for (var index = 0; index < content.Length; index++)
            {
                if (content[index].Equals("resources:"))
                {
                    index++;
                    if (content[index].Equals("  repositories:"))
                    {
                        index++;
                        if (content[index].TrimStart().StartsWith("- repository:"))
                        {
                            do
                            {
                                index++;
                                if (content[index].Trim().StartsWith("name:"))
                                {
                                    var fields = content[index].Split(variableSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                                    if (fields.Length != 2)
                                    {
                                        return templateType;
                                    }

                                    var templateName = fields[1];
                                    if (templateName.StartsWith('\'') && templateName.EndsWith('\''))
                                    {
                                        templateName = templateName.Replace("'", string.Empty);
                                    }
                                    else if (templateName.StartsWith('"') && templateName.EndsWith('"'))
                                    {
                                        templateName = templateName.Replace("\"", string.Empty);
                                    }

                                    if (templateName.EndsWith(v2TemplateRepo, StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        templateType = PipelineTemplateType.V2;
                                    }
                                    else if (templateName.EndsWith(v1TemplateRepo, StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        templateType = PipelineTemplateType.V1;
                                    }
                                    return templateType;
                                }
                            } while (index < content.Length);
                        }
                    }
                }
            }


            return templateType;
        }

        private static void ParseV1Template(string[] content, Dictionary<string, string> variables, FileItem file)
        {
            var portfolio = string.Empty;
            var product = string.Empty;
            var appBlueprint = string.Empty;

            for (var index = 0; index < content.Length; index++)
            {
                if (content[index].Trim().StartsWith("- template:"))
                {
                    do
                    {
                        index++;
                    } while (content[index].Trim().StartsWith("- template:"));

                    if (content[index].Trim().Equals("parameters:"))
                    {
                        while (++index < content.Length)
                        {
                            if (content[index].Trim().StartsWith('-'))
                            {
                                break;
                            }

                            else
                            {
                                var fields = content[index].Split(variableSeparator, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                                if (fields.Length == 2)
                                {
                                    switch (fields[0])
                                    {
                                        case "portfolioName":
                                            portfolio = GetParameterValue(fields[1], variables);
                                            break;

                                        case "productName":
                                            product = GetParameterValue(fields[1], variables);
                                            break;

                                        case "applicationBlueprint":
                                            appBlueprint = GetParameterValue(fields[1], variables);
                                            break;
                                    }
                                }
                            }
                        }

                        if (portfolio != string.Empty && product != string.Empty && appBlueprint != string.Empty)
                        {
                            break;
                        }
                    }
                }
            }

            if (portfolio.StartsWith('$'))
            {
                portfolio = null;
            }

            if (product.StartsWith('$'))
            {
                product = null;
            }

            file.PipelineProperties["portfolio"] = portfolio;
            file.PipelineProperties["product"] = product;
            file.PipelineProperties["blueprint"] = appBlueprint;
        }

        private static void ParseV2Template(string[] content, Dictionary<string, string> variables, FileItem file)
        {
            var portfolio = string.Empty;
            var product = string.Empty;
            var suppressCD = "false"; // default if not specified in pipeline
            var blueprintType = BlueprintType.None;

            for (var index = 0; index < content.Length; index++)
            {
                if (content[index].Equals("extends:"))
                {
                    index++;
                    if (content[index].Trim().StartsWith("template:"))
                    {
                        blueprintType = GetBlueprintType(content[index]);

                        index++;
                        if (content[index].Trim().Equals("parameters:"))
                        {
                            while (++index < content.Length)
                            {
                                var fields = content[index].Split(variableSeparator, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

                                if (fields[0].StartsWith('-'))
                                {
                                    break;
                                }
                                else if (fields.Length != 2)
                                {
                                    // This is probably a list of items assigned to a parameter

                                    while (content[++index].Trim().StartsWith('-'))
                                    {
                                        //nop
                                    }

                                    index--;
                                    continue;
                                }

                                switch (fields[0])
                                {
                                    case "portfolioName":
                                        portfolio = GetParameterValue(fields[1], variables);
                                        break;

                                    case "productName":
                                        product = GetParameterValue(fields[1], variables);
                                        break;

                                    case "suppressCD":
                                        suppressCD = GetParameterValue(fields[1], variables);
                                        break;
                                }
                            }
                        }
                    }
                }
            }

            file.PipelineProperties["portfolio"] = portfolio;
            file.PipelineProperties["product"] = product;
            file.PipelineProperties["suppressCD"] = suppressCD;
            if (blueprintType != BlueprintType.None)
            {
                file.PipelineProperties["blueprintType"] = blueprintType.ToString();
            }
        }

        private static string GetParameterValue(string parameter, Dictionary<string, string> variables)
        {
            string value = parameter;
            if (parameter.StartsWith("${{variables.") && parameter.EndsWith("}}"))
            {
                value = variables[parameter[13..^2]];
            }

            if (parameter.StartsWith("$(") && parameter.EndsWith(')'))
            {
                value = variables[parameter[2..^1]];
            }

            return value.Replace("\"", string.Empty).Replace("'", string.Empty);
        }

        private static bool IsComment(string line)
        {
            return line.TrimStart().StartsWith('#');
        }

        private static string StripTrailingComments(string line)
        {
            var cleanLine = line;
            if (cleanLine.Contains('#'))
            {
                var index = cleanLine.IndexOf('#');
                cleanLine = cleanLine[..index];
            }

            return cleanLine;
        }

        private static Dictionary<string, string> ParseVariables(string[] content)
        {
            var variables = new Dictionary<string, string>();
            bool inVariables = false;

            for (var index = 0; index < content.Length; index++)
            {
                var line = content[index];

                if (!inVariables)
                {
                    if (line.Trim().Equals("variables:"))
                    {
                        inVariables = true;
                    }
                }
                else
                {
                    if (line.Trim().StartsWith('-'))
                    {
                        var variableLine = line[1..].Trim();
                        var variableFields = variableLine.Split(variableSeparator, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                        if (variableFields.Length != 2 || variableFields[0] != "name")
                        {
                            break;
                        }

                        var name = variableFields[1];

                        variableLine = content[++index];
                        variableFields = variableLine.Split(variableSeparator, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                        if (variableFields.Length != 2 || variableFields[0] != "value")
                        {
                            break;
                        }

                        var value = variableFields[1];

                        variables.Add(name, value);
                    }

                    else if (!line.StartsWith("  ")) // two space indent
                    {
                        break;
                    }

                    else
                    {
                        var fields = line.Split(variableSeparator, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                        if (fields.Length != 2)
                        {
                            break;
                        }

                        variables.Add(fields[0], fields[1]);
                    }

                }
            }

            return variables;
        }

        private static BlueprintType GetBlueprintType(string template)
        {
            var templateType = template[(template.LastIndexOf('/') + 1)..];
            templateType = templateType[..templateType.IndexOf('@')];

            return templateType switch
            {
                "azure-pipeline-azure-function-control.yml" => BlueprintType.AzureFunction,
                "azure-pipeline-batch-console-control.yml" => BlueprintType.BatchConsole,
                "azure-pipeline-nuget-package-control.yml" => BlueprintType.NugetPackage,
                "azure-pipeline-universal-artifact-control.yml" => BlueprintType.UniversalArtifact,
                "azure-pipeline-webapp-control.yml" => BlueprintType.WebApp,
                "azure-pipeline-winapp-appsync-control.yml" => BlueprintType.WinAppAppSync,
                _ => BlueprintType.None,
            };
        }

        private static BlueprintType GetGenericBlueprintType(string genericBlueprint)
        {
            switch (genericBlueprint)
            {
                case "generic-steps":
                    return BlueprintType.GenericSteps;

                case "generic-jobs":
                    return BlueprintType.GenericJobs;

                default:
                    return BlueprintType.None;
            }
        }

        private static string GetConfigFileName(Dictionary<string, string> pipelineProperties)
        {
            var portfolio = pipelineProperties["portfolioName"];
            var product = pipelineProperties["productName"];

            if (!string.IsNullOrEmpty(portfolio) && !string.IsNullOrEmpty(product))
            {
                return $"/deploy/{portfolio}-{product}-config.yml";
            }

            return string.Empty;
        }

        #endregion
    }
}
