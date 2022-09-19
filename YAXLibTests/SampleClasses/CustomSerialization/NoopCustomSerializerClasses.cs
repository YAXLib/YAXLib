// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Xml.Linq;
using YAXLib;
using YAXLib.Customization;

namespace YAXLibTests.SampleClasses.CustomSerialization;

internal class NoopIntCustomSerializer : ICustomSerializer<int>
{
    private NoopIntCustomSerializer() { }

    public void SerializeToAttribute(int objectToSerialize, XAttribute attrToFill,
        ISerializationContext serializationContext)
    {
    }

    public void SerializeToElement(int objectToSerialize, XElement elemToFill,
        ISerializationContext serializationContext)
    {
    }

    public string SerializeToValue(int objectToSerialize, ISerializationContext serializationContext)
    {
        return nameof(NoopIntCustomSerializer);
    }

    public int DeserializeFromAttribute(XAttribute attribute, ISerializationContext serializationContext)
    {
        return 1;
    }

    public int DeserializeFromElement(XElement element, ISerializationContext serializationContext)
    {
        return 2;
    }

    public int DeserializeFromValue(string value, ISerializationContext serializationContext)
    {
        return 3;
    }
}

public class NoopLongCustomSerializer : ICustomSerializer<long>
{
    public void SerializeToAttribute(long objectToSerialize, XAttribute attrToFill,
        ISerializationContext serializationContext)
    {
    }

    public void SerializeToElement(long objectToSerialize, XElement elemToFill,
        ISerializationContext serializationContext)
    {
    }

    public string SerializeToValue(long objectToSerialize, ISerializationContext serializationContext)
    {
        return nameof(NoopLongCustomSerializer);
    }

    public long DeserializeFromAttribute(XAttribute attribute, ISerializationContext serializationContext)
    {
        return 1;
    }

    public long DeserializeFromElement(XElement element, ISerializationContext serializationContext)
    {
        return 2;
    }

    public long DeserializeFromValue(string value, ISerializationContext serializationContext)
    {
        return 3;
    }
}

internal class NoopStringCustomSerializer : ICustomSerializer<string>
{
    public void SerializeToAttribute(string objectToSerialize, XAttribute attrToFill,
        ISerializationContext serializationContext)
    {
    }

    public void SerializeToElement(string objectToSerialize, XElement elemToFill,
        ISerializationContext serializationContext)
    {
    }

    public string SerializeToValue(string objectToSerialize, ISerializationContext serializationContext)
    {
        return nameof(NoopStringCustomSerializer);
    }

    public string DeserializeFromAttribute(XAttribute attribute, ISerializationContext serializationContext)
    {
        throw new NotImplementedException();
    }

    public string DeserializeFromElement(XElement element, ISerializationContext serializationContext)
    {
        return nameof(NoopStringCustomSerializer);
    }

    public string DeserializeFromValue(string value, ISerializationContext serializationContext)
    {
        return nameof(NoopStringCustomSerializer);
    }
}

internal class NoopDoubleCustomSerializer : ICustomSerializer<double>
{
    public void SerializeToAttribute(double objectToSerialize, XAttribute attrToFill,
        ISerializationContext serializationContext)
    {
    }

    public void SerializeToElement(double objectToSerialize, XElement elemToFill,
        ISerializationContext serializationContext)
    {
    }

    public string SerializeToValue(double objectToSerialize, ISerializationContext serializationContext)
    {
        return nameof(NoopDoubleCustomSerializer);
    }

    public double DeserializeFromAttribute(XAttribute attribute, ISerializationContext serializationContext)
    {
        return 1;
    }

    public double DeserializeFromElement(XElement element, ISerializationContext serializationContext)
    {
        return 2;
    }

    public double DeserializeFromValue(string value, ISerializationContext serializationContext)
    {
        return 3;
    }
}

public class NoopFloatCustomSerializer : ICustomSerializer<float>
{
    public void SerializeToAttribute(float objectToSerialize, XAttribute attrToFill,
        ISerializationContext serializationContext)
    {
    }

    public void SerializeToElement(float objectToSerialize, XElement elemToFill,
        ISerializationContext serializationContext)
    {
    }

    public string SerializeToValue(float objectToSerialize, ISerializationContext serializationContext)
    {
        return nameof(NoopFloatCustomSerializer);
    }

    public float DeserializeFromAttribute(XAttribute attribute, ISerializationContext serializationContext)
    {
        return 1;
    }

    public float DeserializeFromElement(XElement element, ISerializationContext serializationContext)
    {
        return 2;
    }

    public float DeserializeFromValue(string value, ISerializationContext serializationContext)
    {
        return 3;
    }
}
