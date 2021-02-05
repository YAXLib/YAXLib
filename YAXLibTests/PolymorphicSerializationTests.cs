using NUnit.Framework;
using YAXLib;
using YAXLibTests.SampleClasses.PolymorphicSerialization;

namespace YAXLibTests
{
    [TestFixture]
    public class PolymorphicSerializationTests
    {
        [Test]
        public void MultipleYaxTypeAttributesWithSameTypeMustThrowAnException()
        {
            var ser = new YAXSerializer(typeof(MultipleYaxTypeAttributesWithSameType));
            var obj = new MultipleYaxTypeAttributesWithSameType();
            Assert.Throws<YAXPolymorphicException>(() => ser.Serialize(obj));
        }

        [Test]
        public void MultipleYaxTypeAttributesWIthSameAliasMustThrowAnException()
        {
            var ser = new YAXSerializer(typeof(MultipleYaxTypeAttributesWithSameAlias));
            var obj = new MultipleYaxTypeAttributesWithSameAlias();
            Assert.Throws<YAXPolymorphicException>(() => ser.Serialize(obj));
        }
    }
}
