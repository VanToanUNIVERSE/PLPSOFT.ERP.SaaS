using Microsoft.AspNetCore.Mvc;

namespace PLPSOFT.ERP.SaaS.Areas.CRM.Controllers
{
    public class FeedbacksController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
