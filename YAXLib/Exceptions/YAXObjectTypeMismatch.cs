// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Globalization;

namespace YAXLib.Exceptions;

/// <summary>
/// Raised when the object provided for serialization is not of the type provided for the serializer. This exception
/// cannot be turned off.
/// This exception is raised during serialization.
/// </summary>
public class YAXObjectTypeMismatch : YAXException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="YAXObjectTypeMismatch" /> class.
    /// </summary>
    /// <param name="expectedType">The expected type.</param>
    /// <param name="receivedType">The type of the object which did not match the expected type.</param>
    public YAXObjectTypeMismatch(Type expectedType, Type? receivedType)
    {
        ExpectedType = expectedType;
        ReceivedType = receivedType;
    }

    /// <summary>
    /// Gets the expected type.
    /// </summary>
    /// <value>The expected type.</value>
    public Type ExpectedType { get; }

    /// <summary>
    /// Gets the type of the object which did not match the expected type.
    /// </summary>
    /// <value>The type of the object which did not match the expected type.</value>
    public Type? ReceivedType { get; }

    /// <summary>
    /// Gets a message that describes the current exception.
    /// </summary>
    /// <returns>
    /// The error message that explains the reason for the exception, or an empty string("").
    /// </returns>
    public override string Message =>
        string.Format(
            CultureInfo.CurrentCulture,
            "Expected an object of type '{0}' but received an object of type '{1}'.",
            ExpectedType.Name,
            ReceivedType?.Name ?? "null");
}