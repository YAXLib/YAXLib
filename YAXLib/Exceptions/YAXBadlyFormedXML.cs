// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Globalization;

namespace YAXLib.Exceptions;

/// <summary>
/// Raised when the XML input does not follow standard XML formatting rules.
/// This exception is raised during deserialization.
/// </summary>
public class YAXBadlyFormedXML : YAXDeserializationException
{
    #region Fields

    /// <summary>
    /// The inner exception.
    /// </summary>
    private readonly Exception? _innerException = null;

    #endregion

    #region Properties

    /// <summary>
    /// Gets a message that describes the current exception.
    /// </summary>
    /// <value></value>
    /// <returns>
    /// The error message that explains the reason for the exception, or an empty string("").
    /// </returns>
    public override string Message
    {
        get
        {
            var msg = string.Format(CultureInfo.CurrentCulture, "The input xml file is not properly formatted!{0}",
                LineInfoMessage);

            if (_innerException != null)
                msg += string.Format(CultureInfo.CurrentCulture, "{0}More Details:{1}{2}", Environment.NewLine,
                    _innerException.Message, Environment.NewLine);

            return msg;
        }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="YAXBadlyFormedXML" /> class.
    /// </summary>
    /// <param name="innerException">The inner exception.</param>
    /// <param name="lineNumber">The line number on which the error occurred</param>
    /// <param name="linePosition">The line position on which the error occurred</param>
    public YAXBadlyFormedXML(Exception? innerException, int lineNumber, int linePosition)
        : base(lineNumber, linePosition)
    {
        _innerException = innerException;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="YAXBadlyFormedXML" /> class.
    /// </summary>
    /// <param name="innerException">The inner exception.</param>
    public YAXBadlyFormedXML(Exception? innerException)
        : base(null)
    {
        _innerException = innerException;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="YAXBadlyFormedXML" /> class.
    /// </summary>
    public YAXBadlyFormedXML()
        : this(null)
    {
    }

    #endregion
}