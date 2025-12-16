using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Entities
{
    public class ReportData
    {
        [JsonProperty("includePageNumber")]
        public bool IncludePageNumber { get; set; }
        [JsonProperty("pageSetup")]
        public PageSetup? PageSetup { get; set; }
        [JsonProperty("texts")]
        public List<Text>? Texts { get; set; }
        [JsonProperty("tables")]
        public List<Table>? Tables { get; set; }
        [JsonProperty("images")]
        public List<Image>? Images { get; set; }
    }
}
