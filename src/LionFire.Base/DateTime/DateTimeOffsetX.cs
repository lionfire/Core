using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.ExtensionMethods;

public static class DateTimeOffsetX
{
    /// <summary>
    /// Assumes Offset of Zero
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static DateTimeOffset ToDateTimeOffset(this Int64 value) => new DateTimeOffset(DateTime.FromBinary(value), TimeSpan.Zero);
    public static DateTimeOffset ToDateTimeOffset(this Int64 value, TimeSpan timeSpan) => new DateTimeOffset(DateTime.FromBinary(value), timeSpan);
    public static long ToBinaryWithoutOffset(this DateTimeOffset dateTimeOffset)
        => dateTimeOffset.DateTime.ToBinary();
}
