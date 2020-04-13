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
    public static class SeedData
    {


        public static async void Run(CosmosClient cosmosClient, List<string> CallTrace)
        {
            Database database = await cosmosClient.CreateDatabaseIfNotExistsAsync(CosmosAPI.databaseID, 400);

            if (database is null)
            {
                throw new ArgumentException("database is null");
            }

            await database.CreateContainerIfNotExistsAsync(new ContainerProperties
            {
                Id = CosmosAPI.collectionID,
                PartitionKeyPath = "/pk",
                DefaultTimeToLive = -1
            });


            CosmosAPI.container = database.GetContainer(CosmosAPI.collectionID);

            if (CosmosAPI.container is null)
            {
                throw new ArgumentException("container is null");
            }


            await EnsureDemoUser(TodosCosmos.LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
            await ReadDBSettings(TodosCosmos.LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

            //SeedUILanguageData.Seed();
        }

        public static async Task<bool> EnsureDemoUser(List<string> CallTrace)
        {
            try
            {

                CosmosDocUser demoUser = await CosmosAPI.cosmosDBClientUser.FindUserByUserName("demouser", TodosCosmos.LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

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

                    await CosmosAPI.cosmosDBClientUser.AddUser(newUser, TodosCosmos.LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

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



        public static async Task<bool> ReadDBSettings(List<string> CallTrace)
        {

            CosmosDocSetting setting = await CosmosAPI.cosmosDBClientSetting.GetSetting(Guid.Empty, "DoActivityLog", TodosCosmos.LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

            if (setting != null)
            {
                if (string.IsNullOrEmpty(setting.Value))
                {
                    await CosmosAPI.cosmosDBClientSetting.SetSetting(Guid.Empty, "DoActivityLog", CosmosAPI.DoActivityLog.ToString().ToLower(), TodosCosmos.LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

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


            CosmosDocSetting settingAppVersion = await CosmosAPI.cosmosDBClientSetting.GetSetting(Guid.Empty, "AppVersion", TodosCosmos.LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

            if (settingAppVersion != null)
            {
                if (string.IsNullOrEmpty(settingAppVersion.Value))
                {
                    await CosmosAPI.cosmosDBClientSetting.SetSetting(Guid.Empty, "AppVersion", "0.0.0,2000,1,1", TodosCosmos.LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
                }
            }


            return true;

        }
    }
}
