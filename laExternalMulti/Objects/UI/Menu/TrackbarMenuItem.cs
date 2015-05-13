using laExternalMulti.Objects.UI.Events;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace laExternalMulti.Objects.UI.Menu
{
    public class TrackbarMenuItem : MenuItem
    {
        #region VARIABLES
        private float minimum, maximum, value, step;
        public delegate void ValueChangedEventHandler(object sender, ValueChangedEventArgs e);
        public event ValueChangedEventHandler ValueChanged;
        #endregion

        #region PROPERTIES
        public float Value 
        { 
            get { return value; }
            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    if (this.value > maximum)
                        this.value = maximum;
                    if (this.value < minimum)
                        this.value = minimum;
                    OnValueChanged(new ValueChangedEventArgs(value));
                }
            }
        }
        #endregion

        #region CONSTRUCTOR
        public TrackbarMenuItem(Theme theme, TextFormat font, float x, float y, float width, float height, Menu parentMenu, string text, float min, float max, float step,  float value, string extraData)
            : base(theme, font, x, y, width, height, parentMenu, text, extraData)
        {
            this.minimum = min;
            this.maximum = max;
            this.value = value;
            this.step = step;
            if (Program.GameImplementation.HasKey(this.ExtraData))
                this.value = Program.GameImplementation.GetValue<float>(this.ExtraData);
        }
        public TrackbarMenuItem(Theme theme, TextFormat font, float x, float y, float width, float height, Menu parentMenu, string text, float min, float max, float step, string extraData)
            : this(theme, font, x, y, width, height, parentMenu, text, min, max, step, min + (max - min) / 2f, extraData) { }
        public TrackbarMenuItem(Theme theme, TextFormat font, Menu parentMenu, string text, float min, float max, float step, string extraData)
            : this(theme, font, 0f, 0f, 100f, 100f, parentMenu, text, min, max, step, extraData) { }
        public TrackbarMenuItem(Theme theme, TextFormat font, Menu parentMenu, string text, float min, float max, float step, float value, string extraData)
            : this(theme, font, 0f, 0f, 100f, 100f, parentMenu, text, min, max, step, value, extraData) { }
        #endregion

        #region METHODS
        public override void OnKeyUp(System.Windows.Forms.Keys key)
        {
            if (key == System.Windows.Forms.Keys.Right)
            {
                if (value == maximum)
                {
                    value = minimum;
                    return;
                }
                if (value + step > maximum)
                    this.Value = maximum;
                else
                    this.Value += step;
            }
            else if (key == System.Windows.Forms.Keys.Left)
            {
                if (value == minimum)
                {
                    value = maximum;
                    return;
                }
                if (value - step < minimum)
                    this.Value = minimum;
                else
                    this.Value -= step;
            }
        }
        protected virtual void OnValueChanged(ValueChangedEventArgs e)
        {
            if (ValueChanged != null)
                ValueChanged(this, e);
        }
        protected override void OnDraw(SharpDX.Direct2D1.WindowRenderTarget device)
        {
            this.DrawRectangle(device, this.Theme.BorderColor, this.X, this.Y, this.Width, this.Height);
            this.FillRectangle(device, this.Theme.BackColor, this.X, this.Y, this.Width, this.Height);
            this.DrawText(
                device, 
                this.Theme.ForeColor,
                this.X + 2f, 
                this.Y + 2f, 
                this.Width - 4f, 
                this.Height - 4f, 
                String.Format("{0}: {1}", this.Text, Math.Round(this.Value,2).ToString()),
                this.Font
                );

            DrawTrackBar(device, this.X + this.Width / 2f, this.Y, this.Width / 2f, this.Height);
        }

        private void DrawTrackBar(SharpDX.Direct2D1.WindowRenderTarget device, float x, float y, float width, float height)
        {
            float 
                lineHeight = height / 4f,
                lineX = x + lineHeight / 2f,
                lineY = y + height / 2f - lineHeight / 2f,
                lineWidth = width - lineHeight;

            this.FillRectangle(device, this.Theme.BackColor, lineX, lineY, lineWidth, lineHeight);
            this.DrawRectangle(device, this.Theme.BorderColor, lineX, lineY, lineWidth, lineHeight);

            float
                barWidth = height / 2f,
                barX = lineWidth / (maximum - minimum) * (value - minimum) + lineX - barWidth / 2f;

            this.FillRectangle(device, this.Theme.BackColor, barX, y, barWidth, height);
            this.DrawRectangle(device, this.Theme.BorderColor, barX, y, barWidth, height);
        }

        public override void SwitchToVal(object value)
        {
            this.Value = (float)value;
        }
        #endregion
    }
}
