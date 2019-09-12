using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TodosShared;
using TodosWebApi.DataLayer;
using TodosWebApi.DataLayer.TSEntities;
using TodosWebApi.GlobalDataLayer;
using TodosWebApi.Models;
using static TodosShared.TSEnums;
using static TodosWebApi.GlobalDataLayer.GlobalEnums;

namespace TodosWebApi.JwtSecurity
{
    public class TokenProviderMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TokenProviderOptions _options;
        //private readonly TodosWebContext _context;
        private readonly TableStorage TS;

        public TokenProviderMiddleware(
            RequestDelegate next,
            IOptions<TokenProviderOptions> options)
        {
            _next = next;
            _options = options.Value;
            //_context = context;

            TS = new TableStorage();
        }

        public Task Invoke(HttpContext context)
        {
            // If the request path doesn't match, skip
            if (!context.Request.Path.Equals(_options.Path, StringComparison.Ordinal))
            {
                return _next(context);
            }

            // Request must be POST with Content-Type: application/x-www-form-urlencoded
            if (!context.Request.Method.Equals("POST")
               || !context.Request.HasFormContentType)
            {
                context.Response.StatusCode = 400;
                return context.Response.WriteAsync("Bad request.");
            }

            return GenerateToken(context);
            //return BuildToken(context);
        }


        private async Task GenerateToken(HttpContext context)
        {

            string UserName = GlobalFunctions.CmdAsymmetricDecrypt(context.Request.Form["UserName"]);
            UserName = UserName.Substring(0, UserName.Length - 10);

            string UserPass = GlobalFunctions.CmdAsymmetricDecrypt(context.Request.Form["UserPass"]);
            UserPass = UserPass.Substring(0, UserPass.Length - 10);

            string UserType = GlobalFunctions.CmdAsymmetricDecrypt(context.Request.Form["UserType"]);
            UserType = UserType.Substring(0, UserType.Length - 10);



            WebApiUserTypesEnum tmpWebApiUserType = (WebApiUserTypesEnum)Convert.ToInt16(UserType);


            string MachineID = GlobalFunctions.CmdAsymmetricDecrypt(context.Request.Form["MachineID"]);
            MachineID = MachineID.Substring(0, MachineID.Length - 10);

            string Par_Out_Result = string.Empty;
            string Par_Out_UserRole = string.Empty;
            string tmp_IPAddress = context.Connection.RemoteIpAddress.ToString();
          
            await TS.AddVisitor(tmp_IPAddress);

            await TS.AddActivityLog("AllUser","Token generation for " + tmp_IPAddress, MethodBase.GetCurrentMethod());

            var identity = await GetIdentity(UserName, UserPass, tmp_IPAddress, MachineID, tmpWebApiUserType, out Par_Out_Result, out Par_Out_UserRole);
            if (identity == null)
            {

                var error_response = new
                {
                    Access_token = "",
                    Expires_in = 0,
                    Error_Message = Par_Out_Result
                };


                // Serialize and return the response
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonConvert.SerializeObject(error_response, new JsonSerializerSettings { Formatting = Formatting.Indented }));

                return;
            }

            var now = DateTime.UtcNow;

            TimeSpan span = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
            double unixTime = span.TotalSeconds;

            // Specifically add the jti (random nonce), iat (issued timestamp), and sub (subject/user) claims.
            // You can add other claims here, if you want:


            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "tmp_User"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(now).ToString(), ClaimValueTypes.Integer64),
                new Claim("UserID", Par_Out_Result), //encrypted
                new Claim("UserName", GlobalFunctions.CmdAsymmetricEncrypt(UserName.ToLower()+GlobalFunctions.GetRandomAlphaNumeric(10))), //encrypted
                //new Claim("MyClientAsymPK", context.Request.Form["MyClientAsymPK"]), //encrypted
                new Claim("ClientSymmKey", context.Request.Form["ClientSymmKey"]), //encrypted
                new Claim("ClientSymmIV", context.Request.Form["ClientSymmIV"]), //encrypted
                new Claim("MachineID", context.Request.Form["MachineID"]), //encrypted
                new Claim("ClientIP", GlobalFunctions.CmdAsymmetricEncrypt(tmp_IPAddress + GlobalFunctions.GetRandomAlphaNumeric(10))), //encrypted
                new Claim("roles", Par_Out_UserRole),
            };




            // Create the JWT and write it to a string
            var jwt = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(_options.Expiration),
                signingCredentials: _options.SigningCredentials);


            try
            {

                var response = new
                {
                    Access_token = new JwtSecurityTokenHandler().WriteToken(jwt),
                    Expires_in = (int)_options.Expiration.TotalSeconds,
                    Error_Message = string.Empty
                };

                // Serialize and return the response
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.Indented }));

            }
            catch (Exception ex)
            {

                var response = new
                {
                    Access_token = string.Empty,
                    Expires_in = 0,
                    Error_Message = ex.Message
                };

                // Serialize and return the response
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.Indented }));

            }

        }

        private static long ToUnixEpochDate(DateTime date)
        {
            return (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
        }

        private Task<ClaimsIdentity> GetIdentity(string Par_Username,
                                                 string Par_Password,
                                                 string Par_IPAddress,
                                                 string Par_MachineID,
                                                 WebApiUserTypesEnum ParWebApiUserType,
                                                 out string Par_Out_Result,
                                                 out string Par_Out_UserRole)
        {

            Par_Out_Result = string.Empty;
            Par_Out_UserRole = string.Empty;

            try
            {


                switch (ParWebApiUserType)
                {
                    case WebApiUserTypesEnum.NotAuthorized:
                        Par_Out_UserRole = "NotAutorizedUser";
                        if (Par_Username.Equals(GlobalData.NotAuthorizedUserName) && Par_Password.Equals(GlobalData.NotAuthorizedUserPass))
                        {
                            Par_Out_Result = GlobalFunctions.CmdAsymmetricEncrypt("-745"+ GlobalFunctions.GetRandomAlphaNumeric(10));
                            return Task.FromResult(new ClaimsIdentity(new System.Security.Principal.GenericIdentity(Par_Username, "Token"), new Claim[] { }));
                        }
                        else
                        {
                            Par_Out_Result = "Invalid NotAutorizedUser UserName or Password";
                        }

                        break;
                    case WebApiUserTypesEnum.Authorized:
                        Par_Out_UserRole = "AutorizedUser";
                        TSUserEntity tsUserEntity = TS.FindUser(Par_Username, false, string.Empty).Result;
                        if (tsUserEntity != null)
                        {

                            if (GlobalFunctions.CompareHash(Par_Password, tsUserEntity))
                            {
                                Par_Out_Result = GlobalFunctions.CmdAsymmetricEncrypt(tsUserEntity.PartitionKey + GlobalFunctions.GetRandomAlphaNumeric(10));
                                return Task.FromResult(new ClaimsIdentity(new System.Security.Principal.GenericIdentity(Par_Username, "Token"), new Claim[] { })); 
                            }
                            else
                            {
                                Par_Out_Result = "Invalid AutorizedUser Password";
                            }
                        }
                        else
                        {
                            Par_Out_Result = "Invalid AutorizedUser Name";
                        }

                        break;
                    case WebApiUserTypesEnum.Admin:
                        Par_Out_UserRole = "Admin";
                        if (Par_IPAddress.Equals(GlobalData.AdminIPAddress))
                        {
                            if (Par_MachineID.Equals(GlobalData.AdminMachineID))
                            {
                                if (Par_Username.Equals(GlobalData.AdminUserName) && Par_Password.Equals(GlobalData.AdminUserPass))
                                {
                                    Par_Out_Result = GlobalFunctions.CmdAsymmetricEncrypt("-1"+ GlobalFunctions.GetRandomAlphaNumeric(10));
                                    return Task.FromResult(new ClaimsIdentity(new System.Security.Principal.GenericIdentity(Par_Username, "Token"), new Claim[] { }));
                                }
                                else
                                {

                                    Par_Out_Result = "Invalid Admin UserName or Password";
                                }
                            }
                            else
                            {
                                Par_Out_Result = "Invalid Admin MachineID";
                            }
                        }
                        else
                        {
                            Par_Out_Result = "Invalid Admin IPAddress";
                        }
                        break;
                    default:
                        break;
                }



            }
            catch (Exception ex)
            {
              bool b= TS.AddErrorLog("AllUsers", ex.Message, MethodBase.GetCurrentMethod()).Result;
               
            }

            // Credentials are invalid, or account doesn't exist
            return Task.FromResult<ClaimsIdentity>(null);
        }
    }
}
