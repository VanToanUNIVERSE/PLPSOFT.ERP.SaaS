using Microsoft.EntityFrameworkCore;
using PLPSOFT.ERP.SaaS.Modules.CRM.Application.DTOs.Feedback;
using PLPSOFT.ERP.SaaS.Modules.CRM.Application.Interfaces;
using PLPSOFT.ERP.SaaS.Modules.CRM.Domain.Entities;
using PLPSOFT.ERP.SaaS.Modules.CRM.Infrastructure.Persistence;

namespace PLPSOFT.ERP.SaaS.Modules.CRM.Infrastructure.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly CrmDbContext _context;

        public FeedbackService(CrmDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // DANH SÁCH
        // =====================================================
        public async Task<List<FeedbackListItemDto>> GetListAsync(long companyId, long branchId)
        {
            return await _context.CustomerFeedbacks
                .Where(f => f.CompanyID == companyId
                         && f.BranchID == branchId
                         && f.IsDeleted == false)
                .Select(f => new FeedbackListItemDto
                {
                    FeedbackID       = f.FeedbackID,
                    CustomerName     = f.Customer != null ? f.Customer.CustomerName : "",
                    Title            = f.Title,
                    FeedbackTypeName = f.FeedbackType != null ? f.FeedbackType.ValueName : "",
                    PriorityName     = f.Priority != null ? f.Priority.ValueName : "",
                    StatusName       = f.Status != null ? f.Status.ValueName : "",
                    Rating           = f.Rating,
                    AssignedToUserName = f.AssignedToUser != null ? f.AssignedToUser.FullName : null,
                    CreatedAt        = f.CreatedAt,
                    ResolvedAt       = f.ResolvedAt
                })
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }

        // =====================================================
        // CHI TIẾT
        // =====================================================
        public async Task<FeedbackDetailDto?> GetByIdAsync(long feedbackId)
        {
            return await _context.CustomerFeedbacks
                .Where(f => f.FeedbackID == feedbackId && f.IsDeleted == false)
                .Select(f => new FeedbackDetailDto
                {
                    FeedbackID         = f.FeedbackID,
                    CompanyID          = f.CompanyID,
                    CompanyName        = f.Company != null ? f.Company.CompanyName : "",
                    BranchID           = f.BranchID,
                    BranchName         = f.Branch != null ? f.Branch.BranchName : "",
                    CustomerID         = f.CustomerID,
                    CustomerName       = f.Customer != null ? f.Customer.CustomerName : "",
                    CustomerPhone      = f.Customer != null ? f.Customer.Phone : null,
                    CustomerEmail      = f.Customer != null ? f.Customer.Email : null,
                    InvoiceID          = f.InvoiceID,
                    InvoiceCode        = f.Invoice != null ? f.Invoice.InvoiceCode : null,
                    FeedbackTypeName   = f.FeedbackType != null ? f.FeedbackType.ValueName : "",
                    PriorityName       = f.Priority != null ? f.Priority.ValueName : "",
                    StatusName         = f.Status != null ? f.Status.ValueName : "",
                    Rating             = f.Rating,
                    Title              = f.Title,
                    Content            = f.Content,
                    AssignedToUserID   = f.AssignedToUserID,
                    AssignedToUserName = f.AssignedToUser != null ? f.AssignedToUser.FullName : null,
                    Resolution         = f.Resolution,
                    CreatedAt          = f.CreatedAt,
                    ResolvedAt         = f.ResolvedAt
                })
                .FirstOrDefaultAsync();
        }

        // =====================================================
        // TẠO MỚI
        // =====================================================
        public async Task<long> CreateAsync(CreateFeedbackDto dto)
        {
            var feedback = new CustomerFeedback
            {
                CompanyID      = dto.CompanyID,
                BranchID       = dto.BranchID,
                CustomerID     = dto.CustomerID,
                InvoiceID      = dto.InvoiceID,
                FeedbackTypeID = dto.FeedbackTypeID,
                PriorityID     = dto.PriorityID,
                StatusID       = dto.StatusID,
                Rating         = dto.Rating,
                Title          = dto.Title,
                Content        = dto.Content,
                AssignedToUserID = dto.AssignedToUserID,
                CreatedAt      = DateTime.Now,
                IsDeleted      = false
            };

            _context.CustomerFeedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            return feedback.FeedbackID;
        }

        // =====================================================
        // CHỈNH SỬA
        // =====================================================
        public async Task UpdateAsync(UpdateFeedbackDto dto)
        {
            var feedback = await _context.CustomerFeedbacks
                .FirstOrDefaultAsync(f => f.FeedbackID == dto.FeedbackID && f.IsDeleted == false);

            if (feedback == null)
                throw new Exception($"Không tìm thấy phản hồi ID = {dto.FeedbackID}");

            feedback.FeedbackTypeID = dto.FeedbackTypeID;
            feedback.PriorityID     = dto.PriorityID;
            feedback.StatusID       = dto.StatusID;
            feedback.Rating         = dto.Rating;
            feedback.Title          = dto.Title;
            feedback.Content        = dto.Content;

            await _context.SaveChangesAsync();
        }

        // =====================================================
        // GÁN NHÂN VIÊN
        // =====================================================
        public async Task AssignAsync(AssignFeedbackDto dto)
        {
            var feedback = await _context.CustomerFeedbacks
                .FirstOrDefaultAsync(f => f.FeedbackID == dto.FeedbackID && f.IsDeleted == false);

            if (feedback == null)
                throw new Exception($"Không tìm thấy phản hồi ID = {dto.FeedbackID}");

            feedback.AssignedToUserID = dto.AssignedToUserID;

            await _context.SaveChangesAsync();
        }

        // =====================================================
        // XỬ LÝ / ĐÓNG PHẢN HỒI
        // =====================================================
        public async Task ResolveAsync(ResolveFeedbackDto dto)
        {
            var feedback = await _context.CustomerFeedbacks
                .FirstOrDefaultAsync(f => f.FeedbackID == dto.FeedbackID && f.IsDeleted == false);

            if (feedback == null)
                throw new Exception($"Không tìm thấy phản hồi ID = {dto.FeedbackID}");

            feedback.Resolution  = dto.Resolution;
            feedback.StatusID    = dto.StatusID;
            feedback.ResolvedAt  = DateTime.Now;

            await _context.SaveChangesAsync();
        }

        // =====================================================
        // XÓA MỀM
        // =====================================================
        public async Task DeleteAsync(long feedbackId)
        {
            var feedback = await _context.CustomerFeedbacks
                .FirstOrDefaultAsync(f => f.FeedbackID == feedbackId && f.IsDeleted == false);

            if (feedback == null)
                throw new Exception($"Không tìm thấy phản hồi ID = {feedbackId}");

            feedback.IsDeleted = true;
            feedback.DeletedAt = DateTime.Now;

            await _context.SaveChangesAsync();
        }
    }
}
