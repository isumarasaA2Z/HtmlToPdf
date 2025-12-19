using HtmlToPdf.core.Data.Repositories;

namespace HtmlToPdf.core.Data
{
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
