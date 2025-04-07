using General.Entities;
using HtmlToPdf.core.Entities;
using HtmlToPdf.core.Interfaces;

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

        public async Task<byte[]> ConvertHtmlToPdf(string htmlReport, Report report, string header, string footer)
        {
            byte[] pdfDoc = null;
            try
            {
                return  pdfDoc;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
