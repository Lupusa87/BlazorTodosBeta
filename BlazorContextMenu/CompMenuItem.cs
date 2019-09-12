using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.RenderTree;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorContextMenu
{
    internal class CompMenuItem:ComponentBase
    {
        [Parameter]
        protected ComponentBase parent { get; set; }

        [Parameter] protected BCMenuItem bcMenuItem { get; set; }


        private CompContextMenu _parent;


        protected override void OnInitialized()
        {

            _parent = parent as CompContextMenu;

            base.OnInitialized();
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);



            builder.OpenElement(1, "div");
            builder.AddAttribute(2, "id", bcMenuItem.ID);
            builder.AddAttribute(3, "class", "bContextMenuItem");
            builder.AddAttribute(4, "style", "height:30px");
            builder.AddAttribute(5, "onclick", EventCallback.Factory.Create<UIMouseEventArgs>(this, OnClick));
            builder.AddContent(6, bcMenuItem.Text);

            builder.CloseElement();

        }

        private void OnClick(UIMouseEventArgs e)
        {
            if (bcMenuItem.Children is null)
            {
             
                _parent.OnClick?.Invoke(bcMenuItem.ID);
            }

        }

    }
}
