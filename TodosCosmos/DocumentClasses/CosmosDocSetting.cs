using Newtonsoft.Json;
using System;
using static TodosCosmos.Enums;

namespace TodosCosmos.DocumentClasses
{
    public class CosmosDocSetting : BaseDocType
    {
        public CosmosDocSetting(Guid pUserID, string SettingKey, string SettingValue)
        {
            UserID = pUserID;
            ID = Guid.NewGuid();

            if (!string.IsNullOrEmpty(SettingKey))
            {
                Key = SettingKey?.ToLower();
            }
            else
            {
                Key = SettingKey;
            }

            Value = SettingValue;

            DocType = (int)DocTypeEnum.Setting;
            GeneratePK();
        }

        [JsonProperty(PropertyName = "q")]
        public Guid UserID { get; set; }

        [JsonProperty(PropertyName = "w")]
        public string Key { get; set; }

        [JsonProperty(PropertyName = "e")]
        public string Value { get; set; }

        [JsonProperty(PropertyName = "_ts")]
        public int TimeStamp { get; set; }


    }
}
