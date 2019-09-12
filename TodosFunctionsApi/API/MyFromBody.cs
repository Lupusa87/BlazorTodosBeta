using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TodosFunctionsApi.API
{
    public static class MyFromBody<T> where T : class
    {

        public static async Task<T> FromBody(HttpRequest req)
        {

            try
            {
                StreamReader st = new StreamReader(req.Body);
                string json = await st.ReadToEndAsync();
                st.Dispose();


                var options = new JsonSerializerOptions
                {
                    AllowTrailingCommas = true,
                    PropertyNameCaseInsensitive = true,
                };

                return JsonSerializer.Deserialize<T>(json, options);
            }
            catch (Exception ex)
            {

                return null;
            }

           

        }
    }
}
