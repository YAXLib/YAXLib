// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

namespace YAXLib.Exceptions;

/// <summary>
/// Class for the <see cref="YAXPolymorphicException" />.
/// </summary>
public class YAXPolymorphicException : YAXException
{
    /// <summary>
    /// CTOR.
    /// </summary>
    /// <param name="message"></param>
    public YAXPolymorphicException(string message)
        : base(message)
    {
    }
}