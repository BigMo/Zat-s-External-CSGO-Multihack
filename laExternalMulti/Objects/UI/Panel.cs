using SharpDX;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace laExternalMulti.Objects.UI
{
    public class Panel : Control
    {
        #region VARIABLES
        #endregion

        #region PROPERTIES
        #endregion

        #region CONSTRUCTORS
        public Panel(Theme theme, TextFormat font, float x, float y, float width, float height)
            : base(theme, font, x, y, width, height) { }
        #endregion

        #region METHODS
        protected override void OnDraw(SharpDX.Direct2D1.WindowRenderTarget device)
        {
            if (Theme.ShadowColor != Color.Transparent)
                FillRectangle(device, Theme.ShadowColor, X, Y, Width, Height);
            if (Theme.BackColor != Color.Transparent)
                FillRectangle(device, Theme.BackColor, X, Y, Width, Height);
        }
        #endregion
    }
}
