using General.Entities;

namespace HtmlToPdf.core.Interfaces
{
    public interface ITransformHelper
    {
        string ReplaceTexts(Report report, string htmlTemplate);
        string ReplaceTables(Report report, string htmlTemplate);
        Task<byte[]> ConvertHtmlToPdf(string htmlReport, Report report, string header, string footer);
        Task<string> SaveHtmlToPdfFile(string htmlReport, Report report, string header, string footer, string? outputFilePath = null);
    }
}
