using System;
using System.Collections.Generic;
using System.Text;

namespace TodosShared
{
    [Serializable]
    public class TSTodoEx:TSTodo
    {
        public int N { get; set; }

        public short DaysLeft { get; set; }

        public string Category { get; set; }
    }
}
