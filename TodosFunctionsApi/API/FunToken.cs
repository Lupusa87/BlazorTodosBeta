using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Reflection;
using TodosShared;
using TodosFunctionsApi.JwtSecurity;
using TodosCosmos;

namespace TodosFunctionsApi.API
{
    public class FunToken
    {

        [FunctionName("FunToken")]
        public async Task<ActionResult<JwtResult>> Token(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "token")] HttpRequest req,
            ILogger log)
        {

            await CosmosAPI.cosmosDBClientActivity.AddActivityLog(Guid.Empty, "Requested token", MethodBase.GetCurrentMethod());

            JwtResult result = await new MyTokenProvider().GenerateToken(req);

            return result;
            //return new OkObjectResult(JsonSerializer.ToString(result));

        }



    }
}
