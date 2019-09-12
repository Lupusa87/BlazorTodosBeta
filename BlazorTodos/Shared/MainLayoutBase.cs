using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TG.Blazor.IndexedDB;
using TodosUITranslator;
using static BlazorTodos.Classes.Enums;

namespace BlazorTodos.Shared
{
    public class MainLayoutBase: LayoutComponentBase
    {
        [Inject]
        HttpClient httpClient { get; set; }

        [Inject]
        IUriHelper uriHelper { get; set; }

        [Inject]
        IJSRuntime jsRuntime { get; set; }

        [Inject] IndexedDBManager indexDbManager { get; set; }

        protected override void OnInitialized()
        {
            if (WebApiFunctions.httpClient is null)
            {
                WebApiFunctions.httpClient = httpClient;
            }

            if (LocalFunctions.uriHelper is null)
            {
                LocalFunctions.uriHelper = uriHelper;
            }

            if (BTodosJsInterop.jsRuntime is null)
            {
                BTodosJsInterop.jsRuntime = jsRuntime;
            }

            if (LocalData.indexDbManager is null)
            {
                LocalData.indexDbManager = indexDbManager;

            }


            if (!uriHelper.GetBaseUri().Equals(uriHelper.GetAbsoluteUri()))
            {
                uriHelper.NavigateTo("/");
            }


            LocalData.mainLayout = this;


           
            LocalFunctions.CmdPrepare();




            base.OnInitialized();
        }


        public void Refresh()
        {
            StateHasChanged();
        }

        protected override void OnAfterRender()
        {


            if (LocalData.compContextMenu !=null)
            {
                LocalData.compContextMenu.OnClick = ContextMenuOnClick;
            }



            base.OnAfterRender();
        }

        private void ContextMenuOnClick(int id)
        {
            if (id == 1)
            {
                LocalFunctions.CmdNavigate("ProfilePage");
            }
            else
            { 
                if (LocalData.bcMenu.Name.Equals("Logout"))
                {
                    LocalFunctions.Logout();  
                }
            }

            LocalFunctions.ContextMenu_Hide();
        }


        protected void CmdOnMouseUp(UIMouseEventArgs e)
        {

            if (LocalData.bcMenu.ID > -1)
            {
                if (e.ClientX < LocalData.bcMenu.X || e.ClientX > LocalData.bcMenu.X + LocalData.bcMenu.width)
                {
                    LocalFunctions.ContextMenu_Hide();
                   
                }
                else if(e.ClientY < LocalData.bcMenu.Y || e.ClientY > LocalData.bcMenu.Y + LocalData.bcMenu.height)
                {
                    LocalFunctions.ContextMenu_Hide();
                   
                }
            }
        }

    }
}
