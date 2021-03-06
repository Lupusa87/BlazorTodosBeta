﻿using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TodosCosmos.DocumentClasses;
using TodosShared;
using static TodosCosmos.Enums;

namespace TodosCosmos.ClientClasses
{
    public class CosmosDBClient_Visitor
    {
        private readonly CosmosDBRepository<CosmosDocVisitorsStat> cosmosDBRepo = new CosmosDBRepository<CosmosDocVisitorsStat>();
        public readonly CosmosDBClient_Base<CosmosDocVisitorsStat> cosmosDBClientBase =new CosmosDBClient_Base<CosmosDocVisitorsStat>();
       

        private readonly string pkPrefix = ((int)DocTypeEnum.Todo).ToString();

        public async Task<bool> AddVisitor(string IPAddress, List<string> CallTrace)
        {
            try
            {

                CosmosDocVisitorsStat visitor = await GetVisitor(IPAddress, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
             
                if (visitor is null)
                {
                   
                    visitor = new CosmosDocVisitorsStat(IPAddress, 0, Guid.Empty);

                    await cosmosDBClientBase.AddItemAsync(visitor, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
               
                    await CosmosAPI.cosmosDBClientSetting.UpdateSettingCounter(Guid.Empty, "IPAddressesCount", true, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));


                    await CosmosAPI.cosmosDBClientSetting.UpdateSettingCounter(Guid.Empty, "VisitsCount", true, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
                    visitor.VisitsCount += 1;


                    await CosmosAPI.cosmosDBClientVisitor.cosmosDBClientBase.UpdateItemAsync(visitor, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));


                }



                return true;

            }
            catch (CosmosException ex)
            {
                await CosmosAPI.cosmosDBClientError.AddErrorLog(Guid.Empty, ex.Message, 
                    LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

                return false;
            }
        }


        public async Task<CosmosDocVisitorsStat> GetVisitor(string IPAddress, List<string> CallTrace)
        {

            return await cosmosDBRepo.FindFirstItemsAsync(x => x.DocType == (int)DocTypeEnum.VisitorStat && x.IUD < 2 &&
                             x.IPAddress == IPAddress,
                             LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));


        }

        public async Task<bool> UpdateVisitor(TSVisitor tsVisitor, List<string> CallTrace)
        {

            CosmosDocVisitorsStat doc = await GetVisitor(tsVisitor.IPAddress, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));

            if (!string.IsNullOrEmpty(tsVisitor.DefaultFont))
            {
                doc.DefaultFont = tsVisitor.DefaultFont;
            }
            else
            {
                doc.DefaultFont = string.Empty;
            }

            return await cosmosDBClientBase.UpdateItemAsync(doc, LocalFunctions.AddThisCaller(CallTrace, MethodBase.GetCurrentMethod()));
        }

    }
}
