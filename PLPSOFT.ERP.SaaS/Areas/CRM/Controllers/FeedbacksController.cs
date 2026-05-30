using Microsoft.AspNetCore.Mvc;
using PLPSOFT.ERP.SaaS.Modules.CRM.Application.DTOs.Feedback;
using PLPSOFT.ERP.SaaS.Modules.CRM.Application.Interfaces;

namespace PLPSOFT.ERP.SaaS.Areas.CRM.Controllers
{
    [Area("CRM")]
    public class FeedbacksController : Controller
    {
        private readonly IFeedbackService _feedbackService;

        // =====================================================================
        // GIÁ TRỊ TẠM — Chưa có module đăng nhập & phân quyền
        // TODO: Khi tích hợp Authentication, thay các hằng số này bằng:
        //   CurrentUserID   → long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier))
        //   CurrentUserName → User.FindFirstValue(ClaimTypes.Name)
        //   CurrentCompanyID / CurrentBranchID → lấy từ Claims hoặc session
        // =====================================================================
        private const long   CurrentUserID    = 3;      // crm02 — Nhân viên quản lý chi nhánh
        private const string CurrentUserName  = "Nhân viên quản lý chi nhánh";
        private const long   CurrentCompanyID = 1;
        private const long   CurrentBranchID  = 1;

        public FeedbacksController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        /// <summary>
        /// Inject thông tin người dùng vào ViewBag cho mọi action.
        /// TODO: Khi có auth thật, lấy từ HttpContext.User thay vì hằng số.
        /// </summary>
        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            ViewBag.CurrentUserName = CurrentUserName;
            base.OnActionExecuting(context);
        }

        // =====================================================
        // DANH SÁCH — GET: /CRM/Feedbacks
        // =====================================================
        public async Task<IActionResult> Index(string? keyword, string? statusCode, string? priorityCode)
        {
            var list = await _feedbackService.GetListAsync(CurrentCompanyID, CurrentBranchID, keyword, statusCode, priorityCode);
            ViewBag.Keyword = keyword;
            ViewBag.StatusCode = statusCode;
            ViewBag.PriorityCode = priorityCode;
            return View(list);
        }

        // =====================================================
        // CHI TIẾT — GET: /CRM/Feedbacks/Details/5
        // =====================================================
        public async Task<IActionResult> Details(long id)
        {
            var detail = await _feedbackService.GetByIdAsync(id);
            if (detail == null) return NotFound();
            return View(detail);
        }

        // =====================================================
        // TẠO MỚI — GET: /CRM/Feedbacks/Create
        // =====================================================
        public async Task<IActionResult> Create()
        {
            ViewBag.FormData = await _feedbackService.GetFormDataAsync(CurrentCompanyID);
            return View(new CreateFeedbackDto
            {
                CompanyID = CurrentCompanyID,
                BranchID  = CurrentBranchID
            });
        }

        // POST: /CRM/Feedbacks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateFeedbackDto dto)
        {
            // Gán trước để tránh ModelState lỗi CompanyID/BranchID = 0
            dto.CompanyID = CurrentCompanyID;
            dto.BranchID  = CurrentBranchID;
            ModelState.Remove(nameof(dto.CompanyID));
            ModelState.Remove(nameof(dto.BranchID));

            if (!ModelState.IsValid)
            {
                ViewBag.FormData = await _feedbackService.GetFormDataAsync(CurrentCompanyID);
                return View(dto);
            }

            try
            {
                var newId = await _feedbackService.CreateAsync(dto);
                TempData["Success"] = "Tạo phản hồi thành công!";
                return RedirectToAction(nameof(Details), new { id = newId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi tạo phản hồi: " + ex.Message);
                ViewBag.FormData = await _feedbackService.GetFormDataAsync(CurrentCompanyID);
                return View(dto);
            }
        }

        // =====================================================
        // CHỈNH SỬA — GET: /CRM/Feedbacks/Edit/5
        // =====================================================
        public async Task<IActionResult> Edit(long id)
        {
            var detail = await _feedbackService.GetByIdAsync(id);
            if (detail == null) return NotFound();

            ViewBag.FormData = await _feedbackService.GetFormDataAsync(CurrentCompanyID);
            ViewBag.Detail = detail;

            return View(new UpdateFeedbackDto
            {
                FeedbackID     = detail.FeedbackID,
                InvoiceID      = detail.InvoiceID,
                FeedbackTypeID = detail.FeedbackTypeID,
                PriorityID     = detail.PriorityID,
                StatusID       = detail.StatusID,
                Rating         = detail.Rating,
                Title          = detail.Title,
                Content        = detail.Content
            });
        }

        // POST: /CRM/Feedbacks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, UpdateFeedbackDto dto)
        {
            dto.FeedbackID = id;
            if (!ModelState.IsValid)
            {
                ViewBag.FormData = await _feedbackService.GetFormDataAsync(CurrentCompanyID);
                ViewBag.Detail   = await _feedbackService.GetByIdAsync(id);
                return View(dto);
            }

            try
            {
                await _feedbackService.UpdateAsync(dto);
                TempData["Success"] = "Cập nhật phản hồi thành công!";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.FormData = await _feedbackService.GetFormDataAsync(CurrentCompanyID);
                ViewBag.Detail   = await _feedbackService.GetByIdAsync(id);
                return View(dto);
            }
        }

        // =====================================================
        // GÁN NHÂN VIÊN — GET: /CRM/Feedbacks/Assign/5
        // =====================================================
        public async Task<IActionResult> Assign(long id)
        {
            var detail = await _feedbackService.GetByIdAsync(id);
            if (detail == null) return NotFound();

            ViewBag.FormData = await _feedbackService.GetFormDataAsync(CurrentCompanyID);
            ViewBag.Detail   = detail;
            return View(new AssignFeedbackDto { FeedbackID = id });
        }

        // POST: /CRM/Feedbacks/Assign/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(long id, AssignFeedbackDto dto)
        {
            dto.FeedbackID = id;
            if (!ModelState.IsValid)
            {
                ViewBag.FormData = await _feedbackService.GetFormDataAsync(CurrentCompanyID);
                return View(dto);
            }
            await _feedbackService.AssignAsync(dto);
            TempData["Success"] = "Gán nhân viên xử lý thành công!";
            return RedirectToAction(nameof(Details), new { id });
        }

        // =====================================================
        // XỬ LÝ / ĐÓNG — GET: /CRM/Feedbacks/Resolve/5
        // =====================================================
        public async Task<IActionResult> Resolve(long id)
        {
            var detail = await _feedbackService.GetByIdAsync(id);
            if (detail == null) return NotFound();

            ViewBag.FormData = await _feedbackService.GetFormDataAsync(CurrentCompanyID);
            ViewBag.Detail   = detail;
            return View(new ResolveFeedbackDto { FeedbackID = id });
        }

        // POST: /CRM/Feedbacks/Resolve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Resolve(long id, ResolveFeedbackDto dto)
        {
            dto.FeedbackID = id;
            if (!ModelState.IsValid)
            {
                ViewBag.FormData = await _feedbackService.GetFormDataAsync(CurrentCompanyID);
                ViewBag.Detail   = await _feedbackService.GetByIdAsync(id);
                return View(dto);
            }

            try
            {
                await _feedbackService.ResolveAsync(dto);
                TempData["Success"] = "Xử lý phản hồi thành công!";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.FormData = await _feedbackService.GetFormDataAsync(CurrentCompanyID);
                ViewBag.Detail   = await _feedbackService.GetByIdAsync(id);
                return View(dto);
            }
        }

        // =====================================================
        // XÓA MỀM — POST: /CRM/Feedbacks/Delete/5
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            await _feedbackService.DeleteAsync(id);
            TempData["Success"] = "Đã xóa phản hồi thành công!";
            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // API: THÔNG BÁO KHIẾU NẠI — GET: /CRM/Feedbacks/ComplaintAlerts
        // Trả về JSON để reminders.js polling và hiển thị lên chuông
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> ComplaintAlerts()
        {
            var alerts = await _feedbackService.GetComplaintAlertsAsync(CurrentBranchID);
            return Json(alerts);
        }

        // =====================================================
        // API: DISMISS ALERT — POST: /CRM/Feedbacks/DismissAlert?alertId=xxx
        // Xóa thông báo khỏi cache sau khi người dùng click vào
        // =====================================================
        [HttpPost]
        public async Task<IActionResult> DismissAlert(string alertId)
        {
            await _feedbackService.DismissComplaintAlertAsync(CurrentBranchID, alertId);
            return Ok();
        }
    }
}