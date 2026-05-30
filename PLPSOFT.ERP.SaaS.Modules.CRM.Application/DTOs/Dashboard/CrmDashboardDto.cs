using PLPSOFT.ERP.SaaS.Modules.CRM.Application.DTOs.Feedback;

namespace PLPSOFT.ERP.SaaS.Modules.CRM.Application.DTOs.Dashboard
{
    public class CrmDashboardDto
    {
        public int TotalNewFeedbacks { get; set; }
        public int TotalUpcomingSchedules { get; set; } // Tạm thời để 0 nếu chưa có DB
        public int TotalOverdueSchedules { get; set; }   // Tạm thời để 0 nếu chưa có DB
        
        public List<FeedbackListItemDto> RecentFeedbacks { get; set; } = new();
        public List<ScheduleListViewModel> TodaySchedules { get; set; } = new();
    }
}
