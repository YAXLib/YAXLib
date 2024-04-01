// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using NUnit.Framework;
using YAXLib;
using YAXLib.Enums;
using YAXLib.Options;

namespace YAXLibTests;

/// <summary>
/// Tests around Double or Single min and max values. Kindly contributed by CodePlex user vincentbl
/// </summary>
[TestFixture]
public class NumericMinMaxTests
{
    [Test]
    public void TestDoubleMax()
    {
        try
        {
            var ser = new YAXSerializer<double>(new SerializerOptions {
                ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.ThrowErrorsOnly,
                ExceptionBehavior = YAXExceptionTypes.Warning,
                SerializationOptions = YAXSerializationOptions.SerializeNullObjects
            });

            var d = 0.55;
            var xml = ser.Serialize(d);
            var deserializedInstance = ser.Deserialize(xml);
            Assert.That(deserializedInstance, Is.EqualTo(d));

            d = double.MaxValue;
            xml = ser.Serialize(d);
            deserializedInstance = ser.Deserialize(xml);
            // Causes a System.OverflowException {"Value was either too large or too small for a Double."}
            Assert.That(deserializedInstance, Is.EqualTo(d));
        }
        catch (Exception ex)
        {
            Assert.Fail("No exception should have been thrown, but received:" + Environment.NewLine + ex);
        }
    }

    [Test]
    public void TestDoubleMin()
    {
        try
        {
            var ser = new YAXSerializer<double>(new SerializerOptions {
                ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.ThrowErrorsOnly,
                ExceptionBehavior = YAXExceptionTypes.Warning,
                SerializationOptions = YAXSerializationOptions.SerializeNullObjects
            });
            var d = double.MinValue;
            var xml = ser.Serialize(d);
            var deserializedInstance = ser.Deserialize(xml);
            Assert.That(deserializedInstance, Is.EqualTo(d));
        }
        catch (Exception ex)
        {
            Assert.Fail("No exception should have been thrown, but received:" + Environment.NewLine + ex);
        }
    }

    [Test]
    public void TestSingleMax()
    {
        try
        {
            var ser = new YAXSerializer<float>(new SerializerOptions {
                ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.ThrowErrorsOnly,
                ExceptionBehavior = YAXExceptionTypes.Warning,
                SerializationOptions = YAXSerializationOptions.SerializeNullObjects
            });

            var f = float.MaxValue;
            var xml = ser.Serialize(f);
            var deserializedInstance = ser.Deserialize(xml);
            Assert.That(deserializedInstance, Is.EqualTo(f));
        }
        catch (Exception ex)
        {
            Assert.Fail("No exception should have been thrown, but received:" + Environment.NewLine + ex);
        }
    }

    [Test]
    public void TestSingleMin()
    {
        try
        {
            var ser = new YAXSerializer<float>(new SerializerOptions {
                ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.ThrowErrorsOnly,
                ExceptionBehavior = YAXExceptionTypes.Warning,
                SerializationOptions = YAXSerializationOptions.SerializeNullObjects
            });
            var f = float.MinValue;
            var xml = ser.Serialize(f);
            var deserializedInstance = ser.Deserialize(xml);
            Assert.That(deserializedInstance, Is.EqualTo(f));
        }
        catch (Exception ex)
        {
            Assert.Fail("No exception should have been thrown, but received:" + Environment.NewLine + ex);
        }
    }
}
