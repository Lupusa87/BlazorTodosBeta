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
    public class CosmosDBClient_UIWordNative
    {
        private readonly CosmosDBRepository<CosmosDocUIWordNative> cosmosDBRepo = new CosmosDBRepository<CosmosDocUIWordNative>();
        private readonly CosmosDBClient_Base<CosmosDocUIWordNative> cosmosDBClientBase = new CosmosDBClient_Base<CosmosDocUIWordNative>();
        private readonly string pkPrefix = ((int)DocTypeEnum.UIWordNative).ToString();


        public async Task<bool> Add(TSUIWordNative tsUIWordNative, List<string> CallTrace)
        {
            return await cosmosDBClientBase.AddItemAsync(new CosmosDocUIWordNative(tsUIWordNative), LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
        }


        public async Task<bool> Update(TSUIWordNative tsUIWordNative, List<string> CallTrace)
        {
            return await cosmosDBClientBase.UpdateItemAsync(new CosmosDocUIWordNative(tsUIWordNative), LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
        }

        public async Task<bool> UpdateEntity(CosmosDocUIWordNative tsUIWordNative, List<string> CallTrace)
        {
            return await cosmosDBClientBase.UpdateItemAsync(tsUIWordNative, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
        }


        public async Task<bool> Delete(DocDeleteModeEnum deleteMode, TSUIWordNative tsUIWordNative, List<string> CallTrace)
        {
            return await cosmosDBClientBase.DeleteItemAsync(deleteMode, new CosmosDocUIWordNative(tsUIWordNative), pkPrefix, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
        }

        public async Task<TSUIWordNative> Get(TSUIWordNative tsUIWordNative, List<string> CallTrace)
        {
            return (await cosmosDBClientBase.GetItemAsync(new CosmosDocUIWordNative(tsUIWordNative), pkPrefix, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()))).toTSUIWordNative();

        }

        public async Task<List<TSUIWordNative>> GetAll(List<string> CallTrace)
        {
            List<TSUIWordNative> TsUIWordNatives = new List<TSUIWordNative>();

            try
            {
                IEnumerable<CosmosDocUIWordNative> result = await cosmosDBRepo.GetItemsAsync(x => x.DocType == (int)DocTypeEnum.UIWordNative && x.IUD < 2,
                    LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

                foreach (var item in result)
                {
                    TsUIWordNatives.Add(item.toTSUIWordNative());
                }


            }
            catch (CosmosException ex)
            {
                await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, ex.Message, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
            }


            return TsUIWordNatives;



        }

        public async Task<CosmosDocUIWordNative> Find(string SearchCrtiteria, string column, List<string> CallTrace)
        {


            try
            {
                QueryDefinition sql = new QueryDefinition("SELECT * FROM c WHERE c.DocType = " + (int)DocTypeEnum.UIWordNative + " and c." + column + "='" + SearchCrtiteria + "'");

                return await cosmosDBRepo.FindFirstItemsAsync(sql, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
            }
            catch (CosmosException ex)
            {
                await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, ex.Message, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

                return null;
            }


        }


        public async Task<CosmosDocUIWordNative> FindByWord(string Word, List<string> CallTrace)
        {
            return await cosmosDBRepo.FindFirstItemsAsync(x => x.DocType == (int)DocTypeEnum.UIWordNative && x.IUD < 2 &&
            x.Word.ToLower() == Word.ToLower(),
            LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

        }

    }
}
