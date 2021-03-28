// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Xml.Linq;
using NUnit.Framework;
using YAXLib;
using YAXLib.Options;

namespace YAXLibTests
{
    [TestFixture]
    public class SerializerOptionsTests
    {
        private SerializerOptions GetDefaultOptions()
        {
            return new();
        }

        private SerializerOptions GetNonDefaultOptions()
        {
            return new() {
                ExceptionBehavior = YAXExceptionTypes.Ignore,
                ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
                AttributeName = new YAXAttributeName
                    {Dimensions = "test-dims", RealType = "test-realtype"},
                Namespace = new YAXNameSpace
                    {Prefix = "testyaxlib", Uri = XNamespace.Get("https://test.sinairv.com/yaxlib/")},
                MaxRecursion = 123,
                SerializationOptions = YAXSerializationOptions.SuppressMetadataAttributes
            };
        }

        [Test]
        public void Get_Set_All_Options_Test()
        {
            var opt1 = GetDefaultOptions();
            var opt2 = GetNonDefaultOptions();

            Assert.IsFalse(opt1.ExceptionBehavior == opt2.ExceptionBehavior);
            Assert.IsFalse(opt1.ExceptionHandlingPolicies == opt2.ExceptionHandlingPolicies);
            Assert.IsFalse(opt1.AttributeName.Dimensions == opt2.AttributeName.Dimensions);
            Assert.IsFalse(opt1.Namespace.Prefix == opt2.Namespace.Prefix);
            Assert.IsFalse(opt1.Namespace.Uri == opt2.Namespace.Uri);
            Assert.IsFalse(opt1.MaxRecursion == opt2.MaxRecursion);
            Assert.IsFalse(opt1.SerializationOptions == opt2.SerializationOptions);
        }

        /// <summary>
        /// Get YAXSerializer properties which are wrappers to <see cref="SerializerOptions"/>
        /// </summary>
        [Test]
        public void Get_YAXSerializer_WrapperProps_Test()
        {
            var opt = GetNonDefaultOptions();
            var ser = new YAXSerializer(typeof(SerializerOptions), opt);

            Assert.IsTrue(ser.DefaultExceptionType == opt.ExceptionBehavior);
            Assert.IsTrue(ser.SerializationOption == opt.SerializationOptions);
            Assert.IsTrue(ser.ExceptionHandlingPolicy == opt.ExceptionHandlingPolicies);
            Assert.IsTrue(ser.YaxLibNamespacePrefix == opt.Namespace.Prefix);
            Assert.IsTrue(ser.YaxLibNamespaceUri == opt.Namespace.Uri);
            Assert.IsTrue(ser.DimensionsAttributeName == opt.AttributeName.Dimensions);
            Assert.IsTrue(ser.RealTypeAttributeName == opt.AttributeName.RealType);
            Assert.IsTrue(ser.MaxRecursion == opt.MaxRecursion);
        }
        
        /// <summary>
        /// Set YAXSerializer properties which are wrappers to <see cref="SerializerOptions"/>
        /// </summary>
        [Test]
        public void Set_YAXSerializer_WrapperProps_Test()
        {
            var opt = GetNonDefaultOptions();
            var ser = new YAXSerializer(typeof(SerializerOptions), new SerializerOptions());
            
            ser.YaxLibNamespacePrefix = opt.Namespace.Prefix;
            ser.YaxLibNamespaceUri = opt.Namespace.Uri;
            ser.DimensionsAttributeName = opt.AttributeName.Dimensions;
            ser.RealTypeAttributeName = opt.AttributeName.RealType;
            ser.MaxRecursion = opt.MaxRecursion;
            
            Assert.IsTrue(ser.YaxLibNamespacePrefix == opt.Namespace.Prefix);
            Assert.IsTrue(ser.YaxLibNamespaceUri == opt.Namespace.Uri);
            Assert.IsTrue(ser.DimensionsAttributeName == opt.AttributeName.Dimensions);
            Assert.IsTrue(ser.RealTypeAttributeName == opt.AttributeName.RealType);
            Assert.IsTrue(ser.MaxRecursion == opt.MaxRecursion);
        }
    }
}
