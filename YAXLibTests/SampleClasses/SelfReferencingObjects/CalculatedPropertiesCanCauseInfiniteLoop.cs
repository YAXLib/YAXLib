using System;

namespace YAXLibTests.SampleClasses.SelfReferencingObjects
{
    [ShowInDemoApplication(SortKey = "_")]
    public class CalculatedPropertiesCanCauseInfiniteLoop
    {
        public decimal Data { get; set; }

        public CalculatedPropertiesCanCauseInfiniteLoop Reciprocal
        {
            get
            {
                if (Data == 0M)
                    return null;

                decimal reciprocal = 1.0M/Data;
                return new CalculatedPropertiesCanCauseInfiniteLoop {Data = reciprocal};
            }
        }

        public static CalculatedPropertiesCanCauseInfiniteLoop GetSampleInstance()
        {
            return new CalculatedPropertiesCanCauseInfiniteLoop{Data = 2.0M};
        }

        public override string ToString()
        {
            return GeneralToStringProvider.GeneralToString(this);
        }
    }
}
