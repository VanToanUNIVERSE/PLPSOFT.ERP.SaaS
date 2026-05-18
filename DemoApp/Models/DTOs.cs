using System;

namespace DemoApp.Models
{
    public class TimelineItemDto
    {
        public DateTime EventTime { get; set; }
        public string EventType { get; set; } 
        public string EventBadge { get; set; } 
        public string Title { get; set; }
        public string Description { get; set; }
        public string CssClass { get; set; }
        public string IconClass { get; set; }
        public string StatusBadge { get; set; }
    }

    public class CustomerNoteDto
    {
        public long NoteID { get; set; }
        public long CustomerID { get; set; }
        public string Content { get; set; }
        public long CreatedByUserID { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CustomerSummaryDto
    {
        public long CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int TotalNotes { get; set; }
        public int TotalFeedbacks { get; set; }
        public int TotalSchedules { get; set; }
        public int TotalInvoices { get; set; }
    }
}
