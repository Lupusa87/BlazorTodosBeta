using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
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
using TodosCosmos;
using TodosCosmos.DocumentClasses;
using TodosGlobal;
using TodosShared;
using static TodosGlobal.GlobalClasses;
using static TodosShared.TSEnums;

namespace TodosFunctionsApi
{
    public static class LocalFunctions
    {
        private static Random rnd = new Random();


        //public static string Get_Caller_Name([CallerMemberName]string name = "")
        //{
        //    return name;
        //}

        public static bool CompareHash(string Par_password, CosmosDocUser currCosmosDocUser)
        {
            return StructuralComparisons.StructuralEqualityComparer.Equals(GlobalFunctions.CmdHashPasswordBytes(Par_password, currCosmosDocUser.Salt), currCosmosDocUser.HashedPassword);
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

        public static string CmdGetValueFromRoleClaim(IEnumerable<Claim> UserClaims, int SaltLength)
        {
            Claim c1 = UserClaims.Single(x => x.Type == ClaimTypes.Role);

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
                string result = ParInput;
                return result.Substring(0, result.Length - SaltLength);
            }
            catch (Exception ex)
            {
                CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, ex.Message, MethodBase.GetCurrentMethod()).Wait();
                return string.Empty;
            }
        }


        public static string GetRandomPassword(int Par_Lenght)
        {
            // lupusa - 01.02.2015
            string result = string.Empty;

            if (Par_Lenght > 9)
            {
                CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, "Unique password max lenght is 9", MethodBase.GetCurrentMethod()).Wait();
                return result;
            }

            //int[] Numbers_List = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

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



    }
}
