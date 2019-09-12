using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TodosShared
{
    [Serializable]
    public class TSUser
    {
        public Guid ID { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [MinLength(3, ErrorMessage = "{0} min lenght should be 3")]
        [MaxLength(100, ErrorMessage = "{0} max lenght is 100")]
        [DataType(DataType.Text)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [MinLength(6, ErrorMessage = "{0} min lenght should be 6")]
        [MaxLength(30, ErrorMessage = "{0} max lenght is 30")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }


        [Required(ErrorMessage = "{0} is required")]
        [MinLength(3, ErrorMessage = "{0} min lenght should be 3")]
        [MaxLength(100, ErrorMessage = "{0} max lenght is 100")]
        [DataType(DataType.Text)]
        public string FullName { get; set; }

        public bool IsLive { get; set; }

        public int TodosCount { get; set; }

        public DateTime CreateDate { get; set; }

        public Guid LangID { get; set; }


        [Required(ErrorMessage = "{0} is required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "{0} lenght should be 10")]
        [DataType(DataType.Text)]
        public string EmailedCode { get; set; }

    }
}
