// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Xml.Linq;
using YAXLib;
using YAXLib.KnownTypes;

namespace YAXLibTests.SampleClasses.KnownTypes;

internal class UsingSerializationContextKnownType : KnownTypeBase<UsingSerializationContextSample>
{
    public override void Serialize(UsingSerializationContextSample obj, XElement elem, XNamespace overridingNamespace,
        ISerializationContext serializationContext)
    {
        var serialized = serializationContext.TypeContext.Serialize(obj);
        // There is only one property
        serialized.Element(nameof(UsingSerializationContextSample.Text))!.Value = obj.Text + " KnownType";
        elem.Add(serialized.Element(nameof(UsingSerializationContextSample.Text)));
    }

    public override UsingSerializationContextSample Deserialize(XElement elem, XNamespace overridingNamespace,
        ISerializationContext serializationContext)
    {
        var deserialized = (UsingSerializationContextSample) serializationContext.TypeContext.Deserialize(elem);
        deserialized.Text = deserialized.Text.Replace(" KnownType", string.Empty);
        return deserialized;
    }
}
