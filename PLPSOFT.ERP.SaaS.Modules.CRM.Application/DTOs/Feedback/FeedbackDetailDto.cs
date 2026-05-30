namespace PLPSOFT.ERP.SaaS.Modules.CRM.Application.DTOs.Feedback
{
    /// <summary>
    /// Dùng để xem chi tiết 1 phản hồi
    /// </summary>
    public class FeedbackDetailDto
    {
        public long FeedbackID { get; set; }
        public long CompanyID { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public long BranchID { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public long CustomerID { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string? CustomerPhone { get; set; }
        public string? CustomerEmail { get; set; }
        public long? InvoiceID { get; set; }
        public string? InvoiceCode { get; set; }
        
        public long FeedbackTypeID { get; set; }
        public string FeedbackTypeName { get; set; } = string.Empty;
        
        public long PriorityID { get; set; }
        public string PriorityName { get; set; } = string.Empty;
        
        public long StatusID { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public long? AssignedToUserID { get; set; }
        public string? AssignedToUserName { get; set; }
        public string? Resolution { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
    }
}
