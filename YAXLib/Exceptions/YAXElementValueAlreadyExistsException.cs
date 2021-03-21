// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Globalization;
using System.Xml;

namespace YAXLib
{
    /// <summary>
    ///     Raised when the element value corresponding to some property is not present in the given XML file, when
    ///     deserializing.
    ///     This exception is raised during deserialization.
    /// </summary>
    [Obsolete("unused - will be removed in Yax 3")]
    public class YAXElementValueAlreadyExistsException : YAXDeserializationException
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="YAXAttributeMissingException" /> class.
        /// </summary>
        /// <param name="elementName">Name of the element.</param>
        public YAXElementValueAlreadyExistsException(string elementName) :
            this(elementName, null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="YAXAttributeMissingException" /> class.
        /// </summary>
        /// <param name="elementName">Name of the element.</param>
        /// <param name="lineInfo">IXmlLineInfo derived object, e.g. XElement, XAttribute containing line info</param>
        public YAXElementValueAlreadyExistsException(string elementName, IXmlLineInfo lineInfo) :
            base(lineInfo)
        {
            ElementName = elementName;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the name of the attribute.
        /// </summary>
        /// <value>The name of the attribute.</value>
        public string ElementName { get; }

        /// <summary>
        ///     Gets a message that describes the current exception.
        /// </summary>
        /// <value></value>
        /// <returns>
        ///     The error message that explains the reason for the exception, or an empty string("").
        /// </returns>
        public override string Message => string.Format(CultureInfo.CurrentCulture,
            "Element with the given name already has value: '{0}'.{1}", ElementName, LineInfoMessage);

        #endregion
    }
}