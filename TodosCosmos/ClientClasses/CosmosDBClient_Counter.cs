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
    public class CosmosDBClient_Counter
    {
        private readonly CosmosDBRepository<CosmosDocCounter> cosmosDBRepo = new CosmosDBRepository<CosmosDocCounter>();
        private readonly CosmosDBClient_Base<CosmosDocCounter> cosmosDBClientBase = new CosmosDBClient_Base<CosmosDocCounter>();
        private readonly string pkPrefix = ((int)DocTypeEnum.Counter).ToString();


        public async Task<bool> AddCounter(CosmosDocCounter docCounter, List<string> CallTrace)
        {
            return await cosmosDBClientBase.AddItemAsync(docCounter, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
        }


        public async Task<bool> DeleteCounter(TSCounter tsCounter, List<string> CallTrace)
        {
            return await cosmosDBClientBase.DeleteItemAsync(new CosmosDocCounter(tsCounter), pkPrefix, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
        }


        public async Task<List<TSCounter>> GetNewestCounters(int Count, List<string> CallTrace)
        {
            List<TSCounter> TsCounters = new List<TSCounter>();

            try
            {
                IEnumerable<CosmosDocCounter> result = await cosmosDBRepo.TakeNewestItemsAsync(x => x.DocType == (int)DocTypeEnum.Counter && x.IUD < 2, x=>x.TimeStamp, Count, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

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



    }
}
