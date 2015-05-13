using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace laExternalMulti.Objects.UI.Events
{
    public class ValueChangedEventArgs
    {
        #region VARIABLES
        private float value;
        #endregion

        #region PROPERTIES
        public float Value { get { return value; } }
        #endregion

        #region CONSTRUCTOR
        public ValueChangedEventArgs(float value)
        {
            this.value = value;
        }
        #endregion
    }
}
