using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PLPSOFT.ERP.SaaS.Modules.CRM.Application.DTOs;
using PLPSOFT.ERP.SaaS.Modules.CRM.Application.Interfaces;
using PLPSOFT.ERP.SaaS.Modules.CRM.Domain.Entities;
using PLPSOFT.ERP.SaaS.Modules.CRM.Infrastructure.Persistence;

namespace PLPSOFT.ERP.SaaS.Modules.CRM.Infrastructure.Services
{
    /// <summary>
    /// Service xử lý nghiệp vụ Customer Timeline & Notes
    /// Gom dữ liệu từ: CustomerNotes, CustomerFeedbacks, CustomerSchedules, SalesInvoices
    /// Sắp xếp mới nhất lên đầu
    /// </summary>
    public class CustomerTimelineService : ICustomerTimelineService
    {
        private readonly CrmDbContext _context;

        public CustomerTimelineService(CrmDbContext context)
        {
            _context = context;
        }

        /// <summary>Tạo ghi chú mới</summary>
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

        /// <summary>Lấy danh sách ghi chú theo khách hàng</summary>
        public async Task<List<CustomerNoteDto>> GetNotesByCustomerAsync(long customerId)
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

        /// <summary>
        /// Lấy toàn bộ Timeline 360 độ khách hàng
        /// Gom: [Hóa đơn] + [Phản hồi] + [Lịch hẹn] + [Ghi chú]
        /// Sắp xếp: Mới nhất nằm trên cùng
        /// </summary>
        public async Task<List<TimelineItemDto>> GetTimelineAsync(long customerId)
        {
            var timeline = new List<TimelineItemDto>();

            // 1. [Hóa đơn] - Từ bảng dbo.SalesInvoices
            var invoices = await _context.SalesInvoices
                .Where(i => i.CustomerID == customerId)
                .ToListAsync();
            timeline.AddRange(invoices.Select(i => new TimelineItemDto
            {
                EventTime = DateTime.Now,
                EventType = "INVOICE",
                EventBadge = "[Hóa đơn]",
                Title = "Khách hàng mua hàng",
                Description = $"Mã HĐ: {i.InvoiceCode} – Tổng tiền: {i.TotalAmount:N0} VNĐ",
                CssClass = "timeline-invoice"
            }));

            // 2. [Phản hồi] - Từ bảng crm.CustomerFeedbacks
            var feedbacks = await _context.CustomerFeedbacks
                .Where(f => f.CustomerID == customerId)
                .ToListAsync();
            timeline.AddRange(feedbacks.Select(f => new TimelineItemDto
            {
                EventTime = f.CreatedAt,
                EventType = "FEEDBACK",
                EventBadge = "[Phản hồi]",
                Title = f.Title,
                Description = f.Content,
                CssClass = "timeline-feedback"
            }));

            // 3. [Lịch hẹn] - Từ bảng crm.CustomerSchedules
            var schedules = await _context.CustomerSchedules
                .Where(s => s.CustomerID == customerId)
                .ToListAsync();
            timeline.AddRange(schedules.Select(s => new TimelineItemDto
            {
                EventTime = s.StartTime,
                EventType = "SCHEDULE",
                EventBadge = "[Lịch hẹn]",
                Title = s.Title,
                Description = $"Thời gian: {s.StartTime:dd/MM/yyyy HH:mm} – {s.EndTime:HH:mm}",
                CssClass = "timeline-schedule"
            }));

            // 4. [Ghi chú] - Từ bảng crm.CustomerNotes
            var notes = await _context.CustomerNotes
                .Where(n => n.CustomerID == customerId)
                .ToListAsync();
            timeline.AddRange(notes.Select(n => new TimelineItemDto
            {
                EventTime = n.CreatedAt,
                EventType = "NOTE",
                EventBadge = "[Ghi chú]",
                Title = "Ghi chú nội bộ",
                Description = n.Content,
                CssClass = "timeline-note"
            }));

            // Sắp xếp MỚI NHẤT lên đầu
            return timeline.OrderByDescending(t => t.EventTime).ToList();
        }
    }
}
