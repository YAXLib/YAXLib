// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;

namespace YAXLib.Attributes
{
    /// <summary>
    ///     Defines an alias for the field, property, class, or struct under
    ///     which it will be serialized. This attribute is applicable to fields,
    ///     properties, classes, and structs.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class |
                    AttributeTargets.Struct)]
    public class YAXSerializeAsAttribute : YAXBaseAttribute, IYaxMemberLevelAttribute, IYaxTypeLevelAttribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="YAXSerializeAsAttribute" /> class.
        /// </summary>
        /// <param name="serializeAs">the alias for the property under which the property will be serialized.</param>
        public YAXSerializeAsAttribute(string serializeAs)
        {
            SerializeAs = serializeAs;
        }

        /// <summary>
        ///     Gets or sets the alias for the property under which the property will be serialized.
        /// </summary>
        public string SerializeAs { get; set; }

        /// <inheritdoc/>
        void IYaxMemberLevelAttribute.Setup(MemberWrapper memberWrapper)
        {
            memberWrapper.Alias = StringUtils.RefineSingleElement(SerializeAs);
        }

        /// <inheritdoc/>
        void IYaxTypeLevelAttribute.Setup(UdtWrapper udtWrapper)
        {
            udtWrapper.Alias = StringUtils.RefineSingleElement(SerializeAs);
        }
    }
}