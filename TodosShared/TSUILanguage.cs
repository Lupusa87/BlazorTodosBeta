using System;
using System.Collections.Generic;
using System.Text;

namespace TodosShared
{
    [Serializable]
    public class TSUILanguage
    {
        public Guid ID { get; set; }
        public string Name { get; set; }

        public string Code { get; set; }

        public string FlagLink { get; set; }

        public DateTime Version { get; set; }

    }
}
