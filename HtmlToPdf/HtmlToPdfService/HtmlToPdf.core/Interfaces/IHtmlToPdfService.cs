using General.Entities;

namespace HtmlToPdf.core.Interfaces
{
    public interface IHtmlToPdfService
    {
        Task<ReturnResponse> GetConvertedHtmltoPdf(Report report);
        Task<ReturnResponse> GetConvertedHtml(Report report);
        Task<string> SaveHtmlToPdfFile(Report report, string? outputFilePath = null);
    }
}
