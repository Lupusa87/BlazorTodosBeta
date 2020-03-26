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

                httpClient.BaseAddress = LocalData.WebApi_Uri;
                httpClient.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);

                return await httpClient.GetStringAsync("setupdata");

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

                    a = a.Substring(10, a.Length - 20);
                   
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


                GlobalFunctions.CmdAdjustDate(result, false);

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


            string result = await httpClient.PostJsonAsync<string>("user/add", tsUserForSend);
            //string result = await httpClient.MyPostJsonGetStringAsync("user/add", tsUserForSend);


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


           
            string result = await httpClient.PostJsonAsync<string>("user/changepassword", tsUserForSend);

            httpClient.DefaultRequestHeaders.Accept.Clear();


            return result;

        }



        public static async Task<string> CmdAddTodo(TSTodo ParTSTodo)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LocalData.CurrJWT);

            GlobalFunctions.CmdTrimEntity(ParTSTodo);

            TSTodo tsTodoForSend = GlobalFunctions.CopyObject<TSTodo>(ParTSTodo);

            GlobalFunctions.CmdAdjustDate(tsTodoForSend, true);


            
            string result = await httpClient.PostJsonAsync<string>("todo/add", tsTodoForSend);


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

            GlobalFunctions.CmdAdjustDate(tsTodoForSend, true);
            

            string result = await httpClient.PutJsonAsync<string>("todo/update", tsTodoForSend);


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

            TSTodo tsTodoForSend = new TSTodo();
            tsTodoForSend.UserID = ParTSTodo.UserID;
            tsTodoForSend.ID = ParTSTodo.ID;
           

            string result = await httpClient.SendJsonAsync<string>(HttpMethod.Delete, "todo/delete", tsTodoForSend);


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
                    GlobalFunctions.CmdAdjustDate(item, false);
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
                    GlobalFunctions.CmdAdjustDate(item, false);
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
                    GlobalFunctions.CmdAdjustDate(LocalData.currFeedback, false);
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
                    
                    GlobalFunctions.CmdAdjustDate(item, false);
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


            string result = await httpClient.PostJsonAsync<string>("category/add", tsCategoryForSend);


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


            string result = await httpClient.PutJsonAsync<string>("category/update", tsCategoryForSend);


            httpClient.DefaultRequestHeaders.Accept.Clear();


            return result;

        }


        public static async Task<string> CmdDeleteCategory(TSCategory ParTSCategory)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LocalData.CurrJWT);

            GlobalFunctions.CmdTrimEntity(ParTSCategory);


            TSCategory tsCategoryForSend = new TSCategory();
            tsCategoryForSend.UserID = ParTSCategory.UserID;
            tsCategoryForSend.ID = ParTSCategory.ID;

            string result = await httpClient.SendJsonAsync<string>(HttpMethod.Delete, "category/delete", tsCategoryForSend);


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
