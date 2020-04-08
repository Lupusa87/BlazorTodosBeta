using System;
using System.Collections.Generic;
using System.Text;

namespace TodosCosmos
{
    public class Enums
    {
        public enum DocTypeEnum
        {
            User = 0,
            Todo = 1,
            Category = 2,
            Activity = 3,
            Error = 4,
            Feedback = 5,
            Reaction = 6,
            Setting = 7,
            VisitorStat = 8,
            EmailedCode = 9,
            UILanguage = 10,
            UIWordNative = 11,
            UIWordForeign = 12,
            FeedMessage = 13,
            Reminder = 14,
        }

        public enum RequestedActionEnum
        {
            SendEmail = 0,
            SendSMS = 1,
            NotifyAdmin = 2,
            UpdateStat = 3,
        }

        public enum DocStateMarkEnum
        {
            Insert = 0,
            Update = 1,
            Delete = 2,
            Processing = 9,
        }

    }
}
