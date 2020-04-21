using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using TodosGlobal;

namespace TodosFunctionsApi.API
{

    public class FunTestAPI
    {

        [FunctionName("FunTestAPI")]
        public ActionResult<IEnumerable<string>> Testapi(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "testapi")] HttpRequest req)
        {

            return new string[] { "API is live", "Azure Function rocks", GlobalData.IsDevelopmentMode.ToString() };
        }


       
    }

}
