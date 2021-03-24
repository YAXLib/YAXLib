// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Xml.Linq;
using NUnit.Framework;
using YAXLib;

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
                ExceptionBehavior = YAXExceptionTypes.Error,
                ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.ThrowWarningsAndErrors,
                AttributeName = new SerializerOptions.YAXAttributeName
                    {Dimensions = "test-dims", RealType = "test-realtype"},
                Namespace = new SerializerOptions.YAXNameSpace
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

            Assert.IsFalse(
                opt1.ExceptionBehavior == opt2.ExceptionBehavior ||
                opt1.ExceptionHandlingPolicies == opt2.ExceptionHandlingPolicies ||
                opt1.AttributeName.Dimensions == opt2.AttributeName.Dimensions ||
                opt1.Namespace.Prefix == opt2.Namespace.Prefix ||
                opt1.Namespace.Uri == opt2.Namespace.Uri ||
                opt1.MaxRecursion == opt2.MaxRecursion ||
                opt1.SerializationOptions == opt2.SerializationOptions
            );
        }

        /// <summary>
        /// Get YAXSerializer properties which are wrappers to <see cref="SerializerOptions"/>
        /// </summary>
        [Test]
        public void Get_YAXSerializer_WrapperProps_Test()
        {
            var opt = GetNonDefaultOptions();
            var ser = new YAXSerializer(typeof(SerializerOptions), opt);

            Assert.IsTrue(
                ser.DefaultExceptionType == opt.ExceptionBehavior &&
                ser.SerializationOption == opt.SerializationOptions &&
                ser.ExceptionHandlingPolicy == opt.ExceptionHandlingPolicies &&
                ser.YaxLibNamespacePrefix == opt.Namespace.Prefix &&
                ser.YaxLibNamespaceUri == opt.Namespace.Uri &&
                ser.DimensionsAttributeName == opt.AttributeName.Dimensions &&
                ser.RealTypeAttributeName == opt.AttributeName.RealType &&
                ser.MaxRecursion == opt.MaxRecursion
            );
        }
        
        /// <summary>
        /// Set YAXSerializer properties which are wrappers to <see cref="SerializerOptions"/>
        /// </summary>
        [Test]
        public void Set_YAXSerializer_WrapperProps_Test()
        {
            var opt = GetNonDefaultOptions();
            var ser = new YAXSerializer(typeof(SerializerOptions), new SerializerOptions());

            // These don't have setters:
            // ser.Options = new SerializerOptions();
            // ser.DefaultExceptionType = opt.ExceptionBehavior;
            // ser.SerializationOption = opt.SerializationOptions;
            // ser.ExceptionHandlingPolicy = opt.ExceptionHandlingPolicies;
            
            ser.YaxLibNamespacePrefix = opt.Namespace.Prefix;
            ser.YaxLibNamespaceUri = opt.Namespace.Uri;
            ser.DimensionsAttributeName = opt.AttributeName.Dimensions;
            ser.RealTypeAttributeName = opt.AttributeName.RealType;
            ser.MaxRecursion = opt.MaxRecursion;
            
            Assert.IsTrue(
                ser.YaxLibNamespacePrefix == opt.Namespace.Prefix &&
                ser.YaxLibNamespaceUri == opt.Namespace.Uri &&
                ser.DimensionsAttributeName == opt.AttributeName.Dimensions &&
                ser.RealTypeAttributeName == opt.AttributeName.RealType &&
                ser.MaxRecursion == opt.MaxRecursion
            );
        }
    }
}
