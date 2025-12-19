using HtmlToPdf.core.Data.Entities;

namespace HtmlToPdf.core.Data.Repositories
{
    public interface IAuditLogRepository : IRepository<AuditLog>
    {
        Task<IEnumerable<AuditLog>> GetByUserIdAsync(string userId, int count = 100);
        Task<IEnumerable<AuditLog>> GetByActionAsync(string action, int count = 100);
        Task<IEnumerable<AuditLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<AuditLog>> GetFailedActionsAsync(int count = 100);
        Task LogActionAsync(string action, string entityType, string? entityId, string? userId, string? details, bool isSuccess = true, string? errorMessage = null);
    }
}
