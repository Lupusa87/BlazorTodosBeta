using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace TodosShared
{
    [Serializable]
    public class TSAppVersion
    {
        [JsonPropertyName("q")]
        [JsonProperty(PropertyName = "q")]
        public string VersionNumber { get; set; }

        [JsonPropertyName("w")]
        [JsonProperty(PropertyName = "w")]
        public DateTime VersionDate { get; set; }

    }
}
