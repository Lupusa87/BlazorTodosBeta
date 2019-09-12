using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TodosShared
{
    [Serializable]
    public class TSCategory
    {
        public Guid ID { get; set; }

        public Guid UserID { get; set; }


        [Required(ErrorMessage = "{0} is required")]
        [MinLength(3, ErrorMessage = "{0} min lenght should be 3")]
        [MaxLength(100, ErrorMessage = "{0} max lenght is 100")]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        public int TodosCount { get; set; }
    }
}
