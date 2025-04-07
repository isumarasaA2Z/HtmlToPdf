using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlToPdf.core.Entities
{
    public class HtmlReportResponse
    {
        public int Status { get; set; }
        public bool Success { get; set; }
        public string Content { get; set; }
        public string ResponseMessage { get; set; }
        public string ExceptionMessage { get; set; }
        public string WarningMessage { get; set; }
    }
}
