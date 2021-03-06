﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace TodosShared
{


    [Serializable]
    public class TSUIWordsPair
    {
        [JsonPropertyName("q")]
        [JsonProperty(PropertyName = "q")]
        public int N { get; set; }

        [JsonPropertyName("w")]
        [JsonProperty(PropertyName = "w")]
        public string Native { get; set; }

        [JsonPropertyName("e")]
        [JsonProperty(PropertyName = "e")]
        public string Foreign { get; set; }
    }
}
