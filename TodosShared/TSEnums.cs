using System;
using System.Collections.Generic;
using System.Text;

namespace TodosShared
{
    
    public class TSEnums
    {
        public enum WebApiUserTypesEnum
        {
            NotAuthorized = 0,
            Authorized = 1,
            Admin = 2,
            UnDefined = 3,
        }

        public enum EmailOperationsEnum
        {
            Registration = 0,
            EmailChange = 1,
            PasswordChange = 2,
            PasswordRecovery = 3,
            TodoReminder = 4,
        }
    }
}
