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


        public async Task<bool> AddReaction(TSReaction tsReaction, List<string> CallTrace)
        {
            return await cosmosDBClientBase.AddItemAsync(new CosmosDocReaction(tsReaction), LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
        }


        public async Task<bool> UpdateReaction(TSReaction tsReaction, List<string> CallTrace)
        {
            return await cosmosDBClientBase.UpdateItemAsync(new CosmosDocReaction(tsReaction), LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
        }

        public async Task<bool> UpdateReactionEntity(CosmosDocReaction tsReaction, List<string> CallTrace)
        {
            return await cosmosDBClientBase.UpdateItemAsync(tsReaction, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
        }


        public async Task<TSReaction> GetReaction(TSReaction tsReaction, List<string> CallTrace)
        {
            return (await cosmosDBClientBase.GetItemAsync(new CosmosDocReaction(tsReaction), pkPrefix, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()))).toTSReaction();

        }


        public async Task<CosmosDocReaction> FindReaction(string SearchCrtiteria, string column, List<string> CallTrace)
        {
            try
            {
                QueryDefinition sql = new QueryDefinition("SELECT * FROM c WHERE c.DocType = " + (int)DocTypeEnum.Reaction + " and c." + column + "='" + SearchCrtiteria + "'");

                return await cosmosDBRepo.FindFirstItemsAsync(sql, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
            }
            catch (CosmosException ex)
            {
                await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, ex.Message, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

                return null;
            }
        }

        public async Task<CosmosDocReaction> FindReaction(Guid UserID, List<string> CallTrace)
        {

            return await cosmosDBRepo.FindFirstItemsAsync(x => x.DocType == (int)DocTypeEnum.Reaction &&
            x.UserID == UserID, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

        }
    }
}
