using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
// using Microsoft.EntityFrameworkCore;
using PLPSOFT.ERP.SaaS.Modules.CRM.Application.DTOs;
using PLPSOFT.ERP.SaaS.Modules.CRM.Application.Interfaces;
using PLPSOFT.ERP.SaaS.Modules.CRM.Domain.Entities;
// using PLPSOFT.ERP.SaaS.Infrastructure.Persistence; // Thay bằng Context thật

namespace PLPSOFT.ERP.SaaS.Modules.CRM.Application.Services
{
    public class CustomerTimelineService : ICustomerTimelineService
    {
        // private readonly CrmDbContext _context;

        // public CustomerTimelineService(CrmDbContext context)
        // {
        //     _context = context;
        // }

        public async Task<long> CreateNoteAsync(CustomerNoteDto dto)
        {
            // Code thực tế:
            // var note = new CustomerNote
            // {
            //     CustomerID = dto.CustomerID,
            //     Content = dto.Content,
            //     CreatedByUserID = dto.CreatedByUserID,
            //     CreatedAt = DateTime.Now
            // };
            // _context.CustomerNotes.Add(note);
            // await _context.SaveChangesAsync();
            // return note.NoteID;
            
            return await Task.FromResult(1L); // Mock data cho việc test
        }

        public async Task<List<CustomerNoteDto>> GetNotesAsync(long customerId)
        {
            // Code thực tế:
            // return await _context.CustomerNotes
            //     .Where(x => x.CustomerID == customerId)
            //     .OrderByDescending(x => x.CreatedAt)
            //     .Select(x => new CustomerNoteDto { ... })
            //     .ToListAsync();
            
            return await Task.FromResult(new List<CustomerNoteDto>());
        }

        public async Task<List<TimelineItemDto>> GetTimelineAsync(long customerId)
        {
            var timeline = new List<TimelineItemDto>();

            // LƯU Ý KHI RÁP VÀO MODULE CHÍNH: Bỏ comment các đoạn dưới đây và sử dụng DbContext

            /*
            // 1. Lấy Hóa đơn (Invoices)
            var invoices = await _context.SalesInvoices.Where(i => i.CustomerID == customerId).ToListAsync();
            timeline.AddRange(invoices.Select(i => new TimelineItemDto {
                EventTime = DateTime.Now, // Thực tế là i.CreatedDate
                EventType = "INVOICE",
                EventBadge = "[Hóa đơn]",
                Title = "Khách hàng mua hàng",
                Description = $"Mã hóa đơn: {i.InvoiceCode} - Tổng tiền: {i.TotalAmount:N2}",
                CssClass = "border-left-success"
            }));

            // 2. Phản hồi (Feedbacks)
            var feedbacks = await _context.CustomerFeedbacks.Where(f => f.CustomerID == customerId).ToListAsync();
            timeline.AddRange(feedbacks.Select(f => new TimelineItemDto {
                EventTime = f.CreatedAt,
                EventType = "FEEDBACK",
                EventBadge = "[Phản hồi]",
                Title = "Khách khiếu nại sản phẩm lỗi",
                Description = f.Content,
                CssClass = "border-left-danger"
            }));

            // 3. Lịch hẹn (Schedules)
            var schedules = await _context.CustomerSchedules.Where(s => s.CustomerID == customerId).ToListAsync();
            timeline.AddRange(schedules.Select(s => new TimelineItemDto {
                EventTime = s.StartTime,
                EventType = "SCHEDULE",
                EventBadge = "[Lịch hẹn]",
                Title = "Nhân viên gọi chăm sóc",
                Description = $"Thời gian: {s.StartTime:dd/MM/yyyy HH:mm}",
                CssClass = "border-left-primary"
            }));

            // 4. Ghi chú (Notes)
            var notes = await _context.CustomerNotes.Where(n => n.CustomerID == customerId).ToListAsync();
            timeline.AddRange(notes.Select(n => new TimelineItemDto {
                EventTime = n.CreatedAt,
                EventType = "NOTE",
                EventBadge = "[Ghi chú]",
                Title = "Khách thích được liên hệ buổi sáng",
                Description = n.Content,
                CssClass = "border-left-warning"
            }));
            */

            // Sắp xếp MỚI NHẤT lên đầu
            return timeline.OrderByDescending(t => t.EventTime).ToList();
        }
    }
}
