using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using TodosCosmos.DocumentClasses;
using static TodosCosmos.Enums;

namespace TodosCosmos.ClientClasses
{
    public class CosmosDBRepository<T> where T : class
    {

        public async Task<T> GetItemAsync(string id, string pk, List<string> CallTrace)
        {
            return await CosmosAPI.container.ReadItemAsync<T>(id, new PartitionKey(pk));
        }

        public async Task<IEnumerable<T>> GetItemsAsync(List<string> CallTrace)
        {
            List<T> result = new List<T>();

            var setIterator = CosmosAPI.container.GetItemLinqQueryable<T>().ToFeedIterator();


            while (setIterator.HasMoreResults)
            {

                result.AddRange(await setIterator.ReadNextAsync());
            }

            return result.AsEnumerable();
        }

        public async Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate, List<string> CallTrace)
        {
            List<T> result = new List<T>();

            var setIterator = CosmosAPI.container.GetItemLinqQueryable<T>().Where(predicate).ToFeedIterator();


            while (setIterator.HasMoreResults)
            {

                result.AddRange(await setIterator.ReadNextAsync());
            }

            return result.AsEnumerable();
        }


        public async Task<T> FindFirstItemsAsync(Expression<Func<T, bool>> predicate, List<string> CallTrace)
        {
            List<T> result = new List<T>();


            try
            {

                var feedIterator = CosmosAPI.container.GetItemLinqQueryable<T>().Where(predicate).ToFeedIterator();

                while (feedIterator.HasMoreResults)
                {
                    result.AddRange(await feedIterator.ReadNextAsync());
                }

            }
            catch (CosmosException ex)
            {

                await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, ex.Message, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

            }

     
            if (result.Count() > 0)
            {
              
                return result.AsEnumerable().FirstOrDefault();
            }
            else
            {
               
                return null;
            }

            
        }


        public async Task<T> FindLastItemsAsync(Expression<Func<T, bool>> predicate, List<string> CallTrace)
        {
            List<T> result = new List<T>();


            try
            {

                var feedIterator = CosmosAPI.container.GetItemLinqQueryable<T>().Where(predicate).ToFeedIterator();

                while (feedIterator.HasMoreResults)
                {
                    result.AddRange(await feedIterator.ReadNextAsync());
                }

            }
            catch (CosmosException ex)
            {

                await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, ex.Message, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

            }


            if (result.Count() > 0)
            {

                return result.AsEnumerable().LastOrDefault();
            }
            else
            {

                return null;
            }


        }


        public async Task<T> FindFirstItemsAsync(QueryDefinition sqlQuery, List<string> CallTrace)
        {
            List<T> result = new List<T>();

            FeedIterator<T> feedIterator = CosmosAPI.container.GetItemQueryIterator<T>(sqlQuery);

            while (feedIterator.HasMoreResults)
            {
                result.AddRange(await feedIterator.ReadNextAsync());
            }


            if (result.Count() > 0)
            {
                return result.AsEnumerable().FirstOrDefault();
            }
            else
            {
                return null;
            }


        }


        public async Task<IEnumerable<T>> GetItemsAsync(QueryDefinition sqlQuery, List<string> CallTrace)
        {

            List<T> result = new List<T>();

            FeedIterator<T> feedIterator = CosmosAPI.container.GetItemQueryIterator<T>(sqlQuery);

            while (feedIterator.HasMoreResults)
            {
                result.AddRange(await feedIterator.ReadNextAsync());
            }


            if (result.Any())
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        

        public async Task<T> CreateItemAsync(T item, List<string> CallTrace)
        {

            return await CosmosAPI.container.CreateItemAsync(item);

        }

        public async Task<T> UpdateItemAsync(T item, List<string> CallTrace)
        {
            PropertyInfo pi = item.GetType().GetProperty("IUD");

            if (pi is object)
            {
                byte iud = (byte)pi.GetValue(item);

                if (iud == 0)
                {
                    pi.SetValue(item, (byte)1);
                }
            }

            return await CosmosAPI.container.UpsertItemAsync(item);
        }

      


        public async Task SoftDeleteItemAsync(string id, string pk, List<string> CallTrace)
        {

            T d = await CosmosAPI.container.ReadItemAsync<T>(id, new PartitionKey(pk));


            PropertyInfo pi_IUD = d.GetType().GetProperty("IUD");

            if (pi_IUD is object)
            {
                pi_IUD.SetValue(d, (byte)DocStateMarkEnum.PreDelete);
            }

            await CosmosAPI.container.UpsertItemAsync(d);
        }

        public async Task DeleteItemAsync(string id, string pk, List<string> CallTrace)
        {
            await CosmosAPI.container.DeleteItemAsync<T>(id, new PartitionKey(pk));
        }


        public int GetCount(Expression<Func<T, bool>> predicate, List<string> CallTrace)
        {

            return CosmosAPI.container.GetItemLinqQueryable<T>().Where(predicate).AsEnumerable().Count();

        }

    }
}
