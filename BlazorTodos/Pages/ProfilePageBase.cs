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
    public class ProfilePageBase:ComponentBase
    {

     

        protected override void OnInitialized()
        {
           

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
    }
}
