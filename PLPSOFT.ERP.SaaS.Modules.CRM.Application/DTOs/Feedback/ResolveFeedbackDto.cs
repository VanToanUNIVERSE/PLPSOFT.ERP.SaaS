using System.ComponentModel.DataAnnotations;

namespace PLPSOFT.ERP.SaaS.Modules.CRM.Application.DTOs.Feedback
{
    /// <summary>
    /// Dùng để xử lý / đóng phản hồi (ghi kết quả giải quyết)
    /// </summary>
    public class ResolveFeedbackDto
    {
        public long FeedbackID { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nội dung xử lý")]
        public string Resolution { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn trạng thái")]
        public long StatusID { get; set; }
    }
}
