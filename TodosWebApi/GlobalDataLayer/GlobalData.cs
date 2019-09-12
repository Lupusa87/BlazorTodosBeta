using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodosWebApi.GlobalDataLayer
{
    public static class GlobalData
    {

        public static bool WebOrLocalMode = true;

        public static string ServerAsymmetricPublicKey = string.Empty;
        public static string ServerAsymmetricPrivateKey = string.Empty;

        public static string NotAuthorizedUserName = string.Empty;
        public static string NotAuthorizedUserPass = string.Empty;

        public static string AdminUserName = string.Empty;
        public static string AdminUserPass = string.Empty;
        public static string AdminIPAddress = string.Empty;
        public static string AdminMachineID = string.Empty;


        public static string JWTSecret = string.Empty;


        public static string AdminNotifyEmail = "VakhtangiAbashidze@gmail.com";

        public static string GmailAccountName = "LupusaSmartDictionary@gmail.com";
        public static string GmailAccountPass = "lupSDlps19";


        public static bool DoActivityLog = true;
    }
}
