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

        public async Task<bool> AddCategory(TSCategory tsCategory, List<string> CallTrace)
        {

            return await cosmosDBClientBase.AddItemAsync(new CosmosDocCategory(tsCategory), LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
            
        }



        public async Task<bool> UpdateCategory(TSCategory tsCategory, List<string> CallTrace)
        {

            return await cosmosDBClientBase.UpdateItemAsync(new CosmosDocCategory(tsCategory), LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
        }


        public async Task<bool> DeleteCategory(DocDeleteModeEnum deleteMode, TSCategory tsCategory, List<string> CallTrace)
        {
            return await cosmosDBClientBase.DeleteItemAsync(deleteMode, new CosmosDocCategory(tsCategory), pkPrefix, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
        }

        public async Task<TSCategory> GetCategory(TSCategory tsCategory, List<string> CallTrace)
        {
            return (await cosmosDBClientBase.GetItemAsync(new CosmosDocCategory(tsCategory), pkPrefix, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()))).toTSCategory();

        }

        public async Task<List<TSCategory>> GetAllCategories(Guid UserID, List<string> CallTrace)
        {



            List<TSCategory> TsCategorys = new List<TSCategory>();
            try
            {
                IEnumerable<CosmosDocCategory> result = await cosmosDBRepo.GetItemsAsync(x => x.DocType == (int)DocTypeEnum.Category && x.IUD < 2 && x.UserID == UserID, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

                foreach (var item in result)
                {
                    TsCategorys.Add(item.toTSCategory());
                }

            }
            catch (CosmosException ex)
            {

                await CosmosAPI.cosmosDBClientError.AddErrorLog(UserID, ex.Message, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

            }

            return TsCategorys;

        }

        public async Task<bool> AddDefaultCategory(Guid UserID, List<string> CallTrace)
        {
            TSCategory tsCategory = new TSCategory();
            tsCategory.ID = Guid.NewGuid();
            tsCategory.UserID = UserID;
            tsCategory.Name = "default";

            return await AddCategory(tsCategory, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
        }
      
    }
}
