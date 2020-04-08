using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TodosCosmos.ClientClasses;


namespace TodosCosmos.DocumentClasses
{
    public class BaseDocType
    {
      
        [JsonProperty(PropertyName = "id")]
        public Guid ID { get; set; } = Guid.Empty;

        [JsonProperty(PropertyName = "dt")]
        public sbyte DocType { get; set; } = -1;

        [JsonProperty(PropertyName = "pk")]
        public string PK { get; set; }

        //for feed purposes 0 is intert, 1 is update 2 is delete
        [JsonProperty(PropertyName = "iud")]
        public byte IUD { get; set; } = 0;

        [JsonProperty(PropertyName = "ttl")]
        public short TTL { get; set; } = -1;   


        public bool GeneratePK()
        {
            if (!ID.Equals(Guid.Empty) && DocType>-1)
            {
                PK = PartitionKeyGenerator.Create(DocType.ToString(), ID.ToString());

                return true;
            }
            else
            {
                return false;
            }
        }


    }
}
