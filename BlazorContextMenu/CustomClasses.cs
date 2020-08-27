using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorContextMenu
{
    public class CustomClasses
    {
        public class PointInt
        {
            public int X { get; set; }
            public int Y { get; set; }

            public PointInt(int pX = 0, int pY = 0)
            {
                X = pX;
                Y = pY;
            }
        }

        public class SizeInt
        {
            public int W { get; set; }
            public int H { get; set; }

            public SizeInt(int pW = 0, int pH = 0)
            {
                W = pW;
                H = pH;
            }
        }

    }
}
