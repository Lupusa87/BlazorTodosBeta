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


        public async Task<bool> AddUser(TSUser tsUser)
        {
            try
            {

                CosmosDocUser newUser = new CosmosDocUser(tsUser)
                {
                    Salt = GlobalFunctions.GetSalt()
                };
                newUser.HashedPassword = GlobalFunctions.CmdHashPasswordBytes(tsUser.Password, newUser.Salt);


                await cosmosDBClientBase.AddItemAsync(newUser);

                await CosmosAPI.cosmosDBClientCategory.AddDefaultCategory(tsUser.ID);

                return true;


            }
            catch (CosmosException ex)
            {


                await CosmosAPI.cosmosDBClientError.AddErrorLog(tsUser.ID, ex.Message, MethodBase.GetCurrentMethod());

                return false;
            }
        }





        public async Task<bool> AddUsersBath(List<TSUser> TsUsers)
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

                    await cosmosDBClientBase.AddItemAsync(a);
                }

               

                return true;

            }
            catch (CosmosException ex)
            {


                await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, ex.Message, MethodBase.GetCurrentMethod());

                return false;
            }
        }

        public async Task<bool> UpdateUserDoc(CosmosDocUser tsUser)
        {
            return await cosmosDBClientBase.UpdateItemAsync(tsUser);

        }

        public async Task<bool> UpdateUser(TSUser tsUser, bool KeepPassAndSalt)
        {
            try
            {

                CosmosDocUser newUser = new CosmosDocUser(tsUser);

                if (KeepPassAndSalt)
                {
                    CosmosDocUser oldUser = await GetUserDoc(tsUser);
                    newUser.Salt = oldUser.Salt;
                    newUser.HashedPassword = oldUser.HashedPassword;
                }
                else
                {
                    newUser.Salt = GlobalFunctions.GetSalt();
                    newUser.HashedPassword = GlobalFunctions.CmdHashPasswordBytes(tsUser.Password, newUser.Salt);
                }

               


                await cosmosDBClientBase.UpdateItemAsync(newUser);


                return true;

            }
            catch (CosmosException ex)
            {


                await CosmosAPI.cosmosDBClientError.AddErrorLog(tsUser.ID, ex.Message, MethodBase.GetCurrentMethod());

                return false;
            }
        }


        public async Task<bool> DeleteUser(TSUser tsUser)
        {
            return await cosmosDBClientBase.DeleteItemAsync(new CosmosDocUser(tsUser), pkPrefix);
        }

        public async Task<CosmosDocUser> GetUserDoc(TSUser tsUser)
        {

            return await cosmosDBClientBase.GetItemAsync(new CosmosDocUser(tsUser), pkPrefix);
        }


        public async Task<TSUser> GetUser(TSUser tsUser)
        {

            return (await cosmosDBClientBase.GetItemAsync(new CosmosDocUser(tsUser), pkPrefix)).toTSUser();
        }

        public async Task<TSUser> GetUser(Guid id)
        {

            return (await cosmosDBClientBase.GetItemAsync(id, id, pkPrefix)).toTSUser();
        }


        public async Task<bool> UpdateUserTodosCount(TSUser tsUser, int change)
        {
            TSUser result = await GetUser(tsUser);

            result.TodosCount += change;

            await UpdateUser(result, true);

            return true;
        }


        public async Task<CosmosDocUser> FindUser(string SearchCrtiteria, string column)
        {
            try
            {
                QueryDefinition sql = new QueryDefinition("SELECT * FROM c WHERE c.DocType = " + (int)DocTypeEnum.User + " and c." + column + "='" + SearchCrtiteria + "'");

                return await cosmosDBRepo.FindFirstItemsAsync(sql);
            }
            catch (CosmosException ex)
            {
                await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, ex.Message, MethodBase.GetCurrentMethod());

                return null;
            }
        }


        public async Task<CosmosDocUser> FindUserByUserName(string UserName)
        {
            return await cosmosDBRepo.FindFirstItemsAsync(x => x.DocType == (int)DocTypeEnum.User &&
            x.UserName.ToLower() == UserName.ToLower());

        }




        public async Task<CosmosDocUser> FindUserByEmail(string Email)
        {

            return await cosmosDBRepo.FindFirstItemsAsync(x => x.DocType == (int)DocTypeEnum.User &&
            x.Email.ToLower() == Email.ToLower());

        }


        public async Task<CosmosDocUser> FindUserByID(Guid id)
        {
            string pkvalue = PartitionKeyGenerator.Create(pkPrefix, id.ToString());
            return await cosmosDBRepo.FindFirstItemsAsync(x => x.DocType == (int)DocTypeEnum.User &&
            x.ID == id && x.PK == pkvalue);

        }

        public async Task<List<TSUser>> GetAllUsers()
        {
            List<TSUser> TsUsers = new List<TSUser>();
            try
            {
                IEnumerable<CosmosDocUser> result = await cosmosDBRepo.GetItemsAsync(x => x.DocType == (int)DocTypeEnum.User);

                foreach (var item in result)
                {
                    TsUsers.Add(item.toTSUser());
                }

            }
            catch (CosmosException ex)
            {

                await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, ex.Message, MethodBase.GetCurrentMethod());
            }

            return TsUsers;
        }


        public async Task<List<TSUserOpen>> GetLiveUsers()
        {
            List<TSUserOpen> r = new List<TSUserOpen>();

            try
            {
                IEnumerable<CosmosDocUser> result = await cosmosDBRepo.GetItemsAsync(x => x.DocType == (int)DocTypeEnum.User && x.IsLive);


                foreach (var item in result)
                {
                    r.Add(item.toTSUserOpen());
                }


                return r;


            }
            catch (CosmosException ex)
            {

                await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, ex.Message, MethodBase.GetCurrentMethod());

                return new List<TSUserOpen>();
            }

        }

        public async Task<bool> UpdateOfflineUsers()
        {


            try
            {
                IEnumerable<CosmosDocUser> result = await cosmosDBRepo.GetItemsAsync(x => x.DocType == (int)DocTypeEnum.User && x.IsLive && x.TimeStamp < GlobalFunctions.ToUnixEpochDate(DateTime.Now.AddSeconds(-30)));

                    foreach (var item in result)
                    {
                        item.IsLive = false;
                        await CosmosAPI.cosmosDBClientUser.UpdateUserDoc(item);
                    }
 
            }
            catch (CosmosException ex)
            {

                await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, ex.Message, MethodBase.GetCurrentMethod());

                return false;
            }


            return true;



        }

        public async Task<bool> UpdateOnlineUsersCount()
        {


            try
            {
                int count = (await cosmosDBRepo.GetItemsAsync(x=>x.DocType == (int)DocTypeEnum.User && x.IsLive)).Count();
                await CosmosAPI.cosmosDBClientSetting.SetSetting(Guid.Empty, "LiveUsersCount", count.ToString());
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
