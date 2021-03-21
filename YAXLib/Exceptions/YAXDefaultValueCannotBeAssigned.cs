﻿// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Globalization;
using System.Xml;

namespace YAXLib
{
    /// <summary>
    ///     Raised when the default value specified by the <c>YAXErrorIfMissedAttribute</c> could not be assigned to the
    ///     property.
    ///     This exception is raised during deserialization.
    /// </summary>
    public class YAXDefaultValueCannotBeAssigned : YAXDeserializationException
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="YAXDefaultValueCannotBeAssigned" /> class.
        /// </summary>
        /// <param name="propName">Name of the property.</param>
        /// <param name="defaultValue">The default value which caused the problem.</param>
        public YAXDefaultValueCannotBeAssigned(string propName, object defaultValue) :
            this(propName, defaultValue, null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="YAXDefaultValueCannotBeAssigned" /> class.
        /// </summary>
        /// <param name="propName">Name of the property.</param>
        /// <param name="defaultValue">The default value which caused the problem.</param>
        /// <param name="lineInfo">IXmlLineInfo derived object, e.g. XElement, XAttribute containing line info</param>
        public YAXDefaultValueCannotBeAssigned(string propName, object defaultValue, IXmlLineInfo lineInfo) :
            base(lineInfo)
        {
            PropertyName = propName;
            TheDefaultValue = defaultValue;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the name of the property.
        /// </summary>
        /// <value>The name of the property.</value>
        public string PropertyName { get; }

        /// <summary>
        ///     Gets the default value which caused the problem.
        /// </summary>
        /// <value>The default value which caused the problem.</value>
        public object TheDefaultValue { get; }

        /// <summary>
        ///     Gets a message that describes the current exception.
        /// </summary>
        /// <value></value>
        /// <returns>
        ///     The error message that explains the reason for the exception, or an empty string("").
        /// </returns>
        public override string Message =>
            string.Format(
                CultureInfo.CurrentCulture,
                "Could not assign the default value specified ('{0}') for the property '{1}'.{2}",
                TheDefaultValue,
                PropertyName,
                LineInfoMessage);

        #endregion
    }
}