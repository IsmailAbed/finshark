using System.ComponentModel.DataAnnotations;

namespace api.Dtos.Comment
{
    public class CreateCommentDto
    {
        [Required]
        [MinLength(5, ErrorMessage ="Title should be 5 characters")]
        [MaxLength(280, ErrorMessage ="Title cannot  be over 280 characters")]
         public string Title { get; set; } = string.Empty;

          [Required]
        [MinLength(5, ErrorMessage ="Title should be 5 characters")]
        [MaxLength(280, ErrorMessage ="Title cannot  be over 280 characters")]
        public string Content { get; set; } = string.Empty;
    }
}