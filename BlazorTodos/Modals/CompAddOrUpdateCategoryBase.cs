using BlazorTodos;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TodosShared;

namespace BlazorTodos.Modals
{

    public class CompAddOrUpdateCategoryBase : ComponentBase
    {

        [Parameter] protected string UniqueID { get; set; }

        protected bool IsButtonDisabled { get; set; } = false;

        [Parameter] protected string ButtonName { get; set; } = "Add category";


        protected override void OnInitialized()
        {

            Bootstrap();

            base.OnInitialized();
        }

        public void Bootstrap()
        {
            if (LocalData.AddOrUpdateMode)
            {
                ButtonName = "Add category";
            }
            else
            {
                ButtonName = "Update category";
            }

            LocalData.currCategory.TodosCount = 0;

        }



        public async Task CmdAddOrUpdateCategory()
        {

            IsButtonDisabled = true;
            StateHasChanged();


            if (LocalFunctions.Validate(LocalData.currCategory))
            {

                if (!LocalData.currCategoryName.Equals(LocalData.currCategory.Name, StringComparison.InvariantCultureIgnoreCase))
                {

                    string a = string.Empty;

                    if (LocalData.AddOrUpdateMode)
                    {
                        LocalData.currCategory.UserID = LocalData.CurrTSUser.ID;
                        a = await WebApiFunctions.CmdAddCategory(LocalData.currCategory);
                    }
                    else
                    {
                        a = await WebApiFunctions.CmdUpdateCategory(LocalData.currCategory);
                    }

                    if (a.Equals("OK"))
                    {
                        LocalData.TSCategoriesList = new List<TSCategoryEx>();
                        LocalData.TSCategoriesDictionary = new Dictionary<Guid, string>();


                        LocalData.TsTodosList = new List<TSTodoEx>();


                        LocalData.btModal.Close();

                        //LocalFunctions.CmdNavigate("CategoriesPage");
                        LocalData.EventConsumerName = "CategoriesPage";
                        LocalData.componentBridge.InvokeRefresh();

                    }
                    else
                    {
                        LocalFunctions.AddError(a, MethodBase.GetCurrentMethod(), true, false);

                    }
                }
                else
                {
                    LocalFunctions.AddMessage("Category name not updated", true, false);
                }

            }

            IsButtonDisabled = false;

        }


        public void Refresh()
        {
            StateHasChanged();
            // Task.Delay(1);
        }
    }
}
