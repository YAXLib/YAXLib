﻿// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
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

    internal interface IKnownType
    {
        /// <summary>
        ///     Gets the underlying known type.
        /// </summary>
        Type Type { get; }

        /// <summary>
        ///     Serializes the specified object int the specified XML element.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="elem">The XML element.</param>
        /// <param name="overridingNamespace">The namespace the element belongs to.</param>
        void Serialize(object obj, XElement elem, XNamespace overridingNamespace);

        /// <summary>
        ///     Deserializes the specified XML element to the known type.
        /// </summary>
        /// <param name="elem">The XML element to deserialize object from.</param>
        /// <param name="overridingNamespace">The namespace the element belongs to.</param>
        /// <returns>The deserialized object</returns>
        object Deserialize(XElement elem, XNamespace overridingNamespace);
    }

    internal static class KnownTypeExtensions
    {
        internal static XName GetXName(this IKnownType self, string name, XNamespace overridingNamespace)
        {
            if (overridingNamespace.IsEmpty())
                return XName.Get(name, overridingNamespace.NamespaceName);
            return XName.Get(name);
        }
    }

    /// <summary>
    ///     Interface for predefined serializers and deserializers for some known dot-net types.
    /// </summary>
    /// <typeparam name="T">The underlying known type</typeparam>
    internal abstract class KnownType<T> : IKnownType
    {
        /// <summary>
        ///     Gets the underlying known type.
        /// </summary>
        public Type Type => typeof(T);

        void IKnownType.Serialize(object obj, XElement elem, XNamespace overridingNamespace)
        {
            Serialize((T) obj, elem, overridingNamespace);
        }

        object IKnownType.Deserialize(XElement baseElement, XNamespace overridingNamespace)
        {
            return Deserialize(baseElement, overridingNamespace);
        }

        /// <summary>
        ///     Serializes the specified object int the specified XML element.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="elem">The XML element.</param>
        /// <param name="overridingNamespace">The namespace the element belongs to.</param>
        public abstract void Serialize(T obj, XElement elem, XNamespace overridingNamespace);

        /// <summary>
        ///     Deserializes the specified XML element to the known type.
        /// </summary>
        /// <param name="elem">The XML element to deserialize object from.</param>
        /// <param name="overridingNamespace">The namespace the element belongs to.</param>
        /// <returns>The deserialized object</returns>
        public abstract T Deserialize(XElement elem, XNamespace overridingNamespace);
    }

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

    internal class RectangleDynamicKnownType : DynamicKnownType
    {
        public override string TypeName => "System.Drawing.Rectangle";

        public override void Serialize(object obj, XElement elem, XNamespace overridingNamespace)
        {
            var objectType = obj.GetType();
            if (objectType.FullName != TypeName)
                throw new ArgumentException("Object type does not match the provided typename", "obj");

            var left = ReflectionUtils.InvokeGetProperty<int>(obj, "Left");
            var top = ReflectionUtils.InvokeGetProperty<int>(obj, "Top");
            var width = ReflectionUtils.InvokeGetProperty<int>(obj, "Width");
            var height = ReflectionUtils.InvokeGetProperty<int>(obj, "Height");

            elem.Add(
                new XElement(this.GetXName("Left", overridingNamespace), left),
                new XElement(this.GetXName("Top", overridingNamespace), top),
                new XElement(this.GetXName("Width", overridingNamespace), width),
                new XElement(this.GetXName("Height", overridingNamespace), height));
        }

        public override object Deserialize(XElement elem, XNamespace overridingNamespace)
        {
            var elemLeft = elem.Element(this.GetXName("Left", overridingNamespace));
            var elemTop = elem.Element(this.GetXName("Top", overridingNamespace));
            var elemWidth = elem.Element(this.GetXName("Width", overridingNamespace));
            var elemHeight = elem.Element(this.GetXName("Height", overridingNamespace));

            if (elemHeight == null || elemWidth == null || elemTop == null || elemLeft == null)
                throw new YAXElementMissingException(elem.Name + ":[Left|Top|Width|Height]", elem);

            return Activator.CreateInstance(Type,
                int.Parse(elemLeft.Value),
                int.Parse(elemTop.Value),
                int.Parse(elemWidth.Value),
                int.Parse(elemHeight.Value));
        }
    }

    internal class ColorDynamicKnownType : DynamicKnownType
    {
        public override string TypeName => "System.Drawing.Color";

        public override void Serialize(object obj, XElement elem, XNamespace overridingNamespace)
        {
            var objectType = obj.GetType();
            if (objectType.FullName != TypeName)
                throw new ArgumentException("Object type does not match the provided typename", "obj");

            var isKnownColor = ReflectionUtils.InvokeGetProperty<bool>(obj, "IsKnownColor");
            if (isKnownColor)
            {
                var colorName = ReflectionUtils.InvokeGetProperty<string>(obj, "Name");
                elem.Value = colorName;
            }
            else
            {
                var a = ReflectionUtils.InvokeGetProperty<byte>(obj, "A");
                var r = ReflectionUtils.InvokeGetProperty<byte>(obj, "R");
                var g = ReflectionUtils.InvokeGetProperty<byte>(obj, "G");
                var b = ReflectionUtils.InvokeGetProperty<byte>(obj, "B");
                elem.Add(
                    new XElement(this.GetXName("A", overridingNamespace), a),
                    new XElement(this.GetXName("R", overridingNamespace), r),
                    new XElement(this.GetXName("G", overridingNamespace), g),
                    new XElement(this.GetXName("B", overridingNamespace), b));
            }
        }

        public override object Deserialize(XElement elem, XNamespace overridingNamespace)
        {
            var elemR = elem.Element(this.GetXName("R", overridingNamespace));
            if (elemR == null)
            {
                var colorName = elem.Value;
                var colorByName = ReflectionUtils.InvokeStaticMethod(Type, "FromName", colorName);
                return colorByName;
            }

            int a = 255, r, g = 0, b = 0;

            var elemA = elem.Element(this.GetXName("A", overridingNamespace));
            if (elemA != null && !int.TryParse(elemA.Value, out a))
                a = 0;

            if (!int.TryParse(elemR.Value, out r))
                r = 0;

            var elemG = elem.Element(this.GetXName("G", overridingNamespace));
            if (elemG != null && !int.TryParse(elemG.Value, out g))
                g = 0;

            var elemB = elem.Element(this.GetXName("B", overridingNamespace));
            if (elemB != null && !int.TryParse(elemB.Value, out b))
                b = 0;

            var result = ReflectionUtils.InvokeStaticMethod(Type, "FromArgb", a, r, g, b);
            return result;
        }
    }

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

    internal class DataTableDynamicKnownType : DynamicKnownType
    {
        public override string TypeName => "System.Data.DataTable";

        public override void Serialize(object obj, XElement elem, XNamespace overridingNamespace)
        {
            using (var xw = elem.CreateWriter())
            {
                var dsType = ReflectionUtils.GetTypeByName("System.Data.DataSet");
                var ds = Activator.CreateInstance(dsType);
                var dsTables = ReflectionUtils.InvokeGetProperty<object>(ds, "Tables");
                var dtCopy = ReflectionUtils.InvokeMethod(obj, "Copy");
                ReflectionUtils.InvokeMethod(dsTables, "Add", dtCopy);
                ReflectionUtils.InvokeMethod(ds, "WriteXml", xw);
            }
        }

        public override object Deserialize(XElement elem, XNamespace overridingNamespace)
        {
            var dsElem = elem.Elements().FirstOrDefault(x => x.Name.LocalName == "NewDataSet");
            if (dsElem == null)
                return null;

            using (var xr = dsElem.CreateReader())
            {
                var dsType = ReflectionUtils.GetTypeByName("System.Data.DataSet");
                var ds = Activator.CreateInstance(dsType);
                ReflectionUtils.InvokeMethod(ds, "ReadXml", xr);
                var dsTables = ReflectionUtils.InvokeGetProperty<object>(ds, "Tables");
                var dsTablesCount = ReflectionUtils.InvokeGetProperty<int>(dsTables, "Count");
                if (dsTablesCount > 0)
                {
                    var dsTablesZero = ReflectionUtils.InvokeIntIndexer<object>(dsTables, "Index", 0);
                    var copyDt = ReflectionUtils.InvokeMethod(dsTablesZero, "Copy");
                    return copyDt;
                }

                return null;
            }
        }
    }

    internal class DataSetDynamicKnownType : DynamicKnownType
    {
        public override string TypeName => "System.Data.DataSet";

        public override void Serialize(object obj, XElement elem, XNamespace overridingNamespace)
        {
            using (var xw = elem.CreateWriter())
            {
                ReflectionUtils.InvokeMethod(obj, "WriteXml", xw);
            }
        }

        public override object Deserialize(XElement elem, XNamespace overridingNamespace)
        {
            var child = elem.Elements().FirstOrDefault();
            if (child == null)
                return null;
            using (var xr = child.CreateReader())
            {
                var dsType = ReflectionUtils.GetTypeByName("System.Data.DataSet");
                var ds = Activator.CreateInstance(dsType);
                ReflectionUtils.InvokeMethod(ds, "ReadXml", xr);
                return ds;
            }
        }
    }

    #region XElement

    // Thanks go to CodePlex user tg73: 
    // http://www.codeplex.com/site/users/view/tg73
    // for providing this implementation in the following issue:
    // http://yaxlib.codeplex.com/workitem/17676

    internal class XElementKnownType : KnownType<XElement>
    {
        public override void Serialize(XElement obj, XElement elem, XNamespace overridingNamespace)
        {
            if (obj != null) elem.Add(obj);
        }

        public override XElement Deserialize(XElement elem, XNamespace overridingNamespace)
        {
            return elem.Elements().FirstOrDefault();
        }
    }

    #endregion

    #region XAttribute

    internal class XAttributeKnownType : KnownType<XAttribute>
    {
        public override void Serialize(XAttribute obj, XElement elem, XNamespace overridingNamespace)
        {
            if (obj != null) elem.Add(obj);
        }

        public override XAttribute Deserialize(XElement elem, XNamespace overridingNamespace)
        {
            return elem.Attributes().FirstOrDefault();
        }
    }

    #endregion

    #region TimeSpan

    internal class TimeSpanKnownType : KnownType<TimeSpan>
    {
        public override void Serialize(TimeSpan timeSpan, XElement elem, XNamespace overridingNamespace)
        {
            elem.Value = timeSpan.ToString();
        }

        public override TimeSpan Deserialize(XElement elem, XNamespace overridingNamespace)
        {
            var elemTicks = elem.Element(this.GetXName("Ticks", overridingNamespace));
            if (elemTicks == null)
            {
                var strTimeSpanString = elem.Value;
                TimeSpan timeSpanResult;
                if (!TimeSpan.TryParse(strTimeSpanString, out timeSpanResult))
                    throw new YAXBadlyFormedInput(elem.Name.ToString(), elem.Value, elem);
                return timeSpanResult;
            }

            var strTicks = elemTicks.Value;
            long ticks;
            if (!long.TryParse(strTicks, out ticks)) throw new YAXBadlyFormedInput("Ticks", elemTicks.Value, elemTicks);
            return new TimeSpan(ticks);
        }
    }

    #endregion

    #region DBNull
    
    internal class DbNullKnownType : KnownType<DBNull>
    {
        public override void Serialize(DBNull obj, XElement elem, XNamespace overridingNamespace)
        {
            if (obj != null)
                elem.Value = "DBNull";
        }

        public override DBNull Deserialize(XElement elem, XNamespace overridingNamespace)
        {
            if (string.IsNullOrEmpty(elem.Value))
                return null;
            return DBNull.Value;
        }
    }

    #endregion

    internal class TypeKnownType : KnownType<Type>
    {
        public override void Serialize(Type obj, XElement elem, XNamespace overridingNamespace)
        {
            if (obj != null)
                elem.Value = obj.FullName;
        }

        public override Type Deserialize(XElement elem, XNamespace overridingNamespace)
        {
            return ReflectionUtils.GetTypeByName(elem.Value);
        }
    }
}