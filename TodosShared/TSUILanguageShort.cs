using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace TodosShared
{

    [Serializable]
    public class TSUILanguageShort
    {

        [JsonPropertyName("q")]
        [JsonProperty(PropertyName = "a")]
        public int N { get; set; }


        [JsonPropertyName("w")]
        [JsonProperty(PropertyName = "w")]
        public string Name { get; set; }

        [JsonPropertyName("e")]
        [JsonProperty(PropertyName = "e")]
        public string Code { get; set; }

        [JsonPropertyName("r")]
        [JsonProperty(PropertyName = "r")]
        public bool added { get; set; }
    }
}
