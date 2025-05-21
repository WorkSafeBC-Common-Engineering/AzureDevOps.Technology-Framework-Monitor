using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectScannerSaveToSqlServer.DataModels
{
    public partial class PipelineType
    {
        public PipelineType()
        {
            Pipelines = [];
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        public string Value { get; set; }

        public virtual ICollection<Pipeline> Pipelines { get; set; }
    }
}
