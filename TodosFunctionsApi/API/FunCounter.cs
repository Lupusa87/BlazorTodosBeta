﻿using Google.Apis.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TodosCosmos;
using TodosCosmos.DocumentClasses;
using TodosFunctionsApi.JwtSecurity;
using TodosShared;
using static TodosShared.TSEnums;

namespace TodosFunctionsApi.API
{

    public class FunCounter
    {

        [FunctionName("FunCounterGetAll")]
        public async Task<ActionResult<IEnumerable<TSCounter>>> GetAll(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Counter/getall")] HttpRequest req)
        {

            return await CosmosAPI.cosmosDBClientCounter.GetNewestCounters(10,TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

        }


        [FunctionName("FunCounterGetReport1")]
        public async Task<ActionResult<IEnumerable<TSReport1>>> GetReport1(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "Counter/getreport1")] HttpRequest req)
        {

            TSReport1 tsReport = await MyFromBody<TSReport1>.FromBody(req, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));


            if (string.IsNullOrEmpty(tsReport.Source))
            {
                tsReport.Source = "0";
            }

            if (string.IsNullOrEmpty(tsReport.Action))
            {
                tsReport.Action = "0";
            }

            return await CosmosAPI.cosmosDBClientCounter.GetReport1(long.Parse(tsReport.Source),
                long.Parse(tsReport.Action),
                TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

        }


        [FunctionName("FunCounterAdd")]
        public async Task<ActionResult> Add(
           [HttpTrigger(AuthorizationLevel.Function, "post", Route = "Counter/add")] HttpRequest req)
        {

            string ip = req.HttpContext.Connection.RemoteIpAddress.ToString().Trim();
            if (ip.Equals("127.0.0.1"))
            {
                return new OkObjectResult("OK");
            }

            TSCounter tsCounter = await MyFromBody<TSCounter>.FromBody(req, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            tsCounter.ID = Guid.NewGuid();
            tsCounter.Date = DateTime.UtcNow;


            CosmosDocCounter docCounter = new CosmosDocCounter(tsCounter)
            {
                IPAddress = ip
            };


            bool b = await CosmosAPI.cosmosDBClientCounter.AddCounter(docCounter, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            if (b)
            {
                return new OkObjectResult("OK");
            }
            else
            {
                return new OkObjectResult("Error:Can't add new counter!");
            }

        }

    }
}
