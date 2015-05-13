using SharpDX;
using SharpDX.Windows;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;

using Factory = SharpDX.Direct2D1.Factory;
using FontFactory = SharpDX.DirectWrite.Factory;
using Format = SharpDX.DXGI.Format;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using laExternalMulti.Objects.UI;
using System.Diagnostics;
using laExternalMulti.Objects.Updaters;

namespace laExternalMulti.Objects
{
    public abstract partial class frmOverlay : Form
    {
        #region VARIABLES
        private WindowRenderTarget device;
        private HwndRenderTargetProperties renderProperties;
        private delegate void VoidDelegate();
        private DrawUpdater drawUpdater;
        private TickUpdater tickUpdater;
        #endregion

        #region PROPERTIES
        public DrawUpdater DrawUpdater { get { return drawUpdater; } }
        public TickUpdater TickUpdater { get { return tickUpdater; } }
        public bool UpdateWhenIdle { get; set; }
        #endregion
        public frmOverlay()
        {
            InitializeComponent();
            this.BackColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = Program.GetRandomString();
            this.Name = Program.GetRandomString();
            this.TransparencyKey = System.Drawing.Color.Black;
            this.TopMost = true;
            this.Icon = CreateIcon();
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.UpdateWhenIdle = false;

            int initialStyle = WinAPI.GetWindowLong(this.Handle, -20);
            WinAPI.SetWindowLong(this.Handle, -20, initialStyle | 0x80000 | 0x20);
            WinAPI.SetWindowPos(this.Handle, WinAPI.HWND_TOPMOST, 0, 0, 0, 0, WinAPI.TOPMOST_FLAGS);
            OnResize(null);

            drawUpdater = new DrawUpdater(this);
            tickUpdater = new TickUpdater(this);
            //drawTick.Interval = (int)(1000f / 60f);
            //drawTick.Tick += drawTick_Tick;

            InitDevice();
            this.Paint += frmOverlay_Paint;
            drawUpdater.Start();
            tickUpdater.Start();
        }

        private System.Drawing.Icon CreateIcon()
        {
            Icon ico = null;
            using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(32, 32))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    Random ran = new Random((int)Environment.TickCount);
                    g.CopyFromScreen(ran.Next(0, 512), ran.Next(0, 512), 0, 0, new Size(128, 128), CopyPixelOperation.SourceCopy);
                }
                ico = Icon.FromHandle(bmp.GetHicon());
            }
            return ico;
        }

        void frmOverlay_Paint(object sender, PaintEventArgs e)
        {
            Draw();
        }

        void drawTick_Tick(object sender, EventArgs e)
        {
            Program.GameController.InputUpdater.Update = true;
            OnTimerTick();
        }

        protected override void OnResize(EventArgs e)
        {
            int[] margins = new int[] { 0, 0, Width, Height };
            WinAPI.DwmExtendFrameIntoClientArea(this.Handle, ref margins);
        }

        #region ABSTRACT METHODS
        protected abstract void OnInitDevice();
        protected abstract void OnDestroyDevice();
        protected abstract void OnTick();
        protected abstract void OnDraw(WindowRenderTarget device);
        #endregion
        #region METHODS
        private void frmMain_Load(object sender, EventArgs e)
        {
            this.Visible = false;
        }

        public void OnTimerTick()
        {
            if (this.Disposing || this.IsDisposed)
                return;
            //Set Visibility
            if (!Program.GameController.IsGameRunning || !Program.GameController.IsInGame)
            {
                this.Visible = false;
                return;
            }
            else 
            {
                this.Visible = true;
            }

            //Set window-size and -position
            if (
                this.Location.X != Program.GameController.WindowArea.X ||
                this.Location.Y != Program.GameController.WindowArea.Y ||
                this.Size.Width != Program.GameController.WindowArea.Width ||
                this.Size.Height != Program.GameController.WindowArea.Height)
            {
                this.Location = Program.GameController.WindowArea.Location;
                this.Size = Program.GameController.WindowArea.Size;
                if (device != null)
                    device.Resize(new Size2(this.Width, this.Height));
            }
            if (device == null)
                InitDevice();

            //Update
            if (Program.GameController.InputUpdater.KeyIsDown(Keys.F8))
                Program.Exit();
            this.OnTick();

            //Draw UI
            //if(this.Visible)
            //    Draw();
        }

        public void InvokeMethod(Action method)
        {
            if (this.InvokeRequired)
                this.Invoke(method);
            else
                method();
        }

        public virtual void InitDevice()
        {
            FactoryManager.Init();
            this.TopMost = true;

            renderProperties = new HwndRenderTargetProperties()
            {
                Hwnd = this.Handle,
                PixelSize = new Size2(this.Width, this.Height),
                PresentOptions = PresentOptions.None
            };
            device = new WindowRenderTarget(FactoryManager.Factory, new RenderTargetProperties(new PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied)), renderProperties);
            device.TextAntialiasMode = TextAntialiasMode.Cleartype;

            this.OnInitDevice();

        }

        public void DestroyDevice()
        {
            this.OnDestroyDevice();
            FactoryManager.FontFactory.Dispose();
            FactoryManager.Factory.Dispose();
            device.Dispose();
        }

        public void Draw()
        {
            if (!Program.GameController.IsInGame)
                if (!UpdateWhenIdle)
                    return;
            device.BeginDraw();
            device.Clear(SharpDX.Color.Transparent);

            this.OnDraw(device);

            device.EndDraw();
        }
        #endregion
    }
}
