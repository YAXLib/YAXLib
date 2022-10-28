// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.


namespace YAXLib.Enums;

/// <summary>
/// Enumerates different kinds of exception handling policies as used by YAX Library.
/// </summary>
public enum YAXExceptionHandlingPolicies
{
    /// <summary>
    /// Throws Both Warnings And Errors
    /// </summary>
    ThrowWarningsAndErrors,

    /// <summary>
    /// Throws Errors only (default)
    /// </summary>
    ThrowErrorsOnly,

    /// <summary>
    /// Does not throw exceptions, the errors can be accessed via the YAXParsingErrors instance
    /// </summary>
    DoNotThrow
}
