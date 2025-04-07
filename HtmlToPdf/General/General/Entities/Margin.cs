using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Entities
{
    public class Margin
    {
        [JsonProperty("left")]
        public int Left { get; set; } = 0;

        [JsonProperty("right")]
        public int Right { get; set; } = 0;

        [JsonProperty("height")]
        public int Height { get; set; } = 0;
    }
}
