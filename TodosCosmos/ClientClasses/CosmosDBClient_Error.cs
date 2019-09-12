using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TodosCosmos.DocumentClasses;


namespace TodosCosmos.ClientClasses
{
    public class CosmosDBClient_Error
    {


        private readonly CosmosDBRepository<CosmosDocErrorLog> cosmosDBRepoErrorLog = new CosmosDBRepository<CosmosDocErrorLog>();

     
        public async Task<bool> AddErrorLog(Guid UserID, string Description, MethodBase MB)
        {

            CosmosDocErrorLog newErrorLog = new CosmosDocErrorLog(UserID, Description, LocalFunctions.GetMethodName(MB));


            await cosmosDBRepoErrorLog.CreateItemAsync(newErrorLog);


            await LocalFunctions.NotifyAdmin("Error: " + Description);

            return true;
        }

     


    }
}
