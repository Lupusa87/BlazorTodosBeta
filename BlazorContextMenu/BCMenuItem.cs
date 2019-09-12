using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorContextMenu
{
    public class BCMenuItem
    {
        public int ID { get; set; }
        public string Icon { get; set; }

        public string Text { get; set; }

        public List<BCMenuItem> Children { get; set; }
    }
}
