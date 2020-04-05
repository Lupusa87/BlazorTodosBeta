using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TodosCosmos;
using TodosCosmos.DocumentClasses;
using TodosFunctionsApi.JwtSecurity;
using TodosShared;
using static TodosShared.TSEnums;

namespace TodosFunctionsApi.API
{

    public class FunStat
    {

       

        private List<WebApiUserTypesEnum> AllowedRoles = new List<WebApiUserTypesEnum>
            {
                WebApiUserTypesEnum.NotAuthorized,
                WebApiUserTypesEnum.Authorized,
                WebApiUserTypesEnum.Admin
            };

          


        [FunctionName("FunStat")]
        public async Task<ActionResult<TSStat>> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "stat")] HttpRequest req,
            ILogger log)
        {


            await CosmosAPI.cosmosDBClientSetting.SetSetting(Guid.Empty, "LatsStatRequest", "ts", TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, AllowedRoles, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            if (User != null)
            {
                Guid userID = Guid.Parse(LocalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod())));
                string userName = LocalFunctions.CmdGetValueFromClaim(User.Claims, "UserName", 10, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

                if (!userID.Equals(Guid.Empty))
                {
                    if (!userName.Equals("demouser"))
                    {
                        

                        TSUser tsUser = await CosmosAPI.cosmosDBClientUser.GetUser(new TSUser {  ID = userID}, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

                        if (tsUser != null)
                        {
                            if (!tsUser.IsLive)
                            {
                                tsUser.IsLive = true;
                                await CosmosAPI.cosmosDBClientUser.UpdateUser(tsUser, true, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
                            }
                        }
                    }
                }
            }
           
            TSStat stat =  new TSStat
            {
                IPsCount = await GetStat("IPAddressesCount"),
                VisitsCount = await GetStat("VisitsCount"),
                UsersCount = await GetStat("UsersCount"),
                LiveUsersCount = await GetStat("LiveUsersCount"),
                TodosCount = await GetStat("TodosCount"),
                LikesCount = await GetStat("LikesCount"),
                DislikesCount = await GetStat("DislikesCount"),
                FeedbackCount = await GetStat("FeedbackCount"),
            };



            return stat;
           // return new OkObjectResult(JsonSerializer.ToString(stat));

        }

        private async Task<int> GetStat(string name)
        {

            CosmosDocSetting setting = await CosmosAPI.cosmosDBClientSetting.GetSetting(Guid.Empty, name, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            
            if (setting != null)
            {

                if (string.IsNullOrEmpty(setting.Value))
                {
                    return 0;
                }
                else
                {
                    return int.Parse(setting.Value);
                }
            }
            else
            {
                return 0;
            }

        }


    }
}
