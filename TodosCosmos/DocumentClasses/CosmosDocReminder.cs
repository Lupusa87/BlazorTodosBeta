using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using static TodosCosmos.Enums;

namespace TodosCosmos.DocumentClasses
{

    public class CosmosDocReminder : BaseDocType
    {
        public CosmosDocReminder(Guid todoID, DateTime remindDate)
        {
            ID = Guid.NewGuid();
            TodoID = todoID;
            RemindDate = remindDate;
            DocType = (int)DocTypeEnum.Reminder;
            GeneratePK();
        }

        [JsonProperty(PropertyName = "q")]
        public Guid TodoID { get; set; }


        [JsonProperty(PropertyName = "w")]
        public DateTime RemindDate { get; set; }
       
    }
}
