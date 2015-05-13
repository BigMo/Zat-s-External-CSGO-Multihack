using laExternalMulti.Objects.UI;
using laExternalMulti.Objects.UI.Menu;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace laExternalMulti.Objects.Implementation.CSGO.UI
{
    public class PlayerListMenu : Menu
    {
        #region CONSTRUCTOR
        public PlayerListMenu(Theme theme, Theme themeSelected, Theme themeUnselected, TextFormat font, TextFormat fontCaption, float x, float y, float width, float height, string caption, float paddingX, float paddingY, float spacingY, bool fixedPosition)
            : base(theme, themeSelected, themeUnselected, font, fontCaption, x, y, width, height, caption, paddingX, paddingY, spacingY, fixedPosition) { }
        public PlayerListMenu(Theme theme, Theme themeSelected, Theme themeUnselected, TextFormat font, TextFormat fontCaption, float x, float y, float width, float height, string caption, float paddingX, float paddingY, float spacingY)
            : base(theme, themeSelected, themeUnselected, font, fontCaption, x, y, width, height, caption, paddingX, paddingY, spacingY) { }
        public PlayerListMenu(Theme theme, Theme themeSelected, Theme themeUnselected, TextFormat font, TextFormat fontCaption, float x, float y, float width, float height, string caption)
            : base(theme, themeSelected, themeUnselected, font, fontCaption, x, y, width, height, caption) { }
        #endregion

        #region METHODS
        protected override void OnDraw(SharpDX.Direct2D1.WindowRenderTarget device)
        {
            if (!Visible)
                return;
            //Reorder();
            //if (this.Y + this.Height > Program.GameController.Form.Height)
            //{
            //    base.SetPosition(this.X, Program.GameController.Form.Height - this.Height);
            //}
        }
        #endregion

        public void Reorder()
        {
            float y = this.ChildControls[0].Y + this.ChildControls[0].Height + SpaceY;
            for (int i = 1; i < this.ChildControls.Count; i++)
            {
                if(this.ChildControls[i].Visible) {
                    this.ChildControls[i].SetPosition(this.ChildControls[i].X, y);
                    y += SpaceY + this.ChildControls[i].Height;
                }
            }
        }
    }
}
