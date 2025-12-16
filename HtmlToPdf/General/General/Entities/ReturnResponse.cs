using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Entities
{
    public class ReturnResponse
    {
        public int Status { get; set; }
        public string RequestId { get; set; }
#pragma warning disable CA1056 // URI-like properties should not be strings
        public byte[] OutputPdf { get; set; }
#pragma warning restore CA1056 // URI-like properties should not be strings
        public string ConvertedHtml { get; set; }
        public bool OperationSuccess { get; set; }
        public string ResponseMessage { get; set; }
        public string ExceptionMessage { get; set; }
        public string WarningMessage { get; set; }
    }
}
