using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.ExtensionMethods;

public static class NumericTypesX
{
    public static bool IsNumeric(this Type t)
    {
        return t.IsInteger() || t.IsFloat();
    }

    public static bool IsInteger(this Type t)
    {
        return Type.GetTypeCode(t) switch
        {
            TypeCode.Int16 => true,
            TypeCode.Int32 => true,
            TypeCode.Int64 => true,
            TypeCode.UInt16 => true,
            TypeCode.UInt32 => true,
            TypeCode.UInt64 => true,
            TypeCode.Byte => true,
            TypeCode.SByte => true,
            _ => false,
        };
    }

    public static bool IsFloat(this Type t)
    {
        return Type.GetTypeCode(t) switch
        {
            TypeCode.Single => true,
            TypeCode.Double => true,
            TypeCode.Decimal => true,
            _ => false,
        };
    }
}
