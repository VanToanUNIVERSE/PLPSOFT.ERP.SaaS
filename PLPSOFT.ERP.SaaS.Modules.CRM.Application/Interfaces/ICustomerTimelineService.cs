using System.Collections.Generic;
using System.Threading.Tasks;
using PLPSOFT.ERP.SaaS.Modules.CRM.Application.DTOs;

namespace PLPSOFT.ERP.SaaS.Modules.CRM.Application.Interfaces
{
    /// <summary>
    /// Interface cho dịch vụ Customer Timeline & Notes
    /// </summary>
    public interface ICustomerTimelineService
    {
        /// <summary>Tạo ghi chú mới cho khách hàng</summary>
        Task<long> CreateNoteAsync(CustomerNoteDto dto);

        /// <summary>Lấy danh sách ghi chú theo CustomerID (mới nhất lên trước)</summary>
        Task<List<CustomerNoteDto>> GetNotesByCustomerAsync(long customerId);

        /// <summary>Lấy toàn bộ Timeline (Hóa đơn + Phản hồi + Lịch hẹn + Ghi chú) theo CustomerID</summary>
        Task<List<TimelineItemDto>> GetTimelineAsync(long customerId);

        /// <summary>Tìm kiếm danh sách khách hàng theo tên và sắp xếp mới nhất lên đầu</summary>
        Task<List<PLPSOFT.ERP.SaaS.Modules.CRM.Domain.Entities.Customer>> SearchCustomersAsync(string searchName);
    }
}
