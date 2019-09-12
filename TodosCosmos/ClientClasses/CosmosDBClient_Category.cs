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
    public class CosmosDBClient_Category
    {

        private readonly CosmosDBRepository<CosmosDocCategory> cosmosDBRepo = new CosmosDBRepository<CosmosDocCategory>();
        private readonly CosmosDBClient_Base<CosmosDocCategory> cosmosDBClientBase = new CosmosDBClient_Base<CosmosDocCategory>();
        private readonly string pkPrefix = ((int)DocTypeEnum.Category).ToString();

        public async Task<bool> AddCategory(TSCategory tsCategory)
        {

            return await cosmosDBClientBase.AddItemAsync(new CosmosDocCategory(tsCategory));
            
        }



        public async Task<bool> UpdateCategory(TSCategory tsCategory)
        {

            return await cosmosDBClientBase.UpdateItemAsync(new CosmosDocCategory(tsCategory));
        }


        public async Task<bool> DeleteCategory(TSCategory tsCategory)
        {
            return await cosmosDBClientBase.DeleteItemAsync(new CosmosDocCategory(tsCategory), pkPrefix);
        }

        public async Task<TSCategory> GetCategory(TSCategory tsCategory)
        {
            return (await cosmosDBClientBase.GetItemAsync(new CosmosDocCategory(tsCategory), pkPrefix)).toTSCategory();

        }

        public async Task<List<TSCategory>> GetAllCategories(Guid UserID)
        {



            List<TSCategory> TsCategorys = new List<TSCategory>();
            try
            {
                IEnumerable<CosmosDocCategory> result = await cosmosDBRepo.GetItemsAsync(x => x.DocType == (int)DocTypeEnum.Category && x.UserID == UserID);

                foreach (var item in result)
                {
                    TsCategorys.Add(item.toTSCategory());
                }

            }
            catch (CosmosException ex)
            {

                await CosmosAPI.cosmosDBClientError.AddErrorLog(UserID, ex.Message, MethodBase.GetCurrentMethod());

            }

            return TsCategorys;

        }

        public async Task<bool> AddDefaultCategory(Guid UserID)
        {
            TSCategory tsCategory = new TSCategory();
            tsCategory.ID = Guid.NewGuid();
            tsCategory.UserID = UserID;
            tsCategory.Name = "default";

            return await AddCategory(tsCategory);
        }
      
    }
}
