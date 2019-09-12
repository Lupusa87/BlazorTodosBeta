using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos;
using System.Reflection;
using System.Collections.Generic;
using TodosShared;
using System.Security.Claims;
using TodosFunctionsApi.JwtSecurity;
using static TodosShared.TSEnums;
using static TodosGlobal.GlobalClasses;
using TodosCosmos;
using TodosGlobal;

namespace TodosFunctionsApi.API
{
    public class FunCategory
    {


        private List<WebApiUserTypesEnum> AllowedRoles = new List<WebApiUserTypesEnum>
        {
                WebApiUserTypesEnum.Authorized, WebApiUserTypesEnum.Admin
        };


        [FunctionName("FunCaregoryGetAll")]
        public async Task<ActionResult<IEnumerable<TSCategory>>> GetAll(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "category/getall")] HttpRequest req,
            ILogger log)
        {

            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, AllowedRoles);



            await CosmosAPI.cosmosDBClientActivity.AddActivityLog(Guid.Empty, "Requested public data", MethodBase.GetCurrentMethod());

            Guid UserID = Guid.Parse(LocalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10));
            await CosmosAPI.cosmosDBClientActivity.AddActivityLog(UserID, "Requested Categories", MethodBase.GetCurrentMethod());



                return await CosmosAPI.cosmosDBClientCategory.GetAllCategories(UserID);


        }

        [FunctionName("FunCaregoryGet")]
        public async Task<ActionResult<TSCategory>> Get(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "category/get")] HttpRequest req,
           ILogger log)
        {

            TSCategory tsCategory = await MyFromBody<TSCategory>.FromBody(req);

            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, AllowedRoles);


            Guid UserID = Guid.Parse(LocalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10));
            await CosmosAPI.cosmosDBClientActivity.AddActivityLog(UserID, "Requested Category", MethodBase.GetCurrentMethod());



                return await CosmosAPI.cosmosDBClientCategory.GetCategory(tsCategory);


        }


        [FunctionName("FunCaregoryAdd")]
        public async Task<ActionResult> Add(
         [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "category/add")] HttpRequest req,
         ILogger log)
        {

            TSCategory tsCategory = await MyFromBody<TSCategory>.FromBody(req);

            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, AllowedRoles);

            Guid userID = Guid.Parse(LocalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10));
            string userName = LocalFunctions.CmdGetValueFromClaim(User.Claims, "UserName", 10);
            await CosmosAPI.cosmosDBClientActivity.AddActivityLog(userID, "post Category", MethodBase.GetCurrentMethod());

           
            tsCategory.ID = Guid.NewGuid();

            bool b = await CosmosAPI.cosmosDBClientCategory.AddCategory(tsCategory);

            if (b)
            {

                await TodosCosmos.LocalFunctions.NotifyAdmin("New category " + userName + " " + tsCategory.Name);

                return new OkObjectResult("OK");
            }
            else
            {
                return new OkObjectResult("Error:Can't add new Category!");
            }

        }



        [FunctionName("FunCaregoryUpdate")]
        public async Task<ActionResult> Update(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "category/update")] HttpRequest req,
        ILogger log)
        {

            TSCategory tsCategory = await MyFromBody<TSCategory>.FromBody(req);

            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, AllowedRoles);


            Guid UserID = Guid.Parse(LocalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10));
            await CosmosAPI.cosmosDBClientActivity.AddActivityLog(UserID, "put Category", MethodBase.GetCurrentMethod());

           

            bool b = await CosmosAPI.cosmosDBClientCategory.UpdateCategory(tsCategory);

            if (b)
            {

                return new OkObjectResult("OK");
            }
            else
            {
                return new OkObjectResult("Error:Can't add new Category!");
            }
        }



        [FunctionName("FunCaregoryDelete")]
        public async Task<ActionResult> Delete(
      [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "category/delete")] HttpRequest req,
      ILogger log)
        {

            TSCategory tsCategory = await MyFromBody<TSCategory>.FromBody(req);

            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, AllowedRoles);

            Guid userID = Guid.Parse(LocalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10));

            await CosmosAPI.cosmosDBClientActivity.AddActivityLog(userID, "delete Category", MethodBase.GetCurrentMethod());

           

            bool b = await CosmosAPI.cosmosDBClientCategory.DeleteCategory(tsCategory);

            if (b)
            {
                return new OkObjectResult("OK");
            }
            else
            {
                return new OkObjectResult("Error:Can't add new Category!");
            }
        }
    }
}
