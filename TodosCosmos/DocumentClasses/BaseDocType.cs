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

        [JsonProperty(PropertyName = "doctype")]
        public int DocType { get; set; } = -1;

        [JsonProperty(PropertyName = "pk")]
        public string PK { get; set; }


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
