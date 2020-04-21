using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TodosCosmos;
using TodosFunctionsApi.JwtSecurity;
using TodosGlobal;
using TodosShared;
using static TodosGlobal.GlobalClasses;
using static TodosShared.TSEnums;

namespace TodosFunctionsApi.API
{

    public class FunTodo
    {

       

        private readonly List<WebApiUserTypesEnum> AllowedRoles = new List<WebApiUserTypesEnum>
            {
                WebApiUserTypesEnum.Authorized, WebApiUserTypesEnum.Admin
            };

           


        [FunctionName("FunTodoGetAll")]
        public async Task<ActionResult<IEnumerable<TSTodo>>> GetAll(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo/getall")] HttpRequest req)
        {

            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, AllowedRoles, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));


            Guid UserID = Guid.Parse(LocalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod())));
            await CosmosAPI.cosmosDBClientActivity.AddActivityLog(UserID, "Requested todos", TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));




            return await CosmosAPI.cosmosDBClientTodo.GetAllTodos(UserID, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

           

        }


        [FunctionName("FunTodoGet")]
        public async Task<ActionResult<TSTodo>> Get(
         [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo/get")] HttpRequest req)
        {

            TSTodo tsTodo = await MyFromBody<TSTodo>.FromBody(req, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, AllowedRoles, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));


            Guid UserID = Guid.Parse(LocalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod())));

            await CosmosAPI.cosmosDBClientActivity.AddActivityLog(UserID, "Requested todo", TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));



            return await CosmosAPI.cosmosDBClientTodo.GetTodo(tsTodo, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
            
        }


        [FunctionName("FunTodoAdd")]
        public async Task<ActionResult> Add(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "todo/add")] HttpRequest req)
        {

            TSTodo tsTodo = await MyFromBody<TSTodo>.FromBody(req, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, AllowedRoles, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            Guid userID = Guid.Parse(LocalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod())));
            string userName = LocalFunctions.CmdGetValueFromClaim(User.Claims, "UserName", 10, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
            
            await CosmosAPI.cosmosDBClientActivity.AddActivityLog(userID, "post todo", TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

         
            tsTodo.ID = Guid.NewGuid();
            tsTodo.CreateDate = DateTime.Now;


            bool b = await CosmosAPI.cosmosDBClientTodo.AddTodo(tsTodo, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            if (b)
            {

                TSUser currUser = new TSUser()
                {
                    ID = userID,
                    UserName = userName,
                };

                await CosmosAPI.cosmosDBClientUser.UpdateUserTodosCount(currUser, 1, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
                await CosmosAPI.cosmosDBClientSetting.UpdateSettingCounter(Guid.Empty, "TodosCount", true, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
                return new OkObjectResult("OK");
            }
            else
            {
                return new OkObjectResult("Error:Can't add new todo!");
            }

        }


        [FunctionName("FunTodoUpdate")]
        public async Task<ActionResult> Update(
       [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "todo/update")] HttpRequest req)
        {

            TSTodo tsTodo = await MyFromBody<TSTodo>.FromBody(req, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, AllowedRoles, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));


            Guid UserID = Guid.Parse(LocalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod())));

            await CosmosAPI.cosmosDBClientActivity.AddActivityLog(UserID, "put todo", TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));


          

            bool b = await CosmosAPI.cosmosDBClientTodo.UpdateTodo(tsTodo, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            if (b)
            {

                return new OkObjectResult("OK");
            }
            else
            {
                return new OkObjectResult("Error:Can't add new todo!");
            }
        }


        [FunctionName("FunTodoDelete")]
        public async Task<ActionResult> Delete(
     [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "todo/delete")] HttpRequest req)
        {

            TSTodo tsTodo = await MyFromBody<TSTodo>.FromBody(req, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, AllowedRoles, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            Guid userID = Guid.Parse(LocalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod())));
            string userName = LocalFunctions.CmdGetValueFromClaim(User.Claims, "UserName", 10, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            await CosmosAPI.cosmosDBClientActivity.AddActivityLog(userID, "delete todo", TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

         
           

            bool b = await CosmosAPI.cosmosDBClientTodo.DeleteTodo(tsTodo, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            if (b)
            {

                TSUser currUser = new TSUser()
                {
                    ID = userID,
                    UserName = userName,
                };

                await CosmosAPI.cosmosDBClientUser.UpdateUserTodosCount(currUser, -1, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

                await CosmosAPI.cosmosDBClientSetting.UpdateSettingCounter(Guid.Empty, "TodosCount", false, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
                return new OkObjectResult("OK");
            }
            else
            {
                return new OkObjectResult("Error:Can't add new todo!");
            }
        }
    }
}
