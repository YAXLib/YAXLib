using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YAXLib;
using System.Collections;

namespace DemoApplication.SampleClasses
{
    public class FreeSample
    {
        public IEnumerable RefToIEnumerable1 { get; set; }
        public IEnumerable RefToIEnumerable2 { get; set; }
        public IEnumerable RefToIEnumerable3 { get; set; }

        public List<object> ListOfObjects { get; set; }

        public static FreeSample GetSampleInstance()
        {
            int[] someIntArray = new int[] { 1, 2, 3 };
            string[] someStrArray = new string[] { "Hi", "Hello" };

            List<object> lst = new List<object>();
            lst.Add(7); // adding an integer
            lst.Add(3.14); // adding a double
            lst.Add("Congrats"); // adding a string
            lst.Add(StringSplitOptions.RemoveEmptyEntries); // adding some enum member

            return new FreeSample()
            {
                RefToIEnumerable1 = someIntArray,
                RefToIEnumerable2 = someStrArray,
                RefToIEnumerable3 = GetOdds(),
                ListOfObjects = lst
            };
        }

        public static IEnumerable<int> GetOdds()
        {
            for (int i = 1; i < 20; i += 2)
                yield return i;
        }

        public override string ToString()
        {
            return GeneralToStringProvider.GeneralToString(this);
        }

    }
}
