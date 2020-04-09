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


        [JsonProperty(PropertyName = "q")]
        public string UserName { get; set; }

        [JsonProperty(PropertyName = "w")]
        public string Email { get; set; }


        [JsonProperty(PropertyName = "e")]
        public string FullName { get; set; }

        [JsonProperty(PropertyName = "r")]
        public byte[] HashedPassword { get; set; }

        [JsonProperty(PropertyName = "t")]
        public string Salt { get; set; }

        [JsonProperty(PropertyName = "y")]
        public bool IsLive { get; set; }

        [JsonProperty(PropertyName = "u")]
        public int TodosCount { get; set; }

        [JsonProperty(PropertyName = "i")]
        public DateTime CreateDate { get; set; }

        [JsonProperty(PropertyName = "o")]
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
