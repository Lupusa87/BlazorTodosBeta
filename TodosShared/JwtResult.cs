using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace TodosShared
{
    [Serializable]
    public class JwtResult
    {
        [JsonPropertyName("q")]
        [JsonProperty(PropertyName = "q")]
        public string AccessToken { get; set; }

        [JsonPropertyName("w")]
        [JsonProperty(PropertyName = "w")]
        public int ExpiresIn { get; set; }


        [JsonPropertyName("e")]
        [JsonProperty(PropertyName = "e")]
        public string ErrorMessage { get; set; }
    }
}
