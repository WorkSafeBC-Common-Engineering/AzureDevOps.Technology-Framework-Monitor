using Parser.Interfaces;

using ProjectData;

namespace YamlFileParser
{
    public class PipelineParser : IFileParser
    {
        #region Private Members

        private static readonly char[] variableSeparator = [':'];
        private static readonly char[] pathSeparator = ['/', '\\'];
        private const string v1TemplateRepo = "/AzureDevOps.Automation.Pipeline.Templates";
        private const string v2TemplateRepo = "/AzureDevOps.Automation.Pipeline.Templates.v2";

        private const string defaultBlueprint = "generic";
        private const string templatePathRootFolder = "blueprints";

        #endregion

        #region IFileParser Implementation

        void IFileParser.Initialize(object data)
        {
            // no op
        }

        void IFileParser.Parse(FileItem file, string[] content)
        {
            var cleanContent = StripComments(content);
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
                        file.PipelineProperties.Remove("blueprint");
                        ParseV1Template(cleanContent, variables, file);
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
                    break;
            }
        }

        #endregion

        #region Private Methods

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
            var blueprint = string.Empty;

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
                                            blueprint = GetParameterValue(fields[1], variables);
                                            break;
                                    }
                                }
                            }
                        }

                        if (portfolio != string.Empty && product != string.Empty)
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
            file.PipelineProperties["blueprint"] = blueprint;
        }

        private static void ParseV2Template(string[] content, Dictionary<string, string> variables, FileItem file)
        {
            var blueprint = defaultBlueprint;

            for (var index = 0; index < content.Length; index++)
            {
                if (content[index].Equals("extends:"))
                {
                    index++;
                    if (content[index].Trim().StartsWith("template:"))
                    {
                        var blueprintValue = GetBlueprintName(content[index]);
                        if (blueprintValue != string.Empty)
                        {
                            blueprint = blueprintValue;
                        }

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
                                        file.PipelineProperties["portfolio"] = GetParameterValue(fields[1], variables);
                                        break;

                                    case "productName":
                                        file.PipelineProperties["product"] = GetParameterValue(fields[1], variables);
                                        break;

                                    default:
                                        file.PipelineProperties[fields[0]] = GetParameterValue(fields[1], variables);
                                        break;
                                }
                            }
                        }
                    }
                }
            }

            file.PipelineProperties["blueprint"] = blueprint;
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

        private static string GetBlueprintName(string line)
        {
            var fields = line.Split(variableSeparator, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (fields.Length != 2)
            {
                return string.Empty;
            }

            var pathFields = fields[1].Split(pathSeparator, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (pathFields.Length <= 1 ||  pathFields[0] != templatePathRootFolder)
            {
                return string.Empty;
            }

            return pathFields[1];
        }

        #endregion
    }
}
