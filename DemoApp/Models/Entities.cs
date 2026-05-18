using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoApp.Models
{
    /// <summary>
    /// Thông tin khách hàng
    /// </summary>
    [Table("Customers", Schema = "crm")]
    public class Customer
    {
        [Key] public long CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// Ghi chú khách hàng
    /// </summary>
    [Table("CustomerNotes", Schema = "crm")]
    public class CustomerNote
    {
        [Key] public long NoteID { get; set; }
        public long CustomerID { get; set; }
        public string Content { get; set; }
        public long CreatedByUserID { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// Phản hồi / khiếu nại khách hàng
    /// </summary>
    [Table("CustomerFeedbacks", Schema = "crm")]
    public class CustomerFeedback
    {
        [Key] public long FeedbackID { get; set; }
        public long CustomerID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Status { get; set; } = "Mới"; // Mới, Đang xử lý, Đã xử lý
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// Lịch chăm sóc / lịch hẹn khách hàng
    /// </summary>
    [Table("CustomerSchedules", Schema = "crm")]
    public class CustomerSchedule
    {
        [Key] public long ScheduleID { get; set; }
        public long CustomerID { get; set; }
        public string Title { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; } = "Chờ"; // Chờ, Hoàn thành, Hủy
        public DateTime CreatedAt { get; set; }
        public long AssignedToUserID { get; set; }
    }

    /// <summary>
    /// Hóa đơn bán hàng
    /// </summary>
    [Table("SalesInvoices", Schema = "dbo")]
    public class SalesInvoice
    {
        [Key] public long InvoiceID { get; set; }
        public long CustomerID { get; set; }
        public string InvoiceCode { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime InvoiceDate { get; set; }
    }
}
