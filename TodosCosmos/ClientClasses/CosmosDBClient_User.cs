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
    public class CosmosDBClient_User
    {

        private readonly CosmosDBRepository<CosmosDocUser> cosmosDBRepo = new CosmosDBRepository<CosmosDocUser>();
        private readonly CosmosDBClient_Base<CosmosDocUser> cosmosDBClientBase = new CosmosDBClient_Base<CosmosDocUser>();
        private readonly string pkPrefix = ((int)DocTypeEnum.User).ToString();


        public async Task<bool> AddUser(TSUser tsUser, List<string> CallTrace)
        {
            try
            {

                CosmosDocUser newUser = new CosmosDocUser(tsUser)
                {
                    Salt = GlobalFunctions.GetSalt()
                };
                newUser.HashedPassword = GlobalFunctions.CmdHashPasswordBytes(tsUser.Password, newUser.Salt);


                await cosmosDBClientBase.AddItemAsync(newUser, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

                await CosmosAPI.cosmosDBClientCategory.AddDefaultCategory(tsUser.ID, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

                return true;


            }
            catch (CosmosException ex)
            {


                await CosmosAPI.cosmosDBClientError.AddErrorLog(tsUser.ID, ex.Message, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

                return false;
            }
        }

        public async Task<bool> AddUsersBath(List<TSUser> TsUsers, List<string> CallTrace)
        {
            try
            {

                

                foreach (var item in TsUsers)
                {
                    CosmosDocUser a = new CosmosDocUser(item)
                    {
                        Salt = GlobalFunctions.GetSalt()
                    };
                    a.HashedPassword = GlobalFunctions.CmdHashPasswordBytes(item.Password, a.Salt);

                    await cosmosDBClientBase.AddItemAsync(a, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
                }

               

                return true;

            }
            catch (CosmosException ex)
            {


                await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, ex.Message, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

                return false;
            }
        }

        public async Task<bool> UpdateUserDoc(CosmosDocUser tsUser, List<string> CallTrace)
        {
            return await cosmosDBClientBase.UpdateItemAsync(tsUser, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

        }

        public async Task<bool> UpdateUser(TSUser tsUser, bool KeepPassAndSalt, List<string> CallTrace)
        {
            try
            {

                CosmosDocUser newUser = new CosmosDocUser(tsUser);

                if (KeepPassAndSalt)
                {
                    CosmosDocUser oldUser = await GetUserDoc(tsUser, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
                    newUser.Salt = oldUser.Salt;
                    newUser.HashedPassword = oldUser.HashedPassword;
                }
                else
                {
                    newUser.Salt = GlobalFunctions.GetSalt();
                    newUser.HashedPassword = GlobalFunctions.CmdHashPasswordBytes(tsUser.Password, newUser.Salt);
                }


                await cosmosDBClientBase.UpdateItemAsync(newUser, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));


                return true;

            }
            catch (CosmosException ex)
            {


                await CosmosAPI.cosmosDBClientError.AddErrorLog(tsUser.ID, ex.Message, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

                return false;
            }
        }


        public async Task<bool> DeleteUser(TSUser tsUser, List<string> CallTrace)
        {
            return await cosmosDBClientBase.DeleteItemAsync(new CosmosDocUser(tsUser), pkPrefix, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
        }

        public async Task<CosmosDocUser> GetUserDoc(TSUser tsUser, List<string> CallTrace)
        {

            return await cosmosDBClientBase.GetItemAsync(new CosmosDocUser(tsUser), pkPrefix, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
        }

        public async Task<CosmosDocUser> GetUserDoc(Guid id, List<string> CallTrace)
        {

            return await cosmosDBClientBase.GetItemAsync(id, id, pkPrefix, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
        }


        public async Task<TSUser> GetUser(TSUser tsUser, List<string> CallTrace)
        {

            return (await cosmosDBClientBase.GetItemAsync(new CosmosDocUser(tsUser), pkPrefix, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()))).toTSUser();
        }

        public async Task<TSUser> GetUser(Guid id, List<string> CallTrace)
        {

            return (await cosmosDBClientBase.GetItemAsync(id, id, pkPrefix, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()))).toTSUser();
        }


        public async Task<bool> UpdateUserTodosCount(TSUser tsUser, int change, List<string> CallTrace)
        {
            TSUser result = await GetUser(tsUser, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

            result.TodosCount += change;

            await UpdateUser(result, true, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

            return true;
        }


        public async Task<CosmosDocUser> FindUser(string SearchCrtiteria, string column, List<string> CallTrace)
        {
            try
            {
                QueryDefinition sql = new QueryDefinition("SELECT * FROM c WHERE c.DocType = " + (int)DocTypeEnum.User + " and c." + column + "='" + SearchCrtiteria + "'");

                return await cosmosDBRepo.FindFirstItemsAsync(sql, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
            }
            catch (CosmosException ex)
            {
                await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, ex.Message, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

                return null;
            }
        }


        public async Task<CosmosDocUser> FindUserByUserName(string UserName, List<string> CallTrace)
        {

            return await cosmosDBRepo.FindFirstItemsAsync(x => x.DocType == (int)DocTypeEnum.User && x.IUD < 2 &&
            x.UserName.ToLower() == UserName.ToLower(),
            LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

        }




        public async Task<CosmosDocUser> FindUserByEmail(string Email, List<string> CallTrace)
        {

            return await cosmosDBRepo.FindFirstItemsAsync(x => x.DocType == (int)DocTypeEnum.User && x.IUD < 2 &&
            x.Email.ToLower() == Email.ToLower(),
            LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

        }


        public async Task<CosmosDocUser> FindUserByID(Guid id, List<string> CallTrace)
        {
            string pkvalue = PartitionKeyGenerator.Create(pkPrefix, id.ToString());
            return await cosmosDBRepo.FindFirstItemsAsync(x => x.DocType == (int)DocTypeEnum.User && x.IUD < 2 &&
            x.ID == id && x.PK == pkvalue,
            LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

        }

        public async Task<List<TSUser>> GetAllUsers(List<string> CallTrace)
        {
            List<TSUser> TsUsers = new List<TSUser>();
            try
            {
                IEnumerable<CosmosDocUser> result = await cosmosDBRepo.GetItemsAsync(x => x.DocType == (int)DocTypeEnum.User && x.IUD < 2, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

                foreach (var item in result)
                {
                    TsUsers.Add(item.toTSUser());
                }

            }
            catch (CosmosException ex)
            {

                await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, ex.Message, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
            }

            return TsUsers;
        }


        public async Task<List<TSUserOpen>> GetLiveUsers(List<string> CallTrace)
        {
            List<TSUserOpen> r = new List<TSUserOpen>();

            try
            {
                IEnumerable<CosmosDocUser> result = await cosmosDBRepo.GetItemsAsync(x => x.DocType == (int)DocTypeEnum.User && x.IUD < 2 && x.IsLive, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));


                foreach (var item in result)
                {
                    r.Add(item.toTSUserOpen());
                }


                return r;


            }
            catch (CosmosException ex)
            {

                await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, ex.Message, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

                return new List<TSUserOpen>();
            }

        }

        public async Task<bool> UpdateOfflineUsers(List<string> CallTrace)
        {


            try
            {
                IEnumerable<CosmosDocUser> result = await cosmosDBRepo.GetItemsAsync(x => x.DocType == (int)DocTypeEnum.User && x.IUD < 2 && x.IsLive && x.TimeStamp < GlobalFunctions.ToUnixEpochDate(DateTime.Now.AddSeconds(-30)),
                    LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

                foreach (var item in result)
                {
                    item.IsLive = false;

                    await CosmosAPI.cosmosDBClientUser.UpdateUserDoc(item, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
                }

            }
            catch (CosmosException ex)
            {

                await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, ex.Message, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

                return false;
            }


            return true;



        }

        public async Task<bool> UpdateOnlineUsersCount(List<string> CallTrace)
        {


            try
            {
                int count = (await cosmosDBRepo.GetItemsAsync(x=>x.DocType == (int)DocTypeEnum.User && x.IsLive, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()))).Count();
                await CosmosAPI.cosmosDBClientSetting.SetSetting(Guid.Empty, "LiveUsersCount", count.ToString(), LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
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
