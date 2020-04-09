using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace TodosShared
{
    

    [Serializable]
    public class TSFeedback
    {
        [JsonPropertyName("q")]
        [JsonProperty(PropertyName = "q")]
        public Guid ID { get; set; }

        [JsonPropertyName("w")]
        [JsonProperty(PropertyName = "w")]
        public Guid UserID { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [MinLength(3, ErrorMessage = "{0} min lenght should be 3")]
        [MaxLength(1000, ErrorMessage = "{0} max lenght is 1000")]
        [DataType(DataType.Text)]
        [JsonPropertyName("e")]
        [JsonProperty(PropertyName = "e")]
        public string Text { get; set; }

        [JsonPropertyName("r")]
        [JsonProperty(PropertyName = "r")]
        public string UserName { get; set; }

        [JsonPropertyName("t")]
        [JsonProperty(PropertyName = "t")]
        public DateTime AddDate { get; set; }
    }
}
