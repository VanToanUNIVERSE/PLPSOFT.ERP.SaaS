using Microsoft.AspNetCore.Mvc;
using PLPSOFT.ERP.SaaS.Modules.CRM.Application.DTOs;
using PLPSOFT.ERP.SaaS.Modules.CRM.Application.Interfaces;

namespace PLPSOFT.ERP.SaaS.Areas.CRM.Controllers
{
    [Area("CRM")]
    public class SchedulesController : Controller
    {
        private readonly IScheduleService _scheduleService;

        // TODO: Lấy CompanyID và UserID từ session/claims thực tế
        private const long DefaultCompanyID = 1;
        private const long DefaultUserID = 1;

        public SchedulesController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        #region Danh sách lịch chăm sóc

        /// <summary>
        /// GET: /CRM/Schedules
        /// Hiển thị danh sách tất cả lịch chăm sóc, có hỗ trợ lọc
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index(
            long? assignedToUserID,
            long? customerID,
            long? scheduleTypeID,
            long? statusID,
            string? timeFilter)
        {
            var filter = new ScheduleFilterDto
            {
                AssignedToUserID = assignedToUserID,
                CustomerID = customerID,
                ScheduleTypeID = scheduleTypeID,
                StatusID = statusID,
                TimeFilter = timeFilter
            };

            ViewBag.StatusID = statusID;
            ViewBag.ScheduleTypeID = scheduleTypeID;
            ViewBag.TimeFilter = timeFilter;
            ViewBag.AssignedToUserID = assignedToUserID;

            var schedules = await _scheduleService.GetAllAsync(filter);

            // Lấy dữ liệu dropdown cho bộ lọc
            var formData = await _scheduleService.GetFormDataAsync(DefaultCompanyID);
            ViewBag.Users = formData.Users;
            ViewBag.Customers = formData.Customers;
            ViewBag.ScheduleTypes = formData.ScheduleTypes;
            ViewBag.Filter = filter;

            return View(schedules);
        }

        #endregion

        #region Chi tiết lịch chăm sóc

        /// <summary>
        /// GET: /CRM/Schedules/Details/5
        /// Xem chi tiết lịch chăm sóc
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Details(long id)
        {
            var schedule = await _scheduleService.GetByIdAsync(id);
            if (schedule == null)
                return NotFound();

            return View(schedule);
        }

        #endregion

        #region Tạo mới lịch chăm sóc

        /// <summary>
        /// GET: /CRM/Schedules/Create
        /// Hiển thị form tạo lịch mới
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var formData = await _scheduleService.GetFormDataAsync(DefaultCompanyID);
            formData.CreateDto = new CreateScheduleDto
            {
                CompanyID = DefaultCompanyID,
                CreatedByUserID = DefaultUserID
            };

            return View(formData);
        }

        /// <summary>
        /// POST: /CRM/Schedules/Create
        /// Xử lý tạo lịch mới
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind(Prefix = "CreateDto")] CreateScheduleDto dto)
        {
            if (dto.StartTime.HasValue && dto.EndTime.HasValue && dto.EndTime < dto.StartTime)
            {
                ModelState.AddModelError("CreateDto.EndTime", "Thời gian kết thúc phải sau thời gian bắt đầu.");
            }

            if (!ModelState.IsValid)
            {
                var formData = await _scheduleService.GetFormDataAsync(DefaultCompanyID);
                formData.CreateDto = dto;
                return View(formData);
            }

            try
            {
                dto.CompanyID = DefaultCompanyID;
                dto.CreatedByUserID = DefaultUserID;
                dto.BranchID = 1; // Default BranchID

                var id = await _scheduleService.CreateAsync(dto);
                TempData["SuccessMessage"] = "Tạo lịch chăm sóc thành công.";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                var formData = await _scheduleService.GetFormDataAsync(DefaultCompanyID);
                formData.CreateDto = dto;
                return View(formData);
            }
        }

        #endregion

        #region Cập nhật lịch chăm sóc

        /// <summary>
        /// GET: /CRM/Schedules/Edit/5
        /// Hiển thị form sửa lịch
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var schedule = await _scheduleService.GetByIdAsync(id);
            if (schedule == null)
                return NotFound();

            var formData = await _scheduleService.GetFormDataAsync(DefaultCompanyID);
            formData.UpdateDto = new UpdateScheduleDto
            {
                ScheduleID = schedule.ScheduleID,
                CustomerID = schedule.CustomerID,
                BranchID = schedule.BranchID,
                ScheduleTypeID = schedule.ScheduleTypeID,
                Title = schedule.Title,
                Description = schedule.Description,
                StartTime = schedule.StartTime,
                EndTime = schedule.EndTime,
                AssignedToUserID = schedule.AssignedToUserID
            };

            return View(formData);
        }

        /// <summary>
        /// POST: /CRM/Schedules/Edit/5
        /// Xử lý cập nhật lịch
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind(Prefix = "UpdateDto")] UpdateScheduleDto dto)
        {
            if (id != dto.ScheduleID)
                return BadRequest();

            if (dto.EndTime < dto.StartTime)
            {
                ModelState.AddModelError("EndTime", "Thời gian kết thúc phải sau thời gian bắt đầu.");
            }

            if (!ModelState.IsValid)
            {
                var formData = await _scheduleService.GetFormDataAsync(DefaultCompanyID);
                formData.UpdateDto = dto;
                return View(formData);
            }

            var result = await _scheduleService.UpdateAsync(dto);
            if (!result)
                return NotFound();

            TempData["SuccessMessage"] = "Cập nhật lịch chăm sóc thành công.";
            return RedirectToAction(nameof(Details), new { id });
        }

        #endregion

        #region Cập nhật trạng thái

        /// <summary>
        /// POST: /CRM/Schedules/Complete/5
        /// Chuyển trạng thái sang COMPLETED
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(long id)
        {
            var result = await _scheduleService.CompleteAsync(id);
            if (!result)
                return NotFound();

            TempData["SuccessMessage"] = "Đã hoàn thành lịch chăm sóc.";
            return RedirectToAction(nameof(Details), new { id });
        }

        /// <summary>
        /// POST: /CRM/Schedules/Cancel/5
        /// Chuyển trạng thái sang CANCELED
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(long id)
        {
            var result = await _scheduleService.CancelAsync(id);
            if (!result)
                return NotFound();

            TempData["SuccessMessage"] = "Đã hủy lịch chăm sóc.";
            return RedirectToAction(nameof(Details), new { id });
        }

        #endregion

        #region Xóa lịch chăm sóc

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            var success = await _scheduleService.DeleteAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Đã xóa lịch chăm sóc thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể xóa lịch chăm sóc.";
            }
            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Lọc lịch quá hạn

        /// <summary>
        /// GET: /CRM/Schedules/Overdue
        /// Hiển thị danh sách lịch quá hạn
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Overdue()
        {
            var schedules = await _scheduleService.GetOverdueAsync();
            return View("Index", schedules);
        }

        #endregion

        #region Lọc lịch sắp tới

        /// <summary>
        /// GET: /CRM/Schedules/Upcoming?thisWeek=false
        /// Hiển thị danh sách lịch sắp tới (trong ngày hoặc trong tuần)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Upcoming(bool thisWeek = false)
        {
            var schedules = await _scheduleService.GetUpcomingAsync(thisWeek);
            ViewBag.ThisWeek = thisWeek;
            return View("Index", schedules);
        }

        #endregion

        #region Nhắc hẹn (lịch sắp đến trong 30 phút)

        /// <summary>
        /// GET: /CRM/Schedules/Reminders
        /// API trả JSON danh sách lịch sắp đến trong 30 phút
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Reminders()
        {
            var reminders = await _scheduleService.GetRemindersAsync();
            return Json(reminders);
        }

        #endregion
    }
}