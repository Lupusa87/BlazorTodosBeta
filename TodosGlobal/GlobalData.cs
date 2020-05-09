using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace TodosGlobal
{
    public static class GlobalData
    {
        public static bool WebOrLocalMode = true; //if local will not send admin notify emails

        public static bool IsDevelopmentMode;

        public static string NotAuthorizedUserName = string.Empty;
        public static string NotAuthorizedUserPass = string.Empty;

        public static string AdminUserName = string.Empty;
        public static string AdminUserPass = string.Empty;
        public static string AdminIPAddress = string.Empty;
        public static string AdminMachineID = string.Empty;


        public static string JWTSecret = string.Empty;


        public static string AdminNotifyEmail = string.Empty;
        public static string AdminNotifyPhone = string.Empty;

        public static string GmailAccountName = string.Empty;
        public static string GmailAccountPass = string.Empty;

        public static string AzureTranslatorSubscriptionKey = string.Empty;
        public static string SendGridApiKey = string.Empty;


        public static string TwilioSID = string.Empty;
        public static string TwilioAuthToken = string.Empty;
        public static string TwilioPhoneNumber = string.Empty;
    }
}
