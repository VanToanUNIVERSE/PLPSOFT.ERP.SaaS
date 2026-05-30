using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using PLPSOFT.ERP.SaaS.Modules.CRM.Application.DTOs;
using PLPSOFT.ERP.SaaS.Modules.CRM.Application.Interfaces;

namespace PLPSOFT.ERP.SaaS.Areas.CRM.Controllers
{
    /// <summary>
    /// Controller xử lý Customer Timeline & Notes
    /// URL: /CRM/CustomerTimeline/Index/{customerId}
    /// </summary>
    [Area("CRM")]
    public class CustomerTimelineController : Controller
    {
        private readonly ICustomerTimelineService _timelineService;

        public CustomerTimelineController(ICustomerTimelineService timelineService)
        {
            _timelineService = timelineService;
        }

        /// <summary>
        /// Hiển thị danh sách Khách hàng + ô tìm kiếm theo tên
        /// GET: /CRM/CustomerTimeline/Index
        /// </summary>
        public async Task<IActionResult> Index(string searchName)
        {
            ViewBag.SearchName = searchName;
            var customers = await _timelineService.SearchCustomersAsync(searchName);
            return View(customers);
        }

        /// <summary>
        /// Hiển thị chi tiết Timeline 360 độ + Ghi chú của khách hàng cụ thể
        /// GET: /CRM/CustomerTimeline/Details/1
        /// </summary>
        public async Task<IActionResult> Details(long id)
        {
            ViewBag.CustomerID = id;

            var timeline = await _timelineService.GetTimelineAsync(id);
            var notes = await _timelineService.GetNotesByCustomerAsync(id);

            ViewBag.Notes = notes;
            return View(timeline);
        }

        /// <summary>
        /// Thêm ghi chú mới cho khách hàng
        /// POST: /CRM/CustomerTimeline/AddNote
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddNote([FromBody] CustomerNoteDto model)
        {
            if (string.IsNullOrWhiteSpace(model.Content))
            {
                return BadRequest("Nội dung ghi chú không được để trống.");
            }

            // TODO: Khi có hệ thống Auth, lấy UserID từ User.Identity
            model.CreatedByUserID = 1;

            var noteId = await _timelineService.CreateNoteAsync(model);
            return Ok(new { NoteID = noteId, Message = "Thêm ghi chú thành công." });
        }
    }
}
