using System;
using NUnit.Framework;
using YAXLib;

namespace YAXLibTests
{
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
                var ser = new YAXSerializer(typeof (double), YAXExceptionHandlingPolicies.ThrowErrorsOnly,
                    YAXExceptionTypes.Warning, YAXSerializationOptions.SerializeNullObjects);
                double d = 0.55;
                var xml = ser.Serialize(d);
                var deseredInstance = ser.Deserialize(xml);
                Assert.AreEqual(d, deseredInstance);

                d = double.MaxValue;
                xml = ser.Serialize(d);
                deseredInstance = ser.Deserialize(xml);
                    // Causes a System.OverflowException {"Value was either too large or too small for a Double."}
                Assert.AreEqual(d, deseredInstance);
            }
            catch (Exception ex)
            {
                Assert.Fail("No exception should have been throwned, but received:" + Environment.NewLine + ex);
            }
        }

        [Test]
        public void TestDoubleMin()
        {
            try
            {
                var ser = new YAXSerializer(typeof (double), YAXExceptionHandlingPolicies.ThrowErrorsOnly,
                    YAXExceptionTypes.Warning, YAXSerializationOptions.SerializeNullObjects);
                double d = double.MinValue;
                var xml = ser.Serialize(d);
                var deseredInstance = ser.Deserialize(xml);
                Assert.AreEqual(d, deseredInstance);
            }
            catch (Exception ex)
            {
                Assert.Fail("No exception should have been throwned, but received:" + Environment.NewLine + ex);
            }
        }

        [Test]
        public void TestSingleMax()
        {
            try
            {
                var ser = new YAXSerializer(typeof (float), YAXExceptionHandlingPolicies.ThrowErrorsOnly,
                    YAXExceptionTypes.Warning, YAXSerializationOptions.SerializeNullObjects);
                float f = float.MaxValue;
                var xml = ser.Serialize(f);
                var deseredInstance = ser.Deserialize(xml);
                Assert.AreEqual(f, deseredInstance);
            }
            catch (Exception ex)
            {
                Assert.Fail("No exception should have been throwned, but received:" + Environment.NewLine + ex);
            }
        }

        [Test]
        public void TestSingleMin()
        {
            try
            {
                var ser = new YAXSerializer(typeof (float), YAXExceptionHandlingPolicies.ThrowErrorsOnly,
                    YAXExceptionTypes.Warning, YAXSerializationOptions.SerializeNullObjects);
                float f = float.MinValue;
                var xml = ser.Serialize(f);
                var deseredInstance = ser.Deserialize(xml);
                Assert.AreEqual(f, deseredInstance);
            }
            catch (Exception ex)
            {
                Assert.Fail("No exception should have been throwned, but received:" + Environment.NewLine + ex);
            }
        }

    }
}
