using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Entities
{
    public class PageMargin
    {
        [JsonProperty("headerMargin")]
        public Margin? HeaderMargin { get; set; }

        [JsonProperty("footerMargin")]
        public Margin? FooterMargin { get; set; }
    }
}
