using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodosWebApi.DataLayer.TSEntities
{
    public class TSEmailedCodeEntity: TableEntity
    {

        public TSEmailedCodeEntity(TSEmailedCode tsEmailedCode)
        {
            PartitionKey = tsEmailedCode.Email.ToLower();
            RowKey = tsEmailedCode.ID;
            Code = tsEmailedCode.Code;
            OperationType = tsEmailedCode.OperationType;
            IPAddress= tsEmailedCode.IPAddress;
            MachineID = tsEmailedCode.MachineID;
            AddDate = tsEmailedCode.AddDate;
        }

        public TSEmailedCodeEntity() { }

       
        public string Code { get; set; }

        public int OperationType { get; set; }

        public string IPAddress { get; set; }

        public string MachineID { get; set; }

        public DateTime AddDate { get; set; }

        public TSEmailedCode toTSEmailedCode()
        {


            return new TSEmailedCode()
            {
                Email = PartitionKey,
                ID = RowKey,
                Code = Code,
                OperationType = OperationType,
                IPAddress = IPAddress,
                MachineID = MachineID,
                AddDate = AddDate,
            };
        }
    }
}
