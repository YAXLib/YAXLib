// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Xml.Linq;
using YAXLib.Customization;
using YAXLib.KnownTypes;
using YAXLib.Options;

namespace YAXLibTests.SampleClasses.KnownTypes;

internal class UsingSerializationContextKnownType : KnownTypeBase<UsingSerializationContextSample>
{
    private readonly SerializerOptions _serializerOptions = new();

    public override void Serialize(UsingSerializationContextSample? obj, XElement elem, XNamespace overridingNamespace,
        ISerializationContext serializationContext)
    {
        if (obj == null) return;

        var serialized = serializationContext.TypeContext.Serialize(obj, _serializerOptions);
        // There is only one property
        serialized.Element(nameof(UsingSerializationContextSample.Text))!.Value = obj.Text + " KnownType";
        elem.Add(serialized.Element(nameof(UsingSerializationContextSample.Text)));
    }

    public override UsingSerializationContextSample? Deserialize(XElement elem, XNamespace overridingNamespace,
        ISerializationContext serializationContext)
    {
        var deserialized =
            (UsingSerializationContextSample?) serializationContext.TypeContext.Deserialize(elem, _serializerOptions);
        deserialized!.Text = deserialized.Text?.Replace(" KnownType", string.Empty);
        return deserialized;
    }
}
