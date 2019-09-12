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
    public class FeedbackPageBase : ComponentBase
    {
       

        public string FeedbackListCount { get; set; } = LocalData.uiTranslator.Translate("Feedback list") + " (0)";

        public string AddOrUpdateButton { get; set; } = "Add feedback";

        public static List<TSFeedbackEx> FeedbackList { get; set; } = new List<TSFeedbackEx>();




        protected async override void OnInitialized()
        {

            if (LocalData.componentBridge != null)
            {
                LocalData.componentBridge.OnRefresh = CmdRefresh;
            }


            await CmdGetAllFeedback();


            base.OnInitialized();
        }

        private async void CmdRefresh()
        {
            if (LocalData.EventConsumerName.Equals("FeedbackPage"))
            {
                await CmdGetAllFeedback();
            }
        }

       

        public async Task CmdGetAllFeedback()
        {
            try
            {
                FeedbackList = await WebApiFunctions.CmdGetAllFeedback();

                FeedbackListCount = LocalData.uiTranslator.Translate("Feedback list") + " (" + FeedbackList.Count + ")";

                if (LocalData.currFeedback.UserID.Equals(Guid.Empty))
                {

                    AddOrUpdateButton = LocalData.uiTranslator.Translate("Add feedback");
                }
                else
                {
                    AddOrUpdateButton = LocalData.uiTranslator.Translate("Update feedback");
                }

                StateHasChanged();
            }
            catch (Exception ex)
            {
                LocalFunctions.AddError(ex.Message, MethodBase.GetCurrentMethod(), true, false);

            }
        }




        public void CmdAddOrUpdateFeedback()
        {
          

            if (LocalData.currFeedback.UserID.Equals(Guid.Empty))
            {
                LocalData.AddOrUpdateMode = true;
                LocalData.compAddOrUpdateFeedback.Bootstrap();
                LocalFunctions.DisplayModal(ModalForm.AddOrUpdateFeedback);
            }
            else
            {
                LocalData.AddOrUpdateMode = false;
                LocalData.compAddOrUpdateFeedback.Bootstrap();
                LocalFunctions.DisplayModal(ModalForm.AddOrUpdateFeedback);

            }
        }

        public void CmdLike()
        {
            CmdLikeorDislike(true);
        }

        public void CmdDislike()
        {
            CmdLikeorDislike(false);
        }

        private async void CmdLikeorDislike(bool b)
        {
            if (LocalData.IsAuthenticated)
            {
                if (!LocalData.CurrTSUser.UserName.Equals("demouser"))
                {

                    if (LocalData.currReaction.UserID.Equals(Guid.Empty))
                    {
                        await WebApiFunctions.CmdAddOrUpdateReaction(new TSReaction() {
                            LikeOrDislike = b,
                         ID = LocalData.currReaction.ID,
                         UserID = LocalData.currReaction.UserID});

                        await LocalFunctions.GetStat();

                        LocalFunctions.AddMessage("Thank you for your feedback", true, false);
                    }
                    else
                    {
                        if (LocalData.currReaction.LikeOrDislike != b)
                        {
                            await WebApiFunctions.CmdAddOrUpdateReaction(new TSReaction()
                            {
                                LikeOrDislike = b,
                                ID = LocalData.currReaction.ID,
                                UserID = LocalData.currReaction.UserID
                            });

                            await LocalFunctions.GetStat();


                            LocalFunctions.AddMessage("Thank you for your feedback", true, false);
                        }
                        else
                        {
                            if (LocalData.currReaction.LikeOrDislike)
                            {
                                LocalFunctions.AddMessage("You liked already", true, false);
                            }
                            else
                            {
                                LocalFunctions.AddMessage("You disliked already", true, false);
                            }
                        }
                    }
                    
                }
                else
                {
                    LocalFunctions.AddMessage("Demo user can't vote", true, false);
                }
            }
            else
            {
                LocalFunctions.AddMessage("Only autenticated user can vote", true, false);
            }


            StateHasChanged();
        }

    }
}
