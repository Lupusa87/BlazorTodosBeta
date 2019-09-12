using System;
using TodosShared;
using static TodosCosmos.Enums;

namespace TodosCosmos.DocumentClasses
{


    public class CosmosDocFeedback : BaseDocType
    {
        public CosmosDocFeedback()
        {
            ID = Guid.NewGuid();
            DocType = (int)DocTypeEnum.Feedback;
            GeneratePK();
        }

        public CosmosDocFeedback(TSFeedback tsFeedback)
        {
            UserID = tsFeedback.UserID;
            ID = tsFeedback.ID;
            Text = tsFeedback.Text;
            AddDate = tsFeedback.AddDate;
            DocType = (int)DocTypeEnum.Feedback;
            GeneratePK();
        }

      


        public Guid UserID { get; set; }

      
        public string Text { get; set; }

        public DateTime AddDate { get; set; }

        public TSFeedback toTSFeedback()
        {

            return new TSFeedback()
            {
                UserID = UserID,
                ID = ID,
                Text = Text,
                AddDate = AddDate,
            };
        }
    }
}
