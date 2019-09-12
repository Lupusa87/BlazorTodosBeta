using Google.Apis.Auth.OAuth2;
using Google.Cloud.Translation.V2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace GoogleTranslatorAPI
{
    public class GoogleTranslator
    {

        //https://cloud.google.com/translate/docs/reference/libraries
        //8/10/2019


        TranslationClient client = null;


        

        public GoogleTranslator()
        {


            try
            {
                GoogleCredential credential = GoogleCredential.FromFile("wise-sphere-249416-72e117af0d69.json");

                client = TranslationClient.Create(credential);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Can't load google translator keys!");
                //throw;
            }



            

         
        }

        public string Translate(string inputText, string to)
        {
           
            Console.OutputEncoding = System.Text.Encoding.Unicode;

           
            var response = client.TranslateText(inputText, to);

            return response.TranslatedText;
        }


        public IEnumerable<Language> GetLanguages()
        {

            //https://cloud.google.com/translate/docs/discovering-supported-languages#translate_list_codes-csharp

            return client.ListLanguages(target: "en").AsEnumerable();  
        }


        public async Task<IEnumerable<Language>> GetLanguagesAsync()
        {

            //https://cloud.google.com/translate/docs/discovering-supported-languages#translate_list_codes-csharp

            return (await client.ListLanguagesAsync(target: "en")).AsEnumerable();
        }

    }
}
