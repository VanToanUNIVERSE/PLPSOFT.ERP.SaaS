using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using PLPSOFT.ERP.SaaS.Modules.CRM.Application.Interfaces;

namespace PLPSOFT.ERP.SaaS.Areas.CRM.Controllers
{
    [Area("CRM")]
    public class CrmController : Controller
    {
        private readonly IFeedbackService _feedbackService;
        private readonly IScheduleService _scheduleService;
        private const long CurrentCompanyID = 1;
        private const long CurrentBranchID = 1;

        public CrmController(IFeedbackService feedbackService, IScheduleService scheduleService)
        {
            _feedbackService = feedbackService;
            _scheduleService = scheduleService;
        }

        // Đường dẫn sẽ là: /CRM/Crm/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var data = await _feedbackService.GetDashboardDataAsync(CurrentCompanyID, CurrentBranchID);
            
            // Lấy thêm dữ liệu lịch chăm sóc
            var allUpcoming = await _scheduleService.GetUpcomingAsync();
            var todayUpcoming = await _scheduleService.GetUpcomingAsync(false);
            var overdue = await _scheduleService.GetOverdueAsync();
            
            data.TotalUpcomingSchedules = allUpcoming.Count;
            data.TotalOverdueSchedules = overdue.Count;
            data.TodaySchedules = todayUpcoming.Take(5).ToList();

            return View(data);
        }
    }
}