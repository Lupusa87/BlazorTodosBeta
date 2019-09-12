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


        public async Task<bool> SetSetting(Guid UserID, string Key, string Value)
        {

            CosmosDocSetting tsSetting = await GetSetting(UserID, Key);

            tsSetting.Value = Value;
           
            return await cosmosDBClientBase.UpdateItemAsync(tsSetting);

        }

        public async Task<CosmosDocSetting> GetSetting(Guid UserID, string Key)
        {
            try
            {

                IEnumerable<CosmosDocSetting> result = await cosmosDBRepo.GetItemsAsync(x => x.DocType == (int)DocTypeEnum.Setting &&
                x.UserID==UserID &&
                x.Key.ToLower() == Key.ToLower());


                if (result.Count()>0)
                {
                    return result.FirstOrDefault();
                }
                else
                {
                    
                    CosmosDocSetting newSetting = new CosmosDocSetting(UserID, Key, string.Empty);

                    await cosmosDBClientBase.AddItemAsync(newSetting);

                    return newSetting;
                }

                
            }
            catch (CosmosException ex)
            {


                await CosmosAPI.cosmosDBClientError.AddErrorLog(UserID, ex.Message, MethodBase.GetCurrentMethod());

                return null;
            }
        }


        public async Task<bool> UpdateSettingCounter(Guid UserID, string KeyName, bool IncreaseOrDecrease)
        {
            try
            {


                CosmosDocSetting tsSetting = await GetSetting(UserID, KeyName);

                if (tsSetting != null)
                {
                    if (string.IsNullOrEmpty(tsSetting.Value))
                    {


                        if (IncreaseOrDecrease)
                        {
                            await SetSetting(UserID, KeyName, "1");
                        }
                        else
                        {
                            await SetSetting(UserID, KeyName, "-1");
                        }
                    }
                    else
                    {
                        int NewID = int.Parse(tsSetting.Value);


                        if (IncreaseOrDecrease)
                        {
                            await SetSetting(UserID, KeyName, (NewID + 1).ToString());
                        }
                        else
                        {
                            await SetSetting(UserID, KeyName, (NewID - 1).ToString());
                        }

                    }
                }


                return true;


            }
            catch (CosmosException ex)
            {


                await CosmosAPI.cosmosDBClientError.AddErrorLog(UserID, ex.Message, MethodBase.GetCurrentMethod());

                return false;
            }
        }


       

       
    }
}
