using General.Entities;
using HtmlToPdf.core.Entities;
using HtmlToPdf.core.Interfaces;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using System.Net;
using System.Net.Http;

namespace HtmlToPdf.core.Helpers
{
    public class TransformHelper : ITransformHelper
    {
        public string ReplaceTables(Report report, string htmlTemplate)
        {
            foreach (var tableItem in report.ReportData.Tables)
            {
                string tableName = $"{Properties.Resources.TablePrefix}{tableItem.Name}{Properties.Resources.GeneralPostfix}";
                var headers = new List<string>();
                if (htmlTemplate.Contains(tableName, StringComparison.Ordinal))
                {
                    string table = $"<table {tableItem.TableMetaData}> <thead> <tr {tableItem.HeaderRowMetaData}>";

                    foreach (var headerItem in tableItem.headers)
                    {
                        string header = $"<th {tableItem.HeaderCellMetaData}>{headerItem}</th>";
                        table += header;
                    }

                    table += @"</tr> </thead>";
                    foreach (var rowItem in tableItem.rows)
                    {
                        table += $"<tr {tableItem.RowMetaData}>";

                        foreach (var columnItem in rowItem.Columns)
                        {
                            string column = $"<td {tableItem.CellMetaData}>{columnItem}</td>";
                            table += column;
                        }

                        table += @"</tr>";
                    }

                    table += @"</table>";
                    htmlTemplate = htmlTemplate.Replace(tableName, table, StringComparison.Ordinal);
                }
            }

            return htmlTemplate;
        }
        public string ReplaceTexts(Report report, string htmlTemplate)
        {

            foreach (var item in report.ReportData.Texts)
            {
                string textName = $"{Properties.Resources.TextPrefix}{item.Name}{Properties.Resources.GeneralPostfix}";
                // Assign the result of Replace back to templateData
                htmlTemplate = htmlTemplate.Replace(textName, item.value, StringComparison.Ordinal);
            }

            return htmlTemplate;
        }

        #region Html To Pdf Convertion Methods

        public async Task<byte[]> ConvertHtmlToPdf(string htmlReport, Report report, string header, string footer)
        {
            try
            {
                var margins = new MarginOptions
                {
                    Top = report.ReportData.PageSetup.PageMargin,
                    Bottom = report.ReportData.PageSetup.PageMargin,
                    Left = report.ReportData.PageSetup.PageMargin,
                    Right = report.ReportData.PageSetup.PageMargin
                };

                PageSetup pageSetup;

                var pdfBytes = await ConvertHtmlToPdfHelperAsync(htmlReport, "Outputpath", margins, pageSetup, header, footer);

                Console.WriteLine("PDF has been created successfully!");

                return pdfBytes;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");

                return null;
            }
        }

        // Pdf convertion helper function
        public async Task ConvertHtmlToPdfHelperAsync(string htmlReport, string outputPath, MarginOptions margins,PageSetup pageSetup ,string header, string footer)
        {
            var launchOptions = new LaunchOptions
            {
                Headless = true
            };

            using var browser = await Puppeteer.LaunchAsync(launchOptions);

            using var page = await browser.NewPageAsync();

            // Set the HTML content
            await page.SetContentAsync(htmlReport);

            // Create header template with company name and user name
            string headerTemplate = $@"
            <div style='font-family: {pageSetup.FontFamily}, sans-serif; font-size: 10px; width: 100%; padding: 10px 20px; box-sizing: border-box;'>
                <div style='display: flex; justify-content: space-between; align-items: center;'>
                    <div style='font-weight: bold;'>{WebUtility.HtmlEncode(header)}</div>
                    <div></div>
                </div>
            </div>";

            // Create footer template with email and date
            string footerTemplate = $@"
            <div style='font-family: {pageSetup.FontFamily}, sans-serif; font-size: 10px; width: 100%; padding: 10px 20px; box-sizing: border-box;'>
                <div style='display: flex; justify-content: space-between; align-items: center;'>
                    <div>{WebUtility.HtmlEncode(footer)}</div>
                    <div>Date: {DateTime.Now.ToString("yyyy-MM-dd")}</div>
                    <div>Page <span class='pageNumber'></span> of <span class='totalPages'></span></div>
                </div>
            </div>";

            // Configure PDF options
            var pdfOptions = new PdfOptions
            {
                Format = PaperFormat.A4,
                PrintBackground = true,
                MarginOptions = margins,
                DisplayHeaderFooter = true,
                HeaderTemplate = headerTemplate,
                FooterTemplate = footerTemplate
            };

            // To use in API and Serverless
            // var pdfBytes=await page.PdfDataAsync(pdfOptions);

            // Generate PDF
            await page.PdfAsync(outputPath, pdfOptions);
        }

        #endregion
    }
}
