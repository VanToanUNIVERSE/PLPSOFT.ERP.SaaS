using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DemoApp.Models;
using DemoApp.Data;

namespace DemoApp.Services
{
    public interface ICustomerTimelineService
    {
        /// <summary>
        /// Tạo ghi chú mới cho khách hàng
        /// </summary>
        Task<long> CreateNoteAsync(CustomerNoteDto dto);

        /// <summary>
        /// Lấy danh sách ghi chú theo khách hàng
        /// </summary>
        Task<List<CustomerNoteDto>> GetNotesAsync(long customerId);

        /// <summary>
        /// Lấy timeline (lịch sử tương tác) của khách hàng
        /// Gồm: ghi chú, phản hồi/khiếu nại, lịch hẹn, hóa đơn
        /// </summary>
        Task<List<TimelineItemDto>> GetTimelineAsync(long customerId);

        /// <summary>
        /// Lấy thông tin tổng quan khách hàng
        /// </summary>
        Task<CustomerSummaryDto> GetCustomerSummaryAsync(long customerId);

        /// <summary>
        /// Lấy danh sách tất cả khách hàng
        /// </summary>
        Task<List<CustomerSummaryDto>> GetAllCustomersAsync();
    }

    public class CustomerTimelineService : ICustomerTimelineService
    {
        private readonly CrmDbContext _context;

        public CustomerTimelineService(CrmDbContext context)
        {
            _context = context;
        }

        public async Task<long> CreateNoteAsync(CustomerNoteDto dto)
        {
            var note = new CustomerNote
            {
                CustomerID = dto.CustomerID,
                Content = dto.Content,
                CreatedByUserID = dto.CreatedByUserID,
                CreatedAt = DateTime.Now
            };
            _context.CustomerNotes.Add(note);
            await _context.SaveChangesAsync();
            return note.NoteID;
        }

        public async Task<List<CustomerNoteDto>> GetNotesAsync(long customerId)
        {
            return await _context.CustomerNotes
                .Where(n => n.CustomerID == customerId)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new CustomerNoteDto
                {
                    NoteID = n.NoteID,
                    CustomerID = n.CustomerID,
                    Content = n.Content,
                    CreatedByUserID = n.CreatedByUserID,
                    CreatedAt = n.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<List<TimelineItemDto>> GetTimelineAsync(long customerId)
        {
            var timeline = new List<TimelineItemDto>();

            // === Hóa đơn bán hàng ===
            var invoices = await _context.SalesInvoices
                .Where(i => i.CustomerID == customerId)
                .ToListAsync();
            timeline.AddRange(invoices.Select(i => new TimelineItemDto
            {
                EventTime = i.InvoiceDate,
                EventType = "INVOICE",
                EventBadge = "Hóa đơn",
                Title = $"Hóa đơn #{i.InvoiceCode}",
                Description = $"Tổng tiền: {i.TotalAmount:N0} VNĐ",
                CssClass = "timeline-invoice",
                IconClass = "bi-receipt",
                StatusBadge = ""
            }));

            // === Phản hồi / Khiếu nại ===
            var feedbacks = await _context.CustomerFeedbacks
                .Where(f => f.CustomerID == customerId)
                .ToListAsync();
            timeline.AddRange(feedbacks.Select(f => new TimelineItemDto
            {
                EventTime = f.CreatedAt,
                EventType = "FEEDBACK",
                EventBadge = "Phản hồi",
                Title = f.Title,
                Description = f.Content,
                CssClass = "timeline-feedback",
                IconClass = "bi-chat-left-dots",
                StatusBadge = f.Status
            }));

            // === Lịch chăm sóc / Lịch hẹn ===
            var schedules = await _context.CustomerSchedules
                .Where(s => s.CustomerID == customerId)
                .ToListAsync();
            timeline.AddRange(schedules.Select(s => new TimelineItemDto
            {
                EventTime = s.StartTime,
                EventType = "SCHEDULE",
                EventBadge = "Lịch hẹn",
                Title = s.Title,
                Description = $"Từ {s.StartTime:dd/MM/yyyy HH:mm} đến {s.EndTime:dd/MM/yyyy HH:mm}",
                CssClass = "timeline-schedule",
                IconClass = "bi-calendar-event",
                StatusBadge = s.Status
            }));

            // === Ghi chú nội bộ ===
            var notes = await _context.CustomerNotes
                .Where(n => n.CustomerID == customerId)
                .ToListAsync();
            timeline.AddRange(notes.Select(n => new TimelineItemDto
            {
                EventTime = n.CreatedAt,
                EventType = "NOTE",
                EventBadge = "Ghi chú",
                Title = "Ghi chú nội bộ",
                Description = n.Content,
                CssClass = "timeline-note",
                IconClass = "bi-sticky",
                StatusBadge = ""
            }));

            return timeline.OrderByDescending(t => t.EventTime).ToList();
        }

        public async Task<CustomerSummaryDto> GetCustomerSummaryAsync(long customerId)
        {
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null) return null;

            return new CustomerSummaryDto
            {
                CustomerID = customer.CustomerID,
                CustomerName = customer.CustomerName,
                Phone = customer.Phone,
                Email = customer.Email,
                TotalNotes = await _context.CustomerNotes.CountAsync(n => n.CustomerID == customerId),
                TotalFeedbacks = await _context.CustomerFeedbacks.CountAsync(f => f.CustomerID == customerId),
                TotalSchedules = await _context.CustomerSchedules.CountAsync(s => s.CustomerID == customerId),
                TotalInvoices = await _context.SalesInvoices.CountAsync(i => i.CustomerID == customerId)
            };
        }

        public async Task<List<CustomerSummaryDto>> GetAllCustomersAsync()
        {
            var customers = await _context.Customers.ToListAsync();
            var result = new List<CustomerSummaryDto>();
            foreach (var c in customers)
            {
                result.Add(new CustomerSummaryDto
                {
                    CustomerID = c.CustomerID,
                    CustomerName = c.CustomerName,
                    Phone = c.Phone,
                    Email = c.Email,
                    TotalNotes = await _context.CustomerNotes.CountAsync(n => n.CustomerID == c.CustomerID),
                    TotalFeedbacks = await _context.CustomerFeedbacks.CountAsync(f => f.CustomerID == c.CustomerID),
                    TotalSchedules = await _context.CustomerSchedules.CountAsync(s => s.CustomerID == c.CustomerID),
                    TotalInvoices = await _context.SalesInvoices.CountAsync(i => i.CustomerID == c.CustomerID)
                });
            }
            return result;
        }
    }
}
