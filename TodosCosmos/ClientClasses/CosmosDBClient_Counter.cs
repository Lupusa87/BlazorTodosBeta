using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TodosCosmos.DocumentClasses;
using TodosShared;
using static TodosCosmos.Enums;

namespace TodosCosmos.ClientClasses
{
    public class CosmosDBClient_Counter
    {
        private readonly CosmosDBRepository<CosmosDocCounter> cosmosDBRepo = new CosmosDBRepository<CosmosDocCounter>();
        private readonly CosmosDBClient_Base<CosmosDocCounter> cosmosDBClientBase = new CosmosDBClient_Base<CosmosDocCounter>();
        private readonly string pkPrefix = ((int)DocTypeEnum.Counter).ToString();


        public async Task<bool> AddCounter(CosmosDocCounter docCounter, List<string> CallTrace)
        {
            return await cosmosDBClientBase.AddItemAsync(docCounter, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
        }


        public async Task<bool> DeleteCounter(DocDeleteModeEnum deleteMode, TSCounter tsCounter, List<string> CallTrace)
        {
            return await cosmosDBClientBase.DeleteItemAsync(deleteMode, new CosmosDocCounter(tsCounter), pkPrefix, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
        }



        public async Task<bool> DeleteAllCounters(DocDeleteModeEnum deleteMode, List<string> CallTrace)
        {

            try
            {
                IEnumerable<CosmosDocCounter> result = await cosmosDBRepo.GetItemsAsync(x => x.DocType == (int)DocTypeEnum.Counter && x.IUD < 2, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

                if (result.Any())
                {
                    foreach (var item in result)
                    {
                        await cosmosDBClientBase.DeleteItemAsync(deleteMode, item, pkPrefix, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
                    }
                }

            }
            catch (CosmosException ex)
            {

                await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, ex.Message, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

                return false;
            }


            return true;



        }


        public async Task<List<TSCounter>> GetNewestCounters(int Count, List<string> CallTrace)
        {
            List<TSCounter> TsCounters = new List<TSCounter>();

            try
            {
                List<CosmosDocCounter> result = await cosmosDBRepo.TakeNewestItemsAsync(x => x.DocType == (int)DocTypeEnum.Counter && x.IUD < 2, x=>x.TimeStamp, Count, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

                foreach (var item in result)
                {
                    TsCounters.Add(item.toTSCounter());
                }


            }
            catch (CosmosException ex)
            {
                await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, ex.Message, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
            }


            return TsCounters;



        }

        public async Task<List<TSReport1>> GetReport1(long fromDate, long toDate, List<string> CallTrace)
        {
            try
            {
                //https://docs.microsoft.com/en-us/dotnet/api/microsoft.azure.cosmos.container.getitemqueryiterator?view=azure-dotnet
                QueryDefinition query = new QueryDefinition("SELECT c.q as w, c.w as e, COUNT(1) AS r" +
                                " FROM c where c.iud<2 and c.dt=15 and c._ts>=@var1 and c._ts<@var2" +
                                " GROUP BY c.q, c.w")
                                .WithParameter("@var1",fromDate)
                                .WithParameter("@var2",toDate);

                CosmosDBRepository<TSReport1> cosmosDBRepoReport1 = new CosmosDBRepository<TSReport1>();

                return await cosmosDBRepoReport1.ExecuteQueryAsync(query, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

            }
            catch (CosmosException ex)
            {
                await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, ex.Message, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
            }

            return new List<TSReport1>();

        }

    }
}
