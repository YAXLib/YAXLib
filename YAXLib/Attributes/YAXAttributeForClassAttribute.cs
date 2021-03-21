﻿// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;

namespace YAXLib
{
    /// <summary>
    ///     Makes a property to appear as an attribute for the enclosing class (i.e. the parent element) if possible.
    ///     This attribute is applicable to fields and properties only.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class YAXAttributeForClassAttribute : YAXBaseAttribute
    {
    }
}