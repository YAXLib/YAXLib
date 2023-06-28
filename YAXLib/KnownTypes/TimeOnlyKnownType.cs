// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Xml.Linq;
using YAXLib.Customization;
using YAXLib.Exceptions;

namespace YAXLib.KnownTypes;

internal class TimeOnlyKnownType : KnownTypeBase<TimeOnly>
{
    /// <inheritdoc />
    public override void Serialize(TimeOnly timeOnly, XElement elem, XNamespace overridingNamespace,
        ISerializationContext serializationContext)
    {
        var elemDayNumber = new XElement(nameof(timeOnly.Ticks), timeOnly.Ticks.ToString());
        elem.Add(elemDayNumber);
    }

    /// <inheritdoc />
    public override TimeOnly Deserialize(XElement elem, XNamespace overridingNamespace,
        ISerializationContext serializationContext)
    {
        var elemTicks = elem.Element(overridingNamespace.GetXName(nameof(TimeOnly.Ticks)));
        long ticks;

        // Child element not found, so we use the element value as a fallback
        if (elemTicks == null)
        {
            var strTimeOnlyString = elem.Value;
            if (!long.TryParse(strTimeOnlyString, out ticks))
                throw new YAXBadlyFormedInput(elem.Name.ToString(), elem.Value, elem);
            return TimeOnly.FromTimeSpan(TimeSpan.FromTicks(ticks));
        }

        // Process the child element
        if (!long.TryParse(elemTicks.Value, out ticks))
            throw new YAXBadlyFormedInput(elemTicks.Name.ToString(), elemTicks.Value, elemTicks);
        return TimeOnly.FromTimeSpan(new TimeSpan(ticks));
    }
}
