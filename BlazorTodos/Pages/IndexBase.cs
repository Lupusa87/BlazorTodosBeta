using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using TG.Blazor.IndexedDB;
using TodosShared;
using TodosUITranslator;
using static BlazorTodos.Classes.Enums;

namespace BlazorTodos.Pages
{
    public class IndexBase:ComponentBase
    {


        protected override void OnAfterRender(bool firstRender)
        {
            if (LocalData.uiTranslator != null)
            {

                LocalData.uiTranslator.OnNotFoundWord = TranslatorOnNotFoundWord;
                LocalData.uiTranslator.OnUILanguageChanged = TranslatorOnUILanguageChanged;
            }

            base.OnAfterRender(firstRender);
        }

      
        public void TranslatorOnNotFoundWord(string word)
        {
            BTodosJsInterop.Log("Could not translate word - " + word + ", not found!");
        }


        public void TranslatorOnUILanguageChanged()
        {
            StateHasChanged();
        }

        public void CmdDisplayRegistration()
        {

            if (!LocalData.IsAuthenticated)
            {
                LocalFunctions.DisplayModal(ModalForm.Registration);
            }
            else
            {
                LocalFunctions.AddMessage("Please logout first, to register new user", true, false);
            }
            

        }

        public void CmdLoginDemoUser()
        {

            if (LocalData.IsReady)
            {
                if (!LocalData.IsAuthenticated)
                {
                    LocalFunctions.Authorize("DemoUser", "123456789");
                }
                else
                {
                    LocalFunctions.AddMessage("Please logout first, to log into demo user", true, false);
                }
            }
            else
            {
                LocalFunctions.AddMessage("Not ready yet, please try again", true, false);
            }
           


        }


        public void CmdDisplayLogin()
        {
            if (!LocalData.IsAuthenticated)
            {
                LocalFunctions.DisplayModal(ModalForm.Login);
            }
            else
            {
                LocalFunctions.AddMessage("You are authenticated already", true, false);
            }
        }



    }
}
