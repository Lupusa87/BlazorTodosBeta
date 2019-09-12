using System;
using System.Collections.Generic;
using System.Text;

namespace TodosShared
{
    [Serializable]
    public class JwtResult
    {
        public string AccessToken { get; set; }
        public int ExpiresIn { get; set; }
        public string ErrorMessage { get; set; }
    }
}
