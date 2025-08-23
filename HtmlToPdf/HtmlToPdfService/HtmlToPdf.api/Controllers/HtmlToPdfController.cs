using General.Entities;
using HtmlToPdf.core.Helpers;
using HtmlToPdf.core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Swashbuckle.AspNetCore.Annotations;
using System.Reflection.Metadata;

namespace HtmlToPdf.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("_allowSpecificOrigins")]
    public class HtmlToPdfController : ControllerBase
    {
        private readonly IHtmlToPdfService _htmlToPdfService;
        
        public HtmlToPdfController(IHtmlToPdfService htmlToPdfService)
        {
            _htmlToPdfService = htmlToPdfService
                           ?? throw new ArgumentNullException(nameof(htmlToPdfService));
        }

        [HttpPost]
        [Route("/api/convert-to_pdf")]
        [SwaggerOperation("ConvertToPdf")]
        public async Task<IActionResult> ConvertToPdf([FromBody] Report report)
        {
            try
            {
                if (report == null)
                {
                    return BadRequest("Report data is required.");
                }

                var operationResult = await _htmlToPdfService.GetConvertedHtmltoPdf(report);
                
                if (operationResult == null || !operationResult.OperationSuccess)
                {
                    return StatusCode(500, "PDF generation failed.");
                }

                // Return PDF as file download
                return File(operationResult.OutputPdf, "application/pdf", $"report_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
            }
            catch (Exception ex)
            {
                // Log the exception here if you have logging configured
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("/health")]
        [SwaggerOperation("HealthCheck")]
        public IActionResult HealthCheck()
        {
            return Ok(new { status = "healthy", timestamp = DateTime.UtcNow, service = "HtmlToPdf API" });
        }

        [HttpPost]
        [Route("/api/fill-html")]
        [SwaggerOperation("FillHtml")]
        public async Task<IActionResult> FillHtml([FromBody] Report report)
        {
            try
            {
                if (report == null)
                {
                    return BadRequest("Report data is required.");
                }

                // This would return just the filled HTML without converting to PDF
                var htmlResult = await _htmlToPdfService.GetConvertedHtml(report);
                
                if (htmlResult == null || !htmlResult.OperationSuccess)
                {
                    return StatusCode(500, "HTML generation failed.");
                }

                return Ok(new { html = htmlResult.ToString() }); // You may need to adjust this based on the actual return type
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
