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
using TodosGlobal;
using TodosShared;
using static TodosCosmos.Enums;
using static TodosGlobal.GlobalClasses;
using static TodosShared.TSEnums;

namespace TodosFunctionsApi.API
{

    public class FunUILanguage
    {


        private List<WebApiUserTypesEnum> AllowedRoles = new List<WebApiUserTypesEnum>
            {
                WebApiUserTypesEnum.NotAuthorized,
                WebApiUserTypesEnum.Authorized,
                WebApiUserTypesEnum.Admin
            };




        [FunctionName("FunUILanguageGetAll")]
        public async Task<ActionResult<IEnumerable<TSUILanguage>>> GetAll(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "uilanguage/getall")] HttpRequest req,
            ILogger log)
        {

            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, AllowedRoles, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            return await CosmosAPI.cosmosDBClientUILanguage.GetAll(TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
        }



        [FunctionName("FunUILanguageGet")]
        public async Task<ActionResult<TSUILanguage>> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "uilanguage/get")] HttpRequest req,
            ILogger log)
        {

            TSUILanguage tsUILanguage = await MyFromBody<TSUILanguage>.FromBody(req, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));


            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, AllowedRoles, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));


            TSUILanguage result = await CosmosAPI.cosmosDBClientUILanguage.Get(tsUILanguage, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));



            if (result is null)
            {
                return new TSUILanguage();
            }
            else
            {
                return result;
            }

        }

        [FunctionName("FunUILanguageAdd")]
        public async Task<ActionResult> Add(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "uilanguage/add")] HttpRequest req,
           ILogger log)
        {

            List<WebApiUserTypesEnum> localAllowedRoles = new List<WebApiUserTypesEnum>
            {
                WebApiUserTypesEnum.Admin
            };

            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, localAllowedRoles, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            TSUILanguage tsUILanguage = await MyFromBody<TSUILanguage>.FromBody(req, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));



            CosmosDocUILanguage l = await CosmosAPI.cosmosDBClientUILanguage.FindByName(tsUILanguage.Name, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));


            if (l is null)
            {

                bool b = await CosmosAPI.cosmosDBClientUILanguage.Add(tsUILanguage, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

                if (b)
                {
                    return new OkObjectResult("OK");
                }
                else
                {
                    return new OkObjectResult("Error:Can't add new UILanguage!");
                }
            }
            else
            {
                return new OkObjectResult("Error:UILanguage with name - " + tsUILanguage.Name + " exists already!");
            }

        }



        [FunctionName("FunUILanguageUpdate")]
        public async Task<ActionResult> Update(
      [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "uilanguage/update")] HttpRequest req,
      ILogger log)
        {

            TSUILanguage tsUILanguage = await MyFromBody<TSUILanguage>.FromBody(req, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            List<WebApiUserTypesEnum> localAllowedRoles = new List<WebApiUserTypesEnum>
            {
                WebApiUserTypesEnum.Admin
            };

            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, localAllowedRoles, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));


            bool b = await CosmosAPI.cosmosDBClientUILanguage.Update(tsUILanguage, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            if (b)
            {

                return new OkObjectResult("OK");
            }
            else
            {
                return new OkObjectResult("Error:Can't add new todo!");
            }
        }


        [FunctionName("FunUILanguageDelete")]
        public async Task<ActionResult> Delete(
    [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "uilanguage/delete")] HttpRequest req,
    ILogger log)
        {

            TSUILanguage tsUILanguage = await MyFromBody<TSUILanguage>.FromBody(req, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, AllowedRoles, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));



            bool b = await CosmosAPI.cosmosDBClientUILanguage.Delete(DocDeleteModeEnum.Soft, tsUILanguage, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            if (b)
            {
                return new OkObjectResult("OK");
            }
            else
            {
                return new OkObjectResult("Error:Can't add new todo!");
            }
        }
    }
}
