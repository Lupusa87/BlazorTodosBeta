using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace TodosShared
{

    [Serializable]
    public class TSStat
    {
        [JsonPropertyName("q")]
        [JsonProperty(PropertyName = "q")]
        public int UsersCount { get; set; }


        [JsonPropertyName("w")]
        [JsonProperty(PropertyName = "w")]
        public int LiveUsersCount { get; set; }


        [JsonPropertyName("e")]
        [JsonProperty(PropertyName = "e")]
        public int TodosCount { get; set; }


        [JsonPropertyName("r")]
        [JsonProperty(PropertyName = "r")]
        public int IPsCount { get; set; }

        [JsonPropertyName("t")]
        [JsonProperty(PropertyName = "t")]
        public int VisitsCount { get; set; }

        [JsonPropertyName("y")]
        [JsonProperty(PropertyName = "y")]
        public int LikesCount { get; set; }

        [JsonPropertyName("u")]
        [JsonProperty(PropertyName = "u")]
        public int DislikesCount { get; set; }


        [JsonPropertyName("i")]
        [JsonProperty(PropertyName = "i")]
        public int FeedbackCount { get; set; }
        
    }
}
