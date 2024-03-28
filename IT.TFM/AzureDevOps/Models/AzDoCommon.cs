using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDevOps.Models
{
    public class Self
    {
        public string Href { get; set; }
    }

    public class Web
    {
        public string Href { get; set; }
    }

    public class Links
    {
        public Self Self { get; set; }
        public Web Web { get; set; }
    }

    public class Avatar
    {
        public string href { get; set; }
    }

    public class Createdby
    {
        public string displayName { get; set; }
        public string url { get; set; }
        public Links1 _links { get; set; }
        public string id { get; set; }
        public string uniqueName { get; set; }
        public string imageUrl { get; set; }
        public string descriptor { get; set; }
        public bool inactive { get; set; }
    }

    public class Modifiedby
    {
        public string displayName { get; set; }
        public string url { get; set; }
        public Links1 _links { get; set; }
        public string id { get; set; }
        public string uniqueName { get; set; }
        public string imageUrl { get; set; }
        public string descriptor { get; set; }
    }

    public class SystemDebug
    {
        public string Value { get; set; }
        public bool AllowOverride { get; set; }
    }

    public class Definition
    {
        public string id { get; set; }
        public string name { get; set; }
    }
    public class Variables
    {
        public BuildConfiguration BuildConfiguration { get; set; }
        public BuildPlatform BuildPlatform { get; set; }
        [JsonProperty("Portfolio.ProductName")]
        public PortfolioProductName PortfolioProductName { get; set; }
        public SystemDebug SystemDebug { get; set; }
        public Approotfolder AppRootFolder { get; set; }
        public Productfamilyname ProductFamilyName { get; set; }
        public SystemDebug systemdebug { get; set; }
        public Temprootfolder TempRootFolder { get; set; }
        public Webservicename WebServiceName { get; set; }
        public Websitenamelayer7 WebsiteNameLayer7 { get; set; }
        public Apppoolidentityusername AppPoolIdentityUsername { get; set; }

    }
}
