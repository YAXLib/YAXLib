// Copyright 2009 Sina Iravanian - <sina@sinairv.com>
//
// This source file(s) may be redistributed, altered and customized
// by any means PROVIDING the authors name and all copyright
// notices remain intact.
// THIS SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED. USE IT AT YOUR OWN RISK. THE AUTHOR ACCEPTS NO
// LIABILITY FOR ANY DATA DAMAGE/LOSS THAT THIS PRODUCT MAY CAUSE.
//-----------------------------------------------------------------------

// The KnownDotNetTypes class is replaced by KnownTypes class.
// The new design for KnownTypes is adopted from Tomanu's YAXLib fork
// named YYAAXXLib. The fork can be found at:
// http://tomanuyyaaxxlib.codeplex.com/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;
using System.Diagnostics;

namespace YAXLib
{
    /// <summary>
    /// Provides serialization and deserialization methods for some known .NET built-in types.
    /// </summary>
    internal class KnownTypes
    {
        private static readonly Dictionary<Type, IKnownType> s_dictKnownTypes = new Dictionary<Type, IKnownType>();

        static KnownTypes()
        {
            // NOTE: known-types MUST be registered here
            Add(new RectangleKnownType());
            Add(new TimeSpanKnownType());
            Add(new ColorKnownType());
            Add(new XElementKnownType());
            Add(new XAttributeKnownType());
            Add(new DbNullKnownType());
        }

        public static void Add(IKnownType kt)
        {
            s_dictKnownTypes[kt.Type] = kt;
        }

        public static bool IsKnowType(Type type)
        {
            return s_dictKnownTypes.ContainsKey(type);
        }

        public static void Serialize(object obj, XElement elem, XNamespace overridingNamespace)
        {
            if(obj != null)
            {
                s_dictKnownTypes[obj.GetType()].Serialize(obj, elem, overridingNamespace);
            }
        }

        public static object Deserialize(XElement elem, Type type, XNamespace overridingNamespace)
        {
            return s_dictKnownTypes[type].Deserialize(elem, overridingNamespace);
        }
    }

    internal interface IKnownType
    {
        /// <summary>
        /// Serializes the specified object int the specified XML element.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="elem">The XML element.</param>
        /// <param name="overridingNamespace">The namespace the element belongs to.</param>
        void Serialize(object obj, XElement elem, XNamespace overridingNamespace);

        /// <summary>
        /// Deserializes the specified XML element to the known type.
        /// </summary>
        /// <param name="elem">The XML element to deserialize object from.</param>
        /// <param name="overridingNamespace">The namespace the element belongs to.</param>
        /// <returns>The deserialized object</returns>
        object Deserialize(XElement elem, XNamespace overridingNamespace);

        /// <summary>
        /// Gets the underlying known type.
        /// </summary>
        Type Type { get; }
    }

    /// <summary>
    /// Interface for predefined serializers and deserializers for some known dot-net types.
    /// </summary>
    /// <typeparam name="T">The underlying known type</typeparam>
    internal abstract class KnownType<T> : IKnownType
    {
        /// <summary>
        /// Serializes the specified object int the specified XML element.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="elem">The XML element.</param>
        /// <param name="overridingNamespace">The namespace the element belongs to.</param>
        public abstract void Serialize(T obj, XElement elem, XNamespace overridingNamespace);

        /// <summary>
        /// Deserializes the specified XML element to the known type.
        /// </summary>
        /// <param name="elem">The XML element to deserialize object from.</param>
        /// <param name="overridingNamespace">The namespace the element belongs to.</param>
        /// <returns>The deserialized object</returns>
        public abstract T Deserialize(XElement elem, XNamespace overridingNamespace);

        /// <summary>
        /// Gets the underlying known type.
        /// </summary>
        public Type Type 
        { 
            get { return typeof (T); }
        }

        void IKnownType.Serialize(object obj, XElement elem, XNamespace overridingNamespace)
        {
            Serialize((T)obj, elem, overridingNamespace);
        }

        object IKnownType.Deserialize(XElement baseElement, XNamespace overridingNamespace)
        {
            return Deserialize(baseElement, overridingNamespace);
        }

        protected XName GetXName(string name, XNamespace overridingNamespace)
        {
            if (overridingNamespace.HasNamespace())
                return XName.Get(name, overridingNamespace.NamespaceName);
            else
                return XName.Get(name);
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
            Debug.Assert(obj != null);
            if (obj != null)
            {
                elem.Add(obj);
            }
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
            Debug.Assert(obj != null);
            if(obj != null)
            {
                elem.Add(obj);
            }
        }

        public override XAttribute Deserialize(XElement elem, XNamespace overridingNamespace)
        {
            return elem.Attributes().FirstOrDefault();
        }
    }

    #endregion

    #region Rectangle
    internal class RectangleKnownType : KnownType<Rectangle>
    {
        public override void Serialize(Rectangle rect, XElement elem, XNamespace overridingNamespace)
        {
            elem.Add(
                new XElement(GetXName("Left", overridingNamespace), rect.Left),
                new XElement(GetXName("Top", overridingNamespace), rect.Top),
                new XElement(GetXName("Width", overridingNamespace), rect.Width),
                new XElement(GetXName("Height", overridingNamespace), rect.Height));
        }

        public override Rectangle Deserialize(XElement elem, XNamespace overridingNamespace)
        {
            var elemLeft = elem.Element(GetXName("Left", overridingNamespace));
            var elemTop = elem.Element(GetXName("Top", overridingNamespace));
            var elemWidth = elem.Element(GetXName("Width", overridingNamespace));
            var elemHeight = elem.Element(GetXName("Height", overridingNamespace));

            if (elemHeight == null || elemWidth == null || elemTop == null || elemLeft == null)
                throw new YAXElementMissingException(elem.Name + ":[Left|Top|Width|Height]");

            return new Rectangle(
                Int32.Parse(elemLeft.Value),
                Int32.Parse(elemTop.Value),
                Int32.Parse(elemWidth.Value),
                Int32.Parse(elemHeight.Value));
        }
    }
    #endregion 

    #region Color
    internal class ColorKnownType : KnownType<Color>
    {
        public override void Serialize(Color color, XElement elem, XNamespace overridingNamespace)
        {
            if (color.IsKnownColor)
            {
                elem.Value = color.Name;
            }
            else
            {
                elem.Add(
                    new XElement(GetXName("A", overridingNamespace), color.A),
                    new XElement(GetXName("R", overridingNamespace), color.R),
                    new XElement(GetXName("G", overridingNamespace), color.G),
                    new XElement(GetXName("B", overridingNamespace), color.B));
            }
        }

        public override Color Deserialize(XElement elem, XNamespace overridingNamespace)
        {
            var elemR = elem.Element(GetXName("R", overridingNamespace));
            if (elemR == null)
            {
                string colorName = elem.Value;
                return Color.FromName(colorName);
            }

            int a = 255, r, g = 0, b = 0;

            var elemA = elem.Element(GetXName("A", overridingNamespace));
            if (elemA != null && !Int32.TryParse(elemA.Value, out a))
                a = 0;

            if (!Int32.TryParse(elemR.Value, out r))
                r = 0;

            var elemG = elem.Element(GetXName("G", overridingNamespace));
            if (elemG != null && !Int32.TryParse(elemG.Value, out g))
                g = 0;

            var elemB = elem.Element(GetXName("B", overridingNamespace));
            if (elemB != null && !Int32.TryParse(elemB.Value, out b))
                b = 0;

            return Color.FromArgb(a, r, g, b);
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
            var elemTicks = elem.Element(GetXName("Ticks", overridingNamespace));
            if (elemTicks == null)
            {
                string strTimeSpanString = elem.Value;
                TimeSpan timeSpanResult;
                if (!TimeSpan.TryParse(strTimeSpanString, out timeSpanResult))
                {
                    throw new YAXBadlyFormedInput(elem.Name.ToString(), elem.Value);
                }
                return timeSpanResult;
            }
            else
            {
                string strTicks = elemTicks.Value;
                long ticks;
                if (!Int64.TryParse(strTicks, out ticks))
                {
                    throw new YAXBadlyFormedInput("Ticks", elemTicks.Value);
                }
                return new TimeSpan(ticks);
            }
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
            if (String.IsNullOrEmpty(elem.Value))
                return null;
            else
                return DBNull.Value;
        }
    }
    #endregion
}
