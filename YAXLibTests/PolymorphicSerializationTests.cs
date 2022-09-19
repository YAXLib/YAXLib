// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using NUnit.Framework;
using YAXLib;
using YAXLib.Exceptions;
using YAXLibTests.SampleClasses.PolymorphicSerialization;

namespace YAXLibTests;

[TestFixture]
public class PolymorphicSerializationTests
{
    [Test]
    public void MultipleYaxTypeAttributesWithSameTypeMustThrowAnException()
    {
        var ser = new YAXSerializer(typeof(MultipleYaxTypeAttributesWithSameType));
        var obj = new MultipleYaxTypeAttributesWithSameType();
        Assert.Throws<YAXPolymorphicException>(() => ser.Serialize(obj));
    }

    [Test]
    public void MultipleYaxTypeAttributesWIthSameAliasMustThrowAnException()
    {
        var ser = new YAXSerializer(typeof(MultipleYaxTypeAttributesWithSameAlias));
        var obj = new MultipleYaxTypeAttributesWithSameAlias();
        Assert.Throws<YAXPolymorphicException>(() => ser.Serialize(obj));
    }
}