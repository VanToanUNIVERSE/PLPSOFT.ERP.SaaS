namespace PLPSOFT.ERP.SaaS.Modules.CRM.Application.DTOs
{
    /// <summary>
    /// ViewModel hiển thị danh sách lịch chăm sóc
    /// </summary>
    public class ScheduleListViewModel
    {
        public long ScheduleID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerCode { get; set; } = string.Empty;
        public string AssignedToUserName { get; set; } = string.Empty;
        public string ScheduleTypeName { get; set; } = string.Empty;
        public string StatusName { get; set; } = string.Empty;
        public string StatusCode { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime CreatedAt { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public bool IsOverdue { get; set; }
    }

    /// <summary>
    /// ViewModel chi tiết lịch chăm sóc
    /// </summary>
    public class ScheduleDetailViewModel
    {
        public long ScheduleID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime CreatedAt { get; set; }

        // Customer info
        public long CustomerID { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerCode { get; set; } = string.Empty;
        public string? CustomerPhone { get; set; }
        public string? CustomerEmail { get; set; }

        // Company & Branch
        public long CompanyID { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public long BranchID { get; set; }
        public string BranchName { get; set; } = string.Empty;

        // Schedule Type & Status
        public long ScheduleTypeID { get; set; }
        public string ScheduleTypeName { get; set; } = string.Empty;
        public long StatusID { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public string StatusCode { get; set; } = string.Empty;

        // Users
        public long CreatedByUserID { get; set; }
        public string CreatedByUserName { get; set; } = string.Empty;
        public long AssignedToUserID { get; set; }
        public string AssignedToUserName { get; set; } = string.Empty;

        public bool IsOverdue { get; set; }
    }

    /// <summary>
    /// DTO tạo mới lịch chăm sóc
    /// </summary>
    public class CreateScheduleDto
    {
        public long CompanyID { get; set; }
        public long BranchID { get; set; }
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Vui lòng chọn khách hàng.")]
        public long? CustomerID { get; set; }
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Vui lòng chọn loại lịch.")]
        public long? ScheduleTypeID { get; set; }
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Vui lòng nhập tiêu đề công việc.")]
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Vui lòng chọn thời gian bắt đầu.")]
        public DateTime? StartTime { get; set; }
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Vui lòng chọn thời gian kết thúc.")]
        public DateTime? EndTime { get; set; }
        public long CreatedByUserID { get; set; }
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Vui lòng chọn người phụ trách.")]
        public long? AssignedToUserID { get; set; }
    }

    /// <summary>
    /// DTO cập nhật lịch chăm sóc
    /// </summary>
    public class UpdateScheduleDto
    {
        public long ScheduleID { get; set; }
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Vui lòng chọn khách hàng.")]
        public long? CustomerID { get; set; }
        public long BranchID { get; set; }
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Vui lòng chọn loại lịch.")]
        public long? ScheduleTypeID { get; set; }
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Vui lòng nhập tiêu đề công việc.")]
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Vui lòng chọn thời gian bắt đầu.")]
        public DateTime? StartTime { get; set; }
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Vui lòng chọn thời gian kết thúc.")]
        public DateTime? EndTime { get; set; }
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Vui lòng chọn người phụ trách.")]
        public long? AssignedToUserID { get; set; }
    }

    /// <summary>
    /// DTO lọc lịch chăm sóc
    /// </summary>
    public class ScheduleFilterDto
    {
        public long? AssignedToUserID { get; set; }
        public long? CustomerID { get; set; }
        public long? ScheduleTypeID { get; set; }
        public long? StatusID { get; set; }
        public string? TimeFilter { get; set; }
    }

    /// <summary>
    /// DTO dùng cho dropdown chọn dữ liệu
    /// </summary>
    public class DropdownItem
    {
        public long Value { get; set; }
        public string Text { get; set; } = string.Empty;
    }

    /// <summary>
    /// ViewModel cho trang tạo/sửa lịch (chứa dữ liệu dropdown)
    /// </summary>
    public class ScheduleFormViewModel
    {
        public CreateScheduleDto? CreateDto { get; set; }
        public UpdateScheduleDto? UpdateDto { get; set; }

        public List<DropdownItem> Customers { get; set; } = new();
        public List<DropdownItem> Branches { get; set; } = new();
        public List<DropdownItem> ScheduleTypes { get; set; } = new();
        public List<DropdownItem> Users { get; set; } = new();
    }

    /// <summary>
    /// ViewModel cho nhắc hẹn lịch chăm sóc
    /// </summary>
    public class ReminderViewModel
    {
        public long ScheduleID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string AssignedToUserName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Loại nhắc hẹn: "STARTING" = sắp đến lịch, "ENDING" = sắp hết lịch
        /// </summary>
        public string ReminderType { get; set; } = string.Empty;

        /// <summary>
        /// Thông báo nhắc hẹn (ví dụ: "Còn 15 phút nữa sẽ bắt đầu")
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Số phút còn lại
        /// </summary>
        public int MinutesRemaining { get; set; }
    }
}
