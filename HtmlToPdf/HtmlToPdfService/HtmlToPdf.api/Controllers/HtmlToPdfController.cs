using General.Entities;
using HtmlToPdf.core.Helpers;
using HtmlToPdf.core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Reflection.Metadata;

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
        [Route("/api/convert-to_pdf")]
        [SwaggerOperation("ConvertToPdf")]
        //public async Task<ActionResult> ConvertToPdf([FromBody] Report report)
        public async Task<byte[]> ConvertToPdf([FromBody] Report report)
        {
            TransformHelper _transformHelper = new TransformHelper();
            var operationResult = await _htmlToPdfService.GetConvertedHtmltoPdf(report);
            // return StatusCode(StatusCodes.Status200OK);
            return operationResult.OutputPdf;
        }
    }
}
