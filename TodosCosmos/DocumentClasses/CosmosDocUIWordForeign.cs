using Newtonsoft.Json;
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

        [JsonProperty(PropertyName = "q")]
        public string Word { get; set; }

        [JsonProperty(PropertyName = "w")]
        public Guid UIWordNativeID { get; set; }

        [JsonProperty(PropertyName = "e")]
        public Guid UILanguageID { get; set; }

        [JsonProperty(PropertyName = "r")]
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
