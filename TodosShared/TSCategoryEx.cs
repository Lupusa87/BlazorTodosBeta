using System;
using System.Collections.Generic;
using System.Text;

namespace TodosShared
{
    [Serializable]
    public class TSCategoryEx: TSCategory
    {
        public int N { get; set; }

        public short DaysLeft { get; set; }
    }
}
