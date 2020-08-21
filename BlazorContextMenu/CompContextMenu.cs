using BlazorWindowHelper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static BlazorShared.Classes.CustomClasses;

namespace BlazorContextMenu
{
    public class CompContextMenu:ComponentBase, IDisposable
    {

        public Action<int> OnClick { get; set; }


        [Parameter]
        public BCMenu bcMenu { get; set; }

        private bool ShouldNormalizePosition;


        public void Refresh()
        {
            ShouldNormalizePosition = true;
            StateHasChanged();
        }



        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {

            base.BuildRenderTree(builder);

            if (bcMenu.Children.Any())
            {
                builder.OpenElement(1, "div");
                builder.AddAttribute(2, "id", "bcmenu" + bcMenu.ID);
                builder.AddAttribute(3, "class", "bContextMenu");

                builder.AddAttribute(4, "style", "font-size:1.7vh;padding:0.5vh;top:" +bcMenu.Position.Y+"px;"+"left:" + bcMenu.Position.X +"px");
               

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

        protected override void OnAfterRender(bool firstRender)
        {

            if (ShouldNormalizePosition)
            {
                NormalizeContextMenuPosition();
                ShouldNormalizePosition = false;
            }

            base.OnAfterRender(firstRender);
        }


        private async void NormalizeContextMenuPosition()
        {

            SizeInt WindowSize = new SizeInt
            {
                W = (int)await BWHJsInterop.GetWindowWidth(),
                H = (int)await BWHJsInterop.GetWindowHeight()
            };



            bcMenu.ActualSize.W = (int)(await BWHJsInterop.GetElementActualWidth("bcmenu" + bcMenu.ID)*1.1);
            BWHJsInterop.SetElementWidth("bcmenu" + bcMenu.ID, bcMenu.ActualSize.W);


            bcMenu.ActualSize.H = (int)await BWHJsInterop.GetElementActualHeight("bcmenu" + bcMenu.ID);


            if (bcMenu.Position.Y + bcMenu.ActualSize.H > WindowSize.H)
            {
                bcMenu.Position.Y = WindowSize.H - bcMenu.ActualSize.H - 10;
            }

            if (bcMenu.Position.X + bcMenu.ActualSize.W > WindowSize.W)
            {
                bcMenu.Position.X = WindowSize.W - bcMenu.ActualSize.W - 10;
            }

            BWHJsInterop.SetElementTop("bcmenu" + bcMenu.ID, bcMenu.Position.Y);
            BWHJsInterop.SetElementLeft("bcmenu" + bcMenu.ID, bcMenu.Position.X);

        }

        public void Dispose()
        {

        }

    }
}
