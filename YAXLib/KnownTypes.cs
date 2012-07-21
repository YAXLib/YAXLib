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

        public static void Serialize(object obj, XElement elem)
        {
            if(obj != null)
            {
                s_dictKnownTypes[obj.GetType()].Serialize(obj, elem);
            }
        }

        public static object Deserialize(XElement elem, Type type)
        {
            return s_dictKnownTypes[type].Deserialize(elem);
        }
    }

    internal interface IKnownType
    {
        /// <summary>
        /// Serializes the specified object int the specified XML element.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="elem">The XML element.</param>
        void Serialize(object obj, XElement elem);

        /// <summary>
        /// Deserializes the specified XML element to the known type.
        /// </summary>
        /// <param name="elem">The XML element to deserialize object from.</param>
        /// <returns>The deserialized object</returns>
        object Deserialize(XElement elem);

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
        public abstract void Serialize(T obj, XElement elem);

        /// <summary>
        /// Deserializes the specified XML element to the known type.
        /// </summary>
        /// <param name="elem">The XML element to deserialize object from.</param>
        /// <returns>The deserialized object</returns>
        public abstract T Deserialize(XElement elem);

        /// <summary>
        /// Gets the underlying known type.
        /// </summary>
        public Type Type 
        { 
            get { return typeof (T); }
        }

        void IKnownType.Serialize(object obj, XElement elem)
        {
            Serialize((T)obj, elem);
        }

        object IKnownType.Deserialize(XElement baseElement)
        {
            return Deserialize(baseElement);
        }

    }

    #region XElement

    // Thanks go to CodePlex user tg73: 
    // http://www.codeplex.com/site/users/view/tg73
    // for providing this implementation in the following issue:
    // http://yaxlib.codeplex.com/workitem/17676

    internal class XElementKnownType : KnownType<XElement>
    {
        public override void Serialize(XElement obj, XElement elem)
        {
            Debug.Assert(obj != null);
            if (obj != null)
            {
                elem.Add(obj);
            }
        }

        public override XElement Deserialize(XElement elem)
        {
            return elem.Elements().FirstOrDefault();
        }
    }

    #endregion

    #region XAttribute

    internal class XAttributeKnownType : KnownType<XAttribute>
    {
        public override void Serialize(XAttribute obj, XElement elem)
        {
            Debug.Assert(obj != null);
            if(obj != null)
            {
                elem.Add(obj);
            }
        }

        public override XAttribute Deserialize(XElement elem)
        {
            return elem.Attributes().FirstOrDefault();
        }
    }

    #endregion

    #region Rectangle
    internal class RectangleKnownType : KnownType<Rectangle>
    {
        public override void Serialize(Rectangle rect, XElement elem)
        {
            elem.Add(
                new XElement("Left", rect.Left),
                new XElement("Top", rect.Top),
                new XElement("Width", rect.Width),
                new XElement("Height", rect.Height));
        }

        public override Rectangle Deserialize(XElement elem)
        {
            var elemLeft = elem.Element("Left");
            var elemTop = elem.Element("Top");
            var elemWidth = elem.Element("Width");
            var elemHeight = elem.Element("Height");

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
        public override void Serialize(Color color, XElement elem)
        {
            if (color.IsKnownColor)
            {
                elem.Value = color.Name;
            }
            else
            {
                elem.Add(
                    new XElement("A", color.A),
                    new XElement("R", color.R),
                    new XElement("G", color.G),
                    new XElement("B", color.B));
            }
        }

        public override Color Deserialize(XElement elem)
        {
            var elemR = elem.Element("R");
            if (elemR == null)
            {
                string colorName = elem.Value;
                return Color.FromName(colorName);
            }

            int a = 255, r, g = 0, b = 0;

            var elemA = elem.Element("A");
            if (elemA != null && !Int32.TryParse(elemA.Value, out a))
                a = 0;

            if (!Int32.TryParse(elemR.Value, out r))
                r = 0;

            var elemG = elem.Element("G");
            if (elemG != null && !Int32.TryParse(elemG.Value, out g))
                g = 0;

            var elemB = elem.Element("B");
            if (elemB != null && !Int32.TryParse(elemB.Value, out b))
                b = 0;

            return Color.FromArgb(a, r, g, b);
        }
    }

    #endregion

    #region TimeSpan
    internal class TimeSpanKnownType : KnownType<TimeSpan>
    {
        public override void Serialize(TimeSpan timeSpan, XElement elem)
        {
            elem.Value = timeSpan.ToString();
        }

        public override TimeSpan Deserialize(XElement elem)
        {
            var elemTicks = elem.Element("Ticks");
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
        public override void Serialize(DBNull obj, XElement elem)
        {
            if (obj != null)
                elem.Value = "DBNull";
        }

        public override DBNull Deserialize(XElement elem)
        {
            if (String.IsNullOrEmpty(elem.Value))
                return null;
            else
                return DBNull.Value;
        }
    }
    #endregion
}
