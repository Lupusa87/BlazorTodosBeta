using Newtonsoft.Json;
using System;
using static TodosCosmos.Enums;

namespace TodosCosmos.DocumentClasses
{
    public class CosmosDocActivityLog : BaseDocType
    {
        public CosmosDocActivityLog(Guid pUserID, string pDescription, string pMethodName)
        {
            UserID = pUserID;
            ID = Guid.NewGuid();
            Description = pDescription;
            MethodName = pMethodName;
            DocType = (int)DocTypeEnum.Activity;
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
