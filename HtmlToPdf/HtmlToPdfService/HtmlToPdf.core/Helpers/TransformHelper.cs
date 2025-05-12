using General.Entities;
using HtmlToPdf.core.Entities;
using HtmlToPdf.core.Interfaces;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices;

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

        // Ensure chromium is downloaded
        private async Task EnsureChromiumDownloadedAsync()
        {
            Console.WriteLine("[DEBUG] EnsureChromiumDownloadedAsync - Started");
            var chromiumPath = Path.Combine(Directory.GetCurrentDirectory(), "chromium");
            Console.WriteLine($"[DEBUG] Checking Chromium path: {chromiumPath}");

            // Check if Chromium is already downloaded
            var chromePath = Path.Combine(chromiumPath, "Chrome", "Win64-132.0.6834.83", "chrome-win64", "chrome.exe");

            if (!Directory.Exists(chromiumPath) || !File.Exists(chromePath))
            {
                Console.WriteLine("[DEBUG] Chromium not found or incomplete, initiating download...");
                // If Chromium is not downloaded or chrome.exe is missing, initiate the download
                await DownloadChromiumToCustomFolderAsync();
            }
            else
            {
                Console.WriteLine("[DEBUG] Chromium already exists");
            }
            Console.WriteLine("[DEBUG] EnsureChromiumDownloadedAsync - Completed");
        }

        // Download Chromium to a custom folder
        public async Task DownloadChromiumToCustomFolderAsync()
        {
            Console.WriteLine("[DEBUG] DownloadChromiumToCustomFolderAsync - Started");
            var chromiumPath = Path.Combine(Directory.GetCurrentDirectory(), "chromium");
            Console.WriteLine($"[DEBUG] Downloading Chromium to: {chromiumPath}");

            var fetcherOptions = new BrowserFetcherOptions
            {
                Path = chromiumPath
            };

            var browserFetcher = new BrowserFetcher(fetcherOptions);
            Console.WriteLine("[DEBUG] BrowserFetcher created, starting download...");

            // Download Chromium
            await browserFetcher.DownloadAsync();
            Console.WriteLine("[DEBUG] Chromium download completed");
            Console.WriteLine("[DEBUG] DownloadChromiumToCustomFolderAsync - Completed");
        }


        public async Task<byte[]> ConvertHtmlToPdfAsync(string htmlReport, Report report, string header, string footer, CancellationToken cancellationToken = default)
        {
            Console.WriteLine("[DEBUG] ConvertHtmlToPdfAsync - Started");
            if (string.IsNullOrWhiteSpace(htmlReport))
            {
                Console.WriteLine("[ERROR] HTML content is empty");
                throw new ArgumentException("HTML content cannot be empty.", nameof(htmlReport));
            }

            if (report == null)
            {
                Console.WriteLine("[ERROR] Report is null");
                throw new ArgumentNullException(nameof(report));
            }

            // Check Chromium is downloaded or not
            Console.WriteLine("[DEBUG] Checking Chromium installation...");
            await EnsureChromiumDownloadedAsync();

            try
            {
                Console.WriteLine("[DEBUG] Setting up PDF margins...");
                var margins = new MarginOptions
                {
                    Top = report.ReportData.PageSetup.PageMargin.HeaderMargin,
                    Bottom = report.ReportData.PageSetup.PageMargin.FooterMargin,
                    Left = report.ReportData.PageSetup.PageMargin.HeaderMargin,
                    Right = report.ReportData.PageSetup.PageMargin.FooterMargin
                };

                Console.WriteLine("[DEBUG] Generating PDF from HTML...");
                var pdfBytes = await GeneratePdfFromHtmlAsync(
                    htmlContent: htmlReport,
                    margins: margins,
                    pageSetup: report.ReportData.PageSetup,
                    header: header,
                    footer: footer,
                    cancellationToken: cancellationToken);

                Console.WriteLine("[DEBUG] PDF generation successful");
                Console.WriteLine("[DEBUG] ConvertHtmlToPdfAsync - Completed");
                return pdfBytes;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] PDF generation failed: {ex.Message}");
                throw new PdfGenerationException("Failed to generate PDF from HTML.", ex);
            }
        }

        private async Task<byte[]> GeneratePdfFromHtmlAsync(string htmlContent, MarginOptions margins, PageSetup pageSetup, string header, string footer, CancellationToken cancellationToken)
        {
            Console.WriteLine("[DEBUG] GeneratePdfFromHtmlAsync - Started");
            var chromiumRoot = Path.Combine(Directory.GetCurrentDirectory(), "chromium", "Chrome", "Win64-132.0.6834.83", "chrome-win64");
            Console.WriteLine($"[DEBUG] Looking for Chromium in: {chromiumRoot}");

            // Specific path for Windows chrome.exe
            var chromePath = Path.Combine(chromiumRoot, "chrome.exe");

            Console.WriteLine($"Chrome Path {chromePath}");

            if (chromePath == null)
            {
                Console.WriteLine("[ERROR] Chrome executable not found");
                throw new FileNotFoundException("Chrome executable not found in chromium folder.");
            }

            Console.WriteLine($"[DEBUG] Found Chrome at: {chromePath}");

            var launchOptions = new LaunchOptions
            {
                Headless = true,
                ExecutablePath = chromePath
            };

            Console.WriteLine("[DEBUG] Launching browser...");
            await using var browser = await Puppeteer.LaunchAsync(launchOptions).ConfigureAwait(false);
            Console.WriteLine("[DEBUG] Creating new page...");
            await using var page = await browser.NewPageAsync().ConfigureAwait(false);

            Console.WriteLine("[DEBUG] Setting page content...");
            await page.SetContentAsync(htmlContent).ConfigureAwait(false);

            Console.WriteLine("[DEBUG] Configuring PDF options...");
            var pdfOptions = new PdfOptions
            {
                Format = PaperFormat.A4,
                PrintBackground = true,
                MarginOptions = margins,
                DisplayHeaderFooter = true,
                HeaderTemplate = BuildHeaderTemplate(header, pageSetup),
                FooterTemplate = BuildFooterTemplate(footer, pageSetup)
            };

            Console.WriteLine("[DEBUG] Generating PDF...");
            var result = await page.PdfDataAsync(pdfOptions).ConfigureAwait(false);
            Console.WriteLine("[DEBUG] PDF generated successfully");
            Console.WriteLine("[DEBUG] GeneratePdfFromHtmlAsync - Completed");
            return result;
        }

        private static string BuildHeaderTemplate(string headerText, PageSetup pageSetup)
        {
            return $@"
                    <div style='font-family: {pageSetup.FontFamily}, sans-serif; font-size: 10px; width: 100%; padding: 10px 20px; box-sizing: border-box;'>
                        <div style='display: flex; justify-content: space-between; align-items: center;'>
                            <div style='font-weight: bold;'>{WebUtility.HtmlEncode(headerText)}</div>
                            <div></div>
                        </div>
                    </div>";
        }

        private static string BuildFooterTemplate(string footerText, PageSetup pageSetup)
        {
            return $@"
                    <div style='font-family: {pageSetup.FontFamily}, sans-serif; font-size: 10px; width: 100%; padding: 10px 20px; box-sizing: border-box;'>
                        <div style='display: flex; justify-content: space-between; align-items: center;'>
                            <div>{WebUtility.HtmlEncode(footerText)}</div>
                            <div>Date: {DateTime.UtcNow:yyyy-MM-dd}</div>
                            <div>Page <span class='pageNumber'></span> of <span class='totalPages'></span></div>
                        </div>
                    </div>";
        }

        // Custom exception for better error handling
        public class PdfGenerationException : Exception
        {
            public PdfGenerationException(string message, Exception innerException)
                : base(message, innerException) { }
        }

        #endregion
    }
}
