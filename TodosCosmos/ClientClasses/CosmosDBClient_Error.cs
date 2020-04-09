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
    public class CosmosDBClient_Error
    {


        private readonly CosmosDBRepository<CosmosDocErrorLog> cosmosDBRepo = new CosmosDBRepository<CosmosDocErrorLog>();
        private readonly CosmosDBClient_Base<CosmosDocErrorLog> cosmosDBClientBase = new CosmosDBClient_Base<CosmosDocErrorLog>();
        private readonly string pkPrefix = ((int)DocTypeEnum.Error).ToString();

        public async Task<bool> AddErrorLog(Guid UserID, string Description, List<string> CallTrace)
        {

            CosmosDocErrorLog newErrorLog = new CosmosDocErrorLog(UserID, Description, LocalFunctions.GetCallTraceString(CallTrace));

            await cosmosDBRepo.CreateItemAsync(newErrorLog, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

            return true;
        }



        public async Task<bool> DeleteErrorLog(CosmosDocErrorLog tsErrorLog, List<string> CallTrace)
        {
            return await cosmosDBClientBase.DeleteItemAsync(tsErrorLog, pkPrefix, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
        }

        public async Task<bool> DeleteAllErrorLogs(List<string> CallTrace)
        {
     
            try
            {
                IEnumerable<CosmosDocErrorLog> result = await cosmosDBRepo.GetItemsAsync(x => x.DocType == (int)DocTypeEnum.Error && x.IUD < 2, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

                if (result.Any())
                {
                    foreach (var item in result)
                    {
                        await cosmosDBClientBase.DeleteItemAsync(item, pkPrefix, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
                    }
                }

            }
            catch (CosmosException ex)
            {
              
                await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, ex.Message,LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

                return false;
            }


            return true;



        }


    }
}
