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


        public async Task<bool> AddTodo(TSTodo tsTodo)
        {

            return await cosmosDBClientBase.AddItemAsync(new CosmosDocTodo(tsTodo));
        }

        public async Task<bool> AddTodosBath(List<TSTodo> TsTodos)
        {

            if (TsTodos.Any())
            {
                List<CosmosDocTodo> items = new List<CosmosDocTodo>();
                foreach (var item in TsTodos)
                {
                    items.Add(new CosmosDocTodo(item));
                }
                return await cosmosDBClientBase.AddItemsBathAsync(items);
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> UpdateTodo(TSTodo tsTodo)
        {

                return await cosmosDBClientBase.UpdateItemAsync(new CosmosDocTodo(tsTodo));

        }

        public async Task<bool> UpdateTodoEntity(CosmosDocTodo tsTodo)
        {
            return await cosmosDBClientBase.UpdateItemAsync(tsTodo);
        }


        public async Task<bool> DeleteTodo(TSTodo tsTodo)
        {
            return await cosmosDBClientBase.DeleteItemAsync(new CosmosDocTodo(tsTodo), pkPrefix);
        }

        public async Task<TSTodo> GetTodo(TSTodo tsTodo)
        {

            return (await cosmosDBClientBase.GetItemAsync(new CosmosDocTodo(tsTodo), pkPrefix)).toTSTodo();

        }

        public async Task<List<TSTodo>> GetAllTodos(Guid UserID)
        {
            List<TSTodo> TsTodos = new List<TSTodo>();
            try
            {
                IEnumerable<CosmosDocTodo> result = await cosmosDBRepo.GetItemsAsync(x => x.DocType == (int)DocTypeEnum.Todo && x.UserID==UserID);

                foreach (var item in result)
                {
                    TsTodos.Add(item.toTSTodo());
                }

            }
            catch (CosmosException ex)
            {

                await CosmosAPI.cosmosDBClientError.AddErrorLog(UserID, ex.Message, MethodBase.GetCurrentMethod());

            }

            return TsTodos;

        }



        public async Task<bool> SendTodoReminders()
        {

            try
            {


                IEnumerable<CosmosDocTodo> result = await cosmosDBRepo.GetItemsAsync(x => x.DocType == (int)DocTypeEnum.Todo &&
                x.UserID != CosmosAPI.DemoUserID && !x.IsReminderEmailed &&
                x.HasRemindDate && x.RemindDate<DateTime.Now);



                if (result.Any())
                {
                    bool b = false;
                    string body = string.Empty;
                    foreach (var item in result)
                    {



                        TSUser currUser = await CosmosAPI.cosmosDBClientUser.GetUser(item.ID);

                        if (currUser != null)
                        {
                            item.IsReminderEmailed = true;


                            TSEmail tmpEmail = new TSEmail()
                            {
                                To = currUser.Email,
                                OperationCode = 4,
                            };

                            body = "Name - " + item.Name + "\n\nDescription - " + item.Description + "\n\nDuedate - " + item.DueDate.ToString("MM/dd/yyyy HH:mm:ss.fff") + ".";
                            tmpEmail = LocalFunctions.SendEmail(tmpEmail, string.Empty, body).Result;


                            b = UpdateTodoEntity(item).Result;
                        }


                    }
                }

            }
            catch (CosmosException ex)
            {

                await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, ex.Message, MethodBase.GetCurrentMethod());

                return false;
            }


            return true;



        }
        
    }
}
