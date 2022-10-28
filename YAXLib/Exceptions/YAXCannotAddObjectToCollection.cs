// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Globalization;
using System.Xml;

namespace YAXLib.Exceptions;

/// <summary>
/// Raised when some member of the collection in the input XML, cannot be added to the collection object.
/// This exception is raised during deserialization.
/// </summary>
public class YAXCannotAddObjectToCollection : YAXDeserializationException
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="YAXCannotAddObjectToCollection" /> class.
    /// </summary>
    /// <param name="propName">Name of the property.</param>
    /// <param name="obj">The object that could not be added to the collection.</param>
    public YAXCannotAddObjectToCollection(string propName, object? obj) :
        this(propName, obj, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="YAXCannotAddObjectToCollection" /> class.
    /// </summary>
    /// <param name="propName">Name of the property.</param>
    /// <param name="obj">The object that could not be added to the collection.</param>
    /// <param name="lineInfo">IXmlLineInfo derived object, e.g. XElement, XAttribute containing line info</param>
    public YAXCannotAddObjectToCollection(string propName, object? obj, IXmlLineInfo? lineInfo) :
        base(lineInfo)
    {
        PropertyName = propName;
        ObjectToAdd = obj;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the name of the property.
    /// </summary>
    /// <value>The name of the property.</value>
    public string PropertyName { get; }

    /// <summary>
    /// Gets the object that could not be added to the collection.
    /// </summary>
    /// <value>the object that could not be added to the collection.</value>
    public object? ObjectToAdd { get; }

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
            "Could not add object ('{0}') to the collection ('{1}').{2}",
            ObjectToAdd ?? "(null)",
            PropertyName,
            LineInfoMessage);

    #endregion
}