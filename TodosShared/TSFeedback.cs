using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TodosShared
{
    

    [Serializable]
    public class TSFeedback
    {
        public Guid ID { get; set; }

        public Guid UserID { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [MinLength(3, ErrorMessage = "{0} min lenght should be 3")]
        [MaxLength(1000, ErrorMessage = "{0} max lenght is 1000")]
        [DataType(DataType.Text)]
        public string Text { get; set; }

        public string UserName { get; set; }

        public DateTime AddDate { get; set; }
    }
}
