using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HtmlToPdf.core.Data.Entities
{
    public class GeneratedPdf
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PdfGenerationRequestId { get; set; }

        [Required]
        [MaxLength(100)]
        public string RequestId { get; set; } = string.Empty;

        [Required]
        public byte[] PdfContent { get; set; } = Array.Empty<byte>();

        public long FileSizeBytes { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? ConvertedHtml { get; set; }

        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(500)]
        public string? BlobStorageUrl { get; set; }

        [MaxLength(64)]
        public string? ContentHash { get; set; }

        [ForeignKey(nameof(PdfGenerationRequestId))]
        public virtual PdfGenerationRequest PdfGenerationRequest { get; set; } = null!;
    }
}
