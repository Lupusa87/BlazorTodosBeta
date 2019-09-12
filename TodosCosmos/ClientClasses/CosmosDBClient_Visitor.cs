﻿using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TodosCosmos.DocumentClasses;
using static TodosCosmos.Enums;

namespace TodosCosmos.ClientClasses
{
    public class CosmosDBClient_Visitor
    {
        private readonly CosmosDBRepository<CosmosDocVisitorsStat> cosmosDBRepo = new CosmosDBRepository<CosmosDocVisitorsStat>();
        private readonly CosmosDBClient_Base<CosmosDocVisitorsStat> cosmosDBClientBase =new CosmosDBClient_Base<CosmosDocVisitorsStat>();
       

        private readonly string pkPrefix = ((int)DocTypeEnum.Todo).ToString();

        public async Task<bool> AddVisitor(string IPAddress)
        {
            try
            {

                CosmosDocVisitorsStat visitor = await GetVisitor(IPAddress);
             
                if (visitor is null)
                {
                   
                    visitor = new CosmosDocVisitorsStat(IPAddress, 0, Guid.Empty);

                    await cosmosDBClientBase.AddItemAsync(visitor);
               
                    await CosmosAPI.cosmosDBClientSetting.UpdateSettingCounter(Guid.Empty, "IPAddressesCount", true);
        
                }

                await CosmosAPI.cosmosDBClientSetting.UpdateSettingCounter(Guid.Empty, "VisitsCount", true);
                visitor.Count += 1;
           

                await cosmosDBClientBase.UpdateItemAsync(visitor);
        

                return true;

            }
            catch (CosmosException ex)
            {
                await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, ex.Message, MethodBase.GetCurrentMethod());

                return false;
            }
        }


        public async Task<CosmosDocVisitorsStat> GetVisitor(string IPAddress)
        {

            return await cosmosDBRepo.FindFirstItemsAsync(x => x.DocType == (int)DocTypeEnum.VisitorStat &&
                             x.IPAddress == IPAddress);


        }



    }
}
