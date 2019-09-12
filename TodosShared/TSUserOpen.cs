using System;
using System.Collections.Generic;
using System.Text;

namespace TodosShared
{
    [Serializable]
    public class TSUserOpen
    {
        public string FullName { get; set; }

        public bool IsLive { get; set; }

        public DateTime CreateDate { get; set; }

        public int TodosCount { get; set; }
    }
}
