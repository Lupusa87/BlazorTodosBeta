using System;
using TodosShared;
using static TodosCosmos.Enums;

namespace TodosCosmos.DocumentClasses
{
    public class CosmosDocReaction : BaseDocType
    {


        public CosmosDocReaction()
        {
            ID = Guid.NewGuid();
            DocType = (int)DocTypeEnum.Reaction;
            GeneratePK();
        }

        public CosmosDocReaction(TSReaction tsReaction)
        {
            UserID = tsReaction.UserID;
            ID = tsReaction.ID;
            LikeOrDislike = tsReaction.LikeOrDislike;
            DocType = (int)DocTypeEnum.Reaction;
            GeneratePK();
        }



       

        public Guid UserID { get; set; }

       
        public bool LikeOrDislike { get; set; }

        public TSReaction toTSReaction()
        {

            return new TSReaction()
            {
                UserID = UserID,
                ID = ID,
                LikeOrDislike = LikeOrDislike,
            };
        }
    }
}
