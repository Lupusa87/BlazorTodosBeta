using System;
using System.Collections.Generic;
using System.Text;

namespace TodosShared
{

    [Serializable]
    public class TSUILanguageShort
    {

        public int N { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public bool added { get; set; }
    }
}
