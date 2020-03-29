using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BlazorTodos
{
    public static class HttpClientEx
    {

        //public static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        //{
        //    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        //};


        //static JsonDocumentOptions options = new JsonDocumentOptions
        //{
        //    AllowTrailingCommas = true
        //};

        static JsonSerializerOptions opt = new JsonSerializerOptions
        {
           
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true

        };

        public static async Task<T> MyGetJsonAsync<T>(this HttpClient client, string requestUri)
        {
            if (client is HttpClient)
            {
                string a = await client.GetStringAsync(requestUri);

                
                return JsonSerializer.Deserialize<T>(a, opt);

                //JsonDocument document = JsonDocument.Parse(a, options);
                //return JsonSerializer.Deserialize<T>(document.RootElement.GetProperty("value").GetRawText(), opt);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public static async Task<T> MyPostFormGetJsonAsync<T>(this HttpClient client, string requestUri, HttpContent content)
        {

            if (client is HttpClient)
            {

                var response = await client.PostAsync(requestUri, content);

                return JsonSerializer.Deserialize<T>(await response.Content.ReadAsStringAsync(), opt);

                
                //JsonDocument document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                //return JsonSerializer.Deserialize<T>(document.RootElement.GetProperty("value").GetRawText(), opt);

            }
            else
            {
                throw new InvalidOperationException();
            }
        }


        public static async Task<T> MyPostJsonGetJsonAsync<T>(this HttpClient client, string requestUri, T content)
        {
            if (client is HttpClient)
            {
               
                return await SendJsonGetJsonAsync<T>(client, HttpMethod.Post, requestUri, content);

            }
            else
            {
                throw new InvalidOperationException();
            }
        }


        public static async Task<U> MyPostJsonGetJsonEnumAsync<U, T>(this HttpClient client, string requestUri, T content) where U : IEnumerable<T>
        {
            if (client is HttpClient)
            {
              
                return await SendJsonGetJsonEnumAsync<U,T>(client, HttpMethod.Post, requestUri, content);

            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public static async Task<string> MyPostJsonGetStringAsync(this HttpClient client, string requestUri, object content)
        {
            if (client is HttpClient)
            {
                var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Post, requestUri)
                {
                    Content = new StringContent(JsonSerializer.Serialize(content, opt), Encoding.UTF8, "application/json")
                });


                return await response.Content.ReadAsStringAsync();


                //var stringContent = await response.Content.ReadAsStringAsync();

                //JsonDocument document = JsonDocument.Parse(stringContent);

                //return document.RootElement.GetProperty("value").GetRawText();

            }
            else
            {
                throw new InvalidOperationException();
            }
        }


        private static async Task<T> SendJsonGetJsonAsync<T>( HttpClient httpClient, HttpMethod method, string requestUri, object content)
        {
            var response = await httpClient.SendAsync(new HttpRequestMessage(method, requestUri)
            {
                Content = new StringContent(JsonSerializer.Serialize(content, opt), Encoding.UTF8, "application/json")

            });


            return JsonSerializer.Deserialize<T>(await response.Content.ReadAsStringAsync(), opt);

            //var stringContent = await response.Content.ReadAsStringAsync();

            //JsonDocument document = JsonDocument.Parse(stringContent);

            //return JsonSerializer.Deserialize<T>(document.RootElement.GetProperty("value").GetRawText(), opt);

        }


        private static async Task<U> SendJsonGetJsonEnumAsync<U, T>(HttpClient httpClient, HttpMethod method, string requestUri, object content) where U : IEnumerable<T>
        {
           
            var response = await httpClient.SendAsync(new HttpRequestMessage(method, requestUri)
            {
                Content = new StringContent(JsonSerializer.Serialize(content, opt), Encoding.UTF8, "application/json")
            });

            return JsonSerializer.Deserialize<U>(await response.Content.ReadAsStringAsync(), opt);

            //var stringContent = await response.Content.ReadAsStringAsync();

            //JsonDocument document = JsonDocument.Parse(stringContent);

            //return JsonSerializer.Deserialize<U>(document.RootElement.GetProperty("value").GetRawText(), opt);



        }
    }
}
