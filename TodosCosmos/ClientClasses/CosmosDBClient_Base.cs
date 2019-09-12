using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TodosCosmos.DocumentClasses;

namespace TodosCosmos.ClientClasses
{
    public class CosmosDBClient_Base<T> where T : class
    {

        private readonly CosmosDBRepository<T> cosmosDBRepo = new CosmosDBRepository<T>();

        public async Task<bool> AddItemAsync(T newItem)
        {
            try
            {
               

                await cosmosDBRepo.CreateItemAsync(newItem);


                return true;

            }
            catch (CosmosException ex)
            {


                await CosmosAPI.cosmosDBClientError.AddErrorLog(GetProperty(newItem, "UserID"), ex.Message, MethodBase.GetCurrentMethod());

                return false;
            }
        }


        public async Task<bool> AddItemsBathAsync(List<T> Items)
        {
            try
            {

                foreach (var item in Items)
                {

                    await cosmosDBRepo.CreateItemAsync(item);
                }



                return true;

            }
            catch (CosmosException ex)
            {


                await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, ex.Message, MethodBase.GetCurrentMethod());

                return false;
            }
        }

        public async Task<bool> UpdateItemAsync(T item)
        {
            try
            {

                await cosmosDBRepo.UpdateItemAsync(item);

                return true;
            }
            catch (CosmosException ex)
            {
                await CosmosAPI.cosmosDBClientError.AddErrorLog(GetProperty(item, "UserID"), ex.Message, MethodBase.GetCurrentMethod());

                return false;
            }
        }


        public async Task<bool> DeleteItemAsync(T item, string pkPrefix)
        {
            try
            {
                string id = GetProperty(item, "ID").ToString();


                string pkValue = PartitionKeyGenerator.Create(pkPrefix, id.ToString());

                T deleteItem = await cosmosDBRepo.GetItemAsync(id,pkValue);


                if (deleteItem == null)
                {
                    return false;
                }
                else
                {
                    await cosmosDBRepo.DeleteItemAsync(id, pkValue);

                    return true;
                }

            }
            catch (CosmosException ex)
            {


                await CosmosAPI.cosmosDBClientError.AddErrorLog(GetProperty(item, "UserID"), ex.Message, MethodBase.GetCurrentMethod());

                return false;
            }
        }




        public async Task<T> GetItemAsync(T item, string pkPrefix)
        {

            try
            {

                string id = GetProperty(item, "ID").ToString();

                try
                {
                    Guid g = Guid.Parse(id);
                    if (g.Equals(Guid.Empty))
                    {
                        throw new ArgumentException("ID is empty!");
                    }

                }
                catch (Exception)
                {

                    throw;
                }
               


                string pkValue = PartitionKeyGenerator.Create(pkPrefix, id.ToString());

                return await cosmosDBRepo.GetItemAsync(id, pkValue);

            }
            catch (CosmosException ex)
            {
                await CosmosAPI.cosmosDBClientError.AddErrorLog(GetProperty(item, "UserID"), ex.Message, MethodBase.GetCurrentMethod());

                return null;
            }
        }


        public async Task<T> GetItemAsync(Guid id,Guid UserID, string pkPrefix)
        {

            try
            {

                string pkValue = PartitionKeyGenerator.Create(pkPrefix, id.ToString());

                return await cosmosDBRepo.GetItemAsync(id.ToString(), pkValue);

            }
            catch (CosmosException ex)
            {
                await CosmosAPI.cosmosDBClientError.AddErrorLog(UserID, ex.Message, MethodBase.GetCurrentMethod());

                return null;
            }
        }


        public Guid GetProperty(T item, string Prop)
        {

            if (item is CosmosDocUser)
            {
                return (Guid)item.GetType().GetProperty("ID").GetValue(item);
            }
            else
            {
                return (Guid)item.GetType().GetProperty(Prop).GetValue(item);
            }



        }
    }
}
