using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Collections.Generic;
using TodosShared;
using System.Security.Claims;
using TodosFunctionsApi.JwtSecurity;
using static TodosShared.TSEnums;
using TodosCosmos;
using static TodosGlobal.GlobalClasses;
using TodosGlobal;
using TodosCosmos.DocumentClasses;
using System.IO;
using TodosCosmos.Diagnostics;
using static TodosCosmos.Enums;

namespace TodosFunctionsApi.API
{
    public class FunUser
    {


        private readonly List<WebApiUserTypesEnum> AllowedRoles = new List<WebApiUserTypesEnum>
            {
                WebApiUserTypesEnum.Authorized,
                WebApiUserTypesEnum.Admin
            };


        [FunctionName("FunUserGetAll")]
        public async Task<ActionResult<IEnumerable<TSUser>>> GetAll(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user/getall")] HttpRequest req)
        {

            List<WebApiUserTypesEnum>  localAllowedRoles = new List<WebApiUserTypesEnum>
            {
                WebApiUserTypesEnum.Admin
            };

            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, localAllowedRoles, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));


            Guid UserID =Guid.Parse(LocalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod())));
            await CosmosAPI.cosmosDBClientActivity.AddActivityLog(UserID, "Requested all users", TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            return  await CosmosAPI.cosmosDBClientUser.GetAllUsers(TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

        }

        [FunctionName("FunUserAuthorize")]
        public async Task<ActionResult<TSUser>> Authorize(
          [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "user/authorize")] HttpRequest req)          
        {


            TSUser tsUser = await MyFromBody<TSUser>.FromBody(req, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
           

            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, AllowedRoles, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));


            Guid UserID = Guid.Parse(LocalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod())));


            await CosmosAPI.cosmosDBClientActivity.AddActivityLog(UserID, "Requested authentication", TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            tsUser.ID = UserID;

            TSUser result = await CosmosAPI.cosmosDBClientUser.GetUser(tsUser, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
         

            if (!result.ID.Equals(Guid.Empty))
            {

                await CosmosAPI.cosmosDBClientFeedMessage.AddFeedMessage(RequestedActionEnum.NotifyAdmin, "New Login " + result.UserName, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
                await CosmosAPI.cosmosDBClientSetting.UpdateSettingCounter(Guid.Empty, "LiveUsersCount", true, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            }
            else
            {

                result.UserName = "Error!";
                result.FullName = "Can't find user!";

            }

         

            return result;
        }


        [FunctionName("FunUserSendMail")]
        public async Task<ActionResult<TSEmail>> SendMail(
         [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "user/sendmail")] HttpRequest req)         
        {

            TSEmail tsEmail = await MyFromBody<TSEmail>.FromBody(req, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            if (tsEmail is null)
            {
                await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, "tsEmail is null", TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
            }

            List<WebApiUserTypesEnum> localAllowedRoles = new List<WebApiUserTypesEnum>
            {
                WebApiUserTypesEnum.NotAuthorized,
                 WebApiUserTypesEnum.Authorized,
                  WebApiUserTypesEnum.Admin
            };

            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, localAllowedRoles, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

      
            Guid UserID = Guid.Parse(LocalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod())));
            await CosmosAPI.cosmosDBClientActivity.AddActivityLog(UserID, "send mail", TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));


            string MachineID = LocalFunctions.CmdGetValueFromClaim(User.Claims, "MachineID", 10, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));


 

            TSEmail result = await TodosCosmos.LocalFunctions.SendEmail(tsEmail,req.HttpContext.Connection.RemoteIpAddress.ToString(), MachineID, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
            result.To = string.Empty;
            result.OperationCode = 0;

            return result;

        }


        [FunctionName("FunUserPassRecovery")]
        public async Task<ActionResult<TSEmail>> PassRecovery(
  [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "user/passrecovery")] HttpRequest req)  
        {

            TSEmail tsEmail = await MyFromBody<TSEmail>.FromBody(req, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            List<WebApiUserTypesEnum> localAllowedRoles = new List<WebApiUserTypesEnum>
            {
                WebApiUserTypesEnum.NotAuthorized,
                 WebApiUserTypesEnum.Authorized,
                  WebApiUserTypesEnum.Admin
            };

            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, localAllowedRoles, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));


            Guid UserID = Guid.Parse(LocalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod())));
            await CosmosAPI.cosmosDBClientActivity.AddActivityLog(UserID, "Pass Recovery", TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

         

            string newPass = GlobalFunctions.GetSalt();

            CosmosDocUser cosmosDocUser = await CosmosAPI.cosmosDBClientUser.FindUserByUserName(tsEmail.To, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
            if (cosmosDocUser != null)
            {
                cosmosDocUser.Salt = GlobalFunctions.GetSalt();
                cosmosDocUser.HashedPassword = GlobalFunctions.CmdHashPasswordBytes(newPass, cosmosDocUser.Salt);

                bool b = await CosmosAPI.cosmosDBClientUser.UpdateUserDoc(cosmosDocUser, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

                if (b)
                {
                    tsEmail.To = cosmosDocUser.Email;
                    TSEmail result = await TodosCosmos.LocalFunctions.SendEmail(tsEmail, string.Empty, newPass, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
                    result.To = string.Empty;
                    result.OperationCode = 0;

                    return result;
                }
                else
                {
                    return new TSEmail { Result = "Error: Could not recover user password" };
                }
            }
            else
            {
                return new TSEmail { Result = "Error: User not found" };
            }
        }



        [FunctionName("FunUserCheckUserName")]
        public async Task<ActionResult<TSEmail>> CheckUserName(
  [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "user/checkusername")] HttpRequest req)  
        {

            TSEmail tsEmail = await MyFromBody<TSEmail>.FromBody(req, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            List<WebApiUserTypesEnum> localAllowedRoles = new List<WebApiUserTypesEnum>
            {
                WebApiUserTypesEnum.NotAuthorized,
                 WebApiUserTypesEnum.Authorized,
                  WebApiUserTypesEnum.Admin
            };

            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, localAllowedRoles, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));


            Guid UserID = Guid.Parse(LocalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod())));
            await CosmosAPI.cosmosDBClientActivity.AddActivityLog(UserID, "Check UserName", TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));


           


            CosmosDocUser user = await CosmosAPI.cosmosDBClientUser.FindUserByUserName(tsEmail.To, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

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

            return result;

        }



        [FunctionName("FunUserCheckEmail")]
        public async Task<ActionResult<TSEmail>> CheckEmail(
[HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "user/checkemail")] HttpRequest req)
        {
            TSEmail result = new TSEmail
            {
                To = string.Empty,
                OperationCode = 0
            };

            TSEmail tsEmail = await MyFromBody<TSEmail>.FromBody(req, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));


            if (tsEmail is null)
            {
                await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, "tsEmail is null", TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

                result.Result = "Error";
                return result;
            }
            else
            {

                if (string.IsNullOrEmpty(tsEmail.To))
                {

                    await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, "tsEmail to is empty!", TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
                    result.Result = "Error";
                    return result;
                }
            }

            List<WebApiUserTypesEnum> localAllowedRoles = new List<WebApiUserTypesEnum>
            {
                 WebApiUserTypesEnum.NotAuthorized,
                 WebApiUserTypesEnum.Authorized,
                 WebApiUserTypesEnum.Admin
            };

            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, localAllowedRoles, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));


            Guid UserID = Guid.Parse(LocalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod())));
            await CosmosAPI.cosmosDBClientActivity.AddActivityLog(UserID, "Check Email", TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));


           


            CosmosDocUser user = await CosmosAPI.cosmosDBClientUser.FindUserByEmail(tsEmail.To, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

           

            if (user is null)
            {
                result.Result = "OK";
            }
            else
            {
                result.Result = "Email already exists";
            }

            return result;

        }



        [FunctionName("FunUserLogout")]
        public async Task<ActionResult> Logout(
[HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user/logout")] HttpRequest req)
        {


            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, AllowedRoles, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));


            Guid UserID = Guid.Parse(LocalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod())));


            await CosmosAPI.cosmosDBClientActivity.AddActivityLog(UserID, "user logout", TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            await CosmosAPI.cosmosDBClientSetting.UpdateSettingCounter(Guid.Empty, "LiveUsersCount", false, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            return new OkObjectResult("OK");
        }


        [FunctionName("FunUserAdd")]
        public async Task<ActionResult> Add(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "user/add")] HttpRequest req)
        {

            TSUser tsUser = await MyFromBody<TSUser>.FromBody(req, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            List<WebApiUserTypesEnum> localAllowedRoles = new List<WebApiUserTypesEnum>
            {
                 WebApiUserTypesEnum.NotAuthorized,
                 WebApiUserTypesEnum.Authorized,
                  WebApiUserTypesEnum.Admin
            };


            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, localAllowedRoles, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            Guid UserID = Guid.Parse(LocalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod())));
            await CosmosAPI.cosmosDBClientActivity.AddActivityLog(UserID, "post user", TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));


            string MachineID = LocalFunctions.CmdGetValueFromClaim(User.Claims, "MachineID", 10, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            string IPAddress = req.HttpContext.Connection.RemoteIpAddress.ToString();



            CosmosEmailedCode emailedCode = await CosmosAPI.cosmosDBClientEmailedCode.FindEmaiedCode(tsUser.Email, IPAddress, MachineID, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            if (emailedCode != null)
            {
                if (emailedCode.Code.ToLower().Equals(tsUser.EmailedCode))
                {

                    await CosmosAPI.cosmosDBClientEmailedCode.DeleteEmailedCodes(tsUser.Email, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

                    tsUser.ID = Guid.NewGuid();
                    tsUser.CreateDate = DateTime.UtcNow;


                    if (await CosmosAPI.cosmosDBClientUser.AddUser(tsUser, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod())))
                    {
                        await CosmosAPI.cosmosDBClientFeedMessage.AddFeedMessage(RequestedActionEnum.NotifyAdmin, "New User registration " + tsUser.UserName, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

                        await CosmosAPI.cosmosDBClientSetting.UpdateSettingCounter(Guid.Empty, "UsersCount", true, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

                        return new OkObjectResult("OK");
                    }
                    else
                    {
                        return new OkObjectResult("Error:Can't add new user!");
                    }
                }
                else
                {
                    return new OkObjectResult("Error:Emailed code is not correct!");
                }
            }
            else
            {

                await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, "EmaiedCode expected but not found", TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
                return new OkObjectResult("Error:Server can't find emailed code to compare!");
            }

        }

        [FunctionName("ChangePassword")]
        public async Task<ActionResult> ChangePassword(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "user/changepassword")] HttpRequest req)        
        {

            TSUser tsUser = await MyFromBody<TSUser>.FromBody(req, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, AllowedRoles, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            Guid UserID = Guid.Parse(LocalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod())));
            await CosmosAPI.cosmosDBClientActivity.AddActivityLog(UserID, "change password", TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));


            string MachineID = LocalFunctions.CmdGetValueFromClaim(User.Claims, "MachineID", 10, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            string IPAddress = req.HttpContext.Connection.RemoteIpAddress.ToString();



            CosmosEmailedCode emailedCode = await CosmosAPI.cosmosDBClientEmailedCode.FindEmaiedCode(tsUser.Email, IPAddress, MachineID, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            if (emailedCode != null)
            {
                if (emailedCode.Code.ToLower().Equals(tsUser.EmailedCode))
                {

                    await CosmosAPI.cosmosDBClientEmailedCode.DeleteEmailedCodes(tsUser.Email, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));


                    TSUser currUser = (await CosmosAPI.cosmosDBClientUser.FindUserByID(UserID, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()))).toTSUser();

                    currUser.Password = tsUser.Password;


                    if (await CosmosAPI.cosmosDBClientUser.UpdateUser(currUser, false, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod())))
                    {

                        return new OkObjectResult("OK");
                    }
                    else
                    {
                        return new OkObjectResult("Error:Can't add new user!");
                    }
                }
                else
                {
                    return new OkObjectResult("Error:Emailed code is not correct!");
                }
            }
            else
            {

                await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, "EmaiedCode expected but not found", TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
                return new OkObjectResult("Error:Server can't find emailed code to compare!");
            }

        }


        [FunctionName("FunUserGetLiveUsers")]
        public async Task<ActionResult<IEnumerable<TSUserOpen>>> GetLiveUsers(
          [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user/getliveusers")] HttpRequest req)          
        {

            List<WebApiUserTypesEnum> localAllowedRoles = new List<WebApiUserTypesEnum>
            {
                WebApiUserTypesEnum.NotAuthorized,
                 WebApiUserTypesEnum.Authorized,
                  WebApiUserTypesEnum.Admin
            };

            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, localAllowedRoles, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));


            Guid UserID = Guid.Parse(LocalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod())));

            await CosmosAPI.cosmosDBClientActivity.AddActivityLog(UserID, "get live users", TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

          
                return await CosmosAPI.cosmosDBClientUser.GetLiveUsers(TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            
        }


        [FunctionName("FunUserUpdate")]
        public async Task<ActionResult> Update(
     [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "User/update")] HttpRequest req)     
        {

            TSUser tsUser = await MyFromBody<TSUser>.FromBody(req, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));


            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, AllowedRoles, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));


            Guid UserID = Guid.Parse(LocalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod())));
            await CosmosAPI.cosmosDBClientActivity.AddActivityLog(UserID, "put user", TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

           

                bool b = await CosmosAPI.cosmosDBClientUser.UpdateUser(tsUser, false, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

                if (b)
                {

                    return new OkResult();
                }
                else
                {
                    return new UnprocessableEntityResult();
                }
           


        }

        [FunctionName("FunUserUpdatefont")]
        public async Task<ActionResult> UpdateFont(
    [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "User/updatefont")] HttpRequest req)
        {

            TSVisitor tsVisitor = await MyFromBody<TSVisitor>.FromBody(req, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));


            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, AllowedRoles, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));


            Guid UserID = Guid.Parse(LocalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod())));
            await CosmosAPI.cosmosDBClientActivity.AddActivityLog(UserID, "update user font", TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            CosmosDocUser doc = await CosmosAPI.cosmosDBClientUser.GetUserDoc(UserID, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            if (!string.IsNullOrEmpty(tsVisitor.DefaultFont))
            {
                doc.DefaultFont = tsVisitor.DefaultFont;
            }
            else
            {
                doc.DefaultFont = string.Empty;
            }

            bool b = await CosmosAPI.cosmosDBClientUser.UpdateUserDoc(doc, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            if (b)
            {

                return new OkObjectResult("OK");
            }
            else
            {
                return new UnprocessableEntityResult();
            }



        }


        [FunctionName("FunUserDelete")]
        public async Task<ActionResult> Delete(
    [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "User/delete")] HttpRequest req)
        {

            TSUser tsUser = await MyFromBody<TSUser>.FromBody(req, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));


            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, AllowedRoles, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));


            Guid UserID = Guid.Parse(LocalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod())));
            await CosmosAPI.cosmosDBClientActivity.AddActivityLog(UserID, "delete user", TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

          


                bool b = await CosmosAPI.cosmosDBClientUser.DeleteUser(tsUser, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

                if (b)
                {

                    return new OkResult();
                }
                else
                {
                    return new UnprocessableEntityResult();
                }
           


        }
    }
}
