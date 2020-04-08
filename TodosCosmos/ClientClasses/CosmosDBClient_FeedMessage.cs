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
    public class CosmosDBClient_FeedMessage
    {
        private readonly CosmosDBRepository<CosmosDocFeedMessage> cosmosDBRepo = new CosmosDBRepository<CosmosDocFeedMessage>();
        private readonly CosmosDBClient_Base<CosmosDocFeedMessage> cosmosDBClientBase = new CosmosDBClient_Base<CosmosDocFeedMessage>();
        private readonly string pkPrefix = ((int)DocTypeEnum.FeedMessage).ToString();

        public async Task<bool> AddFeedMessage(RequestedActionEnum requestedAction, string bag, List<string> CallTrace)
        {

            CosmosDocFeedMessage newFeedMessage = new CosmosDocFeedMessage(requestedAction, bag);

            await cosmosDBRepo.CreateItemAsync(newFeedMessage, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

            return true;
        }



        public async Task<bool> DeleteFeedMessage(CosmosDocFeedMessage tsFeedMessage, List<string> CallTrace)
        {
            return await cosmosDBClientBase.DeleteItemAsync(tsFeedMessage, pkPrefix, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
        }

        public async Task<bool> DeleteAllFeedMessages(List<string> CallTrace)
        {

            try
            {
                IEnumerable<CosmosDocFeedMessage> result = await cosmosDBRepo.GetItemsAsync(x => x.DocType == (int)DocTypeEnum.FeedMessage, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

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

                await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, ex.Message, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

                return false;
            }


            return true;



        }

    }
}
