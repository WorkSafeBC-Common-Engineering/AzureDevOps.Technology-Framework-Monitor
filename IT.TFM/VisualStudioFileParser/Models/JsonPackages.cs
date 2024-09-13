using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VisualStudioFileParser.Models
{
    internal class PackageIssuesInstance
    {
        public int Version { get; set; }
        public string Parameters { get; set; }
        public Problem[] Problems { get; set; }
        public string[] Sources { get; set; }
        public Project[] Projects { get; set; }
    }

    internal class Problem
    {
        public string Project { get; set; }
        public string Level { get; set; }
        public string Text { get; set; }
    }

    internal class Project
    {
        public string Path { get; set; }

        public Framework[] Frameworks { get; set; }
    }

    internal class Framework
    {
        [JsonPropertyName("framework")]
        public string FrameworkVersion { get; set; }
        public ToplevelPackage[] TopLevelPackages { get; set; }
        public TransitivePackage[] TransitivePackages { get; set; }
    }

    internal class ToplevelPackage
    {
        public string Id { get; set; }
        public string RequestedVersion { get; set; }
        public string ResolvedVersion { get; set; }
        public string LatestVersion { get; set; }
        public Vulnerability[] Vulnerabilities { get; set; }
    }

    internal class TransitivePackage
    {
        public string Id { get; set; }
        public string RequestedVersion { get; set; }
        public string ResolvedVersion { get; set; }
        public string LatestVersion { get; set; }
        public Vulnerability[] Vulnerabilities { get; set; }
    }

    internal class Vulnerability
    {
        public string Severity { get; set; }

        public string AdvisoryUrl { get; set; }
    }
}
