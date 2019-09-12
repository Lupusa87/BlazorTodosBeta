using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TodosShared;
using TodosWebApi.DataLayer.TSEntities;
using TodosWebApi.GlobalDataLayer;

namespace TodosWebApi.DataLayer
{
    public class TableStorage
    {
        //https://docs.microsoft.com/en-us/azure/visual-studio/vs-storage-aspnet5-getting-started-tables?toc=%2Faspnet%2Fcore%2Ftoc.json&bc=%2Faspnet%2Fcore%2Fbreadcrumb%2Ftoc.json&view=aspnetcore-3.0
        //https://www.codeproject.com/Articles/1203654/Azure-Table-Storage-in-ASP-NET-Core
        //22.03.2019

        CloudStorageAccount storageAccount;
        CloudTableClient tableClient;
        CloudTable tableUsers;
        CloudTable tableTodos;
        CloudTable tableCategories;
        CloudTable tableSettings;
        CloudTable tableActivityLog;
        CloudTable tableErrorLog;
        CloudTable tableVisitorsStat;
        CloudTable tableEmailedCodes;
        CloudTable tableFeedback;
        CloudTable tableReactions;

        public TableStorage()
        {
            try
            {

            
            storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=3abd5975-0ee0-4-231-b9ee;AccountKey=lJ8TG4OXnEXCFpjz9eGDbpalTIINNXyPqlDSoGuF1rtzEgZUZr30MEQEuqk1r7zTkZlxzEdq8XTbXCqakF6tdQ==;TableEndpoint=https://3abd5975-0ee0-4-231-b9ee.table.cosmos.azure.com:443/;");

            tableClient = storageAccount.CreateCloudTableClient();


            tableUsers = tableClient.GetTableReference("Users");
            tableTodos = tableClient.GetTableReference("Todos");
            tableCategories = tableClient.GetTableReference("Categories");
            tableSettings = tableClient.GetTableReference("Settings");
            tableActivityLog = tableClient.GetTableReference("ActivityLog");
            tableErrorLog = tableClient.GetTableReference("ErrorLog");
            tableVisitorsStat = tableClient.GetTableReference("VisitorsStat");
            tableEmailedCodes = tableClient.GetTableReference("EmailedCodes");
            tableFeedback = tableClient.GetTableReference("Feedback");
            tableReactions = tableClient.GetTableReference("Reaction");


            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
        }

        public async Task<bool> EnuserTables()
        {
            try
            {
                bool b = await tableUsers.CreateIfNotExistsAsync();
                b = await tableTodos.CreateIfNotExistsAsync();
                b = await tableCategories.CreateIfNotExistsAsync();
                b = await tableSettings.CreateIfNotExistsAsync();
                b = await tableActivityLog.CreateIfNotExistsAsync();
                b = await tableErrorLog.CreateIfNotExistsAsync();
                b = await tableVisitorsStat.CreateIfNotExistsAsync();
                b = await tableEmailedCodes.CreateIfNotExistsAsync();
                b = await tableFeedback.CreateIfNotExistsAsync();
                b = await tableReactions.CreateIfNotExistsAsync();
                return b;


            }
            catch (StorageException ex)
            {

                
                await AddErrorLog("AllUsers", ex.Message, MethodBase.GetCurrentMethod());

                return false;
            }
        }


        #region User

        public async Task<bool> AddUser(TSUser tsUser)
        {
            try
            {

                TSUserEntity newUser = new TSUserEntity(tsUser)
                {
                    Salt = GlobalFunctions.GetSalt()
                };
                newUser.HashedPassword = GlobalFunctions.CmdHashPasswordBytes(tsUser.Password, newUser.Salt);
                TableOperation insertOperation = TableOperation.Insert(newUser);


                await tableUsers.ExecuteAsync(insertOperation);


                await AddDefaultCategory(tsUser.UserID);

                return true;


            }
            catch (StorageException ex)
            {

                
                await AddErrorLog(tsUser.UserID, ex.Message, MethodBase.GetCurrentMethod());

                return false;
            }
        }





        public async Task<bool> AddUsersBath(List<TSUser> TsUsers)
        {
            try
            {

                TableBatchOperation batchOperation = new TableBatchOperation();

                foreach (var item in TsUsers)
                {
                    TSUserEntity a = new TSUserEntity(item)
                    {
                        Salt = GlobalFunctions.GetSalt()
                    };
                    a.HashedPassword = GlobalFunctions.CmdHashPasswordBytes(item.Password, a.Salt);
                    batchOperation.Insert(new TSUserEntity(item));
                }

                await tableUsers.ExecuteBatchAsync(batchOperation);

                return true;

            }
            catch (StorageException ex)
            {

                
                await AddErrorLog("AllUsers", ex.Message, MethodBase.GetCurrentMethod());

                return false;
            }
        }

        public async Task<bool> UpdateUserEntity(TSUserEntity tsUser)
        {
            try
            {
                TableOperation Operation = TableOperation.InsertOrReplace(tsUser);

                await tableUsers.ExecuteAsync(Operation);

                return true;
            }
            catch (StorageException ex)
            {
                await AddErrorLog(tsUser.PartitionKey, ex.Message, MethodBase.GetCurrentMethod());

                return false;
            }
        }

        public async Task<bool> UpdateUser(TSUser tsUser, bool KeepPassAndSalt)
        {
            try
            {
                TSUserEntity newUser = new TSUserEntity(tsUser);

                if (KeepPassAndSalt)
                {
                    TSUserEntity oldUser = GetUserEntity(tsUser).Result;
                    newUser.Salt = oldUser.Salt;
                    newUser.HashedPassword = oldUser.HashedPassword;
                }
                else
                {
                    newUser.Salt = GlobalFunctions.GetSalt();
                    newUser.HashedPassword = GlobalFunctions.CmdHashPasswordBytes(tsUser.Password, newUser.Salt);
                }

                TableOperation Operation = TableOperation.InsertOrReplace(newUser);


                await tableUsers.ExecuteAsync(Operation);


                return true;

            }
            catch (StorageException ex)
            {


                await AddErrorLog(tsUser.UserID, ex.Message, MethodBase.GetCurrentMethod());

                return false;
            }
        }


        public async Task<bool> DeleteUser(TSUser tsUser)
        {
            try
            {
                TableOperation retrieveOperation = TableOperation.Retrieve<TSUserEntity>(tsUser.UserID, tsUser.UserName.ToLower());


                TableResult retrievedResult = await tableUsers.ExecuteAsync(retrieveOperation);


                TSUserEntity deleteEntity = (TSUserEntity)retrievedResult.Result;


                if (deleteEntity != null)
                {
                    TableOperation deleteOperation = TableOperation.Delete(deleteEntity);


                    await tableUsers.ExecuteAsync(deleteOperation);

                    return true;
                }

                else
                {
                    return false;
                }

            }
            catch (StorageException ex)
            {

                
                await AddErrorLog(tsUser.UserID, ex.Message, MethodBase.GetCurrentMethod());

                return false;
            }
        }

        public async Task<TSUserEntity> GetUserEntity(TSUser tsUser)
        {
           
            try
            {
               

                TableOperation retrieveOperation = TableOperation.Retrieve<TSUserEntity>(tsUser.UserID, tsUser.UserName.ToLower());


                TableResult retrievedResult = await tableUsers.ExecuteAsync(retrieveOperation);


                if (retrievedResult.Result != null)
                {
                    return (TSUserEntity)retrievedResult.Result;
                }
                else
                {
                    return null;
                }

            }
            catch (StorageException ex)
            {
                await AddErrorLog(tsUser.UserID, ex.Message, MethodBase.GetCurrentMethod());

                return null;
            }
        }


        public async Task<TSUser> GetUser(TSUser tsUser)
        {
            return (await GetUserEntity(tsUser)).toTSUser();
        }

        public async Task<bool> UpdateUserTodosCount(TSUser tsUser, int change)
        {
            TSUser result = await GetUser(tsUser);

            result.TodosCount += change;

            await UpdateUser(result,true);

            return true;
        }


        public async Task<TSUserEntity> FindUser(string SearchCrtiteria, bool SearchPartritionOrRow, string column)
        {
            try
            {
                if (string.IsNullOrEmpty(column))
                {
                    column = "RowKey";
                    if (SearchPartritionOrRow)
                    {
                        column = "PartitionKey";
                    }
                }


                TableQuery<TSUserEntity> query = new TableQuery<TSUserEntity>().Where(TableQuery.GenerateFilterCondition(column, QueryComparisons.Equal, SearchCrtiteria.ToLower())).Take(1);

                TableContinuationToken token = null;
                do
                {
                    TableQuerySegment<TSUserEntity> resultSegment = await tableUsers.ExecuteQuerySegmentedAsync(query, token);
                    token = resultSegment.ContinuationToken;

                    if (resultSegment.Results.Any())
                    {
                        return resultSegment.Results.FirstOrDefault();
                    }
                    else
                    {
                        return null;
                    }

                } while (token != null);

            }
            catch (StorageException ex)
            {

                
                await AddErrorLog("AllUsers", ex.Message, MethodBase.GetCurrentMethod());

                return null;
            }
        }

        public async Task<List<TSUser>> GetAllUsers()
        {
            List<TSUser> TsUsers = new List<TSUser>();
            try
            {
                List<TSUserEntity> result = new List<TSUserEntity>();


                TableQuery<TSUserEntity> query = new TableQuery<TSUserEntity>();


                TableContinuationToken token = null;
                do
                {
                    TableQuerySegment<TSUserEntity> resultSegment = await tableUsers.ExecuteQuerySegmentedAsync(query, token);
                    token = resultSegment.ContinuationToken;

                    result.AddRange(resultSegment);

                } while (token != null);





                foreach (var item in result)
                {
                    TsUsers.Add(item.toTSUser());
                }


                return TsUsers;


            }
            catch (StorageException ex)
            {

                
                await AddErrorLog("AllUsers", ex.Message, MethodBase.GetCurrentMethod());

                return TsUsers;
            }
        }


        public async Task<List<TSUserOpen>> GetLiveUsers()
        {
            List<TSUserOpen> r = new List<TSUserOpen>();

            try
            {
                List<TSUserEntity> result = new List<TSUserEntity>();

                string IsLiveFilter = TableQuery.GenerateFilterConditionForBool("IsLive", QueryComparisons.Equal, true);
               
                TableQuery<TSUserEntity> query = new TableQuery<TSUserEntity>().Where(IsLiveFilter);


                TableContinuationToken token = null;
                do
                {
                    TableQuerySegment<TSUserEntity> resultSegment = await tableUsers.ExecuteQuerySegmentedAsync(query, token);
                    token = resultSegment.ContinuationToken;

                    result.AddRange(resultSegment);

                } while (token != null);



                foreach (var item in result)
                {
                    r.Add(item.toTSUserOpen());
                }


                return r;


            }
            catch (StorageException ex)
            {

                await AddErrorLog("AllUsers", ex.Message, MethodBase.GetCurrentMethod());

                return new List<TSUserOpen>();
            }

        }

        public async Task<bool> UpdateOfflineUsers()
        {
        

            try
            {
                List<TSUserEntity> result = new List<TSUserEntity>();

                string IsLiveFilter = TableQuery.GenerateFilterConditionForBool("IsLive", QueryComparisons.Equal, true);
                string DateFilter = TableQuery.GenerateFilterConditionForDate("Timestamp", QueryComparisons.LessThan, DateTime.Now.AddSeconds(-30));

                TableQuery<TSUserEntity> query = new TableQuery<TSUserEntity>().Where(TableQuery.CombineFilters(IsLiveFilter, TableOperators.And, DateFilter));


                TableContinuationToken token = null;
                do
                {
                    TableQuerySegment<TSUserEntity> resultSegment = await tableUsers.ExecuteQuerySegmentedAsync(query, token);
                    token = resultSegment.ContinuationToken;

                    if (resultSegment.Any())
                    {
                        result.AddRange(resultSegment);
                    }

                } while (token != null);


                if (result.Any())
                {
                    TableBatchOperation batchOperation = new TableBatchOperation();

                    foreach (var item in result)
                    {
                        item.IsLive = false;
                        batchOperation.InsertOrReplace(item);
                    }

                    await tableUsers.ExecuteBatchAsync(batchOperation);

                }

            }
            catch (StorageException ex)
            {

                await AddErrorLog("AllUsers", ex.Message, MethodBase.GetCurrentMethod());

                return false;
            }


            return true;



        }

        public async Task<bool> UpdateOnlineUsersCount()
        {


            try
            {
                List<TSUserEntity> result = new List<TSUserEntity>();

                string IsLiveFilter = TableQuery.GenerateFilterConditionForBool("IsLive", QueryComparisons.Equal, true);

                TableQuery<TSUserEntity> query = new TableQuery<TSUserEntity>().Where(IsLiveFilter).Select(new List<string>{ "PartitionKey" });


                TableContinuationToken token = null;
                do
                {
                    TableQuerySegment<TSUserEntity> resultSegment = await tableUsers.ExecuteQuerySegmentedAsync(query, token);
                    token = resultSegment.ContinuationToken;

                    if (resultSegment.Any())
                    {
                        result.AddRange(resultSegment);
                    }

                } while (token != null);

                await SetSetting("AllUsers", "LiveUsersCount", result.Count.ToString());
            }
            catch (StorageException ex)
            {

                await AddErrorLog("AllUsers", ex.Message, MethodBase.GetCurrentMethod());

                return false;
            }


            return true;



        }

        #endregion

        #region Todo
        public async Task<bool> AddTodo(TSTodo tsTodo)
        {
            try
            {
                TSTodoEntity newTodo = new TSTodoEntity(tsTodo);

                TableOperation insertOperation = TableOperation.Insert(newTodo);


                await tableTodos.ExecuteAsync(insertOperation);


                return true;

            }
            catch (StorageException ex)
            {

                
                await AddErrorLog(tsTodo.UserID, ex.Message, MethodBase.GetCurrentMethod());

                return false;
            }
        }

        public async Task<bool> AddTodosBath(List<TSTodo> TsTodos)
        {

            if (TsTodos.Any())
            {

                try
                {

                    TableBatchOperation batchOperation = new TableBatchOperation();

                    foreach (var item in TsTodos)
                    {
                        batchOperation.Insert(new TSTodoEntity(item));
                    }

                    await tableTodos.ExecuteBatchAsync(batchOperation);

                    return true;

                }
                catch (StorageException ex)
                {

                    
                    await AddErrorLog(TsTodos.FirstOrDefault().UserID, ex.Message, MethodBase.GetCurrentMethod());

                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> UpdateTodo(TSTodo tsTodo)
        {
            try
            {

                TSTodoEntity newTodo = new TSTodoEntity(tsTodo);

                TableOperation Operation = TableOperation.InsertOrReplace(newTodo);

                await tableTodos.ExecuteAsync(Operation);

                return true;

            }
            catch (StorageException ex)
            {

                await AddErrorLog(tsTodo.UserID, ex.Message, MethodBase.GetCurrentMethod());

                return false;
            }
        }

        public async Task<bool> UpdateTodoEntity(TSTodoEntity tsTodo)
        {
            try
            {
                TableOperation Operation = TableOperation.InsertOrReplace(tsTodo);

                await tableTodos.ExecuteAsync(Operation);

                return true;

            }
            catch (StorageException ex)
            {


                await AddErrorLog(tsTodo.PartitionKey, ex.Message, MethodBase.GetCurrentMethod());

                return false;
            }
        }


        public async Task<bool> DeleteTodo(TSTodo tsTodo)
        {
            try
            {
                TableOperation retrieveOperation = TableOperation.Retrieve<TSTodoEntity>(tsTodo.UserID, tsTodo.TodoID.ToString());


                TableResult retrievedResult = await tableTodos.ExecuteAsync(retrieveOperation);


                TSTodoEntity deleteEntity = (TSTodoEntity)retrievedResult.Result;


                if (deleteEntity != null)
                {
                    TableOperation deleteOperation = TableOperation.Delete(deleteEntity);


                    await tableTodos.ExecuteAsync(deleteOperation);

                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (StorageException ex)
            {

                
                await AddErrorLog(tsTodo.UserID, ex.Message, MethodBase.GetCurrentMethod());

                return false;
            }
        }

        public async Task<TSTodo> GetTodo(TSTodo tsTodo)
        {
            TSTodoEntity result = new TSTodoEntity();

            try
            {
                

                TableOperation retrieveOperation = TableOperation.Retrieve<TSTodoEntity>(tsTodo.UserID, tsTodo.TodoID.ToString());


                TableResult retrievedResult = await tableTodos.ExecuteAsync(retrieveOperation);


                if (retrievedResult.Result != null)
                {
                    result = (TSTodoEntity)retrievedResult.Result;

                }

                return result.toTSTodo();
            }
            catch (StorageException ex)
            {

                
                await AddErrorLog(tsTodo.UserID, ex.Message, MethodBase.GetCurrentMethod());

                return result.toTSTodo();
            }

        }

        public async Task<List<TSTodo>> GetAllTodos(string UserID)
        {
            List<TSTodo> TsTodos = new List<TSTodo>();


            try
            {
                List<TSTodoEntity> result = new List<TSTodoEntity>();

                TableQuery<TSTodoEntity> query = new TableQuery<TSTodoEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, UserID));


                TableContinuationToken token = null;
                do
                {
                    TableQuerySegment<TSTodoEntity> resultSegment = await tableTodos.ExecuteQuerySegmentedAsync(query, token);
                    token = resultSegment.ContinuationToken;

                    result.AddRange(resultSegment);

                } while (token != null);

                foreach (var item in result)
                {
                    TsTodos.Add(item.toTSTodo());
                }

            }
            catch (StorageException ex)
            {
                
                await AddErrorLog(UserID, ex.Message, MethodBase.GetCurrentMethod());
            }


            return TsTodos;



        }



        public async Task<bool> SendTodoReminders(string DemoUserID)
        {


            try
            {
                List<TSTodoEntity> result = new List<TSTodoEntity>();

                string IsReminderEmailedFilter = TableQuery.GenerateFilterConditionForBool("IsReminderEmailed", QueryComparisons.Equal, false);
                string HasRemindDateFilter = TableQuery.GenerateFilterConditionForBool("HasRemindDate", QueryComparisons.Equal, true);
                string RemindFilter = TableQuery.GenerateFilterConditionForDate("RemindDate", QueryComparisons.LessThan, DateTime.Now);
                string DemoUserFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.NotEqual, DemoUserID);


                string finalFilter = TableQuery.CombineFilters(
                    TableQuery.CombineFilters(
                        TableQuery.CombineFilters(
                            IsReminderEmailedFilter,
                            TableOperators.And,
                            HasRemindDateFilter),
                        TableOperators.And, RemindFilter),
                        TableOperators.And, DemoUserFilter);



                TableQuery<TSTodoEntity> query = new TableQuery<TSTodoEntity>().Where(finalFilter);


                TableContinuationToken token = null;
                do
                {
                    TableQuerySegment<TSTodoEntity> resultSegment = await tableTodos.ExecuteQuerySegmentedAsync(query, token);
                    token = resultSegment.ContinuationToken;

                    if (resultSegment.Any())
                    {
                        result.AddRange(resultSegment);
                    }

                } while (token != null);


                if (result.Any())
                {
                    bool b = false;
                    string body = string.Empty;
                    foreach (var item in result)
                    {
                        


                        TSUserEntity currUser = FindUser(item.PartitionKey, true, string.Empty).Result;

                        if (currUser!=null)
                        {
                            item.IsReminderEmailed = true;


                            TSEmail tmpEmail = new TSEmail()
                            {
                                To = currUser.Email,
                                OperationCode = 4,
                            };

                            body = "Name - " + item.Name + "\n\nDescription - " + item.Description + "\n\nDuedate - " + item.DueDate.ToString("MM/dd/yyyy HH:mm:ss.fff") + ".";
                            tmpEmail = GlobalFunctions.SendEmail(tmpEmail, string.Empty, body).Result;


                            b = UpdateTodoEntity(item).Result;
                        }
                 

                    }
                }

            }
            catch (StorageException ex)
            {

                await AddErrorLog("AllUsers", ex.Message, MethodBase.GetCurrentMethod());

                return false;
            }


            return true;



        }
        #endregion


        #region EmailedCode
        public async Task<bool> AddEmailedCode(TSEmailedCode tsEmailedCode)
        {
            try
            {
                string EmailedCodeID = await GetNewID("AllUsers", "LastEmailedCodeID", false);
                tsEmailedCode.ID = EmailedCodeID;
                tsEmailedCode.AddDate = DateTime.UtcNow;

                TSEmailedCodeEntity newEmailedCode = new TSEmailedCodeEntity(tsEmailedCode);

                TableOperation insertOperation = TableOperation.Insert(newEmailedCode);


                await tableEmailedCodes.ExecuteAsync(insertOperation);


                return true;

            }
            catch (StorageException ex)
            {

                await AddErrorLog("AllUsers", ex.Message, MethodBase.GetCurrentMethod());

                return false;
            }
        }


        public async Task<bool> DeleteExpiredEmaiedCodes()
        {

            try
            {
                List<TSEmailedCodeEntity> result = new List<TSEmailedCodeEntity>();

               
                string DateFilter = TableQuery.GenerateFilterConditionForDate("AddDate", QueryComparisons.LessThan, DateTime.Now.AddMinutes(-1));

                TableQuery<TSEmailedCodeEntity> query = new TableQuery<TSEmailedCodeEntity>().Where(DateFilter);


                TableContinuationToken token = null;
                do
                {
                    TableQuerySegment<TSEmailedCodeEntity> resultSegment = await tableEmailedCodes.ExecuteQuerySegmentedAsync(query, token);
                    token = resultSegment.ContinuationToken;

                    if (resultSegment.Any())
                    {
                        result.AddRange(resultSegment);
                    }

                } while (token != null);


                if (result.Any())
                {
                    TableBatchOperation batchOperation = new TableBatchOperation();

                    foreach (var item in result)
                    {
                        batchOperation.Delete(item);
                    }

                    await tableEmailedCodes.ExecuteBatchAsync(batchOperation);

                }

            }
            catch (StorageException ex)
            {

                await AddErrorLog("AllUsers", ex.Message, MethodBase.GetCurrentMethod());

                return false;
            }


            return true;



        }



        public async Task<bool> DeleteEmaiedCodes(string Email)
        {

            try
            {
                List<TSEmailedCodeEntity> result = new List<TSEmailedCodeEntity>();


                string Filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, Email.ToLower());

                TableQuery<TSEmailedCodeEntity> query = new TableQuery<TSEmailedCodeEntity>().Where(Filter);


                TableContinuationToken token = null;
                do
                {
                    TableQuerySegment<TSEmailedCodeEntity> resultSegment = await tableEmailedCodes.ExecuteQuerySegmentedAsync(query, token);
                    token = resultSegment.ContinuationToken;

                    if (resultSegment.Any())
                    {
                        result.AddRange(resultSegment);
                    }

                } while (token != null);


                if (result.Any())
                {
                    TableBatchOperation batchOperation = new TableBatchOperation();

                    foreach (var item in result)
                    {
                        batchOperation.Delete(item);
                    }

                    await tableEmailedCodes.ExecuteBatchAsync(batchOperation);

                }

            }
            catch (StorageException ex)
            {

                await AddErrorLog("AllUsers", ex.Message, MethodBase.GetCurrentMethod());

                return false;
            }


            return true;



        }


        public async Task<TSEmailedCodeEntity> FindEmaiedCode(string pEmail, string pIPAddress, string pMachineID)
        {
            try
            {


                string EmailFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, pEmail.ToLower());
                string IPAddressFilter = TableQuery.GenerateFilterCondition("IPAddress", QueryComparisons.Equal, pIPAddress.ToLower());
                string MachineIDFilter = TableQuery.GenerateFilterCondition("MachineID", QueryComparisons.Equal, pMachineID.ToLower());


                string finalFilter = TableQuery.CombineFilters(
                        TableQuery.CombineFilters(
                            EmailFilter,
                            TableOperators.And,
                            IPAddressFilter),
                        TableOperators.And, MachineIDFilter);

                TableQuery<TSEmailedCodeEntity> query = new TableQuery<TSEmailedCodeEntity>().Where(finalFilter).Take(1);


                TableContinuationToken token = null;
                do
                {
                    TableQuerySegment<TSEmailedCodeEntity> resultSegment = await tableEmailedCodes.ExecuteQuerySegmentedAsync(query, token);
                    token = resultSegment.ContinuationToken;

                    if (resultSegment.Results.Any())
                    {
                        return resultSegment.Results.FirstOrDefault();
                    }
                    else
                    {
                        return null;
                    }

                } while (token != null);

            }
            catch (StorageException ex)
            {


                await AddErrorLog("AllUsers", ex.Message, MethodBase.GetCurrentMethod());

                return null;
            }
        }



        public async Task<TSEmailedCode> GetEmailedCode(TSEmailedCode tsEmailedCode)
        {
            TSEmailedCodeEntity result = new TSEmailedCodeEntity();

            try
            {

                TableOperation retrieveOperation = TableOperation.Retrieve<TSEmailedCodeEntity>(tsEmailedCode.Email.ToLower(), tsEmailedCode.ID.ToString());


                TableResult retrievedResult = await tableEmailedCodes.ExecuteAsync(retrieveOperation);


                if (retrievedResult.Result != null)
                {
                    result = (TSEmailedCodeEntity)retrievedResult.Result;

                }

                return result.toTSEmailedCode();
            }
            catch (StorageException ex)
            {


                await AddErrorLog("AllUsers", ex.Message, MethodBase.GetCurrentMethod());

                return result.toTSEmailedCode();
            }

        }

      
        #endregion

        #region Settings




        public async Task<bool> SetSetting(string UserID, string Key, string Value)
        {

            try
            {
                TableOperation Operation = TableOperation.InsertOrReplace(new TSSettingEntity(UserID, Key, Value));

                await tableSettings.ExecuteAsync(Operation);

                return true;
            }
            catch (StorageException ex)
            {

                //if (ex.RequestInformation.HttpStatusCode == 412)
                //    WriteLine("Optimistic concurrency violation – entity has changed since it was retrieved.");
                //else
                //    throw;

                
                await AddErrorLog(UserID, ex.Message, MethodBase.GetCurrentMethod());


                return false;
            }




        }

        public async Task<TSSettingEntity> GetSetting(string UserID, string Key)
        {
            try
            {

                TableOperation retrieveOperation = TableOperation.Retrieve<TSSettingEntity>(UserID, Key.ToLower());


                TableResult retrievedResult = await tableSettings.ExecuteAsync(retrieveOperation);


                if (retrievedResult.Result != null)
                {

                    return (TSSettingEntity)retrievedResult.Result;
                }
                else
                {

                    TSSettingEntity newSetting = new TSSettingEntity(UserID, Key, string.Empty);

                    TableOperation Operation = TableOperation.Insert(newSetting);

                    await tableSettings.ExecuteAsync(Operation);

                    return newSetting;
                }

            }
            catch (StorageException ex)
            {

                
                await AddErrorLog(UserID, ex.Message, MethodBase.GetCurrentMethod());

                return new TSSettingEntity();
            }
        }


        public async Task<bool> UpdateSettingCounter(string UserID, string KeyName, bool IncreaseOrDecrease)
        {
            try
            {
                TSSettingEntity tsSetting = await GetSetting(UserID, KeyName);

                if (string.IsNullOrEmpty(tsSetting.Value))
                {


                    if (IncreaseOrDecrease)
                    {
                        await SetSetting(UserID, KeyName, "1");
                    }
                    else
                    {
                        await SetSetting(UserID, KeyName, "-1");
                    }
                }
                else
                {
                    int NewID = int.Parse(tsSetting.Value);


                    if (IncreaseOrDecrease)
                    {
                        await SetSetting(UserID, KeyName, (NewID + 1).ToString());
                    }
                    else
                    {
                        await SetSetting(UserID, KeyName, (NewID - 1).ToString());
                    }

                }

                return true;


            }
            catch (StorageException ex)
            {

                
                await AddErrorLog(UserID, ex.Message, MethodBase.GetCurrentMethod());

                return false;
            }
        }


        public async Task<string> GetNewID(string UserID, string KeyName, bool HasSalt)
        {
            try
            {
               
                TSSettingEntity tsSetting = await GetSetting(UserID, KeyName);

                if (string.IsNullOrEmpty(tsSetting.Value))
                {
                
                    await SetSetting(UserID, KeyName, "1");

                    if (HasSalt)
                    {
                        return "1_" + Guid.NewGuid().ToString("d").Substring(1, 4).ToLower();
                    }
                    else
                    {
                        return "1";
                    }
                }
                else
                {
                  
                    int NewID = int.Parse(tsSetting.Value) + 1;


                    await SetSetting(UserID, KeyName, NewID.ToString());

                    if (HasSalt)
                    {
                        return NewID + "_" + Guid.NewGuid().ToString("d").Substring(1, 4).ToLower();
                    }
                    else
                    {
                        return NewID.ToString();
                    }

                }

            }
            catch (StorageException ex)
            {
         
                
                await AddErrorLog(UserID, ex.Message, MethodBase.GetCurrentMethod());

                return "0";
            }
        }

        #endregion


        #region Visitors

        public async Task<bool> AddVisitor(string IPAddress)
        {
            try
            {
                TSVisitorsStatEntity entity = await GetVisitor(IPAddress);

                if (entity is null)
                {

                    entity = new TSVisitorsStatEntity("AllUsers", IPAddress, 0);


                    await UpdateSettingCounter("AllUsers", "IPAddressesCount", true);
                }


                await UpdateSettingCounter("AllUsers", "VisitsCount", true);
                entity.Count += 1;
              

                TableOperation Operation = TableOperation.InsertOrReplace(entity);

                await tableVisitorsStat.ExecuteAsync(Operation);


                return true;

            }
            catch (StorageException ex)
            {

                
                await AddErrorLog("AllUsers", ex.Message, MethodBase.GetCurrentMethod());

                return false;
            }
        }


        public async Task<TSVisitorsStatEntity> GetVisitor(string IPAddress)
        {
            try
            {
                TSVisitorsStatEntity result = new TSVisitorsStatEntity();

                TableOperation retrieveOperation = TableOperation.Retrieve<TSVisitorsStatEntity>("AllUsers", IPAddress);


                TableResult retrievedResult = await tableVisitorsStat.ExecuteAsync(retrieveOperation);


                if (retrievedResult.Result != null)
                {
                    return (TSVisitorsStatEntity)retrievedResult.Result;
                }

                return null;

            }
            catch (StorageException ex)
            {
                await AddErrorLog("AllUsers", ex.Message, MethodBase.GetCurrentMethod());

                return null;
            }
        }


        #endregion



        #region ActivityLog
        public async Task<bool> AddActivityLog(string UserID, string Description, MethodBase MB)
        {

            if (GlobalData.DoActivityLog)
            {



                try
                {

                    string ActivityID = await GetNewID("AllUsers", "LastActivityID", false);
                    TSActivityLogEntity newActivityLog = new TSActivityLogEntity(UserID, ActivityID, Description, GetMethodName(MB));


                    TableOperation insertOperation = TableOperation.Insert(newActivityLog);


                    await tableActivityLog.ExecuteAsync(insertOperation);


                    return true;


                }
                catch (StorageException ex)
                {


                    await AddErrorLog("AllUsers", ex.Message, MethodBase.GetCurrentMethod());

                    return false;
                }
            }
            else
            {
                return true;
            }
        }
        #endregion

        #region ErrorLog
        public async Task<bool> AddErrorLog(string UserID, string Description, MethodBase MB)
        {
            string ErrorID = await GetNewID("AllUsers", "LastErrorID", false);
            TSErrorLogEntity newErrorLog = new TSErrorLogEntity(UserID, ErrorID, Description, GetMethodName(MB));


            TableOperation insertOperation = TableOperation.Insert(newErrorLog);


            await tableErrorLog.ExecuteAsync(insertOperation);

            await GlobalFunctions.NotifyAdmin("Error: " + Description);

            return true;
        }
        #endregion

        #region LocalFunctions
        public string GetMethodName(MethodBase MB)
        {

            string a = MB.DeclaringType.FullName.Replace("TodosWebApi.", null);
            a = a.Replace("TokenProviderMiddleware", "TPM");
            a = a.Replace("Controllers.", null);
            a = a.Replace("+<", ".");

            int k = a.IndexOf(">");

            if (k > -1)
            {
                a = a.Substring(0, k);
            }

            return a;
        }

        #endregion



        #region Category
        public async Task<bool> AddCategory(TSCategory tsCategory)
        {
            try
            {
                TSCategoryEntity newCategory = new TSCategoryEntity(tsCategory);

                TableOperation insertOperation = TableOperation.Insert(newCategory);


                await tableCategories.ExecuteAsync(insertOperation);


                return true;

            }
            catch (StorageException ex)
            {


                await AddErrorLog(tsCategory.UserID, ex.Message, MethodBase.GetCurrentMethod());

                return false;
            }
        }

       

        public async Task<bool> UpdateCategory(TSCategory tsCategory)
        {
            try
            {

                TSCategoryEntity newCategory = new TSCategoryEntity(tsCategory);

                TableOperation Operation = TableOperation.InsertOrReplace(newCategory);


                await tableCategories.ExecuteAsync(Operation);


                return true;

            }
            catch (StorageException ex)
            {


                await AddErrorLog(tsCategory.UserID, ex.Message, MethodBase.GetCurrentMethod());

                return false;
            }
        }


        public async Task<bool> DeleteCategory(TSCategory tsCategory)
        {
            try
            {
                TableOperation retrieveOperation = TableOperation.Retrieve<TSCategoryEntity>(tsCategory.UserID, tsCategory.CategoryID.ToString());


                TableResult retrievedResult = await tableCategories.ExecuteAsync(retrieveOperation);


                TSCategoryEntity deleteEntity = (TSCategoryEntity)retrievedResult.Result;


                if (deleteEntity != null)
                {
                    TableOperation deleteOperation = TableOperation.Delete(deleteEntity);


                    await tableCategories.ExecuteAsync(deleteOperation);

                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (StorageException ex)
            {


                await AddErrorLog(tsCategory.UserID, ex.Message, MethodBase.GetCurrentMethod());

                return false;
            }
        }

        public async Task<TSCategory> GetCategory(TSCategory tsCategory)
        {
            TSCategoryEntity result = new TSCategoryEntity();

            try
            {


                TableOperation retrieveOperation = TableOperation.Retrieve<TSCategoryEntity>(tsCategory.UserID, tsCategory.CategoryID.ToString());


                TableResult retrievedResult = await tableCategories.ExecuteAsync(retrieveOperation);


                if (retrievedResult.Result != null)
                {
                    result = (TSCategoryEntity)retrievedResult.Result;

                }

                return result.toTSCategory();
            }
            catch (StorageException ex)
            {


                await AddErrorLog(tsCategory.UserID, ex.Message, MethodBase.GetCurrentMethod());

                return result.toTSCategory();
            }

        }

        public async Task<List<TSCategory>> GetAllCategories(string UserID)
        {
            List<TSCategory> TsCategorys = new List<TSCategory>();


            try
            {
                List<TSCategoryEntity> result = new List<TSCategoryEntity>();

                TableQuery<TSCategoryEntity> query = new TableQuery<TSCategoryEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, UserID));


                TableContinuationToken token = null;
                do
                {
                    TableQuerySegment<TSCategoryEntity> resultSegment = await tableCategories.ExecuteQuerySegmentedAsync(query, token);
                    token = resultSegment.ContinuationToken;

                    result.AddRange(resultSegment);

                } while (token != null);

                foreach (var item in result)
                {
                    TsCategorys.Add(item.toTSCategory());



                }

            }
            catch (StorageException ex)
            {

                await AddErrorLog(UserID, ex.Message, MethodBase.GetCurrentMethod());
            }


            return TsCategorys;



        }



        public async Task<bool> AddDefaultCategory(string UserID)
        {
            TSCategory tsCategory = new TSCategory();
            string a = await GetNewID(UserID, "LastCategoryID", false);
            tsCategory.CategoryID = int.Parse(a);
            tsCategory.UserID = UserID;
            tsCategory.Name = "default";

            return await AddCategory(tsCategory);
        }
        #endregion




        #region Feedback
        public async Task<bool> AddFeedback(TSFeedback tsFeedback)
        {
            try
            {
                TSFeedbackEntity newFeedback = new TSFeedbackEntity(tsFeedback);

                TableOperation insertOperation = TableOperation.Insert(newFeedback);


                await tableFeedback.ExecuteAsync(insertOperation);


                return true;

            }
            catch (StorageException ex)
            {


                await AddErrorLog(tsFeedback.UserID, ex.Message, MethodBase.GetCurrentMethod());

                return false;
            }
        }

     
        public async Task<bool> UpdateFeedback(TSFeedback tsFeedback)
        {
            try
            {

                TSFeedbackEntity newFeedback = new TSFeedbackEntity(tsFeedback);

                TableOperation Operation = TableOperation.InsertOrReplace(newFeedback);


                await tableFeedback.ExecuteAsync(Operation);


                return true;

            }
            catch (StorageException ex)
            {


                await AddErrorLog(tsFeedback.UserID, ex.Message, MethodBase.GetCurrentMethod());

                return false;
            }
        }

        public async Task<bool> UpdateFeedbackEntity(TSFeedbackEntity tsFeedback)
        {
            try
            {
                TableOperation Operation = TableOperation.InsertOrReplace(tsFeedback);

                await tableFeedback.ExecuteAsync(Operation);

                return true;

            }
            catch (StorageException ex)
            {


                await AddErrorLog(tsFeedback.PartitionKey, ex.Message, MethodBase.GetCurrentMethod());

                return false;
            }
        }


        public async Task<bool> DeleteFeedback(TSFeedback tsFeedback)
        {
            try
            {
                TableOperation retrieveOperation = TableOperation.Retrieve<TSFeedbackEntity>(tsFeedback.UserID, tsFeedback.FeedbackID.ToString());


                TableResult retrievedResult = await tableFeedback.ExecuteAsync(retrieveOperation);


                TSFeedbackEntity deleteEntity = (TSFeedbackEntity)retrievedResult.Result;


                if (deleteEntity != null)
                {
                    TableOperation deleteOperation = TableOperation.Delete(deleteEntity);


                    await tableFeedback.ExecuteAsync(deleteOperation);

                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (StorageException ex)
            {


                await AddErrorLog(tsFeedback.UserID, ex.Message, MethodBase.GetCurrentMethod());

                return false;
            }
        }

        public async Task<TSFeedback> GetFeedback(TSFeedback tsFeedback)
        {
            TSFeedbackEntity result = new TSFeedbackEntity();

            try
            {


                TableOperation retrieveOperation = TableOperation.Retrieve<TSFeedbackEntity>(tsFeedback.UserID, tsFeedback.FeedbackID.ToString());


                TableResult retrievedResult = await tableFeedback.ExecuteAsync(retrieveOperation);


                if (retrievedResult.Result != null)
                {
                    result = (TSFeedbackEntity)retrievedResult.Result;

                }

                return result.toTSFeedback();
            }
            catch (StorageException ex)
            {


                await AddErrorLog(tsFeedback.UserID, ex.Message, MethodBase.GetCurrentMethod());

                return result.toTSFeedback();
            }

        }

        public async Task<List<TSFeedback>> GetAllFeedback()
        {
            List<TSFeedback> TsFeedbacks = new List<TSFeedback>();


            try
            {
                List<TSFeedbackEntity> result = new List<TSFeedbackEntity>();

                TableQuery<TSFeedbackEntity> query = new TableQuery<TSFeedbackEntity>();


                TableContinuationToken token = null;
                do
                {
                    TableQuerySegment<TSFeedbackEntity> resultSegment = await tableFeedback.ExecuteQuerySegmentedAsync(query, token);
                    token = resultSegment.ContinuationToken;

                    result.AddRange(resultSegment);

                } while (token != null);

                foreach (var item in result)
                {
                    TsFeedbacks.Add(item.toTSFeedback());
                }

            }
            catch (StorageException ex)
            {

                await AddErrorLog("AllUsers", ex.Message, MethodBase.GetCurrentMethod());
            }


            return TsFeedbacks;



        }

        public async Task<TSFeedbackEntity> FindFeedback(string SearchCrtiteria, bool SearchPartritionOrRow, string column)
        {
            try
            {
                if (string.IsNullOrEmpty(column))
                {
                    column = "RowKey";
                    if (SearchPartritionOrRow)
                    {
                        column = "PartitionKey";
                    }
                }


                TableQuery<TSFeedbackEntity> query = new TableQuery<TSFeedbackEntity>().Where(TableQuery.GenerateFilterCondition(column, QueryComparisons.Equal, SearchCrtiteria.ToLower())).Take(1);

                TableContinuationToken token = null;
                do
                {
                    TableQuerySegment<TSFeedbackEntity> resultSegment = await tableFeedback.ExecuteQuerySegmentedAsync(query, token);
                    token = resultSegment.ContinuationToken;

                    if (resultSegment.Results.Any())
                    {
                        return resultSegment.Results.FirstOrDefault();
                    }
                    else
                    {
                        return null;
                    }

                } while (token != null);

            }
            catch (StorageException ex)
            {


                await AddErrorLog("AllUsers", ex.Message, MethodBase.GetCurrentMethod());

                return null;
            }
        }
        #endregion


        #region Reaction
        public async Task<bool> AddReaction(TSReaction tsReaction)
        {
            try
            {
                TSReactionEntity newReaction = new TSReactionEntity(tsReaction);

                TableOperation insertOperation = TableOperation.Insert(newReaction);


                await tableReactions.ExecuteAsync(insertOperation);


                return true;

            }
            catch (StorageException ex)
            {


                await AddErrorLog(tsReaction.UserID, ex.Message, MethodBase.GetCurrentMethod());

                return false;
            }
        }


        public async Task<bool> UpdateReaction(TSReaction tsReaction)
        {
            try
            {

                TSReactionEntity newReaction = new TSReactionEntity(tsReaction);

                TableOperation Operation = TableOperation.InsertOrReplace(newReaction);


                await tableReactions.ExecuteAsync(Operation);


                return true;

            }
            catch (StorageException ex)
            {


                await AddErrorLog(tsReaction.UserID, ex.Message, MethodBase.GetCurrentMethod());

                return false;
            }
        }

        public async Task<bool> UpdateReactionEntity(TSReactionEntity tsReaction)
        {
            try
            {
                TableOperation Operation = TableOperation.InsertOrReplace(tsReaction);

                await tableReactions.ExecuteAsync(Operation);

                return true;

            }
            catch (StorageException ex)
            {


                await AddErrorLog(tsReaction.PartitionKey, ex.Message, MethodBase.GetCurrentMethod());

                return false;
            }
        }


        public async Task<TSReaction> GetReaction(TSReaction tsReaction)
        {
            TSReactionEntity result = new TSReactionEntity();

            try
            {


                TableOperation retrieveOperation = TableOperation.Retrieve<TSReactionEntity>(tsReaction.UserID, tsReaction.ReactionID.ToString());


                TableResult retrievedResult = await tableReactions.ExecuteAsync(retrieveOperation);


                if (retrievedResult.Result != null)
                {
                    result = (TSReactionEntity)retrievedResult.Result;

                }

                return result.toTSReaction();
            }
            catch (StorageException ex)
            {


                await AddErrorLog(tsReaction.UserID, ex.Message, MethodBase.GetCurrentMethod());

                return result.toTSReaction();
            }

        }


        public async Task<TSReactionEntity> FindReaction(string SearchCrtiteria, bool SearchPartritionOrRow, string column)
        {
            try
            {
                if (string.IsNullOrEmpty(column))
                {
                    column = "RowKey";
                    if (SearchPartritionOrRow)
                    {
                        column = "PartitionKey";
                    }
                }


                TableQuery<TSReactionEntity> query = new TableQuery<TSReactionEntity>().Where(TableQuery.GenerateFilterCondition(column, QueryComparisons.Equal, SearchCrtiteria.ToLower())).Take(1);

                TableContinuationToken token = null;
                do
                {
                    TableQuerySegment<TSReactionEntity> resultSegment = await tableReactions.ExecuteQuerySegmentedAsync(query, token);
                    token = resultSegment.ContinuationToken;

                    if (resultSegment.Results.Any())
                    {
                        return resultSegment.Results.FirstOrDefault();
                    }
                    else
                    {
                        return null;
                    }

                } while (token != null);

            }
            catch (StorageException ex)
            {


                await AddErrorLog("AllUsers", ex.Message, MethodBase.GetCurrentMethod());

                return null;
            }
        }
        #endregion
    }
}
