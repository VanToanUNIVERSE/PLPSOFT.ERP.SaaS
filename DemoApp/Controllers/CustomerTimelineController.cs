using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using DemoApp.Models;
using DemoApp.Services;

namespace DemoApp.Controllers
{
    public class CustomerTimelineController : Controller
    {
        private readonly ICustomerTimelineService _timelineService;

        public CustomerTimelineController(ICustomerTimelineService timelineService)
        {
            _timelineService = timelineService;
        }

        /// <summary>
        /// Trang chính - Hiển thị timeline 360° của khách hàng
        /// </summary>
        public async Task<IActionResult> Index(long id = 1)
        {
            var customers = await _timelineService.GetAllCustomersAsync();
            var summary = await _timelineService.GetCustomerSummaryAsync(id);
            var timeline = await _timelineService.GetTimelineAsync(id);

            ViewBag.CustomerID = id;
            ViewBag.Customers = customers;
            ViewBag.Summary = summary;
            return View(timeline);
        }

        /// <summary>
        /// Trang ghi chú - Xem tất cả ghi chú của khách hàng
        /// </summary>
        public async Task<IActionResult> Notes(long id = 1)
        {
            var customers = await _timelineService.GetAllCustomersAsync();
            var summary = await _timelineService.GetCustomerSummaryAsync(id);
            var notes = await _timelineService.GetNotesAsync(id);

            ViewBag.CustomerID = id;
            ViewBag.Customers = customers;
            ViewBag.Summary = summary;
            return View(notes);
        }

        /// <summary>
        /// API: Thêm ghi chú mới
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddNote([FromBody] CustomerNoteDto model)
        {
            if (string.IsNullOrWhiteSpace(model.Content))
                return BadRequest(new { message = "Nội dung ghi chú không được để trống" });

            model.CreatedByUserID = 1; // Demo: hardcode user
            var noteId = await _timelineService.CreateNoteAsync(model);
            return Ok(new { noteId, message = "Thêm ghi chú thành công" });
        }

        /// <summary>
        /// API: Lấy timeline theo filter
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetTimeline(long customerId, string filter = "ALL")
        {
            var timeline = await _timelineService.GetTimelineAsync(customerId);
            if (filter != "ALL")
            {
                timeline = timeline.Where(t => t.EventType == filter).ToList();
            }
            return Json(timeline);
        }

        /// <summary>
        /// API: Lấy ghi chú theo khách hàng (JSON)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetNotes(long customerId)
        {
            var notes = await _timelineService.GetNotesAsync(customerId);
            return Json(notes);
        }
    }
}
