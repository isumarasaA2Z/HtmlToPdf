using General.Entities;
using HtmlToPdf.core.Entities;
using HtmlToPdf.core.Interfaces;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;

namespace HtmlToPdf.core.Helpers
{
    public class TransformHelper : ITransformHelper
    {
        public string ReplaceTables(Report? report, string htmlTemplate)
        {
            // Add null checks to prevent exceptions
            if (report?.ReportData?.Tables == null)
            {
                Console.WriteLine("[DEBUG] No tables found or report data is null");
                return htmlTemplate;
            }

            foreach (var tableItem in report.ReportData.Tables)
            {
                string tableName = $"{Properties.Resources.TablePrefix}{tableItem.Name}{Properties.Resources.GeneralPostfix}";
                var headers = new List<string>();
                if (htmlTemplate.Contains(tableName, StringComparison.Ordinal))
                {
                    string table = $"<table {tableItem.TableMetaData}> <thead> <tr {tableItem.HeaderRowMetaData}>";

                    // Add null check for headers
                    if (tableItem.headers != null)
                    {
                        foreach (var headerItem in tableItem.headers)
                        {
                            string header = $"<th {tableItem.HeaderCellMetaData}>{headerItem}</th>";
                            table += header;
                        }
                    }

                    table += @"</tr> </thead>";
                    
                    // Add null check for rows
                    if (tableItem.rows != null)
                    {
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
                    }

                    table += @"</table>";
                    htmlTemplate = htmlTemplate.Replace(tableName, table, StringComparison.Ordinal);
                }
            }

            return htmlTemplate;
        }
        public string ReplaceTexts(Report? report, string htmlTemplate)
        {
            // Add null checks to prevent exceptions
            if (report?.ReportData?.Texts == null)
            {
                Console.WriteLine("[DEBUG] No texts found or report data is null");
                return htmlTemplate;
            }

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
                var margins = BuildCustomMargins(report.ReportData?.PageSetup?.PageMargin);

                Console.WriteLine("[DEBUG] Applying custom fonts to HTML content...");
                var enhancedHtml = ApplyCustomFontsToHtml(htmlReport, report.ReportData?.PageSetup?.FontFamily ?? "Arial");

                Console.WriteLine("[DEBUG] Generating PDF from HTML...");
                var pdfBytes = await GeneratePdfFromHtmlAsync(
                    htmlContent: enhancedHtml,
                    margins: margins,
                    pageSetup: report.ReportData?.PageSetup ?? new PageSetup(),
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
                Format = GetCustomPageSize(pageSetup?.Size),
                PrintBackground = true,
                MarginOptions = margins,
                DisplayHeaderFooter = true,
                HeaderTemplate = BuildHeaderTemplate(header, pageSetup),
                FooterTemplate = BuildFooterTemplate(footer, pageSetup)
            };

            // Apply orientation if specified
            if (!string.IsNullOrWhiteSpace(pageSetup?.Orientation))
            {
                Console.WriteLine($"[DEBUG] Setting page orientation: {pageSetup.Orientation}");
                pdfOptions.Landscape = pageSetup.Orientation.Equals("landscape", StringComparison.OrdinalIgnoreCase);
            }

            Console.WriteLine("[DEBUG] Generating PDF...");
            var result = await page.PdfDataAsync(pdfOptions).ConfigureAwait(false);
            Console.WriteLine("[DEBUG] PDF generated successfully");
            Console.WriteLine("[DEBUG] GeneratePdfFromHtmlAsync - Completed");
            return result;
        }

        private MarginOptions BuildCustomMargins(PageMargin? pageMargin)
        {
            Console.WriteLine("[DEBUG] Building custom margins from PageMargin settings");
            
            if (pageMargin?.HeaderMargin == null || pageMargin?.FooterMargin == null)
            {
                Console.WriteLine("[DEBUG] Using default margins (20mm all sides)");
                return new MarginOptions
                {
                    Top = "20mm",
                    Bottom = "20mm",
                    Left = "20mm",
                    Right = "20mm"
                };
            }

            var margins = new MarginOptions
            {
                Top = $"{pageMargin.HeaderMargin.Height}mm",
                Bottom = $"{pageMargin.FooterMargin.Height}mm",
                Left = $"{pageMargin.HeaderMargin.Left}mm",
                Right = $"{pageMargin.HeaderMargin.Right}mm"
            };

            Console.WriteLine($"[DEBUG] Custom margins - Top: {margins.Top}, Bottom: {margins.Bottom}, Left: {margins.Left}, Right: {margins.Right}");
            return margins;
        }

        private string ApplyCustomFontsToHtml(string htmlContent, string fontFamily)
        {
            Console.WriteLine($"[DEBUG] Applying custom font family: {fontFamily}");
            
            if (string.IsNullOrWhiteSpace(fontFamily))
            {
                Console.WriteLine("[DEBUG] No custom font specified, using default");
                return htmlContent;
            }

            // Check if HTML already has a style tag
            if (htmlContent.Contains("<style>"))
            {
                // Insert font-family into existing body style
                var bodyStylePattern = @"body\s*\{([^}]*)font-family:\s*[^;]+;([^}]*)\}";
                var bodyStyleReplacement = $"body {{$1font-family: {fontFamily}, sans-serif;$2}}";
                
                if (System.Text.RegularExpressions.Regex.IsMatch(htmlContent, bodyStylePattern))
                {
                    htmlContent = System.Text.RegularExpressions.Regex.Replace(htmlContent, bodyStylePattern, bodyStyleReplacement);
                }
                else
                {
                    // Add font-family to existing body style
                    var existingBodyPattern = @"body\s*\{([^}]*)\}";
                    var existingBodyReplacement = $"body {{$1 font-family: {fontFamily}, sans-serif;}}";
                    htmlContent = System.Text.RegularExpressions.Regex.Replace(htmlContent, existingBodyPattern, existingBodyReplacement);
                }
            }
            else
            {
                // Add a new style tag with font-family
                var headEndIndex = htmlContent.IndexOf("</head>", StringComparison.OrdinalIgnoreCase);
                if (headEndIndex != -1)
                {
                    var fontStyle = $"\n    <style>\n        body {{ font-family: {fontFamily}, sans-serif; }}\n    </style>\n";
                    htmlContent = htmlContent.Insert(headEndIndex, fontStyle);
                }
            }

            Console.WriteLine("[DEBUG] Custom font applied to HTML content");
            return htmlContent;
        }

        private PaperFormat GetCustomPageSize(string? size)
        {
            Console.WriteLine($"[DEBUG] Converting page size: {size}");
            
            if (string.IsNullOrWhiteSpace(size))
            {
                Console.WriteLine("[DEBUG] No page size specified, using A4");
                return PaperFormat.A4;
            }

            return size.ToUpperInvariant() switch
            {
                "A3" => PaperFormat.A3,
                "A4" => PaperFormat.A4,
                "A5" => PaperFormat.A5,
                "LETTER" => PaperFormat.Letter,
                "LEGAL" => PaperFormat.Legal,
                "TABLOID" => PaperFormat.Tabloid,
                "LEDGER" => PaperFormat.Ledger,
                _ => PaperFormat.A4
            };
        }

        private static string BuildHeaderTemplate(string headerText, PageSetup? pageSetup)
        {
            Console.WriteLine("[DEBUG] Building custom header template");
            
            // Use HeaderText configuration if available, otherwise use headerText parameter
            var headerConfig = pageSetup?.HeaderText;
            var displayText = !string.IsNullOrWhiteSpace(headerConfig?.Text) ? headerConfig.Text : headerText;
            var fontFamily = !string.IsNullOrWhiteSpace(headerConfig?.Font) ? headerConfig.Font : pageSetup?.FontFamily ?? "Arial";
            var fontSize = headerConfig?.FontSize > 0 ? headerConfig.FontSize : 10;
            var alignment = GetTextAlignment(headerConfig?.Alignment);

            return $@"
                    <div style='font-family: {fontFamily}, sans-serif; font-size: {fontSize}px; width: 100%; padding: 10px 20px; box-sizing: border-box;'>
                        <div style='display: flex; justify-content: {alignment}; align-items: center;'>
                            <div style='font-weight: bold;'>{WebUtility.HtmlEncode(displayText ?? "")}</div>
                        </div>
                    </div>";
        }

        private static string BuildFooterTemplate(string footerText, PageSetup? pageSetup)
        {
            Console.WriteLine("[DEBUG] Building custom footer template");
            
            // Use FooterText configuration if available, otherwise use footerText parameter
            var footerConfig = pageSetup?.FooterText;
            var displayText = !string.IsNullOrWhiteSpace(footerConfig?.Text) ? footerConfig.Text : footerText;
            var fontFamily = !string.IsNullOrWhiteSpace(footerConfig?.Font) ? footerConfig.Font : pageSetup?.FontFamily ?? "Arial";
            var fontSize = footerConfig?.FontSize > 0 ? footerConfig.FontSize : 10;
            var alignment = GetTextAlignment(footerConfig?.Alignment);

            return $@"
                    <div style='font-family: {fontFamily}, sans-serif; font-size: {fontSize}px; width: 100%; padding: 10px 20px; box-sizing: border-box;'>
                        <div style='display: flex; justify-content: {alignment}; align-items: center;'>
                            <div>{WebUtility.HtmlEncode(displayText ?? "")}</div>
                            <div style='margin-left: auto; display: flex; gap: 20px;'>
                                <span>Date: {DateTime.UtcNow:yyyy-MM-dd}</span>
                                <span>Page <span class='pageNumber'></span> of <span class='totalPages'></span></span>
                            </div>
                        </div>
                    </div>";
        }

        private static string GetTextAlignment(string? alignment)
        {
            if (string.IsNullOrWhiteSpace(alignment))
                return "space-between";

            return alignment.ToLowerInvariant() switch
            {
                "left" => "flex-start",
                "center" => "center",
                "right" => "flex-end",
                _ => "space-between"
            };
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
