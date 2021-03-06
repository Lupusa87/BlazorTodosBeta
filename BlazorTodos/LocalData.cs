﻿using BlazorContextMenu;
using BlazorTodos.Classes;
using BlazorTodos.Components;
using BlazorTodos.Modal;
using BlazorTodos.Modals;
using BlazorTodos.Pages;
using BlazorTodos.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using TG.Blazor.IndexedDB;
using TodosShared;
using TodosUITranslator;
using static BlazorTodos.Classes.CustomClasses;
using static BlazorTodos.Classes.Enums;

namespace BlazorTodos
{
    public class LocalData
    {
        public static bool WebOrLocalMode = true; // determines where is azure functions backend
        public static bool IsDevelopmentMode;
        public static TSAppVersion AppVersion = new TSAppVersion
        {
            VersionNumber = "2.0.0",
            VersionDate = new DateTime(2021, 1, 12)
        };

        public static string CurrDefaultFont = "Sylfaen";
        public static string OldDefaultFont;
        public static TSVisitor CurrVisitor = new TSVisitor();

        public static Uri WebApi_Uri
        {
            get
            {
                if (WebOrLocalMode)
                {
                    return new Uri("https://blazortodosfunctionsapi.azurewebsites.net/api/");            
                }
                else
                {
                    return new Uri("http://localhost:7071/api/");
                }
            }
        }

        public static bool IsPreparingStarted = false;

        
        public static string LoginLogout = "Login";

        public static bool IsAuthenticated = false;
        public static TSUser CurrTSUser = new TSUser();

        public static string CurrJWT = string.Empty;

        public static bool IsReady = false;

        public static bool IsDownloadedSetupData = false;

        
        public static SecureString ServerNotAuthorizedUserName = null;
        public static SecureString ServerNotAuthorizedUserPass = null;



        public static BTModal btModal { get; set; } = new BTModal();

        public static BTModal btModalMessage { get; set; } = new BTModal();

        public static BTModal btModalError { get; set; } = new BTModal();

        public static BTModal btModalConfirm { get; set; } = new BTModal();

        public static CompMessage compMessage { get; set; } = new CompMessage();

        public static CompError compError { get; set; } = new CompError();

        public static List<BTMessage> BTMessagesList { get; set; } = new List<BTMessage>();

        public static List<BTError> BTErrorsList { get; set; } = new List<BTError>();

        public static CompAddOrUpdateTodo compAddOrUpdateTodo { get; set; } = new CompAddOrUpdateTodo();

        public static CompAddOrUpdateCategory compAddOrUpdateCategory { get; set; } = new CompAddOrUpdateCategory();

        public static CompAddOrUpdateFeedback compAddOrUpdateFeedback { get; set; } = new CompAddOrUpdateFeedback();

        public static CompLogin compLogin { get; set; } = new CompLogin();
        public static CompRegistration compRegistration { get; set; } = new CompRegistration();
        public static CompDefaultFont compDefaultFont { get; set; } = new CompDefaultFont();
        public static CompChangePassword compChangePassword { get; set; } = new CompChangePassword();


        public static ComponentBridge componentBridge { get; set; } = new ComponentBridge();

        public static bool AddOrUpdateMode { get; set; } = true;
        public static TSTodo CurrTodo { get; set; } = new TSTodo();
        public static TSTodo BeforeUpdateTodo { get; set; } = new TSTodo();
        public static string EventConsumerName { get; set; }



        public static IndexPage indexPage { get; set; } = new IndexPage();

        public static ProfilePage profilePage { get; set; } = new ProfilePage();
        public static MainLayout mainLayout { get; set; } = new MainLayout();
        public static CompHeader compHeader { get; set; } = new CompHeader();
        public static CompFooter compFooter { get; set; } = new CompFooter();
        public static TSStat tsStat { get; set; } = new TSStat() { IPsCount = 0, LiveUsersCount = 0, TodosCount = 0, UsersCount = 0, VisitsCount = 0 };


        public static bool AppHasGlobalError = false;

        public static Dictionary<Guid, string> TSCategoriesDictionary { get; set; } = new Dictionary<Guid, string>();

        private static List<TSCategoryEx> _TSCategoriesList { get; set; } = new List<TSCategoryEx>();

        public static List<TSCategoryEx> TSCategoriesList
        {
            get
            {
                return _TSCategoriesList;
            }
            set
            {
                _TSCategoriesList = value;
                TSCategoriesDictionary= _TSCategoriesList.ToDictionary(x => x.ID, x => x.Name);
            }
        }

        

        public static TSCategory currCategory { get; set; } = new TSCategory();
        public static string currCategoryName { get; set; }

        public static List<TSTodoEx> TsTodosList { get; set; } = new List<TSTodoEx>();


        public static int TimezoneOffset { get; set; } = -99999;



        public static CompContextMenu compContextMenu { get; set; } = new CompContextMenu();
        public static BCMenu bcMenu { get; set; } = new BCMenu();


        public static SizeInt WindowSize { get; set; } = new SizeInt();


        public static List<DesktopItem> DesktopItemsList = new List<DesktopItem>();

        public static string MachineID { get; set; }

        public static string ConfirmModalText { get; set; }


        public static TSFeedback currFeedback = new TSFeedback();
        public static TSReaction currReaction = new TSReaction();

        public static string oldFeedbackText { get; set; }



        public static bool UsingUITranslator { get; set; } = false;

        public static UITranslator uiTranslator { get; set; } = new UITranslator();


        public static bool UsingIndexedDb { get; set; } = false;

        public static IndexedDBManager indexedDbManager { get; set; } = null;


        public static List<TSUILanguageShortEx> UISupportedLanguagesList { get; set; } = null;
    } 
}
