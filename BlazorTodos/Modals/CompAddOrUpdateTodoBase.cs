using BlazorTodos.Helpers;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TodosShared;
using static BlazorTodos.Classes.Enums;

namespace BlazorTodos.Modals
{
    public class CompAddOrUpdateTodoBase : ComponentBase
    {

        [Parameter] public string UniqueID { get; set; }

        protected bool IsButtonDisabled { get; set; } = false;

        [Parameter] public string ButtonName { get; set; } = "Add todo";


        protected List<string> OptionsList { get; set; } = new List<string>();

        protected List<int> NumbersList { get; set; } = new List<int>();


        protected byte OptionsSelectedIndex { get; set; } = 0;

        protected int MinutesEnd { get; set; } = 60;

        protected int HoursEnd { get; set; } = 12;

        protected int DaysEnd { get; set; } = 30;


        protected override void OnInitialized()
        {

            Bootstrap();

            base.OnInitialized();
        }


        public void Bootstrap()
        {

            if (LocalData.AddOrUpdateMode)
            {
                ButtonName = "Add todo";
            }
            else
            {
                ButtonName = "Update todo";
            }

            AdjustReminder(LocalData.CurrTodo.DueDate);

        }


        public async Task CmdAddOrUpdateTodo()
        {

            IsButtonDisabled = true;
            StateHasChanged();

            if (LocalData.CurrTodo.HasDueDate)
            {
                if (LocalData.CurrTodo.DueDate <= LocalFunctions.ToLocalDate(DateTime.Now).AddMinutes(3))
                {
                  LocalFunctions.AddError("Due date should be minimum after 3 minute from now", MethodBase.GetCurrentMethod(), false, false);
                }
            }


            LocalFunctions.Validate(LocalData.CurrTodo);


            if (LocalFunctions.HasError())
            {
                LocalFunctions.DisplayErrors();
            }
            else
            {


                string a = string.Empty;

                if (LocalData.AddOrUpdateMode)
                {
                    LocalData.CurrTodo.UserID = LocalData.CurrTSUser.ID;
                    a = await WebApiFunctions.CmdAddTodo(LocalData.CurrTodo);
                }
                else
                {

                   

                    if (!GlobalFunctions.AreEqualTwoObjects(LocalData.CurrTodo, LocalData.BeforeUpdateTodo))
                    {
                        a = await WebApiFunctions.CmdUpdateTodo(LocalData.CurrTodo);

                    }
                    else
                    {
                        LocalFunctions.AddMessage("Todo properties not updated", true, false);
                        return;
                    }
                }

                if (a.Equals("OK"))
                {
                    LocalData.TsTodosList = new List<TSTodoEx>();

                    LocalData.btModal.Close();

                    LocalData.EventConsumerName = "TodosPage";
                    LocalData.componentBridge.InvokeRefresh();

                }
                else
                {
                    LocalFunctions.AddError(a, MethodBase.GetCurrentMethod(), true, false);

                }

            }

            IsButtonDisabled = false;

        }


        public void ComboPrioritySelectionChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value.ToString(), out int val))
            {
                LocalData.CurrTodo.Priority = val;
            }

        }


        public void ComboCategorySelectionChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value.ToString(), out int val))
            {
                LocalData.CurrTodo.CategoryID = LocalData.TSCategoriesList[val].ID;
            }

        }


        public void Refresh()
        {
            StateHasChanged();
        }

        protected void AdjustReminder(DateTime dueDate)
        {
 
            TimeSpan ts = dueDate - LocalFunctions.ToLocalDate(DateTime.Now);
            MinutesEnd = 60;
            HoursEnd = 12;
            DaysEnd = 30;


            NumbersList = new List<int>();
            OptionsList = new List<string>();

            if ((int)ts.TotalSeconds > 0)
            {
                OptionsList.Add("Minutes");
            }


            if ((int)ts.TotalMinutes < 60)
            {
                MinutesEnd = (int)ts.TotalMinutes - 1;
            }


            if ((int)ts.TotalHours > 0)
            {
                OptionsList.Add("Hours");


                if ((int)ts.TotalHours < 12)
                {
                    HoursEnd = (int)ts.TotalHours - 1;
                }
            }

            if ((int)ts.TotalDays > 0)
            {
                OptionsList.Add("Days");


                if ((int)ts.TotalDays < 30)
                {
                    DaysEnd = (int)ts.TotalDays - 1;
                }
            }



        }



        public void ComboOptionsSelectionChanged(ChangeEventArgs e)
        {
            NumbersList = new List<int>();

            if (byte.TryParse(e.Value.ToString(), out byte index))
            {
                OptionsSelectedIndex = index;
                switch (OptionsSelectedIndex)
                {
                    case 0: //minute
                        AddNumber(1, MinutesEnd);
                        AddNumber(2, MinutesEnd);
                        AddNumber(3, MinutesEnd);
                        AddNumber(4, MinutesEnd);
                        for (int i = 5; i <= MinutesEnd; i += 5)
                        {
                            NumbersList.Add(i);
                        }
                        break;
                    case 1: //hour
                        for (int i = 1; i <= HoursEnd; i++)
                        {
                            NumbersList.Add(i);
                        }
                        break;
                    case 2: //day
                        AddNumber(1, DaysEnd);
                        AddNumber(2, DaysEnd);
                        AddNumber(3, DaysEnd);
                        AddNumber(4, DaysEnd);
                        for (int i = 5; i <= DaysEnd; i += 5)
                        {
                            NumbersList.Add(i);
                        }
                        break;
                    default:
                        break;
                }
            }

            LocalData.CurrTodo.RemindDate = LocalFunctions.ToLocalDate(DateTime.Now);


            StateHasChanged();
        }

        private void AddNumber(int val, int limiter)
        {
            if (limiter >= val)
            {
                NumbersList.Add(val);
            }
        }

        public void ComboNumbersSelectionChanged(ChangeEventArgs e)
        {
            
            if (byte.TryParse(e.Value.ToString(), out byte val))
            {
                switch (OptionsSelectedIndex)
                {
                    case 0: //minute
                        LocalData.CurrTodo.RemindDate = LocalData.CurrTodo.DueDate.AddMinutes(-NumbersList[val]);
                        break;
                    case 1: //hour
                        LocalData.CurrTodo.RemindDate = LocalData.CurrTodo.DueDate.AddHours(-NumbersList[val]);
                        break;
                    case 2: //day
                        LocalData.CurrTodo.RemindDate = LocalData.CurrTodo.DueDate.AddDays(-NumbersList[val]);
                        break;
                    default:
                        break;
                }
            }


            StateHasChanged();
        }

      

        public void CmdDueDateOnChange(ChangeEventArgs e)
        {
            DateTime d;
            if (DateTime.TryParse(e.Value.ToString(), out d))
            {
                AdjustReminder(d);
            }

        }


        public void CmdAddCategory()
        {
            LocalFunctions.CmdNavigate("CategoriesPage");
            LocalData.btModal.Close();
        }
    }
}
