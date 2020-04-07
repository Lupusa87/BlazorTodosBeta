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
            GeneratePK();
        }

        public string Email { get; set; }

        public string Code { get; set; }

        public int OperationType { get; set; }


        public string IPAddress { get; set; }


        public string MachineID { get; set; }


        [JsonProperty(PropertyName = "ttl")]
        public byte TTL { get; set; } = 200;   //120 knows user + 80 for processing

    }
}
