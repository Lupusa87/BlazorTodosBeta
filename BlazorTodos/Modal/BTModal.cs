using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static BlazorTodos.Classes.Enums;

namespace BlazorTodos.Modal
{
    public class BTModal
    {

        public bool IsDisplayed { get; set; } = false;
        public ModalForm modalForm { get; set; }
        public Action OnShow;
        public Action OnClose;
        public Action OnRefresh;

        public void Show(ModalForm _modalForm)
        {
            modalForm = _modalForm;
            OnShow?.Invoke();
        }

        public void Close()
        {
            OnClose?.Invoke();
        }

        public void Refresh()
        {
            OnRefresh?.Invoke();
        }
    }
}
