// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;

namespace YAXLib.Attributes
{
    /// <summary>
    ///     Makes a field or property to appear as a value for another element, if possible.
    ///     This attribute is applicable to fields and properties.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class YAXValueForAttribute : YAXBaseAttribute, IYaxMemberLevelAttribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="YAXAttributeForAttribute" /> class.
        /// </summary>
        /// <param name="parent">The element of which the property becomes an attribute.</param>
        public YAXValueForAttribute(string parent)
        {
            Parent = parent;
        }

        /// <summary>
        ///     Gets or sets the element for which the property becomes a value.
        /// </summary>
        public string Parent { get; set; }
        
        /// <inheritdoc/>
        void IYaxMemberLevelAttribute.Setup(MemberWrapper memberWrapper)
        {
            if (memberWrapper.IsAllowedToProcess())            
            {
                memberWrapper.IsSerializedAsValue = true;

                StringUtils.ExtractPathAndAliasFromLocationString(Parent, out var path,
                    out var alias);

                memberWrapper.SerializationLocation = path;
                if (!string.IsNullOrEmpty(alias))
                    memberWrapper.Alias = StringUtils.RefineSingleElement(alias);
            }
        }
        
    }
}