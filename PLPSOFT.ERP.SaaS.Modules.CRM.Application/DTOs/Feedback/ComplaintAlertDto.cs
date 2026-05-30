namespace PLPSOFT.ERP.SaaS.Modules.CRM.Application.DTOs.Feedback
{
    /// <summary>
    /// DTO đại diện cho một thông báo khiếu nại hiển thị trên chuông notification.
    /// Được lưu tạm trong IMemoryCache — không lưu vào database.
    /// </summary>
    public class ComplaintAlertDto
    {
        /// <summary>ID tạm thời (GUID, không dấu gạch nối) — dùng để dismiss khỏi cache</summary>
        public string AlertId { get; set; } = string.Empty;

        /// <summary>ID phản hồi gốc — dùng để điều hướng đến trang Details</summary>
        public long FeedbackID { get; set; }

        /// <summary>Tên khách hàng đã gửi khiếu nại</summary>
        public string CustomerName { get; set; } = string.Empty;

        /// <summary>Tiêu đề khiếu nại</summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>Tên chi nhánh liên quan</summary>
        public string BranchName { get; set; } = string.Empty;

        /// <summary>Thời điểm khiếu nại được tạo</summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Chuỗi hiển thị thân thiện: "vừa xong", "5 phút trước", "2 giờ trước"...
        /// Được tính toán runtime, không lưu vào cache.
        /// </summary>
        public string TimeAgo { get; set; } = string.Empty;
    }
}
