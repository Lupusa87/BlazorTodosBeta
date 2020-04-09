using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace TodosShared
{
    [Serializable]
    public class TSUserOpen
    {
        [JsonPropertyName("q")]
        [JsonProperty(PropertyName = "q")]
        public string FullName { get; set; }

        [JsonPropertyName("w")]
        [JsonProperty(PropertyName = "w")]
        public bool IsLive { get; set; }

        [JsonPropertyName("e")]
        [JsonProperty(PropertyName = "e")]
        public DateTime CreateDate { get; set; }

        [JsonPropertyName("r")]
        [JsonProperty(PropertyName = "r")]
        public int TodosCount { get; set; }
    }
}
