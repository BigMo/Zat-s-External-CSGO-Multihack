using laExternalMulti.Objects.UI.Events;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace laExternalMulti.Objects.UI.Menu
{
    public abstract class  MenuItem : Control
    {
        #region VARIABLES
        private string extraData;
        private Menu parentMenu;
        #endregion

        #region PROPERTIES
        
        public string ExtraData { get { return extraData; } }
        public Menu ParentMenu { get { return parentMenu; } }
        #endregion

        #region CONSTRUCTOR
        public MenuItem(Theme theme, TextFormat font, float x, float y, float width, float height, Menu parentMenu, string text, string extraData)
            : base(theme, font, x, y, width, height)
        {
            this.parentMenu = parentMenu;
            this.Text = text;
            this.extraData = extraData;
        }
        public MenuItem(Theme theme, TextFormat font, float x, float y, float width, float height, Menu parentMenu, string text)
            : this(theme, font, x, y, width, height, parentMenu, text, string.Empty) { }
        public MenuItem(Theme theme, TextFormat font, float x, float y, float width, float height, string text)
            : this(theme, font, x, y, width, height, null, text, string.Empty) { }
        #endregion

        public abstract void SwitchToVal(object value);
    }
}
