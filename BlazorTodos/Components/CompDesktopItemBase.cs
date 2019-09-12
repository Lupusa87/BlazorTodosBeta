using BlazorTodos.Classes;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorTodos.Components
{
    public class CompDesktopItemBase:ComponentBase
    {
        [Parameter]
        protected DesktopItem desktopItem { get; set; }


        public void CmdOnClick(UIMouseEventArgs e)
        {
            LocalFunctions.CmdNavigate(desktopItem.NavigateTo);
        }

    }
}
