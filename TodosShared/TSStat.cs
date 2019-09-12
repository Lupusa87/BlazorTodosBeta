using System;
using System.Collections.Generic;
using System.Text;

namespace TodosShared
{

    [Serializable]
    public class TSStat
    {
        public int UsersCount { get; set; }

        public int LiveUsersCount { get; set; }

        public int TodosCount { get; set; }

        public int IPsCount { get; set; }

        public int VisitsCount { get; set; }

        public int LikesCount { get; set; }

        public int DislikesCount { get; set; }

        public int FeedbackCount { get; set; }
        
    }
}
