using General.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlToPdf.core.Interfaces
{
    public interface IHtmlToPdfService
    {
        Task<ReturnResponse> GetConvertedHtmltoPdf(Report report);
        Task<ReturnResponse> GetConvertedHtml(Report report);
    }
}
