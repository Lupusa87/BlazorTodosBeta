using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TodosCosmos.DocumentClasses;
using static TodosCosmos.Enums;

namespace TodosCosmos.ClientClasses
{
    public class CosmosDBClient_Activity
    {
        private readonly CosmosDBRepository<CosmosDocActivityLog> cosmosDBRepo = new CosmosDBRepository<CosmosDocActivityLog>();
        private readonly CosmosDBClient_Base<CosmosDocActivityLog> cosmosDBClientBase = new CosmosDBClient_Base<CosmosDocActivityLog>();
        private readonly string pkPrefix = ((int)DocTypeEnum.Activity).ToString();

        public async Task<bool> AddActivityLog(Guid UserID, string Description, List<string> CallTrace)
        {

            if (CosmosAPI.DoActivityLog)
            {

                try
                {
                    CosmosDocActivityLog newActivityLog = new CosmosDocActivityLog(UserID, Description, LocalFunctions.GetCallTraceString(CallTrace));

                    await cosmosDBRepo.CreateItemAsync(newActivityLog, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

                    return true;

                }
                catch (CosmosException ex)
                {

                    await CosmosAPI.cosmosDBClientError.AddErrorLog(UserID, ex.Message, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

                    return false;
                }
            }
            else
            {
                return true;
            }
        }


        public async Task<IEnumerable<CosmosDocActivityLog>> GetAll(List<string> CallTrace)
        {
            
            try
            {
                return await cosmosDBRepo.GetItemsAsync(x => x.DocType == (int)DocTypeEnum.Activity, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

            }
            catch (CosmosException ex)
            {

                await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, ex.Message, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
                return null;
            }



        }


        public async Task<bool> Delete(CosmosDocActivityLog tsActivity, List<string> CallTrace)
        {
            return await cosmosDBClientBase.DeleteItemAsync(tsActivity, pkPrefix, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
        }
    }
}
