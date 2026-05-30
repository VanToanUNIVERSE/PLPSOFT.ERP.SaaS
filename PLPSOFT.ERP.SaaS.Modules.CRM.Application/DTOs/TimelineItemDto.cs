using System;

namespace PLPSOFT.ERP.SaaS.Modules.CRM.Application.DTOs
{
    /// <summary>
    /// DTO chuẩn hóa cho 1 sự kiện trên Timeline (Hóa đơn, Phản hồi, Lịch hẹn, Ghi chú)
    /// Dùng chung cho tất cả các loại sự kiện, hiển thị trên 1 trục thời gian thống nhất.
    /// </summary>
    public class TimelineItemDto
    {
        /// <summary>Thời gian xảy ra sự kiện</summary>
        public DateTime EventTime { get; set; }

        /// <summary>Loại: INVOICE, FEEDBACK, SCHEDULE, NOTE</summary>
        public string EventType { get; set; } = string.Empty;

        /// <summary>Badge hiển thị: [Hóa đơn], [Phản hồi], [Lịch hẹn], [Ghi chú]</summary>
        public string EventBadge { get; set; } = string.Empty;

        /// <summary>Tiêu đề sự kiện</summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>Mô tả chi tiết</summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>CSS class cho viền màu phân biệt loại</summary>
        public string CssClass { get; set; } = string.Empty;
    }
}
