using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace laExternalMulti.Objects.UI
{
    public class Theme
    {
        #region VARIABLES
        private Color foreColor;
        private Color backColor;
        private Color borderColor;
        private Color shadowColor;
        private float shadowOffsetX, shadowOffsetY;
        #endregion

        #region PROPERTIES
        public Color ForeColor { get { return foreColor; } set { foreColor = value; } }
        public Color BackColor { get { return backColor; } set { backColor = value; } }
        public Color BorderColor { get { return borderColor; } set { borderColor = value; } }
        public Color ShadowColor { get { return shadowColor; } set { shadowColor = value; } }
        public float ShadowOffsetX { get { return shadowOffsetX; } set { shadowOffsetX = value; } }
        public float ShadowOffsetY { get { return shadowOffsetY; } set { shadowOffsetY = value; } }
        #endregion

        #region CONSTRUCTOR 
        public Theme(Color foreColor, Color backColor, Color borderColor, Color shadowColor, float shadowOffsetX, float shadowOffsetY)
        {
            this.foreColor = foreColor;
            this.backColor = backColor;
            this.borderColor = borderColor;
            this.shadowColor = shadowColor;
            this.shadowOffsetX = shadowOffsetX;
            this.shadowOffsetY = shadowOffsetY;
        }
        #endregion
    }
}
