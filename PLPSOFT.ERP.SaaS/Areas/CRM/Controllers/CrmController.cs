using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using PLPSOFT.ERP.SaaS.Modules.CRM.Application.Interfaces;

namespace PLPSOFT.ERP.SaaS.Areas.CRM.Controllers
{
    [Area("CRM")]
    public class CrmController : Controller
    {
        private readonly IFeedbackService _feedbackService;
        private const long CurrentCompanyID = 1;
        private const long CurrentBranchID = 1;

        public CrmController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        // Đường dẫn sẽ là: /CRM/Crm/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var data = await _feedbackService.GetDashboardDataAsync(CurrentCompanyID, CurrentBranchID);
            return View(data);
        }
    }
}