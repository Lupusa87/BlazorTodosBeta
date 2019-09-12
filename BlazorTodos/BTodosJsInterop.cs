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
        
        public static Task<bool> Alert(string msg)
        {

            return jsRuntime.InvokeAsync<bool>(
                "BTodosJSFunctions.Alert", msg);
        }

        public static Task<bool> Log(string msg)
        {

            return jsRuntime.InvokeAsync<bool>(
                "BTodosJSFunctions.Log", msg);
        }



        public static Task<int> GetTimezoneOffset()
        {
            return jsRuntime.InvokeAsync<int>(
                "BTodosJSFunctions.GetTimezoneOffset");
        }

        public static Task<long> GetDateMilliseconds()
        {
            return jsRuntime.InvokeAsync<long>(
                "BTodosJSFunctions.GetDateMilliseconds");
        }


        public static Task<string> GetMachineID()
        {
            return jsRuntime.InvokeAsync<string>(
                "BTodosJSFunctions.GetMachineID");
        }

        public static Task<double> GetElementActualWidth(string elementID)
        {
            return jsRuntime.InvokeAsync<double>(
                "BTodosJSFunctions.GetElementActualWidth", elementID);
        }

        public static Task<double> GetElementActualHeight(string elementID)
        {
            return jsRuntime.InvokeAsync<double>(
                "BTodosJSFunctions.GetElementActualHeight", elementID);
        }

        public static Task<double> GetElementActualTop(string elementID)
        {
            return jsRuntime.InvokeAsync<double>(
                "BTodosJSFunctions.GetElementActualTop", elementID);
        }

        public static Task<double> GetElementActualLeft(string elementID)
        {
            return jsRuntime.InvokeAsync<double>(
                "BTodosJSFunctions.GetElementActualLeft", elementID);
        }


        public static Task<double> GetWindowWidth()
        {
            return jsRuntime.InvokeAsync<double>(
                "BTodosJSFunctions.GetWindowWidth");
        }

        public static Task<double> GetWindowHeight()
        {
            return jsRuntime.InvokeAsync<double>(
                "BTodosJSFunctions.GetWindowHeight");
        }

    }
}
