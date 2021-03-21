﻿// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;

namespace YAXLib
{
    /// <summary>
    ///     Add this attribute to properties or fields which you wish to be serialized, when
    ///     the enclosing class uses the <c>YAXSerializableType</c> attribute in which <c>FieldsToSerialize</c>
    ///     has been set to <c>AttributedFieldsOnly</c>.
    ///     This attribute is applicable to fields and properties.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class YAXSerializableFieldAttribute : YAXBaseAttribute
    {
    }
}