using System;
using System.Collections.Generic;
using System.Text;

namespace TodosShared
{

    [Serializable]
    public class TSUIWordForeign
    {
        public Guid ID { get; set; }
        public string Word { get; set; }

        public Guid UIWordNativeID { get; set; }

        public Guid UILanguageID { get; set; }

        public bool Human { get; set; }
    }
}
