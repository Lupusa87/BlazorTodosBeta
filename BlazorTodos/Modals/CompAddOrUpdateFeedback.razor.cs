using Microsoft.AspNetCore.Components;
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

        [Parameter] public string UniqueID { get; set; }

        protected bool IsButtonDisabled { get; set; } = false;

        [Parameter] public string ButtonName { get; set; } = "Add feedback";

        protected override void OnInitialized()
        {

            Bootstrap();

            base.OnInitialized();
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
