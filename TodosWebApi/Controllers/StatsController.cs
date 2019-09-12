using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TodosShared;
using TodosWebApi.DataLayer;
using TodosWebApi.DataLayer.TSEntities;
using TodosWebApi.GlobalDataLayer;

namespace TodosWebApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,AutorizedUser,NotAutorizedUser")]
    [ApiController]
    public class StatsController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly TableStorage TS;

        public StatsController(ILogger<TodosController> logger)
        {
            _logger = logger;
            TS = new TableStorage();
        }


        // GET: api/Stats
        [HttpGet]
        public async Task<TSStat> Get()
        {

            string userID = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "UserID", 10);
            string userName = GlobalFunctions.CmdGetValueFromClaim(User.Claims, "UserName", 10);

            if (!string.IsNullOrEmpty(userID))
            {
                if (userID!="-745" && !userName.Equals("demouser"))
                {
                    TSUser tsUser = new TSUser()
                    {
                        UserID = userID,
                        UserName = userName,
                    };

                    tsUser = TS.GetUser(tsUser).Result;

                    if (tsUser != null)
                    {
                        tsUser.IsLive = true;
                        bool b = await TS.UpdateUser(tsUser, true);
                    }
                }
            }


            return new TSStat
            {
                IPsCount = GetStat("IPAddressesCount"),
                VisitsCount = GetStat("VisitsCount"),
                UsersCount = GetStat("UsersCount"),
                LiveUsersCount = GetStat("LiveUsersCount"),
                TodosCount = GetStat("TodosCount"),
                LikesCount = GetStat("LikesCount"),
                DislikesCount = GetStat("DislikesCount"),
                FeedbackCount = GetStat("FeedbackCount"),
            };

        }


        private int GetStat(string name)
        {

            string a = TS.GetSetting("AllUsers", name).Result.Value;

            if (string.IsNullOrEmpty(a))
            {
                return 0;
            }
            else
            {
                return int.Parse(a);
            }

        }

      

    }
}
