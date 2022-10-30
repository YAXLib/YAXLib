// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Globalization;
using System.Xml;

namespace YAXLib.Exceptions;

/// <summary>
/// Raised when the attribute corresponding to some property is not present in the given XML file, when deserializing.
/// This exception is raised during deserialization.
/// </summary>
public class YAXAttributeMissingException : YAXDeserializationException
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="YAXAttributeMissingException" /> class.
    /// </summary>
    /// <param name="attrName">Name of the attribute.</param>
    public YAXAttributeMissingException(string attrName) :
        this(attrName, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="YAXAttributeMissingException" /> class.
    /// </summary>
    /// <param name="attrName">Name of the attribute.</param>
    /// <param name="lineInfo">IXmlLineInfo derived object, e.g. XElement, XAttribute containing line info</param>
    public YAXAttributeMissingException(string attrName, IXmlLineInfo? lineInfo) :
        base(lineInfo)
    {
        AttributeName = attrName;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the name of the attribute.
    /// </summary>
    /// <value>The name of the attribute.</value>
    public string AttributeName { get; }

    /// <summary>
    /// Gets a message that describes the current exception.
    /// </summary>
    /// <value></value>
    /// <returns>
    /// The error message that explains the reason for the exception, or an empty string("").
    /// </returns>
    public override string Message => string.Format(CultureInfo.CurrentCulture,
        "No attributes with this name found: '{0}'.{1}", AttributeName, LineInfoMessage);

    #endregion
}