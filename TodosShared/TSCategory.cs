using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace TodosShared
{
    [Serializable]
    public class TSCategory
    {

        [JsonPropertyName("q")]
        [JsonProperty(PropertyName = "q")]
        public Guid ID { get; set; }

        [JsonPropertyName("w")]
        [JsonProperty(PropertyName = "w")]
        public Guid UserID { get; set; }


        [Required(ErrorMessage = "{0} is required")]
        [MinLength(3, ErrorMessage = "{0} min lenght should be 3")]
        [MaxLength(100, ErrorMessage = "{0} max lenght is 100")]
        [DataType(DataType.Text)]
        [JsonPropertyName("e")]
        [JsonProperty(PropertyName = "e")]
        public string Name { get; set; }

        [JsonPropertyName("r")]
        [JsonProperty(PropertyName = "r")]
        public int TodosCount { get; set; }
    }
}
