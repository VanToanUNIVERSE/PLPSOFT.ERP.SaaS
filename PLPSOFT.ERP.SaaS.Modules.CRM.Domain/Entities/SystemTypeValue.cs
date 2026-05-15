using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PLPSOFT.ERP.SaaS.Modules.CRM.Domain.Entities
{
    [Table("SystemTypeValues")]
    public class SystemTypeValue
    {
        [Key]
        public long TypeValueID { get; set; }

        public long TypeID { get; set; }

        [Required]
        [MaxLength(50)]
        public string ValueCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string ValueName { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        [ForeignKey("TypeID")]
        public virtual SystemType? SystemType { get; set; }
    }
}
