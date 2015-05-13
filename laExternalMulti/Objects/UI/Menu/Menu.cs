using SharpDX;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace laExternalMulti.Objects.UI.Menu
{
    public class Menu : ListPanel
    {
        #region VARIABLES
        private Menu parent;
        private Menu child;
        private int menuItemIndex;
        private Theme themeUnselected, themeSelected;
        private Label lblCaption;
        private bool fixedPosition;
        private TextFormat fontCaption;
        #endregion

        #region PROPERTIES
        public Menu ParentMenu { get { return parent; } set { parent = value; } }
        public Menu ChildMenu 
        { 
            get { return child; } 
            set
            {
                child = value;
                if (child != null)
                    child.CheckForOverlap();
            }
        }
        public Theme ThemeItemSelected { get { return themeSelected; } set { themeSelected = value; } }
        public Theme ThemeItemUnselected { get { return themeUnselected; } set { themeUnselected = value; } }
        private int MenuItemIndex { get { return menuItemIndex; } set { menuItemIndex = value; } }
        private Control MenuItem { get { return ChildControls[menuItemIndex]; } }
        public TextFormat FontCaption { get { return fontCaption; } set { fontCaption = value; } }
        public override bool Visible { get { return base.Visible; } set { base.Visible = value; this.CheckForOverlap(); } }
        #endregion

        #region CONSTRUCTOR
        public Menu(Theme theme, Theme themeSelected, Theme themeUnselected, TextFormat font, TextFormat fontCaption, float x, float y, float width, float height, string caption, float paddingX, float paddingY, float spacingY, bool fixedPosition)
            : base(theme, font, x, y, width, height, paddingX, paddingY, spacingY)
        {
            //themeUnselected = new Theme(Theme.ForeColor * 0.9f, Theme.BackColor * 0.9f, Theme.ForeColor * 0.9f, Color.Transparent, 0f, 0f);
            //themeSelected = new Theme(Theme.ForeColor * 1.1f, Theme.BackColor * 1.1f, Theme.ForeColor * 1.1f, Color.Transparent, 0f, 0f);
            this.themeSelected = themeSelected;
            this.themeUnselected = themeUnselected;
            this.fontCaption = fontCaption;
            lblCaption = new Label(theme, fontCaption, 0, 0, width, 20f, caption);
            this.menuItemIndex = 1;
            this.AddChildControl(lblCaption);
            this.fixedPosition = fixedPosition;
        }
        public Menu(Theme theme, Theme themeSelected, Theme themeUnselected, TextFormat font, TextFormat fontCaption, float x, float y, float width, float height, string caption, float paddingX, float paddingY, float spacingY)
            : this(theme, themeSelected, themeUnselected, font, fontCaption, x, y, width, height, caption, paddingX, paddingY, spacingY, false) { }
        public Menu(Theme theme, Theme themeSelected, Theme themeUnselected, TextFormat font, TextFormat fontCaption, float x, float y, float width, float height, string caption)
            : this(theme, themeSelected, themeUnselected, font, fontCaption, x, y, width, height, caption, 4f, 4f, 4f) { }
        #endregion

        #region METHODS
        public override void AddChildControl(Control child)
        {
            base.AddChildControl(child);
            if(ChildControls.IndexOf(child) == menuItemIndex)
            {
                child.Theme = themeSelected;
            }
        }
        protected override void OnAutoSize()
        {
            base.OnAutoSize();
            this.CheckForOverlap();
        }
        public void CheckForOverlap()
        {
            if (Program.GameImplementation == null)
                return;
            if (Program.GameController == null)
                return;
            if (Program.GameController.Form == null)
                return;
            if (this.Y + this.Height > Program.GameController.Form.Height && !this.fixedPosition)
            {
                base.SetPosition(this.X, Program.GameController.Form.Height - this.Height);
            }
        }
        public override void OnKeyUp(System.Windows.Forms.Keys key)
        {
            //Return if no children
            if (this.ChildControls.Count == 0)
                return;

            //Redirect to childMenu if set
            if (child != null)
            {
                child.OnKeyUp(key);
                return;
            }

            if (ParentMenu != null && key == System.Windows.Forms.Keys.Back)
            {
                ParentMenu.ChildMenu = null;
                return;
            }

            //Navigate
            if (key == System.Windows.Forms.Keys.Up || key == System.Windows.Forms.Keys.Down)
            {
                MenuItem.Theme = themeUnselected;
                if (key == System.Windows.Forms.Keys.Down)
                    menuItemIndex++;
                if (key == System.Windows.Forms.Keys.Up)
                    menuItemIndex--;
                menuItemIndex %= ChildControls.Count;
                if (menuItemIndex < 0)
                    menuItemIndex = ChildControls.Count + menuItemIndex;
                if (menuItemIndex == 0)
                {
                    OnKeyUp(key);
                }
                MenuItem.Theme = themeSelected;
                return;
            }

            //Redirect to current child
            this.ChildControls[this.menuItemIndex].OnKeyUp(key);
        }
        //public override void SetPosition(float x, float y)
        //{
        //    float distX = x - X, distY = y - Y;
        //    foreach (Control childControl in ChildControls)
        //        childControl.SetPosition(childControl.X + distX, childControl.Y + distY);
        //    base.SetPosition(x, y);
        //}
        protected override void OnDraw(SharpDX.Direct2D1.WindowRenderTarget device)
        {
            this.FillRectangle(device,
                this.Theme.BackColor,
                this.X,
                this.Y,
                this.Width,
                this.Height);

            if (ChildMenu != null)
                ChildMenu.Draw(device);

            if (!fixedPosition && this.Y + this.Height > Program.GameController.Form.Height)
            {
                this.SetPosition(this.X, Program.GameController.Form.Height - this.Height);
            }
        }
        #endregion
    }
}
