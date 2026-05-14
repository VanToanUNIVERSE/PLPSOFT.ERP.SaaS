using System.ComponentModel.DataAnnotations;

namespace PLPSOFT.ERP.SaaS.Modules.CRM.Application.DTOs.Feedback
{
    /// <summary>
    /// Dùng để chỉnh sửa thông tin phản hồi
    /// </summary>
    public class UpdateFeedbackDto
    {
        public long FeedbackID { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn loại phản hồi")]
        public long FeedbackTypeID { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn mức độ ưu tiên")]
        public long PriorityID { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn trạng thái")]
        public long StatusID { get; set; }

        [Range(1, 5, ErrorMessage = "Đánh giá phải từ 1 đến 5")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tiêu đề")]
        [MaxLength(200, ErrorMessage = "Tiêu đề tối đa 200 ký tự")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập nội dung phản hồi")]
        public string Content { get; set; } = string.Empty;
    }
}
