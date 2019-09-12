using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorTodos.Classes
{
    public class DesktopItem
    {
        public int ID { get; set; } = -1;

        public int Row { get; set; }
        public int Column { get; set; }


        public string Text { get; set; }
        public string Icon { get; set; }

        public bool IsSelected { get; set; }

        public string NavigateTo { get; set; }

    }
}
