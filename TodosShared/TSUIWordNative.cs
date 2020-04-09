using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace TodosShared
{

    [Serializable]
    public class TSUIWordNative
    {
        [JsonPropertyName("q")]
        [JsonProperty(PropertyName = "q")]
        public Guid ID { get; set; }

        [JsonPropertyName("w")]
        [JsonProperty(PropertyName = "w")]
        public string Word { get; set; }

    }
}
