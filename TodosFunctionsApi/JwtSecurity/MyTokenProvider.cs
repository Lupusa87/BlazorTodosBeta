using Microsoft.AspNetCore.Http;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static TodosShared.TSEnums;
using TodosShared;
using Microsoft.IdentityModel.Tokens;
using TodosGlobal;
using TodosCosmos.DocumentClasses;
using TodosCosmos;

namespace TodosFunctionsApi.JwtSecurity
{
    public class MyTokenProvider
    {

        private readonly MyTokenProviderOptions _options;

        public MyTokenProvider()
        {
           

            _options = new MyTokenProviderOptions
            {
                Audience = "ExampleAudience",
                Issuer = "ExampleIssuer",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(GlobalData.JWTSecret)), SecurityAlgorithms.HmacSha256),

            };
        }


        public async Task<JwtResult> GenerateToken(HttpRequest request)
        {


            string UserName = request.Form["UserName"];
            UserName = UserName.Substring(0, UserName.Length - 10);

            string UserPass = request.Form["UserPass"];
            UserPass = UserPass.Substring(0, UserPass.Length - 10);

            string UserType = request.Form["UserType"];
            UserType = UserType.Substring(0, UserType.Length - 10);


            WebApiUserTypesEnum tmpWebApiUserType = (WebApiUserTypesEnum)int.Parse(UserType);

     
            string MachineID = request.Form["MachineID"];
            MachineID = MachineID.Substring(0, MachineID.Length - 10);
  
            string Par_Out_Result = string.Empty;
            string tmp_IPAddress = request.HttpContext.Connection.RemoteIpAddress.ToString();


            var identity = await GetIdentity(UserName, UserPass, tmp_IPAddress, MachineID, tmpWebApiUserType, out Par_Out_Result);

            if (identity == null)
            {

                JwtResult result = new JwtResult
                {
                    AccessToken = string.Empty,
                    ExpiresIn = 0,
                    ErrorMessage = Par_Out_Result,
                };
              
                return result;
            }


            var now = DateTime.UtcNow;

            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "tmp_User"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, GlobalFunctions.ToUnixEpochDate(now).ToString(), ClaimValueTypes.Integer64),
                new Claim("UserID", Par_Out_Result), 
                new Claim("UserName", UserName.ToLower()+GlobalFunctions.GetRandomAlphaNumeric(10)), 
                new Claim("MachineID", request.Form["MachineID"]), 
                new Claim("ClientIP", tmp_IPAddress + GlobalFunctions.GetRandomAlphaNumeric(10)),
                new Claim("roles", ((int)tmpWebApiUserType).ToString() + GlobalFunctions.GetRandomAlphaNumeric(10)), 
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

                JwtResult result = new JwtResult
                {
                    AccessToken = new JwtSecurityTokenHandler().WriteToken(jwt),
                    ExpiresIn = (int)_options.Expiration.TotalSeconds,
                    ErrorMessage = string.Empty
                };

                return result;
            }
            catch (Exception ex)
            {

                JwtResult result = new JwtResult
                {
                    AccessToken = string.Empty,
                    ExpiresIn = 0,
                    ErrorMessage = ex.Message
                };

                return result;
            }

        }



        private Task<ClaimsIdentity> GetIdentity(string Par_Username,
                                                 string Par_Password,
                                                 string Par_IPAddress,
                                                 string Par_MachineID,
                                                 WebApiUserTypesEnum ParWebApiUserType,
                                                 out string Par_Out_Result)
        {

            Par_Out_Result = string.Empty;

            try
            {


                switch (ParWebApiUserType)
                {
                    case WebApiUserTypesEnum.NotAuthorized:
                        if (Par_Username.Equals(GlobalData.NotAuthorizedUserName) && Par_Password.Equals(GlobalData.NotAuthorizedUserPass))
                        {
                            Par_Out_Result = Guid.Empty.ToString() + GlobalFunctions.GetRandomAlphaNumeric(10);
                            return Task.FromResult(new ClaimsIdentity(new System.Security.Principal.GenericIdentity(Par_Username, "Token"), new Claim[] { }));
                        }
                        else
                        {
                            Par_Out_Result = "Invalid NotAutorizedUser UserName or Password";
                        }
                        break;
                    case WebApiUserTypesEnum.Authorized:
                        CosmosDocUser cosmosDocUser = CosmosAPI.cosmosDBClientUser.FindUserByUserName(Par_Username).Result;
                        if (cosmosDocUser != null)
                        {
                            if (LocalFunctions.CompareHash(Par_Password, cosmosDocUser))
                            {
                               
                                Par_Out_Result = cosmosDocUser.ID.ToString() + GlobalFunctions.GetRandomAlphaNumeric(10);
                             
                                return Task.FromResult(new ClaimsIdentity(new System.Security.Principal.GenericIdentity(Par_Username, "Token"), new Claim[] { }));
                            }
                            else
                            {
                                
                                Par_Out_Result = "Invalid User Password";
                            }
                        }
                        else
                        {
                           
                            Par_Out_Result = "Invalid User Name";
                        }

                        break;
                    case WebApiUserTypesEnum.Admin:
                      
                        if (Par_IPAddress.Equals(GlobalData.AdminIPAddress))
                        {
                            if (Par_MachineID.Equals(GlobalData.AdminMachineID))
                            {
                                if (Par_Username.Equals(GlobalData.AdminUserName) && Par_Password.Equals(GlobalData.AdminUserPass))
                                {
                                    Par_Out_Result = Guid.Empty.ToString() + GlobalFunctions.GetRandomAlphaNumeric(10);
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
                        Par_Out_Result = "unknown error";
                        break;
                }



            }
            catch (Exception ex)
            {
                bool b = CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, ex.Message, MethodBase.GetCurrentMethod()).Result;

            }

            return Task.FromResult<ClaimsIdentity>(null);
        }
    }


    public class MyTokenProviderOptions
    {
        public string Issuer { get; set; }

        public string Audience { get; set; }

        public TimeSpan Expiration { get; set; } = TimeSpan.FromMinutes(720);

        public SigningCredentials SigningCredentials { get; set; }
    }
}
