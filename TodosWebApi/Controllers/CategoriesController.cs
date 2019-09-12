using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TodosShared;
using TodosWebApi.DataLayer;
using TodosWebApi.GlobalDataLayer;
using static TodosWebApi.GlobalDataLayer.GlobalClasses;

namespace CategoriesWebApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,AutorizedUser,NotAutorizedUser")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {

        private readonly TableStorage TS;

        public CategoriesController()
        {
            TS = new TableStorage();
        }

     
        [Route("GetAll")]
        [HttpGet]
        [Authorize(Roles = "AutorizedUser")]
        public async Task<ActionResult<IEnumerable<TSCategory>>> GetAll()
        {
            string UserID = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10);
            await TS.AddActivityLog(UserID, "Requested Categories", MethodBase.GetCurrentMethod());


            SymmKeyAndIV ClientSymmKeyAndIV = new SymmKeyAndIV
            {
                Key = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "ClientSymmKey", 5),
                IV = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "ClientSymmIV", 10)
            };


            if (!string.IsNullOrEmpty(ClientSymmKeyAndIV.Key))
            {

                List<TSCategory> list = await TS.GetAllCategories(GlobalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10));

                foreach (var item in list)
                {
                    GlobalFunctions.CmdEncryptEntitySymm(item, ClientSymmKeyAndIV);
                }

                return list;
            }
            else
            {
                return new List<TSCategory>();
            }

        }

      
        [HttpGet]
        [Authorize(Roles = "AutorizedUser")]
        public async Task<ActionResult<TSCategory>> Get([FromBody] TSCategory TSCategory)
        {
            string UserID = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10);
            await TS.AddActivityLog(UserID, "Requested Category", MethodBase.GetCurrentMethod());

            SymmKeyAndIV ClientSymmKeyAndIV = new SymmKeyAndIV
            {
                Key = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "ClientSymmKey", 5),
                IV = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "ClientSymmIV", 10)
            };


            if (!string.IsNullOrEmpty(ClientSymmKeyAndIV.Key))
            {

                TSCategory result = await TS.GetCategory(TSCategory);

                GlobalFunctions.CmdEncryptEntitySymm(result, ClientSymmKeyAndIV);


                return result;
            }
            else
            {
                return new TSCategory();
            }

        }

        // POST api/values
        [HttpPost]
        [Authorize(Roles = "AutorizedUser")]
        public async Task<ActionResult> Post([FromBody] TSCategory TSCategory)
        {
            string userID = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10);
            string userName = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "UserName", 10);
            await TS.AddActivityLog(userID, "post Category", MethodBase.GetCurrentMethod());

            GlobalFunctions.CmdDecryptEntityAsymm(TSCategory);
            string a = await TS.GetNewID(TSCategory.UserID, "LastCategoryID", false);
            TSCategory.CategoryID = int.Parse(a);

            bool b = await TS.AddCategory(TSCategory);

            if (b)
            {

                await GlobalFunctions.NotifyAdmin("New category " + userName + " " + TSCategory.Name);

                return Ok("OK");
            }
            else
            {
                return Ok("Error:Can't add new Category!");
            }

        }

        [HttpPut]
        [Authorize(Roles = "AutorizedUser")]
        public async Task<ActionResult> Put([FromBody] TSCategory TSCategory)
        {


            string UserID = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10);
            await TS.AddActivityLog(UserID, "put Category", MethodBase.GetCurrentMethod());

            GlobalFunctions.CmdDecryptEntityAsymm(TSCategory);

            bool b = await TS.UpdateCategory(TSCategory);

            if (b)
            {

                return Ok("OK");
            }
            else
            {
                return Ok("Error:Can't add new Category!");
            }
        }

        [HttpDelete]
        [Authorize(Roles = "AutorizedUser")]
        public async Task<ActionResult> Delete([FromBody] TSCategory TSCategory)
        {
            string userID = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10);
            string userName = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "UserName", 10);

            await TS.AddActivityLog(userID, "delete Category", MethodBase.GetCurrentMethod());

            GlobalFunctions.CmdDecryptEntityAsymm(TSCategory);

            bool b = await TS.DeleteCategory(TSCategory);

            if (b)
            {
                return Ok("OK");
            }
            else
            {
                return Ok("Error:Can't add new Category!");
            }
        }
    }
}
