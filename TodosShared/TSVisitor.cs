using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace TodosShared
{
    [Serializable]
    public class TSVisitor
    {

        [JsonPropertyName("q")]
        [JsonProperty(PropertyName = "q")]
        public string IPAddress { get; set; }

        [JsonPropertyName("w")]
        [JsonProperty(PropertyName = "w")]
        public string DefaultFont { get; set; }
    }
}
