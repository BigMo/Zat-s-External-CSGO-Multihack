using SharpDX;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace laExternalMulti.Objects.UI
{
    class ProgressBar : Control
    {
        #region VARIABLES
        private float maxValue, currValue, blinkingThreshold;
        private bool blinkingEnabled;
        #endregion

        #region PROPERTIES
        public float Maximum { get { return maxValue; } set { maxValue = value; } }
        public float BlinkingThreshold { get { return blinkingThreshold; } set { blinkingThreshold = value; } }
        public float Value {
            get { return currValue; } 
            set 
            { 
                currValue = value; 
                if (currValue > maxValue) 
                    currValue = maxValue; 
            }
        }
        public bool BlinkingEnabled { get { return blinkingEnabled; } set { blinkingEnabled = value; } }
        public ProgressBarTheme ProgressBarTheme { get { return (ProgressBarTheme)Theme; } }
        #endregion

        #region CONSTRUCTORS
        public ProgressBar(ProgressBarTheme theme, TextFormat font, float x, float y, float width, float height, float value, float maxValue, bool blinkingEnabled, float blinkingThreshold)
            : base(theme, font, x, y, width, height)
        {
            this.currValue = value;
            this.maxValue = maxValue;
            this.blinkingEnabled = blinkingEnabled;
            this.blinkingThreshold = blinkingThreshold;
        }
        public ProgressBar(ProgressBarTheme theme, TextFormat font, float x, float y, float width, float height, float value, float maxValue, bool blinkingEnabled)
            : this(theme, font, x, y, width, height, value, maxValue, blinkingEnabled, 0.75f) { }
        public ProgressBar(ProgressBarTheme theme, TextFormat font, float x, float y, float width, float height, float value, float maxValue)
            : this(theme, font, x, y, width, height, value, maxValue, true) { }
        public ProgressBar(ProgressBarTheme theme, TextFormat font, float x, float y, float width, float height, float value)
            : this(theme, font, x, y, width, height, value, 100f) { }
        #endregion

        #region METHODS
        protected override void OnDraw(SharpDX.Direct2D1.WindowRenderTarget device)
        {
            /* BackColor: Background
             * ForeColor: BarLow
             * ShadowColor: BarHigh
             * BorderColor: Border & Font
             */
            if (Theme.BackColor != Color.Transparent)
                FillRectangle(device,
                    ProgressBarTheme.BackColor,
                    X,
                    Y,
                    Width,
                    Height);
            DrawRectangle(device,
                ProgressBarTheme.BorderColor,
                X,
                Y,
                Width,
                Height);
            Color color = ProgressBarTheme.Low;
            if (ProgressBarTheme.IsTransitioning)
            {
                color = new Color(
                    (byte)(ProgressBarTheme.Low.R + (ProgressBarTheme.High.R - ProgressBarTheme.Low.R) / maxValue * currValue),
                    (byte)(ProgressBarTheme.Low.G + (ProgressBarTheme.High.G - ProgressBarTheme.Low.G) / maxValue * currValue),
                    (byte)(ProgressBarTheme.Low.B + (ProgressBarTheme.High.B - ProgressBarTheme.Low.B) / maxValue * currValue),
                    (byte)(ProgressBarTheme.Low.A + (ProgressBarTheme.High.A - ProgressBarTheme.Low.A) / maxValue * currValue)
                    );
            }
            if (this.Enabled)
                FillRectangle(device,
                    (
                        blinkingEnabled && currValue >= maxValue  * blinkingThreshold ? 
                            color * (0.75f + 0.25f * GetColorMultiplier()) : 
                            color
                    ),
                    X + 1f,
                    Y + 1f,
                    Width / maxValue * currValue - 2f,
                    Height - 2f);
            else
                FillRectangle(device,
                    ProgressBarTheme.BorderColor * 0.25f,
                    X + 1f,
                    Y + 1f,
                    Width - 2f,
                    Height - 2f);
            DrawText(device);
        }

        protected virtual void DrawText(SharpDX.Direct2D1.WindowRenderTarget device)
        {
            if (!string.IsNullOrEmpty(Text))
            {
                DrawText(device,
                    ProgressBarTheme.ForeColor,
                    X + 4f,
                    Y + 2f,
                    Width - 4f,
                    Height,
                    Text,
                    Font);
            }
        }
        #endregion
    }
}
