using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using TodosShared;
using static BlazorTodos.Classes.Enums;

namespace BlazorTodos.Pages
{
    public partial class TodosPage
    {

       

        public string TodosListCount { get; set; } = LocalData.uiTranslator.Translate("Todos list") + " (0)";

        protected Guid CurrID { get; set; } = Guid.Empty;

        protected async override void OnInitialized()
        {


            CurrID = Guid.Empty;

            if (LocalData.componentBridge != null)
            {
                LocalData.componentBridge.OnRefresh = CmdRefresh;
                LocalData.componentBridge.OnConfirm = CmdActualDelete;
            }

           

            if (LocalData.IsAuthenticated)
            {
                await CmdGetAllTodos();             

            }
            else
            {
                LocalFunctions.CmdNavigate();

                LocalFunctions.AddMessage("Please login", true, false);
            }


            base.OnInitialized();
        }

        private void CmdRefresh()
        {
            if (LocalData.EventConsumerName.Equals("TodosPage"))
            {
                CmdReload();

            }
        }

        protected async void CmdReload()
        {

            if (LocalData.IsAuthenticated)
            {
                await CmdGetAllTodos();
            }
            else
            {

                StateHasChanged();
            }
        }

        private async void CmdActualDelete()
        {
            if (LocalData.EventConsumerName.Equals("TodosPage"))
            {
                if (LocalData.TsTodosList.Any())
                {
                    string a = await WebApiFunctions.CmdDeleteTodo(LocalData.TsTodosList.Single(x => x.ID == CurrID));

                    if (a.Equals("OK"))
                    {
                        LocalData.TsTodosList = new List<TSTodoEx>();

                        LocalFunctions.CmdNavigate("TodosPage");
                        LocalData.componentBridge.InvokeRefresh();

                    }
                    else
                    {
                        LocalFunctions.AddError(a, MethodBase.GetCurrentMethod(), true, false);

                    }
                }
            }

        }


        public async Task CmdGetAllTodos()
        {
            try
            {

                await LocalFunctions.GetTodosAndCategories();

                TodosListCount = LocalData.uiTranslator.Translate("Todos list") + " (" + LocalData.TsTodosList.Count + ")";

                StateHasChanged();


            }
            catch (Exception ex)
            {

                LocalFunctions.AddError(ex.Message, MethodBase.GetCurrentMethod(), true, false);
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


        public void CmdAdd()
        {
            LocalData.AddOrUpdateMode = true;
            LocalData.CurrTodo = new TSTodo
            {
                DueDate = DateTime.Now.AddDays(7),
                Priority = 1,
                CategoryID = Guid.Empty
            };

            LocalData.BeforeUpdateTodo = new TSTodo();
            LocalData.compAddOrUpdateTodo.Bootstrap();
            LocalFunctions.DisplayModal(ModalForm.AddOrUpdateTodo);
        }


        public void CmdUpdate()
        {

            if (LocalData.TsTodosList.Any())
            {

                if (!CurrID.Equals(Guid.Empty))
                {
                    LocalData.AddOrUpdateMode = false;
                    LocalData.CurrTodo = new TSTodo();
                    LocalData.CurrTodo = LocalData.TsTodosList.Single(x => x.ID == CurrID);


    

                    if (!LocalData.CurrTodo.HasDueDate)
                    {
                        LocalData.CurrTodo.DueDate = DateTime.Now.AddDays(7);

                    }


                    LocalData.BeforeUpdateTodo = GlobalFunctions.CopyObject<TSTodo>(LocalData.CurrTodo);
                    LocalData.compAddOrUpdateTodo.Bootstrap();
                    LocalFunctions.DisplayModal(ModalForm.AddOrUpdateTodo);

                   
                }
                else
                {
                    LocalFunctions.AddMessage("Todo is not selected!", true, false);
                }
            }
            else
            {
                LocalFunctions.AddMessage("Todo is not selected!", true, false);

            }

        }


        public void CmdDelete()
        {
            if (LocalData.TsTodosList.Any())
            {
                if (!CurrID.Equals(Guid.Empty))
                {
                    LocalFunctions.DisplayConfirm("TodosPage", "Are you sure to delete todo?");
                }
                else
                {
                    LocalFunctions.AddMessage("Todo is not selected!", true, false);
                }
            }
            else
            {
                LocalFunctions.AddMessage("Todo is not selected!", true, false);

            }
        }


        public void CmdRowClick(MouseEventArgs e, Guid id)
        {
            CurrID = id;


           

            if (e.Detail > 1)
            {
                CmdUpdate();
            }
        }


        public string CmdGetStyle(bool b)
        {
            if (b)
            {
                return "background-color:rgba(0, 0, 0, 0.10)";
            }
            else
            {
                return string.Empty;
            }
        }


        
    }
}
