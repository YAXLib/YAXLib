// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using YAXLib.Enums;

namespace YAXLib.Attributes
{
    /// <summary>
    ///     Specifies the behavior of the deserialization method, if the element/attribute corresponding to this property is
    ///     missed in the XML input.
    ///     This attribute is applicable to fields and properties.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class YAXErrorIfMissedAttribute : YAXBaseAttribute, IYaxMemberLevelAttribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="YAXErrorIfMissedAttribute" /> class.
        /// </summary>
        /// <param name="treatAs">The value indicating this situation is going to be treated as Error or Warning.</param>
        public YAXErrorIfMissedAttribute(YAXExceptionTypes treatAs)
        {
            TreatAs = treatAs;
            DefaultValue = null;
        }

        /// <summary>
        ///     Gets or sets the value indicating this situation is going to be treated as Error or Warning.
        /// </summary>
        /// <value>The value indicating this situation is going to be treated as Error or Warning.</value>
        public YAXExceptionTypes TreatAs { get; set; }

        /// <summary>
        ///     Gets or sets the default value for the property if the element/attribute corresponding to this property is missed
        ///     in the XML input.
        ///     Setting <c>null</c> means do nothing.
        /// </summary>
        /// <value>The default value.</value>
        public object DefaultValue { get; set; }

        /// <inheritdoc/>
        void IYaxMemberLevelAttribute.Setup(MemberWrapper memberWrapper)
        {
            memberWrapper.TreatErrorsAs = TreatAs;
            memberWrapper.DefaultValue = DefaultValue;
        }
    }
}