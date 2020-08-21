using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using TodosCosmos;
using TodosCosmos.DocumentClasses;
using TodosGlobal;
using static TodosCosmos.Enums;

namespace TodosFunctionsApi.Timers
{
    //CRON expression
    //https://www.freeformatter.com/cron-expression-generator-quartz.html

    public static class FunTimer
    {
        [FunctionName("FunTimer1")]
        public static async Task Timer1([TimerTrigger("*/30 * * * * *")]TimerInfo myTimer, ILogger log)
        {


            //await CosmosAPI.cosmosDBClientError.DeleteAllErrorLogs(DocDeleteModeEnum.Hard, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
           
            //await CosmosAPI.cosmosDBClientCounter.DeleteAllCounters(DocDeleteModeEnum.Hard, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
     

            CosmosDocSetting setting = await CosmosAPI.cosmosDBClientSetting.GetSetting(Guid.Empty, "LastStatRequest", TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            if (setting != null)
            {

                if (GlobalFunctions.ToUnixEpochDate(DateTime.Now.AddMinutes(-1)) < setting.TimeStamp)
                {
                    await CosmosAPI.cosmosDBClientUser.UpdateOfflineUsers(TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
                    await CosmosAPI.cosmosDBClientUser.UpdateOnlineUsersCount(TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
                }

            }


        }

        [FunctionName("FunTimer2")]
        public static async void Timer2([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {
           
            //bool b;
            //CosmosDocSetting setting = await CosmosAPI.cosmosDBClientSetting.GetSetting(Guid.Empty, "DoActivityLog", TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            //if (setting != null)
            //{
            //    if (string.IsNullOrEmpty(setting.Value))
            //    {
            //        if (CosmosAPI.DoActivityLog)
            //        {
            //            CosmosAPI.DoActivityLog = false;
            //        }
            //    }
            //    else
            //    {
            //        b = bool.Parse(setting.Value);
            //        if (CosmosAPI.DoActivityLog != b)
            //        {
            //            CosmosAPI.DoActivityLog = b;
            //        }
            //    }
            //}

            await CosmosAPI.cosmosDBClientReminder.SendTodoReminders(TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
        }
    }
}
