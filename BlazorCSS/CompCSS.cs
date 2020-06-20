using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCSS
{
    public class CompCSS : ComponentBase
    {

        bool EnabledRender = true;

        [Parameter]
        public string StyleID { get; set; }

        [Parameter]
        public string StyleData { get; set; }

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

            builder.OpenElement(k++, "style");
            builder.AddAttribute(k++, "id", StyleID);
            builder.AddContent(k++, StyleData);
            builder.CloseElement();
        }

    }
}
