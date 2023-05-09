using System;
using System.Collections.Generic;
using System.Text;

namespace AzureDevOps.Models
{
    public class AzDoProject
    {
        public string Abbreviation { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Id { get; set; } = string.Empty;

        public string LastUpdateTime { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Revision { get; set; } = string.Empty;

        public string ProjectState { get; set; } = string.Empty;

        public string Url { get; set; } = string.Empty;

        public string Visibility { get; set; } = string.Empty;
    }

    public class AzDoProjectList
    {
        public int Count { get; set; }

        public AzDoProject[] Value { get; set; } = Array.Empty<AzDoProject>();
    }
}
