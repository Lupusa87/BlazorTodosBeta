using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TodosShared;
using static TodosShared.TSEnums;

namespace BlazorTodos.Modals
{
    public partial class CompChangePassword
    {

        public string NewPassword { get; set; }

        public string ConfirmPassword { get; set; }

        public string EmailedCode { get; set; }
        

        protected bool IsButtonDisabled { get; set; } = true;

        protected bool IsSendMailVisible { get; set; } = true;
        protected bool IsEmailedCodeDisabled { get; set; } = true;


        public void Validate()
        {


            if (!string.IsNullOrEmpty(NewPassword))
            {
                NewPassword = NewPassword.Trim();
            }

            if (string.IsNullOrEmpty(NewPassword))
            {
                LocalFunctions.AddError("Please enter new password", MethodBase.GetCurrentMethod(), false, false);
            }
            else
            {

                if (NewPassword.Length < 6)
                {

                    LocalFunctions.AddError("Password min lenght should be 6", MethodBase.GetCurrentMethod(), false, false);
                }

                if (NewPassword.Length > 30)
                {

                    LocalFunctions.AddError("Password max lenght is 30", MethodBase.GetCurrentMethod(), false, false);
                }
            }


            if (!string.IsNullOrEmpty(ConfirmPassword))
            {
                ConfirmPassword = ConfirmPassword.Trim();
            }

            if (string.IsNullOrEmpty(ConfirmPassword))
            {
                LocalFunctions.AddError("Please enter confirm password", MethodBase.GetCurrentMethod(), false, false);
            }
            else if (ConfirmPassword != NewPassword)
            {
                LocalFunctions.AddError("Confirm password is not same", MethodBase.GetCurrentMethod(), false, false);
            }


            if (!string.IsNullOrEmpty(EmailedCode))
            {
                EmailedCode = EmailedCode.Trim();
            }

            if (string.IsNullOrEmpty(EmailedCode))
            {
                LocalFunctions.AddError("Please enter emailed code", MethodBase.GetCurrentMethod(), false, false);
            }
            else
            {

                if (EmailedCode.Length != 10)
                {

                    LocalFunctions.AddError("Emailed code lenght should be 10", MethodBase.GetCurrentMethod(), false, false);
                }

              
            }
           

    }


        public async void cmdKeyUp(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                await CmdChangePassword();
            }

        }

        public async Task CmdChangePassword()
        {

            IsButtonDisabled = true;
            StateHasChanged();

            Validate();


            if (LocalFunctions.HasError())
            {
                LocalFunctions.DisplayErrors();
            }
            else
            {

                TSUser tsUser = new TSUser()
                {
                    Email = LocalData.CurrTSUser.Email,
                    Password = NewPassword,
                    EmailedCode = EmailedCode,
                };




               await WebApi.CmdUserChangePassword(tsUser);
            }


            IsButtonDisabled = false;

        }

        public async Task CmdSendMail()
        {
            IsSendMailVisible = false;
            StateHasChanged();


            await WebApi.CmdSendMail(LocalData.CurrTSUser.Email.Trim(), EmailOperationsEnum.PasswordChange);

            IsSendMailVisible = false;
            IsEmailedCodeDisabled = false;
            IsButtonDisabled = false;


            StateHasChanged();
        }

    }
}
