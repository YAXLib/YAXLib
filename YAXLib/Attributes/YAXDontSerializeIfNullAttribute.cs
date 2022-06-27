// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;

namespace YAXLib.Attributes
{
    /// <summary>
    ///     Prevents serialization of fields or properties when their value is null.
    ///     This attribute is applicable to fields and properties.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class YAXDontSerializeIfNullAttribute : YAXBaseAttribute, IYaxMemberLevelAttribute
    {
        /// <inheritdoc/>
        void IYaxMemberLevelAttribute.Setup(MemberWrapper memberWrapper)
        {
            memberWrapper.IsAttributedAsDontSerializeIfNull = true;
        }
    }
}