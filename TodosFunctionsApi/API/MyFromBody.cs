using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TodosCosmos;

namespace TodosFunctionsApi.API
{
    public static class MyFromBody<T> where T : class
    {
        static JsonSerializerOptions options = new JsonSerializerOptions
        {
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true,
        }; 

        public static async Task<T> FromBody(HttpRequest req, List<string> CallTrace)
        {

            try
            {

                StreamReader st = new StreamReader(req.Body);
                string json = await st.ReadToEndAsync();
                st.Dispose();


                T result = JsonSerializer.Deserialize<T>(json, options);

                if (result is null)
                {
                    await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, "Deserialization failed!", TodosCosmos.LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
                }

                return result;

            }
            catch (Exception ex)
            {
                await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, "FromBody parse error: " + ex.Message, TodosCosmos.LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

                //throw new ArgumentNullException("FromBody parse error: " + ex.Message);
                return null;
            }

           

        }
    }
}
