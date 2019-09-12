using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorTodos.Modals
{

    public class CompErrorBase : ComponentBase
    {


        [Parameter] protected string UniqueID { get; set; }


        public void refresh()
        {
            StateHasChanged();
        }
    }
}
