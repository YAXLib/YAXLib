using YAXLib;

namespace YAXLibTests.SampleClasses
{
    class ClassWithDuplicateYaxValue
    {
        [YAXSerializableField]
        [YAXValueForClass]
        public string Value1 { get; set; }

        [YAXSerializableField]
        [YAXValueForClass]
        public string Value2 { get; set; }

        public static ClassWithDuplicateYaxValue GetSampleInstance()
        {
            return new ClassWithDuplicateYaxValue()
            {
                Value1 = "lorum ipsum",
                Value2 = "lorum oopsum"
            };
        }
    }

    class ClassWithInvalidFormat
    {
        [YAXSerializableField]
        [YAXFormat("fancyFormat")]
        public int Value1 { get; set; }

        public static ClassWithInvalidFormat GetSampleInstance()
        {
            return new ClassWithInvalidFormat()
            {
                Value1 = 500,
            };
        }
    }
}