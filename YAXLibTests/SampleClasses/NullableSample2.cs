using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YAXLib;

namespace YAXLibTests.SampleClasses
{
    public class NullableSample2
    {
        [YAXAttributeForClass]
        public int? Number { get; set; }

        public override string ToString()
        {
            return GeneralToStringProvider.GeneralToString(this);
        }

        public static NullableSample2 GetSampleInstance()
        {
            return new NullableSample2() { Number = 10 };
        }
    }
}
