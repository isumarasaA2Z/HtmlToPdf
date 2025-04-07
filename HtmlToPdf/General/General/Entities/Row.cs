using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Entities
{
    public class Row
    {
        [JsonProperty("columns")]
        public List<string> Columns { get; set; }
    }
}
