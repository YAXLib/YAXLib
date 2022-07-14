// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using YAXLib.Enums;

namespace YAXLib.Attributes
{
    /// <summary>
    ///     Controls the serialization of collection instances.
    ///     This attribute is applicable to fields and properties, and collection classes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class |
                    AttributeTargets.Struct)]
    public class YAXCollectionAttribute : YAXBaseAttribute, IYaxMemberLevelAttribute, IYaxTypeLevelAttribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="YAXCollectionAttribute" /> class.
        /// </summary>
        /// <param name="serType">type of the serialization of the collection.</param>
        public YAXCollectionAttribute(YAXCollectionSerializationTypes serType)
        {
            SerializationType = serType;
            SeparateBy = " ";
            EachElementName = null;
            IsWhiteSpaceSeparator = true;
        }

        /// <summary>
        ///     Gets or sets the type of the serialization of the collection.
        /// </summary>
        /// <value>The type of the serialization of the collection.</value>
        public YAXCollectionSerializationTypes SerializationType { get; set; }

        /// <summary>
        ///     Gets or sets the string to separate collection items, if the Serialization type is set to <c>Serially</c>.
        /// </summary>
        /// <value>the string to separate collection items, if the Serialization Type is set to <c>Serially</c>.</value>
        public string SeparateBy { get; set; }

        /// <summary>
        ///     Gets or sets the name of each child element corresponding to the collection members, if the Serialization type is
        ///     set to <c>Recursive</c>.
        /// </summary>
        /// <value>
        ///     The name of each child element corresponding to the collection members, if the Serialization type is set to
        ///     <c>Recursive</c>.
        /// </value>
        public string EachElementName { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether white space characters are to be
        ///     treated as separators or not.
        /// </summary>
        /// <value>
        ///     <c>true</c> if white space separator characters are to be
        ///     treated as separators; otherwise, <c>false</c>.
        /// </value>
        public bool IsWhiteSpaceSeparator { get; set; }

        /// <inheritdoc/>
        void IYaxMemberLevelAttribute.Setup(MemberWrapper memberWrapper)
        {
            memberWrapper.CollectionAttributeInstance = this;
        }

        /// <inheritdoc/>
        void IYaxTypeLevelAttribute.Setup(UdtWrapper udtWrapper)
        {
            udtWrapper.CollectionAttributeInstance = this;
        }
    }
}