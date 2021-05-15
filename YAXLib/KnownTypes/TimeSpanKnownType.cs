// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Xml.Linq;

namespace YAXLib
{
    internal class TimeSpanKnownType : KnownType<TimeSpan>
    {
        public override bool CanSerialize => true;
        public override bool CanDeserialize => true;

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
}