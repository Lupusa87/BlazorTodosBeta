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
    public class CosmosDBClient_Todo
    {

        private readonly CosmosDBRepository<CosmosDocTodo> cosmosDBRepo=new CosmosDBRepository<CosmosDocTodo>();
        private readonly CosmosDBClient_Base<CosmosDocTodo> cosmosDBClientBase = new CosmosDBClient_Base<CosmosDocTodo>();
        private readonly string pkPrefix = ((int)DocTypeEnum.Todo).ToString();


        public async Task<bool> AddTodo(TSTodo tsTodo, List<string> CallTrace)
        {

            bool b = await cosmosDBClientBase.AddItemAsync(new CosmosDocTodo(tsTodo), LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

            if (b)
            {
                if (tsTodo.Reminders.Any() && !tsTodo.IsDone)
                {
                    foreach (var item in tsTodo.Reminders)
                    {
                        await CosmosAPI.cosmosDBClientReminder.AddReminder(tsTodo.ID, item, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
                    }
                }
            }

            return b;
        }

       

        public async Task<bool> UpdateTodo(TSTodo tsTodo, List<string> CallTrace)
        {
            await CosmosAPI.cosmosDBClientReminder.DeleteTodosAllReminders(tsTodo.ID, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            bool b = await cosmosDBClientBase.UpdateItemAsync(new CosmosDocTodo(tsTodo), LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

            if (b)
            {
                if (tsTodo.Reminders.Any() && !tsTodo.IsDone)
                {
                    foreach (var item in tsTodo.Reminders)
                    {
                        await CosmosAPI.cosmosDBClientReminder.AddReminder(tsTodo.ID, item, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
                    }
                }
            }

            return b;
        }

        public async Task<bool> UpdateTodoEntity(CosmosDocTodo tsTodo, List<string> CallTrace)
        {
            return await cosmosDBClientBase.UpdateItemAsync(tsTodo, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
        }


        public async Task<bool> DeleteTodo(TSTodo tsTodo, List<string> CallTrace)
        {
            await CosmosAPI.cosmosDBClientReminder.DeleteTodosAllReminders(tsTodo.ID, LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            return await cosmosDBClientBase.DeleteItemAsync(new CosmosDocTodo(tsTodo), pkPrefix, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
        }

        public async Task<TSTodo> GetTodo(TSTodo tsTodo, List<string> CallTrace)
        {

            TSTodo result = (await cosmosDBClientBase.GetItemAsync(new CosmosDocTodo(tsTodo), pkPrefix, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()))).toTSTodo();

            IEnumerable<CosmosDocReminder> reminders = await CosmosAPI.cosmosDBClientReminder.GetTodosAllReminders(result.ID, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
            
            if (reminders.Any())
            {
                foreach (var item in reminders.OrderBy(x=>x.RemindDate))
                {
                    result.Reminders.Add(item.RemindDate);
                }
                
            }

            return result;
        }

        public async Task<List<TSTodo>> GetAllTodos(Guid UserID, List<string> CallTrace)
        {
            List<TSTodo> TsTodos = new List<TSTodo>();
            try
            {
                IEnumerable<CosmosDocTodo> result = await cosmosDBRepo.GetItemsAsync(x => x.DocType == (int)DocTypeEnum.Todo && x.IUD < 2 && x.UserID==UserID,
                    LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

                foreach (var item in result)
                {

                    IEnumerable<CosmosDocReminder> reminders = await CosmosAPI.cosmosDBClientReminder.GetTodosAllReminders(item.ID, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

                    if (reminders.Any())
                    {
                        foreach (var item2 in reminders.OrderBy(x=>x.RemindDate))
                        {
                            item.Reminders.Add(item2.RemindDate);
                        }
                    }

                    TsTodos.Add(item.toTSTodo());
                }

            }
            catch (CosmosException ex)
            {

                await CosmosAPI.cosmosDBClientError.AddErrorLog(UserID, ex.Message, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

            }

            return TsTodos;

        }



        public async Task<CosmosDocTodo> FindTodoByID(Guid id, List<string> CallTrace)
        {
            string pkvalue = PartitionKeyGenerator.Create(pkPrefix, id.ToString());
            return await cosmosDBRepo.FindFirstItemsAsync(x => x.DocType == (int)DocTypeEnum.Todo && x.IUD < 2 &&
            x.ID == id && x.PK == pkvalue,
            LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

        }
    }
}
