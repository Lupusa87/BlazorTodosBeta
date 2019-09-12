using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;


namespace TodosFunctionsApi.API
{

    public class FunTestAPI
    {

        [FunctionName("FunTestAPI")]
        public ActionResult<IEnumerable<string>> testapi(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "testapi")] HttpRequest req,
            ILogger log)
        {
            return new string[] { "API is live", "Azure Functions rocks" };
        }


       
    }

}
