using General.Entities;
using HtmlToPdf.core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace HtmlToPdf.api.Controllers
{
    public class HtmlToPdfController : Controller
    {
        private readonly IHtmlToPdfService _htmlToPdfService;

        public HtmlToPdfController(IHtmlToPdfService htmlToPdfService)
        {
            _htmlToPdfService = htmlToPdfService
                           ?? throw new ArgumentNullException(nameof(htmlToPdfService));
        }

        [HttpPost]
        [Route("/api/convert-to-pdf")]
        [SwaggerOperation(Summary = "Convert HTML to PDF", Description = "Converts HTML template to PDF and returns as downloadable file")]
        public async Task<ActionResult> ConvertToPdf([FromBody] Report report)
        {
            try
            {
                if (report == null)
                {
                    return BadRequest(new { Error = "Report cannot be null" });
                }

                Console.WriteLine("[ConvertToPdf] Starting PDF conversion...");
                var operationResult = await _htmlToPdfService.GetConvertedHtmltoPdf(report);

                if (!operationResult.OperationSuccess || operationResult.OutputPdf == null || operationResult.OutputPdf.Length == 0)
                {
                    Console.WriteLine("[ConvertToPdf] PDF generation failed");
                    return StatusCode(StatusCodes.Status500InternalServerError, 
                        new { Error = "Failed to generate PDF", Message = operationResult.ExceptionMessage });
                }

                string fileName = $"report_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                Console.WriteLine($"[ConvertToPdf] PDF generated successfully. Size: {operationResult.OutputPdf.Length} bytes");

                Response.Headers.Append("Content-Disposition", $"attachment; filename={fileName}");
                
                return File(operationResult.OutputPdf, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ConvertToPdf] ERROR: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { Error = "An error occurred while converting to PDF", Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("/api/save-to-pdf-file")]
        [SwaggerOperation(Summary = "Save PDF to file", Description = "Converts HTML template to PDF and saves to specified file path")]
        public async Task<ActionResult<string>> SaveToPdfFile([FromBody] Report report, [FromQuery] string? outputFilePath = null)
        {
            try
            {
                if (report == null)
                {
                    return BadRequest(new { Error = "Report cannot be null" });
                }

                var savedFilePath = await _htmlToPdfService.SaveHtmlToPdfFile(report, outputFilePath);

                return Ok(new 
                { 
                    FilePath = savedFilePath, 
                    Message = "PDF saved successfully",
                    Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { Error = "An error occurred while saving PDF to file", Message = ex.Message });
            }
        }
    }
}
