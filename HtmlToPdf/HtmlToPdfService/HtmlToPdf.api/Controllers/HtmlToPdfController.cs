using General.Entities;
using HtmlToPdf.core.Data;
using HtmlToPdf.core.Data.Entities;
using HtmlToPdf.core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.Diagnostics;
using System.Security.Cryptography;

namespace HtmlToPdf.api.Controllers
{
    [ApiController]
    public class HtmlToPdfController : ControllerBase
    {
        private readonly IHtmlToPdfService _htmlToPdfService;
        private readonly ILogger<HtmlToPdfController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HtmlToPdfController(
            IHtmlToPdfService htmlToPdfService,
            ILogger<HtmlToPdfController> logger,
            IUnitOfWork unitOfWork)
        {
            _htmlToPdfService = htmlToPdfService
                           ?? throw new ArgumentNullException(nameof(htmlToPdfService));
            _logger = logger
                           ?? throw new ArgumentNullException(nameof(logger));
            _unitOfWork = unitOfWork
                           ?? throw new ArgumentNullException(nameof(unitOfWork));
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
            var stopwatch = Stopwatch.StartNew();
            string? ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            try
            {
                if (report == null)
                {
                    _logger.LogWarning("Received null report object");
                    await LogFailedRequest("PDF_GENERATION", "Report", null, "Null report", ipAddress);
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
                    await LogFailedRequest("PDF_GENERATION", "Report", report.RequestId, "Null ReportData", ipAddress);
                    return BadRequest(new ProblemDetails
                    {
                        Title = "Invalid request",
                        Detail = "Report data cannot be null",
                        Status = StatusCodes.Status400BadRequest
                    });
                }

                _logger.LogInformation("Converting report {RequestId} to PDF", report.RequestId ?? "unknown");

                var operationResult = await _htmlToPdfService.GetConvertedHtmltoPdf(report);
                stopwatch.Stop();

                if (!operationResult.OperationSuccess)
                {
                    _logger.LogError("PDF conversion failed for request {RequestId}: {Error}",
                        report.RequestId ?? "unknown",
                        operationResult.ExceptionMessage);

                    await SaveFailedRequest(report, operationResult.ExceptionMessage, ipAddress, (int)stopwatch.ElapsedMilliseconds);

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

                    await SaveFailedRequest(report, "Empty PDF generated", ipAddress, (int)stopwatch.ElapsedMilliseconds);

                    return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                    {
                        Title = "PDF conversion failed",
                        Detail = "Generated PDF is empty",
                        Status = StatusCodes.Status500InternalServerError
                    });
                }

                // ✅ SAVE TO DATABASE
                await SaveSuccessfulRequest(report, operationResult.OutputPdf, operationResult.ConvertedHtml, ipAddress, (int)stopwatch.ElapsedMilliseconds);

                _logger.LogInformation("Successfully converted report {RequestId} to PDF ({Size} bytes, {Ms}ms)",
                    report.RequestId ?? "unknown",
                    operationResult.OutputPdf.Length,
                    stopwatch.ElapsedMilliseconds);

                return File(operationResult.OutputPdf, "application/pdf", $"document_{report.RequestId ?? DateTime.UtcNow.Ticks.ToString()}.pdf");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                await LogFailedRequest("PDF_GENERATION", "Report", report?.RequestId, ex.Message, ipAddress);
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

        #region Database Helpers

        private async Task SaveSuccessfulRequest(Report report, byte[] pdfBytes, string? convertedHtml, string? ipAddress, int processingTimeMs)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Save request
                var pdfRequest = new PdfGenerationRequest
                {
                    RequestId = report.RequestId ?? Guid.NewGuid().ToString(),
                    TemplateName = report.TemplateName ?? "default",
                    RequestPayload = JsonConvert.SerializeObject(report),
                    CreatedAt = DateTime.UtcNow,
                    IpAddress = ipAddress,
                    IsSuccess = true,
                    ProcessingTimeMs = processingTimeMs
                };

                await _unitOfWork.PdfRequestRepository.AddAsync(pdfRequest);
                await _unitOfWork.SaveChangesAsync();

                // Save PDF
                var generatedPdf = new GeneratedPdf
                {
                    PdfGenerationRequestId = pdfRequest.Id,
                    RequestId = pdfRequest.RequestId,
                    PdfContent = pdfBytes,
                    FileSizeBytes = pdfBytes.Length,
                    ConvertedHtml = convertedHtml,
                    GeneratedAt = DateTime.UtcNow,
                    ContentHash = ComputeHash(pdfBytes)
                };

                await _unitOfWork.PdfRepository.AddAsync(generatedPdf);

                // Log audit
                await _unitOfWork.AuditLogRepository.LogActionAsync(
                    action: "PDF_GENERATED",
                    entityType: "GeneratedPdf",
                    entityId: pdfRequest.RequestId,
                    userId: "SYSTEM",
                    details: $"PDF generated successfully ({pdfBytes.Length} bytes, {processingTimeMs}ms)",
                    isSuccess: true
                );

                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("Saved PDF to database: RequestId={RequestId}, Size={Size} bytes",
                    pdfRequest.RequestId, pdfBytes.Length);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Failed to save PDF to database for request {RequestId}", report.RequestId);
                // Don't throw - PDF was generated successfully, database save is secondary
            }
        }

        private async Task SaveFailedRequest(Report report, string? errorMessage, string? ipAddress, int processingTimeMs)
        {
            try
            {
                var pdfRequest = new PdfGenerationRequest
                {
                    RequestId = report.RequestId ?? Guid.NewGuid().ToString(),
                    TemplateName = report.TemplateName ?? "default",
                    RequestPayload = JsonConvert.SerializeObject(report),
                    CreatedAt = DateTime.UtcNow,
                    IpAddress = ipAddress,
                    IsSuccess = false,
                    ErrorMessage = errorMessage,
                    ProcessingTimeMs = processingTimeMs
                };

                await _unitOfWork.PdfRequestRepository.AddAsync(pdfRequest);
                await _unitOfWork.AuditLogRepository.LogActionAsync(
                    action: "PDF_GENERATION_FAILED",
                    entityType: "PdfGenerationRequest",
                    entityId: pdfRequest.RequestId,
                    userId: "SYSTEM",
                    details: errorMessage,
                    isSuccess: false,
                    errorMessage: errorMessage
                );

                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save failed request to database");
            }
        }

        private async Task LogFailedRequest(string action, string entityType, string? entityId, string? errorMessage, string? ipAddress)
        {
            try
            {
                await _unitOfWork.AuditLogRepository.LogActionAsync(
                    action: action,
                    entityType: entityType,
                    entityId: entityId,
                    userId: "SYSTEM",
                    details: null,
                    isSuccess: false,
                    errorMessage: errorMessage
                );
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log audit entry");
            }
        }

        private string ComputeHash(byte[] data)
        {
            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(data);
            return Convert.ToHexString(hash).ToLower();
        }

        #endregion
    }
}
