using System.Drawing;
using System.Xml.Linq;

using NUnit.Framework;

using YAXLib;
using System;
using YAXLibTests.SampleClasses;

namespace YAXLibTests
{
    [TestFixture]
    public class KnownTypeTests
    {
        [Test]
        public void TestExtensionMethod()
        {
            var colorKnownType = new ColorKnownType();
            var t1 = colorKnownType.Type;
            IKnownType kt = new ColorKnownType();

            Assert.That(kt.Type, Is.EqualTo(t1));
        }

        [Test]
        public void TestColorNames()
        {
            var colorKnownType = new ColorKnownType();

            var elem = new XElement("TheColor", "Red");
            var desCl = colorKnownType.Deserialize(elem, "");
            Assert.That(desCl.ToArgb(), Is.EqualTo(Color.Red.ToArgb()));

            var serElem = new XElement("TheColor");
            colorKnownType.Serialize(Color.Red, serElem, "");
            Assert.That(serElem.ToString(), Is.EqualTo(elem.ToString()));

            var elemRgbForRed = new XElement("TheColor", 
                new XElement("A", 255),
                new XElement("R", 255),
                new XElement("G", 0),
                new XElement("B", 0));
            var desCl2 = colorKnownType.Deserialize(elemRgbForRed, "");
            Assert.That(desCl2.ToArgb(), Is.EqualTo(Color.Red.ToArgb()));

            var elemRgbAndValueForRed = new XElement("TheColor",
                "Blue",
                new XElement("R", 255),
                new XElement("G", 0),
                new XElement("B", 0));
            var desCl3 = colorKnownType.Deserialize(elemRgbAndValueForRed, "");
            Assert.That(desCl3.ToArgb(), Is.EqualTo(Color.Red.ToArgb()));
        }

        [Test]
        public void TestWrappers()
        {
            var typeToTest = typeof (TimeSpan);
            var serializer = new YAXSerializer(typeToTest);
            var typeWrapper = new UdtWrapper(typeToTest, serializer);

            Assert.That(typeWrapper.IsKnownType, Is.True);
        }

        [Test]
        public void TestSingleKnownTypeSerialization()
        {
            var typeToTest = typeof(Color);
            var serializer = new YAXSerializer(typeToTest);

            var col1 = Color.FromArgb(145, 123, 123);
            var colStr1 = serializer.Serialize(col1);

            const string expectedCol1 = @"<Color>
  <A>255</A>
  <R>145</R>
  <G>123</G>
  <B>123</B>
</Color>";

            Assert.That(colStr1, Is.EqualTo(expectedCol1));

            var col2 = SystemColors.ButtonFace;
            var colStr2 = serializer.Serialize(col2);
            const string expectedCol2 = @"<Color>ButtonFace</Color>";

            Assert.That(colStr2, Is.EqualTo(expectedCol2));
        }

        [Test]
        public void TestSerializingNDeserializingNullKnownTypes()
        {
            var inst = ClassContainingXElement.GetSampleInstance();
            inst.TheElement = null;
            inst.TheAttribute = null;

            var ser = new YAXSerializer(typeof (ClassContainingXElement), YAXExceptionHandlingPolicies.ThrowErrorsOnly,
                                        YAXExceptionTypes.Warning, YAXSerializationOptions.SerializeNullObjects);

            try
            {
                var xml = ser.Serialize(inst);
                var deseredInstance = ser.Deserialize(xml);
                Assert.That(deseredInstance.ToString(), Is.EqualTo(inst.ToString()));
            }
            catch (Exception ex)
            {
                Assert.Fail("No exception should have been throwned, but received:\r\n" + ex);
            }

        }
    }
}
