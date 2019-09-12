using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorTodos.Classes
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

        public class PointDouble
        {
            private double _X { get; set; }

            public double X
            {
                get
                {
                    return _X;
                }
                set
                {
                    _X = Math.Round(value, 3);
                }
            }

            private double _Y { get; set; }

            public double Y
            {
                get
                {
                    return _Y;
                }
                set
                {
                    _Y = Math.Round(value, 3);
                }
            }

            public PointDouble(double pX = 0, double pY = 0)
            {
                _X = Math.Round(pX, 3);
                _Y = Math.Round(pY, 3);
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

        public class SizeDouble
        {
            private double _W { get; set; }

            public double W
            {
                get
                {
                    return _W;
                }
                set
                {
                    _W = Math.Round(value, 3);
                }
            }

            private double _H { get; set; }

            public double H
            {
                get
                {
                    return _H;
                }
                set
                {
                    _H = Math.Round(value, 3);
                }
            }


            public SizeDouble(double pW = 0, double pH = 0)
            {
                _W = Math.Round(pW, 3);
                _H = Math.Round(pH, 3);
            }
        }

    }
}
