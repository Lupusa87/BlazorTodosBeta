﻿using AzureTranslatorAPI;
using GoogleTranslatorAPI;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using TodosGlobal;
using System.Linq;
using System.Collections;

[assembly: FunctionsStartup(typeof(TodosFunctionsApi.Startup))]

namespace TodosFunctionsApi
{
    //10.07.2019
    //https://docs.microsoft.com/en-us/azure/azure-functions/functions-dotnet-dependency-injection
    //https://dev.to/azure/using-entity-framework-with-azure-functions-50aa


    //https://github.com/Azure/azure-cosmos-dotnet-v3/tree/master/Microsoft.Azure.Cosmos.Samples/CodeSamples/AzureFunctions

    public class Startup : FunctionsStartup
    {


        

        private static IConfigurationRoot configuration = new ConfigurationBuilder()
               .SetBasePath(Environment.CurrentDirectory)
               .AddJsonFile("AppSettings.json", optional: true, reloadOnChange: true)
               .AddEnvironmentVariables()
               .Build();

        public override void Configure(IFunctionsHostBuilder builder)
        {

            GlobalData.IsDevelopmentMode = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT"));

          

            string endpoint = configuration["CosmosDbEndPoint"];
            if (string.IsNullOrEmpty(endpoint))
            {
                throw new ArgumentNullException("Please specify a valid endpoint in the appSettings.json file or your Azure Functions Settings.");
            }

            string authKey = configuration["CosmosDbKey"];
            if (string.IsNullOrEmpty(authKey))
            {
                throw new ArgumentException("Please specify a valid AuthorizationKey in the appSettings.json file or your Azure Functions Settings.");
            }

            TodosCosmos.CosmosAPI.databaseID = configuration["CosmosDbDatabaseName"];
            if (string.IsNullOrEmpty(TodosCosmos.CosmosAPI.databaseID))
            {
                throw new ArgumentException("Please specify a valid DbName in the appSettings.json file or your Azure Functions Settings.");
            }

            TodosCosmos.CosmosAPI.collectionID = configuration["CosmosDbCollectionName"];
            if (string.IsNullOrEmpty(TodosCosmos.CosmosAPI.collectionID))
            {
                throw new ArgumentException("Please specify a valid CollectionName in the appSettings.json file or your Azure Functions Settings.");
            }




            // delays startup about 3 sec, if will be used shoud move to another function app to be used on deman and not for each azure function execution
            //3/27/2020
            //builder.Services.AddSingleton((s) =>
            //{
            //    return new GoogleTranslator();
            //});


            GlobalFunctions.CmdReadEncryptedSettings();


            //Console.WriteLine("?.?>?>?>??????????????????????????????????????");


            //DateTime dt1 = DateTime.UtcNow;

            //Console.WriteLine(dt1.ToString("MM/dd/yyyy HH:mm:ss.fff"));


            SeedData.Run(new CosmosClient(endpoint, authKey), TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));


            //DateTime dt2 = DateTime.UtcNow;
            //Console.WriteLine(dt2.ToString("MM/dd/yyyy HH:mm:ss.fff"));
            //Console.WriteLine((dt2 - dt1).Duration().ToString());


            //builder.Services.AddHttpClient();

            //builder.Services.AddSingleton((s) =>
            //{
            //    return new CosmosClient(Environment.GetEnvironmentVariable("COSMOSDB_CONNECTIONSTRING"));
            //});


            //builder.Services.AddSingleton((s) =>
            //{

            //    CosmosClientBuilder configurationBuilder = new CosmosClientBuilder(endpoint, authKey);
            //    return configurationBuilder
            //            .Build();
            //});



            //GoogleTranslator gt = new GoogleTranslator();

            //string a = gt.Translate("hello", "ka");

            //Console.OutputEncoding = System.Text.Encoding.Unicode;
            //Console.WriteLine("=============????????????????==============");
            //Console.WriteLine(a);


        }
    }
}
