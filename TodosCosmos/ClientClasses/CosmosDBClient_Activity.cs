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
        private readonly CosmosDBRepository<CosmosDocActivityLog> cosmosDBRepoActivityLog = new CosmosDBRepository<CosmosDocActivityLog>();
        private readonly CosmosDBClient_Base<CosmosDocActivityLog> cosmosDBClientBase = new CosmosDBClient_Base<CosmosDocActivityLog>();
        private readonly string pkPrefix = ((int)DocTypeEnum.Activity).ToString();

        public async Task<bool> AddActivityLog(Guid UserID, string Description, MethodBase MB)
        {

            if (CosmosAPI.DoActivityLog)
            {

                try
                {
                    CosmosDocActivityLog newActivityLog = new CosmosDocActivityLog(UserID, Description, LocalFunctions.GetMethodName(MB));

                    await cosmosDBRepoActivityLog.CreateItemAsync(newActivityLog);

                    return true;

                }
                catch (CosmosException ex)
                {

                    await CosmosAPI.cosmosDBClientError.AddErrorLog(UserID, ex.Message, MethodBase.GetCurrentMethod());

                    return false;
                }
            }
            else
            {
                return true;
            }
        }


        public async Task<IEnumerable<CosmosDocActivityLog>> GetAll()
        {
            
            try
            {
                return await cosmosDBRepoActivityLog.GetItemsAsync(x => x.DocType == (int)DocTypeEnum.Activity);

            }
            catch (CosmosException ex)
            {

                await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, ex.Message, MethodBase.GetCurrentMethod());
                return null;
            }



        }


        public async Task<bool> Delete(CosmosDocActivityLog tsActivity)
        {
            return await cosmosDBClientBase.DeleteItemAsync(tsActivity, pkPrefix);
        }
    }
}
