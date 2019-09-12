using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TodosCosmos.DocumentClasses;
using TodosShared;
using static TodosCosmos.Enums;

namespace TodosCosmos.ClientClasses
{
    public class CosmosDBClient_Reaction
    {
        private readonly CosmosDBRepository<CosmosDocReaction> cosmosDBRepo = new CosmosDBRepository<CosmosDocReaction>();
        private readonly CosmosDBClient_Base<CosmosDocReaction> cosmosDBClientBase = new CosmosDBClient_Base<CosmosDocReaction>();
        


        private readonly string pkPrefix = ((int)DocTypeEnum.Reaction).ToString();


        public async Task<bool> AddReaction(TSReaction tsReaction)
        {
            return await cosmosDBClientBase.AddItemAsync(new CosmosDocReaction(tsReaction));
        }


        public async Task<bool> UpdateReaction(TSReaction tsReaction)
        {
            return await cosmosDBClientBase.UpdateItemAsync(new CosmosDocReaction(tsReaction));
        }

        public async Task<bool> UpdateReactionEntity(CosmosDocReaction tsReaction)
        {
            return await cosmosDBClientBase.UpdateItemAsync(tsReaction);
        }


        public async Task<TSReaction> GetReaction(TSReaction tsReaction)
        {
            return (await cosmosDBClientBase.GetItemAsync(new CosmosDocReaction(tsReaction), pkPrefix)).toTSReaction();

        }


        public async Task<CosmosDocReaction> FindReaction(string SearchCrtiteria, string column)
        {
            try
            {
                QueryDefinition sql = new QueryDefinition("SELECT * FROM c WHERE c.DocType = " + (int)DocTypeEnum.Reaction + " and c." + column + "='" + SearchCrtiteria + "'");

                return await cosmosDBRepo.FindFirstItemsAsync(sql);
            }
            catch (CosmosException ex)
            {
                await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, ex.Message, MethodBase.GetCurrentMethod());

                return null;
            }
        }

        public async Task<CosmosDocReaction> FindReaction(Guid UserID)
        {

            return await cosmosDBRepo.FindFirstItemsAsync(x => x.DocType == (int)DocTypeEnum.Reaction &&
            x.UserID == UserID);

        }
    }
}
