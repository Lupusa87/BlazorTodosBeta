using BlazorTodos.Classes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorTodos.Components
{
    public partial class CompDesktopItem
    {
        [Parameter]
        public DesktopItem desktopItem { get; set; }


        public void CmdOnClick(MouseEventArgs e)
        {
            LocalFunctions.CmdNavigate(desktopItem.NavigateTo);
        }

    }
}
