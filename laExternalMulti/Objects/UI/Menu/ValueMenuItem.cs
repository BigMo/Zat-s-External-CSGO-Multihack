using laExternalMulti.Objects.UI.Events;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace laExternalMulti.Objects.UI.Menu
{
    public class ValueMenuItem : MenuItem
    {
        #region VARIABLES
        private object[] options;
        private int currentOption;
        public delegate void OptionChangedEventHandler(object sender, OptionChangedEventArgs e);
        public event OptionChangedEventHandler OptionChanged;
        #endregion

        #region PROPERTIES
        public object[] Options { get { return options; } }
        public int CurrentOptionIndex
        {
            get { return currentOption; }
            protected set
            {
                if (currentOption != value)
                {
                    currentOption = value;
                    if (currentOption < 0)
                        currentOption += options.Length;
                    if (currentOption >= options.Length)
                        currentOption %= options.Length;
                    OnOptionChanged(new OptionChangedEventArgs(this.CurrentOptionValue, this.CurrentOptionIndex));
                }
            }
        }
        public virtual object CurrentOptionValue { get { return options[currentOption]; } }
        #endregion

        #region CONSTRUCTOR
        public ValueMenuItem(Theme theme, TextFormat font, float x, float y, float width, float height, Menu parentMenu, string text, string extraData, object[] options, int selectedOption)
            : base(theme, font, x, y, width, height, parentMenu, text, extraData)
        {
            this.options = options;
            if (Program.GameImplementation.HasKey(this.ExtraData))
            {
                for (int i = 0; i < this.Options.Length; i++)
                    if (this.options[i] == Program.GameImplementation.GetValue(this.ExtraData))
                        this.CurrentOptionIndex = i;
            }
            else
            {
                this.CurrentOptionIndex = selectedOption;
            }
        }
        public ValueMenuItem(Theme theme, TextFormat font, float x, float y, float width, float height, Menu parentMenu, string text, string extraData, object[] options)
            : this(theme, font, x, y, width, height, parentMenu, text, extraData, options, 0) { }
        public ValueMenuItem(Theme theme, TextFormat font, float x, float y, float width, float height, Menu parentMenu, string text, object[] options)
            : this(theme, font, x, y, width, height, parentMenu, text, "", options, 0) { }
        public ValueMenuItem(Theme theme, TextFormat font, float x, float y, float width, float height, Menu parentMenu, object[] options)
            : this(theme, font, x, y, width, height, parentMenu, "ValuMenuItem", options) { }
        public ValueMenuItem(Theme theme, TextFormat font, float x, float y, Menu parentMenu, object[] options)
            : this(theme, font, x, y, 100f, 20f, parentMenu, options) { }
        public ValueMenuItem(Theme theme, TextFormat font, Menu parentMenu, object[] options)
            : this(theme, font, 0f, 0f, parentMenu, options) { }
        public ValueMenuItem(Theme theme, TextFormat font, Menu parentMenu, string text, object[] options, int selectedOption)
            : this(theme, font, 0f, 0f, 100f, 20f, parentMenu, text, "", options, selectedOption) { }
        public ValueMenuItem(Theme theme, TextFormat font, Menu parentMenu, string text, string extraData, object[] options)
            : this(theme, font, 0f, 0f, 100f, 20f, parentMenu, text, extraData, options, 0) { }
        public ValueMenuItem(Theme theme, TextFormat font, Menu parentMenu, string text, object[] options, string extraData)
            : this(theme, font, 0f, 0f, 100f, 20f, parentMenu, text, extraData, options, 0) { }
        //new ValueMenuItem(themeBasic, fontSmall, highlightPlayerMenu, "Player", switchOnOff, 1);
        #endregion

        #region METHODS
        protected virtual void OnOptionChanged(OptionChangedEventArgs e)
        {
            if (OptionChanged != null)
                OptionChanged(this, e);
        }
        public override void OnKeyUp(System.Windows.Forms.Keys key)
        {
            switch (key)
            {
                case System.Windows.Forms.Keys.Left:
                    CurrentOptionIndex--;
                    break;
                case System.Windows.Forms.Keys.Right:
                    CurrentOptionIndex++;
                    break;
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
                String.Format("{0}: {1}", this.Text, Program.GameImplementation.GetValue(this.ExtraData).ToString()), //this.CurrentOptionValue), 
                this.Font);
        }
        public override void SwitchToVal(object value) 
        {
            for (int i = 0; i < options.Length; i++)
            {
                if (value.Equals(options[i]))
                {
                    this.CurrentOptionIndex = i;
                    break;
                }
            }
        }
        #endregion
    }
}
