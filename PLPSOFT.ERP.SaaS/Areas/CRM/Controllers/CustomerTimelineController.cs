using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using PLPSOFT.ERP.SaaS.Modules.CRM.Application.DTOs;
using PLPSOFT.ERP.SaaS.Modules.CRM.Application.Interfaces;

namespace PLPSOFT.ERP.SaaS.Web.Areas.CRM.Controllers
{
    [Area("CRM")]
    public class CustomerTimelineController : Controller
    {
        private readonly ICustomerTimelineService _timelineService;

        public CustomerTimelineController(ICustomerTimelineService timelineService)
        {
            _timelineService = timelineService;
        }

        // GET: /CRM/CustomerTimeline/Details/1
        public async Task<IActionResult> Details(long id)
        {
            ViewBag.CustomerID = id;
            var timeline = await _timelineService.GetTimelineAsync(id);
            var notes = await _timelineService.GetNotesAsync(id);
            ViewBag.Notes = notes;
            return View(timeline);
        }

        [HttpPost]
        public async Task<IActionResult> AddNote([FromBody] CustomerNoteDto model)
        {
            if (string.IsNullOrWhiteSpace(model.Content))
            {
                return BadRequest("Nội dung không được để trống");
            }

            model.CreatedByUserID = 1; // Fix cứng User tạm thời
            await _timelineService.CreateNoteAsync(model);
            return Ok();
        }
    }
}
