using System;

namespace PLPSOFT.ERP.SaaS.Modules.CRM.Application.DTOs
{
    public class CustomerNoteDto
    {
        public long NoteID { get; set; }
        public long CustomerID { get; set; }
        public string Content { get; set; }
        public long CreatedByUserID { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
