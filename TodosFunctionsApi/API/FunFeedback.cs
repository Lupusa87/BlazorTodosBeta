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

            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, AllowedRoles);

      
            Guid userID =Guid.Parse(LocalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10));

            await CosmosAPI.cosmosDBClientActivity.AddActivityLog(userID, "Requested Feedbacks", MethodBase.GetCurrentMethod());



                List<TSFeedback> list = await CosmosAPI.cosmosDBClientFeedback.GetAllFeedback();

                foreach (var item in list)
                {
                    item.UserName = CosmosAPI.cosmosDBClientUser.FindUserByID(item.UserID).Result.FullName;
                  
                }

                return list;
          

        }


        [FunctionName("FunFeedbackGet")]
        public async Task<ActionResult<TSFeedback>> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "feedback/get")] HttpRequest req,
            ILogger log)
        {

            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, AllowedRoles);


            Guid userID = Guid.Parse(LocalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10));

            await CosmosAPI.cosmosDBClientActivity.AddActivityLog(userID, "Requested Feedback", MethodBase.GetCurrentMethod());



                CosmosDocFeedback feedbackDoc = await CosmosAPI.cosmosDBClientFeedback.FindFeedback(userID);


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

            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, localAllowedRoles);

            TSFeedback tsFeedback = await MyFromBody<TSFeedback>.FromBody(req);

          

            tsFeedback.UserID = Guid.Parse(LocalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10));
            await CosmosAPI.cosmosDBClientActivity.AddActivityLog(tsFeedback.UserID, "Add or update Feedback", MethodBase.GetCurrentMethod());

            string userName = LocalFunctions.CmdGetValueFromClaim(User.Claims, "UserName", 10);

            if (!tsFeedback.UserID.Equals(Guid.Empty))
            {
                if (userName.Equals("demouser"))
                {
                    return new TSFeedback();
                }
            }

            CosmosDocFeedback oldFeedback = await CosmosAPI.cosmosDBClientFeedback.FindFeedback(tsFeedback.UserID);

            if (oldFeedback is null)
            {

                tsFeedback.ID = Guid.NewGuid();
                tsFeedback.AddDate = DateTime.UtcNow;
                bool b = await CosmosAPI.cosmosDBClientFeedback.AddFeedback(tsFeedback);

                if (b)
                {

                    await TodosCosmos.LocalFunctions.NotifyAdmin("New Feedback " + userName + " " + tsFeedback.Text);
                    await CosmosAPI.cosmosDBClientSetting.UpdateSettingCounter(Guid.Empty, "FeedbackCount", true);

                }

            }
            else
            {

                if (oldFeedback.Text != tsFeedback.Text)
                {

                    bool b = await CosmosAPI.cosmosDBClientFeedback.UpdateFeedback(tsFeedback);

                    if (b)
                    {

                        await TodosCosmos.LocalFunctions.NotifyAdmin("Feedback update to " + tsFeedback.Text + " " + userName);
                    }
                }
            }


                CosmosDocFeedback feedbackDoc = await CosmosAPI.cosmosDBClientFeedback.FindFeedback(tsFeedback.UserID);

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
