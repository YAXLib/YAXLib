using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YAXLib;

namespace YAXLibTests.SampleClasses
{
    [ShowInDemoApplication(SortKey = "_")]
    public class OneLetterAlias
    {
        [YAXValueFor("./T")]
        public string Title { get; set; }

        [YAXValueFor("./A")]
        public string Author { get; set; }

        public override string ToString()
        {
            return GeneralToStringProvider.GeneralToString(this);
        }

        public static OneLetterAlias GetSampleInstance()
        {
            return new OneLetterAlias
            {
                Title = "Inside C#",
                Author = "Tom Archer & Andrew Whitechapel",
            };
        }
    }
}
