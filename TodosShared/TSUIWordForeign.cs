using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace TodosShared
{

    [Serializable]
    public class TSUIWordForeign
    {
        [JsonPropertyName("q")]
        [JsonProperty(PropertyName = "q")]
        public Guid ID { get; set; }

        [JsonPropertyName("w")]
        [JsonProperty(PropertyName = "w")]
        public string Word { get; set; }


        [JsonPropertyName("e")]
        [JsonProperty(PropertyName = "e")]
        public Guid UIWordNativeID { get; set; }


        [JsonPropertyName("r")]
        [JsonProperty(PropertyName = "r")]
        public Guid UILanguageID { get; set; }

        [JsonPropertyName("t")]
        [JsonProperty(PropertyName = "t")]
        public bool Human { get; set; }
    }
}
