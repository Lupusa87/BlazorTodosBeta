using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TodosCosmos;
using TodosCosmos.DocumentClasses;
using TodosGlobal;
using TodosShared;

namespace TodosFunctionsApi
{
    public class SeedData
    {

        public SeedData(CosmosClient cosmosClient, List<string> CallTrace)
        {

            cosmosClient.CreateDatabaseIfNotExistsAsync(CosmosAPI.databaseID, 400).Wait();


            Database database = cosmosClient.GetDatabase(CosmosAPI.databaseID);

            if (database is null)
            {
                throw new ArgumentException("database is null");
            }

            database.CreateContainerIfNotExistsAsync(new ContainerProperties { Id = CosmosAPI.collectionID,
                PartitionKeyPath = "/pk",
                DefaultTimeToLive = -1}).Wait();
           


            CosmosAPI.container = cosmosClient.GetContainer(CosmosAPI.databaseID, CosmosAPI.collectionID);

            if (CosmosAPI.container is null)
            {
                throw new ArgumentException("container is null");
            }


            EnsureDemoUser(TodosCosmos.LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
            ReadDBSettings(TodosCosmos.LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

            //SeedUILanguageData.Seed();


        }

        public bool EnsureDemoUser(List<string> CallTrace)
        {
            try
            {

                CosmosDocUser demoUser = CosmosAPI.cosmosDBClientUser.FindUserByUserName("demouser", TodosCosmos.LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod())).Result;

                if (demoUser is null)
                {
                    TSUser newUser = new TSUser()
                    {
                        Email = "DemoUser@ggg.ge",
                        Password = "123456789",
                        FullName = "Demo User",
                        UserName = "demouser",
                        ID = Guid.NewGuid(),
                        CreateDate = DateTime.Now,
                    };

                    CosmosAPI.cosmosDBClientUser.AddUser(newUser, TodosCosmos.LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

                    demoUser = CosmosAPI.cosmosDBClientUser.FindUserByUserName("demouser", TodosCosmos.LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod())).Result;

                    if (demoUser is null)
                    {

                        throw new ArgumentException("demouser is null");
                    }

                }
              


                CosmosAPI.DemoUserID = demoUser.ID;

        }
            catch (CosmosException)
            {

                throw new ArgumentException("Can't add/check demouser");
    }

            return true;

        }



        public bool ReadDBSettings(List<string> CallTrace)
        {
            CosmosAPI.DoActivityLog = true;

            CosmosDocSetting setting = CosmosAPI.cosmosDBClientSetting.GetSetting(Guid.Empty, "DoActivityLog", TodosCosmos.LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod())).Result;

            if (setting != null)
            {
                if (string.IsNullOrEmpty(setting.Value))
                {
                    CosmosAPI.cosmosDBClientSetting.SetSetting(Guid.Empty, "DoActivityLog", "true", TodosCosmos.LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

                    if (!CosmosAPI.DoActivityLog)
                    {
                        CosmosAPI.DoActivityLog = true;
                    }
                }
                else
                {
                    bool b = setting.Value.ToLower().Equals("true");
                    if (CosmosAPI.DoActivityLog != b)
                    {
                        CosmosAPI.DoActivityLog = b;
                    }
                }
            }

            return true;

        }
    }
}
