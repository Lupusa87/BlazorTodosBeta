using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AzureTranslatorAPI
{
    public class AzureTranslator
    {
        //https://docs.microsoft.com/en-us/azure/cognitive-services/translator/quickstart-csharp-translate
        //8/9/2019

        string _host = "https://api.cognitive.microsofttranslator.com";
        string _route = "/translate?api-version=3.0&to=";
        string _subscriptionKey = string.Empty;

        JsonDocumentOptions options = new JsonDocumentOptions
        {
            AllowTrailingCommas = true
        };

        JsonSerializerOptions opt = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true

        };



        private HttpClient client = null;

        public AzureTranslator(string subscriptionKey)
        {


            if (string.IsNullOrEmpty(subscriptionKey))
            {
                throw new ArgumentException("azure translator subscription key is empty!");
            }

            _subscriptionKey = subscriptionKey;
            client = new HttpClient();
        }


        //public async Task TranslateTextRequest(string subscriptionKey, string host, string route, string inputText)
        public async Task<string> TranslateAsync(string inputText, string to)
        {

            object[] body = new object[] { new { Text = inputText } };
            var requestBody = JsonSerializer.Serialize(body);

            
            using (var request = new HttpRequestMessage())
            {
              
                request.Method = HttpMethod.Post;
               
                request.RequestUri = new Uri(_host + _route + to);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);

                
                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
               
                string result = await response.Content.ReadAsStringAsync();

                result = result.Replace("[", null);
                result = result.Replace("]", null);

                JsonDocument document = JsonDocument.Parse(result, options);
          
                return document.RootElement.GetProperty("translations").GetProperty("text").GetRawText();
            }


           
        }


        public string Translate(string inputText, string to)
        {



            object[] body = new object[] { new { Text = inputText } };
            var requestBody = JsonSerializer.Serialize(body);



           
            using (var request = new HttpRequestMessage())
            {

                request.Method = HttpMethod.Post;

                request.RequestUri = new Uri(_host + _route + to);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);


                HttpResponseMessage response = client.SendAsync(request).Result;

                string result = response.Content.ReadAsStringAsync().Result;

                result = result.Replace("[", null);
                result = result.Replace("]", null);

                JsonDocument document = JsonDocument.Parse(result, options);
 
                return document.RootElement.GetProperty("translations").GetProperty("text").GetRawText();
            }



        }
    }



   


}
