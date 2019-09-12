using System;
using System.Collections.Generic;
using System.Text;

namespace TodosShared
{
    [Serializable]
    public class TSUserOpenEx: TSUserOpen
    {
        public int N { get; set; }

        public short Days { get; set; }
    }
}
