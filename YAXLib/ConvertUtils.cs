// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;

namespace YAXLib;

internal static class ConvertUtils
{
    /// <summary>
    /// Determines if the specified value is an integer.
    /// </summary>
    /// <param name="value">The value to be checked for integer type.</param>
    /// <returns>
    /// <c>true</c> if the value is of a numeric integral type (sbyte, byte, short, ushort, int, uint, long, ulong); otherwise, <c>false</c>.
    /// </returns>
    public static bool IsInteger(object value)
    {
        switch (Type.GetTypeCode(value.GetType()))
        {
            case TypeCode.SByte:
            case TypeCode.Byte:
            case TypeCode.Int16:
            case TypeCode.UInt16:
            case TypeCode.Int32:
            case TypeCode.UInt32:
            case TypeCode.Int64:
            case TypeCode.UInt64:
                return true;
            default:
                return false;
        }
    }
}
