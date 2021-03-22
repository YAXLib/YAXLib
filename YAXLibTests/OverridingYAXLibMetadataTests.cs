﻿// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using NUnit.Framework;
using YAXLib;
using YAXLibTests.SampleClasses;

namespace YAXLibTests
{
    [TestFixture]
    public class OverridingYAXLibMetadataTests
    {
        [Test]
        public void CanOverrideAllMetadata()
        {
            var ser = new YAXSerializer(typeof(YAXLibMetadataOverridingWithNamespace));

            ser.YaxLibNamespacePrefix = "yax";
            ser.YaxLibNamespaceUri = "http://namespace.org/yax";
            ser.DimensionsAttributeName = "dm";
            ser.RealTypeAttributeName = "type";

            var sampleInstance = YAXLibMetadataOverridingWithNamespace.GetSampleInstance();
            var result = ser.Serialize(sampleInstance);

            var expected =
                @"<YAXLibMetadataOverridingWithNamespace xmlns:yax=""http://namespace.org/yax"" xmlns=""http://namespace.org/sample"">
  <IntArray yax:dm=""2,3"">
    <Int32>1</Int32>
    <Int32>2</Int32>
    <Int32>3</Int32>
    <Int32>2</Int32>
    <Int32>3</Int32>
    <Int32>4</Int32>
  </IntArray>
  <Obj yax:type=""System.String"">Hello, World!</Obj>
</YAXLibMetadataOverridingWithNamespace>";
            Assert.That(result, Is.EqualTo(expected));

            var desObj = (YAXLibMetadataOverridingWithNamespace) ser.Deserialize(expected);

            Assert.That(desObj.Obj.ToString(), Is.EqualTo(sampleInstance.Obj.ToString()));
            Assert.That(desObj.IntArray.Length, Is.EqualTo(sampleInstance.IntArray.Length));
        }

        [Test]
        public void CanUseTheDefaultNamespace()
        {
            var ser = new YAXSerializer(typeof(YAXLibMetadataOverridingWithNamespace));

            ser.YaxLibNamespacePrefix = string.Empty;
            ser.YaxLibNamespaceUri = "http://namespace.org/sample";
            ser.DimensionsAttributeName = "dm";
            ser.RealTypeAttributeName = "type";

            var sampleInstance = YAXLibMetadataOverridingWithNamespace.GetSampleInstance();
            var result = ser.Serialize(sampleInstance);

            var expected =
                @"<YAXLibMetadataOverridingWithNamespace xmlns=""http://namespace.org/sample"">
  <IntArray dm=""2,3"">
    <Int32>1</Int32>
    <Int32>2</Int32>
    <Int32>3</Int32>
    <Int32>2</Int32>
    <Int32>3</Int32>
    <Int32>4</Int32>
  </IntArray>
  <Obj type=""System.String"">Hello, World!</Obj>
</YAXLibMetadataOverridingWithNamespace>";
            Assert.That(result, Is.EqualTo(expected));

            var desObj = (YAXLibMetadataOverridingWithNamespace) ser.Deserialize(expected);

            Assert.That(desObj.Obj.ToString(), Is.EqualTo(sampleInstance.Obj.ToString()));
            Assert.That(desObj.IntArray.Length, Is.EqualTo(sampleInstance.IntArray.Length));
        }

        [Test]
        public void CanSuppressMetadata()
        {
            var ser = new YAXSerializer(typeof(YAXLibMetadataOverriding),
                YAXSerializationOptions.SuppressMetadataAttributes);

            var sampleInstance = YAXLibMetadataOverriding.GetSampleInstance();
            var result = ser.Serialize(sampleInstance);

            var expected =
                @"<YAXLibMetadataOverriding>
  <IntArray>
    <Int32>1</Int32>
    <Int32>2</Int32>
    <Int32>3</Int32>
    <Int32>2</Int32>
    <Int32>3</Int32>
    <Int32>4</Int32>
  </IntArray>
  <Obj>Hello, World!</Obj>
</YAXLibMetadataOverriding>";
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void CanSuppressMetadataButUseCustomNamespace()
        {
            var ser = new YAXSerializer(typeof(YAXLibMetadataOverridingWithNamespace),
                YAXSerializationOptions.SuppressMetadataAttributes);

            var sampleInstance = YAXLibMetadataOverridingWithNamespace.GetSampleInstance();
            var result = ser.Serialize(sampleInstance);

            var expected =
                @"<YAXLibMetadataOverridingWithNamespace xmlns=""http://namespace.org/sample"">
  <IntArray>
    <Int32>1</Int32>
    <Int32>2</Int32>
    <Int32>3</Int32>
    <Int32>2</Int32>
    <Int32>3</Int32>
    <Int32>4</Int32>
  </IntArray>
  <Obj>Hello, World!</Obj>
</YAXLibMetadataOverridingWithNamespace>";
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}