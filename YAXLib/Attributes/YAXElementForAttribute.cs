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
    public class YAXElementForAttribute : YAXBaseAttribute, IYaxMemberLevelAttribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="YAXElementForAttribute" /> class.
        /// </summary>
        /// <param name="parent">The element of which the property becomes a child element.</param>
        public YAXElementForAttribute(string parent)
        {
            Parent = parent;
        }

        /// <summary>
        ///     Gets or sets the element of which the property becomes a child element.
        /// </summary>
        /// <value>The element of which the property becomes a child element.</value>
        public string Parent { get; set; }

        /// <inheritdoc/>
        void IYaxMemberLevelAttribute.Setup(MemberWrapper memberWrapper)
        {
            memberWrapper.IsSerializedAsElement = true;

            StringUtils.ExtractPathAndAliasFromLocationString(Parent, out var path,
                out var alias);

            memberWrapper.SerializationLocation = path;
            if (!string.IsNullOrEmpty(alias))
                memberWrapper.Alias = StringUtils.RefineSingleElement(alias);
        }
    }
}