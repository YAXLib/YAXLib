// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Xml.Linq;
using YAXLib.Customization;
using YAXLib.Exceptions;

namespace YAXLib.KnownTypes;

internal class DateOnlyKnownType : KnownTypeBase<DateOnly>
{
    /// <inheritdoc />
    public override void Serialize(DateOnly dateOnly, XElement elem, XNamespace overridingNamespace,
        ISerializationContext serializationContext)
    {
        var elemDayNumber = new XElement(nameof(DateOnly.DayNumber), dateOnly.DayNumber.ToString());
        elem.Add(elemDayNumber);
    }

    /// <inheritdoc />
    public override DateOnly Deserialize(XElement elem, XNamespace overridingNamespace,
        ISerializationContext serializationContext)
    {
        var elemDayNumber = elem.Element(overridingNamespace.GetXName(nameof(DateOnly.DayNumber)));

        // Child element not found, so we use the element value as a fallback
        if (elemDayNumber == null)
        {
            var strDateOnlyString = elem.Value;
            if (!int.TryParse(strDateOnlyString, out var dateOnlyResult))
                throw new YAXBadlyFormedInput(elem.Name.ToString(), elem.Value, elem);
            return DateOnly.FromDayNumber(dateOnlyResult);
        }

        // Process the child element
        if (!int.TryParse(elemDayNumber.Value, out var dayNumber))
            throw new YAXBadlyFormedInput(elemDayNumber.Name.ToString(), elemDayNumber.Value, elemDayNumber);
        return DateOnly.FromDayNumber(dayNumber);
    }
}
