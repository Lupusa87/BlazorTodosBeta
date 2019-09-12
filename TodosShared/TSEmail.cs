using System;
using System.Collections.Generic;
using System.Text;

namespace TodosShared
{
    [Serializable]
    public class TSEmail
    {
        public string To { get; set; }
        public int OperationCode { get; set; }
        public string Result { get; set; }
    }
}
