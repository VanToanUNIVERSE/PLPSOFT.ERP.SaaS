using Microsoft.EntityFrameworkCore;
using PLPSOFT.ERP.SaaS.Modules.CRM.Application.DTOs;
using PLPSOFT.ERP.SaaS.Modules.CRM.Application.Interfaces;
using PLPSOFT.ERP.SaaS.Modules.CRM.Domain.Entities;
using PLPSOFT.ERP.SaaS.Modules.CRM.Infrastructure.Persistence;

namespace PLPSOFT.ERP.SaaS.Modules.CRM.Infrastructure.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly CrmDbContext _context;

        public ScheduleService(CrmDbContext context)
        {
            _context = context;
        }

        #region Lấy danh sách lịch chăm sóc

        public async Task<List<ScheduleListViewModel>> GetAllAsync(ScheduleFilterDto? filter = null)
        {
            var query = BuildBaseQuery();

            // Áp dụng bộ lọc
            if (filter != null)
            {
                if (filter.AssignedToUserID.HasValue)
                    query = query.Where(s => s.AssignedToUserID == filter.AssignedToUserID.Value);

                if (filter.CustomerID.HasValue)
                    query = query.Where(s => s.CustomerID == filter.CustomerID.Value);

                if (filter.ScheduleTypeID.HasValue)
                    query = query.Where(s => s.ScheduleTypeID == filter.ScheduleTypeID.Value);

                if (filter.StatusID.HasValue)
                    query = query.Where(s => s.StatusID == filter.StatusID.Value);

                if (!string.IsNullOrEmpty(filter.TimeFilter))
                {
                    var now = DateTime.Now;
                    if (filter.TimeFilter == "overdue")
                    {
                        query = query.Where(s => s.EndTime < now 
                            && s.Status != null 
                            && s.Status.ValueCode != "COMPLETED" 
                            && s.Status.ValueCode != "CANCELED");
                    }
                    else if (filter.TimeFilter == "upcoming_day")
                    {
                        var endOfDay = now.Date.AddDays(1).AddSeconds(-1);
                        query = query.Where(s => s.EndTime >= now && s.StartTime <= endOfDay);
                    }
                    else if (filter.TimeFilter == "upcoming_week")
                    {
                        int daysUntilSunday = ((int)DayOfWeek.Sunday - (int)now.DayOfWeek + 7) % 7;
                        if (daysUntilSunday == 0) daysUntilSunday = 7;
                        var endOfWeek = now.Date.AddDays(daysUntilSunday).AddDays(1).AddSeconds(-1);
                        query = query.Where(s => s.EndTime >= now && s.StartTime <= endOfWeek);
                    }
                }
            }

            return await ProjectToListViewModel(query)
                .OrderByDescending(s => s.StartTime)
                .ToListAsync();
        }

        #endregion

        #region Chi tiết lịch chăm sóc

        public async Task<ScheduleDetailViewModel?> GetByIdAsync(long id)
        {
            var schedule = await BuildBaseQuery()
                .Where(s => s.ScheduleID == id)
                .Select(s => new ScheduleDetailViewModel
                {
                    ScheduleID = s.ScheduleID,
                    Title = s.Title,
                    Description = s.Description,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    CreatedAt = s.CreatedAt,

                    CustomerID = s.CustomerID,
                    CustomerName = s.Customer != null ? s.Customer.CustomerName : "",
                    CustomerCode = s.Customer != null ? s.Customer.CustomerCode : "",
                    CustomerPhone = s.Customer != null ? s.Customer.Phone : null,
                    CustomerEmail = s.Customer != null ? s.Customer.Email : null,

                    CompanyID = s.CompanyID,
                    CompanyName = s.Company != null ? s.Company.CompanyName : "",
                    BranchID = s.BranchID,
                    BranchName = s.Branch != null ? s.Branch.BranchName : "",

                    ScheduleTypeID = s.ScheduleTypeID,
                    ScheduleTypeName = s.ScheduleType != null ? s.ScheduleType.ValueName : "",
                    StatusID = s.StatusID,
                    StatusName = s.Status != null ? s.Status.ValueName : "",
                    StatusCode = s.Status != null ? s.Status.ValueCode : "",

                    CreatedByUserID = s.CreatedByUserID,
                    CreatedByUserName = s.CreatedByUser != null ? s.CreatedByUser.FullName : "",
                    AssignedToUserID = s.AssignedToUserID,
                    AssignedToUserName = s.AssignedToUser != null ? s.AssignedToUser.FullName : "",

                    IsOverdue = s.EndTime < DateTime.Now
                                && s.Status != null
                                && s.Status.ValueCode != "COMPLETED"
                                && s.Status.ValueCode != "CANCELED"
                })
                .FirstOrDefaultAsync();

            return schedule;
        }

        #endregion

        #region Tạo mới lịch chăm sóc

        public async Task<long> CreateAsync(CreateScheduleDto dto)
        {
            // Lấy StatusID mặc định: PLANNED
            var plannedStatus = await _context.SystemTypeValues
                .FirstOrDefaultAsync(v => v.ValueCode == "PLANNED"
                    && v.Type != null
                    && v.Type.TypeCode == "SCHEDULE_STATUS");

            if (plannedStatus == null)
                throw new InvalidOperationException("Không tìm thấy trạng thái PLANNED trong hệ thống.");

            var schedule = new CustomerSchedule
            {
                CompanyID = dto.CompanyID,
                BranchID = dto.BranchID,
                CustomerID = dto.CustomerID.GetValueOrDefault(),
                ScheduleTypeID = dto.ScheduleTypeID.GetValueOrDefault(),
                StatusID = plannedStatus.TypeValueID,
                Title = dto.Title,
                Description = dto.Description,
                StartTime = dto.StartTime.GetValueOrDefault(),
                EndTime = dto.EndTime.GetValueOrDefault(),
                CreatedByUserID = dto.CreatedByUserID,
                AssignedToUserID = dto.AssignedToUserID.GetValueOrDefault(),
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };

            _context.CustomerSchedules.Add(schedule);
            await _context.SaveChangesAsync();

            return schedule.ScheduleID;
        }

        #endregion

        #region Cập nhật lịch chăm sóc

        public async Task<bool> UpdateAsync(UpdateScheduleDto dto)
        {
            var schedule = await _context.CustomerSchedules
                .FirstOrDefaultAsync(s => s.ScheduleID == dto.ScheduleID);

            if (schedule == null) return false;

            schedule.CustomerID = dto.CustomerID.GetValueOrDefault();
            schedule.BranchID = dto.BranchID;
            schedule.ScheduleTypeID = dto.ScheduleTypeID.GetValueOrDefault();
            schedule.Title = dto.Title;
            schedule.Description = dto.Description;
            schedule.StartTime = dto.StartTime.GetValueOrDefault();
            schedule.EndTime = dto.EndTime.GetValueOrDefault();
            schedule.AssignedToUserID = dto.AssignedToUserID.GetValueOrDefault();

            await _context.SaveChangesAsync();
            return true;
        }

        #endregion

        #region Cập nhật trạng thái

        public async Task<bool> CompleteAsync(long id)
        {
            return await ChangeStatusAsync(id, "COMPLETED");
        }

        public async Task<bool> CancelAsync(long id)
        {
            return await ChangeStatusAsync(id, "CANCELED");
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var schedule = await _context.CustomerSchedules.FindAsync(id);
            if (schedule == null)
                return false;

            schedule.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<bool> ChangeStatusAsync(long scheduleId, string statusCode)
        {
            var schedule = await _context.CustomerSchedules
                .FirstOrDefaultAsync(s => s.ScheduleID == scheduleId);

            if (schedule == null) return false;

            var newStatus = await _context.SystemTypeValues
                .FirstOrDefaultAsync(v => v.ValueCode == statusCode
                    && v.Type != null
                    && v.Type.TypeCode == "SCHEDULE_STATUS");

            if (newStatus == null) return false;

            schedule.StatusID = newStatus.TypeValueID;
            await _context.SaveChangesAsync();
            return true;
        }

        #endregion

        #region Lọc lịch quá hạn

        public async Task<List<ScheduleListViewModel>> GetOverdueAsync()
        {
            var now = DateTime.Now;

            var query = BuildBaseQuery()
                .Where(s => s.EndTime < now
                    && s.Status != null
                    && s.Status.ValueCode != "COMPLETED"
                    && s.Status.ValueCode != "CANCELED");

            return await ProjectToListViewModel(query)
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }

        #endregion

        #region Lọc lịch sắp tới

        public async Task<List<ScheduleListViewModel>> GetUpcomingAsync(bool? thisWeek = null)
        {
            var now = DateTime.Now;
            DateTime? endRange = null;

            if (thisWeek.HasValue)
            {
                if (thisWeek.Value)
                {
                    // Tính ngày cuối tuần (Chủ nhật)
                    int daysUntilSunday = ((int)DayOfWeek.Sunday - (int)now.DayOfWeek + 7) % 7;
                    if (daysUntilSunday == 0) daysUntilSunday = 7;
                    endRange = now.Date.AddDays(daysUntilSunday).AddDays(1).AddSeconds(-1);
                }
                else
                {
                    // Trong ngày
                    endRange = now.Date.AddDays(1).AddSeconds(-1);
                }
            }

            var query = BuildBaseQuery()
                .Where(s => s.EndTime >= now
                    && s.Status != null
                    && s.Status.ValueCode == "PLANNED");

            if (endRange.HasValue)
            {
                query = query.Where(s => s.StartTime <= endRange.Value);
            }

            return await ProjectToListViewModel(query)
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }

        #endregion

        #region Logic nhắc hẹn (lịch sắp đến trong 30 phút)

        public async Task<List<ScheduleListViewModel>> GetRemindersAsync()
        {
            var now = DateTime.Now;
            var thirtyMinutesLater = now.AddMinutes(30);

            var query = BuildBaseQuery()
                .Where(s => s.StartTime >= now
                    && s.StartTime <= thirtyMinutesLater
                    && s.Status != null
                    && s.Status.ValueCode == "PLANNED");

            return await ProjectToListViewModel(query)
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }

        #endregion

        #region Nhắc hẹn nâng cao (sắp đến + sắp hết lịch)

        public async Task<List<ReminderViewModel>> GetAllRemindersAsync()
        {
            var now = DateTime.Now;
            var thirtyMinutesLater = now.AddMinutes(30);
            var fifteenMinutesLater = now.AddMinutes(15);
            var reminders = new List<ReminderViewModel>();

            // 1. Lịch sắp bắt đầu trong 30 phút tới
            var startingSoon = await BuildBaseQuery()
                .Where(s => s.StartTime >= now
                    && s.StartTime <= thirtyMinutesLater
                    && s.Status != null
                    && s.Status.ValueCode == "PLANNED")
                .Select(s => new ReminderViewModel
                {
                    ScheduleID = s.ScheduleID,
                    Title = s.Title,
                    CustomerName = s.Customer != null ? s.Customer.CustomerName : "",
                    AssignedToUserName = s.AssignedToUser != null ? s.AssignedToUser.FullName : "",
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    ReminderType = "STARTING"
                })
                .ToListAsync();

            foreach (var item in startingSoon)
            {
                item.MinutesRemaining = (int)Math.Ceiling((item.StartTime - now).TotalMinutes);
                item.Message = item.MinutesRemaining <= 1
                    ? $"⏰ Lịch \"{item.Title}\" sắp bắt đầu ngay!"
                    : $"⏰ Còn {item.MinutesRemaining} phút nữa lịch \"{item.Title}\" sẽ bắt đầu";
            }

            reminders.AddRange(startingSoon);

            // 2. Lịch đang diễn ra và sắp kết thúc trong 15 phút tới
            var endingSoon = await BuildBaseQuery()
                .Where(s => s.StartTime <= now
                    && s.EndTime >= now
                    && s.EndTime <= fifteenMinutesLater
                    && s.Status != null
                    && s.Status.ValueCode == "PLANNED")
                .Select(s => new ReminderViewModel
                {
                    ScheduleID = s.ScheduleID,
                    Title = s.Title,
                    CustomerName = s.Customer != null ? s.Customer.CustomerName : "",
                    AssignedToUserName = s.AssignedToUser != null ? s.AssignedToUser.FullName : "",
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    ReminderType = "ENDING"
                })
                .ToListAsync();

            foreach (var item in endingSoon)
            {
                item.MinutesRemaining = (int)Math.Ceiling((item.EndTime - now).TotalMinutes);
                item.Message = item.MinutesRemaining <= 1
                    ? $"⚠️ Lịch \"{item.Title}\" sắp hết thời gian!"
                    : $"⚠️ Còn {item.MinutesRemaining} phút nữa lịch \"{item.Title}\" sẽ kết thúc";
            }

            reminders.AddRange(endingSoon);

            // Sắp xếp: lịch gần nhất lên đầu
            return reminders.OrderBy(r => r.MinutesRemaining).ToList();
        }

        #endregion

        #region Dữ liệu dropdown cho form

        public async Task<ScheduleFormViewModel> GetFormDataAsync(long companyId)
        {
            var customers = await _context.Customers
                .Where(c => c.CompanyID == companyId)
                .Select(c => new DropdownItem
                {
                    Value = c.CustomerID,
                    Text = c.CustomerCode + " - " + c.CustomerName
                })
                .ToListAsync();

            var branches = await _context.Branches
                .Where(b => b.CompanyID == companyId)
                .Select(b => new DropdownItem
                {
                    Value = b.BranchID,
                    Text = b.BranchCode + " - " + b.BranchName
                })
                .ToListAsync();

            var scheduleTypes = await _context.SystemTypeValues
                .Where(v => v.Type != null
                    && v.Type.TypeCode == "SCHEDULE_TYPE"
                    && v.IsActive)
                .Select(v => new DropdownItem
                {
                    Value = v.TypeValueID,
                    Text = v.ValueName
                })
                .ToListAsync();

            var users = await _context.Users
                .Where(u => u.CompanyID == companyId)
                .Select(u => new DropdownItem
                {
                    Value = u.UserID,
                    Text = u.FullName
                })
                .ToListAsync();

            return new ScheduleFormViewModel
            {
                Customers = customers,
                Branches = branches,
                ScheduleTypes = scheduleTypes,
                Users = users
            };
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Xây dựng query cơ sở với tất cả Include
        /// </summary>
        private IQueryable<CustomerSchedule> BuildBaseQuery()
        {
            return _context.CustomerSchedules
                .Include(s => s.Customer)
                .Include(s => s.Company)
                .Include(s => s.Branch)
                .Include(s => s.ScheduleType)
                .Include(s => s.Status)
                .Include(s => s.CreatedByUser)
                .Include(s => s.AssignedToUser)
                .Where(s => !s.IsDeleted)
                .AsNoTracking();
        }

        /// <summary>
        /// Chuyển đổi query sang ScheduleListViewModel
        /// </summary>
        private IQueryable<ScheduleListViewModel> ProjectToListViewModel(IQueryable<CustomerSchedule> query)
        {
            var now = DateTime.Now;

            return query.Select(s => new ScheduleListViewModel
            {
                ScheduleID = s.ScheduleID,
                Title = s.Title,
                CustomerName = s.Customer != null ? s.Customer.CustomerName : "",
                CustomerCode = s.Customer != null ? s.Customer.CustomerCode : "",
                AssignedToUserName = s.AssignedToUser != null ? s.AssignedToUser.FullName : "",
                ScheduleTypeName = s.ScheduleType != null ? s.ScheduleType.ValueName : "",
                StatusName = s.Status != null ? s.Status.ValueName : "",
                StatusCode = s.Status != null ? s.Status.ValueCode : "",
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                CreatedAt = s.CreatedAt,
                BranchName = s.Branch != null ? s.Branch.BranchName : "",
                IsOverdue = s.EndTime < now
                            && s.Status != null
                            && s.Status.ValueCode != "COMPLETED"
                            && s.Status.ValueCode != "CANCELED"
            });
        }

        #endregion
    }
}
