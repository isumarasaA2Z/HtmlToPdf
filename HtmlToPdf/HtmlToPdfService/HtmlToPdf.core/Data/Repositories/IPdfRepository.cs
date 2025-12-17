using HtmlToPdf.core.Data.Entities;

namespace HtmlToPdf.core.Data.Repositories
{
    /// <summary>
    /// Repository interface for PDF operations
    /// </summary>
    public interface IPdfRepository : IRepository<GeneratedPdf>
    {
        Task<GeneratedPdf?> GetByRequestIdAsync(string requestId);
        Task<IEnumerable<GeneratedPdf>> GetRecentPdfsAsync(int count = 50);
        Task<IEnumerable<GeneratedPdf>> GetPdfsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<long> GetTotalStorageSizeAsync();
        Task<GeneratedPdf?> GetWithRequestAsync(string requestId);
    }
}
