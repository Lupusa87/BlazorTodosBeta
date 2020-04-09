using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace TodosShared
{
    [Serializable]
    public class TSTodo
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
        public string Name { get; set; }


       
        [MaxLength(1000, ErrorMessage = "{0} max lenght is 1000")]
        [DataType(DataType.Text)]
        [JsonPropertyName("r")]
        [JsonProperty(PropertyName = "r")]
        public string Description { get; set; }

        [Range(1, 10, ErrorMessage = "{0} should be between 1 and 10")]
        [JsonPropertyName("t")]
        [JsonProperty(PropertyName = "t")]
        public int Priority { get; set; }

        [JsonPropertyName("y")]
        [JsonProperty(PropertyName = "y")]
        public bool IsDone { get; set; }

        [JsonPropertyName("u")]
        [JsonProperty(PropertyName = "u")]
        public DateTime CreateDate { get; set; }

        [JsonPropertyName("i")]
        [JsonProperty(PropertyName = "i")]
        public bool HasDueDate { get; set; }

        [JsonPropertyName("o")]
        [JsonProperty(PropertyName = "o")]
        public DateTime DueDate { get; set; }

        [JsonPropertyName("p")]
        [JsonProperty(PropertyName = "p")]
        public Guid CategoryID { get; set; }

        [JsonPropertyName("a")]
        [JsonProperty(PropertyName = "a")]
        public List<DateTime> Reminders { get; set; } = new List<DateTime>();
    }
}
