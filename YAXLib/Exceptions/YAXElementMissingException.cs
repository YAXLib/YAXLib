// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Globalization;
using System.Xml;

namespace YAXLib.Exceptions;

/// <summary>
/// Raised when the element corresponding to some property is not present in the given XML file, when deserializing.
/// This exception is raised during deserialization.
/// </summary>
public class YAXElementMissingException : YAXDeserializationException
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="YAXElementMissingException" /> class.
    /// </summary>
    /// <param name="elemName">Name of the element.</param>
    public YAXElementMissingException(string elemName) :
        this(elemName, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="YAXElementMissingException" /> class.
    /// </summary>
    /// <param name="elemName">Name of the element.</param>
    /// <param name="lineInfo">IXmlLineInfo derived object, e.g. XElement, XAttribute containing line info</param>
    public YAXElementMissingException(string elemName, IXmlLineInfo? lineInfo) :
        base(lineInfo)
    {
        ElementName = elemName;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or the name of the element.
    /// </summary>
    /// <value>The name of the element.</value>
    public string ElementName { get; }

    /// <summary>
    /// Gets a message that describes the current exception.
    /// </summary>
    /// <value></value>
    /// <returns>
    /// The error message that explains the reason for the exception, or an empty string("").
    /// </returns>
    public override string Message => string.Format(CultureInfo.CurrentCulture,
        "No elements with this name found: '{0}'.{1}", ElementName, LineInfoMessage);

    #endregion
}