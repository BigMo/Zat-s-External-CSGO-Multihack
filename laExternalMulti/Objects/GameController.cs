using laExternalMulti.Objects.Updaters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace laExternalMulti.Objects
{
    /// <summary>
    /// Provides basic information about the specified process:
    /// - Process running?
    /// - Process-window rectangle
    /// - Input
    /// - Update form
    /// </summary>
    public class GameController
    {
        #region VARIABLES
        //Game-related
        private Process process;
        private Rectangle rect;
        private string processName;
        private Thread threadControl;
        private bool gameRunning;
        private int tickRate;

        //Updaters
        private InputUpdater inputUpdater;
        private frmOverlay form;
        #endregion

        #region PROPERTIES
        public Rectangle WindowArea { get { return rect; } }
        public bool IsGameRunning { get { return gameRunning; } }
        public Process Process { get { return process; } }
        public InputUpdater InputUpdater { get { return inputUpdater; } }
        public frmOverlay Form { get { return form; } }
        public bool IsInGame { get; private set; }
        #endregion

        #region CONSTRUCTOR
        public GameController(int tickRate, string processName, frmOverlay form)
        {
            this.tickRate = tickRate;
            this.processName = processName;
            this.form = form;
            this.inputUpdater = new InputUpdater();
        }
        #endregion

        #region METHODS
        public void Start()
        {
            threadControl = new Thread(new ThreadStart(controlLoop));
            threadControl.IsBackground = true;
            threadControl.Start();
            inputUpdater.StartUpdater();
        }

        public void Stop()
        {
            if (threadControl != null)
            {
                threadControl.Abort();
                threadControl = null;
            }
        }

        private void controlLoop()
        {
            while (true)
            {
                if (GetGameProcess(ref process))
                {
                    rect = WinAPI.GetRectangle(process.MainWindowHandle);
                    gameRunning = true;
                    IsInGame = WinAPI.GetForegroundWindow() == process.MainWindowHandle;
                }
                else
                {
                    if (gameRunning)
                    {
                        //Terminate
                        Form.InvokeMethod(new Action(Form.Close));
                        threadControl.Abort();
                    }
                    gameRunning = false;
                }
                Thread.Sleep(tickRate);
            }
        }

        private bool GetGameProcess(ref Process proc)
        {
            Process[] procs = Process.GetProcessesByName(processName);
            if (procs.Length == 0)
            {
                proc = null;
                return false;
            }
            proc = procs[0];
            return true;
        }

        public ProcessModule GetModuleByName(string name)
        {
            try
            {
                foreach (ProcessModule module in process.Modules)
                {
                    if (module.FileName.EndsWith(name))
                        return module;
                }
            }
            catch { }
            return null;
        }

        public long GetModuleSize(string name)
        {
            ProcessModule module = GetModuleByName(name);
            if (module != null)
                return module.ModuleMemorySize;
            return 0L;
        }

        public IntPtr GetModuleBaseAddressByName(string name)
        {
            ProcessModule module = GetModuleByName(name);
            if (module != null)
                return module.BaseAddress;
            return IntPtr.Zero;
        }
        #endregion
    }
}
