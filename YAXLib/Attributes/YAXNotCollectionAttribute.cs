// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;

namespace YAXLib.Attributes
{
    /// <summary>
    ///     Specifies that a particular class, or a particular property or variable type, that is
    ///     driven from <c>IEnumerable</c> should not be treated as a collection class/object.
    ///     This attribute is applicable to fields, properties, classes, and structs.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class |
                    AttributeTargets.Struct)]
    public class YAXNotCollectionAttribute : YAXBaseAttribute, IYaxMemberLevelAttribute, IYaxTypeLevelAttribute
    {
        /// <inheritdoc/>
        void IYaxMemberLevelAttribute.Setup(MemberWrapper memberWrapper)
        {
            // arrays are always treated as collections
            if (!ReflectionUtils.IsArray(memberWrapper.MemberType))
                memberWrapper.IsAttributedAsNotCollection = true;
        }

        /// <inheritdoc/>
        void IYaxTypeLevelAttribute.Setup(UdtWrapper udtWrapper)
        {
            // arrays are always treated as collections
            if (!ReflectionUtils.IsArray(udtWrapper.UnderlyingType))
                udtWrapper.IsAttributedAsNotCollection = true;
        }
    }
}