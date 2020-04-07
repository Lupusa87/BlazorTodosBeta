using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TodosCosmos.DocumentClasses;
using static TodosCosmos.Enums;

namespace TodosCosmos.ClientClasses
{
    public class CosmosDBClient_EmailedCode
    {

        private readonly CosmosDBRepository<CosmosEmailedCode> cosmosDBRepo = new CosmosDBRepository<CosmosEmailedCode>();
        private readonly CosmosDBClient_Base<CosmosEmailedCode> cosmosDBClientBase = new CosmosDBClient_Base<CosmosEmailedCode>();

        private readonly string pkPrefix = ((int)DocTypeEnum.EmailedCode).ToString();



        public async Task<bool> AddEmailedCode(CosmosEmailedCode tsEmailedCode, List<string> CallTrace)
        {
            try
            {
                return await cosmosDBClientBase.AddItemAsync(tsEmailedCode, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

            }
            catch (CosmosException ex)
            {

                await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, ex.Message, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

                return false;
            }
        }


        public async Task<bool> DeleteEmailedCodes(string Email, List<string> CallTrace)
        {

            try
            {
                IEnumerable<CosmosEmailedCode> result = await cosmosDBRepo.GetItemsAsync(x => x.DocType == (int)DocTypeEnum.EmailedCode && x.Email.ToLower() == Email.ToLower(), LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

                if (result.Any())
                {
                    foreach (var item in result)
                    {
                        await cosmosDBClientBase.DeleteItemAsync(item, pkPrefix, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
                    }
                }

            }
            catch (CosmosException ex)
            {

                await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, ex.Message, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

                return false;
            }


            return true;



        }


        public async Task<CosmosEmailedCode> FindEmaiedCode(string pEmail, string pIPAddress, string pMachineID, List<string> CallTrace)
        {

               return  await cosmosDBRepo.FindLastItemsAsync(x => x.DocType == (int)DocTypeEnum.EmailedCode &&
                x.Email.ToLower() == pEmail.ToLower() &&
                x.IPAddress.ToLower() == pIPAddress.ToLower() &&
                x.MachineID.ToLower() == pMachineID.ToLower(), LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

        }



        public async Task<CosmosEmailedCode> GetEmailedCode(CosmosEmailedCode tsEmailedCode, List<string> CallTrace)
        {

            return (await cosmosDBClientBase.GetItemAsync(tsEmailedCode, pkPrefix, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod())));

        }


    }
}
