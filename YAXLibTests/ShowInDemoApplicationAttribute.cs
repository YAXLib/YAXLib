using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YAXLibTests
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class ShowInDemoApplicationAttribute : Attribute
    {
        public ShowInDemoApplicationAttribute()
        {
            SortKey = null;
        }

        public string SortKey { get; set; }
    }
}
