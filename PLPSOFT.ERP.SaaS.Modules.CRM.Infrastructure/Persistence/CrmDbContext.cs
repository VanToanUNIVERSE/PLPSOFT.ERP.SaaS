using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using PLPSOFT.ERP.SaaS.Modules.CRM.Domain.Entities;

namespace PLPSOFT.ERP.SaaS.Modules.CRM.Infrastructure.Persistence
{
    /// <summary>
    /// DbContext cho Module 11: CRM
    /// Kết nối tới schema [crm] và [dbo] trong PLPSOFT_ERP_SAAS_V2026
    /// </summary>
    public class CrmDbContext : DbContext
    {
        public CrmDbContext(DbContextOptions<CrmDbContext> options) : base(options) { }

        // Bảng chính (Module 11)
        public DbSet<CustomerNote> CustomerNotes { get; set; }
        public DbSet<CustomerFeedback> CustomerFeedbacks { get; set; }
        public DbSet<CustomerSchedule> CustomerSchedules { get; set; }

        // Bảng bổ trợ (Read-only, từ Module khác)
        public DbSet<SalesInvoice> SalesInvoices { get; set; }
    }

    // ============================================
    // CÁC ENTITY BỔ TRỢ (Dependency - Read Only)
    // Chỉ dùng để đọc dữ liệu cho Timeline
    // CRUD do các nhóm khác phụ trách
    // ============================================

    /// <summary>
    /// Entity ánh xạ bảng crm.CustomerFeedbacks (Read-only cho Timeline)
    /// Nhóm phụ trách Feedback sẽ viết Entity đầy đủ hơn
    /// </summary>
    [Table("CustomerFeedbacks", Schema = "crm")]
    public class CustomerFeedback
    {
        [Key] public long FeedbackID { get; set; }
        public long CustomerID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// Entity ánh xạ bảng crm.CustomerSchedules (Read-only cho Timeline)
    /// Nhóm phụ trách Schedule sẽ viết Entity đầy đủ hơn
    /// </summary>
    [Table("CustomerSchedules", Schema = "crm")]
    public class CustomerSchedule
    {
        [Key] public long ScheduleID { get; set; }
        public long CustomerID { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public long AssignedToUserID { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// Entity ánh xạ bảng dbo.SalesInvoices (Read-only cho Timeline)
    /// Nhóm Module 4 phụ trách CRUD
    /// </summary>
    [Table("SalesInvoices", Schema = "dbo")]
    public class SalesInvoice
    {
        [Key] public long InvoiceID { get; set; }
        public long CustomerID { get; set; }
        public string InvoiceCode { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
    }
}
