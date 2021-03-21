// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;

namespace YAXLib
{
    /// <summary>
    ///     Makes a field or property to appear as a value for another element, if possible.
    ///     This attribute is applicable to fields and properties.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class YAXValueForAttribute : YAXBaseAttribute
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="YAXAttributeForAttribute" /> class.
        /// </summary>
        /// <param name="parent">The element of which the property becomes an attribute.</param>
        public YAXValueForAttribute(string parent)
        {
            Parent = parent;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the element for which the property becomes a value.
        /// </summary>
        public string Parent { get; set; }

        #endregion
    }
}