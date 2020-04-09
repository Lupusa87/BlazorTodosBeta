using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorTodos.Pages
{
    public partial class DesktopPage
    {

        

        protected override void OnInitialized()
        {

            if (LocalData.uiTranslator != null)
            {
                LocalData.uiTranslator.OnUILanguageChanged = TranslatorOnUILanguageChanged;
            }
          
            base.OnInitialized();
        }

     


        public void TranslatorOnUILanguageChanged()
        {

            StateHasChanged();
        }

    }
}
