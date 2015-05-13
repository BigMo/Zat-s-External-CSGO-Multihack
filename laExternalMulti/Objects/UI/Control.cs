using SharpDX;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FontFactory = SharpDX.DirectWrite.Factory;
using Factory = SharpDX.Direct2D1.Factory;
using System.Diagnostics;
using SharpDX.Direct2D1;
using System.Windows.Forms;

namespace laExternalMulti.Objects.UI
{
    public abstract class Control : IDisposable
    {
        #region VARIABLES
        private float x, y, width, height;
        private TextFormat font;
        private List<Control> childControls;
        private Control parent;
        private bool visible;
        private Theme theme;
        private string text;
        private bool autoSize;
        private bool enabled;
        #endregion

        #region PROPERTIES
        public virtual float X { get { return x; } set { x = value; } }
        public virtual float Y { get { return y; } set { y = value; } }
        public virtual float Width { get { return width; } set { width = value; } }
        public virtual float Height { get { return height; } set { height = value; } }
        public TextFormat Font { get { return font; } set { font = value; } }
        public Theme Theme { get { return theme; } set { theme = value; } }
        public List<Control> ChildControls { get { return childControls; } set { childControls = value; } }
        public RectangleF Rectangle { get { return new RectangleF(x, y, width, height); } }
        public virtual bool Visible { get { return visible; } set { visible = value; } }
        public bool Enabled { get { return enabled; } set { enabled = value; } }
        public Control Parent
        {
            get { return parent; }
            set
            {
                //Remove from previous parent's children
                if (parent != null)
                    if (parent.ChildControls.Contains(this))
                        parent.RemoveChildControl(this);

                parent = value;

                //Add to new parent's children
                if (parent != null)
                    if (!parent.ChildControls.Contains(this))
                        parent.AddChildControl(this);
            }
        }
        public string Text { get { return text; } set { text = value; if (autoSize) OnAutoSize(); } }
        public bool AutoSize { get { return autoSize; } set { autoSize = value; OnAutoSize(); } }
        #endregion

        #region CONSTRUCTORS
        public Control(Theme theme, TextFormat font, float x, float y, float width, float height, bool visible, bool enabled)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.theme = theme;
            this.font = font;
            this.childControls = new List<Control>();
            this.Parent = null;
            this.visible = visible;
            this.enabled = enabled;
            this.autoSize = true;
        }
        public Control(Theme theme, TextFormat font, float x, float y, float width, float height, bool visible)
            : this(theme, font, x, y, width, height, visible, true) { }
        public Control(Theme theme, TextFormat font, float x, float y, float width, float height)
            : this(theme, font, x, y, width, height, true) { }
        public Control(Theme theme, TextFormat font, float x, float y)
            : this(theme, font, x, y, 100f, 100f) { }
        public Control(Theme theme, TextFormat font)
            : this(theme, font, 0f, 0f) { }
        #endregion

        #region METHODS
        protected static Theme GetDefaultTheme()
        {
            return new Theme(Color.LightGray, Color.Gray, Color.DarkGray, Color.Transparent, 2f, 2f);
        }
        public virtual void Draw(WindowRenderTarget device)
        {
            if (visible)
            {
                OnDraw(device);
                foreach (Control child in childControls)
                    if (child.Visible)
                        child.Draw(device);
            }
        }
        protected abstract void OnDraw(WindowRenderTarget device);
        public virtual void AddChildControl(Control child)
        {
            childControls.Add(child);
            child.Parent = this;
        }
        public virtual void RemoveChildControl(Control child)
        {
            childControls.Remove(child);
            child.Parent = null;
        }
        public void RemoveChildControlAt(int index)
        {
            RemoveChildControl(childControls[index]);
        }
        public void PurgeChildControls()
        {
            while (childControls.Count > 0)
            {
                Control cntrl = childControls[0];
                childControls.Remove(cntrl);
                cntrl.Dispose();
            }
        }
        public Control GetLastChild()
        {
            return childControls[childControls.Count - 1];
        }
        public virtual void SetPosition(float x, float y)
        {
            foreach (Control child in ChildControls)
                child.SetPosition(child.X + (x - this.x), child.Y + (y - this.Y));
            this.x = x;
            this.y = y;
        }
        public void InvokeAutoSize()
        {
            this.OnAutoSize();
        }
        protected virtual void OnAutoSize()
        {
            /* Nothing to do? */
        }
        public virtual void OnKeyDown(Keys key)
        {

        }
        public virtual void OnKeyUp(Keys key)
        {

        }
        public virtual void OnKeyIsDown(Keys key)
        {

        }
        public void CheckAutoSize()
        {
            OnAutoSize();
        }
        #region DRAW-METHODS
        protected void DrawLine(WindowRenderTarget device, Color color, float x1, float y1, float x2, float y2, float strokeWidth)
        {
            using (SolidColorBrush brush = new SolidColorBrush(device, color))
            {
                device.DrawLine(new Vector2(x1, y1), new Vector2(x2, y2), brush, strokeWidth);
            }
        }
        protected void DrawLines(WindowRenderTarget device, Color color, float strokeWidth, params Vector2[] lines)
        {
            for (int i = 0; i < lines.Length - 1; i++)
                if(lines[i] != Vector2.Zero && lines[i+1] != Vector2.Zero)
                    this.DrawLine(device,
                        color,
                        lines[i].X,
                        lines[i].Y,
                        lines[i + 1].X,
                        lines[i + 1].Y,
                        strokeWidth);
            if (lines[0] != Vector2.Zero && lines[lines.Length - 1] != Vector2.Zero)
                this.DrawLine(device,
                    color,
                    lines[0].X,
                    lines[0].Y,
                    lines[lines.Length - 1].X,
                    lines[lines.Length - 1].Y,
                    strokeWidth);
        }
        protected void ConnectLines(WindowRenderTarget device, Color color, float strokeWidth, Vector2[] linesA, Vector2[] linesB)
        {
            using (SolidColorBrush brush = new SolidColorBrush(device, color))
            {
                for (int i = 0; i < linesA.Length; i++)
                    device.DrawLine(linesA[i], linesB[i], brush, strokeWidth);
            }
        }
        protected void DrawText(WindowRenderTarget device, Color color, float x, float y, float width, float height, string text, TextFormat textFormat)
        {
            using (SolidColorBrush brush = new SolidColorBrush(device, color))
            {
                RectangleF rect = new RectangleF(x, y, width, height);
                device.DrawText(text, textFormat, rect, brush);
            }
        }
        protected void DrawText(WindowRenderTarget device, Color color, Color shadowColor, float x, float y, float width, float height, float widthOffset, float heightOffset, string text, TextFormat textFormat)
        {
            DrawText(device, shadowColor, x + widthOffset, y + heightOffset, width, height, text, textFormat);
            DrawText(device, color, x, y, width, height, text, textFormat);
        }
        protected void DrawRectangle(WindowRenderTarget device, Color color, float x, float y, float width, float height, float strokeWidth = 1f)
        {
            using (SolidColorBrush brush = new SolidColorBrush(device, color))
            {
                device.DrawRectangle(new RectangleF(x, y, width, height), brush, strokeWidth);
            }
        }
        protected void FillRectangle(WindowRenderTarget device, Color color, float x, float y, float width, float height)
        {
            using (SolidColorBrush brush = new SolidColorBrush(device, color))
            {
                device.FillRectangle(new RectangleF(x, y, width, height), brush);
            }
        }
        protected void DrawEllipse(WindowRenderTarget device, Color color, float x, float y, float width, float height, bool centered = false, float strokeWidth = 1f)
        {
            using (SolidColorBrush brush = new SolidColorBrush(device, color))
            {
                device.DrawEllipse(
                    new Ellipse(
                        new Vector2(
                            (centered ? x : x - width / 2f),
                            (centered ? y : y - height / 2f)
                        ),
                        width / 2f,
                        height / 2f
                    ),
                    brush,
                    strokeWidth
                );
            }
        }
        protected void FillEllipse(WindowRenderTarget device, Color color, float x, float y, float width, float height, bool centered = false)
        {
            using (SolidColorBrush brush = new SolidColorBrush(device, color))
            {
                device.FillEllipse(
                    new Ellipse(
                        new Vector2(
                            (centered ? x : x + width / 2f),
                            (centered ? y : y + height / 2f)
                        ),
                        width / 2f,
                        height / 2f
                    ),
                    brush
                );
            }
        }
        protected void DrawPolygon(WindowRenderTarget device, Color color, float x1, float y1, float x2, float y2, float x3, float y3, float strokeWidth = 1f)
        {
            DrawPolygon(
                device,
                color,
                new Vector2(x1, y1),
                new Vector2(x2, y2),
                new Vector2(x3, y3),
                strokeWidth
            );
        }
        protected void DrawPolygon(WindowRenderTarget device, Color color, Vector2 start, Vector2 middle, Vector2 end, float strokeWidth = 1f)
        {
            using (PathGeometry gmtry = CreatePathGeometry(start, middle, end))
            {
                using (SolidColorBrush brush = new SolidColorBrush(device, color))
                {
                    device.DrawGeometry(gmtry, brush);
                }
            }
        }
        protected void DrawPolygon(WindowRenderTarget device, Color color, float strokeWidth, params Vector2[] points)
        {
            using (PathGeometry gmtry = CreatePathGeometry(points))
            {
                using (SolidColorBrush brush = new SolidColorBrush(device, color))
                {
                    device.DrawGeometry(gmtry, brush);
                }
            }
        }
        protected void FillPolygon(WindowRenderTarget device, Color color, float x1, float y1, float x2, float y2, float x3, float y3)
        {
            FillPolygon(
                device,
                color,
                new Vector2(x1, y1),
                new Vector2(x2, y2),
                new Vector2(x3, y3)
            );
        }
        protected void FillPolygon(WindowRenderTarget device, Color color, Vector2 start, Vector2 middle, Vector2 end)
        {
            using (PathGeometry gmtry = CreatePathGeometry(start, middle, end))
            {
                using (SolidColorBrush brush = new SolidColorBrush(device, color))
                {
                    device.FillGeometry(gmtry, brush);
                }
            }
        }
        protected void FillPolygon(WindowRenderTarget device, Color color, params Vector2[] points)
        {
            using (PathGeometry gmtry = CreatePathGeometry(points))
            {
                using (SolidColorBrush brush = new SolidColorBrush(device, color))
                {
                    device.FillGeometry(gmtry, brush);
                }
            }
        }
        protected void DrawGeometry(WindowRenderTarget device, Color color, float strokeWidth, params Vector2[] points)
        {
            using (PathGeometry gmtry = CreatePathGeometry(points))
            {
                using (SolidColorBrush brush = new SolidColorBrush(device, color))
                {
                    device.DrawGeometry(gmtry, brush, strokeWidth);
                }
            }
        }
        protected void FillGeometry(WindowRenderTarget device, Color color, params Vector2[] points)
        {
            using (PathGeometry gmtry = CreatePathGeometry(points))
            {
                using (SolidColorBrush brush = new SolidColorBrush(device, color))
                {
                    device.FillGeometry(gmtry, brush);
                }
            }
        }
        private PathGeometry CreatePathGeometry(params Vector2[] points)
        {
            PathGeometry gmtry = new PathGeometry(FactoryManager.Factory);

            GeometrySink sink = gmtry.Open();
            sink.SetFillMode(FillMode.Winding);
            sink.BeginFigure(points[0], FigureBegin.Filled);
            sink.AddLines(points);
            sink.EndFigure(FigureEnd.Closed);
            sink.Close();

            return gmtry;
        }
        public static RectangleF MeasureString(TextFormat format, string text, float maxWidth, float maxHeight)
        {
            RectangleF rect;
            using (TextLayout layout = new TextLayout(FactoryManager.FontFactory, text, format, maxWidth, maxHeight))
            {
                rect = new RectangleF(0, 0, layout.Metrics.Width, layout.Metrics.Height);
            }
            return rect;
        }
        public static RectangleF MeasureString(TextFormat format, string text)
        {
            return MeasureString(format, text, float.MaxValue, float.MaxValue);
        }
        public static float GetColorMultiplier()
        {
            return (float)Math.Sin(Geometry.DegToRad(DateTime.Now.Millisecond)) / (float)Math.PI;
        }
        #endregion
        #endregion

        #region DESTRUCTORS
        public void Dispose()
        {
            foreach (Control child in childControls)
                child.Dispose();
            childControls.Clear();
            this.Font.Dispose();
        }
        #endregion
    }
}
