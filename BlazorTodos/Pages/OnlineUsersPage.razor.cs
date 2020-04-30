using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using TodosShared;
using static BlazorTodos.Classes.Enums;

namespace BlazorTodos.Pages
{
    public partial class OnlineUsersPage
    {
      

        public List<TSUserOpenEx> TsUserOpenList { get; set; }

        public string UsersListCount { get; set; } = LocalData.uiTranslator.Translate("Online users list") + " (0)";

        protected async override void OnInitialized()
        {

            //if (LocalData.IsAuthenticated)
            //{
                await CmdGetLiveUsers();

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
                    await CmdGetLiveUsers();
                }
                else
                {

                    StateHasChanged();
                }

           
        }


        public async Task CmdGetLiveUsers()
        {
            try
            {

                TsUserOpenList = await WebApiFunctions.CmdGetLiveUsers();

      

                foreach (var item in TsUserOpenList)
                {
                    item.Days = (short)(DateTime.Now - item.CreateDate).TotalDays;
                }

                for (int i = 0; i < TsUserOpenList.Count; i++)
                {
                    TsUserOpenList[i].N = i + 1;
                }


                UsersListCount = LocalData.uiTranslator.Translate("Online users list") + " (" + TsUserOpenList.Count + ")";


                StateHasChanged();


            }
            catch (Exception ex)
            {

                LocalFunctions.AddError(ex.Message, MethodBase.GetCurrentMethod(), true, false);
            }
        }


    }
}

