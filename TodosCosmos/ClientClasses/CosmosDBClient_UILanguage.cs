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
    public class CosmosDBClient_UILanguage
    {
        private readonly CosmosDBRepository<CosmosDocUILanguage> cosmosDBRepo = new CosmosDBRepository<CosmosDocUILanguage>();
        private readonly CosmosDBClient_Base<CosmosDocUILanguage> cosmosDBClientBase = new CosmosDBClient_Base<CosmosDocUILanguage>();
        private readonly string pkPrefix = ((int)DocTypeEnum.UILanguage).ToString();


        public async Task<bool> Add(TSUILanguage tsUILanguage)
        {
            return await cosmosDBClientBase.AddItemAsync(new CosmosDocUILanguage(tsUILanguage));
        }


        public async Task<bool> Update(TSUILanguage tsUILanguage)
        {
            return await cosmosDBClientBase.UpdateItemAsync(new CosmosDocUILanguage(tsUILanguage));
        }

        public async Task<bool> Update(CosmosDocUILanguage tsUILanguage)
        {
            return await cosmosDBClientBase.UpdateItemAsync(tsUILanguage);
        }


        public async Task<bool> Delete(TSUILanguage tsUILanguage)
        {
            return await cosmosDBClientBase.DeleteItemAsync(new CosmosDocUILanguage(tsUILanguage), pkPrefix);
        }

        public async Task<TSUILanguage> Get(TSUILanguage tsUILanguage)
        {
            return (await cosmosDBClientBase.GetItemAsync(new CosmosDocUILanguage(tsUILanguage), pkPrefix)).toTSUILanguage();

        }

        public async Task<List<TSUILanguage>> GetAll()
        {
            List<TSUILanguage> TsUILanguages = new List<TSUILanguage>();

            try
            {
                IEnumerable<CosmosDocUILanguage> result = await cosmosDBRepo.GetItemsAsync(x => x.DocType == (int)DocTypeEnum.UILanguage);

                foreach (var item in result)
                {
                    TsUILanguages.Add(item.toTSUILanguage());
                }


            }
            catch (CosmosException ex)
            {
                await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, ex.Message, MethodBase.GetCurrentMethod());
            }


            return TsUILanguages;



        }

        public async Task<CosmosDocUILanguage> Find(string SearchCrtiteria, string column)
        {


            try
            {
                QueryDefinition sql = new QueryDefinition("SELECT * FROM c WHERE c.DocType = " + (int)DocTypeEnum.UILanguage + " and c." + column + "='" + SearchCrtiteria + "'");

                return await cosmosDBRepo.FindFirstItemsAsync(sql);
            }
            catch (CosmosException ex)
            {
                await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, ex.Message, MethodBase.GetCurrentMethod());

                return null;
            }


        }


        public async Task<CosmosDocUILanguage> FindByName(string Name)
        {
            return await cosmosDBRepo.FindFirstItemsAsync(x => x.DocType == (int)DocTypeEnum.UILanguage &&
            x.Name.ToLower() == Name.ToLower());

        }
    }
}
