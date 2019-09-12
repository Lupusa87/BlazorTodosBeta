using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TodosShared;
using TodosWebApi.DataLayer;
using TodosWebApi.DataLayer.TSEntities;
using TodosWebApi.GlobalDataLayer;
using static TodosShared.TSEnums;
using static TodosWebApi.GlobalDataLayer.GlobalClasses;

namespace TodosWebApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,AutorizedUser,NotAutorizedUser")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        private readonly ILogger _logger;
        private readonly TableStorage TS;

        public UsersController(ILogger<UsersController> logger)
        {
            _logger = logger;
            TS = new TableStorage();
        }

        // GET api/values

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<TSUser>>> Get()
        {

            string UserID = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10);
            await TS.AddActivityLog(UserID, "Requested all users", MethodBase.GetCurrentMethod());

            List<TSUser> users = new List<TSUser>();

            SymmKeyAndIV ClientSymmKeyAndIV = new SymmKeyAndIV();
            ClientSymmKeyAndIV.Key = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "ClientSymmKey", 5);
            ClientSymmKeyAndIV.IV = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "ClientSymmIV", 10);

            if (!ClientSymmKeyAndIV.Key.Equals("Error"))
            {
                users = await TS.GetAllUsers();

                for (int i = 0; i < users.Count; i++)
                {
                    GlobalFunctions.CmdEncryptEntitySymm(users[0], ClientSymmKeyAndIV);
                }
            }
            else
            {
                return BadRequest("Can't get crypto key!");
            }
            
            return users;
        }


        [Route("Authorize")]
        [Authorize(Roles = "NotAutorizedUser")]
        public async Task<ActionResult<TSUser>> Authorize([FromBody] TSUser tsUser)
        {
            string UserID = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10);
            await TS.AddActivityLog(UserID, "Requested authentication", MethodBase.GetCurrentMethod());

            TSUser result = new TSUser();

            SymmKeyAndIV ClientSymmKeyAndIV = new SymmKeyAndIV
            {
                Key = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "ClientSymmKey", 5),
                IV = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "ClientSymmIV", 10)
            };


            if (!string.IsNullOrEmpty(ClientSymmKeyAndIV.Key))
            {

                GlobalFunctions.CmdDecryptEntityAsymm(tsUser);

                tsUser.UserID = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10);

                result = TS.GetUser(tsUser).Result;

                if (!string.IsNullOrEmpty(result.UserID))
                {
                    await GlobalFunctions.NotifyAdmin("New login " + result.UserName);

                    await TS.UpdateSettingCounter("AllUsers", "LiveUsersCount", true);

                    GlobalFunctions.CmdEncryptEntitySymm(result, ClientSymmKeyAndIV);


                    
                }
                else
                {

                    result.UserName = "Error!";
                    result.FullName = "Can't find user!";
                    GlobalFunctions.CmdEncryptEntitySymm(result, ClientSymmKeyAndIV);
                }
                
            }
            else
            {
                result.UserName = "Error!";
                result.FullName = "Can't get crypto key!";
            }

            return result;
        }

        [Route("SendMail")]
        [HttpPost]
        public async Task<IActionResult> SendMail([FromBody]TSEmail tsEmail)
        {
            string UserID = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10);
            await TS.AddActivityLog(UserID, "send mail", MethodBase.GetCurrentMethod());


            string MachineID = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "MachineID", 10);


            GlobalFunctions.CmdDecryptEntityAsymm(tsEmail);


            TSEmail result = await GlobalFunctions.SendEmail(tsEmail, HttpContext.Connection.RemoteIpAddress.ToString(), MachineID);
            result.To = string.Empty;
            result.OperationCode = 0;
            return Ok(result);

        }



        [Route("PassRecovery")]
        [Authorize(Roles = "NotAutorizedUser")]
        [HttpPost]
        public async Task<IActionResult> PassRecovery([FromBody]TSEmail tsEmail)
        {
            string UserID = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10);
            await TS.AddActivityLog(UserID, "Pass Recovery", MethodBase.GetCurrentMethod());

            GlobalFunctions.CmdDecryptEntityAsymm(tsEmail);


            EmailOperationsEnum currOperationCode = (EmailOperationsEnum)tsEmail.OperationCode;


            string newPass = GlobalFunctions.GetSalt();


            TSUserEntity tsUserEntity = TS.FindUser(tsEmail.To, false, string.Empty).Result;
            if (tsUserEntity != null)
            {
                tsUserEntity.Salt = GlobalFunctions.GetSalt();
                tsUserEntity.HashedPassword = GlobalFunctions.CmdHashPasswordBytes(newPass, tsUserEntity.Salt);

                bool b = TS.UpdateUserEntity(tsUserEntity).Result;

                if (b)
                {
                    tsEmail.To = tsUserEntity.Email;
                    TSEmail result = await GlobalFunctions.SendEmail(tsEmail, string.Empty, newPass);
                    result.To = string.Empty;
                    result.OperationCode = 0;
                    return Ok(result);
                }
                else
                {
                    return Ok("Error: Could not recover user password");
                }
            }
            else
            {
                return Ok("Error: User not found");
            }
        }

        [Route("CheckUserName")]
        [HttpPost]
        public async Task<IActionResult> CheckUserName([FromBody]TSEmail tsEmail)
        {
            string UserID = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10);
            await TS.AddActivityLog(UserID, "Check UserName", MethodBase.GetCurrentMethod());


            GlobalFunctions.CmdDecryptEntityAsymm(tsEmail);


            TSUserEntity user = TS.FindUser(tsEmail.To, false, string.Empty).Result;

            TSEmail result = new TSEmail
            {
                To = string.Empty,
                OperationCode = 0
            };

            if (user is null)
            {
                result.Result = "OK";
            }
            else
            {
                result.Result = "UserName already exists";
            }

            return Ok(result);

        }


        [Route("CheckEmail")]
        [HttpPost]
        public async Task<IActionResult> CheckEmail([FromBody]TSEmail tsEmail)
        {
            string UserID = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10);
            await TS.AddActivityLog(UserID, "Check Email", MethodBase.GetCurrentMethod());


            GlobalFunctions.CmdDecryptEntityAsymm(tsEmail);


            TSUserEntity user = TS.FindUser(tsEmail.To, true, "Email").Result;

            TSEmail result = new TSEmail
            {
                To = string.Empty,
                OperationCode = 0
            };

            if (user is null)
            {
                result.Result = "OK";
            }
            else
            {
                result.Result = "Email already exists";
            }

            return Ok(result);

        }


        [Route("Logout")]
        [HttpGet]
        [Authorize(Roles = "AutorizedUser")]
        public async void Logout()
        {
            string UserID = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10);
            await TS.AddActivityLog(UserID, "user logout", MethodBase.GetCurrentMethod());

            await TS.UpdateSettingCounter("AllUsers", "LiveUsersCount", false);
        }

        // POST api/values
        [HttpPost]
        [Authorize(Roles = "NotAutorizedUser")]
        public async Task<ActionResult> Post([FromBody] TSUser tsUser)
        {

            string UserID = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10);

            await TS.AddActivityLog(UserID, "post user", MethodBase.GetCurrentMethod());


            GlobalFunctions.CmdDecryptEntityAsymm(tsUser);


            bool b = TS.DeleteExpiredEmaiedCodes().Result;


            string MachineID = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "MachineID", 10);

            string IPAddress = HttpContext.Connection.RemoteIpAddress.ToString();



            TSEmailedCodeEntity emailedCode = TS.FindEmaiedCode(tsUser.Email, IPAddress, MachineID).Result;

            if (emailedCode != null)
            {
                if (emailedCode.Code.ToLower().Equals(tsUser.EmailedCode))
                {
                   
                    await TS.DeleteEmaiedCodes(tsUser.Email);

                    tsUser.UserID = await TS.GetNewID("AllUsers", "LastUserID", true);
                    tsUser.CreateDate = DateTime.Now;


                    b = await TS.AddUser(tsUser);

                    if (b)
                    {
                        await TS.UpdateSettingCounter("AllUsers", "UsersCount", true);


                        await GlobalFunctions.NotifyAdmin("New user");

                        return Ok("OK");
                    }
                    else
                    {
                        return Ok("Error:Can't add new user!");
                    }
                }
                else
                {
                    return Ok("Error:Emailed code is not correct!");
                }
            }
            else
            {

                await TS.AddErrorLog("AllUsers", "EmaiedCode expected but not found", MethodBase.GetCurrentMethod());
                return Ok("Error:Server can't find emailed code to compare!");
            }

        }



        [Route("GetLiveUsers")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TSUserOpen>>> GetLiveUsers()
        {
           
            string userID = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10);

            await TS.AddActivityLog(userID, "get live users", MethodBase.GetCurrentMethod());

            SymmKeyAndIV ClientSymmKeyAndIV = new SymmKeyAndIV();
            ClientSymmKeyAndIV.Key = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "ClientSymmKey", 5);
            ClientSymmKeyAndIV.IV = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "ClientSymmIV", 10);

            if (!ClientSymmKeyAndIV.Key.Equals("Error"))
            {

                List<TSUserOpen> a = await TS.GetLiveUsers();

                foreach (var item in a)
                {
                    GlobalFunctions.CmdEncryptEntitySymm(item, ClientSymmKeyAndIV);
                }

                return a;
               
            }
            else
            {
                return BadRequest("Can't get crypto key!");
            }
        }

        // PUT api/values/5
        [HttpPut]
        [Authorize(Roles = "AutorizedUser")]
        public async Task<ActionResult> Put([FromBody] TSUser tsUser)
        {

            string UserID = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10);
            await TS.AddActivityLog(UserID, "put user", MethodBase.GetCurrentMethod());

            SymmKeyAndIV ClientSymmKeyAndIV = new SymmKeyAndIV();
            ClientSymmKeyAndIV.Key = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "ClientSymmKey", 5);
            ClientSymmKeyAndIV.IV = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "ClientSymmIV", 10);

            if (!ClientSymmKeyAndIV.Key.Equals("Error"))
            {

                GlobalFunctions.CmdDecryptEntitySymm(tsUser, ClientSymmKeyAndIV);

                bool b = await TS.UpdateUser(tsUser, false);

                if (b)
                {

                    return Ok();
                }
                else
                {
                    return UnprocessableEntity();
                }
            }
            else
            {
                return BadRequest("Can't get crypto key!");
            }

            
        }

        // DELETE api/values/5
        [HttpDelete]
        [Authorize(Roles = "AutorizedUser")]
        public async Task<ActionResult> Delete([FromBody] TSUser tsUser)
        {
            string UserID = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10);
            await TS.AddActivityLog(UserID, "delete user", MethodBase.GetCurrentMethod());

            SymmKeyAndIV ClientSymmKeyAndIV = new SymmKeyAndIV();
            ClientSymmKeyAndIV.Key = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "ClientSymmKey", 5);
            ClientSymmKeyAndIV.IV = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "ClientSymmIV", 10);

            if (!ClientSymmKeyAndIV.Key.Equals("Error"))
            {

                GlobalFunctions.CmdDecryptEntitySymm(tsUser, ClientSymmKeyAndIV);

                bool b = await TS.DeleteUser(tsUser);

                if (b)
                {

                    return Ok();
                }
                else
                {
                    return UnprocessableEntity();
                }
            }
            else
            {
                return BadRequest("Can't get crypto key!");
            }

            
        }
    }
}
