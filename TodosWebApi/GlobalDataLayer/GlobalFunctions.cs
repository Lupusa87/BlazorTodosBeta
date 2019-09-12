using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
//using SmartDictionaryWeb.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TodosShared;
using TodosWebApi.DataLayer;
using TodosWebApi.DataLayer.TSEntities;
using TodosWebApi.Models;
using static TodosShared.TSEnums;
using static TodosWebApi.GlobalDataLayer.GlobalClasses;
using static TodosWebApi.GlobalDataLayer.GlobalEnums;
//using static SmartDictionaryWeb.GlobalDataLayer.GlobalClasses;
//using static SmartDictionaryWeb.GlobalDataLayer.GlobalEnums;

namespace TodosWebApi.GlobalDataLayer
{
    public static class GlobalFunctions
    {
        private static Random rnd = new Random();

        private static readonly TableStorage TS = new TableStorage();

        //public static string Get_Caller_Name([CallerMemberName]string name = "")
        //{
        //    return name;
        //}

        public static bool CompareHash(string Par_password, TSUserEntity currTSUserEntity)
        {
            return StructuralComparisons.StructuralEqualityComparer.Equals(CmdHashPasswordBytes(Par_password, currTSUserEntity.Salt), currTSUserEntity.HashedPassword);
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


        public static string CmdAsymmetricEncrypt(string Par_Data, string Par_Public_Key = "")
        {
            CspParameters cspParams = new CspParameters { ProviderType = 1 };
            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(512, cspParams);

            if (string.IsNullOrEmpty(Par_Public_Key))
            {
                rsaProvider.ImportCspBlob(Convert.FromBase64String(GlobalData.ServerAsymmetricPublicKey));
            }
            else
            {
                rsaProvider.ImportCspBlob(Convert.FromBase64String(Par_Public_Key));
            }

            int a = rsaProvider.KeySize;

            byte[] plainBytes = Encoding.UTF8.GetBytes(Par_Data);
            byte[] encryptedBytes = rsaProvider.Encrypt(plainBytes, false);

            return Convert.ToBase64String(encryptedBytes);
        }

        public static string CmdAsymmetricDecrypt(string encrypted_Data)
        {
            CspParameters cspParams = new CspParameters { ProviderType = 1 };
            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(512, cspParams);

            rsaProvider.ImportCspBlob(Convert.FromBase64String(GlobalData.ServerAsymmetricPrivateKey));

            try
            {
                byte[] plainBytes = rsaProvider.Decrypt(Convert.FromBase64String(encrypted_Data), false);

                string plainText = Encoding.UTF8.GetString(plainBytes, 0, plainBytes.Length);

                return plainText;
            }
            catch (Exception)
            {

                return string.Empty;
            }

        }

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

        public static void CmdEncryptEntityAsymm<T>(T Par_entity, string Par_Public_Key = "")
        {

            string tmp_str = string.Empty;
            foreach (PropertyInfo item in typeof(T).GetProperties().Where(x => x.PropertyType == typeof(string)))
            {

                tmp_str = (string)item.GetValue(Par_entity);
                if (!string.IsNullOrEmpty(tmp_str))
                {
                    item.SetValue(Par_entity, CmdAsymmetricEncrypt(tmp_str, Par_Public_Key));
                }
            }
        }

        public static void CmdEncryptEntitySymm<T>(T Par_entity, SymmKeyAndIV ParSymmKeyAndIV)
        {

            string tmp_str = string.Empty;
            foreach (PropertyInfo item in typeof(T).GetProperties().Where(x => x.PropertyType == typeof(string)))
            {

                tmp_str = (string)item.GetValue(Par_entity);
                if (!string.IsNullOrEmpty(tmp_str))
                {
                    item.SetValue(Par_entity, CmdSymmetricEncrypt(tmp_str, ParSymmKeyAndIV));
                }
            }
        }

        public static void CmdDecryptEntityAsymm<T>(T Par_entity)
        {

            string tmp_str = string.Empty;
            foreach (PropertyInfo item in typeof(T).GetProperties().Where(x => x.PropertyType == typeof(string)))
            {

                tmp_str = (string)item.GetValue(Par_entity);
                if (!string.IsNullOrEmpty(tmp_str))
                {
                    item.SetValue(Par_entity, CmdAsymmetricDecrypt(tmp_str));
                }
            }
        }

        public static void CmdDecryptEntitySymm<T>(T Par_entity, SymmKeyAndIV ParSymmKeyAndIV)
        {

            string tmp_str = string.Empty;
            foreach (PropertyInfo item in typeof(T).GetProperties().Where(x => x.PropertyType == typeof(string)))
            {

                tmp_str = (string)item.GetValue(Par_entity);
                if (!string.IsNullOrEmpty(tmp_str))
                {
                    item.SetValue(Par_entity, CmdSymmetricDecrypt(tmp_str, ParSymmKeyAndIV));
                }
            }
        }

        public static string CmdSymmetricEncrypt(string plainText, SymmKeyAndIV ParSymmKeyAndIV)
        {
            return Convert.ToBase64String(CmdSymmetricEncrypt(plainText, Convert.FromBase64String(ParSymmKeyAndIV.Key), Convert.FromBase64String(ParSymmKeyAndIV.IV)));
        }

        private static byte[] CmdSymmetricEncrypt(string plainText, byte[] Key, byte[] IV)
        {

            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;
          
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

              
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

               
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            return encrypted;

        }

        public static string CmdSymmetricDecrypt(string cipherText, SymmKeyAndIV ParSymmKeyAndIV)
        {
            return CmdSymmetricDecrypt(Convert.FromBase64String(cipherText), Convert.FromBase64String(ParSymmKeyAndIV.Key), Convert.FromBase64String(ParSymmKeyAndIV.IV));
        }

        private static string CmdSymmetricDecrypt(byte[] cipherText, byte[] Key, byte[] IV)
        {
   
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");


            string plaintext = null;


            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;


                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);


                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

            return plaintext;


        }

        public static string CmdGetValueFromClaim(IEnumerable<Claim> UserClaims, string ClaimName, int SaltLength)
        {
            Claim c1 = UserClaims.Single(x => x.Type.Equals(ClaimName));
           
            if (c1 != null)
            {
                return CmdParseClaimValue(c1.Value, SaltLength);
            }
            else
            {
                return string.Empty;
            }
        }


        public static string CmdParseClaimValue(string ParInput,int SaltLength)
        {
            try
            {
                string result = CmdAsymmetricDecrypt(ParInput);
                return result.Substring(0, result.Length - SaltLength);
            }
            catch (Exception ex)
            {
                bool b = TS.AddErrorLog("AllUsers", ex.Message, MethodBase.GetCurrentMethod()).Result;
                return string.Empty;
            }
        }


        public static string CmdGetPublicData()
        {
            SymmKeyAndIV tmpSymmKeyAndIV = new SymmKeyAndIV();
            using (Aes myAes = Aes.Create())
            {
                tmpSymmKeyAndIV.Key = Convert.ToBase64String(myAes.Key);
                tmpSymmKeyAndIV.IV = Convert.ToBase64String(myAes.IV);
            }

            string publickey = CmdSymmetricEncrypt(GetSalt() + GlobalData.ServerAsymmetricPublicKey + GetSalt(), tmpSymmKeyAndIV);

            string NotAuthorizedUser = CmdSymmetricEncrypt(GetSalt() + GlobalData.NotAuthorizedUserName + GlobalData.NotAuthorizedUserPass + GetSalt(), tmpSymmKeyAndIV);

            tmpSymmKeyAndIV.Key = GetSalt() + tmpSymmKeyAndIV.Key + GetSalt();
            tmpSymmKeyAndIV.IV = GetSalt() + GetSalt() + tmpSymmKeyAndIV.IV +GetSalt()+ GetSalt();

            return GetSalt() + publickey + NotAuthorizedUser + tmpSymmKeyAndIV.IV + tmpSymmKeyAndIV.Key + GetSalt();
        }


        public static async Task NotifyAdmin(string ActivityDescription)
        {
            await CmdSendEmail(GlobalData.AdminNotifyEmail.Trim(), "Blazor Todo new Activity", ActivityDescription);
        }


        public static async Task<TSEmail> SendEmail(TSEmail ParTSEmail,string ParIPAddress,string ParMachineID)
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

                    tmp_Code = GetSalt();
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
                    tmp_Code = GetSalt();
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
                    tmp_Code = GetSalt();
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
                TSEmailedCode tsEmailedCode = new TSEmailedCode
                {
                    Email = ParTSEmail.To.Trim(),
                    Code = tmp_Code,
                    IPAddress = ParIPAddress,
                    OperationType = ParTSEmail.OperationCode,
                    MachineID = ParMachineID
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
                message.To.Add(new MailboxAddress(string.Empty, ParEmail));
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
                await TS.AddErrorLog("AllUsers", ex.Message, MethodBase.GetCurrentMethod());
                
                return false;
            }


            return result;
        }

        private static async Task<bool> CmdSaveEmailedCode(TSEmailedCode ParEmailedCode)
        {
            bool result = true;
            try
            {
                bool b = await TS.DeleteExpiredEmaiedCodes();

                b = await TS.DeleteEmaiedCodes(ParEmailedCode.Email);
            
                result = await TS.AddEmailedCode(ParEmailedCode);
            }
            catch (Exception ex)
            {
                await TS.AddErrorLog("AllUsers", ex.Message, MethodBase.GetCurrentMethod());
                result = false;
            }
            return result;
        }




        public static string GetRandomPassword(int Par_Lenght)
        {
            // lupusa - 01.02.2015
            string result = string.Empty;

            if (Par_Lenght > 9)
            {
                bool b = TS.AddErrorLog("AllUsers", "Unique password max lenght is 9", MethodBase.GetCurrentMethod()).Result;
               return result;
            }

            int[] Numbers_List = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            int Tmp_Value = 0;
            Random Rnd1 = new Random();
            do
            {
                Tmp_Value = Rnd1.Next(1, 9);
                if (!result.Contains(Tmp_Value.ToString()))
                {
                    result += Tmp_Value;
                }

            } while (result.Length < Par_Lenght);

            return result;
        }


        public static void CmdReadEncryptedSettings()
        {

            
            GlobalData.JWTSecret = GetRandomAlphaNumeric(40);

            GlobalData.NotAuthorizedUserName = GetRandomAlphaNumeric(20);
            GlobalData.NotAuthorizedUserPass = GetRandomAlphaNumeric(20);


            CspParameters cspParams = new CspParameters { ProviderType = 1 };

            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(512, cspParams);

            GlobalData.ServerAsymmetricPublicKey = Convert.ToBase64String(rsaProvider.ExportCspBlob(false));
            GlobalData.ServerAsymmetricPrivateKey = Convert.ToBase64String(rsaProvider.ExportCspBlob(true));

            

        }
    }
}
