// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Globalization;

namespace YAXLib.Exceptions;

/// <summary>
/// Raised when trying to serialize self-referential types. This exception cannot be turned off.
/// This exception is raised during serialization.
/// </summary>
public class YAXCannotSerializeSelfReferentialTypes : YAXException
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="YAXCannotSerializeSelfReferentialTypes" /> class.
    /// </summary>
    /// <param name="type">The the self-referential type that caused the problem.</param>
    public YAXCannotSerializeSelfReferentialTypes(Type type)
    {
        SelfReferentialType = type;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the self-referential type that caused the problem.
    /// </summary>
    /// <value>The type of the self-referential type that caused the problem.</value>
    public Type SelfReferentialType { get; }

    /// <summary>
    /// Gets a message that describes the current exception.
    /// </summary>
    /// <returns>
    /// The error message that explains the reason for the exception, or an empty string("").
    /// </returns>
    public override string Message => string.Format(CultureInfo.CurrentCulture,
        "Self Referential types ('{0}') cannot be serialized.", SelfReferentialType.FullName);

    #endregion
}