﻿using BlazorTodos.Classes;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorTodos.Modals
{
    public partial class CompMessage
    {

        [Parameter] public string UniqueID { get; set; }


        protected override void OnParametersSet()
        {

            base.OnParametersSet();
        }

        public void refresh()
        {
            StateHasChanged();
        }
    }
}
