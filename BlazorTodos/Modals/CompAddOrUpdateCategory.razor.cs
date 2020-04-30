using BlazorTodos;
using BlazorWindowHelper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TodosShared;

namespace BlazorTodos.Modals
{

    public partial class CompAddOrUpdateCategory
    {
        public Guid FirstInputID = Guid.NewGuid();

        public bool ShouldSetFocus = false;

        [Parameter] public string UniqueID { get; set; }

        protected bool IsButtonDisabled { get; set; } = false;

        [Parameter] public string ButtonName { get; set; } = "Add category";


        protected override void OnInitialized()
        {

            Bootstrap();

            base.OnInitialized();
        }

        protected override void OnAfterRender(bool firstRender)
        {

            if (firstRender)
            {
                BWHJsInterop.SetFocus(FirstInputID.ToString());
            }


            if (ShouldSetFocus)
            {
                BWHJsInterop.SetFocus(FirstInputID.ToString());
                ShouldSetFocus = false;
            }

            base.OnAfterRender(firstRender);
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

        public async void cmdKeyUp(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
               await CmdAddOrUpdateCategory();
            }

        }

        public async Task CmdAddOrUpdateCategory()
        {

            IsButtonDisabled = true;
            StateHasChanged();


            if (LocalFunctions.Validate(LocalData.currCategory))
            {

                if (!LocalData.currCategoryName.Equals(LocalData.currCategory.Name, StringComparison.InvariantCultureIgnoreCase))
                {

                    string a;

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
