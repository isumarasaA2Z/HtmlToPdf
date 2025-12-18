using General.Entities;
using HtmlToPdf.core.Entities;
using HtmlToPdf.core.Interfaces;

namespace HtmlToPdf.core
{
    public class HtmlToPdfService : IHtmlToPdfService
    {
        private readonly ITransformHelper? _transformHelper;

        public HtmlToPdfService(ITransformHelper transformHelper)
        {
            _transformHelper = transformHelper ?? throw new ArgumentNullException(nameof(transformHelper));
        }

        public Task<ReturnResponse> GetConvertedHtml(ReportData document)
        {
            throw new NotImplementedException();
        }

        public Task<ReturnResponse> GetConvertedHtml(Report report)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetConvertedHtmlAsync(ReportData document)
        {
            string convertedHtml = string.Empty;
            return convertedHtml;
        }

        public async Task<ReturnResponse> GetConvertedHtmltoPdf(Report report)
        {
            ReturnResponse returnResponse = new ReturnResponse();
            string headerHtml = string.Empty;
            string footerHtml = string.Empty;
            string mainHtml = @"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Purchase Order Quotation</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 20px; }
        h1 { text-align: center; }
        table { width: 100%; border-collapse: collapse; margin-top: 20px; }
        th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }
        th { background-color: #f4f4f4; }
    </style>
</head>
<body>
    <h1>Purchase Order Quotation</h1>
    <p>This Purchase Order <strong>#{text.orderNO}#</strong> contains the following data:</p>
    
    <table>
        <tr>
            <th>Order No</th>
            <th>Line No</th>
            <th>Part No</th>
            <th>Quantity</th>
        </tr>
        #{table.Order}#
    </table>
    
    <h2>Delivery Locations:</h2>
    <ul>
        <li>No: 5, Jayamalapauara, Gampola</li>
        <li>No: 10 Kandy Road, Peradeniya</li>
        <li>No: 55 KCC, Kandy</li>
    </ul>

    <p>Order Reference: <strong>#{text.orderno1}#</strong></p>
</body>
</html>";
            mainHtml = _transformHelper.ReplaceTexts(report, mainHtml);
            mainHtml = _transformHelper.ReplaceTables(report, mainHtml);
            byte[] file = await _transformHelper.ConvertHtmlToPdf(mainHtml, report, headerHtml, footerHtml);
            returnResponse.OutputPdf = file;
            returnResponse.OperationSuccess = true;
            return returnResponse;
        }

        public async Task<string> SaveHtmlToPdfFile(Report report, string? outputFilePath = null)
        {
            string headerHtml = string.Empty;
            string footerHtml = string.Empty;
            string mainHtml = @"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Purchase Order Quotation</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 20px; }
        h1 { text-align: center; }
        table { width: 100%; border-collapse: collapse; margin-top: 20px; }
        th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }
        th { background-color: #f4f4f4; }
    </style>
</head>
<body>
    <h1>Purchase Order Quotation</h1>
    <p>This Purchase Order <strong>#{text.orderNO}#</strong> contains the following data:</p>
    
    <table>
        <tr>
            <th>Order No</th>
            <th>Line No</th>
            <th>Part No</th>
            <th>Quantity</th>
        </tr>
        #{table.Order}#
    </table>
    
    <h2>Delivery Locations:</h2>
    <ul>
        <li>No: 5, Jayamalapauara, Gampola</li>
        <li>No: 10 Kandy Road, Peradeniya</li>
        <li>No: 55 KCC, Kandy</li>
    </ul>

    <p>Order Reference: <strong>#{text.orderno1}#</strong></p>
</body>
</html>";
            mainHtml = _transformHelper.ReplaceTexts(report, mainHtml);
            mainHtml = _transformHelper.ReplaceTables(report, mainHtml);
            string savedFilePath = await _transformHelper.SaveHtmlToPdfFile(mainHtml, report, headerHtml, footerHtml, outputFilePath);
            return savedFilePath;
        }
    }
}
