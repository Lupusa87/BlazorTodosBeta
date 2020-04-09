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
    public partial class CompAddOrUpdateTodo
    {

        [Parameter] public string UniqueID { get; set; }

        protected bool IsButtonDisabled { get; set; } = false;

        [Parameter] public string ButtonName { get; set; } = "Add todo";


        protected List<string> OptionsList { get; set; } = new List<string>();

        protected List<int> NumbersList { get; set; } = new List<int>();


        protected sbyte OptionsSelectedIndex { get; set; } = 0;

        protected int MinutesEnd { get; set; } = 60;

        protected int HoursEnd { get; set; } = 12;

        protected int DaysEnd { get; set; } = 30;



        protected int CurrComboReminderIndex = 0;

        protected DateTime CurrRemindInputDate = LocalFunctions.ToLocalDate(DateTime.Now).AddDays(7).AddMinutes(-5);

        protected override void OnInitialized()
        {

            Bootstrap();

            base.OnInitialized();
        }


        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                AdjustOptions();
            }

            base.OnAfterRender(firstRender);
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
        }


        public async Task CmdAddOrUpdateTodo()
        {

            IsButtonDisabled = true;
            StateHasChanged();


            if (LocalData.CurrTodo.IsDone)
            {
                LocalData.CurrTodo.HasDueDate = false;
                LocalData.CurrTodo.Reminders = new List<DateTime>();
            }

            if (LocalData.CurrTodo.HasDueDate)
            {
                if (LocalData.CurrTodo.DueDate <= LocalFunctions.ToLocalDate(DateTime.Now).AddMinutes(3))
                {
                  LocalFunctions.AddError("Due date should be minimum after 3 minute from now", MethodBase.GetCurrentMethod(), false, false);
                }
            }
            else
            {
                LocalData.CurrTodo.Reminders = new List<DateTime>();
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
                        if (!GlobalFunctions.AreEqualTwoLists(LocalData.CurrTodo.Reminders, LocalData.BeforeUpdateTodo.Reminders))
                        {
                            a = await WebApiFunctions.CmdUpdateTodo(LocalData.CurrTodo);
                        }
                        else
                        { 
                            LocalFunctions.AddMessage("Todo properties not updated", true, false);
                            IsButtonDisabled = false;
                            StateHasChanged();
                            return;
                        }
                    }
                }

                if (a.Equals("OK"))
                {

                    LocalData.CurrTodo = new TSTodo();

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

        public void ComboReminderSelectionChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value.ToString(), out int val))
            {
                CurrComboReminderIndex = val;
            }

        }

        public void CmdRemoveReminder()
        {
            if (LocalData.CurrTodo.Reminders.Count() > CurrComboReminderIndex)
            {
                LocalData.CurrTodo.Reminders.RemoveAt(CurrComboReminderIndex);
                CurrComboReminderIndex = 0;
            }
        }


        public void CmdAddReminder()
        {
            if (LocalData.CurrTodo.Reminders.Count == 12)
            {
                LocalFunctions.AddError("You can set maximum 12 reminders!", MethodBase.GetCurrentMethod(), true, false);
            }


            if (LocalData.CurrTodo.Reminders.Any(x => x.Equals(CurrRemindInputDate)))
            {
                LocalFunctions.AddError("There is already reminder for same datetime!", MethodBase.GetCurrentMethod(), true, false);
            }


            if (CurrRemindInputDate < LocalFunctions.ToLocalDate(DateTime.Now).AddMinutes(3))
            {
                LocalFunctions.AddError("Remainder date should be future date minimum 3 minutes from now!", MethodBase.GetCurrentMethod(), true, false);
            }


            if (CurrRemindInputDate > LocalData.CurrTodo.DueDate.AddMinutes(-3))
            {
                LocalFunctions.AddError("Remainder date should be before Due datetime minimum 3 minute!", MethodBase.GetCurrentMethod(), true, false);
            }

            if (LocalFunctions.HasError())
            {
                LocalFunctions.DisplayErrors();
            }
            else
            {

                LocalData.CurrTodo.Reminders.Add(CurrRemindInputDate);

                LocalData.CurrTodo.Reminders = LocalData.CurrTodo.Reminders.OrderBy(x => x).ToList();

                CurrComboReminderIndex = LocalData.CurrTodo.Reminders.IndexOf(CurrRemindInputDate);

                StateHasChanged();
            }

        }



        protected void AdjustOptions()
        {
 
            TimeSpan ts = LocalData.CurrTodo.DueDate - LocalFunctions.ToLocalDate(DateTime.Now);
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


            if (OptionsList.Any())
            {
                AdjustNumbers(0);
            }
        }
       

        public string GetReminderDateString(DateTime dt)
        {
            TimeSpan ts = LocalData.CurrTodo.DueDate - dt;
            return dt.ToString("MM/dd/yyyy HH:mm:ss") + " before " + ts.ToString(@"dd\.hh\:mm\:ss");
        }

        public void ComboOptionsSelectionChanged(ChangeEventArgs e)
        {
            NumbersList = new List<int>();

            if (sbyte.TryParse(e.Value.ToString(), out sbyte index))
            {
                AdjustNumbers(index);
            }

        }

        public void AdjustNumbers(sbyte index)
        {
            NumbersList = new List<int>();

            if (index>-1)
            {
                OptionsSelectedIndex = index;
                switch (OptionsSelectedIndex)
                {
                    case 0: //minute
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


                if (NumbersList.Any())
                {
                    ApplyToReminderDate(0);
                    StateHasChanged();
                }

            }
            
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
            
            if (sbyte.TryParse(e.Value.ToString(), out sbyte val))
            {
                ApplyToReminderDate(val);
            }

        }


        public void ApplyToReminderDate(sbyte index)
        {

            if (index>-1)
            {
                switch (OptionsSelectedIndex)
                {
                    case 0: //minute
                        CurrRemindInputDate = LocalData.CurrTodo.DueDate.AddMinutes(-NumbersList[index]);
                        break;
                    case 1: //hour
                        CurrRemindInputDate = LocalData.CurrTodo.DueDate.AddHours(-NumbersList[index]);
                        break;
                    case 2: //day
                        CurrRemindInputDate = LocalData.CurrTodo.DueDate.AddDays(-NumbersList[index]);
                        break;
                    default:
                        break;
                }
            }

        }


        public void CmdDueDateOnChange(ChangeEventArgs e)
        {
            DateTime d;
            if (DateTime.TryParse(e.Value.ToString(), out d))
            {
                AdjustOptions();
            }

        }


        public void CmdAddCategory()
        {
            LocalFunctions.CmdNavigate("CategoriesPage");
            LocalData.btModal.Close();
        }
    }
}
