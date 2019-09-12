using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
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

    public class FunUIWordForeign
    {


        private List<WebApiUserTypesEnum> AllowedRoles = new List<WebApiUserTypesEnum>
            {
                WebApiUserTypesEnum.NotAuthorized,
                WebApiUserTypesEnum.Authorized,
                WebApiUserTypesEnum.Admin
            };




        [FunctionName("FunUIWordForeignGetAll")]
        public async Task<ActionResult<IEnumerable<TSUIWordForeign>>> GetAll(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "uiwordforeign/getall")] HttpRequest req,
            ILogger log)
        {

            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, AllowedRoles);

            return await CosmosAPI.cosmosDBClientUIWordForeign.GetAll();
        }


        [FunctionName("FunUIWordForeignGetAllByLang")]
        public async Task<ActionResult<IEnumerable<TSUIWordForeign>>> GetAllByLang(
          [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "uiwordforeign/getallbylang")] HttpRequest req,
          ILogger log)
        {

            TSUIWordForeign tsUIWordForeign = await MyFromBody<TSUIWordForeign>.FromBody(req);


            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, AllowedRoles);


            return await CosmosAPI.cosmosDBClientUIWordForeign.GetAllByLang(tsUIWordForeign.UILanguageID);

        }


        [FunctionName("FunUIWordForeignGet")]
        public async Task<ActionResult<TSUIWordForeign>> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "uiwordforeign/get")] HttpRequest req,
            ILogger log)
        {

            TSUIWordForeign tsUIWordForeign = await MyFromBody<TSUIWordForeign>.FromBody(req);


            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, AllowedRoles);


            TSUIWordForeign result = await CosmosAPI.cosmosDBClientUIWordForeign.Get(tsUIWordForeign);



            if (result is null)
            {
                return new TSUIWordForeign();
            }
            else
            {
                return result;
            }

        }

        [FunctionName("FunUIWordForeignAdd")]
        public async Task<ActionResult> Add(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "uiwordforeign/add")] HttpRequest req,
           ILogger log)
        {

            List<WebApiUserTypesEnum> localAllowedRoles = new List<WebApiUserTypesEnum>
            {
                WebApiUserTypesEnum.Admin
            };

            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, localAllowedRoles);

            TSUIWordForeign tsUIWordForeign = await MyFromBody<TSUIWordForeign>.FromBody(req);



            CosmosDocUIWordForeign l = await CosmosAPI.cosmosDBClientUIWordForeign.FindByWordAndNativeWordID(tsUIWordForeign.Word, tsUIWordForeign.UIWordNativeID);


            if (l is null)
            {

                bool b = await CosmosAPI.cosmosDBClientUIWordForeign.Add(tsUIWordForeign);

                if (b)
                {
                    return new OkObjectResult("OK");
                }
                else
                {
                    return new OkObjectResult("Error:Can't add new UIWordForeign!");
                }
            }
            else
            {
                return new OkObjectResult("Error:UIWordForeign with word - " + tsUIWordForeign.Word + " and same NativeWordID exists already!");
            }

        }



        [FunctionName("FunUIWordForeignUpdate")]
        public async Task<ActionResult> Update(
      [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "uiwordforeign/update")] HttpRequest req,
      ILogger log)
        {

            TSUIWordForeign tsUIWordForeign = await MyFromBody<TSUIWordForeign>.FromBody(req);

            List<WebApiUserTypesEnum> localAllowedRoles = new List<WebApiUserTypesEnum>
            {
                WebApiUserTypesEnum.Admin
            };

            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, localAllowedRoles);


            bool b = await CosmosAPI.cosmosDBClientUIWordForeign.Update(tsUIWordForeign);

            if (b)
            {

                return new OkObjectResult("OK");
            }
            else
            {
                return new OkObjectResult("Error:Can't add new todo!");
            }
        }


        [FunctionName("FunUIWordForeignDelete")]
        public async Task<ActionResult> Delete(
    [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "uiwordforeign/delete")] HttpRequest req,
    ILogger log)
        {

            TSUIWordForeign tsUIWordForeign = await MyFromBody<TSUIWordForeign>.FromBody(req);

            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, AllowedRoles);



            bool b = await CosmosAPI.cosmosDBClientUIWordForeign.Delete(tsUIWordForeign);

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
