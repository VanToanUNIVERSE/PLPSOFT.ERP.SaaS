using System;

namespace PLPSOFT.ERP.SaaS.Modules.CRM.Application.DTOs
{
    public class TimelineItemDto
    {
        public DateTime EventTime { get; set; }
        public string EventType { get; set; } // Hóa đơn, Phản hồi, Lịch hẹn, Ghi chú
        public string EventBadge { get; set; } // Hiển thị dạng [Hóa đơn], [Ghi chú]
        public string Title { get; set; }
        public string Description { get; set; }
        public string Actor { get; set; }
        public string Status { get; set; }
        public string CssClass { get; set; }
    }
}
