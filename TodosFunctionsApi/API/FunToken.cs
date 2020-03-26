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
using System.Text;
using TodosGlobal;
using Microsoft.IdentityModel.Tokens;

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


            try
            {
                SymmetricSecurityKey sk = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("asasasassasasasa"));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.InnerException.Message);
            }

         
            //string a = GlobalData.JWTSecret;


            //Encoding.ASCII.GetBytes(GlobalData.JWTSecret);

            //SymmetricSecurityKey sk = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(GlobalData.JWTSecret));

            //SigningCredentials SigningCredentials1 = new SigningCredentials(sk, SecurityAlgorithms.HmacSha256);

            //MyTokenProviderOptions _options = new MyTokenProviderOptions
            //{
            //    Audience = "ExampleAudience",
            //    Issuer = "ExampleIssuer",
            //    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(GlobalData.JWTSecret)), SecurityAlgorithms.HmacSha256),

            //};


            MyTokenProvider tp = new MyTokenProvider();


            JwtResult result = await tp.GenerateToken(req);

            return result;
            //return new OkObjectResult(JsonSerializer.ToString(result));

        }



    }
}
