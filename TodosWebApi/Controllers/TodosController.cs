using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TodosShared;
using TodosWebApi.DataLayer;
using TodosWebApi.GlobalDataLayer;
using static TodosWebApi.GlobalDataLayer.GlobalClasses;

namespace TodosWebApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,AutorizedUser,NotAutorizedUser")]
    [ApiController]
    public class TodosController : ControllerBase
    {


        private readonly TableStorage TS;

        public TodosController()
        {
            TS = new TableStorage();
        }


        [Route("GetAll")]
        [HttpGet]
        [Authorize(Roles = "AutorizedUser")]
        public async Task<ActionResult<IEnumerable<TSTodo>>> GetAll()
        {
            string UserID = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10);
            await TS.AddActivityLog(UserID, "Requested todos", MethodBase.GetCurrentMethod());


            SymmKeyAndIV ClientSymmKeyAndIV = new SymmKeyAndIV
            {
                Key = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "ClientSymmKey", 5),
                IV = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "ClientSymmIV", 10)
            };


            if (!string.IsNullOrEmpty(ClientSymmKeyAndIV.Key))
            {

                List<TSTodo> list = await TS.GetAllTodos(GlobalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10));


                foreach (var item in list)
                {
                    GlobalFunctions.CmdEncryptEntitySymm(item, ClientSymmKeyAndIV);
                }

                return list;
            }
            else
            {
                return new List<TSTodo>();
            }

        }


        [HttpGet]
        [Authorize(Roles = "AutorizedUser")]
        public async Task<ActionResult<TSTodo>> Get([FromBody] TSTodo TSTodo)
        {
            string UserID = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10);
            await TS.AddActivityLog(UserID, "Requested todo", MethodBase.GetCurrentMethod());

            SymmKeyAndIV ClientSymmKeyAndIV = new SymmKeyAndIV
            {
                Key = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "ClientSymmKey", 5),
                IV = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "ClientSymmIV", 10)
            };


            if (!string.IsNullOrEmpty(ClientSymmKeyAndIV.Key))
            {

                TSTodo result = await TS.GetTodo(TSTodo);

                GlobalFunctions.CmdEncryptEntitySymm(result, ClientSymmKeyAndIV);
                

                return result;
            }
            else
            {
                return new TSTodo();
            }
        }

        // POST api/values
        [HttpPost]
        [Authorize(Roles = "AutorizedUser")]
        public async Task<ActionResult> Post([FromBody] TSTodo TSTodo)
        {
            string userID = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10);
            string userName = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "UserName", 10);
            await TS.AddActivityLog(userID, "post todo", MethodBase.GetCurrentMethod());

            GlobalFunctions.CmdDecryptEntityAsymm(TSTodo);
            string a = await TS.GetNewID(TSTodo.UserID, "LastTodoID", false);
            TSTodo.TodoID = int.Parse(a);
            TSTodo.CreateDate = DateTime.Now;


            bool b = await TS.AddTodo(TSTodo);

            if (b)
            {

                await GlobalFunctions.NotifyAdmin("New todo " + userName);


                TSUser currUser = new TSUser()
                {
                    UserID = userID,
                    UserName = userName,
                };

                await TS.UpdateUserTodosCount(currUser,1);
                await TS.UpdateSettingCounter("AllUsers", "TodosCount", true);
                return Ok("OK");
            }
            else
            {
                return Ok("Error:Can't add new todo!");
            }

        }

        [HttpPut]
        [Authorize(Roles = "AutorizedUser")]
        public async Task<ActionResult> Put([FromBody] TSTodo TSTodo)
        {

            string UserID = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10);
            await TS.AddActivityLog(UserID, "put todo", MethodBase.GetCurrentMethod());

            GlobalFunctions.CmdDecryptEntityAsymm(TSTodo);

            bool b = await TS.UpdateTodo(TSTodo);

            if (b)
            {

                return Ok("OK");
            }
            else
            {
                return Ok("Error:Can't add new todo!");
            }
        }

        [HttpDelete]
        [Authorize(Roles = "AutorizedUser")]
        public async Task<ActionResult> Delete([FromBody] TSTodo TSTodo)
        {
            string userID = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10);
            string userName = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "UserName", 10);

            await TS.AddActivityLog(userID, "delete todo", MethodBase.GetCurrentMethod());

            GlobalFunctions.CmdDecryptEntityAsymm(TSTodo);

            bool b = await TS.DeleteTodo(TSTodo);

            if (b)
            {
                TSUser currUser = new TSUser()
                {
                    UserID = userID,
                    UserName = userName,
                };

                await TS.UpdateUserTodosCount(currUser, 1);

                await TS.UpdateSettingCounter("AllUsers", "TodosCount", false);
                return Ok("OK");
            }
            else
            {
                return Ok("Error:Can't add new todo!");
            }
        }
    }
}
