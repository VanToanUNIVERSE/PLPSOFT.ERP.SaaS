using Microsoft.AspNetCore.Mvc;

namespace PLPSOFT.ERP.SaaS.Areas.CRM.Controllers
{
    [Area("CRM")]
    public class CrmController : Controller
    {
        // Đường dẫn sẽ là: /CRM/Crm/Dashboard
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}