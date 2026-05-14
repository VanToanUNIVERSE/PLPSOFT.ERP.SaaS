namespace PLPSOFT.ERP.SaaS.Modules.CRM.Application.DTOs.Feedback
{
    /// <summary>
    /// Chứa dữ liệu dropdown cho form Tạo mới / Chỉnh sửa
    /// </summary>
    public class FeedbackFormDataDto
    {
        public List<DropdownItem> Customers { get; set; } = new();
        public List<DropdownItem> Users { get; set; } = new();
        public List<DropdownItem> FeedbackTypes { get; set; } = new();
        public List<DropdownItem> Priorities { get; set; } = new();
        public List<DropdownItem> Statuses { get; set; } = new();
    }

    public class DropdownItem
    {
        public long Value { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}
