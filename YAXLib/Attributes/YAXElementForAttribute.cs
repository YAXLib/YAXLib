// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;

namespace YAXLib.Attributes
{
    /// <summary>
    ///     Makes a property or field to appear as a child element
    ///     for another element. This attribute is applicable to fields and properties.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class YAXElementForAttribute : YAXBaseAttribute
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="YAXElementForAttribute" /> class.
        /// </summary>
        /// <param name="parent">The element of which the property becomes a child element.</param>
        public YAXElementForAttribute(string parent)
        {
            Parent = parent;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the element of which the property becomes a child element.
        /// </summary>
        /// <value>The element of which the property becomes a child element.</value>
        public string Parent { get; set; }

        #endregion
    }
}