using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Text;
using TodosCosmos.ClientClasses;

namespace TodosCosmos
{
    public static class CosmosAPI
    {
        public static bool DoActivityLog = true;

        public static string databaseID = string.Empty;
        public static string collectionID = string.Empty;
        public static Container container;

        public static Guid DemoUserID;

        public static readonly CosmosDBClient_Activity cosmosDBClientActivity = new CosmosDBClient_Activity();
        public static readonly CosmosDBClient_Error cosmosDBClientError = new CosmosDBClient_Error();
        public static readonly CosmosDBClient_Setting cosmosDBClientSetting = new CosmosDBClient_Setting();
        public static readonly CosmosDBClient_User cosmosDBClientUser = new CosmosDBClient_User();
        public static readonly CosmosDBClient_Category cosmosDBClientCategory = new CosmosDBClient_Category();
        public static readonly CosmosDBClient_Feedback cosmosDBClientFeedback = new CosmosDBClient_Feedback();
        public static readonly CosmosDBClient_Reaction cosmosDBClientReaction = new CosmosDBClient_Reaction();
        public static readonly CosmosDBClient_Todo cosmosDBClientTodo = new CosmosDBClient_Todo();
        public static readonly CosmosDBClient_Visitor cosmosDBClientVisitor = new CosmosDBClient_Visitor();
        public static readonly CosmosDBClient_EmailedCode cosmosDBClientEmailedCode = new CosmosDBClient_EmailedCode();

        public static readonly CosmosDBClient_UILanguage cosmosDBClientUILanguage = new CosmosDBClient_UILanguage();
        public static readonly CosmosDBClient_UIWordNative cosmosDBClientUIWordNative = new CosmosDBClient_UIWordNative();
        public static readonly CosmosDBClient_UIWordForeign cosmosDBClientUIWordForeign = new CosmosDBClient_UIWordForeign();

        public static readonly CosmosDBClient_FeedMessage cosmosDBClientFeedMessage = new CosmosDBClient_FeedMessage();
    }
}
