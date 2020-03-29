using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TodosCosmos.DocumentClasses;
using TodosGlobal;
using TodosShared;
using static TodosShared.TSEnums;

namespace TodosCosmos
{
    public static class LocalFunctions
    {



        public static async Task NotifyAdmin(string ActivityDescription)
        {
            if (GlobalData.WebOrLocalMode)
            {
                await CmdSendEmail(GlobalData.AdminNotifyEmail.Trim(), "Blazor Todo new Activity", ActivityDescription);
            }
            else
            {
                await Task.FromResult(true);

            }
        }


        public static async Task<TSEmail> SendEmail(TSEmail ParTSEmail, string ParIPAddress, string ParMachineID)
        {



            var attr = new EmailAddressAttribute();

            if (!attr.IsValid(ParTSEmail.To))
            {
                ParTSEmail.Result = "Error:Email format is not valid!";
                return ParTSEmail;
            }


            if (ParTSEmail.To.ToLower().Equals("demouser@ggg.ge"))
            {
                ParTSEmail.Result = "Error:Can't sent any email to demo user!";
                return ParTSEmail;
            }

            bool MustSaveEmailedCode = false;

            EmailOperationsEnum tmp_Operation = (EmailOperationsEnum)ParTSEmail.OperationCode;
            string tmp_Code = string.Empty;
            switch (tmp_Operation)
            {
                case EmailOperationsEnum.Registration:

                    tmp_Code = GlobalFunctions.GetSalt();
                    if (CmdSendEmail(ParTSEmail.To.Trim(), "Registration", "Hello,\n\nYour code is " + tmp_Code + "\nThank you for registration.\n\nBest Regards,\nSite Administration").Result)
                    {
                        ParTSEmail.Result = "OK";
                        MustSaveEmailedCode = true;
                    }
                    else
                    {
                        ParTSEmail.Result = "Error";
                        MustSaveEmailedCode = false;
                    }
                    break;
                case EmailOperationsEnum.EmailChange:
                    tmp_Code = GlobalFunctions.GetSalt();
                    if (CmdSendEmail(ParTSEmail.To.Trim(), "Email change", "Hello,\n\nYour code is " + tmp_Code + "\n\nBest Regards,\nSite Administration").Result)
                    {
                        ParTSEmail.Result = "OK";
                        MustSaveEmailedCode = true;
                    }
                    else
                    {
                        ParTSEmail.Result = "Error";
                        MustSaveEmailedCode = false;
                    }
                    break;
                case EmailOperationsEnum.PasswordChange:
                    tmp_Code = GlobalFunctions.GetSalt();
                    if (CmdSendEmail(ParTSEmail.To.Trim(), "Password change", "Hello,\n\nYour code is " + tmp_Code + "\n\nBest Regards,\nSite Administration").Result)
                    {
                        ParTSEmail.Result = "OK";
                        MustSaveEmailedCode = true;
                    }
                    else
                    {
                        ParTSEmail.Result = "Error";
                        MustSaveEmailedCode = false;
                    }
                    break;
                case EmailOperationsEnum.PasswordRecovery:
                    MustSaveEmailedCode = false;
                    if (CmdSendEmail(ParTSEmail.To.Trim(), "Password Recovery", "Hello,\n\nYour new password is " + ParMachineID + "\n\nPlease change password after login.\n\nBest Regards,\nSite Administration").Result)
                    {
                        ParTSEmail.Result = "OK";
                    }
                    else
                    {
                        ParTSEmail.Result = "Error";
                    }
                    break;
                case EmailOperationsEnum.TodoReminder:
                    MustSaveEmailedCode = false;
                    if (CmdSendEmail(ParTSEmail.To.Trim(), "Todo Reminder", "Hello,\n\nYour requested todo remind is here.\n\n" + ParMachineID + "\n\nBest Regards,\nSite Administration").Result)
                    {
                        ParTSEmail.Result = "OK";
                    }
                    else
                    {
                        ParTSEmail.Result = "Error";
                    }
                    break;
                default:
                    break;
            }

            if (MustSaveEmailedCode)
            {
                CosmosEmailedCode tsEmailedCode = new CosmosEmailedCode
                {
                    Email = ParTSEmail.To.Trim(),
                    Code = tmp_Code,
                    IPAddress = ParIPAddress,
                    OperationType = ParTSEmail.OperationCode,
                    MachineID = ParMachineID,
                    AddDate = DateTime.UtcNow,
                };

                await CmdSaveEmailedCode(tsEmailedCode);
            }

            return ParTSEmail;
        }

        private static async Task<bool> CmdSendEmail(string ParEmail, string ParSubject, string ParMessage)
        {



            //http://dotnetthoughts.net/how-to-send-emails-from-aspnet-core/
            //http://www.mimekit.net/#
            //https://github.com/jstedfast/MailKit

            //to enable gmail
            //https://www.google.com/settings/security/lesssecureapps
            //https://accounts.google.com/DisplayUnlockCaptcha
            //https://accounts.google.com/b/0/DisplayUnlockCaptcha

            bool result = true;

            try
            {
                MimeMessage message = new MimeMessage();
                message.From.Add(new MailboxAddress("Blazor Todos", GlobalData.GmailAccountName));
                message.To.Add(new MailboxAddress(ParEmail));
                message.Subject = ParSubject;

                message.Body = new TextPart("plain")
                {
                    Text = ParMessage
                };

                using (SmtpClient client = new SmtpClient())
                {
                    client.Connect("smtp.gmail.com", 587, false);

                    client.AuthenticationMechanisms.Remove("XOAUTH2");

                    client.Authenticate(GlobalData.GmailAccountName, GlobalData.GmailAccountPass);

                    client.Send(message);
                    client.Disconnect(true);
                }

            }
            catch (Exception ex)
            {
                await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, ex.Message, MethodBase.GetCurrentMethod());

                return false;
            }


            return result;
        }

        private static async Task<bool> CmdSaveEmailedCode(CosmosEmailedCode ParEmailedCode)
        {
            bool result = true;
            try
            {
                await CosmosAPI.cosmosDBClientEmailedCode.DeleteExpiredEmaiedCodes();

                await CosmosAPI.cosmosDBClientEmailedCode.DeleteEmailedCodes(ParEmailedCode.Email);

                result = await CosmosAPI.cosmosDBClientEmailedCode.AddEmailedCode(ParEmailedCode);
            }
            catch (Exception ex)
            {
                await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, ex.Message, MethodBase.GetCurrentMethod());
                result = false;
            }

            return result;
        }



        public static string GetMethodName(MethodBase MB)
        {

            string a = MB.DeclaringType.FullName.ToLower().Replace("TodosFunctionsApi.", null);
            a = a.Replace("api.", null);
            a = a.Replace("timers.", null);
            a = a.Replace("+<", ".");

            int k = a.IndexOf(">");

            if (k > -1)
            {
                a = a.Substring(0, k);
            }
            a = a.Replace("funtimer.", null);
            return a;
        }
    }
}
