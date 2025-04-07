using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Entities
{
    public class Text
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("Value")]
        public string value { get; set; }
    }
}
