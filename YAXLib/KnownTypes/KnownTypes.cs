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
        private static readonly Dictionary<Type, IKnownType> s_dictKnownTypes = new Dictionary<Type, IKnownType>();

        private static readonly Dictionary<string, IKnownType> s_dictDynamicKnownTypes =
            new Dictionary<string, IKnownType>();

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
        }

        public static void Add(IKnownType kt)
        {
            s_dictKnownTypes[kt.Type] = kt;
        }

        public static void AddDynamicKnownType(DynamicKnownType dkt)
        {
            s_dictDynamicKnownTypes[dkt.TypeName] = dkt;
        }

        public static bool IsKnowType(Type type)
        {
            return s_dictKnownTypes.ContainsKey(type) || s_dictDynamicKnownTypes.ContainsKey(type.FullName);
        }

        public static void Serialize(object obj, XElement elem, XNamespace overridingNamespace)
        {
            if (obj == null)
                return;

            if (s_dictKnownTypes.ContainsKey(obj.GetType()))
                s_dictKnownTypes[obj.GetType()].Serialize(obj, elem, overridingNamespace);
            else if (s_dictDynamicKnownTypes.ContainsKey(obj.GetType().FullName))
                s_dictDynamicKnownTypes[obj.GetType().FullName].Serialize(obj, elem, overridingNamespace);
        }

        public static object Deserialize(XElement elem, Type type, XNamespace overridingNamespace)
        {
            if (s_dictKnownTypes.ContainsKey(type))
                return s_dictKnownTypes[type].Deserialize(elem, overridingNamespace);
            if (s_dictDynamicKnownTypes.ContainsKey(type.FullName))
                return s_dictDynamicKnownTypes[type.FullName].Deserialize(elem, overridingNamespace);
            return null;
        }
    }

    #region XElement

    // Thanks go to CodePlex user tg73: 
    // http://www.codeplex.com/site/users/view/tg73
    // for providing this implementation in the following issue:
    // http://yaxlib.codeplex.com/workitem/17676

    #endregion

    #region XAttribute

    #endregion

    #region TimeSpan

    #endregion

    #region DBNull

    #endregion
}