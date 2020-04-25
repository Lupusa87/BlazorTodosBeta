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


    public partial class CompAddOrUpdateFeedback
    {
        public Guid FirstInputID = Guid.NewGuid();
        public bool ShouldSetFocus = false;
        [Parameter] public string UniqueID { get; set; }

        protected bool IsButtonDisabled { get; set; } = false;

        [Parameter] public string ButtonName { get; set; } = "Add feedback";

        protected override void OnInitialized()
        {

            Bootstrap();

            base.OnInitialized();
        }

        protected override void OnAfterRender(bool firstRender)
        {

            if (firstRender)
            {
                BTodosJsInterop.SetFocus(FirstInputID.ToString());
            }
            if (ShouldSetFocus)
            {
                BTodosJsInterop.SetFocus(FirstInputID.ToString());
                ShouldSetFocus = false;
            }
            base.OnAfterRender(firstRender);
        }



        public void Bootstrap()
        {
            if (LocalData.AddOrUpdateMode)
            {
                ButtonName = "Add feedback";
            }
            else
            {
                ButtonName = "Update feedback";
            }

        }

        public async void cmdKeyUp(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                await CmdAddOrUpdateFeedback();
            }

        }

        public async Task CmdAddOrUpdateFeedback()
        {

            IsButtonDisabled = true;
            StateHasChanged();


            if (LocalFunctions.Validate(LocalData.currFeedback))
            {


                if (LocalData.oldFeedbackText != LocalData.currFeedback.Text.ToLower())
                {
                    await WebApiFunctions.CmdAddOrUpdateFeedback(LocalData.currFeedback);


                    if (!LocalData.currFeedback.UserID.Equals(Guid.Empty))
                    {

                        LocalData.btModal.Close();

                        //LocalFunctions.CmdNavigate("FeedbackPage");
                        LocalData.EventConsumerName = "FeedbackPage";
                        LocalData.componentBridge.InvokeRefresh();


                        LocalFunctions.AddMessage("Thank you for your feedback", true, false);
                    }
                    else
                    {
                        LocalFunctions.AddError("Error adding or updating feedback", MethodBase.GetCurrentMethod(), true, false);

                    }
                }
                else
                {
                    LocalFunctions.AddMessage("Feedback text not updated", true, false);
                }

            }

            IsButtonDisabled = false;

        }


        public void Refresh()
        {
            StateHasChanged();
        }
    }
}
