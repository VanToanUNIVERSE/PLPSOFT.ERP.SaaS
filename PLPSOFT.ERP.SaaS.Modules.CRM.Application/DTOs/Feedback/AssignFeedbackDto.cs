using System.ComponentModel.DataAnnotations;

namespace PLPSOFT.ERP.SaaS.Modules.CRM.Application.DTOs.Feedback
{
    /// <summary>
    /// Dùng để gán nhân viên xử lý phản hồi
    /// </summary>
    public class AssignFeedbackDto
    {
        public long FeedbackID { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn nhân viên xử lý")]
        public long AssignedToUserID { get; set; }
    }
}
