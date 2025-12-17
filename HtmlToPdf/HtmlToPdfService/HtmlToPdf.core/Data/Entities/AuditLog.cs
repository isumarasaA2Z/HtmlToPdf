using System.ComponentModel.DataAnnotations;

namespace HtmlToPdf.core.Data.Entities
{
    /// <summary>
    /// Audit log for tracking all system activities
    /// </summary>
    public class AuditLog
    {
        [Key]
        public long Id { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(50)]
        public string Action { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string EntityType { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? EntityId { get; set; }

        [MaxLength(200)]
        public string? UserId { get; set; }

        [MaxLength(200)]
        public string? UserName { get; set; }

        [MaxLength(45)]
        public string? IpAddress { get; set; }

        [MaxLength(500)]
        public string? Details { get; set; }

        /// <summary>
        /// JSON of the changes (before/after values)
        /// </summary>
        public string? Changes { get; set; }

        public bool IsSuccess { get; set; } = true;

        [MaxLength(1000)]
        public string? ErrorMessage { get; set; }
    }
}
