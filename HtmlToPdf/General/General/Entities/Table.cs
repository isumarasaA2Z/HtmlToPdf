using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Entities
{
    public class Table
    {
        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("tableMetaData")]
        public string? TableMetaData { get; set; }

        [JsonProperty("headerRowMetaData")]
        public string? HeaderRowMetaData { get; set; }

        [JsonProperty("headerCellMetaData")]
        public string? HeaderCellMetaData { get; set; }

        [JsonProperty("dataRowMetaData")]
        public string? RowMetaData { get; set; }

        [JsonProperty("dataCellMetaData")]
        public string? CellMetaData { get; set; }

        [JsonProperty("headers")]
        public List<string>? headers { get; set; }

        [JsonProperty("rows")]
        public List<Row>? rows { get; set; }
    }
}
