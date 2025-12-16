using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Entities
{
    public class HeaderFooterText
    {
        [JsonProperty("text")]
        public string? Text { get; set; }
        [JsonProperty("font")]
        public string? Font { get; set; }
        [JsonProperty("fontSize")]
        public int? FontSize { get; set; }
        [JsonProperty("alignment")]
        public string? Alignment { get; set; }
    }
}
