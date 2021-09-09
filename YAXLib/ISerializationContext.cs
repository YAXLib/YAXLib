// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Reflection;

namespace YAXLib
{
    /// <summary>
    /// Provides information about which Types and/or members being (de)serialized.
    /// </summary>
    internal class SerializationContext : ISerializationContext
    {
        public SerializationContext(MemberWrapper memberWrapper, UdtWrapper udtWrapper)
        : this(memberWrapper)
        {
            ClassType = udtWrapper?.UnderlyingType;
        }

        public SerializationContext(MemberWrapper memberWrapper)
        {
            FieldInfo = memberWrapper?.FieldInfo;
            MemberInfo = memberWrapper?.MemberInfo;
            PropertyInfo = memberWrapper?.PropertyInfo;
            MemberType = memberWrapper?.MemberType;
        }

        /// <summary>
        /// The field info of the field.
        /// </summary>
        public FieldInfo FieldInfo { get; }

        /// <summary>
        /// The member info of the field.
        /// </summary>
        public MemberInfo MemberInfo { get; }

        /// <summary>
        /// The property info of the field.
        /// </summary>
        public PropertyInfo PropertyInfo { get; }

        /// <summary>
        /// The type of the member
        /// </summary>
        public Type MemberType { get; }

        /// <summary>
        /// The type of the class
        /// </summary>
        public Type ClassType { get; }
    }

    /// <summary>
    /// Provides information about which Types and/or members being (de)serialized.
    /// </summary>
    public interface ISerializationContext
    {
        /// <summary>
        /// The field info of the field.
        /// </summary>
        FieldInfo FieldInfo { get; }

        /// <summary>
        /// The member info of the field.
        /// </summary>
        MemberInfo MemberInfo { get; }

        /// <summary>
        /// The property info of the field.
        /// </summary>
        PropertyInfo PropertyInfo { get; }

        /// <summary>
        /// The type of the member
        /// </summary>
        Type MemberType { get; }

        /// <summary>
        /// The type of the class
        /// </summary>
        Type ClassType { get; }
    }
}
