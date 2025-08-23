using General.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlToPdf.core.Interfaces
{

    public interface ITransformHelper
    {
        string ReplaceTexts(Report? report, string htmlTemplate);
        string ReplaceTables(Report? report, string htmlTemplate);
        public Task<byte[]> ConvertHtmlToPdfAsync( string htmlReport,Report report,string header,string footer,CancellationToken cancellationToken = default);

    }
}
