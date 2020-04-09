using BlazorContextMenu;
using BlazorTodos.Classes;
using BlazorTodos.Helpers;
using BlazorWindowHelper;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Threading.Tasks;
using TG.Blazor.IndexedDB;
using TodosShared;
using TodosUITranslator;
using static BlazorTodos.Classes.CustomClasses;
using static BlazorTodos.Classes.Enums;
using static TodosShared.TSEnums;

namespace BlazorTodos
{
    public static class LocalFunctions
    {
        private static TimerHelper timerHelper = new TimerHelper();
        public static NavigationManager navigationManager { get; set; } = null;

        public static void CmdNavigate(string ParRoute = "")
        {
            navigationManager.NavigateTo("/" + ParRoute);
        }


        public async static void CmdPrepare()
        {
            
            if (LocalData.IsDownloadedSetupData)
            {
              
                return;
            }
    
            if (LocalData.IsPreparingStarted)
            {
                return;
            }

          
            if (LocalData.TimezoneOffset==-99999)
            {
                LocalData.TimezoneOffset = await BTodosJsInterop.GetTimezoneOffset();
            }

            //long milliseconds = await BTodosJsInterop.GetDateMilliseconds();
            //.WriteLine(milliseconds);

            //DateTime d = new DateTime(1970,1,1).AddMilliseconds(milliseconds);

            //.WriteLine(d.ToString("MM/dd/yyyy HH:mm:ss.fff"));

        
            if (!LocalData.DesktopItemsList.Any())
            {
                PrepareDesktop();
            }
     
            LocalData.WindowSize.W = (int)(await BTodosJsInterop.GetWindowWidth());
            LocalData.WindowSize.H = (int)(await BTodosJsInterop.GetWindowHeight());

           
            if (string.IsNullOrEmpty(LocalData.MachineID))
            {
               
                LocalData.MachineID = await BTodosJsInterop.GetMachineID();
                
            }
           
            LocalData.IsPreparingStarted = true;
          

            if (!LocalData.IsDownloadedSetupData)
            {

                bool b = await WebApi.Cmd_Get_PublicData();

                if (b)
                {


                    LocalData.CurrJWT = string.Empty;
                    b = await WebApi.CmdGetJWT(LocalData.ServerNotAuthorizedUserName, LocalData.ServerNotAuthorizedUserPass, WebApiUserTypesEnum.NotAuthorized);

                    LocalData.IsReady = b;



                    bool IsAppVersionLatest = await WebApiFunctions.CmdGetAppVersion();

                    if (!IsAppVersionLatest)
                    {
                        LocalData.IsReady = false;
                        return;
                    }


                    if (LocalData.UsingUITranslator)
                    {
                        await GetUILanguages();
                    }



                    LocalData.compFooter.Refresh();

                    if (b)
                    {
                        timerHelper.OnTick = TimerTick;
                        if (LocalData.WebOrLocalMode)
                        {
                            timerHelper.Start(1, 10000);
                        }
                        else
                        {
                            timerHelper.Start(1, 10000 * 60);
                        }
                    }
                }
                else
                {
                    LocalData.IsReady = false;
                }
            }
        

            LocalData.IsPreparingStarted = false;
        }


        public static async void TimerTick()
        {
            if (LocalData.AppHasGlobalError)
            {
                timerHelper.Stop();
            }
            else
            {
              await GetStat();
            }
        }


        public static async Task GetStat()
        {
            LocalData.tsStat = await WebApiFunctions.CmdGetStat();
            LocalData.compFooter.Refresh();
        }


        public static async void Logout()
        {

            LocalData.btModal.Close();
            LocalData.btModalConfirm.Close();
            LocalData.btModalError.Close();
            LocalData.btModalMessage.Close();


            


            await WebApiFunctions.CmdTSUserLogout();
            LocalData.IsAuthenticated = false;
            LocalData.CurrTSUser = new TSUser();
            LocalData.LoginLogout = "Login";

            LocalData.IsReady = false;

            timerHelper.Stop();
            
            LocalData.CurrJWT = string.Empty;
            bool b = await WebApi.CmdGetJWT(LocalData.ServerNotAuthorizedUserName, LocalData.ServerNotAuthorizedUserPass, WebApiUserTypesEnum.NotAuthorized);

            LocalData.IsReady = b;


            timerHelper.OnTick = TimerTick;
            timerHelper.Start(1, 10000);

            LocalData.TSCategoriesList = new List<TSCategoryEx>();
            LocalData.TSCategoriesDictionary = new Dictionary<Guid, string>();
         
            LocalData.TsTodosList = new List<TSTodoEx>();
          
            LocalData.currFeedback = new TSFeedback();
            LocalData.currReaction = new TSReaction();


            CmdNavigate();


            LocalData.componentBridge.InvokeRefresh();
            LocalData.compHeader.Refresh();


        }

        public async static void Authorize(string ParUserName, string ParPassword)
        {
            LocalData.CurrJWT = string.Empty;

            timerHelper.Stop();

            if (await WebApi.CmdGetJWT(GlobalFunctions.ConvertToSecureString(ParUserName), GlobalFunctions.ConvertToSecureString(ParPassword), WebApiUserTypesEnum.Authorized))
            {

             
                TSUser tmpTSUser = new TSUser
                {
                    UserName = ParUserName,
                    Password = ParPassword,
                };

            
                LocalData.CurrTSUser = await WebApiFunctions.CmdTSUserAuthorize(tmpTSUser);
        

                if (LocalData.CurrTSUser.UserName.ToLower().Equals("error!"))
                {
                    LocalData.AppHasGlobalError = true;
                    AddError(LocalData.CurrTSUser.FullName, MethodBase.GetCurrentMethod(), true, false);
                }
                else { 

                    LocalData.IsAuthenticated = !LocalData.CurrTSUser.ID.Equals(Guid.Empty);

                    if (LocalData.IsAuthenticated)
                    {
                        timerHelper.OnTick = TimerTick;
                        timerHelper.Start(1, 10000);

                       
                        await WebApiFunctions.CmdGetFeedback();

                        
                        await WebApiFunctions.CmdGetReaction();
  

                        LocalData.LoginLogout = LocalData.CurrTSUser.UserName;

                        LocalData.btModal.Close();

                        CmdNavigate("DesktopPage");

                        LocalData.EventConsumerName = "TodosPage";
                        LocalData.componentBridge.InvokeRefresh();

                        LocalData.compHeader.Refresh();

                        LocalData.AppHasGlobalError = false;
                  
                    }
                    else
                    {
                        bool b = await WebApi.CmdGetJWT(LocalData.ServerNotAuthorizedUserName, LocalData.ServerNotAuthorizedUserPass, WebApiUserTypesEnum.NotAuthorized);

                        if (b)
                        {
                            timerHelper.OnTick = TimerTick;
                            timerHelper.Start(1, 10000);
                        }

                        LocalData.AppHasGlobalError = true;
                    }
                }
            }

          
        }


      

        public static void AddMessage(string ParMessage, bool Display, bool ClearPreviousMessages)
        {
            if (ClearPreviousMessages)
            {
                ClearMessages();
            }

            LocalData.BTMessagesList.Add(new BTMessage
            {
                ID = LocalData.BTMessagesList.Count + 1,
                Message = LocalData.uiTranslator.Translate(ParMessage),
            });

            if (Display)
            {
                DisplayMessages();
            }
        }

        public static void DisplayMessages()
        {
            if (HasMessage())
            {
                LocalData.btModalMessage.Show(ModalForm.Message);
            }
        }

        public static void AddError(string ParMessage, MethodBase Source, bool Display, bool ClearPreviousErrors, bool canCloseModal = true)
        {

            if (ClearPreviousErrors)
            {
                ClearErrors();
            }

            BTError err = new BTError
            {
                ID = LocalData.BTErrorsList.Count + 1,
                Message = LocalData.uiTranslator.Translate(ParMessage),
                OccurDate = ToLocalDate(DateTime.Now),
            };


            if (!LocalData.ProductionOrDevelopmentMode)
            {
                err.Source = Source.DeclaringType.FullName;
            }


            LocalData.BTErrorsList.Add(err);

            if (Display)
            {

                DisplayErrors(canCloseModal);
            }            
        }

        public static void ClearErrors()
        {
                if (HasError())
                {
                    LocalData.BTErrorsList = new List<BTError>();
                }
            
        }

        public static void ClearMessages()
        {
            if (HasMessage())
            {
                LocalData.BTMessagesList = new List<BTMessage>();
            }

        }

        public static void DisplayErrors(bool canCloseModal = true)
        {
            if (HasError())
            {
                LocalData.btModalError.Show(ModalForm.Error, canCloseModal);
            }
        }


        public static bool HasError()
        {
            return LocalData.BTErrorsList.Any();
        }

        public static bool HasMessage()
        {
            return LocalData.BTMessagesList.Any();
        }


        public static void DisplayModal(ModalForm parModalForm)
        {
            if (LocalData.IsReady)
            {
                LocalData.btModal.Show(parModalForm);
            }
            else
            {
                AddMessage("Not ready yet, please try again", true, false);
            }

        }


        public static void DisplayConfirm(string EventConsumerName, string QuestionText)
        {
            if (LocalData.IsReady)
            {

                LocalData.EventConsumerName = EventConsumerName;
                LocalData.ConfirmModalText = QuestionText;
                LocalData.btModalConfirm.Show(ModalForm.Confirm);
            }
            else
            {
                AddMessage("Not ready yet, please try again", true, false);
            }

        }

        public static string GetUniqueID()
        {
            return Guid.NewGuid().ToString("d").Substring(1, 4);
            //return "1";
        }

        public static string GetCategoryName(Guid CategoryID)
        {
            string result = string.Empty;

            if (!LocalData.TSCategoriesDictionary.Any())
            {
                LocalData.TSCategoriesList = WebApiFunctions.CmdGetAllCategories().Result;

            }
            else
            {
                LocalData.TSCategoriesDictionary.TryGetValue(CategoryID, out result);
            }

            return result;
        }



        public static void ContextMenu_DisplayLogout(PointInt p)
        {

            LocalData.bcMenu = new BCMenu()
            {
                ID = 1,
                Name = "Logout",
            };

            LocalData.bcMenu.Children.Add(new BCMenuItem
            {
                ID = 1,
                Text = LocalData.uiTranslator.Translate("User Profile"),
            });
            LocalData.bcMenu.Children.Add(new BCMenuItem
            {
                ID = 2,
                Text = LocalData.uiTranslator.Translate("Logout"),
            });

            //  LocalData.bcMenu.Show();

            LocalData.bcMenu.X = p.X;
            LocalData.bcMenu.Y = p.Y;

            LocalData.bcMenu.width = 50;

            LocalData.bcMenu.height = LocalData.bcMenu.Children.Count * 30+12;

            NormalizeContextMenuPosition();


            LocalData.compContextMenu.bcMenu = LocalData.bcMenu;
            LocalData.compContextMenu.Refresh();

        }


        public static void ContextMenu_Hide()
        {

            LocalData.bcMenu = new BCMenu();

            LocalData.compContextMenu.bcMenu = LocalData.bcMenu;
            LocalData.compContextMenu.Refresh();



        }



        private static void NormalizeContextMenuPosition()
        {

            if (LocalData.bcMenu.width<100)
            {
                LocalData.bcMenu.width = 100;
            }

            if (LocalData.bcMenu.Y + LocalData.bcMenu.height > LocalData.WindowSize.H)
            {
                LocalData.bcMenu.Y = LocalData.WindowSize.H - LocalData.bcMenu.height-10;
            }

            if (LocalData.bcMenu.X + LocalData.bcMenu.width > LocalData.WindowSize.W)
            {
                LocalData.bcMenu.X = LocalData.WindowSize.W - LocalData.bcMenu.width-10;
            }
        }


        private static void PrepareDesktop()
        {

            LocalData.DesktopItemsList.Add(new DesktopItem
            {
                ID = LocalData.DesktopItemsList.Count + 1,
                Text = LocalData.uiTranslator.Translate("Todos"),
                Icon = "icons/Desktop/Todos.png",
                Row = 1,
                Column = 1,
                NavigateTo = "TodosPage",
            });

           


            LocalData.DesktopItemsList.Add(new DesktopItem
            {
                ID = LocalData.DesktopItemsList.Count + 1,
                Text = LocalData.uiTranslator.Translate("Categories"),
                Icon = "icons/Desktop/Category.png",
                Row = 1,
                Column = 2,
                NavigateTo = "CategoriesPage",
            });


            LocalData.DesktopItemsList.Add(new DesktopItem
            {
                ID = LocalData.DesktopItemsList.Count + 1,
                Text = LocalData.uiTranslator.Translate("Profile"),
                Icon = "icons/Desktop/Profile.png",
                Row = 2,
                Column = 1,
                NavigateTo = "ProfilePage",
            });

            LocalData.DesktopItemsList.Add(new DesktopItem
            {
                ID = LocalData.DesktopItemsList.Count + 1,
                Text = LocalData.uiTranslator.Translate("Online Users"),
                Icon = "icons/Desktop/OnlineUsers.jpg",
                Row = 3,
                Column = 1,
                NavigateTo = "OnlineUsersPage",
            });

            LocalData.DesktopItemsList.Add(new DesktopItem
            {
                ID = LocalData.DesktopItemsList.Count + 1,
                Text = LocalData.uiTranslator.Translate("Feedback"),
                Icon = "icons/Desktop/Comment.png",
                Row = 3,
                Column = 2,
                NavigateTo = "FeedbackPage",
            });


            LocalData.DesktopItemsList.Add(new DesktopItem
            {
                ID = LocalData.DesktopItemsList.Count + 1,
                Text = LocalData.uiTranslator.Translate("About"),
                Icon = "icons/Desktop/Author.png",
                Row = 3,
                Column = 3,
                NavigateTo = "AboutPage",
            });
        }



        public static bool CheckEmailFormat(string email)
        {
            var attr = new EmailAddressAttribute();

            return attr.IsValid(email);
           
        }

        public static bool Validate<T>(T obj, bool displayErrors=true)
        {
            var validationErrors = new List<ValidationResult>();

            var context = new ValidationContext(obj, null, null);
            var isValid = Validator.TryValidateObject(obj, context, validationErrors, true);


        
            //if (HasError())
            //{
            //    DisplayErrors();
            //}
            //else
            //{
            //    ClearErrors();
            //}

            if (!isValid)
            {
                string a = string.Empty;
                foreach (var item in validationErrors)
                {
                    AddError(item.ErrorMessage, MethodBase.GetCurrentMethod(), false, false);
                    a += item.ErrorMessage + Environment.NewLine;
                }

                if (displayErrors)
                {
                    DisplayErrors();
                }
            }

            return isValid;
        }


        public static async Task GetTodosAndCategories()
        {
            bool updatedCategories = false;
            bool updatedTodos = false;

            if (!LocalData.TSCategoriesList.Any())
            {
                updatedCategories = true;
                LocalData.TSCategoriesList = await WebApiFunctions.CmdGetAllCategories();
            }

            if (!LocalData.TsTodosList.Any())
            {
                updatedTodos = true;
                LocalData.TsTodosList = await WebApiFunctions.CmdGetAllTodos();

                foreach (var item in LocalData.TsTodosList.Where(x => x.HasDueDate == false))
                {
                    item.DueDate = new DateTime();
                }

                foreach (var item in LocalData.TsTodosList.Where(x => !x.Reminders.Any()))
                {
                    item.Reminders = new List<DateTime>();
                }


                foreach (var item in LocalData.TsTodosList.Where(x => x.HasDueDate))
                {
                    item.DaysLeft = (short)(item.DueDate - ToLocalDate(DateTime.Now)).TotalDays;
                }

                for (int i = 0; i < LocalData.TsTodosList.Count; i++)
                {
                    LocalData.TsTodosList[i].N = i + 1;
                }


                foreach (var item in LocalData.TsTodosList)
                {
                    item.Category = GetCategoryName(item.CategoryID);

                }
            }


            if (updatedCategories || updatedTodos)
            {

                if (LocalData.TSCategoriesList.Any())
                {
                    for (int i = 0; i < LocalData.TSCategoriesList.Count; i++)
                    {
                        LocalData.TSCategoriesList[i].N = i + 1;
                        LocalData.TSCategoriesList[i].TodosCount = LocalData.TsTodosList.Count(x => x.CategoryID == LocalData.TSCategoriesList[i].ID);
                    }
                }
            }
        }


        public static async Task GetUILanguages()
        {
            LocalData.uiTranslator.ComboUILanguagesSelectedIndex = 1;

            if (!LocalData.uiTranslator.TSUILanguagesList.Any())
            {
                LocalData.uiTranslator.TSUILanguagesList = await WebApiFunctions.CmdGetAllUILanguages();


               

                if (LocalData.UsingIndexedDb)
                {
                    List<TSUILanguage> ListInIndexedDB = await LocalData.indexedDbManager.GetRecords<TSUILanguage>("UILanguages");

                    foreach (var item in LocalData.uiTranslator.TSUILanguagesList)
                    {
                        if (ListInIndexedDB is null)
                        {

                            await AddUILanguageAndStore(item);

                        }
                        else
                        {
                            if (ListInIndexedDB.Any(x => x.Code.Equals(item.Code, StringComparison.InvariantCultureIgnoreCase)))
                            {
                                if (item.Version != ListInIndexedDB.Single(x => x.Code.Equals(item.Code, StringComparison.InvariantCultureIgnoreCase)).Version)
                                {

                                    var updateRecord = new StoreRecord<TSUILanguage>
                                    {
                                        Storename = "UILanguages",
                                        Data = item
                                    };

                                    await LocalData.indexedDbManager.UpdateRecord(updateRecord);
                                    await LocalData.indexedDbManager.ClearStore("UILangDict" + item.Code);
                                }
                            }
                            else
                            {
                                await AddUILanguageAndStore(item);
                            }
                        }



                    }
                }

            }

        }


        private static async Task AddUILanguageAndStore(TSUILanguage item)
        {
            if (LocalData.UsingIndexedDb)
            {
                var newRecord = new StoreRecord<TSUILanguage>
                {
                    Storename = "UILanguages",
                    Data = item
                };

                await LocalData.indexedDbManager.AddRecord(newRecord);


                if (!item.Code.Equals("en", StringComparison.InvariantCultureIgnoreCase))
                {
                    AddStoreForLang(item.Code);

                }
            }
        }

        public static async Task AddStoreForLang(string Lang)
        {

            //if (!LocalData.indexDbManager.Stores.Any(x => x.Name.Equals("UILangDict" + Lang)))
            //{
            //    BTodosJsInterop.Log("creating store UILangDict" + Lang);
            //    LocalData.indexDbManager.Stores.Add(new StoreSchema
            //    {
            //        Name = "UILangDict" + Lang,
            //        PrimaryKey = new IndexSpec { Name = "id", KeyPath = "id", Auto = true },
            //        Indexes = new List<IndexSpec>
            //        {
            //            new IndexSpec{Name="native", KeyPath = "native", Auto=false},
            //            new IndexSpec{Name="foreign", KeyPath = "foreign", Auto=false},
            //        }
            //    });
            //}


            

            //var newRecord = new StoreRecord<UITranslatorItem>
            //{
            //    Storename = "UILangDict" + Lang,
            //    Data = new UITranslatorItem { Native = "test", Foreign = "test" }
            //};

            //await LocalData.indexDbManager.AddRecord(newRecord);
        }




        public static async Task GetUILangDict()
        {
            List<TSUIWordsPair> dictInIndexedDB = null;


            if (LocalData.UsingIndexedDb)
            {
                dictInIndexedDB = await LocalData.indexedDbManager.GetRecords<TSUIWordsPair>("UILangDict" + LocalData.uiTranslator.CurrUILanguage.Code);
            }

            if (dictInIndexedDB is null)
            {

                await DownloadUIDataAndSaveInIndexedDB();
            }
            else
            {
                if (dictInIndexedDB.Any())
                {

                    LocalData.uiTranslator.PrepareDictFromIndexedDB(dictInIndexedDB);
                }
                else
                {

                    await DownloadUIDataAndSaveInIndexedDB();

                }
            }


        }

        public static async Task DownloadUIDataAndSaveInIndexedDB()
        {
            if (!LocalData.uiTranslator.TSUIWordNativesList.Any())
            {
                LocalData.uiTranslator.TSUIWordNativesList = await WebApiFunctions.CmdGetAllUIWordNatives();
            }

            LocalData.uiTranslator.PrepareDict(await WebApiFunctions.CmdGetAllUIWordForeigns(LocalData.uiTranslator.CurrUILanguage.ID));



            if (LocalData.UsingIndexedDb)
            {
                foreach (var item in LocalData.uiTranslator.TSUIWordsPairsList)
                {
                    var newRecord = new StoreRecord<TSUIWordsPair>
                    {
                        Storename = "UILangDict" + LocalData.uiTranslator.CurrUILanguage.Code,
                        Data = item
                    };

                    await LocalData.indexedDbManager.AddRecord(newRecord);
                }
            }
        }

        public static DateTime ToLocalDate(DateTime d)
        {
            if (LocalData.TimezoneOffset != -99999)
            {

                return d.AddHours(-LocalData.TimezoneOffset);
            }
            else
            {
                return d;
            }
        }



        public static void ConsolePrint(string text, bool BeforeLine = false, bool AfterLine = false)
        {
            if (LocalData.ProductionOrDevelopmentMode) return;


            if (BeforeLine) BTodosJsInterop.Log(string.Empty);


            BTodosJsInterop.Log(string.Empty);

            if (AfterLine) BTodosJsInterop.Log(string.Empty);

        }

    }
}
