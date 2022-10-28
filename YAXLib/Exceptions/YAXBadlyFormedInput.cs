// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Globalization;
using System.Xml;

namespace YAXLib.Exceptions;

/// <summary>
/// Raised when the value provided for some property in the XML input, cannot be
/// converted to the type of the property.
/// This exception is raised during deserialization.
/// </summary>
public class YAXBadlyFormedInput : YAXDeserializationException
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="YAXBadlyFormedInput" /> class.
    /// </summary>
    /// <param name="elemName">Name of the element.</param>
    /// <param name="badInput">The value of the input which could not be converted to the type of the property.</param>
    public YAXBadlyFormedInput(string elemName, string badInput)
        : this(elemName, badInput, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="YAXBadlyFormedInput" /> class.
    /// </summary>
    /// <param name="elemName">Name of the element.</param>
    /// <param name="badInput">The value of the input which could not be converted to the type of the property.</param>
    /// <param name="lineInfo">IXmlLineInfo derived object, e.g. XElement, XAttribute containing line info</param>
    public YAXBadlyFormedInput(string elemName, string badInput, IXmlLineInfo? lineInfo)
        : base(lineInfo)
    {
        ElementName = elemName;
        BadInput = badInput;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the name of the element.
    /// </summary>
    /// <value>The name of the element.</value>
    public string ElementName { get; }

    /// <summary>
    /// Gets the value of the input which could not be converted to the type of the property.
    /// </summary>
    /// <value>The value of the input which could not be converted to the type of the property.</value>
    public string BadInput { get; }

    /// <summary>
    /// Gets a message that describes the current exception.
    /// </summary>
    /// <value></value>
    /// <returns>
    /// The error message that explains the reason for the exception, or an empty string("").
    /// </returns>
    public override string Message =>
        string.Format(
            CultureInfo.CurrentCulture,
            "The format of the value specified for the property '{0}' is not proper: '{1}'.{2}",
            ElementName,
            BadInput,
            LineInfoMessage);

    #endregion
}