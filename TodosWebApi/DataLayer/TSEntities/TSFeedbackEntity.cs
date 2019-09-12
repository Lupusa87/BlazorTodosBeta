using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodosShared;

namespace TodosWebApi.DataLayer.TSEntities
{


    public class TSFeedbackEntity : TableEntity
    {
        public TSFeedbackEntity(TSFeedback tsFeedback)
        {
            PartitionKey = tsFeedback.UserID;
            RowKey = tsFeedback.FeedbackID.ToString();
            Text = tsFeedback.Text;
        }

        public TSFeedbackEntity() { }

        public string Text { get; set; }

        public TSFeedback toTSFeedback()
        {

            return new TSFeedback()
            {
                UserID = PartitionKey,
                FeedbackID = int.Parse(RowKey),
                Text = Text,
                AddDate = Timestamp.DateTime,
            };
        }
    }
}
