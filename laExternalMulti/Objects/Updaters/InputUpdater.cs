using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace laExternalMulti.Objects.Updaters
{
    public class InputUpdater : TimerUpdater
    {
        #region VARIABLES
        private Hashtable keys, prevKeys;
        private Int32[] values;
        private bool update;
        #endregion

        #region PROPERTIES
        public bool Update { get { return update; } set { update = value; } }
        #endregion

        #region CONSTRUCTOR
        public InputUpdater() : base()
        {
            keys = new Hashtable();
            prevKeys = new Hashtable();
            values = (Int32[])Enum.GetValues(typeof(Keys));
            Init();
            update = true;
        }
        #endregion

        #region METHODS
        public override void OnUpdaterTick()
        {
            //if (update)
            //{
                UpdateKeyTable();
            //    update = false;
            //}
        }

        private void Init()
        {
            foreach (Int32 key in values)
            {
                if (!prevKeys.ContainsKey(key))
                {
                    prevKeys.Add(key, false);
                    keys.Add(key, false);
                }
            }
        }

        public void UpdateKeyTable()
        {
            prevKeys = (Hashtable)keys.Clone();
            foreach (Int32 key in values)
            {
                keys[key] = WinAPI.GetKeyDown(key);
            }
        }
        public List<Keys> KeysThatWentUp()
        {
            List<Keys> keys = new List<Keys>();
            foreach (Keys key in values)
            {
                if (KeyWentUp(key))
                    keys.Add(key);
            }
            return keys;
        }
        public List<Keys> KeysThatWentDown()
        {
            List<Keys> keys = new List<Keys>();
            foreach (Keys key in values)
            {
                if (KeyWentDown(key))
                    keys.Add(key);
            }
            return keys;
        }
        public List<Keys> KeysThatAreDown()
        {
            List<Keys> keys = new List<Keys>();
            foreach (Keys key in values)
            {
                if (KeyIsDown(key))
                    keys.Add(key);
            }
            return keys;
        }
        public bool KeyWentUp(Keys key)
        {
            return KeyWentUp((Int32)key);
        }
        public bool KeyWentUp(Int32 key)
        {
            if (!KeyExists(key))
                return false;
            return (bool)prevKeys[key] && !(bool)keys[key];
        }
        public bool KeyWentDown(Keys key)
        {
            return KeyWentDown((Int32)key);
        }
        public bool KeyWentDown(Int32 key)
        {
            if (!KeyExists(key))
                return false;
            return !(bool)prevKeys[key] && (bool)keys[key];
        }
        public bool KeyIsDown(Keys key)
        {
            return KeyIsDown((Int32)key);
        }
        public bool KeyIsDown(Int32 key)
        {
            UpdateKeyTable();
            if (!KeyExists(key))
                return false;
            return (bool)prevKeys[key] || (bool)keys[key];
        }

        private bool KeyExists(Int32 key)
        {
            return (prevKeys.ContainsKey(key) && keys.ContainsKey(key));
        }
        #endregion
    }
}
