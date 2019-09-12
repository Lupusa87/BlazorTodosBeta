using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodosShared;

namespace TodosWebApi.DataLayer.TSEntities
{
    public class TSReactionEntity : TableEntity
    {
        public TSReactionEntity(TSReaction tsReaction)
        {
            PartitionKey = tsReaction.UserID;
            RowKey = tsReaction.ReactionID.ToString();
            LikeOrDislike = tsReaction.LikeOrDislike;
        }

        public TSReactionEntity() { }

        public bool LikeOrDislike { get; set; }

        public TSReaction toTSReaction()
        {

            return new TSReaction()
            {
                UserID = PartitionKey,
                ReactionID = int.Parse(RowKey),
                LikeOrDislike = LikeOrDislike,
            };
        }
    }
}
