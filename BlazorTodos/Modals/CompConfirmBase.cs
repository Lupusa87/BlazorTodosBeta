using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorTodos.Modals
{
    public class CompConfirmBase:ComponentBase
    {
        public void CmdConfirm()
        {
            LocalData.btModalConfirm.Close();
            LocalData.componentBridge.InvokeConfirm();
            
        }


    }
}
