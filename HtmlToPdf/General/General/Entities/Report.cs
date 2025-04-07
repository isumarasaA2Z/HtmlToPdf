using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Entities
{
    public class Report
    {
        string? CustomerId { get; set; }
        public string? RequestId { get; set; }
        public ReportData? ReportData { get; set; }
    }
}
