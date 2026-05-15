using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PLPSOFT.ERP.SaaS.Modules.CRM.Domain.Entities
{
    [Table("crm_CustomerSchedules")]
    public class CustomerSchedule
    {
        [Key]
        public long ScheduleID { get; set; }

        public long CompanyID { get; set; }

        public long BranchID { get; set; }

        public long CustomerID { get; set; }

        public long ScheduleTypeID { get; set; }

        public long StatusID { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public long CreatedByUserID { get; set; }

        public long AssignedToUserID { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedAt { get; set; }

        // Navigation Properties
        [ForeignKey("CompanyID")]
        public virtual Company? Company { get; set; }

        [ForeignKey("BranchID")]
        public virtual Branch? Branch { get; set; }

        [ForeignKey("CustomerID")]
        public virtual Customer? Customer { get; set; }

        [ForeignKey("ScheduleTypeID")]
        public virtual SystemTypeValue? ScheduleType { get; set; }

        [ForeignKey("StatusID")]
        public virtual SystemTypeValue? Status { get; set; }

        [ForeignKey("CreatedByUserID")]
        public virtual User? CreatedByUser { get; set; }

        [ForeignKey("AssignedToUserID")]
        public virtual User? AssignedToUser { get; set; }
    }
}
