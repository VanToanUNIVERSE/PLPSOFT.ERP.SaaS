using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using PLPSOFT.ERP.SaaS.Modules.CRM.Application.DTOs.Feedback;
using PLPSOFT.ERP.SaaS.Modules.CRM.Application.Interfaces;
using PLPSOFT.ERP.SaaS.Modules.CRM.Domain.Entities;
using PLPSOFT.ERP.SaaS.Modules.CRM.Infrastructure.Persistence;

namespace PLPSOFT.ERP.SaaS.Modules.CRM.Infrastructure.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly CrmDbContext  _context;
        private readonly IMemoryCache  _cache;

        // Cache key theo BranchID — mỗi chi nhánh có danh sách cảnh báo riêng
        private static string ComplaintCacheKey(long branchId) => $"complaint_alerts_branch_{branchId}";

        public FeedbackService(CrmDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache   = cache;
        }

        // =====================================================
        // DANH SÁCH
        // =====================================================
        public async Task<List<FeedbackListItemDto>> GetListAsync(long companyId, long branchId, string? keyword = null, string? statusCode = null, string? priorityCode = null)
        {
            var query = _context.CustomerFeedbacks
                .Include(f => f.Customer)
                .Include(f => f.FeedbackType)
                .Include(f => f.Priority)
                .Include(f => f.Status)
                .Include(f => f.AssignedToUser)
                .Where(f => f.CompanyID == companyId
                         && f.BranchID == branchId
                         && f.IsDeleted == false);

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(f => f.Title.Contains(keyword) || (f.Customer != null && f.Customer.CustomerName.Contains(keyword)));
            }

            if (!string.IsNullOrEmpty(statusCode))
            {
                query = query.Where(f => f.Status != null && f.Status.ValueCode == statusCode);
            }

            if (!string.IsNullOrEmpty(priorityCode))
            {
                query = query.Where(f => f.Priority != null && f.Priority.ValueCode == priorityCode);
            }

            return await query
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
                    FeedbackTypeID     = f.FeedbackTypeID,
                    FeedbackTypeName   = f.FeedbackType != null ? f.FeedbackType.ValueName : "",
                    PriorityID         = f.PriorityID,
                    PriorityName       = f.Priority != null ? f.Priority.ValueName : "",
                    StatusID           = f.StatusID,
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
            var status = await _context.SystemTypeValues.FindAsync(dto.StatusID);
            if (status != null && (status.ValueCode == "PROCESSING" || status.ValueCode == "RESOLVED" || status.ValueCode == "CLOSED"))
            {
                if (dto.AssignedToUserID == null)
                {
                    throw new InvalidOperationException($"Không thể chuyển trạng thái sang {status.ValueName.ToLower()} khi chưa gán nhân viên xử lý. Vui lòng gán nhân viên trước.");
                }
            }

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

            // =====================================================
            // TỰ ĐỘNG THÔNG BÁO CHUÔNG KHI LOẠI LÀ KHIẾU NẠI
            // Kiểm tra FeedbackType sau khi lưu DB thành công
            // =====================================================
            var feedbackType = await _context.SystemTypeValues.FindAsync(dto.FeedbackTypeID);
            if (feedbackType?.ValueCode == "COMPLAINT")
            {
                var customer = await _context.Customers.FindAsync(dto.CustomerID);
                var branch   = await _context.Branches.FindAsync(dto.BranchID);

                var alert = new ComplaintAlertDto
                {
                    AlertId      = Guid.NewGuid().ToString("N"),
                    FeedbackID   = feedback.FeedbackID,
                    CustomerName = customer?.CustomerName ?? "Khách hàng",
                    Title        = dto.Title,
                    BranchName   = branch?.BranchName ?? "",
                    CreatedAt    = feedback.CreatedAt
                };

                var cacheKey = ComplaintCacheKey(dto.BranchID);
                var alerts = _cache.GetOrCreate(cacheKey, entry =>
                {
                    entry.SlidingExpiration = TimeSpan.FromHours(8);
                    return new List<ComplaintAlertDto>();
                }) ?? new List<ComplaintAlertDto>();

                alerts.Insert(0, alert);

                _cache.Set(cacheKey, alerts, new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromHours(8)
                });
            }

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

            var status = await _context.SystemTypeValues.FindAsync(dto.StatusID);
            if (status != null && (status.ValueCode == "PROCESSING" || status.ValueCode == "RESOLVED" || status.ValueCode == "CLOSED"))
            {
                if (feedback.AssignedToUserID == null)
                {
                    throw new InvalidOperationException($"Không thể chuyển trạng thái sang {status.ValueName.ToLower()} khi chưa gán nhân viên xử lý. Vui lòng gán nhân viên trước.");
                }
            }

            feedback.InvoiceID      = dto.InvoiceID;
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

            var status = await _context.SystemTypeValues.FindAsync(dto.StatusID);
            if (status != null && (status.ValueCode == "PROCESSING" || status.ValueCode == "RESOLVED" || status.ValueCode == "CLOSED"))
            {
                if (feedback.AssignedToUserID == null)
                {
                    throw new InvalidOperationException("Không thể xử lý/đóng phản hồi này vì chưa gán nhân viên xử lý. Vui lòng gán nhân viên trước.");
                }
            }

            feedback.Resolution  = dto.Resolution;
            feedback.StatusID    = dto.StatusID;
            feedback.ResolvedAt  = DateTime.Now;

            await _context.SaveChangesAsync();

            // =====================================================
            // KHI ĐÃ XỬ LÝ / ĐÓNG → TỰ ĐỘNG TẮT THÔNG BÁO CHUÔNG
            // Xóa alert khỏi cache theo FeedbackID khi trạng thái
            // là RESOLVED hoặc CLOSED
            // =====================================================
            if (status != null && (status.ValueCode == "RESOLVED" || status.ValueCode == "CLOSED"))
            {
                DismissAlertByFeedbackId(feedback.BranchID, feedback.FeedbackID);
            }
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

            // Xóa alert chuông nếu feedback bị xóa
            DismissAlertByFeedbackId(feedback.BranchID, feedback.FeedbackID);
        }

        // =====================================================
        // DỮ LIỆU DROPDOWN CHO FORM
        // =====================================================
        public async Task<FeedbackFormDataDto> GetFormDataAsync(long companyId)
        {
            var customers = await _context.Customers
                .Where(c => c.CompanyID == companyId)
                .Select(c => new DropdownItem { Value = c.CustomerID, Text = c.CustomerName })
                .ToListAsync();

            var users = await _context.Users
                .Where(u => u.CompanyID == companyId)
                .Select(u => new DropdownItem { Value = u.UserID, Text = u.FullName })
                .ToListAsync();

            var feedbackTypes = await _context.SystemTypeValues
                .Where(v => v.Type!.TypeCode == "FEEDBACK_TYPE" && v.IsActive)
                .Select(v => new DropdownItem { Value = v.TypeValueID, Text = v.ValueName })
                .ToListAsync();

            var priorities = await _context.SystemTypeValues
                .Where(v => v.Type!.TypeCode == "PRIORITY" && v.IsActive)
                .Select(v => new DropdownItem { Value = v.TypeValueID, Text = v.ValueName })
                .ToListAsync();

            var statuses = await _context.SystemTypeValues
                .Where(v => v.Type!.TypeCode == "CRM_STATUS" && v.IsActive)
                .Select(v => new DropdownItem { Value = v.TypeValueID, Text = v.ValueName, Code = v.ValueCode })
                .ToListAsync();

            var invoices = await _context.SalesInvoices
                .Where(i => i.Customer != null && i.Customer.CompanyID == companyId)
                .Select(i => new DropdownItem { Value = i.InvoiceID, Text = i.InvoiceCode })
                .ToListAsync();

            return new FeedbackFormDataDto
            {
                Customers = customers,
                Users = users,
                FeedbackTypes = feedbackTypes,
                Priorities = priorities,
                Statuses = statuses,
                Invoices = invoices
            };
        }
        // =====================================================
        // DASHBOARD DATA
        // =====================================================
        public async Task<PLPSOFT.ERP.SaaS.Modules.CRM.Application.DTOs.Dashboard.CrmDashboardDto> GetDashboardDataAsync(long companyId, long branchId)
        {
            var query = _context.CustomerFeedbacks
                .Where(f => f.CompanyID == companyId && f.BranchID == branchId && !f.IsDeleted);

            // Giả sử Status Code cho "Mới" là 'NEW' hoặc ID tương ứng
            // Ở đây tôi lấy theo ValueCode từ bảng SystemTypeValue
            var totalNew = await query.CountAsync(f => f.Status != null && f.Status.ValueCode == "NEW");

            var recentFeedbacks = await query
                .OrderByDescending(f => f.CreatedAt)
                .Take(5)
                .Select(f => new FeedbackListItemDto
                {
                    FeedbackID       = f.FeedbackID,
                    CustomerName     = f.Customer != null ? f.Customer.CustomerName : "",
                    Title            = f.Title,
                    FeedbackTypeName = f.FeedbackType != null ? f.FeedbackType.ValueName : "",
                    PriorityName     = f.Priority != null ? f.Priority.ValueName : "",
                    StatusName       = f.Status != null ? f.Status.ValueName : "",
                    Rating           = f.Rating,
                    CreatedAt        = f.CreatedAt
                })
                .ToListAsync();

            return new PLPSOFT.ERP.SaaS.Modules.CRM.Application.DTOs.Dashboard.CrmDashboardDto
            {
                TotalNewFeedbacks = totalNew,
                TotalUpcomingSchedules = 0, // Chờ DB bảng Schedules
                TotalOverdueSchedules = 0,  // Chờ DB bảng Schedules
                RecentFeedbacks = recentFeedbacks
            };
        }

        // =====================================================
        // THÔNG BÁO KHIẾU NẠI — ĐỌC TỪ CACHE
        // =====================================================
        public Task<List<ComplaintAlertDto>> GetComplaintAlertsAsync(long branchId)
        {
            var cacheKey = ComplaintCacheKey(branchId);
            var alerts   = _cache.TryGetValue(cacheKey, out List<ComplaintAlertDto>? cached)
                           ? cached ?? new List<ComplaintAlertDto>()
                           : new List<ComplaintAlertDto>();

            // Tính TimeAgo cho mỗi thông báo
            var now = DateTime.Now;
            foreach (var a in alerts)
            {
                var diff = now - a.CreatedAt;
                a.TimeAgo = diff.TotalMinutes < 1  ? "vừa xong"
                          : diff.TotalMinutes < 60 ? $"{(int)diff.TotalMinutes} phút trước"
                          : diff.TotalHours   < 24 ? $"{(int)diff.TotalHours} giờ trước"
                                                    : $"{(int)diff.TotalDays} ngày trước";
            }

            return Task.FromResult(alerts);
        }

        public Task DismissComplaintAlertAsync(long branchId, string alertId)
        {
            var cacheKey = ComplaintCacheKey(branchId);
            if (_cache.TryGetValue(cacheKey, out List<ComplaintAlertDto>? alerts) && alerts != null)
            {
                alerts.RemoveAll(a => a.AlertId == alertId);
                _cache.Set(cacheKey, alerts, new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromHours(8)
                });
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Xóa alert khỏi cache theo FeedbackID (dùng nội bộ khi Resolve/Delete).
        /// Khác với DismissComplaintAlertAsync vốn dùng AlertId (GUID).
        /// </summary>
        private void DismissAlertByFeedbackId(long branchId, long feedbackId)
        {
            var cacheKey = ComplaintCacheKey(branchId);
            if (_cache.TryGetValue(cacheKey, out List<ComplaintAlertDto>? alerts) && alerts != null)
            {
                alerts.RemoveAll(a => a.FeedbackID == feedbackId);
                _cache.Set(cacheKey, alerts, new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromHours(8)
                });
            }
        }
    }
}
