using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TodosShared;
using static BlazorTodos.Classes.Enums;

namespace BlazorTodos.Pages
{
    public partial class ProfilePage
    {

     

        protected override void OnInitialized()
        {

            LocalData.profilePage = this;


            if (!LocalData.IsAuthenticated)
            {
                LocalFunctions.CmdNavigate();

                LocalFunctions.AddMessage("Please login", true, false);
            }

            base.OnInitialized();
        }


        public void CmdDisplayChangePassword()
        {
            if (LocalData.IsAuthenticated)
            {
                LocalFunctions.DisplayModal(ModalForm.ChangePassword);
            }
            else
            {
                LocalFunctions.AddMessage("You are not authenticated!", true, false);
            }
        }

        public void CmdDisplayDefaultFont()
        {
            if (LocalData.IsAuthenticated)
            {
                LocalData.OldDefaultFont = LocalData.CurrDefaultFont;
                LocalFunctions.DisplayModal(ModalForm.DefaultFont);
            }
            else
            {
                LocalFunctions.AddMessage("You are not authenticated!", true, false);
            }
            
        }

        public void Refresh()
        {
            StateHasChanged();
        }
    }
}
