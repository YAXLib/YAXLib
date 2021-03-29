// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Xml.Linq;

using FluentAssertions;
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

        /// <summary>
        /// Get YAXSerializer properties which are wrappers to <see cref="SerializerOptions"/>
        /// </summary>
        [Test]
        public void Get_YAXSerializer_WrapperProps_Test()
        {
            var opt = GetNonDefaultOptions();
            var ser = new YAXSerializer(typeof(SerializerOptions), opt);

            ser.DefaultExceptionType.Should().Be(opt.ExceptionBehavior);
            ser.SerializationOption.Should().Be(opt.SerializationOptions);
            ser.ExceptionHandlingPolicy.Should().Be(opt.ExceptionHandlingPolicies);
            ser.YaxLibNamespacePrefix.Should().Be(opt.Namespace.Prefix);
            ser.YaxLibNamespaceUri.Should().Be(opt.Namespace.Uri);
            ser.DimensionsAttributeName.Should().Be(opt.AttributeName.Dimensions);
            ser.RealTypeAttributeName.Should().Be(opt.AttributeName.RealType);
            ser.MaxRecursion.Should().Be(opt.MaxRecursion);
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
            ser.YaxLibNamespacePrefix.Should().Be(opt.Namespace.Prefix);
            ser.YaxLibNamespaceUri.Should().Be(opt.Namespace.Uri);
            ser.DimensionsAttributeName.Should().Be(opt.AttributeName.Dimensions);
            ser.RealTypeAttributeName.Should().Be(opt.AttributeName.RealType);
            ser.MaxRecursion.Should().Be(opt.MaxRecursion);
        }
    }
}
