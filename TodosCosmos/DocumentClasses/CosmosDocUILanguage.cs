using System;
using TodosShared;
using static TodosCosmos.Enums;

namespace TodosCosmos.DocumentClasses
{
    public class CosmosDocUILanguage : BaseDocType
    {

        public CosmosDocUILanguage()
        {
            ID = new Guid();
            DocType = (int)DocTypeEnum.UILanguage;
            GeneratePK();
        }

        public CosmosDocUILanguage(TSUILanguage tsUILanguage)
        {

            ID = tsUILanguage.ID;
            Name = tsUILanguage.Name;
            Code = tsUILanguage.Code;
            FlagLink = tsUILanguage.FlagLink;
            Version = tsUILanguage.Version;


            DocType = (int)DocTypeEnum.UILanguage;
            GeneratePK();
        }

        public string Name { get; set; }

        public string Code { get; set; }

        public string FlagLink { get; set; }

        public DateTime Version { get; set; }

        public TSUILanguage toTSUILanguage()
        {

            return new TSUILanguage()
            {
                ID = ID,
                Name = Name,
                Code = Code,
                FlagLink = FlagLink,
                Version = Version,
            };
        }
    }
}
