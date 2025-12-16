namespace HtmlToPdf.core.Interfaces
{
    public interface ITemplateLoader
    {
        Task<string> LoadTemplateAsync(string templateName);
        string GetDefaultTemplate();
    }
}
