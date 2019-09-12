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

            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, AllowedRoles);

            return await CosmosAPI.cosmosDBClientUILanguage.GetAll();
        }



        [FunctionName("FunUILanguageGet")]
        public async Task<ActionResult<TSUILanguage>> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "uilanguage/get")] HttpRequest req,
            ILogger log)
        {

            TSUILanguage tsUILanguage = await MyFromBody<TSUILanguage>.FromBody(req);


            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, AllowedRoles);


            TSUILanguage result = await CosmosAPI.cosmosDBClientUILanguage.Get(tsUILanguage);



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

            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, localAllowedRoles);

            TSUILanguage tsUILanguage = await MyFromBody<TSUILanguage>.FromBody(req);



            CosmosDocUILanguage l = await CosmosAPI.cosmosDBClientUILanguage.FindByName(tsUILanguage.Name);


            if (l is null)
            {

                bool b = await CosmosAPI.cosmosDBClientUILanguage.Add(tsUILanguage);

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

            TSUILanguage tsUILanguage = await MyFromBody<TSUILanguage>.FromBody(req);

            List<WebApiUserTypesEnum> localAllowedRoles = new List<WebApiUserTypesEnum>
            {
                WebApiUserTypesEnum.Admin
            };

            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, localAllowedRoles);


            bool b = await CosmosAPI.cosmosDBClientUILanguage.Update(tsUILanguage);

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

            TSUILanguage tsUILanguage = await MyFromBody<TSUILanguage>.FromBody(req);

            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, AllowedRoles);



            bool b = await CosmosAPI.cosmosDBClientUILanguage.Delete(tsUILanguage);

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
