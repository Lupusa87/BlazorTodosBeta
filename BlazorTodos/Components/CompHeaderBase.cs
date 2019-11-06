using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static BlazorTodos.Classes.CustomClasses;
using static BlazorTodos.Classes.Enums;


namespace BlazorTodos.Components
{
    public class CompHeaderBase : LayoutComponentBase
    {


        public void Refresh()
        {
            StateHasChanged();
        }


        public async void CmdLoginLogout(MouseEventArgs e, string ElementID)
        {

            if (LocalData.LoginLogout.Equals("Login"))
            {
                LocalFunctions.DisplayModal(ModalForm.Login);

            }
            else
            {

                SizeInt s = new SizeInt();
                PointInt p = new PointInt();

                p.Y = (int)(await BTodosJsInterop.GetElementActualTop(ElementID));
                p.X = (int)(await BTodosJsInterop.GetElementActualLeft(ElementID));

                s.W = (int)(await BTodosJsInterop.GetElementActualWidth(ElementID));
                s.H = (int)(await BTodosJsInterop.GetElementActualHeight(ElementID));

                p.Y += s.H;

                LocalFunctions.ContextMenu_DisplayLogout(p);

                
            }
        }
    }
}
