using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HtmlToPdf.core.Data.Entities
{
    public class PdfGenerationRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string RequestId { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string TemplateName { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string RequestPayload { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(200)]
        public string? CreatedBy { get; set; }

        [MaxLength(45)]
        public string? IpAddress { get; set; }

        public bool IsSuccess { get; set; }

        [MaxLength(1000)]
        public string? ErrorMessage { get; set; }

        public int? ProcessingTimeMs { get; set; }

        public virtual GeneratedPdf? GeneratedPdf { get; set; }
    }
}
