using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace TodosShared
{

    [Serializable]
    public class TSReaction
    {
        [JsonPropertyName("q")]
        [JsonProperty(PropertyName = "q")]
        public Guid ID { get; set; } = Guid.Empty;

        [JsonPropertyName("w")]
        [JsonProperty(PropertyName = "w")]
        public Guid UserID { get; set; } = Guid.Empty;

        [JsonPropertyName("e")]
        [JsonProperty(PropertyName = "e")]
        public bool LikeOrDislike { get; set; }

    }
}
