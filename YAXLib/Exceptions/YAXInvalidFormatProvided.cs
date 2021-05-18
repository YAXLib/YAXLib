// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Globalization;

namespace YAXLib.Exceptions
{
    /// <summary>
    ///     Raised when an object cannot be formatted with the format string provided.
    ///     This exception is raised during serialization.
    /// </summary>
    [Obsolete("unused - will be removed in Yax 3")]
    public class YAXInvalidFormatProvided : YAXException
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="YAXInvalidFormatProvided" /> class.
        /// </summary>
        /// <param name="objectType">Type of the fiedl to serialize.</param>
        /// <param name="format">The format string.</param>
        public YAXInvalidFormatProvided(Type objectType, string format)
        {
            ObjectType = objectType;
            Format = format;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the type of the field to serialize
        /// </summary>
        /// <value>The type of the field to serialize.</value>
        public Type ObjectType { get; }

        /// <summary>
        ///     Gets the format string.
        /// </summary>
        /// <value>The format string.</value>
        public string Format { get; }

        /// <summary>
        ///     Gets a message that describes the current exception.
        /// </summary>
        /// <returns>
        ///     The error message that explains the reason for the exception, or an empty string("").
        /// </returns>
        public override string Message =>
            string.Format(
                CultureInfo.CurrentCulture,
                "Could not format objects of type '{0}' with the format string '{1}'",
                ObjectType.Name,
                Format);

        #endregion
    }
}