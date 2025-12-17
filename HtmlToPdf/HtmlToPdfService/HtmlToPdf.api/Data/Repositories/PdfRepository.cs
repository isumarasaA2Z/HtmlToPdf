using HtmlToPdf.core.Data.Entities;
using HtmlToPdf.core.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HtmlToPdf.api.Data.Repositories
{
    public class PdfRepository : Repository<GeneratedPdf>, IPdfRepository
    {
        public PdfRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<GeneratedPdf?> GetByRequestIdAsync(string requestId)
        {
            return await _dbSet
                .Include(p => p.PdfGenerationRequest)
                .FirstOrDefaultAsync(p => p.RequestId == requestId);
        }

        public async Task<IEnumerable<GeneratedPdf>> GetRecentPdfsAsync(int count = 50)
        {
            return await _dbSet
                .Include(p => p.PdfGenerationRequest)
                .OrderByDescending(p => p.GeneratedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<GeneratedPdf>> GetPdfsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Include(p => p.PdfGenerationRequest)
                .Where(p => p.GeneratedAt >= startDate && p.GeneratedAt <= endDate)
                .OrderByDescending(p => p.GeneratedAt)
                .ToListAsync();
        }

        public async Task<long> GetTotalStorageSizeAsync()
        {
            return await _dbSet.SumAsync(p => p.FileSizeBytes);
        }

        public async Task<GeneratedPdf?> GetWithRequestAsync(string requestId)
        {
            return await _dbSet
                .Include(p => p.PdfGenerationRequest)
                .FirstOrDefaultAsync(p => p.RequestId == requestId);
        }
    }
}
