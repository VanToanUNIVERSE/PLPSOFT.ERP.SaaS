using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PLPSOFT.ERP.SaaS.Modules.CRM.Domain.Entities
{
    [Table("Customers")]
    public class Customer
    {
        [Key]
        public long CustomerID { get; set; }

        public long CompanyID { get; set; }

        [Required]
        [MaxLength(50)]
        public string CustomerCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string CustomerName { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(100)]
        public string? Email { get; set; }

        [ForeignKey("CompanyID")]
        public virtual Company? Company { get; set; }
    }
}
