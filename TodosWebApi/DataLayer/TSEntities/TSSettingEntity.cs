using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodosWebApi.DataLayer.TSEntities
{
    public class TSSettingEntity : TableEntity
    {
        public TSSettingEntity(string UserID, string SettingKey, string SettingValue)
        {
            PartitionKey = UserID;
            RowKey = SettingKey.ToLower();
            Value = SettingValue;
        }

        public TSSettingEntity() { }

        public string Value { get; set; }

      
    }
}
