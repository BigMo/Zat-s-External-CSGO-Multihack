using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace laExternalMulti.Objects.UI
{
    public class ProgressBarTheme : Theme
    {
        #region VARIABLES
        private Color colorLow;
        private Color colorHigh;
        private bool isTransitioning;
        #endregion

        #region PROPERTIES
        public Color Low { get { return colorLow; } set { colorLow = value; } }
        public Color High { get { return colorHigh; } set { colorHigh = value; } }
        public bool IsTransitioning { get { return isTransitioning; } set { isTransitioning = value; } }
        #endregion

        #region CONSTRUCTOR
        public ProgressBarTheme(
            Color foreColor, Color backColor, Color borderColor, Color shadowColor, float shadowOffsetX, float shadowOffsetY,
            Color colorLow, Color colorHigh, bool isTransitioning)
            : base(foreColor, backColor, borderColor, shadowColor, shadowOffsetX, shadowOffsetY)
        {
            this.colorLow = colorLow;
            this.colorHigh = colorHigh;
            this.isTransitioning = isTransitioning;
        }
        public ProgressBarTheme(
            Color foreColor, Color backColor, Color borderColor, Color shadowColor, float shadowOffsetX, float shadowOffsetY,
            Color colorLow, Color colorHigh)
            : this(foreColor, backColor, borderColor, shadowColor, shadowOffsetX, shadowOffsetY, colorLow, colorHigh, true) { }
        public ProgressBarTheme(
            Color foreColor, Color backColor, Color borderColor, Color shadowColor, float shadowOffsetX, float shadowOffsetY,
            Color colorLow)
            : this(foreColor, backColor, borderColor, shadowColor, shadowOffsetX, shadowOffsetY, colorLow, Color.Transparent, false) { }
        #endregion
    }
}
