using AzureTranslatorAPI;
using GoogleTranslatorAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TodosCosmos;
using TodosCosmos.DocumentClasses;
using TodosGlobal;
using TodosShared;

namespace TodosFunctionsApi
{
    public static class SeedUILanguageData
    {

        private static Dictionary<string, Guid> UILangDict = new Dictionary<string, Guid>();
        private static Dictionary<string, Guid> UINativeWordsDict = new Dictionary<string, Guid>();

        public static bool Seed()
        {
            //Console.OutputEncoding = Encoding.Unicode;

            //GoogleTranslator googleTranslator = new GoogleTranslator();


            //foreach (var item in googleTranslator.GetLanguages())
            //{
            //    Console.WriteLine(item.Code + "    -      " + item.Name);
            //}


            //Console.WriteLine(googleTranslator.Translate("Hello world", "ru"));


            //AzureTranslator azureTranslator = new AzureTranslator(GlobalData.AzureTranslatorSubscriptionKey);

            //Console.WriteLine(azureTranslator.Translate("Hello world", "ru"));



            //AzureTranlsateAllNativeWordsToLang(azureTranslator, "ru");

            //GoogleTranlsateAllNativeWordsToLang(googleTranslator, "ka");



            DeleteCosmosDBData.DeleteActivity();

            DeleteCosmosDBData.DeleteUILanguages();
            DeleteCosmosDBData.DeleteUIWordNative();
            DeleteCosmosDBData.DeleteUIWordForeign();


            SeedLanguages();

            UILangDict = CosmosAPI.cosmosDBClientUILanguage.GetAll().Result.ToDictionary(x => x.Code, x => x.ID);

            SeedFromFile();


            foreach (var item in CosmosAPI.cosmosDBClientUILanguage.GetAll().Result)
            {
                UpdateLanguageVersion(item);
            }


            return true;
        }



        private static void AzureTranlsateAllNativeWordsToLang(AzureTranslator azureTranslator, string to)
        {

            List<TSUIWordNative> listNatives = CosmosAPI.cosmosDBClientUIWordNative.GetAll().Result;

            foreach (var item in listNatives)
            {
                Console.WriteLine(item.Word + " - " + azureTranslator.Translate(item.Word, to));
            }

        }

        private static void GoogleTranlsateAllNativeWordsToLang(GoogleTranslator googleTranslator, string to)
        {

            List<TSUIWordNative> listNatives = CosmosAPI.cosmosDBClientUIWordNative.GetAll().Result;

            foreach (var item in listNatives)
            {
                Console.WriteLine(item.Word + " - " + googleTranslator.Translate(item.Word, to));
            }

        }

        private static string SeedFromFile()
        {

            List<string> NativeList = new List<string>();

            Dictionary<string, string> GeorgianDict = new Dictionary<string, string>();
            Dictionary<string, string> RussianDict = new Dictionary<string, string>();

            string[] a;


            string[] lines = File.ReadAllLines("E://dict.json");

            foreach (var item in lines)
            {
                a = item.Split("|");


                

                NativeList.Add(a[0]);
                GeorgianDict.Add(a[0], a[1]);

                if (a.Length == 3)
                {
                    if (!string.IsNullOrEmpty(a[2]))
                    {
                        RussianDict.Add(a[0], a[2]);
                    }
                }
            }



            foreach (var item in NativeList)
            {
                AddUIWordNative(item);
            }

            UINativeWordsDict = CosmosAPI.cosmosDBClientUIWordNative.GetAll().Result.ToDictionary(x => x.Word, x => x.ID);


            foreach (var item in GeorgianDict)
            {
                AddUIWordForeign("ka", item.Key, item.Value, true);
            }


            foreach (var item in RussianDict)
            {
                AddUIWordForeign("ru", item.Key, item.Value, true);
            }

            return string.Empty;
        }


        private static bool SeedLanguages()
        {

          

            TSUILanguage lang_English = new TSUILanguage()
            {
                ID = Guid.NewGuid(),
                Name = "English",
                Code = "en",
                Version = DateTime.UtcNow,
            };

            AddLanguage(lang_English);

            TSUILanguage lang_Georgian = new TSUILanguage()
            {
                ID = Guid.NewGuid(),
                Name = "Georgian",
                Code = "ka",
                Version = DateTime.UtcNow,
            };

            AddLanguage(lang_Georgian);

            TSUILanguage lang_Russian = new TSUILanguage()
            {
                ID = Guid.NewGuid(),
                Name = "Russian",
                Code = "ru",
                Version = DateTime.UtcNow,
            };

            AddLanguage(lang_Russian);


            return true;
        }



        private static bool AddLanguage(TSUILanguage lang)
        {
            CosmosDocUILanguage UILanguage = CosmosAPI.cosmosDBClientUILanguage.FindByName(lang.Name).Result;

            if (UILanguage is null)
            {
              return CosmosAPI.cosmosDBClientUILanguage.Add(lang).Result;

            }
            else
            {
                return false;
            }
        }

        private static bool UpdateLanguageVersion(TSUILanguage lang)
        {

           lang.Version = DateTime.UtcNow;

           return CosmosAPI.cosmosDBClientUILanguage.Update(lang).Result;
        }


        private static bool AddUIWordNative(string word)
        {
            if (!string.IsNullOrEmpty(word))
            {
                word = word.ToLower();
                CosmosDocUIWordNative UIWordNative = CosmosAPI.cosmosDBClientUIWordNative.FindByWord(word).Result;

                if (UIWordNative is null)
                {

                    TSUIWordNative uiWordNative = new TSUIWordNative()
                    {
                        ID = Guid.NewGuid(),
                        Word = word
                    };

                    return CosmosAPI.cosmosDBClientUIWordNative.Add(uiWordNative).Result;

                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }


        private static bool AddUIWordForeign(string Lang, string NativeWord, string ForeignWord, bool IsHuman)
        {

            if (string.IsNullOrEmpty(NativeWord))
            {
                throw new ArgumentException("native word is empty!");
            }

            if (string.IsNullOrEmpty(ForeignWord))
            {
                throw new ArgumentException("foreign word is empty!");
            }


            NativeWord = NativeWord.ToLower();


                Guid UIWordNativeID;

                Guid UILanguageID;



           

                if (UINativeWordsDict.TryGetValue(NativeWord, out UIWordNativeID))
                {
                    if (UILangDict.TryGetValue(Lang, out UILanguageID))
                    {
                        CosmosDocUIWordForeign UIWordForeign = CosmosAPI.cosmosDBClientUIWordForeign.FindByWordAndNativeWordID(ForeignWord, UIWordNativeID).Result;

                        if (UIWordForeign is null)
                        {


                            TSUIWordForeign uiWordForeign = new TSUIWordForeign
                            {
                                ID = Guid.NewGuid(),
                                Word = ForeignWord,
                                UILanguageID = UILanguageID,
                                UIWordNativeID = UIWordNativeID,
                                Human = IsHuman
                            };


                            return CosmosAPI.cosmosDBClientUIWordForeign.Add(uiWordForeign).Result;

                        }
                        else
                        {
                        return false;
                            //throw new ArgumentException("word " + ForeignWord + " with same foreign word id exists already! value in base - " + UIWordForeign.Word);
                        }
                    }
                    else
                    {
                        throw new ArgumentException("language " + Lang + " does not exists!");
                    }
                }
                else
                {
                    throw new ArgumentException("native word " + NativeWord + " does not exists!");
                }
           
        }


       


      
    }
}
