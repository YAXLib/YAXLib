// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Globalization;
using System.Xml.Linq;
using YAXLib.Customization;
using YAXLib.Exceptions;

namespace YAXLib.KnownTypes;

internal class DateTimeOffsetKnownType : KnownTypeBase<DateTimeOffset>
{
    /// <inheritdoc />
    public override void Serialize(DateTimeOffset dateTimeOffset, XElement elem, XNamespace overridingNamespace,
        ISerializationContext serializationContext)
    {
        // Use "O" (round-trip) format to preserve offset and precision
        elem.Value = dateTimeOffset.ToString("O", CultureInfo.InvariantCulture);
    }

    /// <inheritdoc />
    public override DateTimeOffset Deserialize(XElement elem, XNamespace overridingNamespace,
        ISerializationContext serializationContext)
    {
        var elemTicks = elem.Element(overridingNamespace.GetXName(nameof(DateTimeOffset.Ticks)));

        // If Ticks child element exists, deserialize from ticks and offset
        if (elemTicks != null)
        {
            if (!long.TryParse(elemTicks.Value, out var ticks))
                throw new YAXBadlyFormedInput(elemTicks.Name.ToString(), elemTicks.Value, elemTicks);

            var elemOffset = elem.Element(overridingNamespace.GetXName(nameof(DateTimeOffset.Offset)));
            if (elemOffset == null)
                throw new YAXElementMissingException(nameof(DateTimeOffset.Offset));

            if (!TimeSpan.TryParse(elemOffset.Value, CultureInfo.InvariantCulture, out var offset))
                throw new YAXBadlyFormedInput(elemOffset.Name.ToString(), elemOffset.Value, elemOffset);

            return new DateTimeOffset(ticks, offset);
        }

        // Fallback: parse from element value using standard DateTimeOffset parsing
        var strDateTimeOffsetString = elem.Value;
        if (!DateTimeOffset.TryParse(strDateTimeOffsetString, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
            throw new YAXBadlyFormedInput(elem.Name.ToString(), elem.Value, elem);

        return result;
    }
}
