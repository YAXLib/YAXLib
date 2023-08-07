// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Buffers;
using System.Text;

namespace YAXLib;

internal static class StringExtensions
{
    /// <summary>
    /// Encodes the given text to Base64, using the given <see cref="Encoding" /> (default: <see cref="Encoding.UTF8" />
    /// </summary>
    /// <param name="textToEncode"></param>
    /// <param name="encoding">Default is <see cref="Encoding.UTF8" />.</param>
    /// <param name="insertLineBreaks">Inserts line breaks after every 76 characters in the string representation.</param>
    /// <returns>The encoded string.</returns>
#if NETSTANDARD2_1_OR_GREATER || NET
    public static string? ToBase64([System.Diagnostics.CodeAnalysis.NotNullIfNotNull(nameof(textToEncode))] this string? textToEncode, Encoding? encoding
        = null, bool insertLineBreaks = true)
    {
        if (textToEncode == null) return null;

        encoding ??= Encoding.UTF8;

        var length = (textToEncode.Length + 2) / 3 * 4;
        var buffer = ArrayPool<char>.Shared.Rent(length);
        try
        {
            if (!Convert.TryToBase64Chars(encoding.GetBytes(textToEncode), buffer.AsSpan(), out var charsWritten,
                    insertLineBreaks ? Base64FormattingOptions.InsertLineBreaks : Base64FormattingOptions.None))
            {
                throw new FormatException("Error encoding text to Base64.");
            }

            return buffer.AsSpan(0, charsWritten).ToString();
        }
        finally
        {
            ArrayPool<char>.Shared.Return(buffer);
        }
    }
#else
        public static string? ToBase64(this string? textToEncode, Encoding? encoding = null,
            bool insertLineBreaks = true)
        {
            if (textToEncode == null) return null;

            encoding ??= Encoding.UTF8;

            var bytes = encoding.GetBytes(textToEncode);
            return Convert.ToBase64String(bytes,
                insertLineBreaks ? Base64FormattingOptions.InsertLineBreaks : Base64FormattingOptions.None);
        }
#endif
    /// <summary>
    /// Decodes the given text from Base64, using the given <see cref="Encoding" /> (default: <see cref="Encoding.UTF8" />
    /// </summary>
    /// <param name="encodedText"></param>
    /// <param name="encoding">Default is <see cref="Encoding.UTF8" />.</param>
    /// <returns>The decoded string.</returns>
#if NETSTANDARD2_1_OR_GREATER || NET
    public static string? FromBase64([System.Diagnostics.CodeAnalysis.NotNullIfNotNull(nameof(encodedText))] this string? encodedText, Encoding? encoding
        = null)
    {
        if (encodedText == null) return null;

        encoding ??= Encoding.UTF8;

        var length = ((encodedText.Length * 3) + 3) / 4;
        var buffer = ArrayPool<byte>.Shared.Rent(length);
        try
        {
            if (!Convert.TryFromBase64String(encodedText, buffer.AsSpan(), out var bytesWritten))
            {
                throw new FormatException("Invalid Base64 sequence.");
            }

            return encoding.GetString(buffer, 0, bytesWritten);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }
#else
        public static string? FromBase64(this string? encodedText, Encoding? encoding = null)
        {
            if (encodedText == null) return null;

            encoding ??= Encoding.UTF8;

            var bytes = Convert.FromBase64String(encodedText);
            return encoding.GetString(bytes);
        }
#endif
}
