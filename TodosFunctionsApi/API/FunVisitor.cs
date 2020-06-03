using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos;
using System.Reflection;
using System.Collections.Generic;
using TodosShared;
using System.Security.Claims;
using TodosFunctionsApi.JwtSecurity;
using static TodosShared.TSEnums;
using static TodosGlobal.GlobalClasses;
using TodosCosmos;
using TodosGlobal;
using TodosCosmos.DocumentClasses;

namespace TodosFunctionsApi.API
{
    public class FunVisitor
    {


        private List<WebApiUserTypesEnum> AllowedRoles = new List<WebApiUserTypesEnum>
        {
            WebApiUserTypesEnum.NotAuthorized,
            WebApiUserTypesEnum.Authorized, 
            WebApiUserTypesEnum.Admin
        };


        [FunctionName("FunVisitorGet")]
        public async Task<ActionResult<TSVisitor>> Get(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Visitor/get")] HttpRequest req,
           ILogger log)
        {

            TSVisitor tsVisitor = new TSVisitor {  IPAddress = req.HttpContext.Connection.RemoteIpAddress.ToString() };





            CosmosDocVisitorsStat doc = await CosmosAPI.cosmosDBClientVisitor.GetVisitor(tsVisitor.IPAddress, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));


            if (doc is null)
            {
                await CosmosAPI.cosmosDBClientVisitor.AddVisitor(req.HttpContext.Connection.RemoteIpAddress.ToString(), TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));


                doc = await CosmosAPI.cosmosDBClientVisitor.GetVisitor(tsVisitor.IPAddress, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            }
            else
            {

                await CosmosAPI.cosmosDBClientSetting.UpdateSettingCounter(Guid.Empty, "VisitsCount", true, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
                doc.VisitsCount += 1;


                await CosmosAPI.cosmosDBClientVisitor.cosmosDBClientBase.UpdateItemAsync(doc, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
            }



            if (!string.IsNullOrEmpty(doc.DefaultFont))
            {
                tsVisitor.DefaultFont = doc.DefaultFont;
            }
            else
            {
                tsVisitor.DefaultFont = string.Empty;
            }


            return tsVisitor;
        }


        [FunctionName("FunVisitorUpdate")]
        public async Task<ActionResult> Update(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "Visitor/update")] HttpRequest req,
        ILogger log)
        {

            MyTokenValidator.Authenticate(req, AllowedRoles, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            TSVisitor tsVisitor = await MyFromBody<TSVisitor>.FromBody(req, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            bool b = await CosmosAPI.cosmosDBClientVisitor.UpdateVisitor(tsVisitor, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));

            if (b)
            {

                return new OkObjectResult("OK");
            }
            else
            {
                return new OkObjectResult("Error:Can't update Visitor!");
            }
        }
    }
}
