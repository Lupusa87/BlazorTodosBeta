using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using static BlazorTodos.Classes.Enums;
using Microsoft.JSInterop;
using TodosShared;
using System.Security;
using static TodosShared.TSEnums;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Http.Json;

namespace BlazorTodos
{
    public static class WebApiFunctions
    {
        //https://www.asp.net/web-api/overview/advanced/calling-a-web-api-from-a-net-client
        //Install-Package Microsoft.AspNet.WebApi.Client


        public static HttpClient httpClient { get; set; } = null;

        internal static async Task<string> CmdDownloadSetupData()
        {

            try
            {
                httpClient.DefaultRequestHeaders.Add("x-functions-key", "eE45GSQAGSOEDDJCeQPDqAsNbry5Zw/rzQ8woBMxFRf/Q5PrNiPokg==");
                string a = await httpClient.GetStringAsync("setupdata");
                httpClient.DefaultRequestHeaders.Remove("x-functions-key");
                return a;
            }
            catch (Exception ex)
            {
                LocalFunctions.AddError(ex.Message, MethodBase.GetCurrentMethod(), true, false);
                return "Error";
            }



        }


        internal static async Task<JwtResult> CmdDownloadToken(SecureString ParUserName, SecureString ParPassword, WebApiUserTypesEnum ParWebApiUserTypesEnum = WebApiUserTypesEnum.NotAuthorized)
        {
            try
            {

                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

                string username = GlobalFunctions.ConvertToPlainString(ParUserName);

                if (string.IsNullOrEmpty(username))
                {

                    return new JwtResult { ErrorMessage = "Username is not provided!" };
                }
               

                string password = GlobalFunctions.ConvertToPlainString(ParPassword);
                if (string.IsNullOrEmpty(password))
                {
                    return new JwtResult { ErrorMessage = "Password is not provided!" };
                }


                var formContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("UserName", username + GlobalFunctions.GetRandomAlphaNumeric()),
                    new KeyValuePair<string, string>("UserPass", password + GlobalFunctions.GetRandomAlphaNumeric()),
                    new KeyValuePair<string, string>("UserType", ((int)ParWebApiUserTypesEnum).ToString() + GlobalFunctions.GetRandomAlphaNumeric()),
                    new KeyValuePair<string, string>("MachineID", LocalData.MachineID + GlobalFunctions.GetRandomAlphaNumeric()),
                });

                return await httpClient.MyPostFormGetJsonAsync<JwtResult>("token", formContent);

            }
            catch (Exception ex)
            {
                LocalFunctions.AddError(ex.Message, MethodBase.GetCurrentMethod(), true, false);

                return new JwtResult();
            }
        }



        internal static async Task<string> Cmd_Get_Setup_Data()
        {

            if (LocalData.IsDownloadedSetupData)
            {
                return "OK";
            }

            LocalData.IsDownloadedSetupData = false;
            try
            {
                
                string a = await CmdDownloadSetupData();

               

                if (a.Equals("Error"))
                {
                    return "Could not connect to server";
                }
                else
                {

                    a = a[10..^10];
                   
                    string tmp_Server_NotAuthorized_UserName = a.Substring(0,20);
                    string tmp_Server_NotAuthorized_UserPass = a.Substring(20,20);
                  


                   
                    LocalData.ServerNotAuthorizedUserPass = GlobalFunctions.ConvertToSecureString(tmp_Server_NotAuthorized_UserPass);
                    LocalData.ServerNotAuthorizedUserName = GlobalFunctions.ConvertToSecureString(tmp_Server_NotAuthorized_UserName);

                    a = string.Empty;
                                     
                    tmp_Server_NotAuthorized_UserName = string.Empty;
                    tmp_Server_NotAuthorized_UserPass = string.Empty;

                    LocalData.IsDownloadedSetupData = true;
                    return "OK";
                }
            }
            catch (Exception ex)
            {
                LocalData.IsDownloadedSetupData = false;
                return ex.Message;
            }

        }


        public static async Task<TSUser> CmdTSUserAuthorize(TSUser ParTSUser)
        {
           
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LocalData.CurrJWT);
            try
            {



                TSUser result = await httpClient.MyPostJsonGetJsonAsync("user/authorize", ParTSUser);


                GlobalFunctions.CmdAdjustEntityDate(result, false);

                return result;

            }
            catch (Exception ex)
            {

                LocalFunctions.AddError(ex.Message, MethodBase.GetCurrentMethod(), true, false);

            }

            return new TSUser();

        }



        public static async Task<TSEmail> CmdSendEmail(TSEmail ParTSEmail)
        {
           

            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LocalData.CurrJWT);
            try
            {
                return await httpClient.MyPostJsonGetJsonAsync("user/sendmail", ParTSEmail);

            }
            catch (Exception ex)
            {
                LocalFunctions.AddError(ex.Message, MethodBase.GetCurrentMethod(), true, false);

            }

            return new TSEmail();

        }



        public static async Task<TSEmail> CmdCheckEmailNotExists(TSEmail ParTSEmail)
        {

           
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LocalData.CurrJWT);
            try
            {
               
                return await httpClient.MyPostJsonGetJsonAsync("user/checkemail", ParTSEmail);
                
            }
            catch (Exception ex)
            {
                
                LocalFunctions.AddError(ex.Message, MethodBase.GetCurrentMethod(), true, false);

            }

            return new TSEmail();

        }


        public static async Task<TSEmail> CmdCheckUserNameNotExists(TSEmail ParTSEmail)
        {
           
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LocalData.CurrJWT);
            try
            {
                return await httpClient.MyPostJsonGetJsonAsync("user/checkusername", ParTSEmail);

            }
            catch (Exception ex)
            {
                LocalFunctions.AddError(ex.Message, MethodBase.GetCurrentMethod(), true, false);

            }

            return new TSEmail();

        }

        public static async Task<bool> CmdTSUserLogout()
        {
            try
            {


                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LocalData.CurrJWT);



                await httpClient.GetAsync("user/logout");


                httpClient.DefaultRequestHeaders.Accept.Clear();

            }
            catch (Exception ex)
            {
                LocalFunctions.AddError(ex.Message, MethodBase.GetCurrentMethod(), true, false);
                return false;
            }
            return true;

        }

        public static async Task<string> CmdTSUserRegister(TSUser ParTSUser)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LocalData.CurrJWT);

            GlobalFunctions.CmdTrimEntity(ParTSUser);

            TSUser tsUserForSend = GlobalFunctions.CopyObject<TSUser>(ParTSUser);



            HttpResponseMessage response = await httpClient.PostAsJsonAsync("user/add", tsUserForSend);
            string result = await response.Content.ReadFromJsonAsync<string>();


            httpClient.DefaultRequestHeaders.Accept.Clear();


            return result;

        }

        public static async Task<string> CmdTSUserChangePassword(TSUser ParTSUser)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LocalData.CurrJWT);

            GlobalFunctions.CmdTrimEntity(ParTSUser);

            TSUser tsUserForSend = GlobalFunctions.CopyObject<TSUser>(ParTSUser);

    
            HttpResponseMessage response = await httpClient.PostAsJsonAsync("user/changepassword", tsUserForSend);
            string result = await response.Content.ReadFromJsonAsync<string>();


            httpClient.DefaultRequestHeaders.Accept.Clear();


            return result;

        }

        public static async Task<string> CmdTSUserUpdateFont(TSVisitor ParTSVisitor)
        {
            string result=string.Empty;
            try
            {

        
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LocalData.CurrJWT);

            
                GlobalFunctions.CmdTrimEntity(ParTSVisitor);

                TSVisitor tsVisitorForSend = GlobalFunctions.CopyObject<TSVisitor>(ParTSVisitor);

            
                HttpResponseMessage response = await httpClient.PostAsJsonAsync("user/updatefont", tsVisitorForSend);
                result = await response.Content.ReadFromJsonAsync<string>();
                httpClient.DefaultRequestHeaders.Accept.Clear();

              

            }
            catch (Exception ex)
            {

                LocalFunctions.AddError(ex.Message, MethodBase.GetCurrentMethod(), true, false);
               
            }

           


            return result;

        }

        public static async Task<string> CmdAddTodo(TSTodo ParTSTodo)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LocalData.CurrJWT);

            GlobalFunctions.CmdTrimEntity(ParTSTodo);

            TSTodo tsTodoForSend = GlobalFunctions.CopyObject<TSTodo>(ParTSTodo);

            GlobalFunctions.CmdAdjustEntityDate(tsTodoForSend, true);


            HttpResponseMessage response = await httpClient.PostAsJsonAsync("todo/add", tsTodoForSend);
            string result = await response.Content.ReadFromJsonAsync<string>();

            httpClient.DefaultRequestHeaders.Accept.Clear();


            return result;

        }


        public static async Task<string> CmdUpdateTodo(TSTodo ParTSTodo)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LocalData.CurrJWT);

            GlobalFunctions.CmdTrimEntity(ParTSTodo);

            TSTodo tsTodoForSend = GlobalFunctions.CopyObject<TSTodo>(ParTSTodo);

            GlobalFunctions.CmdAdjustEntityDate(tsTodoForSend, true);


            HttpResponseMessage response = await httpClient.PutAsJsonAsync("todo/update", tsTodoForSend);
            string result = await response.Content.ReadFromJsonAsync<string>();

            httpClient.DefaultRequestHeaders.Accept.Clear();


            return result;

        }


        public static async Task<string> CmdDeleteTodo(TSTodo ParTSTodo)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LocalData.CurrJWT);

            GlobalFunctions.CmdTrimEntity(ParTSTodo);

            //TSTodo tsTodoForSend = GlobalFunctions.CopyObject<TSTodo>(ParTSTodo);

            TSTodo tsTodoForSend = new TSTodo
            {
                UserID = ParTSTodo.UserID,
                ID = ParTSTodo.ID
            };


            var sendRequest = new HttpRequestMessage(HttpMethod.Delete, "todo/delete")
            {
                Content = JsonContent.Create(tsTodoForSend)
            };

            HttpResponseMessage response = await httpClient.SendAsync(sendRequest);
            string result = await response.Content.ReadFromJsonAsync<string>();

            httpClient.DefaultRequestHeaders.Accept.Clear();


            return result;

        }

        public static async Task<List<TSTodoEx>> CmdGetAllTodos()
        {
            try
            {

                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LocalData.CurrJWT);


                List<TSTodoEx> result = await httpClient.MyGetJsonAsync<List<TSTodoEx>>("todo/getall");

                foreach (var item in result)
                {
                    GlobalFunctions.CmdAdjustEntityDate(item, false);
                }

                return result;

            }
            catch (Exception ex)
            {

                LocalFunctions.AddError(ex.Message, MethodBase.GetCurrentMethod(), true, false);
                return new List<TSTodoEx>();
            }


        }


        public static async Task<List<TSFeedbackEx>> CmdGetAllFeedback()
        {
            try
            {

                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LocalData.CurrJWT);


                List<TSFeedbackEx> result = await httpClient.MyGetJsonAsync<List<TSFeedbackEx>>("feedback/getAll");



                foreach (var item in result)
                {
                    GlobalFunctions.CmdAdjustEntityDate(item, false);
                }


                for (int i = 0; i < result.Count; i++)
                {
                    result[i].N = i + 1;
                }

                return result;

            }
            catch (Exception ex)
            {

                LocalFunctions.AddError(ex.Message, MethodBase.GetCurrentMethod(), true, false);
                return new List<TSFeedbackEx>();
            }


        }


        public static async Task CmdGetFeedback()
        {
            try
            {

                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LocalData.CurrJWT);


                LocalData.currFeedback = await httpClient.MyGetJsonAsync<TSFeedback>("feedback/get");

                if (!LocalData.currFeedback.UserID.Equals(Guid.Empty))
                {
                    GlobalFunctions.CmdAdjustEntityDate(LocalData.currFeedback, false);
                }


                if (string.IsNullOrEmpty(LocalData.currFeedback.Text))
                {
                    LocalData.oldFeedbackText = string.Empty;
                }
                else
                {
                    LocalData.oldFeedbackText = LocalData.currFeedback.Text.ToLower();
                }

            }
            catch (Exception ex)
            {

                LocalFunctions.AddError(ex.Message, MethodBase.GetCurrentMethod(), true, false);
                LocalData.currFeedback = new TSFeedback();
            }


        }

        public static async Task<bool> CmdGetAppVersion()
        {
            try
            {

                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LocalData.CurrJWT);


                TSAppVersion latestAppVersion = await httpClient.MyGetJsonAsync<TSAppVersion>("appversion/get");

                if (latestAppVersion.VersionNumber != LocalData.AppVersion.VersionNumber ||
                    latestAppVersion.VersionDate != LocalData.AppVersion.VersionDate)
                {
                    LocalFunctions.AddError("Your App current version (" + LocalData.AppVersion.VersionNumber + ") is obsolete, please empty cache and reload to get latest version (" + latestAppVersion.VersionNumber + ")",
                         MethodBase.GetCurrentMethod(),true, false, false);

                    return false;
                }


            }
            catch (Exception ex)
            {
                LocalFunctions.AddError(ex.Message, MethodBase.GetCurrentMethod(), true, false);
            }


            return true;

        }

        public static async Task<bool> CmdGetVisitor()
        {
            try
            {              
                LocalData.CurrVisitor = await httpClient.MyGetJsonAsync<TSVisitor>("visitor/get");
              
                if (!string.IsNullOrEmpty(LocalData.CurrVisitor.DefaultFont))
                {
                    if (LocalData.CurrDefaultFont != LocalData.CurrVisitor.DefaultFont)
                    {
                        LocalData.CurrDefaultFont = LocalData.CurrVisitor.DefaultFont;
                        LocalData.mainLayout.Refresh();
                        LocalData.indexPage.Refresh();
                    }
                }


            }
            catch (Exception ex)
            {
                LocalFunctions.AddError(ex.Message, MethodBase.GetCurrentMethod(), true, false);
            }


            return true;

        }


        public static async Task CmdGetReaction()
        {
            try
            {

                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LocalData.CurrJWT);


                LocalData.currReaction = await httpClient.MyGetJsonAsync<TSReaction>("reaction/get");

                

            }
            catch (Exception ex)
            {

                LocalFunctions.AddError(ex.Message, MethodBase.GetCurrentMethod(), true, false);
                LocalData.currReaction = new TSReaction();
            }


        }


        public static async Task<List<TSUserOpenEx>> CmdGetLiveUsers()
        {
            try
            {

                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LocalData.CurrJWT);



                List<TSUserOpenEx> result = await httpClient.MyGetJsonAsync<List<TSUserOpenEx>>("user/getliveusers");

                foreach (var item in result)
                {
                    
                    GlobalFunctions.CmdAdjustEntityDate(item, false);
                }

                return result;

            }
            catch (Exception ex)
            {

                LocalFunctions.AddError(ex.Message, MethodBase.GetCurrentMethod(), true, false);
                return new List<TSUserOpenEx>();
            }


        }


        public static async Task<List<TSUILanguageShortEx>> CmdGetSupportedLanguages()
        {
            try
            {


               

                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LocalData.CurrJWT);

               

                List<TSUILanguageShort> result = await httpClient.MyGetJsonAsync<List<TSUILanguageShort>>("googletranslator/getalllanguages");


                List<TSUILanguageShortEx> result2 = result.Select(g => new TSUILanguageShortEx{ Name = g.Name, Code = g.Code }).ToList();


                return result2;

            }
            catch (Exception ex)
            {

                LocalFunctions.AddError(ex.Message, MethodBase.GetCurrentMethod(), true, false);
                return new List<TSUILanguageShortEx>();
            }


        }


        public static async Task<TSStat> CmdGetStat()
        {
            try
            {
                httpClient.DefaultRequestHeaders.Accept.Clear();
                //httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LocalData.CurrJWT);

                return await httpClient.MyGetJsonAsync<TSStat>("stat");

            }
            catch (Exception ex)
            {
                LocalFunctions.AddError(ex.Message, MethodBase.GetCurrentMethod(), true, false);
                return new TSStat();
            }


        }




        public static async Task<string> CmdAddCategory(TSCategory ParTSCategory)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LocalData.CurrJWT);

            GlobalFunctions.CmdTrimEntity(ParTSCategory);

            TSCategory tsCategoryForSend = GlobalFunctions.CopyObject<TSCategory>(ParTSCategory);


            HttpResponseMessage response = await httpClient.PostAsJsonAsync("category/add", tsCategoryForSend);
            string result = await response.Content.ReadFromJsonAsync<string>();

            httpClient.DefaultRequestHeaders.Accept.Clear();


            return result;

        }


        public static async Task CmdAddOrUpdateReaction(TSReaction ParTSReaction)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LocalData.CurrJWT);

            GlobalFunctions.CmdTrimEntity(ParTSReaction);

            TSReaction tsReactionForSend = GlobalFunctions.CopyObject<TSReaction>(ParTSReaction);


            LocalData.currReaction = await httpClient.MyPostJsonGetJsonAsync("reaction/add", tsReactionForSend);

            httpClient.DefaultRequestHeaders.Accept.Clear();

        }

        public static async Task CmdAddOrUpdateFeedback(TSFeedback ParTSFeedback)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LocalData.CurrJWT);

            GlobalFunctions.CmdTrimEntity(ParTSFeedback);

            TSFeedback tsFeedbackForSend = GlobalFunctions.CopyObject<TSFeedback>(ParTSFeedback);


            LocalData.currFeedback = await httpClient.MyPostJsonGetJsonAsync("feedback/Add", tsFeedbackForSend);


            if (string.IsNullOrEmpty(LocalData.currFeedback.Text))
            {
                LocalData.oldFeedbackText = string.Empty;
            }
            else
            {
                LocalData.oldFeedbackText = LocalData.currFeedback.Text.ToLower();
            }

            httpClient.DefaultRequestHeaders.Accept.Clear();

        }


        public static async Task<string> CmdUpdateCategory(TSCategory ParTSCategory)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LocalData.CurrJWT);

            GlobalFunctions.CmdTrimEntity(ParTSCategory);



            TSCategory tsCategoryForSend = GlobalFunctions.CopyObject<TSCategory>(ParTSCategory as TSCategory);


            HttpResponseMessage response = await httpClient.PutAsJsonAsync("category/update", tsCategoryForSend);
            string result = await response.Content.ReadFromJsonAsync<string>();

            httpClient.DefaultRequestHeaders.Accept.Clear();


            return result;

        }


        public static async Task<string> CmdUpdateVisitor()
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LocalData.CurrJWT);

            GlobalFunctions.CmdTrimEntity(LocalData.CurrVisitor);



            TSVisitor tsVisitorForSend = GlobalFunctions.CopyObject<TSVisitor>(LocalData.CurrVisitor);


            HttpResponseMessage response = await httpClient.PutAsJsonAsync("Visitor/update", tsVisitorForSend);
            string result = await response.Content.ReadFromJsonAsync<string>();

            httpClient.DefaultRequestHeaders.Accept.Clear();


            return result;

        }


        public static async Task<string> CmdDeleteCategory(TSCategory ParTSCategory)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LocalData.CurrJWT);

            GlobalFunctions.CmdTrimEntity(ParTSCategory);


            TSCategory tsCategoryForSend = new TSCategory
            {
                UserID = ParTSCategory.UserID,
                ID = ParTSCategory.ID
            };



            var sendRequest = new HttpRequestMessage(HttpMethod.Delete, "category/delete")
            {
                Content = JsonContent.Create(tsCategoryForSend)
            };

            HttpResponseMessage response = await httpClient.SendAsync(sendRequest);
            string result = await response.Content.ReadFromJsonAsync<string>();

            httpClient.DefaultRequestHeaders.Accept.Clear();


            return result;

        }

        public static async Task<List<TSCategoryEx>> CmdGetAllCategories()
        {
            try
            {
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LocalData.CurrJWT);



                List<TSCategoryEx> result = await httpClient.MyGetJsonAsync<List<TSCategoryEx>>("category/getall");

                return result;
            }
            catch (Exception ex)
            {
                LocalFunctions.AddError(ex.Message, MethodBase.GetCurrentMethod(), true, false);
                return new List<TSCategoryEx>();
            }


        }


        public static async Task<List<TSUILanguage>> CmdGetAllUILanguages()
        {
            try
            {
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LocalData.CurrJWT);



                List<TSUILanguage> result = await httpClient.MyGetJsonAsync<List<TSUILanguage>>("uilanguage/getall");

                return result;
            }
            catch (Exception ex)
            {
                LocalFunctions.AddError(ex.Message, MethodBase.GetCurrentMethod(), true, false);
                return new List<TSUILanguage>();
            }


        }


        public static async Task<List<TSUIWordNative>> CmdGetAllUIWordNatives()
        {
            try
            {
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LocalData.CurrJWT);



                List<TSUIWordNative> result = await httpClient.MyGetJsonAsync<List<TSUIWordNative>>("uiwordnative/getall");

                return result;
            }
            catch (Exception ex)
            {
                LocalFunctions.AddError(ex.Message, MethodBase.GetCurrentMethod(), true, false);
                return new List<TSUIWordNative>();
            }


        }



        public static async Task<List<TSUIWordForeign>> CmdGetAllUIWordForeigns(Guid LangID)
        {

            try
            {
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LocalData.CurrJWT);

                TSUIWordForeign tmpWordForeign = new TSUIWordForeign { UILanguageID = LangID};


               
                List<TSUIWordForeign> result = await httpClient.MyPostJsonGetJsonEnumAsync<List<TSUIWordForeign>, TSUIWordForeign>("uiwordForeign/getallbylang", tmpWordForeign);

                return result;
            }
            catch (Exception ex)
            {
                LocalFunctions.AddError(ex.Message, MethodBase.GetCurrentMethod(), true, false);
                return new List<TSUIWordForeign>();
            }


        }




        public static async Task<TSEmail> CmdRecoverPass(TSEmail ParTSEmail)
        {

            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LocalData.CurrJWT);
            try
            {
                return await httpClient.MyPostJsonGetJsonAsync("user/passrecovery", ParTSEmail);

            }
            catch (Exception ex)
            {
                LocalFunctions.AddError(ex.Message, MethodBase.GetCurrentMethod(), true, false);

            }

            return new TSEmail();

        }

    }
}
