using HtmlToPdf.core.Data.Repositories;

namespace HtmlToPdf.core.Data
{
    /// <summary>
    /// Unit of Work pattern for managing transactions across repositories
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        IPdfRepository PdfRepository { get; }
        IAuditLogRepository AuditLogRepository { get; }
        IRepository<Entities.PdfGenerationRequest> PdfRequestRepository { get; }
        IRepository<Entities.HtmlTemplate> TemplateRepository { get; }

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
