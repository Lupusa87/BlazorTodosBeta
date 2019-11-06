using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodosShared;

namespace BlazorTodos.Components
{
    public class CompFooterBase : ComponentBase
    {


        public int ComboSelectedIndex
        {
            get
            {

                return LocalData.uiTranslator.ComboUILanguagesSelectedIndex;
            }
            set
            {

                if (value == LocalData.uiTranslator.TSUILanguagesList.Count)
                {

                    StateHasChanged();

                    LocalFunctions.CmdNavigate("UISupportedLanguagesPage");

                }
                else
                {
                    LocalData.uiTranslator.ComboUILanguagesSelectedIndex = value;
                    ComboUILanguageSelectionChanged();
                }
            }
        }

        public void Refresh()
        {
            StateHasChanged();
        }




        protected override void OnAfterRender(bool firstRender)
        {

            if (firstRender)
            {

                if (LocalData.uiTranslator.TSUILanguagesList.Any())
                {

                    if (LocalData.uiTranslator.ComboUILanguagesSelectedIndex > 0)
                    {
                        ComboUILanguageSelectionChanged();
                    }
                }
            }

            base.OnAfterRender(firstRender);
        }


        public async void ComboUILanguageSelectionChanged()
        {
            //BTodosJsInterop.Log("======aaaaaaa=====");
            //var results = await LocalData.indexDbManager.GetRecords<TSUILanguage>("UILanguages");
            //foreach (var item in results)
            //{
            //    BTodosJsInterop.Log(item.Name);
            //    await LocalData.indexDbManager.DeleteRecord("UILanguages", item);
            //}

            //BTodosJsInterop.Log("done");
            //// await LocalData.indexDbManager.ClearStore("UILanguages");

            ////  await LocalData.indexDbManager.ClearStore("UILangDict");

            //return;


            LocalData.uiTranslator.CurrUILanguage = LocalData.uiTranslator.TSUILanguagesList[LocalData.uiTranslator.ComboUILanguagesSelectedIndex];


            if (!LocalData.uiTranslator.CurrUILanguage.Code.Equals("en", StringComparison.InvariantCultureIgnoreCase))
            {


                await LocalFunctions.GetUILangDict();

                //BTodosJsInterop.Log("====================");

                //foreach (var item in LocalData.TSUIWordForeignsList)
                //{
                //    BTodosJsInterop.Log(item.Word);
                //}

            }


            if (LocalData.IsAuthenticated)
            {
                LocalFunctions.CmdNavigate("DesktopPage");
            }
            else
            {

                LocalFunctions.CmdNavigate();
            }


            LocalData.uiTranslator.OnUILanguageChanged?.Invoke();

            LocalData.compHeader.Refresh();


           

            StateHasChanged();


            


        }
    }
}
