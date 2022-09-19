// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Globalization;

namespace YAXLib.Exceptions;

/// <summary>
/// Raised when the location of serialization specified cannot be
/// created or cannot be read from.
/// This exception is raised during serialization
/// </summary>
public class YAXBadLocationException : YAXException
{
    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="YAXBadLocationException" /> class.
    /// </summary>
    /// <param name="location">The location.</param>
    public YAXBadLocationException(string location)
    {
        Location = location;
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets or sets the bad location which caused the exception
    /// </summary>
    /// <value>The location.</value>
    public string Location { get; }

    /// <summary>
    /// Gets a message that describes the current exception.
    /// </summary>
    /// <value></value>
    /// <returns>
    /// The error message that explains the reason for the exception, or an empty string("").
    /// </returns>
    public override string Message => string.Format(CultureInfo.CurrentCulture,
        "The location specified cannot be read from or written to: {0}", Location);

    #endregion
}