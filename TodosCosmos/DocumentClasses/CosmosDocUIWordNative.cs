using Newtonsoft.Json;
using System;
using TodosShared;
using static TodosCosmos.Enums;

namespace TodosCosmos.DocumentClasses
{
    public class CosmosDocUIWordNative : BaseDocType
    {

        public CosmosDocUIWordNative()
        {
            ID = new Guid();
            DocType = (int)DocTypeEnum.UIWordNative;
            GeneratePK();
        }

        public CosmosDocUIWordNative(TSUIWordNative tsUIWordNative)
        {

            ID = tsUIWordNative.ID;
            Word = tsUIWordNative.Word;
          

            DocType = (int)DocTypeEnum.UIWordNative;
            GeneratePK();
        }

        [JsonProperty(PropertyName = "q")]
        public string Word { get; set; }

    

        public TSUIWordNative toTSUIWordNative()
        {

            return new TSUIWordNative()
            {
                ID = ID,
                Word = Word,
            };
        }
    }
}
