// Copyright 2009 - 2010 Sina Iravanian - <sina@sinairv.com>
//
// This source file(s) may be redistributed, altered and customized
// by any means PROVIDING the authors name and all copyright
// notices remain intact.
// THIS SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED. USE IT AT YOUR OWN RISK. THE AUTHOR ACCEPTS NO
// LIABILITY FOR ANY DATA DAMAGE/LOSS THAT THIS PRODUCT MAY CAUSE.
//-----------------------------------------------------------------------

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YAXLib;
using System.Threading;
using System.Globalization;
using YAXLibTests.SampleClasses;

namespace YAXLibTests
{
    [TestClass]
    public class NamespaceTest
    {
        [TestMethod]
        public void SingleNamespaceSerializationTest()
        {
            const string result = @"<!-- This example shows usage of a custom default namespace -->
" + "<SingleNamespaceSample xmlns=\"http://namespaces.org/default\">" + @"
  <StringItem>This is a test string</StringItem>
  <IntItem>10</IntItem>
</SingleNamespaceSample>";

            var serializer = new YAXSerializer(typeof(SingleNamespaceSample), YAXExceptionHandlingPolicies.DoNotThrow, YAXExceptionTypes.Warning, YAXSerializationOptions.SerializeNullObjects);
            string got = serializer.Serialize(SingleNamespaceSample.GetInstance());
            Assert.AreEqual(result, got);
        }

        [TestMethod]
        public void MultipleNamespaceSerializationTest()
        {
            const string result = @"<!-- This example shows usage of a number of custom namespaces -->
" + "<ns1:MultipleNamespaceSample xmlns:ns1=\"http://namespaces.org/ns1\" xmlns:ns2=\"http://namespaces.org/ns2\" xmlns:ns3=\"http://namespaces.org/ns3\">" + @"
  <ns1:BoolItem>True</ns1:BoolItem>
  <ns2:StringItem>This is a test string</ns2:StringItem>
  <ns3:IntItem>10</ns3:IntItem>
</ns1:MultipleNamespaceSample>";

            var serializer = new YAXSerializer(typeof(MultipleNamespaceSample), YAXExceptionHandlingPolicies.DoNotThrow, YAXExceptionTypes.Warning, YAXSerializationOptions.SerializeNullObjects);
            string got = serializer.Serialize(MultipleNamespaceSample.GetInstance());
            Assert.AreEqual(result, got);
        }

        [TestMethod]
        public void AttributeNamespaceSerializationTest()
        {
            const string result = "<AttributeNamespaceSample xmlns:ns=\"http://namespaces.org/ns\" xmlns=\"http://namespaces.org/default\">" + @"
  <Attribs " + "attrib=\"value\" ns:attrib2=\"value2\"" + @" />
</AttributeNamespaceSample>";

            var serializer = new YAXSerializer(typeof(AttributeNamespaceSample), YAXExceptionHandlingPolicies.DoNotThrow, YAXExceptionTypes.Warning, YAXSerializationOptions.SerializeNullObjects);
            string got = serializer.Serialize(AttributeNamespaceSample.GetInstance());
            Assert.AreEqual(result, got);
        }

        [TestMethod]
        public void SingleNamespaceDeserializationTest()
        {            
            var serializer = new YAXSerializer(typeof(SingleNamespaceSample), YAXExceptionHandlingPolicies.DoNotThrow, YAXExceptionTypes.Warning, YAXSerializationOptions.SerializeNullObjects);
            string serialized = serializer.Serialize(SingleNamespaceSample.GetInstance());
            var deserialized = serializer.Deserialize(serialized) as SingleNamespaceSample;
            Assert.IsNotNull(deserialized);
            Assert.AreEqual(0, serializer.ParsingErrors.Count);
        }

        [TestMethod]
        public void MultipleNamespaceDeserializationTest()
        {
            var serializer = new YAXSerializer(typeof(MultipleNamespaceSample), YAXExceptionHandlingPolicies.DoNotThrow, YAXExceptionTypes.Warning, YAXSerializationOptions.SerializeNullObjects);
            string serialized = serializer.Serialize(MultipleNamespaceSample.GetInstance());
            var deserialized = serializer.Deserialize(serialized) as MultipleNamespaceSample;
            Assert.IsNotNull(deserialized);
            Assert.AreEqual(0, serializer.ParsingErrors.Count);
        }
        
        [TestMethod]
        public void AttributeNamespaceDeserializationTest()
        {
            var serializer = new YAXSerializer(typeof(AttributeNamespaceSample), YAXExceptionHandlingPolicies.DoNotThrow, YAXExceptionTypes.Warning, YAXSerializationOptions.SerializeNullObjects);
            string got = serializer.Serialize(AttributeNamespaceSample.GetInstance());
            var deserialized = serializer.Deserialize(got) as AttributeNamespaceSample;
            Assert.IsNotNull(deserialized);
            Assert.AreEqual(0, serializer.ParsingErrors.Count);
        }
    }
}
