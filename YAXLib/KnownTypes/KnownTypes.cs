// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using YAXLib.Options;

namespace YAXLib
{
    /// <summary>
    ///     Provides serialization and deserialization methods for some known .NET built-in types.
    /// </summary>
    internal class KnownTypes
    {
        private static readonly Dictionary<Type, IKnownType> _dictKnownBaseTypes = new ();

        private static readonly Dictionary<Type, IKnownType> _dictKnownTypes = new ();

        private static readonly Dictionary<string, IKnownType> _dictDynamicKnownTypes = new();

        static KnownTypes()
        {
            // NOTE: known-types MUST be registered here
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

            AddKnownBaseType(new ExceptionKnownBaseType());
        }

        public static void Add(IKnownType kt)
        {
            _dictKnownTypes[kt.Type] = kt;
        }

        public static void AddKnownBaseType(IKnownType kt)
        {
            _dictKnownBaseTypes[kt.Type] = kt;
        }

        public static void AddDynamicKnownType(DynamicKnownType dkt)
        {
            _dictDynamicKnownTypes[dkt.TypeName] = dkt;
        }

        public static bool TryGetKnownType(Type type, SerializerOptions options, out IKnownType knownType)
        {
            knownType = null;

            if (type == null)
                return false;

            if (_dictKnownTypes.ContainsKey(type))
            {
                knownType = _dictKnownTypes[type];
                knownType.Options = options;
                return true;
            }
            if (type.FullName != null && _dictDynamicKnownTypes.ContainsKey(type.FullName))
            {
                knownType = _dictDynamicKnownTypes[type.FullName];
                knownType.Options = options;
                return true;
            }
            if (_dictKnownBaseTypes.Keys.Any(k => ReflectionUtils.IsBaseClassOrSubclassOf(type, k.FullName)))
            {
                knownType = _dictKnownBaseTypes[_dictKnownBaseTypes.Keys.First(k => ReflectionUtils.IsBaseClassOrSubclassOf(type, k.FullName))];
                knownType.Options = options;
                return true;
            }

            return false;
        }
    }
}