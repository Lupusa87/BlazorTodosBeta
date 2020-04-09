﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using static TodosCosmos.Enums;

namespace TodosCosmos.DocumentClasses
{

    public class CosmosDocFeedMessage : BaseDocType
    {
        public CosmosDocFeedMessage(RequestedActionEnum pRequestedAction, string pBag)
        {
            ID = Guid.NewGuid();
            RequestedAction = (byte)pRequestedAction;
            Bag = pBag;
            DocType = (int)DocTypeEnum.FeedMessage;
            GeneratePK();
        }

        [JsonProperty(PropertyName = "q")]
        public string Bag { get; set; }

        [JsonProperty(PropertyName = "w")]
        public byte RequestedAction { get; set; }
    }
}
