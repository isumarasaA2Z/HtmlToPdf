using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HtmlToPdf.core.Data.Entities
{
    /// <summary>
    /// Stores generated PDF files
    /// </summary>
    public class GeneratedPdf
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PdfGenerationRequestId { get; set; }

        [Required]
        [MaxLength(100)]
        public string RequestId { get; set; } = string.Empty;

        /// <summary>
        /// PDF file content (stored as binary)
        /// </summary>
        [Required]
        public byte[] PdfContent { get; set; } = Array.Empty<byte>();

        /// <summary>
        /// Size of PDF in bytes
        /// </summary>
        public long FileSizeBytes { get; set; }

        /// <summary>
        /// Converted HTML before PDF generation
        /// </summary>
        [Column(TypeName = "nvarchar(max)")]
        public string? ConvertedHtml { get; set; }

        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Optional: Azure Blob Storage URL if storing in blob storage instead of database
        /// </summary>
        [MaxLength(500)]
        public string? BlobStorageUrl { get; set; }

        /// <summary>
        /// Hash of PDF content for integrity verification
        /// </summary>
        [MaxLength(64)]
        public string? ContentHash { get; set; }

        // Navigation property
        [ForeignKey(nameof(PdfGenerationRequestId))]
        public virtual PdfGenerationRequest PdfGenerationRequest { get; set; } = null!;
    }
}
