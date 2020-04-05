using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using TodosGlobal;
using static TodosShared.TSEnums;

namespace TodosFunctionsApi.JwtSecurity
{
    public static class MyTokenValidator
    {
        public static ClaimsPrincipal Authenticate(HttpRequest request,List<WebApiUserTypesEnum> AllowedRoles, List<string> CallTrace)
        {
            
            string token = GetTokenFromRequest(request, TodosCosmos.LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));


         

            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException();
            }

          

            var tokenHandler = new JwtSecurityTokenHandler();

            var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;



            if (jwtToken == null)
                return null;
          

            var tokenValidationParameters = new TokenValidationParameters
            {
                // The signing key must match!
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(GlobalData.JWTSecret)),

                // Validate the JWT Issuer (iss) claim
                ValidateIssuer = true,
                ValidIssuer = "ExampleIssuer",

                // Validate the JWT Audience (aud) claim
                ValidateAudience = true,
                ValidAudience = "ExampleAudience",

                // Validate the token expiry
                ValidateLifetime = true,

                // If you want to allow a certain amount of clock drift, set that here:
                //ClockSkew = TimeSpan.Zero,


            };



            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);


            if (principal == null)
            {
                
                    throw new UnauthorizedAccessException();
                
            }

            if (AllowedRoles.Any())
            {

                WebApiUserTypesEnum UserRole =(WebApiUserTypesEnum)int.Parse(LocalFunctions.CmdGetValueFromRoleClaim(principal.Claims, 10, TodosCosmos.LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod())));




                if (AllowedRoles.Any(x=>x.Equals(UserRole)))
                {
                    return principal;
                }
                else
                {
                    throw new UnauthorizedAccessException();
                }
            }
            else
            {
                return principal;
            }

         
        }


        private static string GetTokenFromRequest(HttpRequest request, List<string> CallTrace)
        {
            var authorizationHeader = request.Headers.SingleOrDefault(x => x.Key == "Authorization");
            var authenticationHeaderValue = AuthenticationHeaderValue.Parse(authorizationHeader.Value);

            if (authenticationHeaderValue == null || !string.Equals(authenticationHeaderValue.Scheme, "Bearer", StringComparison.InvariantCultureIgnoreCase))
                return null;

            return authenticationHeaderValue.Parameter;
        }

    }
}
