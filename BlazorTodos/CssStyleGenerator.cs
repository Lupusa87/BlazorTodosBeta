using BlazorCSS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorTodos
{
    public static class CssStyleGenerator
    {

        static CssHelper CssHelper1 = new CssHelper();

        static string StyleID = string.Empty;

        static BCssItem c = new BCssItem(string.Empty, string.Empty);


        public static void Reset()
        {
            CssHelper1 = new CssHelper();
        }


        public static string GenerateAppCSS(string PStyleID)
        {
            StyleID = PStyleID;
            Reset();


            GenerateGlobalCSS();

            GenerateModalCSS();

            GenerateContextMenuCSS();


            Console.WriteLine(CssHelper1.GetString(StyleID));

            return CssHelper1.GetString(StyleID);
        }


        private static void GenerateModalCSS()
        {

            c = new BCssItem(".bm-container", StyleID);
            c.Values.Add("display", "none");
            c.Values.Add("align-items", "center");
            c.Values.Add("justify-content", "center");
            c.Values.Add("position", "fixed");
            c.Values.Add("top", "0");
            c.Values.Add("left", "0");
            c.Values.Add("width", "100%");
            c.Values.Add("height", "100%");
            c.Values.Add("z-index", "90");
            CssHelper1.blazorCSS.Children.Add(c);


            c = new BCssItem(".bm-overlay", StyleID);
            c.Values.Add("display", "block");
            c.Values.Add("position", "fixed");
            c.Values.Add("width", "100%");
            c.Values.Add("height", "100%");
            c.Values.Add("z-index", "95");
            c.Values.Add("background-color", "rgba(0,0,0,0.25)");
            CssHelper1.blazorCSS.Children.Add(c);


            c = new BCssItem(".bm-active", StyleID);
            c.Values.Add("display", "flex");
            CssHelper1.blazorCSS.Children.Add(c);


            c = new BCssItem(".blazor-modal", StyleID);
            c.Values.Add("display", "flex");
            c.Values.Add("flex-direction", "column");
            c.Values.Add("width", "auto");
            c.Values.Add("background-color", "#f2f2f2");
            c.Values.Add("border-radius", "10px");
            c.Values.Add("border", "1px solid black");
            c.Values.Add("padding", "0");
            c.Values.Add("z-index", "99");
            CssHelper1.blazorCSS.Children.Add(c);


            c = new BCssItem(".bm-header", StyleID);
            c.Values.Add("display", "flex");
            c.Values.Add("border-top-left-radius", "10px");
            c.Values.Add("border-top-right-radius", "10px");
            // c.Values.Add("border", "1px solid black");
            c.Values.Add("height", "60px");
            c.Values.Add("align-items", "flex-start");
            c.Values.Add("justify-content", "space-between");
            c.Values.Add("padding", "0");
            c.Values.Add("background-color", "lightgray");
            CssHelper1.blazorCSS.Children.Add(c);


            c = new BCssItem(".bm-title", StyleID);
            c.Values.Add("margin", "1rem");
            c.Values.Add("font-size", "1.5rem");
            CssHelper1.blazorCSS.Children.Add(c);


            c = new BCssItem(".bm-close", StyleID);
            c.Values.Add("line-height", "0");
            c.Values.Add("padding", "1rem");
            c.Values.Add("margin", "1rem");
            c.Values.Add("margin-right", "0");
            c.Values.Add("font-size", "2rem");
            c.Values.Add("background-color", "transparent");
            c.Values.Add("border", "0");
            c.Values.Add("-webkit-appearance", "none");
            c.Values.Add("cursor", "pointer");
            CssHelper1.blazorCSS.Children.Add(c);


        }


        private static void GenerateContextMenuCSS()
        {

            c = new BCssItem(".bContextMenu", StyleID);
            c.Values.Add("position", "fixed");
            c.Values.Add("-webkit-user-select", "none");
            c.Values.Add("-moz-user-select", "none");
            c.Values.Add("-ms-user-select", "none");
            c.Values.Add("user-select", "none");
            c.Values.Add("background-color", "#fff");
            c.Values.Add("-webkit-box-sizing", "border-box");
            c.Values.Add("box-sizing", "border-box");
            c.Values.Add("-webkit-box-shadow", "0 10px 20px rgba(0,0,0,.3), 0 0 0 1px #eee");
            c.Values.Add("box-shadow", "0 10px 20px rgba(0,0,0,.3), 0 0 0 1px #eee");
            c.Values.Add("padding", "5px 0");

            c.Values.Add("z-index", "90");
            CssHelper1.blazorCSS.Children.Add(c);


            c = new BCssItem(".bContextMenuItem", StyleID);
            c.Values.Add("min-width", "100px");
            c.Values.Add("padding", "6px");
            c.Values.Add("text-align", "left");
            c.Values.Add("white-space", "nowrap");
            c.Values.Add("position", "relative");
            c.Values.Add("cursor", "pointer");
            c.Values.Add("z-index", "95");
            //c.Values.Add("background-color", "rgba(0,0,0,0.5)");
            CssHelper1.blazorCSS.Children.Add(c);



            c = new BCssItem(".bContextMenuItem:hover", StyleID);
            c.Values.Add("background-color", "rgba(0, 0, 0, .05)");
            CssHelper1.blazorCSS.Children.Add(c);



            //c  = new BCssItem(".bm-active", StyleID);
            //c.Values.Add("display", "flex");
            //CssHelper1.blazorCSS.Children.Add(c);


            //c  = new BCssItem(".blazor-modal", StyleID);
            //c.Values.Add("display", "flex");
            //c.Values.Add("flex-direction", "column");
            //c.Values.Add("width", "auto");
            //c.Values.Add("background-color", "#f2f2f2");
            //c.Values.Add("border-radius", "10px");
            //c.Values.Add("border", "1px solid black");
            //c.Values.Add("padding", "0");
            //c.Values.Add("z-index", "99");
            //CssHelper1.blazorCSS.Children.Add(c);


            //c  = new BCssItem(".bm-header", StyleID);
            //c.Values.Add("display", "flex");
            //c.Values.Add("border-top-left-radius", "10px");
            //c.Values.Add("border-top-right-radius", "10px");
            //// c.Values.Add("border", "1px solid black");
            //c.Values.Add("height", "60px");
            //c.Values.Add("align-items", "flex-start");
            //c.Values.Add("justify-content", "space-between");
            //c.Values.Add("padding", "0");
            //c.Values.Add("background-color", "lightgray");
            //CssHelper1.blazorCSS.Children.Add(c);


            //c  = new BCssItem(".bm-title", StyleID);
            //c.Values.Add("margin", "1rem");
            //c.Values.Add("font-size", "1.5rem");
            //CssHelper1.blazorCSS.Children.Add(c);


            //c  = new BCssItem(".bm-close", StyleID);
            //c.Values.Add("line-height", "0");
            //c.Values.Add("padding", "1rem");
            //c.Values.Add("margin", "1rem");
            //c.Values.Add("margin-right", "0");
            //c.Values.Add("font-size", "2rem");
            //c.Values.Add("background-color", "transparent");
            //c.Values.Add("border", "0");
            //c.Values.Add("-webkit-appearance", "none");
            //c.Values.Add("cursor", "pointer");
            //CssHelper1.blazorCSS.Children.Add(c);


        }

        private static void GenerateGlobalCSS()
        {
            c = new BCssItem("html", StyleID);
            c.Values.Add("box-sizing", "border-box");
            CssHelper1.blazorCSS.Children.Add(c);

            c = new BCssItem("*, *:before, *:after", StyleID);
            c.Values.Add("box-sizing", "inherit");
            c.Values.Add("padding", "0");
            c.Values.Add("margin", "0");
            CssHelper1.blazorCSS.Children.Add(c);

            c = new BCssItem("body", StyleID);
            // c.Values.Add("line-height", "0");
            c.Values.Add("padding", "0");
            c.Values.Add("margin", "0");
            CssHelper1.blazorCSS.Children.Add(c);


            c = new BCssItem("select:-moz-focusring", StyleID);
            c.Values.Add("color", "transparent");
            c.Values.Add("text-shadow", "0 0 0 #000");
            CssHelper1.blazorCSS.Children.Add(c);



        }
    }
}
