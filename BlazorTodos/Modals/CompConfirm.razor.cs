using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorTodos.Modals
{
    public partial class CompConfirm
    {
        public void CmdConfirm()
        {
            LocalData.btModalConfirm.Close();
            LocalData.componentBridge.InvokeConfirm();
            
        }


    }
}
