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


        private readonly List<WebApiUserTypesEnum> AllowedRoles = new List<WebApiUserTypesEnum>
        {
                WebApiUserTypesEnum.Authorized, WebApiUserTypesEnum.Admin
        };


        [FunctionName("FunCategoryGetAll")]
        public async Task<ActionResult<IEnumerable<TSCategory>>> GetAll(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "category/getall")] HttpRequest req)
        {

            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, AllowedRoles, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            await CosmosAPI.cosmosDBClientActivity.AddActivityLog(Guid.Empty, "Requested public data", TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            Guid UserID = Guid.Parse(LocalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod())));
          
            await CosmosAPI.cosmosDBClientActivity.AddActivityLog(UserID, "Requested Categories", TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            return await CosmosAPI.cosmosDBClientCategory.GetAllCategories(UserID, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));


        }

        [FunctionName("FunCategoryGet")]
        public async Task<ActionResult<TSCategory>> Get(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "category/get")] HttpRequest req)
        {

            TSCategory tsCategory = await MyFromBody<TSCategory>.FromBody(req, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, AllowedRoles, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));


            Guid UserID = Guid.Parse(LocalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod())));
            await CosmosAPI.cosmosDBClientActivity.AddActivityLog(UserID, "Requested Category", TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));



            return await CosmosAPI.cosmosDBClientCategory.GetCategory(tsCategory, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));


        }


        [FunctionName("FunCategoryAdd")]
        public async Task<ActionResult> Add(
         [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "category/add")] HttpRequest req)
        {

            TSCategory tsCategory = await MyFromBody<TSCategory>.FromBody(req, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, AllowedRoles, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            Guid userID = Guid.Parse(LocalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod())));
            //string userName = LocalFunctions.CmdGetValueFromClaim(User.Claims, "UserName", 10, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
            await CosmosAPI.cosmosDBClientActivity.AddActivityLog(userID, "post Category", TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

           
            tsCategory.ID = Guid.NewGuid();

            bool b = await CosmosAPI.cosmosDBClientCategory.AddCategory(tsCategory, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            if (b)
            {
                return new OkObjectResult("OK");
            }
            else
            {
                return new OkObjectResult("Error:Can't add new Category!");
            }

        }



        [FunctionName("FunCategoryUpdate")]
        public async Task<ActionResult> Update(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "category/update")] HttpRequest req)
        {

            TSCategory tsCategory = await MyFromBody<TSCategory>.FromBody(req, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, AllowedRoles, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));


            Guid UserID = Guid.Parse(LocalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod())));
            await CosmosAPI.cosmosDBClientActivity.AddActivityLog(UserID, "put Category", TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

           

            bool b = await CosmosAPI.cosmosDBClientCategory.UpdateCategory(tsCategory, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            if (b)
            {

                return new OkObjectResult("OK");
            }
            else
            {
                return new OkObjectResult("Error:Can't add new Category!");
            }
        }



        [FunctionName("FunCategoryDelete")]
        public async Task<ActionResult> Delete(
      [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "category/delete")] HttpRequest req)
        {

            TSCategory tsCategory = await MyFromBody<TSCategory>.FromBody(req, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, AllowedRoles, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            Guid userID = Guid.Parse(LocalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod())));

            await CosmosAPI.cosmosDBClientActivity.AddActivityLog(userID, "delete Category", TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

           

            bool b = await CosmosAPI.cosmosDBClientCategory.DeleteCategory(tsCategory, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

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
