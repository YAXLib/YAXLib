// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;

namespace YAXLib.Exceptions
{
    /// <summary>
    ///     The base for all exception classes of YAXLib
    /// </summary>
    public abstract class YAXException : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="YAXException" /> class.
        /// </summary>
        public YAXException()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="YAXException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public YAXException(string message)
            : base(message)
        {
        }
    }
}