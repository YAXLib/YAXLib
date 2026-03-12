// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

namespace YAXLibTests.TestHelpers;

internal static class StringExtensions
{
    /// <summary>
    /// Normalizes all line endings to LF so that XML string comparisons
    /// are not affected by OS-specific or Git-checkout line endings.
    /// </summary>
    public static string NormalizeLineEndings(this string text)
        => text.Replace("\r\n", "\n").Replace("\r", "\n");
}
