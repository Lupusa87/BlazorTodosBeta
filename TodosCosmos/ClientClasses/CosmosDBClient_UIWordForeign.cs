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
    public class CosmosDBClient_UIWordForeign
    {
        private readonly CosmosDBRepository<CosmosDocUIWordForeign> cosmosDBRepo = new CosmosDBRepository<CosmosDocUIWordForeign>();
        private readonly CosmosDBClient_Base<CosmosDocUIWordForeign> cosmosDBClientBase = new CosmosDBClient_Base<CosmosDocUIWordForeign>();
        private readonly string pkPrefix = ((int)DocTypeEnum.UIWordForeign).ToString();


        public async Task<bool> Add(TSUIWordForeign tsUIWordForeign, List<string> CallTrace)
        {
            return await cosmosDBClientBase.AddItemAsync(new CosmosDocUIWordForeign(tsUIWordForeign), LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
        }


        public async Task<bool> Update(TSUIWordForeign tsUIWordForeign, List<string> CallTrace)
        {
            return await cosmosDBClientBase.UpdateItemAsync(new CosmosDocUIWordForeign(tsUIWordForeign), LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
        }

        public async Task<bool> UpdateEntity(CosmosDocUIWordForeign tsUIWordForeign, List<string> CallTrace)
        {
            return await cosmosDBClientBase.UpdateItemAsync(tsUIWordForeign, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
        }


        public async Task<bool> Delete(TSUIWordForeign tsUIWordForeign, List<string> CallTrace)
        {
            return await cosmosDBClientBase.DeleteItemAsync(new CosmosDocUIWordForeign(tsUIWordForeign), pkPrefix, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
        }

        public async Task<TSUIWordForeign> Get(TSUIWordForeign tsUIWordForeign, List<string> CallTrace)
        {
            return (await cosmosDBClientBase.GetItemAsync(new CosmosDocUIWordForeign(tsUIWordForeign), pkPrefix, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()))).toTSUIWordForeign();

        }

        public async Task<List<TSUIWordForeign>> GetAll(List<string> CallTrace)
        {
            List<TSUIWordForeign> TsUIWordForeigns = new List<TSUIWordForeign>();

            try
            {
                IEnumerable<CosmosDocUIWordForeign> result = await cosmosDBRepo.GetItemsAsync(x => x.DocType == (int)DocTypeEnum.UIWordForeign && x.IUD < 2,
                    LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

                foreach (var item in result)
                {
                    TsUIWordForeigns.Add(item.toTSUIWordForeign());
                }


            }
            catch (CosmosException ex)
            {
                await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, ex.Message, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
            }


            return TsUIWordForeigns;



        }


        public async Task<List<TSUIWordForeign>> GetAllByLang(Guid LangID, List<string> CallTrace)
        {
            List<TSUIWordForeign> TsUIWordForeigns = new List<TSUIWordForeign>();

            try
            {
                IEnumerable<CosmosDocUIWordForeign> result = await cosmosDBRepo.GetItemsAsync(x => x.DocType == (int)DocTypeEnum.UIWordForeign && x.IUD < 2 && x.UILanguageID == LangID,
                    LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

                foreach (var item in result)
                {
                    TsUIWordForeigns.Add(item.toTSUIWordForeign());
                }


            }
            catch (CosmosException ex)
            {
                await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, ex.Message, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
            }


            return TsUIWordForeigns;



        }
      

        public async Task<CosmosDocUIWordForeign> Find(string SearchCrtiteria, string column, List<string> CallTrace)
        {


            try
            {
                QueryDefinition sql = new QueryDefinition("SELECT * FROM c WHERE c.DocType = " + (int)DocTypeEnum.UIWordForeign + " and c." + column + "='" + SearchCrtiteria + "'");

                return await cosmosDBRepo.FindFirstItemsAsync(sql, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
            }
            catch (CosmosException ex)
            {
                await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, ex.Message, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

                return null;
            }


        }


        public async Task<CosmosDocUIWordForeign> FindByWordAndNativeWordID(string Word, Guid NativeWordID, List<string> CallTrace)
        {
            return await cosmosDBRepo.FindFirstItemsAsync(x => x.DocType == (int)DocTypeEnum.UIWordForeign && x.IUD < 2 &&
            x.Word.ToLower() == Word.ToLower() && x.UIWordNativeID == NativeWordID,
            LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

        }
    }
}
