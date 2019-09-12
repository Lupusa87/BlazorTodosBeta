using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodosShared;

namespace TodosWebApi.DataLayer.TSEntities
{
    public class TSCategoryEntity : TableEntity
    {
        public TSCategoryEntity(TSCategory tsCategory)
        {
            PartitionKey = tsCategory.UserID;
            RowKey = tsCategory.CategoryID.ToString();
            Name = tsCategory.Name;
        }

        public TSCategoryEntity() { }

        public string Name { get; set; }

        public TSCategory toTSCategory()
        {

            return new TSCategory()
            {
                UserID = PartitionKey,
                CategoryID =int.Parse(RowKey),
                Name = Name,
            };
        }
    }
}
