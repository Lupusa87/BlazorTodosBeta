using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace TodosShared
{
    [Serializable]
    public class TSTodo
    {
        public Guid ID { get; set; }

        public Guid UserID { get; set; }


        [Required(ErrorMessage = "{0} is required")]
        [MinLength(3, ErrorMessage = "{0} min lenght should be 3")]
        [MaxLength(1000, ErrorMessage = "{0} max lenght is 1000")]
        [DataType(DataType.Text)]
        public string Name { get; set; }


       
        [MaxLength(1000, ErrorMessage = "{0} max lenght is 1000")]
        [DataType(DataType.Text)]
        public string Description { get; set; }

        [Range(1, 10, ErrorMessage = "{0} should be between 1 and 10")]
        public int Priority { get; set; }

        public bool IsDone { get; set; }

        public DateTime CreateDate { get; set; }

        public bool HasDueDate { get; set; }

        public DateTime DueDate { get; set; }

        public Guid CategoryID { get; set; }

        public List<DateTime> Reminders { get; set; } = new List<DateTime>();
    }
}
