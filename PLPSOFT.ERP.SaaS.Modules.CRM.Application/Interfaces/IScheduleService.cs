using PLPSOFT.ERP.SaaS.Modules.CRM.Application.DTOs;

namespace PLPSOFT.ERP.SaaS.Modules.CRM.Application.Interfaces
{
    public interface IScheduleService
    {
        /// <summary>
        /// Lấy danh sách tất cả lịch chăm sóc (chưa bị xóa)
        /// </summary>
        Task<List<ScheduleListViewModel>> GetAllAsync(ScheduleFilterDto? filter = null);

        /// <summary>
        /// Lấy chi tiết lịch chăm sóc theo ID
        /// </summary>
        Task<ScheduleDetailViewModel?> GetByIdAsync(long id);

        /// <summary>
        /// Tạo mới lịch chăm sóc
        /// </summary>
        Task<long> CreateAsync(CreateScheduleDto dto);

        /// <summary>
        /// Cập nhật lịch chăm sóc
        /// </summary>
        Task<bool> UpdateAsync(UpdateScheduleDto dto);

        /// <summary>
        /// Cập nhật trạng thái sang COMPLETED
        /// </summary>
        Task<bool> CompleteAsync(long id);

        /// <summary>
        /// Cập nhật trạng thái sang CANCELED
        /// </summary>
        Task<bool> CancelAsync(long id);

        /// <summary>
        /// Xóa mềm lịch chăm sóc
        /// </summary>
        Task<bool> DeleteAsync(long id);

        /// <summary>
        /// Lấy danh sách lịch quá hạn (StartTime đã qua nhưng chưa completed)
        /// </summary>
        Task<List<ScheduleListViewModel>> GetOverdueAsync();

        /// <summary>
        /// Lấy danh sách lịch sắp tới (trong ngày hoặc trong tuần)
        /// </summary>
        Task<List<ScheduleListViewModel>> GetUpcomingAsync(bool thisWeek = false);

        /// <summary>
        /// Lấy danh sách lịch sắp đến trong 30 phút (logic nhắc hẹn)
        /// </summary>
        Task<List<ScheduleListViewModel>> GetRemindersAsync();

        /// <summary>
        /// Lấy dữ liệu dropdown cho form tạo/sửa
        /// </summary>
        Task<ScheduleFormViewModel> GetFormDataAsync(long companyId);
    }
}
