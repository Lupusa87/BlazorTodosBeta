using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using TodosShared;

namespace BlazorTodos.Pages
{
    public class UISupportedLanguagesPageBase: ComponentBase
    {

     
        public List<TSUILanguage> UIExistingLanguagesList { get; set; }

        public string SupportedLanguagesListCount { get; set; } = LocalData.uiTranslator.Translate("Supported languages list") + " (0)";

        public string ExistingLanguagesListCount { get; set; } = LocalData.uiTranslator.Translate("Existing languages list") + " (0)";

        protected async override void OnInitialized()
        {


           

            //if (LocalData.IsAuthenticated)
            //{
            await CmdGetSupportedLanguages();

            //}
            //else
            //{
            //    LocalFunctions.CmdNavigate();

            //    LocalFunctions.DisplayMessage("Please login");
            //}


            base.OnInitialized();
        }

        private async void CmdRefresh()
        {

            if (LocalData.IsAuthenticated)
            {
                await CmdGetSupportedLanguages();
            }
            else
            {

                StateHasChanged();
            }


        }


        public async Task CmdGetSupportedLanguages()
        {

            

            try
            {
                if (LocalData.UISupportedLanguagesList is null)
                {
                    LocalData.UISupportedLanguagesList = await WebApiFunctions.CmdGetSupportedLanguages();

              

                    foreach (var item in LocalData.uiTranslator.TSUILanguagesList)
                    {
                        if (LocalData.UISupportedLanguagesList.Any(x=>x.Code.Equals(item.Code, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            LocalData.UISupportedLanguagesList.Single(x => x.Code.Equals(item.Code, StringComparison.InvariantCultureIgnoreCase)).Exists=true;
                        }
                    }


                    int k = 1;
                    foreach (var item in LocalData.UISupportedLanguagesList.OrderByDescending(x=>x.Exists).ThenBy(x=>x.Name))
                    {
                        item.N = k++;
                    }


                }

                SupportedLanguagesListCount = LocalData.uiTranslator.Translate("Supported languages list") + " (" + LocalData.UISupportedLanguagesList.Count + ")";
                ExistingLanguagesListCount = LocalData.uiTranslator.Translate("Existing languages list") + " (" + LocalData.uiTranslator.TSUILanguagesList.Count + ")"; 

                StateHasChanged();


            }
            catch (Exception ex)
            {

                LocalFunctions.AddError(ex.Message, MethodBase.GetCurrentMethod(), true, false);
            }
        }


        protected async Task CmdAddUILanguage(string LangName)
        {

        }


        protected void CmdViewWordsList(string LangName)
        {
            LocalFunctions.CmdNavigate(@"UIWordsListPage\" + LangName);
            LocalFunctions.ConsolePrint(LangName);
        }

    }
}
