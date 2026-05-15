using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PLPSOFT.ERP.SaaS.Modules.CRM.Domain.Entities
{
    [Table("SystemTypes")]
    public class SystemType
    {
        [Key]
        public long TypeID { get; set; }

        [Required]
        [MaxLength(50)]
        public string TypeCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string TypeName { get; set; } = string.Empty;

        public virtual ICollection<SystemTypeValue> Values { get; set; } = new List<SystemTypeValue>();
    }
}
