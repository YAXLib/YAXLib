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
        public void BasicNamespaceSerializationTest()
        {
            var serializer = new YAXSerializer(typeof(SingleNamespaceSample), YAXExceptionHandlingPolicies.DoNotThrow, YAXExceptionTypes.Warning, YAXSerializationOptions.SerializeNullObjects);
            string got = serializer.Serialize(SingleNamespaceSample.GetInstance());
            Assert.IsNotNull(got);
        }
    }
}
