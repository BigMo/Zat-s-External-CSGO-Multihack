using laExternalMulti.Objects.UI;
using laExternalMulti.Objects.UI.Menu;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace laExternalMulti.Objects.Implementation.CSGO.UI.CSGOMenu
{
    class LoadActionMenuItem : ActionMenuItem
    {
        #region VARIABLES
        string fileName;
        #endregion

        #region CONSTRUCTORS
        public LoadActionMenuItem(Theme theme, TextFormat font, float x, float y, float width, float height, Menu parentMenu, string text, string fileName, bool autoCloseMenu)
            : base(theme, font, x, y, width, height, parentMenu, text, autoCloseMenu)
        {
            this.fileName = fileName;
        }
        public LoadActionMenuItem(Theme theme, TextFormat font, float x, float y, float width, float height, Menu parentMenu, string text, string fileName)
            : this(theme, font, x, y, width, height, parentMenu, text, fileName, true) { }
        public LoadActionMenuItem(Theme theme, TextFormat font, Menu parentMenu, string text, string fileName)
            : this(theme, font, 0f, 0f, 100f, 20f, parentMenu, text, fileName, true) { }
        #endregion

        #region METHODS
        public override void SwitchToVal(object value) { }
        protected override void OnStartMethod()
        {
            Program.GameImplementation.ReadSettings(fileName);
        }
        #endregion
    }
}
