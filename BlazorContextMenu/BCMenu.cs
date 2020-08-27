using System;
using System.Collections.Generic;
using System.Text;
using static BlazorContextMenu.CustomClasses;

namespace BlazorContextMenu
{
    public class BCMenu
    {

        public int ID { get; set; } = -1;
        public string Name { get; set; }

        public PointInt Position { get; set; } = new PointInt();

        public SizeInt ActualSize { get; set; } = new SizeInt();
        
        public List<BCMenuItem> Children = new List<BCMenuItem>();

       
    }
}
