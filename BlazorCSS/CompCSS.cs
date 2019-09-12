using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.RenderTree;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCSS
{
    public class CompCSS : ComponentBase
    {

        bool EnabledRender = true;



        protected override Task OnParametersSetAsync()
        {

            EnabledRender = true;

            return base.OnParametersSetAsync();
        }

        protected override bool ShouldRender()
        {
            return EnabledRender;
        }


        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {

            EnabledRender = false;

            base.BuildRenderTree(builder);

            int k = 0;

            CssHelper cssHelper = new CssHelper();

            builder.OpenElement(k++, "style");
            builder.AddAttribute(k++, "id", "bvgStyle1");
            builder.AddContent(k++, cssHelper.GetString("bvgStyle1"));
            builder.CloseElement();
        }

    }
}
