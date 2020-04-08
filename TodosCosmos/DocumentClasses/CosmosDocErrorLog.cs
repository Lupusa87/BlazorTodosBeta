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

        [JsonProperty(PropertyName = "uid")]
        public Guid UserID { get; set; }

        [JsonProperty(PropertyName = "d")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "mn")]
        public string MethodName { get; set; }
    }
}