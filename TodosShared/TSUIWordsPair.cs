using System;
using System.Collections.Generic;
using System.Text;

namespace TodosShared
{


    [Serializable]
    public class TSUIWordsPair
    {
        public int N { get; set; }
        public string Native { get; set; }

        public string Foreign { get; set; }
    }
}
