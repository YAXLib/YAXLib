// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;

namespace YAXLib.Attributes
{
    /// <summary>
    ///     Specifies the format string provided for serializing data. The format string is the parameter
    ///     passed to the <c>ToString</c> method.
    ///     If this attribute is applied to collection classes, the format, therefore, is applied to
    ///     the collection members.
    ///     This attribute is applicable to fields and properties.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class YAXFormatAttribute : YAXBaseAttribute, IYaxMemberLevelAttribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="YAXFormatAttribute" /> class.
        /// </summary>
        /// <param name="format">The format string.</param>
        public YAXFormatAttribute(string format)
        {
            Format = format;
        }

        /// <summary>
        ///     Gets or sets the format string needed to serialize data. The format string is the parameter
        ///     passed to the <c>ToString</c> method.
        /// </summary>
        /// <value></value>
        public string Format { get; set; }

        /// <inheritdoc/>
        void IYaxMemberLevelAttribute.Setup(MemberWrapper memberWrapper)
        {
            memberWrapper.Format = Format;
        }
    }
}