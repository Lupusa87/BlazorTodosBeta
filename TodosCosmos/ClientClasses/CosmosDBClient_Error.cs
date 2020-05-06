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

        public async Task<bool> AddErrorLog(Guid UserID,
            string Description,
            List<string> CallTrace,
            bool DoNotNotifyAdmin = false)
        {

            CosmosDocErrorLog newErrorLog = new CosmosDocErrorLog(UserID, Description, LocalFunctions.GetCallTraceString(CallTrace));

            if (DoNotNotifyAdmin)
            {
                newErrorLog.IUD = 2;
            }

            await cosmosDBRepo.CreateItemAsync(newErrorLog, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

            return true;
        }



        public async Task<bool> DeleteErrorLog(DocDeleteModeEnum deleteMode, CosmosDocErrorLog tsErrorLog, List<string> CallTrace)
        {
            return await cosmosDBClientBase.DeleteItemAsync(deleteMode, tsErrorLog, pkPrefix, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
        }

        public async Task<bool> DeleteAllErrorLogs(DocDeleteModeEnum deleteMode, List<string> CallTrace)
        {
     
            try
            {
                IEnumerable<CosmosDocErrorLog> result = await cosmosDBRepo.GetItemsAsync(x => x.DocType == (int)DocTypeEnum.Error && x.IUD < 2, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

                if (result.Any())
                {
                    foreach (var item in result)
                    {
                        await cosmosDBClientBase.DeleteItemAsync(deleteMode, item, pkPrefix, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
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
