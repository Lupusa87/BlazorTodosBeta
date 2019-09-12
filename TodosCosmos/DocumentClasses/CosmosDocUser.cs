using Newtonsoft.Json;
using System;
using TodosShared;
using static TodosCosmos.Enums;

namespace TodosCosmos.DocumentClasses
{


    public class CosmosDocUser : BaseDocType
    {

        public CosmosDocUser()
        {
            ID = Guid.NewGuid();
            DocType = (int)DocTypeEnum.User;
            GeneratePK();
        }

        public CosmosDocUser(TSUser tsUser)
        {

            ID = tsUser.ID;
            UserName = tsUser.UserName?.ToLower();
            Email = tsUser.Email?.ToLower();
            FullName = tsUser.FullName;
            IsLive = tsUser.IsLive;
            CreateDate = tsUser.CreateDate;
            TodosCount = tsUser.TodosCount;
            LangID = tsUser.LangID;


            DocType = (int)DocTypeEnum.User;
            GeneratePK();
        }



        public string UserName { get; set; }


        public string Email { get; set; }

        public string FullName { get; set; }

        public byte[] HashedPassword { get; set; }

        public string Salt { get; set; }

        public bool IsLive { get; set; }

        public int TodosCount { get; set; }

        public DateTime CreateDate { get; set; }


        public Guid LangID { get; set; }


        [JsonProperty(PropertyName = "_ts")]
        public int TimeStamp { get; set; }

        public TSUser toTSUser()
        {
            return new TSUser()
            {
                ID = ID,
                UserName = UserName,
                Password = string.Empty,
                Email = Email,
                FullName = FullName,
                IsLive = IsLive,
                CreateDate = CreateDate,
                TodosCount = TodosCount,
                LangID = LangID,
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
