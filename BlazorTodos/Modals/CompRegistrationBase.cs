using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TodosShared;
using static TodosShared.TSEnums;

namespace BlazorTodos.Modals
{
    public class CompRegistrationBase : ComponentBase
    {

        public TSUser tsUser { get; set; } = new TSUser();

        public string ConfirmPassword { get; set; }

        protected bool IsButtonDisabled { get; set; } = true;

        protected bool IsSendMailVisible { get; set; } = true;
        protected bool IsEmailedCodeDisabled { get; set; } = true;


        public void Validate()
        {
            if (!string.IsNullOrEmpty(ConfirmPassword))
            {
                ConfirmPassword = ConfirmPassword.Trim();
            }

            if (string.IsNullOrEmpty(ConfirmPassword))
            {
               LocalFunctions.AddError("Please enter confirm password", MethodBase.GetCurrentMethod(), false, false);
            }
            else if (ConfirmPassword != tsUser.Password)
            {
                LocalFunctions.AddError("Confirm password is not same", MethodBase.GetCurrentMethod(), false, false);
            }

        }

        public async Task CmdAddUser()
        {

            IsButtonDisabled = true;
            StateHasChanged();


            LocalFunctions.Validate(tsUser, false);

            Validate();


            if (LocalFunctions.HasError())
            {
                LocalFunctions.DisplayErrors();
            }
            else
            {
                if (await WebApi.CmdCheckUserNameNotExists(tsUser.UserName.Trim()))
                {

                   await WebApi.CmdRegisterUser(tsUser);
                }
                   
            }


            IsButtonDisabled = false;

        }

        public async Task CmdSendMail()
        {


            IsSendMailVisible = false;
            StateHasChanged();

            if (!string.IsNullOrEmpty(tsUser.Email))
            {
                tsUser.Email = tsUser.Email.Trim();
            }

            if (string.IsNullOrEmpty(tsUser.Email.Trim()))
            {
               LocalFunctions.AddError("Please enter email", MethodBase.GetCurrentMethod(), true, false);
            }
            else
            {
                if (LocalFunctions.CheckEmailFormat(tsUser.Email.Trim()))
                {
                    if (await WebApi.CmdCheckEmailNotExists(tsUser.Email.Trim()))
                    {
                        await WebApi.CmdSendMail(tsUser.Email.Trim(), EmailOperationsEnum.Registration);

                        IsSendMailVisible = false;
                        IsEmailedCodeDisabled = false;
                        IsButtonDisabled = false;
                    }
                    else
                    {
                        IsSendMailVisible = true;
                        IsEmailedCodeDisabled = true;
                        IsButtonDisabled = false;
                    }
                }
                else
                {
                    LocalFunctions.AddError("Email format is not correct", MethodBase.GetCurrentMethod(), true, false);
                }

            }


            StateHasChanged();
        }
        
    }
}
