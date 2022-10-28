// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Globalization;

namespace YAXLib.Exceptions;

/// <summary>
/// Raised when trying to serialize an attribute where
/// another attribute with the same name already exists.
/// This exception is raised during serialization.
/// </summary>
public class YAXAttributeAlreadyExistsException : YAXException
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="YAXAttributeAlreadyExistsException" /> class.
    /// </summary>
    /// <param name="attrName">Name of the attribute.</param>
    public YAXAttributeAlreadyExistsException(string attrName)
    {
        AttrName = attrName;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the name of the attribute.
    /// </summary>
    /// <value>The name of the attribute.</value>
    public string AttrName { get; }

    /// <summary>
    /// Gets a message that describes the current exception.
    /// </summary>
    /// <value></value>
    /// <returns>
    /// The error message that explains the reason for the exception, or an empty string("").
    /// </returns>
    public override string Message => string.Format(CultureInfo.CurrentCulture,
        "An attribute with this name already exists: '{0}'.", AttrName);

    #endregion
}