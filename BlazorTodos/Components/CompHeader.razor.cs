using BlazorWindowHelper;
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
    public partial class CompHeader
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

                p.Y = (int)(await BWHJsInterop.GetElementActualTop(ElementID));
                p.X = (int)(await BWHJsInterop.GetElementActualLeft(ElementID));

                s.W = (int)(await BWHJsInterop.GetElementActualWidth(ElementID));
                s.H = (int)(await BWHJsInterop.GetElementActualHeight(ElementID));

                p.Y += s.H;

                LocalFunctions.ContextMenu_DisplayLogout(p);

                
            }
        }
    }
}
