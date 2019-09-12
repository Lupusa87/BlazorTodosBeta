using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodosWebApi.DataLayer.TSEntities
{
    public class TSVisitorsStatEntity : TableEntity
    {
        public TSVisitorsStatEntity(string UserID, string IPAddress, int pCount)
        {
            PartitionKey = UserID;
            RowKey = IPAddress;
            Count = pCount;

        }

        public TSVisitorsStatEntity() { }

        public int Count { get; set; }


    }
}
