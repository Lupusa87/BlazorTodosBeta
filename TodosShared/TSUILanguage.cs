using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace TodosShared
{
    [Serializable]
    public class TSUILanguage
    {
        [JsonPropertyName("q")]
        [JsonProperty(PropertyName = "q")]
        public Guid ID { get; set; }

        [JsonPropertyName("w")]
        [JsonProperty(PropertyName = "w")]
        public string Name { get; set; }


        [JsonPropertyName("e")]
        [JsonProperty(PropertyName = "e")]
        public string Code { get; set; }


        [JsonPropertyName("r")]
        [JsonProperty(PropertyName = "r")]
        public string FlagLink { get; set; }


        [JsonPropertyName("t")]
        [JsonProperty(PropertyName = "t")]
        public DateTime Version { get; set; }

    }
}
