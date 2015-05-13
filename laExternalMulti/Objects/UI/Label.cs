using SharpDX;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace laExternalMulti.Objects.UI
{
    public class Label : Control
    {
        #region VARIABLES
        private HorizontalAlign align;
        private bool castShadow;
        public enum HorizontalAlign { Left, Center, Right };
        #endregion

        #region PROPERTIES
        public bool CastShadow { get { return castShadow; } set { castShadow = value; } }
        #endregion

        #region CONSTRUCTORS
        public Label(Theme theme, TextFormat font, float x, float y, float width, float height,  string text)
            : base(theme, font, x, y, width, height)
        {
            this.Text = text;
            this.align = HorizontalAlign.Left;
            this.castShadow = false;
        }
        #endregion

        #region METHODS
        protected override void OnDraw(SharpDX.Direct2D1.WindowRenderTarget device)
        {
            float x = this.X;
            RectangleF rect = (AutoSize ? MeasureString(this.Font, this.Text) : Rectangle);
            if (this.align == HorizontalAlign.Center)
                x = this.X - rect.Width / 2f;
            if (this.align == HorizontalAlign.Right)
                x = this.X - rect.Width;
            if (!castShadow)
                DrawText(device, this.Theme.ForeColor, x, this.Y, this.Width, this.Height, this.Text, this.Font);
            else
                DrawText(device, this.Theme.ForeColor, this.Theme.ShadowColor, this.X, this.Y, this.Width, this.Height, this.Theme.ShadowOffsetX, this.Theme.ShadowOffsetY, this.Text, this.Font);
        }

        protected override void OnAutoSize()
        {
            base.OnAutoSize();
            RectangleF rect = MeasureString(this.Font, this.Text);
            this.Width = rect.Width;
            this.Height = rect.Height;
        }
        #endregion
    }
}
