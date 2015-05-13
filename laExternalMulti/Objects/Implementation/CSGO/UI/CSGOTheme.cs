using laExternalMulti.Objects.UI;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace laExternalMulti.Objects.Implementation.CSGO.UI
{
    public class CSGOTheme : Theme
    {
        #region VARIABLES
        private Color colorTeamCT;
        private Color colorTeamT;
        private Color colorLifebarForeground;
        private Color colorLifebarBackground;
        private Color viewColor;
        private Color viewColorOutline;
        private Color lineColor;
        #endregion

        #region PROPERTIES
        public Color TeamCT { get { return colorTeamCT; } set { colorTeamCT = value; } }
        public Color TeamT { get { return colorTeamT; } set { colorTeamT = value; } }
        public Color LifebarForeground { get { return colorLifebarForeground; } set { colorLifebarForeground = value; } }
        public Color LifebarBackground { get { return colorLifebarBackground; } set { colorLifebarBackground = value; } }
        public Color ViewColor { get { return viewColor; } set { viewColor = value; } }
        public Color ViewColorOutline { get { return viewColorOutline; } set { viewColorOutline = value; } }
        public Color Line { get { return lineColor; } set { lineColor = value; } }
        #endregion

        #region CONSTRUCTOR
        public CSGOTheme(
            Color foreColor, Color backColor, Color borderColor, Color shadowColor, float shadowOffsetX, float shadowOffsetY,
            Color colorTeamCT, Color colorTeamT, Color colorLifebarForeground, Color colorLifebarBackground, Color viewColor, Color viewColorOutline, Color lineColor)
            : base(foreColor, backColor, borderColor, shadowColor, shadowOffsetX, shadowOffsetY)
        {
            this.colorTeamCT = colorTeamCT;
            this.colorTeamT = colorTeamT;
            this.colorLifebarBackground = colorLifebarBackground;
            this.colorLifebarForeground = colorLifebarForeground;
            this.viewColor = viewColor;
            this.viewColorOutline = viewColorOutline;
            this.lineColor = lineColor;
        }
        #endregion
    }
}
