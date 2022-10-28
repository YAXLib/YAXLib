// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.


using System.Xml.Linq;

namespace YAXLib.Enums;

/// <summary>
/// Options for embedding a string value to an <see cref="XElement" />.
/// </summary>
public enum TextEmbedding
{
    /// <summary>
    /// No embedding. Legal special characters like '&lt;' or '&amp;' will be entitized.
    /// </summary>
    None = 0,

    /// <summary>
    /// The element text is embedded as character data (CDATA, <see cref="XCData" />).
    /// <para>
    /// This is useful, e.g. if a <see langword="string" /> contains HTML tags or script code.
    /// The result is better human-readable.
    /// </para>
    /// </summary>
    CData = 1,

    /// <summary>
    /// Embedded by encoding to a Base64-encoded string, and restored from a Base64-encoded string.
    /// This is useful, if a <see langword="string" /> may contain control or other
    /// invalid XML characters in an <see cref="XElement" /> value.
    /// </summary>
    Base64 = 2
}
