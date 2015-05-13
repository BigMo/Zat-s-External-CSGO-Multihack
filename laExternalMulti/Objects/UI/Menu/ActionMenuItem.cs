using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace laExternalMulti.Objects.UI.Menu
{
    public abstract class ActionMenuItem : MenuItem
    {
        #region PROPERTIES
        private bool autoCloseMenu;
        #endregion

        #region CONSTRUCTOR
        public ActionMenuItem(Theme theme, TextFormat font, float x, float y, float width, float height, Menu parentMenu, string text, bool autoCloseMenu) 
            : base(theme, font, x, y, width, height, parentMenu, text) 
        {
            this.autoCloseMenu = autoCloseMenu;
        }
        public ActionMenuItem(Theme theme, TextFormat font, float x, float y, float width, float height, Menu parentMenu, string text)
            : this(theme, font, x, y, width, height, parentMenu, text, true) { }
        #endregion

        #region METHODS
        public override void OnKeyUp(System.Windows.Forms.Keys key)
        {
            if (key == System.Windows.Forms.Keys.Right)
            {
                OnStartMethod();
                if (ParentMenu != null && autoCloseMenu)
                    if (ParentMenu.ParentMenu != null)
                        ParentMenu.ParentMenu.ChildMenu = null;
            }
        }

        protected abstract void OnStartMethod();
        protected override void OnDraw(SharpDX.Direct2D1.WindowRenderTarget device)
        {
            this.DrawRectangle(device, this.Theme.BorderColor, this.X, this.Y, this.Width, this.Height);
            this.FillRectangle(device, this.Theme.BackColor, this.X, this.Y, this.Width, this.Height);
            this.DrawText(device, this.Theme.ForeColor, this.X + 2f, this.Y + 2f, this.Width - 4f, this.Height - 4f, this.Text, FactoryManager.GetFont("smallSegoe"));
        }
        #endregion
    }
}
