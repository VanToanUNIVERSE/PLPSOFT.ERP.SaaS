using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace PLPSOFT.ERP.SaaS.Areas.CRM.Controllers
{
    [Area("CRM")] // Đánh dấu Controller này thuộc khu vực CRM
    public class SchedulesController : Controller
    {
        private readonly PLPSOFT.ERP.SaaS.Modules.CRM.Application.Interfaces.IFeedbackService _crmService;
        private const long CurrentCompanyID = 1;

        public SchedulesController(PLPSOFT.ERP.SaaS.Modules.CRM.Application.Interfaces.IFeedbackService crmService)
        {
            _crmService = crmService;
        }

        // Phục vụ đường dẫn: /CRM/Schedules hoặc /CRM/Schedules/Index
        public IActionResult Index()
        {
            return View();
        }

        // Phục vụ đường dẫn: /CRM/Schedules/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.FormData = await _crmService.GetFormDataAsync(CurrentCompanyID);
            return View();
        }

        // Phục vụ đường dẫn: /CRM/Schedules/Edit/1
        public IActionResult Edit(long id)
        {
            return View();
        }

        // Phục vụ đường dẫn: /CRM/Schedules/Details/1
        public IActionResult Details(long id)
        {
            return View();
        }
    }
}