// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;

namespace YAXLib.Attributes
{
    /// <summary>
    ///     The base class for all attributes defined in YAXLib.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public abstract class YAXBaseAttribute : Attribute
    {
    }
}