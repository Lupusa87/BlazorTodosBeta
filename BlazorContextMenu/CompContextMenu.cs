using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.RenderTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazorContextMenu
{
    public class CompContextMenu:ComponentBase, IDisposable
    {

        public Action<int> OnClick { get; set; }


        [Parameter]
        public BCMenu bcMenu { get; set; }



       

        public void Refresh()
        {
            StateHasChanged();
        }



        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {

            base.BuildRenderTree(builder);

            if (bcMenu.Children.Any())
            {
                builder.OpenElement(1, "div");
                builder.AddAttribute(2, "id", bcMenu.ID);
                builder.AddAttribute(3, "class", "bContextMenu");

                builder.AddAttribute(4, "style", "width:" + bcMenu.width + "px;top:" +bcMenu.Y+"px;"+"left:" + bcMenu.X+"px");
               

                foreach (var item in bcMenu.Children)
                {
                    builder.OpenComponent<CompMenuItem>(4);
                    builder.AddAttribute(5, "parent", this);
                    builder.AddAttribute(6, "bcMenuItem", item);
                    builder.CloseComponent();
                }

                builder.CloseElement();
            }
        }


        public void Dispose()
        {

        }

    }
}
