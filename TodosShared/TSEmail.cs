using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace TodosShared
{
    [Serializable]
    public class TSEmail
    {

        [JsonPropertyName("q")]
        [JsonProperty(PropertyName = "q")]
        public string To { get; set; }

        [JsonPropertyName("w")]
        [JsonProperty(PropertyName = "w")]
        public int OperationCode { get; set; }

        [JsonPropertyName("e")]
        [JsonProperty(PropertyName = "e")]
        public string Result { get; set; }
    }
}
