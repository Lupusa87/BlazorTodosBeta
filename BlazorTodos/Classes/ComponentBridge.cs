using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorTodos.Classes
{
    public class ComponentBridge
    {
        public Action OnRefresh;
        public Action OnConfirm;

        public void InvokeRefresh()
        {
             OnRefresh?.Invoke();
        }

        public void InvokeConfirm()
        {

            OnConfirm?.Invoke();
        }
    }
}
