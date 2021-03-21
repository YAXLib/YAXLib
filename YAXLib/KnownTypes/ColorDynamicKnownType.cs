﻿// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Xml.Linq;

namespace YAXLib
{
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
}