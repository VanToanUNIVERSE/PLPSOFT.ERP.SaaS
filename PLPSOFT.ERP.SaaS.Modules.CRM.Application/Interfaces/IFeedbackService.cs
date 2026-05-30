using PLPSOFT.ERP.SaaS.Modules.CRM.Application.DTOs.Feedback;

namespace PLPSOFT.ERP.SaaS.Modules.CRM.Application.Interfaces
{
    public interface IFeedbackService
    {
        /// <summary>Lấy danh sách phản hồi theo công ty/chi nhánh</summary>
        Task<List<FeedbackListItemDto>> GetListAsync(long companyId, long branchId, string? keyword = null, string? statusCode = null, string? priorityCode = null);

        /// <summary>Xem chi tiết 1 phản hồi</summary>
        Task<FeedbackDetailDto?> GetByIdAsync(long feedbackId);

        /// <summary>Tạo mới phản hồi</summary>
        Task<long> CreateAsync(CreateFeedbackDto dto);

        /// <summary>Chỉnh sửa phản hồi</summary>
        Task UpdateAsync(UpdateFeedbackDto dto);

        /// <summary>Gán nhân viên xử lý</summary>
        Task AssignAsync(AssignFeedbackDto dto);

        /// <summary>Xử lý / đóng phản hồi</summary>
        Task ResolveAsync(ResolveFeedbackDto dto);

        /// <summary>Xóa mềm phản hồi</summary>
        Task DeleteAsync(long feedbackId);

        /// <summary>Lấy dữ liệu dropdown cho form tạo mới / chỉnh sửa</summary>
        Task<FeedbackFormDataDto> GetFormDataAsync(long companyId);

        /// <summary>Lấy dữ liệu cho Dashboard CRM</summary>
        Task<PLPSOFT.ERP.SaaS.Modules.CRM.Application.DTOs.Dashboard.CrmDashboardDto> GetDashboardDataAsync(long companyId, long branchId);

        // =====================================================
        // THÔNG BÁO KHIẾU NẠI (Bell Notification — IMemoryCache)
        // =====================================================

        /// <summary>Lấy danh sách thông báo khiếu nại chưa đọc từ bộ nhớ tạm (IMemoryCache)</summary>
        Task<List<ComplaintAlertDto>> GetComplaintAlertsAsync(long branchId);

        /// <summary>Đánh dấu đã đọc / xóa thông báo khỏi bộ nhớ tạm theo AlertId</summary>
        Task DismissComplaintAlertAsync(long branchId, string alertId);
    }
}
