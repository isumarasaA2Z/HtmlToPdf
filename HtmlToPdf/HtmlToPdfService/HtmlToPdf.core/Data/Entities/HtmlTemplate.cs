using System.ComponentModel.DataAnnotations;

namespace HtmlToPdf.core.Data.Entities
{
    public class HtmlTemplate
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string TemplateName { get; set; } = string.Empty;

        [Required]
        public string HtmlContent { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        [MaxLength(200)]
        public string? CreatedBy { get; set; }

        [MaxLength(200)]
        public string? UpdatedBy { get; set; }

        public int Version { get; set; } = 1;
    }
}
