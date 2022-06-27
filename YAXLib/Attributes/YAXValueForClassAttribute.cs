// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;

namespace YAXLib.Attributes
{
    /// <summary>
    ///     Makes a field or property to appear as a value for its parent element, if possible.
    ///     This attribute is applicable to fields and properties.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class YAXValueForClassAttribute : YAXBaseAttribute, IYaxMemberLevelAttribute
    {
        /// <inheritdoc/>
        void IYaxMemberLevelAttribute.Setup(MemberWrapper memberWrapper)
        {
            if (memberWrapper.IsAllowedToProcess())            
            {
                memberWrapper.IsSerializedAsValue = true;
                memberWrapper.SerializationLocation = ".";
            }
        }
    }
}