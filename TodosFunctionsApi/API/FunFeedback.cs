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

    public class FunFeedback
    {


        private List<WebApiUserTypesEnum> AllowedRoles = new List<WebApiUserTypesEnum>
            {
                WebApiUserTypesEnum.NotAuthorized,
                WebApiUserTypesEnum.Authorized,
                WebApiUserTypesEnum.Admin
            };

         


        [FunctionName("FunFeedbackGetAll")]
        public async Task<ActionResult<IEnumerable<TSFeedback>>> GetAll(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "feedback/getall")] HttpRequest req,
            ILogger log)
        {

            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, AllowedRoles, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

      
            Guid userID =Guid.Parse(LocalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod())));

            await CosmosAPI.cosmosDBClientActivity.AddActivityLog(userID, "Requested Feedbacks", TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));



                List<TSFeedback> list = await CosmosAPI.cosmosDBClientFeedback.GetAllFeedback(TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

                foreach (var item in list)
                {
                    item.UserName = CosmosAPI.cosmosDBClientUser.FindUserByID(item.UserID, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod())).Result.FullName;
                  
                }

                return list;
          

        }


        [FunctionName("FunFeedbackGet")]
        public async Task<ActionResult<TSFeedback>> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "feedback/get")] HttpRequest req,
            ILogger log)
        {

            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, AllowedRoles, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));


            Guid userID = Guid.Parse(LocalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod())));

            await CosmosAPI.cosmosDBClientActivity.AddActivityLog(userID, "Requested Feedback", TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));



                CosmosDocFeedback feedbackDoc = await CosmosAPI.cosmosDBClientFeedback.FindFeedback(userID, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));


                if (feedbackDoc != null)
                {

                    return feedbackDoc.toTSFeedback();
                }
                else
                {
                    return new TSFeedback();
                }
           
        }

        [FunctionName("FunFeedbackAdd")]
        public async Task<ActionResult<TSFeedback>> Add(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "feedback/add")] HttpRequest req,
           ILogger log)
        {

            List<WebApiUserTypesEnum> localAllowedRoles = new List<WebApiUserTypesEnum>
            {
                WebApiUserTypesEnum.Authorized,
                WebApiUserTypesEnum.Admin
            };

            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, localAllowedRoles, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            TSFeedback tsFeedback = await MyFromBody<TSFeedback>.FromBody(req, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

          

            tsFeedback.UserID = Guid.Parse(LocalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod())));
            await CosmosAPI.cosmosDBClientActivity.AddActivityLog(tsFeedback.UserID, "Add or update Feedback", TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            string userName = LocalFunctions.CmdGetValueFromClaim(User.Claims, "UserName", 10, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            if (!tsFeedback.UserID.Equals(Guid.Empty))
            {
                if (userName.Equals("demouser"))
                {
                    return new TSFeedback();
                }
            }

            CosmosDocFeedback oldFeedback = await CosmosAPI.cosmosDBClientFeedback.FindFeedback(tsFeedback.UserID, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            if (oldFeedback is null)
            {

                tsFeedback.ID = Guid.NewGuid();
                tsFeedback.AddDate = DateTime.UtcNow;
                bool b = await CosmosAPI.cosmosDBClientFeedback.AddFeedback(tsFeedback, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

                if (b)
                {

                    await TodosCosmos.LocalFunctions.NotifyAdmin("New Feedback " + userName + " " + tsFeedback.Text, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
                    await CosmosAPI.cosmosDBClientSetting.UpdateSettingCounter(Guid.Empty, "FeedbackCount", true, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

                }

            }
            else
            {

                if (oldFeedback.Text != tsFeedback.Text)
                {

                    bool b = await CosmosAPI.cosmosDBClientFeedback.UpdateFeedback(tsFeedback, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

                    if (b)
                    {

                        await TodosCosmos.LocalFunctions.NotifyAdmin("Feedback update to " + tsFeedback.Text + " " + userName, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
                    }
                }
            }


                CosmosDocFeedback feedbackDoc = await CosmosAPI.cosmosDBClientFeedback.FindFeedback(tsFeedback.UserID, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

                if (feedbackDoc != null)
                {

                    return feedbackDoc.toTSFeedback();
                }
                else
                {
                    return new TSFeedback();
                }
           
        }

    }
}
