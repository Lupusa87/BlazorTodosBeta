using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorContextMenu
{
    public class BCMenu
    {

        public int ID { get; set; } = -1;
        public string Name { get; set; }

        public int Y { get; set; }
        public int X { get; set; }


        public int width { get; set; }
        public int height { get; set; }

        public List<BCMenuItem> Children = new List<BCMenuItem>();

       
    }
}
