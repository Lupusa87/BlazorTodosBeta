using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using TodosCosmos;
using TodosCosmos.DocumentClasses;
using TodosFunctionsApi.JwtSecurity;
using TodosShared;
using static TodosShared.TSEnums;

namespace TodosFunctionsApi.API
{

    public class FunAppVersion
    {

        private List<WebApiUserTypesEnum> AllowedRoles = new List<WebApiUserTypesEnum>
            {
                WebApiUserTypesEnum.NotAuthorized,
                WebApiUserTypesEnum.Authorized,
                WebApiUserTypesEnum.Admin
            };




        [FunctionName("FunAppVersion")]
        public async Task<ActionResult<TSAppVersion>> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "appversion/get")] HttpRequest req,
            ILogger log)
        {


            MyTokenValidator.Authenticate(req, AllowedRoles, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            TSAppVersion appVersion = new TSAppVersion
            {
                VersionDate = new DateTime(),
                VersionNumber = "0.0.0"
            };

            CosmosDocSetting setting = await CosmosAPI.cosmosDBClientSetting.GetSetting(Guid.Empty, "AppVersion", TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));


            if (setting != null)
            {
                if (!string.IsNullOrEmpty(setting.Value))
                {
                   return await LocalFunctions.ParseAppVersion(setting.Value, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
                }
               
            }
           


            return appVersion;
            

        }




    }

}
