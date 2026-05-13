using Microsoft.AspNetCore.Mvc;

namespace PLPSOFT.ERP.SaaS.Areas.CRM.Controllers
{
    [Area("CRM")] // Đánh dấu Controller này thuộc khu vực CRM
    public class FeedbacksController : Controller
    {
        // Phục vụ đường dẫn: /CRM/Feedbacks hoặc /CRM/Feedbacks/Index
        public IActionResult Index()
        {
            return View();
        }

        // Phục vụ đường dẫn: /CRM/Feedbacks/Create
        public IActionResult Create()
        {
            return View();
        }

        // Phục vụ đường dẫn: /CRM/Feedbacks/Edit/1
        public IActionResult Edit(long id)
        {
            return View();
        }

        // Phục vụ đường dẫn: /CRM/Feedbacks/Details/1
        public IActionResult Details(long id)
        {
            return View();
        }
    }
}