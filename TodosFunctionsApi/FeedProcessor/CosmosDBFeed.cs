using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using TodosCosmos;
using TodosCosmos.DocumentClasses;
using static TodosCosmos.Enums;

namespace TodosFunctionsApi.FeedProcessor
{

    public static class CosmosDBFeed
    {
        [FunctionName("FunctionCosmosDBFeed")]
        public static async void Run([CosmosDBTrigger(
            databaseName: "%CosmosDbDatabaseName%",
            collectionName: "%CosmosDbCollectionName%",
            ConnectionStringSetting = "CosmosDBConnStr",
            LeaseCollectionName = "leases",
            CreateLeaseCollectionIfNotExists = true)]IReadOnlyList<Document> input, ILogger log)
        {


            ILookup<DocTypeEnum, Document> dict = input.ToLookup(x => (DocTypeEnum)x.GetPropertyValue<byte>("dt"), x => x);

            //process errors
            foreach (var item in dict[DocTypeEnum.Error])
            {

                CosmosDocErrorLog d = (dynamic)item;
                await TodosCosmos.LocalFunctions.NotifyAdmin("Error: " + d.Description, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
                TodosCosmos.LocalFunctions.ConsolePrint("Error: " + d.Description);
            }


            //process todos
            foreach (var item in dict[DocTypeEnum.Todo])
            {

                CosmosDocTodo d = (dynamic)item;

                if (d.IUD == (byte)DocStateMarkEnum.Insert)
                {
                    await TodosCosmos.LocalFunctions.NotifyAdmin("Insert todo " + d.UserID + " " + d.Name, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
                }
                else if (d.IUD == (byte)DocStateMarkEnum.Update)
                {
                    await TodosCosmos.LocalFunctions.NotifyAdmin("Update todo " + d.UserID + " " + d.Name, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
                }
                else if (d.IUD == (byte)DocStateMarkEnum.PreDelete)
                {
                    await TodosCosmos.LocalFunctions.NotifyAdmin("Delete todo " + d.UserID + " " + d.Name, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
                }


            }


            //process categories
            foreach (var item in dict[DocTypeEnum.Category])
            {
                CosmosDocCategory d = (dynamic)item;
                if (d.IUD == (byte)DocStateMarkEnum.Insert)
                {
                    await TodosCosmos.LocalFunctions.NotifyAdmin("Insert category " + d.UserID + " " + d.Name, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
                }
                else if (d.IUD == (byte)DocStateMarkEnum.Update)
                {
                    await TodosCosmos.LocalFunctions.NotifyAdmin("Update category " + d.UserID + " " + d.Name, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
                }
                else if (d.IUD == (byte)DocStateMarkEnum.PreDelete)
                {
                    await TodosCosmos.LocalFunctions.NotifyAdmin("Delete category " + d.UserID + " " + d.Name, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
                }
            }

            //process feedbacks
            foreach (var item in dict[DocTypeEnum.Feedback])
            {
                CosmosDocFeedback d = (dynamic)item;
                if (d.IUD == (byte)DocStateMarkEnum.Insert)
                {
                    await TodosCosmos.LocalFunctions.NotifyAdmin("Insert Feedback " + d.UserID + " " + d.Text, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
                }
                else if (d.IUD == (byte)DocStateMarkEnum.Update)
                {
                    await TodosCosmos.LocalFunctions.NotifyAdmin("Update Feedback " + d.UserID + " " + d.Text, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
                }
                else if (d.IUD == (byte)DocStateMarkEnum.PreDelete)
                {
                    await TodosCosmos.LocalFunctions.NotifyAdmin("Delete Feedback " + d.UserID + " " + d.Text, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
                }
            }

            //process reactions
            foreach (var item in dict[DocTypeEnum.Reaction])
            {
                CosmosDocReaction d = (dynamic)item;
                if (d.IUD == (byte)DocStateMarkEnum.Insert)
                {
                    await TodosCosmos.LocalFunctions.NotifyAdmin("Insert Reaction " + d.UserID + " " + d.LikeOrDislike, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
                }
                else if (d.IUD == (byte)DocStateMarkEnum.Update)
                {
                    await TodosCosmos.LocalFunctions.NotifyAdmin("Update Reaction " + d.UserID + " " + d.LikeOrDislike, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
                }
                else if (d.IUD == (byte)DocStateMarkEnum.PreDelete)
                {
                    await TodosCosmos.LocalFunctions.NotifyAdmin("Delete Reaction " + d.UserID + " " + d.LikeOrDislike, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
                }
            }



            //process users
            foreach (var item in dict[DocTypeEnum.User])
            {
                CosmosDocUser d = (dynamic)item;
                //if (d.IUD == (byte)DocStateMarkEnum.Insert)
                //{
                //    await TodosCosmos.LocalFunctions.NotifyAdmin("Insert user " + d.FullName, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
                //}  no sense to check this because user gets updated because of stat soon after creation
                //if (d.IUD == (byte)DocStateMarkEnum.Update)
                //{
                // await TodosCosmos.LocalFunctions.NotifyAdmin("Update user " + d.FullName, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
                //}
                if (d.IUD == (byte)DocStateMarkEnum.PreDelete)
                {
                    await TodosCosmos.LocalFunctions.NotifyAdmin("Delete user " + d.FullName, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
                }
            }

            //process FeedMessages
            foreach (var item in dict[DocTypeEnum.FeedMessage])
            {
                CosmosDocFeedMessage d = (dynamic)item;
                if (d.IUD == (byte)DocStateMarkEnum.Insert)
                {

                    RequestedActionEnum r = (RequestedActionEnum)d.RequestedAction;

                    switch (r)
                    {
                        case RequestedActionEnum.SendEmail:
                            break;
                        case RequestedActionEnum.SendSMS:
                            break;
                        case RequestedActionEnum.NotifyAdmin:
                            await TodosCosmos.LocalFunctions.NotifyAdmin(d.Bag, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
                            break;
                        case RequestedActionEnum.UpdateStat:
                            break;
                        default:
                            break;
                    }

                    LocalFunctions.SoftDeleteDoc(item);
                }


            }

            //process counters
            foreach (var item in dict[DocTypeEnum.Counter])
            {
                CosmosDocCounter d = (dynamic)item;
                if (d.IUD == (byte)DocStateMarkEnum.Insert)
                {
                    await TodosCosmos.LocalFunctions.NotifyAdmin("New Counter Activity  " + d.Source + " " + d.Action + " " + d.IPAddress,
                        TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod())
                        ,"New blazor activity "  + LocalFunctions.GetDemoNameFromURL(d.Source));
                }
            }


            //Soft Delete all preDelete marked docs
            IEnumerable<Document> shouldDeleteList = input.Where(x => x.GetPropertyValue<byte>("iud") == (byte)DocStateMarkEnum.PreDelete);
            foreach (var item in shouldDeleteList)
            {
                LocalFunctions.SoftDeleteDoc(item);
            }

        }


    }
}
