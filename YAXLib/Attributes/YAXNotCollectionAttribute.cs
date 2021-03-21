// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;

namespace YAXLib
{
    /// <summary>
    ///     Specifies that a particular class, or a particular property or variable type, that is
    ///     driven from <c>IEnumerable</c> should not be treated as a collection class/object.
    ///     This attribute is applicable to fields, properties, classes, and structs.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class |
                    AttributeTargets.Struct)]
    public class YAXNotCollectionAttribute : YAXBaseAttribute
    {
    }
}