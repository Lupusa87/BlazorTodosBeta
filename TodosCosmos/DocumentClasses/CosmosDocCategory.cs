using System;
using TodosShared;
using static TodosCosmos.Enums;
using Newtonsoft.Json;

namespace TodosCosmos.DocumentClasses
{
    public class CosmosDocCategory: BaseDocType
    {

        public CosmosDocCategory()
        {
            ID = new Guid();
            DocType = (int)DocTypeEnum.Category;
            GeneratePK();
        }

        public CosmosDocCategory(TSCategory tsCategory)
        {
            UserID = tsCategory.UserID;
            ID = tsCategory.ID;
            Name = tsCategory.Name;

            DocType = (int)DocTypeEnum.Category;
            GeneratePK();
        }

        [JsonProperty(PropertyName = "uid")]
        public Guid UserID { get; set; }

        [JsonProperty(PropertyName = "n")]
        public string Name { get; set; }

        public TSCategory toTSCategory()
        {

            return new TSCategory()
            {
                UserID = UserID,
                ID = ID,
                Name = Name,
            };
        }
    }
}
