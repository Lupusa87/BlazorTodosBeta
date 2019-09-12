using System;
using TodosShared;
using static TodosCosmos.Enums;

namespace TodosCosmos.DocumentClasses
{
    public class CosmosDocUIWordForeign : BaseDocType
    {

        public CosmosDocUIWordForeign()
        {
            ID = new Guid();
            DocType = (int)DocTypeEnum.UIWordForeign;
            GeneratePK();
        }

        public CosmosDocUIWordForeign(TSUIWordForeign tsUIWordForeign)
        {

            ID = tsUIWordForeign.ID;
            Word = tsUIWordForeign.Word;
            UIWordNativeID = tsUIWordForeign.UIWordNativeID;
            UILanguageID = tsUIWordForeign.UILanguageID;
            Human = tsUIWordForeign.Human;

            DocType = (int)DocTypeEnum.UIWordForeign;
            GeneratePK();
        }

        public string Word { get; set; }

        public Guid UIWordNativeID { get; set; }

        public Guid UILanguageID { get; set; }

        public bool Human { get; set; }


        public TSUIWordForeign toTSUIWordForeign()
        {

            return new TSUIWordForeign()
            {
                ID = ID,
                Word = Word,
                UILanguageID = UILanguageID,
                UIWordNativeID = UIWordNativeID,
                Human = Human,
            };
        }
    }
}
