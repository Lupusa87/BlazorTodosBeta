using Google.Apis.Logging;
using Google.Cloud.Translation.V2;
using GoogleTranslatorAPI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TodosCosmos;
using TodosFunctionsApi.JwtSecurity;
using TodosShared;
using static TodosShared.TSEnums;

namespace TodosFunctionsApi.API
{
    public class FunGoogleTranslator
    {



        private readonly GoogleTranslator _googleTranslator;


        public FunGoogleTranslator(GoogleTranslator googleTranslator)
        {
            _googleTranslator = googleTranslator;
           
        }


        private List<WebApiUserTypesEnum> AllowedRoles = new List<WebApiUserTypesEnum>
            {
                WebApiUserTypesEnum.NotAuthorized,
                WebApiUserTypesEnum.Authorized,
                WebApiUserTypesEnum.Admin
            };


        [FunctionName("FunGoogleTranslatorGetAllLanguages")]
        public async Task<ActionResult<IEnumerable<TSUILanguageShort>>> GetAllLanguages(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "googletranslator/getalllanguages")] HttpRequest req)
        {

            ClaimsPrincipal User = MyTokenValidator.Authenticate(req, AllowedRoles);

            
            IEnumerable<Language> list = await _googleTranslator.GetLanguagesAsync();


            IList<TSUILanguageShort> result = new List<TSUILanguageShort>();

            foreach (var item in list)
            {
                result.Add(new TSUILanguageShort
                {
                    Name = item.Name,
                    Code = item.Code
                });
            }


            return new ActionResult<IEnumerable<TSUILanguageShort>>(result.AsEnumerable());
        }


    }
}
