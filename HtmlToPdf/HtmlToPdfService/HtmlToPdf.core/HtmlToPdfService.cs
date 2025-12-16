using General.Entities;
using HtmlToPdf.core.Entities;
using HtmlToPdf.core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlToPdf.core
{
    public class HtmlToPdfService : IHtmlToPdfService
    {
        private readonly ITransformHelper? _transformHelper;
        private readonly ITemplateLoader? _templateLoader;

        public HtmlToPdfService(ITransformHelper transformHelper, ITemplateLoader templateLoader)
        {
            _transformHelper = transformHelper
                                      ?? throw new ArgumentNullException(nameof(transformHelper));
            _templateLoader = templateLoader
                                      ?? throw new ArgumentNullException(nameof(templateLoader));
        }

        public async Task<ReturnResponse> GetConvertedHtml(ReportData document)
        {
            var report = new Report
            {
                ReportData = document
            };
            return await GetConvertedHtml(report);
        }

        public async Task<ReturnResponse> GetConvertedHtml(Report report)
        {
            try
            {
                var returnResponse = new ReturnResponse();
                string htmlTemplate = await _templateLoader.LoadTemplateAsync(report.TemplateName);

                string convertedHtml = _transformHelper.ReplaceTexts(report, htmlTemplate);
                convertedHtml = _transformHelper.ReplaceTables(report, convertedHtml);
                convertedHtml = _transformHelper.ReplaceImages(report, convertedHtml);

                returnResponse.ConvertedHtml = convertedHtml;
                returnResponse.OperationSuccess = true;

                return returnResponse;
            }
            catch (Exception ex)
            {
                return new ReturnResponse
                {
                    OperationSuccess = false,
                    ExceptionMessage = $"Error converting to HTML: {ex.Message}"
                };
            }
        }

        public async Task<string> GetConvertedHtmlAsync(ReportData document)
        {
            string convertedHtml = string.Empty;
            //Replace template from input here

            return convertedHtml;
        }

        public async Task<ReturnResponse> GetConvertedHtmltoPdf(Report report)
        {
            try
            {
                if (report == null)
                {
                    throw new ArgumentNullException(nameof(report), "Report cannot be null");
                }

                if (report.ReportData == null)
                {
                    throw new ArgumentNullException(nameof(report.ReportData), "Report data cannot be null");
                }

                var returnResponse = new ReturnResponse();
                string headerHtml = GenerateHeaderHtml(report.ReportData.PageSetup?.HeaderText?.Text);
                string footerHtml = GenerateFooterHtml(report.ReportData.PageSetup?.FooterText?.Text);
                string mainHtml = await _templateLoader.LoadTemplateAsync(report.TemplateName);

                mainHtml = _transformHelper.ReplaceTexts(report, mainHtml);
                mainHtml = _transformHelper.ReplaceTables(report, mainHtml);
                mainHtml = _transformHelper.ReplaceImages(report, mainHtml);

                byte[] pdfBytes = await _transformHelper.ConvertHtmlToPdf(mainHtml, report, headerHtml, footerHtml);

                returnResponse.OutputPdf = pdfBytes;
                returnResponse.OperationSuccess = true;
                returnResponse.RequestId = report.RequestId;

                return returnResponse;
            }
            catch (Exception ex)
            {
                return new ReturnResponse
                {
                    OperationSuccess = false,
                    ExceptionMessage = $"Error converting HTML to PDF: {ex.Message}",
                    RequestId = report?.RequestId
                };
            }
        }

        private string GenerateHeaderHtml(string headerText)
        {
            if (string.IsNullOrEmpty(headerText))
            {
                return string.Empty;
            }

            return $@"
<div style='font-size: 10px; text-align: center; width: 100%;'>
    {headerText}
</div>";
        }

        private string GenerateFooterHtml(string footerText)
        {
            if (string.IsNullOrEmpty(footerText))
            {
                return @"
<div style='font-size: 10px; text-align: center; width: 100%;'>
    <span class='pageNumber'></span> / <span class='totalPages'></span>
</div>";
            }

            return $@"
<div style='font-size: 10px; text-align: center; width: 100%;'>
    {footerText} | Page <span class='pageNumber'></span> / <span class='totalPages'></span>
</div>";
        }
    }
}
