using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TodosCosmos;
using TodosCosmos.ClientClasses;
using TodosCosmos.DocumentClasses;
using TodosShared;
using static TodosCosmos.Enums;

namespace TodosFunctionsApi
{
    public static class DeleteCosmosDBData
    {
        public static bool DeleteActivity()
        {
            IEnumerable<CosmosDocActivityLog> list = CosmosAPI.cosmosDBClientActivity.GetAll(TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod())).Result;


            foreach (var item in list)
            {
                 CosmosAPI.cosmosDBClientActivity.Delete(DocDeleteModeEnum.Soft, item, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
            }


            return true;
        }


        public static bool DeleteUILanguages()
        {
            List<TSUILanguage> list = CosmosAPI.cosmosDBClientUILanguage.GetAll(TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod())).Result;


            foreach (var item in list)
            {
                 CosmosAPI.cosmosDBClientUILanguage.Delete(DocDeleteModeEnum.Soft, item, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
            }


            return true;
        }

        public static bool DeleteUIWordNative()
        {
            List<TSUIWordNative> list =  CosmosAPI.cosmosDBClientUIWordNative.GetAll(TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod())).Result;


            foreach (var item in list)
            {
                 CosmosAPI.cosmosDBClientUIWordNative.Delete(DocDeleteModeEnum.Soft, item, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
            }


            return true;
        }

        public static bool DeleteUIWordForeign()
        {
            List<TSUIWordForeign> list =  CosmosAPI.cosmosDBClientUIWordForeign.GetAll(TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod())).Result;


            foreach (var item in list)
            {
                 CosmosAPI.cosmosDBClientUIWordForeign.Delete(DocDeleteModeEnum.Soft, item, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
            }


            return true;
        }
    }
}
