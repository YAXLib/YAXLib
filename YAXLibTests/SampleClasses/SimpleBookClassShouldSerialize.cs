using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YAXLib;

namespace YAXLibTests.SampleClasses
{
    [YAXComment("This example demonstrates serailizing a very simple class")]
    public class BookShouldSerialize
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public int PublishYear { get; set; }
        public double Price { get; set; }

        public bool ShouldSerializePrice()
        {
            return Price != 0;
        }

        public override string ToString()
        {
            return GeneralToStringProvider.GeneralToString(this);
        }

        public static BookShouldSerialize GetSampleInstance()
        {
            return new BookShouldSerialize()
            {
                Title = "Inside C#",
                Author = "Tom Archer & Andrew Whitechapel",
                PublishYear = 2002,
                Price = 0
            };
        }
    }
}
