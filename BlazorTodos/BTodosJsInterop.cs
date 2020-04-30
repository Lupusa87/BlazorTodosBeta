using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorTodos
{
    public class BTodosJsInterop
    {
        public static IJSRuntime jsRuntime;

        public static ValueTask<string> GetMachineID()
        {
            return jsRuntime.InvokeAsync<string>(
                "BTodosJSFunctions.GetMachineID");
        }

       
    }
}
