using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorCSS
{
    public class CssHelper
    {
        public BCss blazorCSS = new BCss();


        public string GetStyle(string selector)
        {
            return blazorCSS.GetStyle(selector);
        }

        public string GetStyleWithSelector(string selector)
        {
            return blazorCSS.GetStyleWithSelector(selector);
        }


        public string GetString(string StyleID)
        {
            return blazorCSS.ToString(StyleID);
        }

        public string GetBase64String()
        {
            return blazorCSS.ToBase64String();
        }
      
    }
}
