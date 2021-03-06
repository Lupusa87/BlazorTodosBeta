﻿using Newtonsoft.Json;
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
            VisitsCount = pCount;
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

        [JsonProperty(PropertyName = "q")]
        public string IPAddress { get; set; }

        [JsonProperty(PropertyName = "w")]
        public int VisitsCount { get; set; }

        [JsonProperty(PropertyName = "e")]
        public Guid LangID { get; set; }

        [JsonProperty(PropertyName = "r")]
        public string DefaultFont { get; set; }


    }
}
