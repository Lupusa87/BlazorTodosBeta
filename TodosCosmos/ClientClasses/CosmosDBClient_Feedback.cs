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


        public async Task<bool> AddFeedback(TSFeedback tsFeedback)
        {
            return await cosmosDBClientBase.AddItemAsync(new CosmosDocFeedback(tsFeedback));
        }


        public async Task<bool> UpdateFeedback(TSFeedback tsFeedback)
        {
            return await cosmosDBClientBase.UpdateItemAsync(new CosmosDocFeedback(tsFeedback));
        }

        public async Task<bool> UpdateFeedbackEntity(CosmosDocFeedback tsFeedback)
        {
            return await cosmosDBClientBase.UpdateItemAsync(tsFeedback);
        }


        public async Task<bool> DeleteFeedback(TSFeedback tsFeedback)
        {
            return await cosmosDBClientBase.DeleteItemAsync(new CosmosDocFeedback(tsFeedback), pkPrefix);
        }

        public async Task<TSFeedback> GetFeedback(TSFeedback tsFeedback)
        {
            return (await cosmosDBClientBase.GetItemAsync(new CosmosDocFeedback(tsFeedback), pkPrefix)).toTSFeedback();

        }

        public async Task<List<TSFeedback>> GetAllFeedback()
        {
            List<TSFeedback> TsFeedbacks = new List<TSFeedback>();

            try
            {
                IEnumerable<CosmosDocFeedback> result = await cosmosDBRepo.GetItemsAsync(x => x.DocType == (int)DocTypeEnum.Feedback);

                foreach (var item in result)
                {
                    TsFeedbacks.Add(item.toTSFeedback());
                }


            }
            catch (CosmosException ex)
            {
                await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, ex.Message, MethodBase.GetCurrentMethod());
            }


            return TsFeedbacks;



        }

        public async Task<CosmosDocFeedback> FindFeedback(string SearchCrtiteria, string column)
        {


            try
            {
                QueryDefinition sql = new QueryDefinition("SELECT * FROM c WHERE c.DocType = " + (int)DocTypeEnum.Feedback + " and c." + column + "='" + SearchCrtiteria + "'");

                return await cosmosDBRepo.FindFirstItemsAsync(sql);
            }
            catch (CosmosException ex)
            {
                await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, ex.Message, MethodBase.GetCurrentMethod());

                return null;
            }

            
        }


        public async Task<CosmosDocFeedback> FindFeedback(Guid UserID)
        {

            return await cosmosDBRepo.FindFirstItemsAsync(x => x.DocType == (int)DocTypeEnum.Feedback &&
            x.UserID == UserID);

        }

    }
}
