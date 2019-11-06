using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
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
    public class CategoriesPageBase : ComponentBase
    {
       

        public string CategoriesListCount { get; set; } = LocalData.uiTranslator.Translate("Categories list") +" (0)";

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
                await CmdGetAllCategories();

            }
            else
            {
                LocalFunctions.CmdNavigate();

                LocalFunctions.AddMessage("Please login", true, false);
            }


            base.OnInitialized();
        }

        private async void CmdRefresh()
        {
            if (LocalData.EventConsumerName.Equals("CategoriesPage"))
            {
                if (LocalData.IsAuthenticated)
                {
                    await CmdGetAllCategories();
                }
                else
                {

                    StateHasChanged();
                }

            }
        }

        private async void CmdActualDelete()
        {
            if (LocalData.EventConsumerName.Equals("CategoriesPage"))
            {
                if (LocalData.TSCategoriesList.Any())
                {
                    string a = await WebApiFunctions.CmdDeleteCategory(LocalData.TSCategoriesList.Single(x => x.ID == CurrID));

                    if (a.Equals("OK"))
                    {
                        LocalData.TSCategoriesList = new List<TSCategoryEx>();
                        LocalData.TSCategoriesDictionary = new Dictionary<Guid, string>();

                        LocalData.btModal.Close();

                        LocalFunctions.CmdNavigate("CategoriesPage");
                        LocalData.componentBridge.InvokeRefresh();

                    }
                    else
                    {
                        LocalFunctions.AddError(a, MethodBase.GetCurrentMethod(), true, false);

                    }
                }
            }

        }


        public async Task CmdGetAllCategories()
        {
            try
            {

                await LocalFunctions.GetTodosAndCategories();

                CategoriesListCount = LocalData.uiTranslator.Translate("Categories list") + " (" + LocalData.TSCategoriesList.Count + ")";


                StateHasChanged();


            }
            catch (Exception ex)
            {
                LocalFunctions.AddError(ex.Message, MethodBase.GetCurrentMethod(), true, false);

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
            LocalData.currCategory = new TSCategory();
            LocalData.currCategoryName = string.Empty;
            LocalData.compAddOrUpdateCategory.Bootstrap();
            LocalFunctions.DisplayModal(ModalForm.AddOrUpdateCategory);
        }


        public void CmdUpdate()
        {

            if (LocalData.TSCategoriesList.Any())
            {
                if (!CurrID.Equals(Guid.Empty))
                {

                    if (!LocalData.TSCategoriesList.Single(x => x.ID == CurrID).Name.Equals("default"))
                    {
                        LocalData.AddOrUpdateMode = false;
                        LocalData.currCategory = LocalData.TSCategoriesList.Single(x => x.ID == CurrID);
                        LocalData.currCategoryName = LocalData.currCategory.Name;
                        LocalData.compAddOrUpdateCategory.Bootstrap();
                        LocalFunctions.DisplayModal(ModalForm.AddOrUpdateCategory);
                    }
                    else
                    {
                        LocalFunctions.AddMessage("Default category can't be updated!", true, false);
                    }

                }
                else
                {
                    LocalFunctions.AddMessage("Category is not selected!", true, false);

                }
            }
            else
            {
                LocalFunctions.AddMessage("Category is not selected!", true, false);

            }

        }


        public void CmdDelete()
        {
            if (LocalData.TSCategoriesList.Any())
            {

                if (!CurrID.Equals(Guid.Empty))
                {
                    if (!LocalData.TSCategoriesList.Single(x => x.ID == CurrID).Name.Equals("default"))
                    {
                        if (LocalData.TSCategoriesList.Single(x => x.ID == CurrID).TodosCount == 0)
                        {
                            LocalFunctions.DisplayConfirm("CategoriesPage", "Are you sure to delete category?");
                        }
                        else
                        {
                            LocalFunctions.AddMessage("Category has todos!", true, false);
                        }
                    }
                    else
                    {
                        LocalFunctions.AddMessage("Default category can't be deleted!", true, false);
                    }
                }
                else
                {
                    LocalFunctions.AddMessage("Category is not selected!", true, false);

                }
            }
            else
            {
                LocalFunctions.AddMessage("Category is not selected!", true, false);

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

