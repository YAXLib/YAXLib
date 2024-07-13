// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using NUnit.Framework;
using YAXLib;
using YAXLib.Enums;
using YAXLib.Options;
using YAXLibTests.SampleClasses;

namespace YAXLibTests;

[TestFixture]
public class OverridingYaxLibMetadataTests
{
    [Test]
    public void CanOverrideAllMetadata()
    {
        var ser = new YAXSerializer(typeof(YaxLibMetadataOverridingWithNamespace),
            new SerializerOptions {
                Namespace = { Prefix = "yax", Uri = "http://namespace.org/yax" },
                AttributeName = { Dimensions = "dm", RealType = "type" }
            });

        var sampleInstance = YaxLibMetadataOverridingWithNamespace.GetSampleInstance();
        var result = ser.Serialize(sampleInstance);

        var expected =
            """
                <YaxLibMetadataOverridingWithNamespace xmlns:yax="http://namespace.org/yax" xmlns="http://namespace.org/sample">
                  <IntArray yax:dm="2,3">
                    <Int32>1</Int32>
                    <Int32>2</Int32>
                    <Int32>3</Int32>
                    <Int32>2</Int32>
                    <Int32>3</Int32>
                    <Int32>4</Int32>
                  </IntArray>
                  <Obj yax:type="System.String">Hello, World!</Obj>
                </YaxLibMetadataOverridingWithNamespace>
                """;
        Assert.That(result, Is.EqualTo(expected));

        var desObj = (YaxLibMetadataOverridingWithNamespace?) ser.Deserialize(expected);

        Assert.Multiple(() =>
        {
            Assert.That(desObj?.Obj?.ToString(), Is.EqualTo(sampleInstance.Obj?.ToString()));
            Assert.That(desObj?.IntArray?.Length, Is.EqualTo(sampleInstance.IntArray?.Length));
        });
    }

    [Test]
    public void CanUseTheDefaultNamespace()
    {
        var ser = new YAXSerializer(typeof(YaxLibMetadataOverridingWithNamespace),
            new SerializerOptions {
                Namespace = { Prefix = string.Empty, Uri = "http://namespace.org/sample" },
                AttributeName = { Dimensions = "dm", RealType = "type" }
            });

        var sampleInstance = YaxLibMetadataOverridingWithNamespace.GetSampleInstance();
        var result = ser.Serialize(sampleInstance);

        var expected =
            """
                <YaxLibMetadataOverridingWithNamespace xmlns="http://namespace.org/sample">
                  <IntArray dm="2,3">
                    <Int32>1</Int32>
                    <Int32>2</Int32>
                    <Int32>3</Int32>
                    <Int32>2</Int32>
                    <Int32>3</Int32>
                    <Int32>4</Int32>
                  </IntArray>
                  <Obj type="System.String">Hello, World!</Obj>
                </YaxLibMetadataOverridingWithNamespace>
                """;
        Assert.That(result, Is.EqualTo(expected));

        var desObj = (YaxLibMetadataOverridingWithNamespace?) ser.Deserialize(expected);

        Assert.Multiple(() =>
        {
            Assert.That(desObj?.Obj?.ToString(), Is.EqualTo(sampleInstance.Obj?.ToString()));
            Assert.That(desObj?.IntArray?.Length, Is.EqualTo(sampleInstance.IntArray?.Length));
        });
    }

    [Test]
    public void CanSuppressMetadata()
    {
        var ser = new YAXSerializer<YaxLibMetadataOverriding>(
            new SerializerOptions { SerializationOptions = YAXSerializationOptions.SuppressMetadataAttributes }
        );

        var sampleInstance = YaxLibMetadataOverriding.GetSampleInstance();
        var result = ser.Serialize(sampleInstance);

        var expected =
            """
                <YaxLibMetadataOverriding>
                  <IntArray>
                    <Int32>1</Int32>
                    <Int32>2</Int32>
                    <Int32>3</Int32>
                    <Int32>2</Int32>
                    <Int32>3</Int32>
                    <Int32>4</Int32>
                  </IntArray>
                  <Obj>Hello, World!</Obj>
                </YaxLibMetadataOverriding>
                """;
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void CanSuppressMetadataButUseCustomNamespace()
    {
        var ser = new YAXSerializer<YaxLibMetadataOverridingWithNamespace>(
            new SerializerOptions { SerializationOptions = YAXSerializationOptions.SuppressMetadataAttributes }
        );

        var sampleInstance = YaxLibMetadataOverridingWithNamespace.GetSampleInstance();
        var result = ser.Serialize(sampleInstance);

        var expected =
            """
                <YaxLibMetadataOverridingWithNamespace xmlns="http://namespace.org/sample">
                  <IntArray>
                    <Int32>1</Int32>
                    <Int32>2</Int32>
                    <Int32>3</Int32>
                    <Int32>2</Int32>
                    <Int32>3</Int32>
                    <Int32>4</Int32>
                  </IntArray>
                  <Obj>Hello, World!</Obj>
                </YaxLibMetadataOverridingWithNamespace>
                """;
        Assert.That(result, Is.EqualTo(expected));
    }
}