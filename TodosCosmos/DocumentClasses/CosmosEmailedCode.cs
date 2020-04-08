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

        [JsonProperty(PropertyName = "e")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "c")]
        public string Code { get; set; }


        [JsonProperty(PropertyName = "ot")]
        public int OperationType { get; set; }


        [JsonProperty(PropertyName = "ia")]
        public string IPAddress { get; set; }

        [JsonProperty(PropertyName = "mid")]
        public string MachineID { get; set; }


      

    }
}
