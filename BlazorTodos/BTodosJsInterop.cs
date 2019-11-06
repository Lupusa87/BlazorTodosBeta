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
        
        public static ValueTask<bool> Alert(string msg)
        {

            return jsRuntime.InvokeAsync<bool>(
                "BTodosJSFunctions.Alert", msg);
        }

        public static ValueTask<bool> Log(string msg)
        {

            return jsRuntime.InvokeAsync<bool>(
                "BTodosJSFunctions.Log", msg);
        }



        public static ValueTask<int> GetTimezoneOffset()
        {
            return jsRuntime.InvokeAsync<int>(
                "BTodosJSFunctions.GetTimezoneOffset");
        }

        public static ValueTask<long> GetDateMilliseconds()
        {
            return jsRuntime.InvokeAsync<long>(
                "BTodosJSFunctions.GetDateMilliseconds");
        }


        public static ValueTask<string> GetMachineID()
        {
            return jsRuntime.InvokeAsync<string>(
                "BTodosJSFunctions.GetMachineID");
        }

        public static ValueTask<double> GetElementActualWidth(string elementID)
        {
            return jsRuntime.InvokeAsync<double>(
                "BTodosJSFunctions.GetElementActualWidth", elementID);
        }

        public static ValueTask<double> GetElementActualHeight(string elementID)
        {
            return jsRuntime.InvokeAsync<double>(
                "BTodosJSFunctions.GetElementActualHeight", elementID);
        }

        public static ValueTask<double> GetElementActualTop(string elementID)
        {
            return jsRuntime.InvokeAsync<double>(
                "BTodosJSFunctions.GetElementActualTop", elementID);
        }

        public static ValueTask<double> GetElementActualLeft(string elementID)
        {
            return jsRuntime.InvokeAsync<double>(
                "BTodosJSFunctions.GetElementActualLeft", elementID);
        }


        public static ValueTask<double> GetWindowWidth()
        {
            return jsRuntime.InvokeAsync<double>(
                "BTodosJSFunctions.GetWindowWidth");
        }

        public static ValueTask<double> GetWindowHeight()
        {
            return jsRuntime.InvokeAsync<double>(
                "BTodosJSFunctions.GetWindowHeight");
        }

    }
}
