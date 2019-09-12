using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TodosShared;
using TodosWebApi.DataLayer;
using TodosWebApi.DataLayer.TSEntities;
using TodosWebApi.GlobalDataLayer;
using static TodosWebApi.GlobalDataLayer.GlobalClasses;

namespace TodosWebApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,AutorizedUser")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {

        private readonly TableStorage TS;

        public FeedbackController()
        {
            TS = new TableStorage();
        }

        [Route("GetAllFeedback")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TSFeedback>>> GetAllFeedback()
        {
            string userID = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10);

            await TS.AddActivityLog(userID, "Requested Feedbacks", MethodBase.GetCurrentMethod());


            SymmKeyAndIV ClientSymmKeyAndIV = new SymmKeyAndIV
            {
                Key = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "ClientSymmKey", 5),
                IV = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "ClientSymmIV", 10)
            };


            if (!string.IsNullOrEmpty(ClientSymmKeyAndIV.Key))
            {

                List<TSFeedback> list = await TS.GetAllFeedback();

                foreach (var item in list)
                {
                    item.UserName = TS.FindUser(item.UserID, true, string.Empty).Result.FullName;
                    GlobalFunctions.CmdEncryptEntitySymm(item, ClientSymmKeyAndIV);
                }

                return list;
            }
            else
            {
                return new List<TSFeedback>();
            }

        }


        [Route("GetFeedback")]
        [HttpGet]
        public async Task<ActionResult<TSFeedback>> GetFeedback()
        {
            string userID = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10);
            await TS.AddActivityLog(userID, "Requested Feedback", MethodBase.GetCurrentMethod());

            SymmKeyAndIV ClientSymmKeyAndIV = new SymmKeyAndIV
            {
                Key = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "ClientSymmKey", 5),
                IV = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "ClientSymmIV", 10)
            };


            if (!string.IsNullOrEmpty(ClientSymmKeyAndIV.Key))
            {

                TSFeedbackEntity feedbackEntity = await TS.FindFeedback(userID, true, string.Empty);


                if (feedbackEntity != null)
                {

                    TSFeedback result = feedbackEntity.toTSFeedback();

                    GlobalFunctions.CmdEncryptEntitySymm(result, ClientSymmKeyAndIV);

                    return result;
                }
                else
                {
                    return new TSFeedback();
                }
            }
            else
            {
                return new TSFeedback();
            }
        }

        [Route("AddFeedback")]
        [HttpPost]
        public async Task<ActionResult<TSFeedback>> AddFeedback([FromBody] TSFeedback TSFeedback)
        {
            GlobalFunctions.CmdDecryptEntityAsymm(TSFeedback);

            TSFeedback.UserID = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10);
            await TS.AddActivityLog(TSFeedback.UserID, "Add or update Feedback", MethodBase.GetCurrentMethod());

            string userName = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "UserName", 10);

            if (!string.IsNullOrEmpty(TSFeedback.UserID))
            {
                if (userName.Equals("demouser"))
                {
                    return new TSFeedback();
                }
            }


            

            TSFeedbackEntity oldFeedback = await TS.FindFeedback(TSFeedback.UserID, true, string.Empty);

            if (oldFeedback is null)
            {
                string a = await TS.GetNewID("AllUsers", "LastFeedbackID", false);
                TSFeedback.FeedbackID = int.Parse(a);


                bool b = await TS.AddFeedback(TSFeedback);

                if (b)
                {

                    await GlobalFunctions.NotifyAdmin("New Feedback " + userName + " " + TSFeedback.Text);
                    await TS.UpdateSettingCounter("AllUsers", "FeedbackCount", true);
                    
                }

            }
            else
            {

                if (oldFeedback.Text != TSFeedback.Text)
                {

                    bool b = await TS.UpdateFeedback(TSFeedback);

                    if (b)
                    {

                        await GlobalFunctions.NotifyAdmin("Feedback update to " + TSFeedback.Text + " " + userName);
                    }
                }
            }



            SymmKeyAndIV ClientSymmKeyAndIV = new SymmKeyAndIV
            {
                Key = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "ClientSymmKey", 5),
                IV = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "ClientSymmIV", 10)
            };


            if (!string.IsNullOrEmpty(ClientSymmKeyAndIV.Key))
            {
                TSFeedbackEntity feedbackEntity = await TS.FindFeedback(TSFeedback.UserID, true, string.Empty);
               
                if (feedbackEntity != null)
                {
                    TSFeedback result = feedbackEntity.toTSFeedback();
                    GlobalFunctions.CmdEncryptEntitySymm(result, ClientSymmKeyAndIV);
                    return result;
                }
                else
                {
                    return new TSFeedback();
                }
            }
            else
            {
                return new TSFeedback();

            }



        }


        [Route("GetReaction")]
        [HttpGet]
        public async Task<ActionResult<TSReaction>> GetReaction()
        {
            string userID = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10);
            await TS.AddActivityLog(userID, "Requested Reaction", MethodBase.GetCurrentMethod());

            SymmKeyAndIV ClientSymmKeyAndIV = new SymmKeyAndIV
            {
                Key = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "ClientSymmKey", 5),
                IV = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "ClientSymmIV", 10)
            };

            if (!string.IsNullOrEmpty(ClientSymmKeyAndIV.Key))
            {
                TSReactionEntity reactionEntity = await TS.FindReaction(userID, true, string.Empty);

                if (reactionEntity != null)
                {
                    TSReaction result = reactionEntity.toTSReaction();
                    GlobalFunctions.CmdEncryptEntitySymm(result, ClientSymmKeyAndIV);

                    return result;
                }
                else
                {
                    return new TSReaction();
                }
            }
            else
            {
                return new TSReaction();
            }
        }

        [Route("AddReaction")]
        [HttpPost]
        public async Task<ActionResult<TSReaction>> AddReaction([FromBody] TSReaction TSReaction)
        {
            TSReaction.UserID = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10);
            await TS.AddActivityLog(TSReaction.UserID, "Add or update Reaction", MethodBase.GetCurrentMethod());

            string userName = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "UserName", 10);

            if (!string.IsNullOrEmpty(TSReaction.UserID))
            {
                if (userName.Equals("demouser"))
                {
                    return new TSReaction();
                }
            }

            TSReactionEntity oldReaction = await TS.FindReaction(TSReaction.UserID, true, string.Empty);
            if (oldReaction is null)
            {
                string a = await TS.GetNewID("AllUsers", "LastReactionID", false);
                TSReaction.ReactionID = int.Parse(a);

                bool b = await TS.AddReaction(TSReaction);

                if (b)
                {
                
                    await GlobalFunctions.NotifyAdmin("New Reaction " + userName + " " + TSReaction.LikeOrDislike);

                    if (TSReaction.LikeOrDislike)
                    {
                        await TS.UpdateSettingCounter("AllUsers", "LikesCount", true);
                    }
                    else
                    {
                        await TS.UpdateSettingCounter("AllUsers", "DislikesCount", true);
                    }
                }

            }
            else
            {

                if (oldReaction.LikeOrDislike != TSReaction.LikeOrDislike)
                {
                    if (oldReaction.LikeOrDislike)
                    {
                        await TS.UpdateSettingCounter("AllUsers", "LikesCount", false);
                    }
                    else
                    {
                        await TS.UpdateSettingCounter("AllUsers", "DislikesCount", false);
                    }

                   

                    bool b = await TS.UpdateReaction(TSReaction);

                    if (b)
                    {

                        await GlobalFunctions.NotifyAdmin("Reaction update to " + TSReaction.LikeOrDislike + " " +userName);

                        if (TSReaction.LikeOrDislike)
                        {
                            await TS.UpdateSettingCounter("AllUsers", "LikesCount", true);
                        }
                        else
                        {
                            await TS.UpdateSettingCounter("AllUsers", "DislikesCount", true);
                        }

                 
                    }
                }
            }


            SymmKeyAndIV ClientSymmKeyAndIV = new SymmKeyAndIV
            {
                Key = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "ClientSymmKey", 5),
                IV = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "ClientSymmIV", 10)
            };


            if (!string.IsNullOrEmpty(ClientSymmKeyAndIV.Key))
            {

                TSReactionEntity reactionEntity = await TS.FindReaction(TSReaction.UserID, true, string.Empty);

                if (reactionEntity != null)
                {
                    TSReaction result = reactionEntity.toTSReaction();
                    GlobalFunctions.CmdEncryptEntitySymm(result, ClientSymmKeyAndIV);
                    return result;
                }
                else
                {
                    return new TSReaction();
                }
            }
            else
            {
                return new TSReaction();

            }

        }

    }
}
