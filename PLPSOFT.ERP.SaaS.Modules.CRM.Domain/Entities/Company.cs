using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PLPSOFT.ERP.SaaS.Modules.CRM.Domain.Entities
{
    [Table("Companies")]
    public class Company
    {
        [Key]
        public long CompanyID { get; set; }

        [Required]
        [MaxLength(50)]
        public string CompanyCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string CompanyName { get; set; } = string.Empty;
    }
}
