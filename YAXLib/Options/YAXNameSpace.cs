// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Xml.Linq;

namespace YAXLib.Options;

/// <summary>
/// XML Namespace definitions for the <see cref="YAXSerializer" />.
/// </summary>
public class YAXNameSpace
{
    /// <summary>
    /// The URI address which holds the xmlns:yaxlib definition.
    /// </summary>
    public XNamespace Uri { get; set; } = XNamespace.None;

    /// <summary>
    /// The prefix used for the xml namespace.
    /// </summary>
    public string Prefix { get; set; } = string.Empty;
}