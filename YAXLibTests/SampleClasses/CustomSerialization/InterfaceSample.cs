// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using YAXLib.Attributes;
using YAXLib.Enums;

namespace YAXLibTests.SampleClasses.CustomSerialization;

public interface ISampleInterface
{
    public int Id { get; set; }
    public string? Name { get; set; }
}

[YAXCustomSerializer(typeof(InterfaceSerializer))]
public class NonGenericClassWithInterface : ISampleInterface
{
    public int Id { get; set; }
    public string? Name { get; set; }
}

[YAXCustomSerializer(typeof(InterfaceSerializer))]
public class GenericClassWithInterface<T> : ISampleInterface
{
    [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
    public T? Something { get; set; }

    public int Id { get; set; }
    public string? Name { get; set; }

    public override string ToString()
    {
        return Id + Name;
    }
}

[YAXCustomSerializer(typeof(InterfaceSerializer))]
public class GenericClassWithoutInterface<T>
{
    [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
    public T? Something { get; set; }

    public int Id { get; set; }
    public string? Name { get; set; }

    public override string ToString()
    {
        return Id + Name;
    }
}

[YAXCustomSerializer(typeof(string))]
public class IllegalTypeOfClassSerializer : ISampleInterface
{
    public int Id { get; set; }
    public string? Name { get; set; }

    public override string ToString()
    {
        return Id + Name;
    }
}