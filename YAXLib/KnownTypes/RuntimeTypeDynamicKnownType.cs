﻿// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

#nullable enable
using System;
using System.Xml.Linq;

namespace YAXLib.KnownTypes
{
    internal class RuntimeTypeDynamicKnownType : DynamicKnownTypeBase
    {
        /// <inheritdoc />
        public override string TypeName => "System.RuntimeType";

        /// <inheritdoc />
        public override void Serialize(object? obj, XElement elem, XNamespace overridingNamespace,
            ISerializationContext serializationContext)
        {
            var objectType = obj?.GetType();
            if (objectType?.FullName != TypeName)
                throw new ArgumentException("Object type does not match the provided typename", nameof(obj));

            elem.Value = ReflectionUtils.InvokeGetProperty<string>(obj, "FullName");
        }

        /// <inheritdoc />
        public override object Deserialize(XElement elem, XNamespace overridingNamespace,
            ISerializationContext serializationContext)
        {
            return ReflectionUtils.GetTypeByName(elem.Value);
        }
    }
}
