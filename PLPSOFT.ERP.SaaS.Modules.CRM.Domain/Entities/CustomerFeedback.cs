namespace PLPSOFT.ERP.SaaS.Modules.CRM.Domain.Entities
{
    public class CustomerFeedback
    {
        public long FeedbackID { get; set; }
        public long CompanyID { get; set; }
        public long BranchID { get; set; }
        public long CustomerID { get; set; }
        public long? InvoiceID { get; set; }
        public long FeedbackTypeID { get; set; }
        public long PriorityID { get; set; }
        public long StatusID { get; set; }
        public int Rating { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public long? AssignedToUserID { get; set; }
        public string? Resolution { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        // Navigation properties
        public Company? Company { get; set; }
        public Branch? Branch { get; set; }
        public Customer? Customer { get; set; }
        public SalesInvoice? Invoice { get; set; }
        public SystemTypeValue? FeedbackType { get; set; }
        public SystemTypeValue? Priority { get; set; }
        public SystemTypeValue? Status { get; set; }
        public User? AssignedToUser { get; set; }
    }
}
