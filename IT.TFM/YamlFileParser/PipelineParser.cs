using Parser.Interfaces;

using ProjectData;

using System.Text.Json;

namespace YamlFileParser
{
    public class PipelineParser : IFileParser
    {
        #region Private Members

        private const string jsonJoinChar = " ";
        private const string v1TemplateRepo = "Common-Engineering-System/AzureDevOps.Automation.Pipeline.Templates";
        private const string v2TemplateRepo = "Common-Engineering-System/AzureDevOps.Automation.Pipeline.Templates.v2";
        private const string gitRepoType = "git";

        #endregion

        #region IFileParser Implementation

        void IFileParser.Initialize(object data)
        {
            // no op
        }

        void IFileParser.Parse(FileItem file, string[] content)
        {
            var fileContents = string.Join(jsonJoinChar, content);
            using var jsonDoc = JsonDocument.Parse(fileContents, new JsonDocumentOptions { CommentHandling = JsonCommentHandling.Skip });
            var root = jsonDoc.RootElement;

            GetTemplateType(file, root);
            GetPortfolioAndProduct(file, root);
        }

        #endregion

        #region Private Methods

        private static void GetTemplateType(FileItem file, JsonElement root)
        {
            var templateType = PipelineTemplateType.Custom;
            var repoTypeValue = string.Empty;
            var repoNameValue = string.Empty;

            if (root.TryGetProperty("resources", out JsonElement resources) && resources.TryGetProperty("repositories", out JsonElement repositories))
            {
                foreach (JsonElement repository in repositories.EnumerateArray())
                {
                    if (repository.TryGetProperty("type", out JsonElement repoType) && 
                        repository.TryGetProperty("name", out JsonElement repoName))
                    {
                        repoTypeValue = repoType.GetString();
                        repoNameValue = repoName.GetString();

                        if (repoTypeValue != gitRepoType)
                        {
                            continue;
                        }

                        if (repoNameValue == v2TemplateRepo)
                        {
                            templateType = PipelineTemplateType.V2;
                            break;
                        }

                        if (repoNameValue == v1TemplateRepo)
                        {
                            templateType = PipelineTemplateType.V1;
                            break;
                        }
                    }
                }
            }

            file.AddPackageProperty("templatetype", templateType.ToString());
            file.AddPackageProperty("repotype", repoTypeValue);
            file.AddPackageProperty("reponame", repoNameValue);
        }

        private static void GetPortfolioAndProduct(FileItem file, JsonElement root)
        {
            if (root.TryGetProperty("extends", out JsonElement extends) && extends.TryGetProperty("parameters", out JsonElement parameters))
            {
                if (parameters.TryGetProperty("portfolioName", out JsonElement portfolioName))
                {
                    var portfolio = portfolioName.GetString() ?? string.Empty;
                    file.AddPackageProperty("portfolio", portfolio);
                }

                if (parameters.TryGetProperty("productName", out JsonElement productName))
                {
                    var product = productName.GetString() ?? string.Empty;
                    file.AddPackageProperty("product", product);
                }
            }
        }

        #endregion
    }
}
