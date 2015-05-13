using laExternalMulti.Objects.UI.Events;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace laExternalMulti.Objects.UI.Menu
{
    class SubMenuItem : MenuItem
    {
        #region VARIABLES
        private Menu childMenu;
        #endregion

        #region PROPERTIES
        public Menu ChildMenu { get { return childMenu; } }
        #endregion

        #region CONSTRUCTOR
        public SubMenuItem(Theme theme, TextFormat font, float x, float y, float width, float height, Menu parentMenu, string text, string extraData, Menu childMenu)
            : base(theme, font, x, y, width, height, parentMenu, text, extraData)
        {
            this.childMenu = childMenu;
        }
        public SubMenuItem(Theme theme, TextFormat font, float x, float y, float width, float height, Menu parentMenu, string text, Menu childMenu)
            : this(theme, font, x, y, width, height, parentMenu, text, "", childMenu) { }
        public SubMenuItem(Theme theme, TextFormat font, float x, float y, float width, float height, Menu parentMenu, Menu childMenu)
            : this(theme, font, x, y, width, height, parentMenu, "SubMenuItem", "", childMenu) { }
        public SubMenuItem(Theme theme, TextFormat font, float x, float y, Menu parentMenu, Menu childMenu)
            : this(theme, font, x, y, 100f, 20f, parentMenu, childMenu) { }
        public SubMenuItem(Theme theme, TextFormat font, Menu parentMenu, Menu childMenu)
            : this(theme, font, 0f, 0f, parentMenu, childMenu) { }
        public SubMenuItem(Theme theme, TextFormat font, Menu parentMenu, string text, Menu childMenu)
            : this(theme, font, 0f, 0f, 100f, 20f, parentMenu, text, "", childMenu) { }
        #endregion

        #region METHODS
        public override void OnKeyUp(System.Windows.Forms.Keys key)
        {
            if (key == System.Windows.Forms.Keys.Left || key == System.Windows.Forms.Keys.Right)
            {
                if (ParentMenu != null)
                {
                    if (ParentMenu.ChildMenu == null)
                    {
                        ParentMenu.ChildMenu = ChildMenu;
                        ChildMenu.ParentMenu = ParentMenu;
                        ChildMenu.SetPosition(ParentMenu.X + ParentMenu.Width + ParentMenu.PaddingY, this.Y);
                    }
                    else
                        ParentMenu.ChildMenu = null;
                }
            }
        }
        protected override void OnDraw(SharpDX.Direct2D1.WindowRenderTarget device)
        {
            this.DrawRectangle(device, this.Theme.BorderColor, this.X, this.Y, this.Width, this.Height);
            this.FillRectangle(device, this.Theme.BackColor, this.X, this.Y, this.Width, this.Height);
            string text = this.Text;
            if (ParentMenu != null)
            {
                if (ParentMenu.ChildMenu == null)
                    text += " >";
                else if(ParentMenu.ChildMenu == ChildMenu)
                    text = "< " + text;
            }
            this.DrawText(device, this.Theme.ForeColor, this.X + 2f, this.Y + 2f, this.Width - 4f, this.Height - 4f, text, this.Font);
        }
        public override void SwitchToVal(object value) { }
        #endregion
    }
}
