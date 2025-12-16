using Newtonsoft.Json;

namespace General.Entities
{
    public class Image
    {
        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("base64Data")]
        public string? Base64Data { get; set; }

        [JsonProperty("url")]
        public string? Url { get; set; }

        [JsonProperty("width")]
        public string? Width { get; set; }

        [JsonProperty("height")]
        public string? Height { get; set; }

        [JsonProperty("altText")]
        public string? AltText { get; set; }

        [JsonProperty("cssClass")]
        public string? CssClass { get; set; }

        [JsonProperty("style")]
        public string? Style { get; set; }
    }
}
