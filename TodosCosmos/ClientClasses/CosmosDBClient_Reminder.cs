using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TodosCosmos.DocumentClasses;
using TodosGlobal;
using TodosShared;
using static TodosCosmos.Enums;

namespace TodosCosmos.ClientClasses
{
    public class CosmosDBClient_Reminder
    {
        private readonly CosmosDBRepository<CosmosDocReminder> cosmosDBRepo = new CosmosDBRepository<CosmosDocReminder>();
        private readonly CosmosDBClient_Base<CosmosDocReminder> cosmosDBClientBase = new CosmosDBClient_Base<CosmosDocReminder>();
        private readonly string pkPrefix = ((int)DocTypeEnum.Reminder).ToString();

        public async Task<bool> AddReminder(Guid todoID, DateTime remindDate, List<string> CallTrace)
        {

            CosmosDocReminder newReminder = new CosmosDocReminder(todoID, remindDate);

            await cosmosDBRepo.CreateItemAsync(newReminder, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

            return true;
        }



        public async Task<bool> DeleteReminder(CosmosDocReminder reminder, List<string> CallTrace)
        {
            return await cosmosDBClientBase.DeleteItemAsync(reminder, pkPrefix, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
        }

        public async Task<bool> DeleteTodosAllReminders(Guid todoID, List<string> CallTrace)
        {

            try
            {
                IEnumerable<CosmosDocReminder> result = await cosmosDBRepo.GetItemsAsync(x => x.DocType == (int)DocTypeEnum.Reminder && x.IUD < 2 && x.TodoID == todoID, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

                if (result.Any())
                {
                    foreach (var item in result)
                    {
                        await DeleteReminder(item, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
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



        public async Task<IEnumerable<CosmosDocReminder>> GetTodosAllReminders(Guid todoID, List<string> CallTrace)
        {
            IEnumerable<CosmosDocReminder> cosmosDocReminders = new List<CosmosDocReminder>();

            try
            {
                cosmosDocReminders = await cosmosDBRepo.GetItemsAsync(x => x.DocType == (int)DocTypeEnum.Reminder && x.IUD < 2 && x.TodoID == todoID,
                    LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

            }
            catch (CosmosException ex)
            {
                await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, ex.Message, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
            }


            return cosmosDocReminders;

        }


        public async Task<bool> SendTodoReminders(List<string> CallTrace)
        {

            try
            {


                IEnumerable<CosmosDocReminder> result = await cosmosDBRepo.GetItemsAsync(x => x.DocType == (int)DocTypeEnum.Reminder && x.IUD < 2
                && x.IUD!=(byte)DocStateMarkEnum.PreDelete && x.IUD != (byte)DocStateMarkEnum.PostDelete && x.RemindDate < DateTime.Now,
                LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));



                if (result.Any())
                {
                  
                    string body = string.Empty;
                    foreach (var item in result)
                    {


                        CosmosDocTodo currTodo = await CosmosAPI.cosmosDBClientTodo.FindTodoByID(item.TodoID, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
                        CosmosDocUser currUser = await CosmosAPI.cosmosDBClientUser.FindUserByID(currTodo.UserID, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

                        if (currUser != null)
                        {
                            if (!currTodo.IsDone)
                            {
                                if (currUser.ID != CosmosAPI.DemoUserID)
                                {
                                    TSEmail tmpEmail = new TSEmail()
                                    {
                                        To = currUser.Email,
                                        OperationCode = 4,
                                    };

                                    body = "Name - " + currTodo.Name + "\n\nDescription - " + currTodo.Description + "\n\nDuedate - " + currTodo.DueDate.ToString("MM/dd/yyyy HH:mm:ss.fff") + ".";
                                    tmpEmail = LocalFunctions.SendEmail(tmpEmail, string.Empty, body,
                                        LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod())).Result;
                                }
                            }

                            await DeleteReminder(item, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
                        }


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
    }
}
