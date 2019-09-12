using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodosWebApi.DataLayer.TSEntities
{
    public class TSActivityLogEntity : TableEntity
    {
        public TSActivityLogEntity(string UserID,string ActivityID, string pDescription, string pMethodName)
        {
            PartitionKey = UserID;
            RowKey = ActivityID;
            Description = pDescription;
            MethodName = pMethodName;
        }

        public TSActivityLogEntity() { }

        public string Description { get; set; }
        public string MethodName { get; set; }
    }
}
