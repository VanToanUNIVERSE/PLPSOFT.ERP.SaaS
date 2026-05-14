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
    }
}

