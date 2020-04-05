using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TodosCosmos.DocumentClasses;
using static TodosCosmos.Enums;

namespace TodosCosmos.ClientClasses
{
    public class CosmosDBClient_Setting
    {
        private readonly CosmosDBRepository<CosmosDocSetting> cosmosDBRepo = new CosmosDBRepository<CosmosDocSetting>();
        private readonly CosmosDBClient_Base<CosmosDocSetting> cosmosDBClientBase = new CosmosDBClient_Base<CosmosDocSetting>();
        

        private readonly string pkPrefix = ((int)DocTypeEnum.Setting).ToString();


        public async Task<bool> SetSetting(Guid UserID, string Key, string Value, List<string> CallTrace)
        {

            CosmosDocSetting tsSetting = await GetSetting(UserID, Key, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

            tsSetting.Value = Value;
           
            return await cosmosDBClientBase.UpdateItemAsync(tsSetting, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

        }

        public async Task<CosmosDocSetting> GetSetting(Guid UserID, string Key, List<string> CallTrace)
        {
            try
            {

                IEnumerable<CosmosDocSetting> result = await cosmosDBRepo.GetItemsAsync(x => x.DocType == (int)DocTypeEnum.Setting &&
                x.UserID==UserID &&
                x.Key.ToLower() == Key.ToLower(), LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));


                if (result.Count()>0)
                {
                    return result.FirstOrDefault();
                }
                else
                {
                    
                    CosmosDocSetting newSetting = new CosmosDocSetting(UserID, Key, string.Empty);

                    await cosmosDBClientBase.AddItemAsync(newSetting, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

                    return newSetting;
                }

                
            }
            catch (CosmosException ex)
            {


                await CosmosAPI.cosmosDBClientError.AddErrorLog(UserID, ex.Message, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

                return null;
            }
        }


        public async Task<bool> UpdateSettingCounter(Guid UserID, string KeyName, bool IncreaseOrDecrease, List<string> CallTrace)
        {
            try
            {


                CosmosDocSetting tsSetting = await GetSetting(UserID, KeyName, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

                if (tsSetting != null)
                {
                    if (string.IsNullOrEmpty(tsSetting.Value))
                    {


                        if (IncreaseOrDecrease)
                        {
                            await SetSetting(UserID, KeyName, "1", LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
                        }
                        else
                        {
                            await SetSetting(UserID, KeyName, "-1", LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
                        }
                    }
                    else
                    {
                        int NewID = int.Parse(tsSetting.Value);


                        if (IncreaseOrDecrease)
                        {
                            await SetSetting(UserID, KeyName, (NewID + 1).ToString(), LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
                        }
                        else
                        {
                            await SetSetting(UserID, KeyName, (NewID - 1).ToString(), LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
                        }

                    }
                }


                return true;


            }
            catch (CosmosException ex)
            {


                await CosmosAPI.cosmosDBClientError.AddErrorLog(UserID, ex.Message, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

                return false;
            }
        }


       

       
    }
}
