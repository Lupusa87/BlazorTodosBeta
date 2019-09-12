using System;
using System.Collections.Generic;
using System.Text;

namespace TodosShared
{

    [Serializable]
    public class TSUILanguageShortEx
    {

        public int N { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public bool Exists { get; set; }
    }
   
}
