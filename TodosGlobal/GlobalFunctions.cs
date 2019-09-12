using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using static TodosGlobal.GlobalClasses;

namespace TodosGlobal
{
    public static class GlobalFunctions
    {

        private static Random rnd = new Random();

        public static string GetSalt()
        {
            //http://www.c-sharpcorner.com/article/hashing-passwords-in-net-core-with-tips/
            // Maximum length of salt
            int max_length = 10; // აწარმოებს 16 სიმბოლოს

            byte[] bytes = new byte[max_length];
            using (RandomNumberGenerator keyGenerator = RandomNumberGenerator.Create())
            {
                keyGenerator.GetBytes(bytes);
                return BitConverter.ToString(bytes).Replace("-", "").ToLower().Substring(0, 10);
            }

        }

        public static byte[] CmdHashPasswordBytes(string Par_PlainText, string Par_Salt)
        {
            //http://net-informations.com/q/faq/encrypt.html
            // slow method - hacker will be spent more time
            using (Rfc2898DeriveBytes crypto = new Rfc2898DeriveBytes(Par_PlainText, Encoding.Unicode.GetBytes(Par_Salt), 3131))
            {
                return crypto.GetBytes(64);
            }
        }


        public static string CmdHashPassword(string Par_PlainText, string Par_Salt)
        {
            return Convert.ToBase64String(CmdHashPasswordBytes(Par_PlainText, Par_Salt));
        }

        public static string GetRandomAlphaNumeric(int Par_Lenght = 20)
        {

            rnd.Next();
            var chars = @"abcdefghijklmnopqrstuvwxyz0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ.,+-*/\|()&^@#$%{}[];:><`?";
            return new string(chars.Select(c => chars[rnd.Next(chars.Length)]).Take(Par_Lenght).ToArray());
        }


        public static string CmdGetPublicData()
        {

            return GetSalt() + GlobalData.NotAuthorizedUserName + GlobalData.NotAuthorizedUserPass + GetSalt();
        }

        public static long ToUnixEpochDate(DateTime date)
        {
            return (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
        }


        public static void CmdReadEncryptedSettings()
        {

            if (string.IsNullOrEmpty(GlobalData.JWTSecret))
            {
                
                GlobalData.JWTSecret = Environment.GetEnvironmentVariable("JwtKey");


               
                GlobalData.NotAuthorizedUserName = Environment.GetEnvironmentVariable("NAUserName");
                GlobalData.NotAuthorizedUserPass = Environment.GetEnvironmentVariable("NAUserPass");


                GlobalData.AdminNotifyEmail = Environment.GetEnvironmentVariable("AdminNotifyEmail");
                GlobalData.GmailAccountName = Environment.GetEnvironmentVariable("GmailAccountName");
                GlobalData.GmailAccountPass = Environment.GetEnvironmentVariable("GmailAccountPass");


                GlobalData.AzureTranslatorSubscriptionKey = Environment.GetEnvironmentVariable("AzureTranslatorSubscriptionKey");
                
            }
        }


    }
}
