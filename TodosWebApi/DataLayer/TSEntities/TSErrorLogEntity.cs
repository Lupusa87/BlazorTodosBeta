using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodosWebApi.DataLayer.TSEntities
{
    public class TSErrorLogEntity : TableEntity
    {
        public TSErrorLogEntity(string UserID,string ErrorID, string pDescription, string pMethodName)
        {
            PartitionKey = UserID;
            RowKey = ErrorID;
            Description = pDescription;
            MethodName = pMethodName;
        }

        public TSErrorLogEntity() { }

        public string Description { get; set; }
        public string MethodName { get; set; }
    }
}
