using Newtonsoft.Json;
using System;
using static TodosCosmos.Enums;

namespace TodosCosmos.DocumentClasses
{
    public class CosmosEmailedCode:BaseDocType
    {

        public CosmosEmailedCode()
        {
            ID = Guid.NewGuid();
            DocType = (int)DocTypeEnum.EmailedCode;
            TTL  = 200;   //120 knows user + 80 for processing
            GeneratePK();
        }

        [JsonProperty(PropertyName = "q")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "w")]
        public string Code { get; set; }


        [JsonProperty(PropertyName = "e")]
        public int OperationType { get; set; }


        [JsonProperty(PropertyName = "r")]
        public string IPAddress { get; set; }

        [JsonProperty(PropertyName = "t")]
        public string MachineID { get; set; }


      

    }
}
