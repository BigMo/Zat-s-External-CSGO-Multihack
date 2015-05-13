using laExternalMulti.Objects.UI;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace laExternalMulti.Objects.Implementation.CSGO.UI
{
    public abstract class CSGOControl : Control
    {
        #region PROPERTIES
        public CSGOTheme CSGOTheme { get { return (CSGOTheme)Theme; } }
        #endregion

        #region CONSTRUCTORS

        #region CONSTRUCTORS
        public CSGOControl(CSGOTheme theme, TextFormat font, float x, float y, float width, float height, bool visible, bool enabled)
            : base(theme, font, x, y, width, height, visible, enabled) { }
        public CSGOControl(CSGOTheme theme, TextFormat font, float x, float y, float width, float height, bool visible)
            : this(theme, font, x, y, width, height, visible, true) { }
        public CSGOControl(CSGOTheme theme, TextFormat font, float x, float y, float width, float height)
            : this(theme, font, x, y, width, height, true) { }
        public CSGOControl(CSGOTheme theme, TextFormat font, float x, float y)
            : this(theme, font, x, y, 100f, 100f) { }
        public CSGOControl(CSGOTheme theme, TextFormat font)
            : this(theme, font, 0f, 0f) { }
        #endregion
        #endregion
    }
}
