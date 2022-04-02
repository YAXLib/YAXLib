// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

// The KnownDotNetTypes class is replaced by KnownTypes class.
// The new design for KnownTypes is adopted from Tomanu's YAXLib fork
// named YYAAXXLib. The fork can be found at:
// http://tomanuyyaaxxlib.codeplex.com/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace YAXLib
{
    /// <summary>
    ///     Provides serialization and deserialization methods for some known .NET built-in types.
    /// </summary>
    internal class KnownTypes
    {
        private static readonly Dictionary<Type, IKnownType> _dictKnownTypes = new();

        private static readonly Dictionary<string, IKnownType> _dictDynamicKnownTypes = new();

        static KnownTypes()
        {
            // Register all known types

            Add(new TimeSpanKnownType());
            Add(new XElementKnownType());
            Add(new XAttributeKnownType());
            Add(new DbNullKnownType());
            Add(new TypeKnownType());

            AddDynamicKnownType(new RectangleDynamicKnownType());
            AddDynamicKnownType(new ColorDynamicKnownType());
            AddDynamicKnownType(new RuntimeTypeDynamicKnownType());
            AddDynamicKnownType(new DataTableDynamicKnownType());
            AddDynamicKnownType(new DataSetDynamicKnownType());
        }

        public static void Add(IKnownType kt)
        {
            _dictKnownTypes[kt.Type] = kt;
        }

        public static void AddDynamicKnownType(DynamicKnownType dkt)
        {
            _dictDynamicKnownTypes[dkt.TypeName] = dkt;
        }

        public static bool IsKnowType(Type type)
        {
            return _dictKnownTypes.ContainsKey(type) || _dictDynamicKnownTypes.ContainsKey(type.FullName);
        }

        public static void Serialize(object obj, XElement elem, XNamespace overridingNamespace)
        {
            if (obj == null)
                return;

            if (_dictKnownTypes.ContainsKey(obj.GetType()))
                _dictKnownTypes[obj.GetType()].Serialize(obj, elem, overridingNamespace);
            else if (_dictDynamicKnownTypes.ContainsKey(obj.GetType().FullName))
                _dictDynamicKnownTypes[obj.GetType().FullName].Serialize(obj, elem, overridingNamespace);
        }

        public static object Deserialize(XElement elem, Type type, XNamespace overridingNamespace)
        {
            if (_dictKnownTypes.ContainsKey(type))
                return _dictKnownTypes[type].Deserialize(elem, overridingNamespace);
            if (_dictDynamicKnownTypes.ContainsKey(type.FullName))
                return _dictDynamicKnownTypes[type.FullName].Deserialize(elem, overridingNamespace);
            return null;
        }
    }
}