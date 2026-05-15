using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PLPSOFT.ERP.SaaS.Modules.CRM.Domain.Entities
{
    [Table("Users")]
    public class User
    {
        [Key]
        public long UserID { get; set; }

        public long CompanyID { get; set; }

        [Required]
        [MaxLength(100)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string FullName { get; set; } = string.Empty;

        [ForeignKey("CompanyID")]
        public virtual Company? Company { get; set; }
    }
}
