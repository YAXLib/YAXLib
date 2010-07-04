using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YAXLib;

namespace DemoApplication.SampleClasses
{
    public class BookClassTesgingSerializeAsValue
    {
        public string Title { get; set; }

        public string Author { get; set; }
        public int PublishYear { get; set; }

        [YAXValueFor("./somthing")]
        public double? Price { get; set; }

        [YAXValueFor(".")]
        public string Comments { get; set; }

        [YAXValueFor("SomeOtherThing")]
        public string SomethingElse { get; set; }


        public override string ToString()
        {
            return GeneralToStringProvider.GeneralToString(this);
        }

        public static BookClassTesgingSerializeAsValue GetSampleInstance()
        {
            return new BookClassTesgingSerializeAsValue()
            {
                Title = "Inside C#",
                Author = "Tom Archer & Andrew Whitechapel",
                PublishYear = 2002,
                Price = 30.5,
                Comments = " Line1 " + Environment.NewLine + "\tLine2\t" + Environment.NewLine + "    Line3  " + Environment.NewLine + "   Line4  " + Environment.NewLine,
                SomethingElse = "Something Else"
            };
        }
    }
}
