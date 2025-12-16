using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Entities
{
    public class PageSetup
    {
        [JsonProperty("size")]
        public string? Size { get; set; }
        [JsonProperty("orientation")]
        public string? Orientation { get; set; }
        [JsonProperty("pageMargin")]
        public PageMargin? PageMargin { get; set; }
        [JsonProperty("footerText")]
        public HeaderFooterText? FooterText { get; set; }
        [JsonProperty("headerText")]
        public HeaderFooterText? HeaderText { get; set; }
        [JsonProperty("fontFamily")]
        public string? FontFamily { get; set; }
    }
}
