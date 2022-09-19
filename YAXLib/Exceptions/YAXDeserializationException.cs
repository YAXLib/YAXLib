// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Globalization;
using System.Xml;

namespace YAXLib.Exceptions;

/// <summary>
/// Base class for all deserialization exceptions, which contains line and position info
/// </summary>
public class YAXDeserializationException : YAXException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="YAXDeserializationException" /> class.
    /// </summary>
    /// <param name="lineInfo">IXmlLineInfo derived object, e.g. XElement, XAttribute containing line info</param>
    /// <param name="message">The message with exception details.</param>
    public YAXDeserializationException(IXmlLineInfo? lineInfo, string message = "") : base(message)
    {
        if (lineInfo != null &&
            lineInfo.HasLineInfo())
        {
            HasLineInfo = true;
            LineNumber = lineInfo.LineNumber;
            LinePosition = lineInfo.LinePosition;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="YAXDeserializationException" /> class.
    /// </summary>
    /// <param name="lineNumber">The line number on which the error occurred</param>
    /// <param name="linePosition">The line position on which the error occurred</param>
    /// <param name="message">The message with exception details.</param>
    public YAXDeserializationException(int lineNumber, int linePosition, string message = "") : base(message)
    {
        HasLineInfo = lineNumber != 0 && linePosition != 0;
        LineNumber = lineNumber;
        LinePosition = linePosition;
    }

    /// <summary>
    /// Gets whether the exception has line information
    /// Note: if this is unexpectedly false, then most likely you need to specify LoadOptions.SetLineInfo on document load
    /// </summary>
    public bool HasLineInfo { get; }

    /// <summary>
    /// Gets the line number on which the exception occurred
    /// </summary>
    public int LineNumber { get; }

    /// <summary>
    /// Gets the position at which the exception occurred
    /// </summary>
    public int LinePosition { get; }

    /// <summary>
    /// Position string for use in error message
    /// </summary>
    protected string LineInfoMessage => HasLineInfo
        ? string.Format(CultureInfo.CurrentCulture, " Line {0}, position {1}.", LineNumber, LinePosition)
        : string.Empty;
}