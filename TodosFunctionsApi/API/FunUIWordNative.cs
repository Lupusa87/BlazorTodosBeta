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

    public class FunUIWordNative
    {


        private List<WebApiUserTypesEnum> AllowedRoles = new List<WebApiUserTypesEnum>
            {
                WebApiUserTypesEnum.NotAuthorized,
                WebApiUserTypesEnum.Authorized,
                WebApiUserTypesEnum.Admin
            };




        [FunctionName("FunUIWordNativeGetAll")]
        public async Task<ActionResult<IEnumerable<TSUIWordNative>>> GetAll(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "uiwordnative/getall")] HttpRequest req,
            ILogger log)
        {

            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, AllowedRoles);

            return await CosmosAPI.cosmosDBClientUIWordNative.GetAll();
        }




        [FunctionName("FunUIWordNativeGet")]
        public async Task<ActionResult<TSUIWordNative>> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "uiwordnative/get")] HttpRequest req,
            ILogger log)
        {

            TSUIWordNative tsUIWordNative = await MyFromBody<TSUIWordNative>.FromBody(req);


            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, AllowedRoles);


            TSUIWordNative result = await CosmosAPI.cosmosDBClientUIWordNative.Get(tsUIWordNative);



            if (result is null)
            {
                return new TSUIWordNative();
            }
            else
            {
                return result;
            }

        }

        [FunctionName("FunUIWordNativeAdd")]
        public async Task<ActionResult> Add(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "uiwordnative/add")] HttpRequest req,
           ILogger log)
        {

            List<WebApiUserTypesEnum> localAllowedRoles = new List<WebApiUserTypesEnum>
            {
                WebApiUserTypesEnum.Admin
            };

            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, localAllowedRoles);

            TSUIWordNative tsUIWordNative = await MyFromBody<TSUIWordNative>.FromBody(req);



            CosmosDocUIWordNative l = await CosmosAPI.cosmosDBClientUIWordNative.FindByWord(tsUIWordNative.Word);


            if (l is null)
            {

                bool b = await CosmosAPI.cosmosDBClientUIWordNative.Add(tsUIWordNative);

                if (b)
                {
                    return new OkObjectResult("OK");
                }
                else
                {
                    return new OkObjectResult("Error:Can't add new UIWordNative!");
                }
            }
            else
            {
                return new OkObjectResult("Error:UIWordNative with word - " + tsUIWordNative.Word + " exists already!");
            }

        }



        [FunctionName("FunUIWordNativeUpdate")]
        public async Task<ActionResult> Update(
      [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "uiwordnative/update")] HttpRequest req,
      ILogger log)
        {

            TSUIWordNative tsUIWordNative = await MyFromBody<TSUIWordNative>.FromBody(req);

            List<WebApiUserTypesEnum> localAllowedRoles = new List<WebApiUserTypesEnum>
            {
                WebApiUserTypesEnum.Admin
            };

            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, localAllowedRoles);


            bool b = await CosmosAPI.cosmosDBClientUIWordNative.Update(tsUIWordNative);

            if (b)
            {

                return new OkObjectResult("OK");
            }
            else
            {
                return new OkObjectResult("Error:Can't add new todo!");
            }
        }


        [FunctionName("FunUIWordNativeDelete")]
        public async Task<ActionResult> Delete(
    [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "uiwordnative/delete")] HttpRequest req,
    ILogger log)
        {

            TSUIWordNative tsUIWordNative = await MyFromBody<TSUIWordNative>.FromBody(req);

            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, AllowedRoles);



            bool b = await CosmosAPI.cosmosDBClientUIWordNative.Delete(tsUIWordNative);

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
