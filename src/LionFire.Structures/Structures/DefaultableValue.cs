using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Structures
{
    public struct DefaultableValue<TValue>
    {
        #region Constants

        public static readonly DefaultableValue<TValue> Default = new DefaultableValue<TValue> { HasValue = false, Value = default };

        #endregion

        public bool HasValue { get; set; }
        public TValue Value { get; set; }

        public DefaultableValue(bool hasValue, TValue value)
        {
            HasValue = hasValue;
            Value = value;
        }        
        
        public static implicit operator DefaultableValue<TValue>((TValue value, bool hasValue) defaultableValue) => new DefaultableValue<TValue> { Value = defaultableValue.value, HasValue = defaultableValue.hasValue };
    }
}
