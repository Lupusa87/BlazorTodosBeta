using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TodosWebApi.GlobalDataLayer
{
    public class GlobalClasses
    {
        public class MyEncryptedAttribute : Attribute
        {
            public bool MyEncrypted { get; set; }


            //public int order { get; private set; }
            //public reportOrderAttribute(int par_order)
            //{
            //    order = par_order;
            //}
        }


        public class SymmKeyAndIV
        {
            public string Key { get; set; }
            public string IV { get; set; }
        }

    }
}
