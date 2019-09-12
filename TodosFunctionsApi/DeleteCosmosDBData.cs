using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TodosCosmos;
using TodosCosmos.ClientClasses;
using TodosCosmos.DocumentClasses;
using TodosShared;

namespace TodosFunctionsApi
{
    public static class DeleteCosmosDBData
    {
        public static bool DeleteActivity()
        {
            IEnumerable<CosmosDocActivityLog> list = CosmosAPI.cosmosDBClientActivity.GetAll().Result;


            foreach (var item in list)
            {
                 CosmosAPI.cosmosDBClientActivity.Delete(item);
            }


            return true;
        }


        public static bool DeleteUILanguages()
        {
            List<TSUILanguage> list = CosmosAPI.cosmosDBClientUILanguage.GetAll().Result;


            foreach (var item in list)
            {
                 CosmosAPI.cosmosDBClientUILanguage.Delete(item);
            }


            return true;
        }

        public static bool DeleteUIWordNative()
        {
            List<TSUIWordNative> list =  CosmosAPI.cosmosDBClientUIWordNative.GetAll().Result;


            foreach (var item in list)
            {

                //Console.WriteLine("deleting " + item.Word);

                 CosmosAPI.cosmosDBClientUIWordNative.Delete(item);
            }


            return true;
        }

        public static bool DeleteUIWordForeign()
        {
            List<TSUIWordForeign> list =  CosmosAPI.cosmosDBClientUIWordForeign.GetAll().Result;


            foreach (var item in list)
            {
                 CosmosAPI.cosmosDBClientUIWordForeign.Delete(item);
            }


            return true;
        }
    }
}
