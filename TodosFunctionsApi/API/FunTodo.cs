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
using TodosFunctionsApi.JwtSecurity;
using TodosGlobal;
using TodosShared;
using static TodosGlobal.GlobalClasses;
using static TodosShared.TSEnums;

namespace TodosFunctionsApi.API
{

    public class FunTodo
    {

       

        private List<WebApiUserTypesEnum> AllowedRoles = new List<WebApiUserTypesEnum>
            {
                WebApiUserTypesEnum.Authorized, WebApiUserTypesEnum.Admin
            };

           


        [FunctionName("FunTodoGetAll")]
        public async Task<ActionResult<IEnumerable<TSTodo>>> GetAll(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo/getall")] HttpRequest req,
            ILogger log)
        {

            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, AllowedRoles);


            Guid UserID = Guid.Parse(LocalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10));
            await CosmosAPI.cosmosDBClientActivity.AddActivityLog(UserID, "Requested todos", MethodBase.GetCurrentMethod());




                return await CosmosAPI.cosmosDBClientTodo.GetAllTodos(UserID);

           

        }


        [FunctionName("FunTodoGet")]
        public async Task<ActionResult<TSTodo>> Get(
         [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo/get")] HttpRequest req,
         ILogger log)
        {

            TSTodo tsTodo = await MyFromBody<TSTodo>.FromBody(req);

            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, AllowedRoles);


            Guid UserID = Guid.Parse(LocalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10));

            await CosmosAPI.cosmosDBClientActivity.AddActivityLog(UserID, "Requested todo", MethodBase.GetCurrentMethod());



                return await CosmosAPI.cosmosDBClientTodo.GetTodo(tsTodo);
            
        }


        [FunctionName("FunTodoAdd")]
        public async Task<ActionResult> Add(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "todo/add")] HttpRequest req,
        ILogger log)
        {

            TSTodo tsTodo = await MyFromBody<TSTodo>.FromBody(req);

            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, AllowedRoles);

            Guid userID = Guid.Parse(LocalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10));
            string userName = LocalFunctions.CmdGetValueFromClaim(User.Claims, "UserName", 10);
            
            await CosmosAPI.cosmosDBClientActivity.AddActivityLog(userID, "post todo", MethodBase.GetCurrentMethod());

         
            tsTodo.ID = Guid.NewGuid();
            tsTodo.CreateDate = DateTime.Now;


            bool b = await CosmosAPI.cosmosDBClientTodo.AddTodo(tsTodo);

            if (b)
            {

                await TodosCosmos.LocalFunctions.NotifyAdmin("New todo " + userName);


                TSUser currUser = new TSUser()
                {
                    ID = userID,
                    UserName = userName,
                };

                await CosmosAPI.cosmosDBClientUser.UpdateUserTodosCount(currUser, 1);
                await CosmosAPI.cosmosDBClientSetting.UpdateSettingCounter(Guid.Empty, "TodosCount", true);
                return new OkObjectResult("OK");
            }
            else
            {
                return new OkObjectResult("Error:Can't add new todo!");
            }

        }


        [FunctionName("FunTodoUpdate")]
        public async Task<ActionResult> Update(
       [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "todo/update")] HttpRequest req,
       ILogger log)
        {

            TSTodo tsTodo = await MyFromBody<TSTodo>.FromBody(req);

            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, AllowedRoles);


            Guid UserID = Guid.Parse(LocalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10));

            await CosmosAPI.cosmosDBClientActivity.AddActivityLog(UserID, "put todo", MethodBase.GetCurrentMethod());

          

            bool b = await CosmosAPI.cosmosDBClientTodo.UpdateTodo(tsTodo);

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
     [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "todo/delete")] HttpRequest req,
     ILogger log)
        {

            TSTodo tsTodo = await MyFromBody<TSTodo>.FromBody(req);

            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, AllowedRoles);

            Guid userID = Guid.Parse(LocalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10));
            string userName = LocalFunctions.CmdGetValueFromClaim(User.Claims, "UserName", 10);

            await CosmosAPI.cosmosDBClientActivity.AddActivityLog(userID, "delete todo", MethodBase.GetCurrentMethod());

         

            bool b = await CosmosAPI.cosmosDBClientTodo.DeleteTodo(tsTodo);

            if (b)
            {
                TSUser currUser = new TSUser()
                {
                    ID = userID,
                    UserName = userName,
                };

                await CosmosAPI.cosmosDBClientUser.UpdateUserTodosCount(currUser, -1);

                await CosmosAPI.cosmosDBClientSetting.UpdateSettingCounter(Guid.Empty, "TodosCount", false);
                return new OkObjectResult("OK");
            }
            else
            {
                return new OkObjectResult("Error:Can't add new todo!");
            }
        }
    }
}
