using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Build.Utilities;
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

            TodosCosmos.LocalFunctions.ConsolePrint("============== feed =================",true);

            ILookup<DocTypeEnum, Document> dict = input.ToLookup(x => (DocTypeEnum)x.GetPropertyValue<byte>("doctype"), x => x);


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
                    TodosCosmos.LocalFunctions.ConsolePrint("Insert todo Id " + d.ID + ", name - " + d.Name);
                }
                else if (d.IUD == (byte)DocStateMarkEnum.Update)
                {
                    await TodosCosmos.LocalFunctions.NotifyAdmin("Update todo " + d.UserID + " " + d.Name, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
                    TodosCosmos.LocalFunctions.ConsolePrint("Update todo Id " + d.ID + ", name - " + d.Name);
                }
                else if (d.IUD == (byte)DocStateMarkEnum.Delete)
                {
                    await TodosCosmos.LocalFunctions.NotifyAdmin("Delete todo " + d.UserID + " " + d.Name, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
                    TodosCosmos.LocalFunctions.ConsolePrint("Delete todo Id " + d.ID + ", name - " + d.Name);

                    LocalFunctions.SoftDeleteDoc(item);
                }


            }


            //process categories
            foreach (var item in dict[DocTypeEnum.Category])
            {
                CosmosDocCategory d = (dynamic)item;
                if (d.IUD == (byte)DocStateMarkEnum.Insert)
                {
                    await TodosCosmos.LocalFunctions.NotifyAdmin("Insert category " + d.UserID + " " + d.Name, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
                    TodosCosmos.LocalFunctions.ConsolePrint("insert category Id " + d.ID + ", name - " + d.Name);
                }
                else if (d.IUD == (byte)DocStateMarkEnum.Update)
                {
                    await TodosCosmos.LocalFunctions.NotifyAdmin("Update category " + d.UserID + " " + d.Name, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
                    TodosCosmos.LocalFunctions.ConsolePrint("update category Id " + d.ID + ", name - " + d.Name);
                }
                else if (d.IUD == (byte)DocStateMarkEnum.Delete)
                {
                    await TodosCosmos.LocalFunctions.NotifyAdmin("Delete category " + d.UserID + " " + d.Name, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
                    TodosCosmos.LocalFunctions.ConsolePrint("delete category Id " + d.ID + ", name - " + d.Name);

                    LocalFunctions.SoftDeleteDoc(item);
                }
            }

            //process feedbacks
            foreach (var item in dict[DocTypeEnum.Feedback])
            {
                CosmosDocFeedback d = (dynamic)item;
                if (d.IUD == (byte)DocStateMarkEnum.Insert)
                {
                    await TodosCosmos.LocalFunctions.NotifyAdmin("Insert Feedback " + d.UserID + " " + d.Text, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
                    TodosCosmos.LocalFunctions.ConsolePrint("insert Feedback Id " + d.ID + ", name - " + d.Text);
                }
                else if (d.IUD == (byte)DocStateMarkEnum.Update)
                {
                    await TodosCosmos.LocalFunctions.NotifyAdmin("Update Feedback " + d.UserID + " " + d.Text, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
                    TodosCosmos.LocalFunctions.ConsolePrint("update Feedback Id " + d.ID + ", name - " + d.Text);
                }
                else if (d.IUD == (byte)DocStateMarkEnum.Delete)
                {
                    await TodosCosmos.LocalFunctions.NotifyAdmin("Delete Feedback " + d.UserID + " " + d.Text, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
                    TodosCosmos.LocalFunctions.ConsolePrint("delete Feedback Id " + d.ID + ", name - " + d.Text);

                    LocalFunctions.SoftDeleteDoc(item);
                }
            }

            //process reactions
            foreach (var item in dict[DocTypeEnum.Reaction])
            {
                CosmosDocReaction d = (dynamic)item;
                if (d.IUD == (byte)DocStateMarkEnum.Insert)
                {
                    await TodosCosmos.LocalFunctions.NotifyAdmin("Insert Reaction " + d.UserID + " " + d.LikeOrDislike, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
                    TodosCosmos.LocalFunctions.ConsolePrint("insert reaction Id " + d.ID + ", LikeOrDislike - " + d.LikeOrDislike);
                }
                else if (d.IUD == (byte)DocStateMarkEnum.Update)
                {
                    await TodosCosmos.LocalFunctions.NotifyAdmin("Update Reaction " + d.UserID + " " + d.LikeOrDislike, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
                    TodosCosmos.LocalFunctions.ConsolePrint("update reaction Id " + d.ID + ", LikeOrDislike - " + d.LikeOrDislike);
                }
                else if (d.IUD == (byte)DocStateMarkEnum.Delete)
                {
                    await TodosCosmos.LocalFunctions.NotifyAdmin("Delete Reaction " + d.UserID + " " + d.LikeOrDislike, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
                    TodosCosmos.LocalFunctions.ConsolePrint("delete reaction Id " + d.ID + ", LikeOrDislike - " + d.LikeOrDislike);

                    LocalFunctions.SoftDeleteDoc(item);
                }
            }



            //process users
            foreach (var item in dict[DocTypeEnum.User])
            {
                CosmosDocUser d = (dynamic)item;
                if (d.IUD == (byte)DocStateMarkEnum.Insert)
                {
                    await TodosCosmos.LocalFunctions.NotifyAdmin("Insert user " + d.FullName, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
                    TodosCosmos.LocalFunctions.ConsolePrint("insert user Id " + d.ID + ", name - " + d.FullName);
                }
                else if (d.IUD == (byte)DocStateMarkEnum.Update)
                {
                    // await TodosCosmos.LocalFunctions.NotifyAdmin("Update user " + d.FullName, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
                    TodosCosmos.LocalFunctions.ConsolePrint("update user Id " + d.ID + ", name - " + d.FullName);
                }
                else if (d.IUD == (byte)DocStateMarkEnum.Delete)
                {
                    await TodosCosmos.LocalFunctions.NotifyAdmin("Delete user " + d.FullName, TodosCosmos.LocalFunctions.AddThisCaller(new List<string>(), MethodBase.GetCurrentMethod()));
                    TodosCosmos.LocalFunctions.ConsolePrint("delete user Id " + d.ID + ", name - " + d.FullName);

                    LocalFunctions.SoftDeleteDoc(item);
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
                            TodosCosmos.LocalFunctions.ConsolePrint("feed action notifyAdmin - " + d.Bag);
                            break;
                        case RequestedActionEnum.UpdateStat:
                            break;
                        default:
                            break;
                    }

                    LocalFunctions.SoftDeleteDoc(item);
                }

                
            }

        }

       
    }
}
