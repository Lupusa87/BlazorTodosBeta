using System;
using static TodosCosmos.Enums;

namespace TodosCosmos.DocumentClasses
{
    public class CosmosDocVisitorsStat : BaseDocType
    {
        public CosmosDocVisitorsStat(string pIPAddress, int pCount, Guid pLangID)
        {
            ID = Guid.NewGuid();
            IPAddress = pIPAddress;
            Count = pCount;
            LangID = pLangID;


            DocType = (int)DocTypeEnum.VisitorStat;
            GeneratePK();
        }

        public CosmosDocVisitorsStat()
        {

            ID = Guid.NewGuid();
            DocType = (int)DocTypeEnum.VisitorStat;
            GeneratePK();

        }


        public string IPAddress { get; set; }

        public int Count { get; set; }

        public Guid LangID { get; set; }


    }
}
