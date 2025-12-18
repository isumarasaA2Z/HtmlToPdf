using General.Entities;
using HtmlToPdf.core.Entities;
using HtmlToPdf.core.Interfaces;
using PuppeteerSharp;
using PuppeteerSharp.Media;

namespace HtmlToPdf.core.Helpers
{
    public class TransformHelper : ITransformHelper
    {
        private static readonly SemaphoreSlim _browserDownloadLock = new SemaphoreSlim(1, 1);
        private static bool _isBrowserDownloaded = false;
        private static readonly string _chromiumPath = Path.Combine(Directory.GetCurrentDirectory(), "chromium");

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
                htmlTemplate = htmlTemplate.Replace(textName, item.value, StringComparison.Ordinal);
            }

            return htmlTemplate;
        }

        public async Task<byte[]> ConvertHtmlToPdf(string htmlReport, Report report, string header, string footer)
        {
            try
            {
                await EnsureChromiumBrowserDownloadedAsync();
                byte[] pdfBytes = await GeneratePdfFromHtmlAsync(htmlReport, report, header, footer);
                Console.WriteLine($"[PDF] Generated successfully: {pdfBytes.Length} bytes");
                return pdfBytes;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PDF] ERROR: {ex.Message}");
                throw new InvalidOperationException($"Failed to convert HTML to PDF: {ex.Message}", ex);
            }
        }

        public async Task<string> SaveHtmlToPdfFile(string htmlReport, Report report, string header, string footer, string? outputFilePath = null)
        {
            try
            {
                await EnsureChromiumBrowserDownloadedAsync();

                if (string.IsNullOrWhiteSpace(outputFilePath))
                {
                    var outputDir = Path.Combine(Directory.GetCurrentDirectory(), "output");
                    Directory.CreateDirectory(outputDir);
                    outputFilePath = Path.Combine(outputDir, $"report_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
                }

                var directory = Path.GetDirectoryName(outputFilePath);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                string savedFilePath = await GeneratePdfFileFromHtmlAsync(htmlReport, report, header, footer, outputFilePath);
                Console.WriteLine($"[PDF] Saved successfully to: {savedFilePath}");
                return savedFilePath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PDF] ERROR: {ex.Message}");
                throw new InvalidOperationException($"Failed to save HTML to PDF file: {ex.Message}", ex);
            }
        }

        private async Task EnsureChromiumBrowserDownloadedAsync()
        {
            if (_isBrowserDownloaded)
            {
                Console.WriteLine("[Chromium] Already available - skipping check");
                return;
            }

            await _browserDownloadLock.WaitAsync();
            try
            {
                if (_isBrowserDownloaded)
                {
                    return;
                }

                Console.WriteLine("[Chromium] Checking browser availability...");
                bool chromiumExists = Directory.Exists(_chromiumPath) && 
                                      Directory.GetFiles(_chromiumPath, "*", SearchOption.AllDirectories).Any();
                
                if (chromiumExists)
                {
                    Console.WriteLine($"[Chromium] Found existing installation at: {_chromiumPath}");
                }
                else
                {
                    Console.WriteLine("[Chromium] Browser not found. Starting download...");
                    Console.WriteLine($"[Chromium] Download path: {_chromiumPath}");
                    
                    var fetcherOptions = new BrowserFetcherOptions
                    {
                        Path = _chromiumPath
                    };
                    
                    var browserFetcher = new BrowserFetcher(fetcherOptions);
                    var downloadResult = await browserFetcher.DownloadAsync();
                    
                    Console.WriteLine($"[Chromium] Download complete!");
                    Console.WriteLine($"[Chromium] Executable: {downloadResult.GetExecutablePath()}");
                }

                _isBrowserDownloaded = true;
                Console.WriteLine("[Chromium] Browser ready");
            }
            finally
            {
                _browserDownloadLock.Release();
            }
        }

        private async Task<byte[]> GeneratePdfFromHtmlAsync(string htmlContent, Report report, string headerHtml, string footerHtml)
        {
            var chromeExePath = Directory.GetFiles(_chromiumPath, "chrome.exe", SearchOption.AllDirectories).FirstOrDefault()
                             ?? Directory.GetFiles(_chromiumPath, "chromium", SearchOption.AllDirectories).FirstOrDefault();

            var launchOptions = new LaunchOptions
            {
                Headless = true,
                ExecutablePath = chromeExePath,
                Args = new[] { "--no-sandbox", "--disable-setuid-sandbox" }
            };

            await using var browser = await Puppeteer.LaunchAsync(launchOptions);
            await using var page = await browser.NewPageAsync();

            await page.SetContentAsync(htmlContent, new NavigationOptions
            {
                WaitUntil = new[] { WaitUntilNavigation.Networkidle0 }
            });

            string headerTemplate = BuildHeaderTemplate(report, headerHtml);
            string footerTemplate = BuildFooterTemplate(report, footerHtml);
            var pdfOptions = BuildPdfOptions(report, headerTemplate, footerTemplate);

            byte[] pdfBytes = await page.PdfDataAsync(pdfOptions);
            return pdfBytes;
        }

        private async Task<string> GeneratePdfFileFromHtmlAsync(string htmlContent, Report report, string headerHtml, string footerHtml, string outputFilePath)
        {
            var chromeExePath = Directory.GetFiles(_chromiumPath, "chrome.exe", SearchOption.AllDirectories).FirstOrDefault()
                             ?? Directory.GetFiles(_chromiumPath, "chromium", SearchOption.AllDirectories).FirstOrDefault();

            var launchOptions = new LaunchOptions
            {
                Headless = true,
                ExecutablePath = chromeExePath,
                Args = new[] { "--no-sandbox", "--disable-setuid-sandbox" }
            };

            await using var browser = await Puppeteer.LaunchAsync(launchOptions);
            await using var page = await browser.NewPageAsync();

            await page.SetContentAsync(htmlContent, new NavigationOptions
            {
                WaitUntil = new[] { WaitUntilNavigation.Networkidle0 }
            });

            string headerTemplate = BuildHeaderTemplate(report, headerHtml);
            string footerTemplate = BuildFooterTemplate(report, footerHtml);
            var pdfOptions = BuildPdfOptions(report, headerTemplate, footerTemplate);

            await page.PdfAsync(outputFilePath, pdfOptions);

            var fileInfo = new FileInfo(outputFilePath);
            Console.WriteLine($"[PDF] File size: {fileInfo.Length} bytes");

            return outputFilePath;
        }

        private string BuildHeaderTemplate(Report report, string customHeaderHtml)
        {
            if (!string.IsNullOrWhiteSpace(customHeaderHtml))
            {
                return customHeaderHtml;
            }

            var pageSetup = report?.ReportData?.PageSetup;
            var headerText = pageSetup?.HeaderText;

            if (headerText == null)
            {
                return "<div></div>";
            }

            string fontFamily = !string.IsNullOrWhiteSpace(headerText.Font) ? headerText.Font : "Arial";
            int fontSize = headerText.FontSize > 0 ? headerText.FontSize : 10;
            string alignment = !string.IsNullOrWhiteSpace(headerText.Alignment) ? headerText.Alignment : "center";

            return $@"
                <div style='font-family: {fontFamily}, sans-serif; font-size: {fontSize}px; width: 100%; 
                            padding: 10px 20px; box-sizing: border-box; text-align: {alignment};'>
                    {headerText.Text}
                </div>";
        }

        private string BuildFooterTemplate(Report report, string customFooterHtml)
        {
            if (!string.IsNullOrWhiteSpace(customFooterHtml))
            {
                return customFooterHtml;
            }

            var pageSetup = report?.ReportData?.PageSetup;
            var footerText = pageSetup?.FooterText;
            bool includePageNumber = report?.ReportData?.IncludePageNumber ?? false;

            if (footerText == null && !includePageNumber)
            {
                return "<div></div>";
            }

            string fontFamily = footerText?.Font ?? "Arial";
            int fontSize = footerText?.FontSize ?? 10;
            string alignment = footerText?.Alignment ?? "center";
            string footerContent = footerText?.Text ?? string.Empty;

            string pageNumberSection = includePageNumber 
                ? "<div>Page <span class='pageNumber'></span> of <span class='totalPages'></span></div>" 
                : string.Empty;

            return $@"
                <div style='font-family: {fontFamily}, sans-serif; font-size: {fontSize}px; width: 100%; 
                            padding: 10px 20px; box-sizing: border-box;'>
                    <div style='display: flex; justify-content: space-between; align-items: center; text-align: {alignment};'>
                        <div>{footerContent}</div>
                        <div>Date: {DateTime.Now:yyyy-MM-dd}</div>
                        {pageNumberSection}
                    </div>
                </div>";
        }

        private PdfOptions BuildPdfOptions(Report report, string headerTemplate, string footerTemplate)
        {
            var pageSetup = report?.ReportData?.PageSetup;
            
            PaperFormat paperFormat = GetPaperFormat(pageSetup?.Size);
            bool landscape = string.Equals(pageSetup?.Orientation, "landscape", StringComparison.OrdinalIgnoreCase);
            MarginOptions margins = BuildMarginOptions(pageSetup?.PageMargin);

            var pdfOptions = new PdfOptions
            {
                Format = paperFormat,
                Landscape = landscape,
                PrintBackground = true,
                MarginOptions = margins,
                DisplayHeaderFooter = !string.IsNullOrWhiteSpace(headerTemplate) || !string.IsNullOrWhiteSpace(footerTemplate),
                HeaderTemplate = headerTemplate ?? "<div></div>",
                FooterTemplate = footerTemplate ?? "<div></div>"
            };

            return pdfOptions;
        }

        private PaperFormat GetPaperFormat(string? size)
        {
            if (string.IsNullOrWhiteSpace(size))
            {
                return PaperFormat.A4;
            }

            return size.ToUpperInvariant() switch
            {
                "A4" => PaperFormat.A4,
                "A3" => PaperFormat.A3,
                "A5" => PaperFormat.A5,
                "LETTER" => PaperFormat.Letter,
                "LEGAL" => PaperFormat.Legal,
                "TABLOID" => PaperFormat.Tabloid,
                _ => PaperFormat.A4
            };
        }

        private MarginOptions BuildMarginOptions(PageMargin? pageMargin)
        {
            if (pageMargin == null)
            {
                return new MarginOptions
                {
                    Top = "20mm",
                    Bottom = "20mm",
                    Left = "20mm",
                    Right = "20mm"
                };
            }

            var headerMargin = pageMargin.HeaderMargin;
            var footerMargin = pageMargin.FooterMargin;

            return new MarginOptions
            {
                Top = headerMargin?.Height > 0 ? $"{headerMargin.Height}px" : "20mm",
                Bottom = footerMargin?.Height > 0 ? $"{footerMargin.Height}px" : "20mm",
                Left = headerMargin?.Left > 0 ? $"{headerMargin.Left}px" : "20mm",
                Right = headerMargin?.Right > 0 ? $"{headerMargin.Right}px" : "20mm"
            };
        }
    }
}
