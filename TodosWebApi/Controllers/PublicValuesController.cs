using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TodosWebApi.DataLayer;
using TodosWebApi.GlobalDataLayer;


namespace TodosWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublicValuesController : Controller
    {
        private readonly ILogger _logger;
        private readonly TableStorage TS;

        public PublicValuesController(ILogger<TodosController> logger)
        {
            _logger = logger;
            TS = new TableStorage();
        }

        // GET: api/values
        //[EnableCors]
        [HttpGet]
        public async Task<string> Get()
        {

            await TS.AddActivityLog("AllUser", "Requested public data", MethodBase.GetCurrentMethod());
            return GlobalFunctions.CmdGetPublicData();
        }

    }
}
