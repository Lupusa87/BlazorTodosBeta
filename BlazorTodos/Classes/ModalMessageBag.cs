using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorTodos.Classes
{
    public class BTMessage
    {
        public int ID { get; set; }

        public string Message { get; set; }
       // public string Source { get; set; }
    }

    public class BTError
    {
        public int ID { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }

        public DateTime OccurDate { get; set; }
    }
}
