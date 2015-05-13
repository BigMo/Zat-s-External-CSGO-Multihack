using laExternalMulti.Objects.UI.Events;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace laExternalMulti.Objects.UI.Menu
{
    public class KeyMenuItem : MenuItem
    {
        #region VARIABLES
        private Keys key;
        private bool acceptKey;
        public delegate void OptionChangedEventHandler(object sender, OptionChangedEventArgs e);
        public event OptionChangedEventHandler OptionChanged;
        #endregion

        #region PROPERTIES
        public Keys Key { get { return key; }
            set {
                if (key != value)
                {
                    key = value;
                    OnOptionChanged(new OptionChangedEventArgs(key, 0));
                }
            }
        }
        #endregion

        #region CONSTRUCTOR
        public KeyMenuItem(Theme theme, TextFormat font, float x, float y, float width, float height, Menu parentMenu, string text, string extraData, Keys key)
            : base(theme, font, x, y, width, height, parentMenu, text, extraData)
        {
            this.Key = key;
            this.acceptKey = false;
            if (Program.GameImplementation.HasKey(this.ExtraData))
                this.Key = Program.GameImplementation.GetValue<Keys>(this.ExtraData);
        }
        public KeyMenuItem(Theme theme, TextFormat font, float x, float y, float width, float height, Menu parentMenu, string text, Keys key)
            : this(theme, font, x, y, width, height, parentMenu, text, "", key) { }
        public KeyMenuItem(Theme theme, TextFormat font, float x, float y, float width, float height, Menu parentMenu, Keys key)
            : this(theme, font, x, y, width, height, parentMenu, "KeyMenuValue", key) { }
        public KeyMenuItem(Theme theme, TextFormat font, float x, float y, Menu parentMenu, Keys key)
            : this(theme, font, x, y, 100f, 20f, parentMenu, key) { }
        public KeyMenuItem(Theme theme, TextFormat font, Menu parentMenu, Keys key)
            : this(theme, font, 0f, 0f, parentMenu, key) { }
        public KeyMenuItem(Theme theme, TextFormat font, Menu parentMenu, string text, Keys key, string extraData)
            : this(theme, font, 0f, 0f, 100f, 20f, parentMenu, text, extraData, key) { }
        public KeyMenuItem(Theme theme, TextFormat font, Menu parentMenu, string text, string extraData, Keys key)
            : this(theme, font, 0f, 0f, 100f, 20f, parentMenu, text, extraData, key) { }
        #endregion

        #region METHODS
        protected virtual void OnOptionChanged(OptionChangedEventArgs e)
        {
            if (OptionChanged != null)
                OptionChanged(this, e);
        }
        public override void OnKeyUp(System.Windows.Forms.Keys key)
        {
            if (key == Keys.Left || key == Keys.Right)
            {
                acceptKey = true;
            }
            else
            {
                if (acceptKey)
                {
                    this.Key = key;
                    this.acceptKey = false;
                }
            }
        }
        protected override void OnDraw(SharpDX.Direct2D1.WindowRenderTarget device)
        {
            this.DrawRectangle(device, this.Theme.BorderColor, this.X, this.Y, this.Width, this.Height);
            this.FillRectangle(device, this.Theme.BackColor, this.X, this.Y, this.Width, this.Height);
            this.DrawText(device,
                this.Theme.ForeColor, 
                this.X + 2f, 
                this.Y + 2f, 
                this.Width - 4f,
                this.Height - 4f,
                String.Format("{0}: {1}", this.Text, this.acceptKey ? "(PRESS KEY)" : this.key.ToString()),
                this.Font);
        }
        public override void SwitchToVal(object value) 
        {
            Keys _key = (Keys)value;
            Key = _key;
        }
        #endregion
    }
}
