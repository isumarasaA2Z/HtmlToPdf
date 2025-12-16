using General.Entities;
using HtmlToPdf.core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace HtmlToPdf.api.Controllers
{
    [ApiController]
    public class HtmlToPdfController : ControllerBase
    {
        private readonly IHtmlToPdfService _htmlToPdfService;
        private readonly ILogger<HtmlToPdfController> _logger;

        public HtmlToPdfController(IHtmlToPdfService htmlToPdfService, ILogger<HtmlToPdfController> logger)
        {
            _htmlToPdfService = htmlToPdfService
                           ?? throw new ArgumentNullException(nameof(htmlToPdfService));
            _logger = logger
                           ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        [Route("/api/convert-to_pdf")]
        [SwaggerOperation(
            Summary = "Convert HTML to PDF",
            Description = "Converts an HTML template with dynamic data to a PDF document")]
        [ProducesResponseType(typeof(byte[]), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ConvertToPdf([FromBody] Report report)
        {
            try
            {
                if (report == null)
                {
                    _logger.LogWarning("Received null report object");
                    return BadRequest(new ProblemDetails
                    {
                        Title = "Invalid request",
                        Detail = "Report cannot be null",
                        Status = StatusCodes.Status400BadRequest
                    });
                }

                if (report.ReportData == null)
                {
                    _logger.LogWarning("Received report with null ReportData");
                    return BadRequest(new ProblemDetails
                    {
                        Title = "Invalid request",
                        Detail = "Report data cannot be null",
                        Status = StatusCodes.Status400BadRequest
                    });
                }

                _logger.LogInformation("Converting report {RequestId} to PDF", report.RequestId ?? "unknown");

                var operationResult = await _htmlToPdfService.GetConvertedHtmltoPdf(report);

                if (!operationResult.OperationSuccess)
                {
                    _logger.LogError("PDF conversion failed for request {RequestId}: {Error}",
                        report.RequestId ?? "unknown",
                        operationResult.ExceptionMessage);

                    return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                    {
                        Title = "PDF conversion failed",
                        Detail = operationResult.ExceptionMessage,
                        Status = StatusCodes.Status500InternalServerError
                    });
                }

                if (operationResult.OutputPdf == null || operationResult.OutputPdf.Length == 0)
                {
                    _logger.LogError("PDF conversion returned empty result for request {RequestId}",
                        report.RequestId ?? "unknown");

                    return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                    {
                        Title = "PDF conversion failed",
                        Detail = "Generated PDF is empty",
                        Status = StatusCodes.Status500InternalServerError
                    });
                }

                _logger.LogInformation("Successfully converted report {RequestId} to PDF ({Size} bytes)",
                    report.RequestId ?? "unknown",
                    operationResult.OutputPdf.Length);

                return File(operationResult.OutputPdf, "application/pdf", $"document_{report.RequestId ?? DateTime.UtcNow.Ticks.ToString()}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error converting report {RequestId} to PDF",
                    report?.RequestId ?? "unknown");

                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "Internal server error",
                    Detail = "An unexpected error occurred while converting to PDF",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }

        [HttpPost]
        [Route("/api/convert-to_html")]
        [SwaggerOperation(
            Summary = "Convert template to HTML",
            Description = "Processes an HTML template with dynamic data and returns the HTML")]
        [ProducesResponseType(typeof(ReturnResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ConvertToHtml([FromBody] Report report)
        {
            try
            {
                if (report == null)
                {
                    _logger.LogWarning("Received null report object");
                    return BadRequest(new ProblemDetails
                    {
                        Title = "Invalid request",
                        Detail = "Report cannot be null",
                        Status = StatusCodes.Status400BadRequest
                    });
                }

                _logger.LogInformation("Converting report {RequestId} to HTML", report.RequestId ?? "unknown");

                var operationResult = await _htmlToPdfService.GetConvertedHtml(report);

                if (!operationResult.OperationSuccess)
                {
                    _logger.LogError("HTML conversion failed for request {RequestId}: {Error}",
                        report.RequestId ?? "unknown",
                        operationResult.ExceptionMessage);

                    return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                    {
                        Title = "HTML conversion failed",
                        Detail = operationResult.ExceptionMessage,
                        Status = StatusCodes.Status500InternalServerError
                    });
                }

                _logger.LogInformation("Successfully converted report {RequestId} to HTML",
                    report.RequestId ?? "unknown");

                return Ok(operationResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error converting report {RequestId} to HTML",
                    report?.RequestId ?? "unknown");

                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "Internal server error",
                    Detail = "An unexpected error occurred while converting to HTML",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }
    }
}
