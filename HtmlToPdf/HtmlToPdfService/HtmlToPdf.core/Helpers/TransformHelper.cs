using General.Entities;
using HtmlToPdf.core.Entities;
using HtmlToPdf.core.Interfaces;
using PuppeteerSharp;
using PuppeteerSharp.Media;

namespace HtmlToPdf.core.Helpers
{
    public class TransformHelper : ITransformHelper
    {
        public string ReplaceTables(Report report, string htmlTemplate)
        {
            if (report?.ReportData?.Tables == null || !report.ReportData.Tables.Any())
            {
                return htmlTemplate;
            }

            foreach (var tableItem in report.ReportData.Tables)
            {
                if (tableItem == null || string.IsNullOrEmpty(tableItem.Name))
                {
                    continue;
                }

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
            if (report?.ReportData?.Texts == null || !report.ReportData.Texts.Any())
            {
                return htmlTemplate;
            }

            foreach (var item in report.ReportData.Texts)
            {
                if (item == null || string.IsNullOrEmpty(item.Name))
                {
                    continue;
                }

                string textName = $"{Properties.Resources.TextPrefix}{item.Name}{Properties.Resources.GeneralPostfix}";
                // Assign the result of Replace back to templateData
                htmlTemplate = htmlTemplate.Replace(textName, item.value ?? string.Empty, StringComparison.Ordinal);
            }

            return htmlTemplate;
        }

        public string ReplaceImages(Report report, string htmlTemplate)
        {
            Console.WriteLine($"[DEBUG] ReplaceImages called. Images count: {report?.ReportData?.Images?.Count ?? 0}");

            if (report?.ReportData?.Images == null || !report.ReportData.Images.Any())
            {
                Console.WriteLine("[DEBUG] No images found or Images is null");
                return htmlTemplate;
            }

            Console.WriteLine($"[DEBUG] Processing {report.ReportData.Images.Count} images");

            foreach (var imageItem in report.ReportData.Images)
            {
                if (imageItem == null || string.IsNullOrEmpty(imageItem.Name))
                {
                    Console.WriteLine("[DEBUG] Skipping null or unnamed image");
                    continue;
                }

                string imageName = $"{Properties.Resources.ImagePrefix}{imageItem.Name}{Properties.Resources.GeneralPostfix}";
                Console.WriteLine($"[DEBUG] Looking for placeholder: {imageName}");

                if (!htmlTemplate.Contains(imageName, StringComparison.Ordinal))
                {
                    Console.WriteLine($"[DEBUG] Placeholder {imageName} not found in template");
                    continue;
                }

                // Build image HTML
                string imageHtml = BuildImageHtml(imageItem);
                Console.WriteLine($"[DEBUG] Replacing {imageName} with image HTML (length: {imageHtml.Length})");

                htmlTemplate = htmlTemplate.Replace(imageName, imageHtml, StringComparison.Ordinal);
            }

            Console.WriteLine("[DEBUG] ReplaceImages completed");
            return htmlTemplate;
        }

        private string BuildImageHtml(Image imageItem)
        {
            string src;

            // Priority: Base64 > URL
            if (!string.IsNullOrEmpty(imageItem.Base64Data))
            {
                // Ensure proper base64 format
                src = imageItem.Base64Data.StartsWith("data:", StringComparison.OrdinalIgnoreCase)
                    ? imageItem.Base64Data
                    : $"data:image/png;base64,{imageItem.Base64Data}";
            }
            else if (!string.IsNullOrEmpty(imageItem.Url))
            {
                src = imageItem.Url;
            }
            else
            {
                // No image source provided
                return $"<!-- Image '{imageItem.Name}' has no source -->";
            }

            // Build attributes
            var attributes = new List<string>
            {
                $"src=\"{src}\"",
                $"alt=\"{imageItem.AltText ?? imageItem.Name ?? "Image"}\""
            };

            if (!string.IsNullOrEmpty(imageItem.Width))
            {
                attributes.Add($"width=\"{imageItem.Width}\"");
            }

            if (!string.IsNullOrEmpty(imageItem.Height))
            {
                attributes.Add($"height=\"{imageItem.Height}\"");
            }

            if (!string.IsNullOrEmpty(imageItem.CssClass))
            {
                attributes.Add($"class=\"{imageItem.CssClass}\"");
            }

            if (!string.IsNullOrEmpty(imageItem.Style))
            {
                attributes.Add($"style=\"{imageItem.Style}\"");
            }

            return $"<img {string.Join(" ", attributes)} />";
        }

        public async Task<byte[]> ConvertHtmlToPdf(string htmlReport, Report report, string header, string footer)
        {
            try
            {
                // Download Chromium if not already downloaded
                var browserFetcher = new BrowserFetcher();
                await browserFetcher.DownloadAsync();

                // Launch the browser
                await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
                {
                    Headless = true,
                    Args = new[] { "--no-sandbox", "--disable-setuid-sandbox" }
                });

                // Create a new page
                await using var page = await browser.NewPageAsync();

                // Set the HTML content
                await page.SetContentAsync(htmlReport);

                // Configure PDF options based on report settings
                var pdfOptions = new PdfOptions
                {
                    Format = GetPaperFormat(report.ReportData.PageSetup?.Size),
                    PrintBackground = true,
                    Landscape = report.ReportData.PageSetup?.Orientation?.Equals("Landscape", StringComparison.OrdinalIgnoreCase) ?? false,
                    MarginOptions = GetMarginOptions(report.ReportData.PageSetup?.PageMargin),
                    DisplayHeaderFooter = !string.IsNullOrEmpty(header) || !string.IsNullOrEmpty(footer),
                    HeaderTemplate = header ?? string.Empty,
                    FooterTemplate = footer ?? string.Empty
                };

                // Generate PDF
                var pdfBytes = await page.PdfDataAsync(pdfOptions);

                return pdfBytes;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error converting HTML to PDF: {ex.Message}", ex);
            }
        }

        private PaperFormat GetPaperFormat(string size)
        {
            return size?.ToUpperInvariant() switch
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

        private MarginOptions GetMarginOptions(PageMargin? pageMargin)
        {
            if (pageMargin == null)
            {
                return new MarginOptions
                {
                    Top = "1cm",
                    Right = "1cm",
                    Bottom = "1cm",
                    Left = "1cm"
                };
            }

            // Use header and footer margins height for top and bottom
            int topMargin = pageMargin.HeaderMargin?.Height ?? 10;
            int bottomMargin = pageMargin.FooterMargin?.Height ?? 10;
            int leftMargin = pageMargin.HeaderMargin?.Left ?? 10;
            int rightMargin = pageMargin.HeaderMargin?.Right ?? 10;

            return new MarginOptions
            {
                Top = $"{topMargin}px",
                Right = $"{rightMargin}px",
                Bottom = $"{bottomMargin}px",
                Left = $"{leftMargin}px"
            };
        }
    }
}
