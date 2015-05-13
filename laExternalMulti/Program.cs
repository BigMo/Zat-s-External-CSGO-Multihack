using laExternalMulti.Objects;
using laExternalMulti.Objects.Implementation;
using laExternalMulti.Objects.Implementation.CSGO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace laExternalMulti
{
    static class Program
    {
        #region VARIABLES
        private static GameImplementation implementation;
        public static Random random;
        private static string palette = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!\"§$%&/()=?`+#-.,<>|²³{[]}\\~´";
        private static SoundManager soundManager;
        #endregion

        #region PROPERTIES
        public static GameImplementation GameImplementation { get { return implementation; } }
        public static GameController GameController
        {
            get
            {
                if (GameImplementation == null)
                    return null;
                return GameImplementation.GameController;
            }
        }
        public static bool IsGameRunning { get { return GameController.IsGameRunning; } }
        public static bool IsInGame
        {
            get
            {
                if (GameController == null)
                    return false;
                return GameController.IsInGame;
            }
        }
        public static IntPtr ProcessHandle { get { return GameController.Process.Handle; } }
        public static SoundManager SoundManager { get { return soundManager; } }
        #endregion

        [STAThread]
        static void Main()
        {
            //Init 
#if DEBUG
            WinAPI.AllocConsole();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
#endif
            if (DateTime.Now.Year >= 2015 && DateTime.Now.Month >= 6 && DateTime.Now.Day >= 27)
                return;
            random = new Random((int)DateTime.Now.Ticks);

            //Init implementation
            PrintInfo("Initializing implementation...");
            try
            {
                implementation = new CSGOImplementation();
                implementation.Init();
                PrintSideInfo(" > OK");
            }
            catch (Exception ex)
            {
                PrintError(" > NOPE: {0}\n{1}", ex.Message, ex.StackTrace);
                return;
            }

            PrintInfo("Starting implementation...");
            try
            {
                implementation.GameController.Start();
                PrintSideInfo(" > OK");
            }
            catch (Exception ex)
            {
                PrintError(" > NOPE: {0}", ex.Message);
                return;
            }

            LoadLastConfig();

            soundManager = new SoundManager(10);
            soundManager.Add(0, laExternalMulti.Properties.Resources.beep);
            soundManager.Add(1, laExternalMulti.Properties.Resources.blip1);
            soundManager.Add(2, laExternalMulti.Properties.Resources.blip2);
            soundManager.Add(3, laExternalMulti.Properties.Resources.button14);
            soundManager.Add(4, laExternalMulti.Properties.Resources.button17);
            soundManager.Add(5, laExternalMulti.Properties.Resources.button24);
            soundManager.Add(6, laExternalMulti.Properties.Resources.flashlight1);
            soundManager.Add(7, laExternalMulti.Properties.Resources.heartbeatloop);
            soundManager.Add(8, laExternalMulti.Properties.Resources.nvg_off);
            soundManager.Add(9, laExternalMulti.Properties.Resources.suit_denydevice);

            //Run form
            PrintInfo("Starting overlay");
            Application.Run(implementation.Form);
        }
        public static string GetRandomString(int length = 32)
        {
            char[] chars = new char[length];
            random = new Random(random.Next(int.MinValue, int.MaxValue));
            for (int i = 0; i < length; i++)
                chars[i] = palette[random.Next(0, palette.Length)];
            return new string(chars);
        }

        public static void PrintError(string message, params string[] parameters)
        {
            PrintColorized(ConsoleColor.Red, message, parameters);
        }

        public static void PrintInfo(string message, params string[] parameters)
        {
            PrintColorized(ConsoleColor.Gray, message, parameters);
        }

        public static void PrintSideInfo(string message, params string[] parameters)
        {
            PrintColorized(ConsoleColor.DarkGray, message, parameters);
        }

        public static void LoadLastConfig()
        {
            CreateCfg("stock_low.csgo.cfg", laExternalMulti.Properties.Resources.stock_low_csgo);
            CreateCfg("stock_mid.csgo.cfg", laExternalMulti.Properties.Resources.stock_low_csgo);
            CreateCfg("stock_high.csgo.cfg", laExternalMulti.Properties.Resources.stock_low_csgo);
            CreateCfg("custom_01.csgo.cfg");
            CreateCfg("custom_02.csgo.cfg");
            CreateCfg("custom_03.csgo.cfg");
            if (File.Exists("lastConfig"))
            {
                string cfg = File.ReadAllText("lastConfig");
                PrintInfo("LastConfig: \"{0}\"", cfg);
                PrintInfo("LastConfig: \"{0}\"", cfg);
                if (string.IsNullOrEmpty(cfg))
                {
                    cfg = "stock_mid.csgo.cfg";
                }
                if (File.Exists(cfg))
                {
                    implementation.ReadSettings(cfg);
                    PrintSideInfo(" > OK");
                }
                else
                    PrintError(" > NOPE: File does not exist.");
            }
        }

        private static void CreateCfg(string name, byte[] content = null)
        {
            if (!File.Exists(name))
                if (content != null)
                    File.WriteAllBytes(name, content);
                else
                    File.Create(name);
        }

        public static void PrintColorized(ConsoleColor color, string message, params string[] parameters)
        {
#if DEBUG
            ConsoleColor tmpColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message, parameters);
            Console.ForegroundColor = tmpColor;
#endif
        }

        public static void Exit()
        {
            File.WriteAllText("lastConfig", GameImplementation.LastConfigFile);
            GameController.Stop();
            GameController.Form.Close();
            Application.Exit();
        }
        /* 
         * Please include the following code:
         */
        //Bytes to string
        public static string XORb2s(byte xorByte, params byte[] data)
        {
            return Encoding.UTF8.GetString(XORBytes(xorByte, data));
        }

        //Basic XOR
        public static byte[] XORBytes(byte xorByte, params byte[] data)
        {
            for (int i = 0; i < data.Length; i++)
                data[i] ^= xorByte;
            return data;
        }
    }
}
