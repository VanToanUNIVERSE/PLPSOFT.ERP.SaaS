using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PLPSOFT.ERP.SaaS.Modules.CRM.Domain.Entities
{
    [Table("Branches")]
    public class Branch
    {
        [Key]
        public long BranchID { get; set; }

        public long CompanyID { get; set; }

        [Required]
        [MaxLength(50)]
        public string BranchCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string BranchName { get; set; } = string.Empty;

        [ForeignKey("CompanyID")]
        public virtual Company? Company { get; set; }
    }
}
