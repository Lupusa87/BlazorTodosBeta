using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodosShared;
using TodosWebApi.GlobalDataLayer;

namespace TodosWebApi.DataLayer.TSEntities
{

    public class TSUserEntity : TableEntity
    {
        public TSUserEntity(TSUser tsUser)
        {

            PartitionKey = tsUser.UserID;
            RowKey = tsUser.UserName.ToLower();
            Email = tsUser.Email.ToLower();
            FullName = tsUser.FullName;
            IsLive = tsUser.IsLive;
            CreateDate = tsUser.CreateDate;
            TodosCount = tsUser.TodosCount;
        }

        public TSUserEntity() { }

        public string Email { get; set; }

        public string FullName { get; set; }

        public byte[] HashedPassword { get; set; }

        public string Salt { get; set; }

        public bool IsLive { get; set; }

        public int TodosCount { get; set; }

        public DateTime CreateDate { get; set; }

        public TSUser toTSUser()
        {
            return new TSUser()
            {
                UserID = PartitionKey,
                UserName = RowKey,
                Password = string.Empty,
                Email = Email,
                FullName = FullName,
                IsLive = IsLive,
                CreateDate = CreateDate,
                TodosCount = TodosCount,
            };
        }

        public TSUserOpen toTSUserOpen()
        {
            return new TSUserOpen()
            {
                FullName = FullName,
                IsLive = IsLive,
                CreateDate = CreateDate,
                TodosCount = TodosCount,
            };
        }
    }
}
