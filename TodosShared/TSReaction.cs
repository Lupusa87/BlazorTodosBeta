using System;
using System.Collections.Generic;
using System.Text;

namespace TodosShared
{

    [Serializable]
    public class TSReaction
    {
        public Guid ID { get; set; } = Guid.Empty;

        public Guid UserID { get; set; } = Guid.Empty;

        public bool LikeOrDislike { get; set; }

    }
}
