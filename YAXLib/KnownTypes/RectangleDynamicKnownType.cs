// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Xml.Linq;

namespace YAXLib
{
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
}