using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace laExternalMulti.Objects.Implementation
{
    public abstract class GameImplementation
    {
        #region VARIABLES
        protected GameController gameController;
        protected frmOverlay form;
        protected Hashtable settingsStorage;
        private string lastConfigFile;
        #endregion

        #region PROPERTIES
        public GameController GameController { get { return gameController; } }
        public frmOverlay Form { get { return form; } }
        public string LastConfigFile { get { return lastConfigFile; } }
        #endregion

        #region CONSTRUCTOR 
        public GameImplementation()
        {
            this.settingsStorage = new Hashtable();
        }
        #endregion

        #region METHODS
        public virtual void Init() { }
        public T GetValue<T>(string valueName)
        {
            return (T)settingsStorage[valueName];
        }

        public Object GetValue(string valueName)
        {
            return settingsStorage[valueName];
        }

        public bool HasKey(string valueName)
        {
            return settingsStorage.ContainsKey(valueName);
        }

        public void SetValue(string valueName, object value)
        {
            settingsStorage[valueName] = value;
        }

        public void Toggle(string valueName)
        {
            if (Convert.ToInt32(GetValue(valueName)) == 0)
                SetValue(valueName, 1);
            else
                SetValue(valueName, 0);
        }

        public virtual void SaveSettings(string fileName)
        {
            List<string> settings = new List<string>();
            using (StreamWriter writer = new StreamWriter(fileName, false))
            {
                Program.PrintInfo("Writing config \"{0}\"", fileName);
                foreach (DictionaryEntry entry in settingsStorage)
                {
                    settings.Add(String.Format("{0}={1}", entry.Key, entry.Value));
                }
                settings.Sort();
                foreach (string line in settings)
                    writer.WriteLine(line);
            }
            lastConfigFile = fileName;
        }

        public virtual void ReadSettings(string fileName)
        {
            if (!File.Exists(fileName))
                return;
            using (StreamReader reader = new StreamReader(fileName))
            {
                string line = null;
                int lineIdx = 0;
                Program.PrintInfo("Reading config \"{0}\"", fileName);
                while ((line = reader.ReadLine()) != null)
                {
                    lineIdx++;
                    if (line.Length == 0)
                        continue;
                    if (!line.Contains('='))
                        continue;
                    Program.PrintSideInfo("[{0}] {1}", lineIdx.ToString().PadLeft(3, '0'), line);
                    try
                    {
                        string[] parts = line.Split('=');
                        if (parts.Length != 2)
                            continue;
                        InterpretSetting(parts[0], parts[1]);
                    }
                    catch 
                    {
                        Program.PrintError("Error interpreting line #{0} \"{1}\": Check for missing data and type mismatches!", lineIdx.ToString(), line);
                    }
                }
                Program.PrintInfo("Finished parsing config");
            }
            OnSettingsRead();
            lastConfigFile = fileName;
        }

        protected virtual void OnSettingsRead()
        {

        }
        protected abstract void InterpretSetting(string name, string value);
        protected static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
        #endregion
    }
}
