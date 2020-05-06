using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TodosCosmos.DocumentClasses;
using static TodosCosmos.Enums;

namespace TodosCosmos.ClientClasses
{
    public class CosmosDBClient_Base<T> where T : class
    {

        private readonly CosmosDBRepository<T> cosmosDBRepo = new CosmosDBRepository<T>();

        public async Task<bool> AddItemAsync(T newItem, List<string> CallTrace)
        {
            try
            {
               

                await cosmosDBRepo.CreateItemAsync(newItem, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));


                return true;

            }
            catch (CosmosException ex)
            {


                await CosmosAPI.cosmosDBClientError.AddErrorLog(GetProperty(newItem, "UserID", LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod())), ex.Message, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

                return false;
            }
        }


        public async Task<bool> AddItemsBathAsync(List<T> Items, List<string> CallTrace)
        {
            try
            {

                foreach (var item in Items)
                {

                    await cosmosDBRepo.CreateItemAsync(item, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
                }



                return true;

            }
            catch (CosmosException ex)
            {


                await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, ex.Message, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

                return false;
            }
        }

        public async Task<bool> UpdateItemAsync(T item, List<string> CallTrace)
        {
            try
            {

                await cosmosDBRepo.UpdateItemAsync(item, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

                return true;
            }
            catch (CosmosException ex)
            {
                await CosmosAPI.cosmosDBClientError.AddErrorLog(GetProperty(item, "UserID", LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod())), ex.Message, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

                return false;
            }
        }


        public async Task<bool> DeleteItemAsync(DocDeleteModeEnum deleteMode, T item, string pkPrefix, List<string> CallTrace)
        {
            try
            {
                string id = GetProperty(item, "ID", LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod())).ToString();


                string pkValue = PartitionKeyGenerator.Create(pkPrefix, id.ToString());

                T deleteItem = await cosmosDBRepo.GetItemAsync(id,pkValue, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));


                if (deleteItem == null)
                {
                    return false;
                }
                else
                {
                    if (deleteMode == DocDeleteModeEnum.Soft)
                    {
                        await cosmosDBRepo.SoftDeleteItemAsync(id, pkValue, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
                    }
                    else
                    {
                        await cosmosDBRepo.DeleteItemAsync(id, pkValue, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
                    }
                    return true;
                }

            }
            catch (CosmosException ex)
            {


                await CosmosAPI.cosmosDBClientError.AddErrorLog(GetProperty(item, "UserID", LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod())), ex.Message, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

                return false;
            }
        }




        public async Task<T> GetItemAsync(T item, string pkPrefix, List<string> CallTrace)
        {

            try
            {

                string id = GetProperty(item, "ID", LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod())).ToString();

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

                return await cosmosDBRepo.GetItemAsync(id, pkValue, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

            }
            catch (CosmosException ex)
            {
                await CosmosAPI.cosmosDBClientError.AddErrorLog(GetProperty(item, "UserID", LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod())), ex.Message, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

                return null;
            }
        }


        public async Task<T> GetItemAsync(Guid id,Guid UserID, string pkPrefix, List<string> CallTrace)
        {

            try
            {

                string pkValue = PartitionKeyGenerator.Create(pkPrefix, id.ToString());

                return await cosmosDBRepo.GetItemAsync(id.ToString(), pkValue, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

            }
            catch (CosmosException ex)
            {
                

                await CosmosAPI.cosmosDBClientError.AddErrorLog(UserID, ex.Message, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

                return null;
            }
        }


        public Guid GetProperty(T item, string Prop, List<string> CallTrace)
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
