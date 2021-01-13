using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Threading.Tasks;
using TodosShared;
using static BlazorTodos.Classes.Enums;
using static TodosShared.TSEnums;

namespace BlazorTodos
{
    public static class WebApi
    {


        public static async Task<bool> Cmd_Get_PublicData()
        {
           

            LocalData.IsDownloadedSetupData = false;
            string a = await WebApiFunctions.Cmd_Get_Setup_Data();
            if (a.Equals("OK"))
            {
                return true;
            }
            else
            {
                LocalFunctions.AddError(a, MethodBase.GetCurrentMethod(), true, false);
                return false;
            }

        }

        public static async Task<bool> CmdGetJWT(SecureString ParUserName, SecureString ParPassword, WebApiUserTypesEnum ParWebApiUserTypesEnum = WebApiUserTypesEnum.NotAuthorized)
        {

            if (!LocalData.IsDownloadedSetupData)
            {
                LocalFunctions.AddError("Bootstrap data is not Downloaded!", MethodBase.GetCurrentMethod(), true, false);
                return false;
            }

            LocalData.CurrJWT = string.Empty;
            try
            {

              
                JwtResult tmp_jwt = await WebApiFunctions.CmdDownloadToken(ParUserName, ParPassword, ParWebApiUserTypesEnum);






                if (string.IsNullOrEmpty(tmp_jwt.ErrorMessage))
                {
                    LocalData.CurrJWT = tmp_jwt.AccessToken;
                }
                else
                {
                   
                    LocalFunctions.AddError(tmp_jwt.ErrorMessage, MethodBase.GetCurrentMethod(), true, false);
                   
                }

            }
            catch (Exception ex)
            {
                LocalFunctions.AddError(ex.Message, MethodBase.GetCurrentMethod(), true, false);
            }


            return (!string.IsNullOrEmpty(LocalData.CurrJWT));
        }

        public static async Task<bool> CmdRegisterUser(TSUser ParTSUser)
        {

            string a = await WebApiFunctions.CmdTSUserRegister(ParTSUser);

            if (a.Equals("OK"))
            {

                LocalFunctions.Authorize(ParTSUser.UserName, ParTSUser.Password);

                return true;
            }
            else
            {
                LocalFunctions.AddError(a, MethodBase.GetCurrentMethod(), true, false);
                return false;
            }

            
        }


        public static async Task<bool> CmdUserChangePassword(TSUser ParTSUser)
        {

            string a = await WebApiFunctions.CmdTSUserChangePassword(ParTSUser);

            if (a.Equals("OK"))
            {
                LocalFunctions.AddMessage("Password was changed successfully, please login again", true, false);
                LocalData.btModal.Close();
                LocalFunctions.Logout();

                return true;
            }
            else
            {
                LocalFunctions.AddError(a, MethodBase.GetCurrentMethod(), true, false);
                return false;
            }


        }


        public static async Task<bool> CmdSendMail(string ParTo, EmailOperationsEnum ParEmailOperationsEnum)
        {

            TSEmail ParTSEmail = new TSEmail { To = ParTo, OperationCode = (int)ParEmailOperationsEnum };


            TSEmail tsEmail = await WebApiFunctions.CmdSendEmail(ParTSEmail);



            if (tsEmail.Result.Equals("OK"))
            {
                LocalFunctions.AddMessage("Email was sent successfully, please check inbox, code is valid for 2 minutes", true, false);
                return true;
            }
            else
            {
                LocalFunctions.AddError(tsEmail.Result, MethodBase.GetCurrentMethod(), true, false);
                return false;
            }



        }


        public static async Task<bool> CmdCheckEmailNotExists(string ParEmail)
        {

            TSEmail ParTSEmail = new TSEmail { To = ParEmail, OperationCode = 0};

            TSEmail tsEmail = await WebApiFunctions.CmdCheckEmailNotExists(ParTSEmail);

            if (tsEmail.Result.Equals("OK"))
            {
                return true;
            }
            else
            {
                LocalFunctions.AddError(tsEmail.Result, MethodBase.GetCurrentMethod(), true, false);
                return false;
            }
        }


        public static async Task<bool> CmdCheckUserNameNotExists(string ParUserName)
        {

            TSEmail ParTSEmail = new TSEmail { To = ParUserName, OperationCode = 0 };

            TSEmail tsEmail = await WebApiFunctions.CmdCheckUserNameNotExists(ParTSEmail);

            if (tsEmail.Result.Equals("OK"))
            {
                return true;
            }
            else
            {
                LocalFunctions.AddError(tsEmail.Result, MethodBase.GetCurrentMethod(), true, false);
                return false;
            }
        }



        public static async Task<bool> CmdRecoverPass(string ParUserName, EmailOperationsEnum ParEmailOperationsEnum)
        {

            TSEmail ParTSEmail = new TSEmail { To = ParUserName, OperationCode = (int)ParEmailOperationsEnum };


            TSEmail tsEmail = await WebApiFunctions.CmdRecoverPass(ParTSEmail);



            if (tsEmail.Result.Equals("OK"))
            {
                LocalFunctions.AddMessage("Password recovered successfully, new password was sent to your email", true, false);
                return true;
            }
            else
            {
                LocalFunctions.AddError(tsEmail.Result, MethodBase.GetCurrentMethod(), true, false);
                return false;
            }



        }
    }
}
