using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PLPSOFT.ERP.SaaS.Modules.CRM.Domain.Entities
{
    /// <summary>
    /// Bảng crm.CustomerNotes - Ghi chú khách hàng (Bảng phụ trách chính)
    /// </summary>
    [Table("CustomerNotes", Schema = "crm")]
    public class CustomerNote
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long NoteID { get; set; }

        [Required]
        public long CustomerID { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        [Required]
        public long CreatedByUserID { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
