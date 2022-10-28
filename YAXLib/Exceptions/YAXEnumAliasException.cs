// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;

namespace YAXLib.Exceptions;

/// <summary>
/// The exception throws when trying to register an already existing <see cref="Enum" /> alias.
/// This exception is raised during serialization and deserialization.
/// </summary>
[Serializable]
public class YAXEnumAliasException : YAXException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="YAXEnumAliasException" /> class.
    /// </summary>
    public YAXEnumAliasException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="YAXEnumAliasException" /> class.
    /// </summary>
    /// <param name="message">The message.</param>
    public YAXEnumAliasException(string message)
        : base(message)
    {
    }
}