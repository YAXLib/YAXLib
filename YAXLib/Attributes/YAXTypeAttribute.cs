// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;

namespace YAXLib.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    internal class YAXTypeAttribute : YAXBaseAttribute, IYaxMemberLevelAttribute
    {
        public YAXTypeAttribute(Type type)
        {
            Type = type;
        }

        public Type Type { get; }

        public string Alias { get; set; }

        /// <inheritdoc/>
        void IYaxMemberLevelAttribute.Setup(MemberWrapper memberWrapper)
        {
            memberWrapper.AddAttributeToListOfRealTypes(this);
        }
    }
}