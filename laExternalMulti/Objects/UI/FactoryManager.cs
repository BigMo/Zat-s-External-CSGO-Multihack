using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FontFactory = SharpDX.DirectWrite.Factory;
using Factory = SharpDX.Direct2D1.Factory;
using System.Collections;
using SharpDX.DirectWrite;

namespace laExternalMulti.Objects.UI
{
    class FactoryManager
    {
        #region VARIABLES
        private static Factory factory;
        private static FontFactory fontFactory;
        private static Hashtable fonts;
        #endregion

        #region PROPERTIES
        public static Factory Factory { get { return factory; } set { factory = value; } }
        public static FontFactory FontFactory { get { return fontFactory; } set { fontFactory = value; } }
        #endregion

        #region METHODS
        public static void Init()
        {
            factory = new Factory();
            fontFactory = new FontFactory();
            fonts = new Hashtable();
        }

        public static void CreateFont(string fontName, string fontFamilyName, float fontSize)
        {
            fonts.Add(fontName, new TextFormat(fontFactory, fontFamilyName, fontSize));
        }

        public static TextFormat GetFont(string fontName)
        {
            return (TextFormat)fonts[fontName];
        }
        #endregion
    }
}
