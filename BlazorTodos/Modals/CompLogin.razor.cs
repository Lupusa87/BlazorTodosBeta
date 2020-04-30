using BlazorWindowHelper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using static BlazorTodos.Classes.Enums;
using static TodosShared.TSEnums;

namespace BlazorTodos.Modals
{

    public partial class CompLogin
    {
        public Guid FirstInputID = Guid.NewGuid();

        public bool ShouldSetFocus = false;
        [Parameter] public string UniqueID { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }


        protected bool IsButtonDisabled { get; set; } = false;

        protected bool IsRecoveryIconVisible { get; set; } = true;

        public void EmptyValues()
        {
            UserName = string.Empty;
            UserPassword = string.Empty;
        }

        protected override void OnInitialized()
        {
            if (LocalData.componentBridge != null)
            {
                LocalData.componentBridge.OnConfirm = CmdActualRecover;
            }

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

        protected void ValidateUserName()
        {
            if (string.IsNullOrEmpty(UserName.Trim()))
            {
                LocalFunctions.AddError("Please enter user name", MethodBase.GetCurrentMethod(), false, false);
            }
            else if (UserName.Trim().Length < 6)
            {
                LocalFunctions.AddError("User name should contain minimum 6 character", MethodBase.GetCurrentMethod(), false, false);
            }
        }

        protected void ValidateFull()
        {
            if (string.IsNullOrEmpty(UserName.Trim()))
            {
               LocalFunctions.AddError("Please enter user name", MethodBase.GetCurrentMethod(), false, false);
            }
            else if (UserName.Trim().Length < 3)
            {
                LocalFunctions.AddError("User name should contain minimum 6 characters", MethodBase.GetCurrentMethod(), false, false);
            }
            else if (UserName.Trim().Length > 100)
            {
                LocalFunctions.AddError("User name can contain maximum 100 characters", MethodBase.GetCurrentMethod(), false, false);
            }

            if (string.IsNullOrEmpty(UserPassword.Trim()))
            {
                LocalFunctions.AddError("Please enter password", MethodBase.GetCurrentMethod(), false, false);
            }
            else if (UserPassword.Trim().Length < 6)
            {
                LocalFunctions.AddError("Password should contain minimum 6 characters", MethodBase.GetCurrentMethod(), false, false);
            }
            else if (UserPassword.Trim().Length > 30)
            {
                LocalFunctions.AddError("Password should contain maximum 30 characters", MethodBase.GetCurrentMethod(), false, false);
            }

        }


        protected void CmdRegister()
        {
            LocalData.btModal.Close();


            if (!LocalData.IsAuthenticated)
            {
                LocalFunctions.DisplayModal(ModalForm.Registration);
            }
            else
            {
                LocalFunctions.AddMessage("Please logout first, to register new user", true, false);
            }
        }


        protected void CmdAuthorize()
        {

            IsButtonDisabled = true;
            IsRecoveryIconVisible = false;
            StateHasChanged();


            ValidateFull();

            if (LocalFunctions.HasError())
            {
                LocalFunctions.DisplayErrors();
            }
            else
            {
                LocalFunctions.Authorize(UserName.Trim(), UserPassword.Trim());

            }

            IsButtonDisabled = false;
            IsRecoveryIconVisible = true;
            StateHasChanged();
        }



        public void CmdRecoverPass()
        {
            ValidateUserName();

            if (LocalFunctions.HasError())
            {
                LocalFunctions.DisplayErrors();
            }
            else
            {
                LocalFunctions.DisplayConfirm("LoginDialog", "Are you sure to recover password?");
            }
        }


        private async void CmdActualRecover()
        {
            if (LocalData.EventConsumerName.Equals("LoginDialog"))
            {
                IsButtonDisabled = true;
                IsRecoveryIconVisible = false;
                StateHasChanged();

                await WebApi.CmdRecoverPass(UserName.Trim(), EmailOperationsEnum.PasswordRecovery);

                IsButtonDisabled = false;
                StateHasChanged();
            }

        }


        public void cmdKeyUp(KeyboardEventArgs e)
        {
            if (e.Key=="Enter")
            {
                CmdAuthorize();
            }
   
        }
    }
}
