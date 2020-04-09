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


        public async Task<bool> Add(TSUILanguage tsUILanguage, List<string> CallTrace)
        {
            return await cosmosDBClientBase.AddItemAsync(new CosmosDocUILanguage(tsUILanguage), LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
        }


        public async Task<bool> Update(TSUILanguage tsUILanguage, List<string> CallTrace)
        {
            return await cosmosDBClientBase.UpdateItemAsync(new CosmosDocUILanguage(tsUILanguage), LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
        }

        public async Task<bool> Update(CosmosDocUILanguage tsUILanguage, List<string> CallTrace)
        {
            return await cosmosDBClientBase.UpdateItemAsync(tsUILanguage, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
        }


        public async Task<bool> Delete(TSUILanguage tsUILanguage, List<string> CallTrace)
        {
            return await cosmosDBClientBase.DeleteItemAsync(new CosmosDocUILanguage(tsUILanguage), pkPrefix, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
        }

        public async Task<TSUILanguage> Get(TSUILanguage tsUILanguage, List<string> CallTrace)
        {
            return (await cosmosDBClientBase.GetItemAsync(new CosmosDocUILanguage(tsUILanguage), pkPrefix, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()))).toTSUILanguage();

        }

        public async Task<List<TSUILanguage>> GetAll(List<string> CallTrace)
        {
            List<TSUILanguage> TsUILanguages = new List<TSUILanguage>();

            try
            {
                IEnumerable<CosmosDocUILanguage> result = await cosmosDBRepo.GetItemsAsync(x => x.DocType == (int)DocTypeEnum.UILanguage && x.IUD < 2,
                    LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

                foreach (var item in result)
                {
                    TsUILanguages.Add(item.toTSUILanguage());
                }


            }
            catch (CosmosException ex)
            {
                await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, ex.Message, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
            }


            return TsUILanguages;



        }

        public async Task<CosmosDocUILanguage> Find(string SearchCrtiteria, string column, List<string> CallTrace)
        {


            try
            {
                QueryDefinition sql = new QueryDefinition("SELECT * FROM c WHERE c.DocType = " + (int)DocTypeEnum.UILanguage + " and c." + column + "='" + SearchCrtiteria + "'");

                return await cosmosDBRepo.FindFirstItemsAsync(sql, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
            }
            catch (CosmosException ex)
            {
                await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, ex.Message, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

                return null;
            }


        }


        public async Task<CosmosDocUILanguage> FindByName(string Name, List<string> CallTrace)
        {
            return await cosmosDBRepo.FindFirstItemsAsync(x => x.DocType == (int)DocTypeEnum.UILanguage && x.IUD < 2 && 
            x.Name.ToLower() == Name.ToLower(),
            LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

        }
    }
}
