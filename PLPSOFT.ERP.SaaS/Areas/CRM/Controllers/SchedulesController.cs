using Microsoft.AspNetCore.Mvc;

namespace PLPSOFT.ERP.SaaS.Areas.CRM.Controllers
{
    [Area("CRM")] // Đánh dấu Controller này thuộc khu vực CRM
    public class SchedulesController : Controller
    {
        // Phục vụ đường dẫn: /CRM/Schedules hoặc /CRM/Schedules/Index
        public IActionResult Index()
        {
            return View();
        }

        // Phục vụ đường dẫn: /CRM/Schedules/Create
        public IActionResult Create()
        {
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