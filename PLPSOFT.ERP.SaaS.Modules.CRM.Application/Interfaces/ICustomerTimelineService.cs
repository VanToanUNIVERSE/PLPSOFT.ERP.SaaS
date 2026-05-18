using System.Collections.Generic;
using System.Threading.Tasks;
using PLPSOFT.ERP.SaaS.Modules.CRM.Application.DTOs;

namespace PLPSOFT.ERP.SaaS.Modules.CRM.Application.Interfaces
{
    public interface ICustomerTimelineService
    {
        Task<long> CreateNoteAsync(CustomerNoteDto dto);
        Task<List<CustomerNoteDto>> GetNotesAsync(long customerId);
        Task<List<TimelineItemDto>> GetTimelineAsync(long customerId);
    }
}
