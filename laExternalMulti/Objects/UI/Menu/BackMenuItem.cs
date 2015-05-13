using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace laExternalMulti.Objects.UI.Menu
{
    class BackMenuItem : MenuItem
    {
        #region CONSTRUCTOR
        public BackMenuItem(Theme theme, TextFormat font, float x, float y, float width, float height, Menu parentMenu)
            : base(theme, font, x, y, width, height, parentMenu, "< Back") { }
        public BackMenuItem(Theme theme, TextFormat font, float x, float y, Menu parentMenu)
            : base(theme, font, x, y, 20f, 100f, parentMenu, "< Back") { }
        public BackMenuItem(Theme theme, TextFormat font, Menu parentMenu)
            : this(theme, font, 0f, 0f, parentMenu) { }
        #endregion

        #region METHODS
        public override void OnKeyUp(System.Windows.Forms.Keys key)
        {
            if (key == System.Windows.Forms.Keys.Left)
            {
                if (ParentMenu != null)
                    if (ParentMenu.ParentMenu != null)
                        ParentMenu.ParentMenu.ChildMenu = null;
            }
        }
        protected override void OnDraw(SharpDX.Direct2D1.WindowRenderTarget device)
        {
            this.DrawRectangle(device, this.Theme.BorderColor, this.X, this.Y, this.Width, this.Height);
            this.FillRectangle(device, this.Theme.BackColor, this.X, this.Y, this.Width, this.Height);
            this.DrawText(device, this.Theme.ForeColor, this.X + 2f, this.Y + 2f, this.Width - 4f, this.Height - 4f, this.Text, this.Font);
        }
        public override void SwitchToVal(object value) { }
        #endregion
    }
}
