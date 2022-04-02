// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Xml.Linq;
using YAXLib.Exceptions;

namespace YAXLib
{
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
                if (!TimeSpan.TryParse(strTimeSpanString, out var timeSpanResult))
                    throw new YAXBadlyFormedInput(elem.Name.ToString(), elem.Value, elem);
                return timeSpanResult;
            }

            if (!long.TryParse(elemTicks.Value, out var ticks)) throw new YAXBadlyFormedInput("Ticks", elemTicks.Value, elemTicks);
            return new TimeSpan(ticks);
        }
    }
}