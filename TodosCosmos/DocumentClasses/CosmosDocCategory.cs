using System;
using TodosShared;
using static TodosCosmos.Enums;

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

        public Guid UserID { get; set; }
        
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
