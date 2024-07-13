// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Globalization;
using System.Xml.Linq;
using YAXLib.Customization;
using YAXLib.Exceptions;

namespace YAXLib.KnownTypes;

internal class TimeSpanKnownType : KnownTypeBase<TimeSpan>
{
    /// <inheritdoc />
    public override void Serialize(TimeSpan timeSpan, XElement elem, XNamespace overridingNamespace,
        ISerializationContext serializationContext)
    {
        elem.Value = timeSpan.ToString(string.Empty, CultureInfo.InvariantCulture);
    }

    /// <inheritdoc />
    public override TimeSpan Deserialize(XElement elem, XNamespace overridingNamespace,
        ISerializationContext serializationContext)
    {
        var elemTicks = elem.Element(overridingNamespace.GetXName("Ticks"));
        if (elemTicks == null)
        {
            var strTimeSpanString = elem.Value;
            if (!TimeSpan.TryParse(strTimeSpanString, CultureInfo.InvariantCulture, out var timeSpanResult))
                throw new YAXBadlyFormedInput(elem.Name.ToString(), elem.Value, elem);
            return timeSpanResult;
        }

        if (!long.TryParse(elemTicks.Value, out var ticks))
            throw new YAXBadlyFormedInput("Ticks", elemTicks.Value, elemTicks);
        return new TimeSpan(ticks);
    }
}
