using System;

namespace PLPSOFT.ERP.SaaS.Modules.CRM.Application.DTOs
{
    /// <summary>
    /// DTO truyền dữ liệu Ghi chú khách hàng giữa View và Service
    /// </summary>
    public class CustomerNoteDto
    {
        public long NoteID { get; set; }
        public long CustomerID { get; set; }
        public string Content { get; set; } = string.Empty;
        public long CreatedByUserID { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
