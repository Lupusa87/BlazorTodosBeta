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
    public class FunReaction
    {
      

        private List<WebApiUserTypesEnum> AllowedRoles = new List<WebApiUserTypesEnum>
            {
                WebApiUserTypesEnum.NotAuthorized,
                WebApiUserTypesEnum.Authorized,
                WebApiUserTypesEnum.Admin
            };

          
        [FunctionName("FunReactionGet")]
        public async Task<ActionResult<TSReaction>> Get(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "reaction/get")] HttpRequest req,
    ILogger log)
        {

            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, AllowedRoles);


            Guid userID = Guid.Parse(LocalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10));

       
            await CosmosAPI.cosmosDBClientActivity.AddActivityLog(userID, "Requested Reaction", MethodBase.GetCurrentMethod());


                CosmosDocReaction reactionDoc = await CosmosAPI.cosmosDBClientReaction.FindReaction(userID);


                if (reactionDoc != null)
                {
                 
                    return reactionDoc.toTSReaction();
                }
                else
                {
                   
                    return new TSReaction();
                }
           
        }



        [FunctionName("FunReactionAdd")]
        public async Task<ActionResult<TSReaction>> Add(
          [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "reaction/add")] HttpRequest req,
          ILogger log)
        {

            List<WebApiUserTypesEnum> localAllowedRoles = new List<WebApiUserTypesEnum>
            {
                WebApiUserTypesEnum.Authorized,
                WebApiUserTypesEnum.Admin
            };

            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, localAllowedRoles);

            Guid userID = Guid.Parse(LocalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10));

            TSReaction tsReaction = await MyFromBody<TSReaction>.FromBody(req);
            tsReaction.UserID = userID;


            await CosmosAPI.cosmosDBClientActivity.AddActivityLog(userID, "Add or update Reaction", MethodBase.GetCurrentMethod());

            string userName = LocalFunctions.CmdGetValueFromClaim(User.Claims, "UserName", 10);

            if (!tsReaction.UserID.Equals(Guid.Empty))
            {
                if (userName.Equals("demouser"))
                {
                    return new TSReaction();
                }
            }

            CosmosDocReaction oldReaction = await CosmosAPI.cosmosDBClientReaction.FindReaction(tsReaction.UserID);
            if (oldReaction is null)
            {

                tsReaction.ID = Guid.NewGuid();
                bool b = await CosmosAPI.cosmosDBClientReaction.AddReaction(tsReaction);

                if (b)
                {

                    await TodosCosmos.LocalFunctions.NotifyAdmin("New Reaction " + userName + " " + tsReaction.LikeOrDislike);

                    if (tsReaction.LikeOrDislike)
                    {
                        await CosmosAPI.cosmosDBClientSetting.UpdateSettingCounter(Guid.Empty, "LikesCount", true);
                    }
                    else
                    {
                        await CosmosAPI.cosmosDBClientSetting.UpdateSettingCounter(Guid.Empty, "DislikesCount", true);
                    }
                }

            }
            else
            {
                tsReaction.ID = oldReaction.ID;
                if (oldReaction.LikeOrDislike != tsReaction.LikeOrDislike)
                {
                    if (oldReaction.LikeOrDislike)
                    {
                        await CosmosAPI.cosmosDBClientSetting.UpdateSettingCounter(Guid.Empty, "LikesCount", false);
                    }
                    else
                    {
                        await CosmosAPI.cosmosDBClientSetting.UpdateSettingCounter(Guid.Empty, "DislikesCount", false);
                    }



                    bool b = await CosmosAPI.cosmosDBClientReaction.UpdateReaction(tsReaction);

                    if (b)
                    {

                        await TodosCosmos.LocalFunctions.NotifyAdmin("Reaction update to " + tsReaction.LikeOrDislike + " " + userName);

                        if (tsReaction.LikeOrDislike)
                        {
                            await CosmosAPI.cosmosDBClientSetting.UpdateSettingCounter(Guid.Empty, "LikesCount", true);
                        }
                        else
                        {
                            await CosmosAPI.cosmosDBClientSetting.UpdateSettingCounter(Guid.Empty, "DislikesCount", true);
                        }


                    }
                }
            }



                CosmosDocReaction reactionEntity = await CosmosAPI.cosmosDBClientReaction.FindReaction(tsReaction.UserID);

                if (reactionEntity != null)
                {
                  
                    return reactionEntity.toTSReaction();
                }
                else
                {
                    return new TSReaction();
                }
           
        }
    }
}
