using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDevOps.Models
{
    public class AzDoPipeline
    {
        public int? Id { get; set; }

        public string? Name { get; set; }

        public string? Folder { get; set; }

        public int? Revision { get; set; }

        public string? Url { get; set; }

        public AzDoPipelineConfiguration? Configuration { get; set; }
    }

    public class AzDoPipelineList
    {
        public int Count { get; set; }

        public AzDoPipeline[]? Value { get; set; }
    }

    public class AzDoPipelineConfiguration
    {
        public string? Type { get; set; }

        public AzDoPipelineConfigurationDesignerHyphenJson? DesignerHyphenJson { get; set; }

        public AzDoPipelineConfigurationDesignerJson? DesignerJson { get; set; }

        public AzDoPipelineConfigurationJustInTime? JustInTime { get; set; }

        public AzDoPipelineConfigurationUnknown? Unknown { get; set; }

        public AzDoPipelineConfigurationYaml? Yaml { get; set; }
    }

    public class AzDoPipelineConfigurationDesignerHyphenJson
    {

    }

    public class AzDoPipelineConfigurationDesignerJson
    {
        public string Type { get; set; } = string.Empty;

        public string QueueStatus { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }

        public AzDoAuthor? AuthoredBy { get; set; }
       
        public string Quality { get; set; } = string.Empty;

        public AzDoPipelineRepository? Repository { get; set; }
    }

    public class AzDoPipelineConfigurationJustInTime
    {

    }

    public class AzDoPipelineConfigurationUnknown
    {

    }

    public class AzDoPipelineConfigurationYaml
    {

    }

    public class  AzDoAuthor
    {
        public string DisplayName { get; set; } = string.Empty;        
    }

    public class AzDoPipelineRepository
    {
        public string Id { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
    }
}
