using Microsoft.AspNetCore.Mvc;
using PLPSOFT.ERP.SaaS.Modules.CRM.Application.DTOs.Feedback;
using PLPSOFT.ERP.SaaS.Modules.CRM.Application.Interfaces;

namespace PLPSOFT.ERP.SaaS.Areas.CRM.Controllers
{
    [Area("CRM")]
    public class FeedbacksController : Controller
    {
        private readonly IFeedbackService _feedbackService;

        // Tạm thời dùng cố định (sau này lấy từ session/user đăng nhập)
        private const long CurrentCompanyID = 1;
        private const long CurrentBranchID = 1;

        public FeedbacksController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
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
                FeedbackID = detail.FeedbackID,
                InvoiceID  = detail.InvoiceID,
                FeedbackTypeID = detail.FeedbackTypeID, // Wait, I need to check if FeedbackDetailDto has IDs
                PriorityID = detail.PriorityID,
                StatusID   = detail.StatusID,
                Rating     = detail.Rating,
                Title      = detail.Title,
                Content    = detail.Content
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

    }
}