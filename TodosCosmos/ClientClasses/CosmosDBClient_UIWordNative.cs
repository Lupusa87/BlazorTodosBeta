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


        public async Task<bool> Add(TSUIWordNative tsUIWordNative)
        {
            return await cosmosDBClientBase.AddItemAsync(new CosmosDocUIWordNative(tsUIWordNative));
        }


        public async Task<bool> Update(TSUIWordNative tsUIWordNative)
        {
            return await cosmosDBClientBase.UpdateItemAsync(new CosmosDocUIWordNative(tsUIWordNative));
        }

        public async Task<bool> UpdateEntity(CosmosDocUIWordNative tsUIWordNative)
        {
            return await cosmosDBClientBase.UpdateItemAsync(tsUIWordNative);
        }


        public async Task<bool> Delete(TSUIWordNative tsUIWordNative)
        {
            return await cosmosDBClientBase.DeleteItemAsync(new CosmosDocUIWordNative(tsUIWordNative), pkPrefix);
        }

        public async Task<TSUIWordNative> Get(TSUIWordNative tsUIWordNative)
        {
            return (await cosmosDBClientBase.GetItemAsync(new CosmosDocUIWordNative(tsUIWordNative), pkPrefix)).toTSUIWordNative();

        }

        public async Task<List<TSUIWordNative>> GetAll()
        {
            List<TSUIWordNative> TsUIWordNatives = new List<TSUIWordNative>();

            try
            {
                IEnumerable<CosmosDocUIWordNative> result = await cosmosDBRepo.GetItemsAsync(x => x.DocType == (int)DocTypeEnum.UIWordNative);

                foreach (var item in result)
                {
                    TsUIWordNatives.Add(item.toTSUIWordNative());
                }


            }
            catch (CosmosException ex)
            {
                await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, ex.Message, MethodBase.GetCurrentMethod());
            }


            return TsUIWordNatives;



        }

        public async Task<CosmosDocUIWordNative> Find(string SearchCrtiteria, string column)
        {


            try
            {
                QueryDefinition sql = new QueryDefinition("SELECT * FROM c WHERE c.DocType = " + (int)DocTypeEnum.UIWordNative + " and c." + column + "='" + SearchCrtiteria + "'");

                return await cosmosDBRepo.FindFirstItemsAsync(sql);
            }
            catch (CosmosException ex)
            {
                await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, ex.Message, MethodBase.GetCurrentMethod());

                return null;
            }


        }


        public async Task<CosmosDocUIWordNative> FindByWord(string Word)
        {
            return await cosmosDBRepo.FindFirstItemsAsync(x => x.DocType == (int)DocTypeEnum.UIWordNative &&
            x.Word.ToLower() == Word.ToLower());

        }

    }
}
