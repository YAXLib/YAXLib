// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Xml.Linq;

namespace YAXLib
{
    internal class RuntimeTypeDynamicKnownType : DynamicKnownType
    {
        public override string TypeName => "System.RuntimeType";

        public override void Serialize(object obj, XElement elem, XNamespace overridingNamespace)
        {
            var objectType = obj.GetType();
            if (objectType.FullName != TypeName)
                throw new ArgumentException("Object type does not match the provided typename", "obj");

            elem.Value = ReflectionUtils.InvokeGetProperty<string>(obj, "FullName");
        }

        public override object Deserialize(XElement elem, XNamespace overridingNamespace)
        {
            return ReflectionUtils.GetTypeByName(elem.Value);
        }
    }
}