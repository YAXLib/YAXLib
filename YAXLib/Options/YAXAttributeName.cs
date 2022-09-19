// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

namespace YAXLib.Options;

/// <summary>
/// Definitions for special attribute names.
/// </summary>
public class YAXAttributeName
{
    /// <summary>
    /// The attribute name used to de-serialize meta-data for multi-dimensional arrays.
    /// </summary>
    public string Dimensions { get; set; } = string.Empty;

    /// <summary>
    /// The attribute name used to de-serialize meta-data for real types of objects serialized through
    /// a reference to their base class or interface.
    /// </summary>
    public string RealType { get; set; } = string.Empty;
}