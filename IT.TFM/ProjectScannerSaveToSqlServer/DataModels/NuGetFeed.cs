using Microsoft.Identity.Client;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectScannerSaveToSqlServer.DataModels
{
    public class NuGetFeed
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [Required]
        public string Url {  get; set; }

        [Required]
        public string FeedUrl { get; set; }

        public int? ProjectId { get; set; }

        public virtual Project Project { get; set; }
    }
}
