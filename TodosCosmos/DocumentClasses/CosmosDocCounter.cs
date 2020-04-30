using Newtonsoft.Json;
using System;
using TodosShared;
using static TodosCosmos.Enums;

namespace TodosCosmos.DocumentClasses
{
    public class CosmosDocCounter : BaseDocType
    {


        public CosmosDocCounter()
        {
            ID = Guid.NewGuid();
            DocType = (int)DocTypeEnum.Counter;
            GeneratePK();
        }

        public CosmosDocCounter(TSCounter tsCounter)
        {
            ID = tsCounter.ID;
            Source = tsCounter.Source;
            Action = tsCounter.Action;
            Date = tsCounter.Date;
            DocType = (int)DocTypeEnum.Counter;
            GeneratePK();
        }

        [JsonProperty(PropertyName = "_ts")]
        public int TimeStamp { get; set; }

        [JsonProperty(PropertyName = "q")]
        public string Source { get; set; }

        [JsonProperty(PropertyName = "w")]
        public string Action { get; set; }

        [JsonProperty(PropertyName = "e")]
        public string IPAddress { get; set; }

        [JsonProperty(PropertyName = "r")]
        public DateTime Date { get; set; }

        public TSCounter toTSCounter()
        {

            return new TSCounter()
            {
                ID = ID,
                Source = Source,
                Action = Action,
                Date = Date,
            };
        }
    }
}
