using SharpDX;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace laExternalMulti.Objects.UI
{
    public class ListPanel : Panel
    {
        #region VARIABLES
        private float paddingX, paddingY, spaceY;
        #endregion

        #region PROPERTIES
        public float PaddingX { get { return paddingX; } set { paddingX = value; } }
        public float PaddingY { get { return paddingY; } set { paddingY = value; } }
        public float SpaceY { get { return spaceY; } set { spaceY = value; } }
        #endregion

        #region CONSTRUCTORS
        public ListPanel(Theme theme, TextFormat font, float x, float y, float width, float height, float paddingX, float paddingY, float spaceY)
            : base(theme, font, x, y, width, height)
        {
            this.paddingX = paddingX;
            this.paddingY = paddingY;
            this.spaceY = spaceY;
        }
        #endregion

        #region METHODS
        public override void AddChildControl(Control child)
        {
            if (ChildControls.Count == 0)
                child.SetPosition(X + paddingX, Y + paddingY + 0);
            else
            {
                Control lastChild = GetLastChild();
                child.SetPosition(X + paddingX, lastChild.Y + lastChild.Height + spaceY);
            }
            base.AddChildControl(child);
            this.OnAutoSize();
        }

        protected override void OnAutoSize()
        {
            if (ChildControls.Count > 0 && AutoSize)
            {
                float height = 0f;
                foreach (Control ctrl in ChildControls)
                {
                    if (ctrl.Visible)
                        height = ctrl.Height + ctrl.Y - this.Y + paddingY * 2;
                }

                if (this.Height != height)
                {
                    this.Height = height;
                }
            }
        }

        protected override void OnDraw(SharpDX.Direct2D1.WindowRenderTarget device)
        { }
        #endregion
    }
}
