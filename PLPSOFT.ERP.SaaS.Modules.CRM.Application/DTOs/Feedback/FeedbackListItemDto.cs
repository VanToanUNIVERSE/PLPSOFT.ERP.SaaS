namespace PLPSOFT.ERP.SaaS.Modules.CRM.Application.DTOs.Feedback
{
    /// <summary>
    /// Dùng để hiển thị danh sách phản hồi khách hàng
    /// </summary>
    public class FeedbackListItemDto
    {
        public long FeedbackID { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string FeedbackTypeName { get; set; } = string.Empty;
        public string PriorityName { get; set; } = string.Empty;
        public string StatusName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? AssignedToUserName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
    }
}
