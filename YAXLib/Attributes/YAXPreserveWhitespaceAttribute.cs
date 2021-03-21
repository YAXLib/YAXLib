// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;

namespace YAXLib
{
    /// <summary>
    ///     Adds the attribute xml:space="preserve" to the serialized element, so that the deserializer would
    ///     perserve all whitespace characters for the string values.
    ///     Add this attribute to any string field that you want their whitespace be preserved during
    ///     deserialization, or add it to the containing class to be applied to all its fields and properties.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class |
                    AttributeTargets.Struct)]
    public class YAXPreserveWhitespaceAttribute : YAXBaseAttribute
    {
    }
}