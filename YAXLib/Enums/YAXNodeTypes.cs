// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.


namespace YAXLib.Enums;

/// <summary>
/// Enumerates possible XML node types upon which a property can be serialized.
/// </summary>
public enum YAXNodeTypes
{
    /// <summary>
    /// Serialize data as an attribute for the base element
    /// </summary>
    Attribute,

    /// <summary>
    /// Serialize data as an element
    /// </summary>
    Element,

    /// <summary>
    /// Serialize data as content of the element
    /// </summary>
    Content
}