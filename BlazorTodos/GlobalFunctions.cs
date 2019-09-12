using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BlazorTodos
{
    public static class GlobalFunctions
    {
        public static Random rnd = new Random();

       

        public static void CmdTrimEntity<T>(T Par_entity)
        {

            string tmp_str = string.Empty;
            foreach (PropertyInfo item in typeof(T).GetProperties().Where(x => x.PropertyType == typeof(string)))
            {

                tmp_str = (string)item.GetValue(Par_entity);
                if (!string.IsNullOrEmpty(tmp_str))
                {
                    item.SetValue(Par_entity, tmp_str.Trim());
                }
            }
        }

        public static void CmdAdjustDate<T>(T Par_entity, bool ToServerOrToLocal)
        {

            DateTime tmpdate;
            foreach (PropertyInfo item in typeof(T).GetProperties().Where(x => x.PropertyType == typeof(DateTime)))
            {


                if (!string.IsNullOrEmpty(item.GetValue(Par_entity).ToString()))
                {
                    tmpdate = (DateTime)item.GetValue(Par_entity);
                    item.SetValue(Par_entity, tmpdate);

                    if (ToServerOrToLocal)
                    {
                        if (LocalData.TimezoneOffset != -99999)
                        {
                            item.SetValue(Par_entity, tmpdate.AddHours(LocalData.TimezoneOffset));
                        }
                       
                    }
                    else
                    {
                        if (LocalData.TimezoneOffset != -99999)
                        {
                            item.SetValue(Par_entity, tmpdate.AddHours(-LocalData.TimezoneOffset));
                        }
                       
                    }
                }
            }
        }


        public static bool AreEqualTwoObjects<T>(T Obj1, T Obj2)
        {

            object propobj1;
            object propobj2;
            foreach (PropertyInfo item in typeof(T).GetProperties())
            {
                
                propobj1 = item.GetValue(Obj1);
                propobj2 = item.GetValue(Obj2);

                if (propobj1 is null || propobj2 is null)
                {
                    if (propobj1 is null && propobj2 is null)
                    {
                    }
                    else
                    {
                        return false;

                    }
                }
                else
                {

                    if (!propobj1.Equals(propobj2))
                    {
                        return false;
                    }
                }
            }

            return true;
        }



       

        public static string ConvertToPlainString(SecureString secureStr)
        {
            string plainStr = new NetworkCredential(string.Empty, secureStr).Password;
            return plainStr;
        }

        public static SecureString ConvertToSecureString(string password)
        {
            if (!string.IsNullOrEmpty(password))
            {
                SecureString securePassword = new SecureString();

                foreach (char c in password)
                {
                    securePassword.AppendChar(c);
                }

                securePassword.MakeReadOnly();
                return securePassword;
            }
            else
            {
                LocalFunctions.AddError("password is empty!", MethodBase.GetCurrentMethod(), true, false);
                return null;
            }
        }


        public static string GetRandomAlphaNumeric(int Par_Lenght = 10)
        {

            rnd.Next();
            var chars = @"abcdefghijklmnopqrstuvwxyz0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ.,+-*/\|()&^@#$%{}[];:><`?";
            return new string(chars.Select(c => chars[rnd.Next(chars.Length)]).Take(Par_Lenght).ToArray());
        }


        public static T CopyObject<T>(object objSource)

        {

            using (MemoryStream stream = new MemoryStream())

            {

                BinaryFormatter formatter = new BinaryFormatter();

                formatter.Serialize(stream, objSource);

                stream.Position = 0;

                return (T)formatter.Deserialize(stream);

            }

        }

    }
}
