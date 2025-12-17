using HtmlToPdf.core.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HtmlToPdf.api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<PdfGenerationRequest> PdfGenerationRequests { get; set; }
        public DbSet<GeneratedPdf> GeneratedPdfs { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<HtmlTemplate> HtmlTemplates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure PdfGenerationRequest
            modelBuilder.Entity<PdfGenerationRequest>(entity =>
            {
                entity.ToTable("PdfGenerationRequests");
                entity.HasIndex(e => e.RequestId).IsUnique();
                entity.HasIndex(e => e.CreatedAt);
                entity.HasIndex(e => e.IsSuccess);

                entity.HasOne(e => e.GeneratedPdf)
                    .WithOne(e => e.PdfGenerationRequest)
                    .HasForeignKey<GeneratedPdf>(e => e.PdfGenerationRequestId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure GeneratedPdf
            modelBuilder.Entity<GeneratedPdf>(entity =>
            {
                entity.ToTable("GeneratedPdfs");
                entity.HasIndex(e => e.RequestId);
                entity.HasIndex(e => e.GeneratedAt);
            });

            // Configure AuditLog
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.ToTable("AuditLogs");
                entity.HasIndex(e => e.Timestamp);
                entity.HasIndex(e => e.Action);
                entity.HasIndex(e => e.EntityType);
                entity.HasIndex(e => e.UserId);
            });

            // Configure HtmlTemplate
            modelBuilder.Entity<HtmlTemplate>(entity =>
            {
                entity.ToTable("HtmlTemplates");
                entity.HasIndex(e => e.TemplateName).IsUnique();
                entity.HasIndex(e => e.IsActive);
            });
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Automatically set UpdatedAt for templates
            var entries = ChangeTracker.Entries<HtmlTemplate>()
                .Where(e => e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
