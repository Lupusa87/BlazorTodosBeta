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
    public class CosmosDBClient_Feedback
    {
        private readonly CosmosDBRepository<CosmosDocFeedback> cosmosDBRepo = new CosmosDBRepository<CosmosDocFeedback>();
        private readonly CosmosDBClient_Base<CosmosDocFeedback> cosmosDBClientBase = new CosmosDBClient_Base<CosmosDocFeedback>();
        private readonly string pkPrefix = ((int)DocTypeEnum.Feedback).ToString();


        public async Task<bool> AddFeedback(TSFeedback tsFeedback, List<string> CallTrace)
        {
            return await cosmosDBClientBase.AddItemAsync(new CosmosDocFeedback(tsFeedback), LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
        }


        public async Task<bool> UpdateFeedback(TSFeedback tsFeedback, List<string> CallTrace)
        {
            return await cosmosDBClientBase.UpdateItemAsync(new CosmosDocFeedback(tsFeedback), LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
        }

        public async Task<bool> UpdateFeedbackEntity(CosmosDocFeedback tsFeedback, List<string> CallTrace)
        {
            return await cosmosDBClientBase.UpdateItemAsync(tsFeedback, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
        }


        public async Task<bool> DeleteFeedback(DocDeleteModeEnum deleteMode, TSFeedback tsFeedback, List<string> CallTrace)
        {
            return await cosmosDBClientBase.DeleteItemAsync(deleteMode, new CosmosDocFeedback(tsFeedback), pkPrefix, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
        }

        public async Task<TSFeedback> GetFeedback(TSFeedback tsFeedback, List<string> CallTrace)
        {
            return (await cosmosDBClientBase.GetItemAsync(new CosmosDocFeedback(tsFeedback), pkPrefix, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()))).toTSFeedback();

        }

        public async Task<List<TSFeedback>> GetAllFeedback(List<string> CallTrace)
        {
            List<TSFeedback> TsFeedbacks = new List<TSFeedback>();

            try
            {
                IEnumerable<CosmosDocFeedback> result = await cosmosDBRepo.GetItemsAsync(x => x.DocType == (int)DocTypeEnum.Feedback && x.IUD < 2, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

                foreach (var item in result)
                {
                    TsFeedbacks.Add(item.toTSFeedback());
                }


            }
            catch (CosmosException ex)
            {
                await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, ex.Message, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
            }


            return TsFeedbacks;



        }

        public async Task<CosmosDocFeedback> FindFeedback(string SearchCrtiteria, string column, List<string> CallTrace)
        {


            try
            {
                QueryDefinition sql = new QueryDefinition("SELECT * FROM c WHERE c.DocType = " + (int)DocTypeEnum.Feedback + " and c." + column + "='" + SearchCrtiteria + "'");

                return await cosmosDBRepo.FindFirstItemsAsync(sql, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
            }
            catch (CosmosException ex)
            {
                await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, ex.Message, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

                return null;
            }

            
        }


        public async Task<CosmosDocFeedback> FindFeedback(Guid UserID, List<string> CallTrace)
        {

            return await cosmosDBRepo.FindFirstItemsAsync(x => x.DocType == (int)DocTypeEnum.Feedback && x.IUD < 2 &&
            x.UserID == UserID, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

        }

    }
}
