using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorTodos.Modals
{

    public partial class CompError
    {


        [Parameter] public string UniqueID { get; set; }


        public void refresh()
        {
            StateHasChanged();
        }
    }
}
