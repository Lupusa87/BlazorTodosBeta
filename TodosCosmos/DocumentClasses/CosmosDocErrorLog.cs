using System;
using static TodosCosmos.Enums;
using Newtonsoft.Json;

namespace TodosCosmos.DocumentClasses
{
    public class CosmosDocErrorLog : BaseDocType
    {
        public CosmosDocErrorLog(Guid pUserID, string pDescription, string pMethodName)
        {
            UserID = pUserID;
            ID = Guid.NewGuid();
            Description = pDescription;
            MethodName = pMethodName;
            DocType = (int)DocTypeEnum.Error;
            GeneratePK();
        }

        [JsonProperty(PropertyName = "q")]
        public Guid UserID { get; set; }

        [JsonProperty(PropertyName = "w")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "e")]
        public string MethodName { get; set; }
    }
}