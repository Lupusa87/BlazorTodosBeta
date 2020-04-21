using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TodosShared;
using static TodosShared.TSEnums;

namespace BlazorTodos.Modals
{
    public partial class CompDefaultFont
    {

        protected bool IsButtonDisabled { get; set; } = false;

        public async void CmdKeyUp(KeyboardEventArgs a)
        {
    
            if (a.Key.Equals("Enter"))
            {
                await CmdSaveFont();
            }
        }

        public async Task CmdSaveFont()
        {

            IsButtonDisabled = true;
            StateHasChanged();


            if (string.IsNullOrEmpty(LocalData.CurrDefaultFont))
            {
                LocalData.CurrDefaultFont = "Sylfaen";
            }

            if (LocalData.OldDefaultFont == LocalData.CurrDefaultFont)
            {
                IsButtonDisabled = false;
                StateHasChanged();
                LocalData.btModal.Close();
            }


            LocalData.CurrVisitor.DefaultFont = LocalData.CurrDefaultFont;
            await WebApiFunctions.CmdUpdateVisitor();

            if (LocalData.IsAuthenticated)
            {
                if (LocalData.CurrTSUser.UserName.Equals("demouser"))
                {
                    LocalFunctions.AddMessage("Demo user can't save font(s), it will be saved for IP address", true, false);
                }
                else
                {
                    LocalData.CurrTSUser.DefaultFont = LocalData.CurrDefaultFont;
                    await WebApiFunctions.CmdTSUserUpdateFont(new TSVisitor { IPAddress = string.Empty, DefaultFont = LocalData.CurrDefaultFont });
                }

              
                LocalData.mainLayout.Refresh();
                LocalData.profilePage.Refresh();

            }
            else
            {
                LocalData.mainLayout.Refresh();
                LocalData.indexPage.Refresh();
            }



            IsButtonDisabled = false;
            StateHasChanged();
            LocalData.btModal.Close();
        }

     
        
    }
}
