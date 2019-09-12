using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodosWebApi.JwtSecurity
{
    public class TokenProviderOptions
    {
        public string Path { get; set; } = "/api/token";

        public string Issuer { get; set; }

        public string Audience { get; set; }

        public TimeSpan Expiration { get; set; } = TimeSpan.FromMinutes(720);

        public SigningCredentials SigningCredentials { get; set; }
    }
}
