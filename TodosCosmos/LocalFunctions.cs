using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TodosCosmos.Diagnostics;
using TodosCosmos.DocumentClasses;
using TodosGlobal;
using TodosShared;
using static TodosShared.TSEnums;

namespace TodosCosmos
{
    public static class LocalFunctions
    {

        public static async Task NotifyAdmin(string ActivityDescription,
                                             List<string> CallTrace,
                                             string Subject = "Blazor Todo new Activity",
                                             string pSenderName = "Blazor Todos")
        {
            if (GlobalData.WebOrLocalMode)
            {
                await CmdSendEmailAsync(GlobalData.AdminNotifyEmail.Trim(), Subject, ActivityDescription, AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()), pSenderName);
            }
            else
            {
                await Task.FromResult(true);

            }
        }


        public static async Task<TSEmail> SendEmail(TSEmail ParTSEmail, string ParIPAddress, string ParMachineID, List<string> CallTrace)
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
                    if (CmdSendEmailAsync(ParTSEmail.To.Trim(), "Registration", "Hello,\n\nYour code is " + tmp_Code + "\nThank you for registration.\n\nBest Regards,\nSite Administration", AddThisCaller(CallTrace, MethodBase.GetCurrentMethod())).Result)
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
                    if (CmdSendEmailAsync(ParTSEmail.To.Trim(), "Email change", "Hello,\n\nYour code is " + tmp_Code + "\n\nBest Regards,\nSite Administration", AddThisCaller(CallTrace, MethodBase.GetCurrentMethod())).Result)
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
                    if (CmdSendEmailAsync(ParTSEmail.To.Trim(), "Password change", "Hello,\n\nYour code is " + tmp_Code + "\n\nBest Regards,\nSite Administration", AddThisCaller(CallTrace, MethodBase.GetCurrentMethod())).Result)
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
                    if (CmdSendEmailAsync(ParTSEmail.To.Trim(), "Password Recovery", "Hello,\n\nYour new password is " + ParMachineID + "\n\nPlease change password after login.\n\nBest Regards,\nSite Administration", AddThisCaller(CallTrace, MethodBase.GetCurrentMethod())).Result)
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
                    if (CmdSendEmailAsync(ParTSEmail.To.Trim(), "Todo Reminder", "Hello,\n\nYour requested todo remind is here.\n\n" + ParMachineID + "\n\nBest Regards,\nSite Administration", AddThisCaller(CallTrace, MethodBase.GetCurrentMethod())).Result)
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
                };

                await CmdSaveEmailedCode(tsEmailedCode, AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
            }

            return ParTSEmail;
        }


        private static async Task<bool> CmdSendEmailAsync(string ParEmail,
                                  string ParSubject,
                                  string ParMessage,
                                  List<string> CallTrace,
                                  string pSenderName = "Blazor Todos")
        {


            bool result = true;

            try
            {
                //https://docs.microsoft.com/en-us/azure/sendgrid-dotnet-how-to-send-email
                //5/6/2020

                var client = new SendGridClient(GlobalData.SendGridApiKey);
                var msg = new SendGridMessage()
                {
                    From = new EmailAddress(GlobalData.GmailAccountName, pSenderName),
                    Subject = ParSubject,
                    PlainTextContent = ParMessage,
                    //HtmlContent = "<strong>Hello, Email!</strong>"
                };
                msg.AddTo(new EmailAddress(ParEmail));
                Response response = await client.SendEmailAsync(msg);

            }
            catch (Exception ex)
            {
                await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, ex.Message, AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

                return false;
            }


            return result;
        }

        private static async Task<bool> CmdSendEmailAsyncGmail(string ParEmail,
                                    string ParSubject,
                                    string ParMessage, 
                                    List<string> CallTrace,
                                    string pSenderName ="Blazor Todos")
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

                //MimeMessage message = new MimeMessage();
                //message.From.Add(new MailboxAddress(pSenderName, GlobalData.GmailAccountName));
                //message.To.Add(new MailboxAddress(ParEmail));
                //message.Subject = ParSubject;

                //message.Body = new TextPart("plain")
                //{
                //    Text = ParMessage
                //};

                //using SmtpClient client = new SmtpClient();

                //await client.ConnectAsync("smtp.gmail.com", 587, false);

                //client.AuthenticationMechanisms.Remove("XOAUTH2");

                //await client.AuthenticateAsync(GlobalData.GmailAccountName, GlobalData.GmailAccountPass);

                //await client.SendAsync(message);

                //await client.DisconnectAsync(true);

                ////client.Connect("smtp.gmail.com", 587, false);

                ////client.AuthenticationMechanisms.Remove("XOAUTH2");

                ////client.Authenticate(GlobalData.GmailAccountName, GlobalData.GmailAccountPass);

                ////client.Send(message);

                ////client.Disconnect(true);


            }
            catch (Exception ex)
            {
                await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, ex.Message, AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

                return false;
            }


            return result;
        }



     


        private static async Task<bool> CmdSaveEmailedCode(CosmosEmailedCode ParEmailedCode, List<string> CallTrace)
        {
            bool result = true;
            try
            {

                await CosmosAPI.cosmosDBClientEmailedCode.DeleteEmailedCodes(ParEmailedCode.Email, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

                result = await CosmosAPI.cosmosDBClientEmailedCode.AddEmailedCode(ParEmailedCode, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
            }
            catch (Exception ex)
            {
                await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, ex.Message, AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
                result = false;
            }

            return result;
        }


        public static string NotWorkingProperly_GetCallTrace()
        {
            //4/4/2020
            //Did not worked as expected, in azure functions trace is giving some weird method names.
            StackTrace st = new StackTrace();

            StringBuilder sb = new StringBuilder();
            sb.Append("trace:");
            for (int i = st.FrameCount - 1; i > 0; i--)
            {

                sb.Append(st.GetFrame(i).GetMethod().Name);

                if (i > 1)
                {
                    sb.Append("=>");
                }
            }

            return sb.ToString();
        }

        public static List<string> AddThisCaller(List<string> CallTrace, MethodBase ParMB)
        {
            string n = GetMethodName(ParMB);
            if (!CallTrace.Any(x=>x.Equals(n)))
            {
                CallTrace.Add(n);
            }

            return CallTrace;

        }

        public static string GetCallTraceString(List<string> CallTrace)
        {
            StringBuilder sb = new StringBuilder();


            foreach (var item in CallTrace)
            {
                sb.Append(item+"=>");
            }

            return sb.ToString();
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



        public static void ConsolePrint(string text, bool BeforeLine=false, bool AfterLine = false)
        {
            if (!GlobalData.IsDevelopmentMode) return;


            if (BeforeLine) Console.WriteLine();


            Console.WriteLine(text);

            if (AfterLine) Console.WriteLine();

        }


       
    }
}
