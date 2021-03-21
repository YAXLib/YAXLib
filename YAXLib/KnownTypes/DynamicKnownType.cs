// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Xml.Linq;

namespace YAXLib
{
    internal abstract class DynamicKnownType : IKnownType
    {
        private Type _type;

        public abstract string TypeName { get; }

        public Type Type
        {
            get
            {
                if (_type == null)
                    _type = ReflectionUtils.GetTypeByName(TypeName);
                return _type;
            }
        }

        public abstract void Serialize(object obj, XElement elem, XNamespace overridingNamespace);

        public abstract object Deserialize(XElement elem, XNamespace overridingNamespace);
    }
}