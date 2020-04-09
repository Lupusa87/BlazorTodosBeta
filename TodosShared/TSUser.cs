using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace TodosShared
{
    [Serializable]
    public class TSUser
    {
        [JsonPropertyName("q")]
        [JsonProperty(PropertyName = "q")]
        public Guid ID { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [MinLength(3, ErrorMessage = "{0} min lenght should be 3")]
        [MaxLength(100, ErrorMessage = "{0} max lenght is 100")]
        [DataType(DataType.Text)]
        [JsonPropertyName("w")]
        [JsonProperty(PropertyName = "w")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [MinLength(6, ErrorMessage = "{0} min lenght should be 6")]
        [MaxLength(30, ErrorMessage = "{0} max lenght is 30")]
        [DataType(DataType.Password)]
        [JsonPropertyName("e")]
        [JsonProperty(PropertyName = "e")]
        public string Password { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [JsonPropertyName("r")]
        [JsonProperty(PropertyName = "r")]
        public string Email { get; set; }


        [Required(ErrorMessage = "{0} is required")]
        [MinLength(3, ErrorMessage = "{0} min lenght should be 3")]
        [MaxLength(100, ErrorMessage = "{0} max lenght is 100")]
        [DataType(DataType.Text)]
        [JsonPropertyName("t")]
        [JsonProperty(PropertyName = "t")]
        public string FullName { get; set; }

        [JsonPropertyName("y")]
        [JsonProperty(PropertyName = "y")]
        public bool IsLive { get; set; }

        [JsonPropertyName("u")]
        [JsonProperty(PropertyName = "u")]
        public int TodosCount { get; set; }


        [JsonPropertyName("i")]
        [JsonProperty(PropertyName = "i")]
        public DateTime CreateDate { get; set; }

        [JsonPropertyName("o")]
        [JsonProperty(PropertyName = "o")]
        public Guid LangID { get; set; }


        [Required(ErrorMessage = "{0} is required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "{0} lenght should be 10")]
        [DataType(DataType.Text)]
        [JsonPropertyName("p")]
        [JsonProperty(PropertyName = "p")]
        public string EmailedCode { get; set; }

    }
}
