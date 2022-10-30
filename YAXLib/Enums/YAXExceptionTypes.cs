// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.


namespace YAXLib.Enums;

/// <summary>
/// Enumerates different possible behaviors of the library toward exceptions
/// </summary>
public enum YAXExceptionTypes
{
    /// <summary>
    /// Ignore non-fatal exceptions; neither throw them, nor log them.
    /// </summary>
    Ignore,

    /// <summary>
    /// Treat exception as a warning
    /// </summary>
    Warning,

    /// <summary>
    /// Treat exception as an error
    /// </summary>
    Error
}
