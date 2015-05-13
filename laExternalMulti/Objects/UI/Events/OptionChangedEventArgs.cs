using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace laExternalMulti.Objects.UI.Events
{
    public class OptionChangedEventArgs : EventArgs
    {
        #region VARIABES
        private object optionValue;
        private int optionIndex;
        #endregion

        #region PROPERTIES
        public object OptionValue { get { return optionValue; } }
        public int OptionIndex { get { return optionIndex; } }
        #endregion

        #region CONSTRUCTOR
        public OptionChangedEventArgs(object optionValue, int optionIndex)
            : base()
        {
            this.optionValue = optionValue;
            this.optionIndex = optionIndex;
        }
        #endregion
    }
}
