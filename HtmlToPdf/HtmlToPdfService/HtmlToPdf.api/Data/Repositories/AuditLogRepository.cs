using HtmlToPdf.core.Data.Entities;
using HtmlToPdf.core.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HtmlToPdf.api.Data.Repositories
{
    public class AuditLogRepository : Repository<AuditLog>, IAuditLogRepository
    {
        public AuditLogRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<AuditLog>> GetByUserIdAsync(string userId, int count = 100)
        {
            return await _dbSet
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.Timestamp)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetByActionAsync(string action, int count = 100)
        {
            return await _dbSet
                .Where(a => a.Action == action)
                .OrderByDescending(a => a.Timestamp)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(a => a.Timestamp >= startDate && a.Timestamp <= endDate)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetFailedActionsAsync(int count = 100)
        {
            return await _dbSet
                .Where(a => !a.IsSuccess)
                .OrderByDescending(a => a.Timestamp)
                .Take(count)
                .ToListAsync();
        }

        public async Task LogActionAsync(string action, string entityType, string? entityId, string? userId, string? details, bool isSuccess = true, string? errorMessage = null)
        {
            var auditLog = new AuditLog
            {
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                UserId = userId,
                Details = details,
                IsSuccess = isSuccess,
                ErrorMessage = errorMessage,
                Timestamp = DateTime.UtcNow
            };

            await AddAsync(auditLog);
        }
    }
}
