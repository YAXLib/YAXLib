﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YAXLib;

namespace YAXLibTests.SampleClasses
{
    [YAXNamespace("http://namespaces.org/default")]
    public class AttributeNamespaceSample
    {
        [YAXAttributeFor("Attribs")]
        public string attrib
        {
            get;
            private set;
        }

        [YAXAttributeFor("Attribs")]
        [YAXNamespace("ns","http://namespaces.org/ns")]
        public string attrib2
        { get; private set; }

        public static AttributeNamespaceSample GetInstance()
        {
            return new AttributeNamespaceSample()
            {
                attrib = "value",
                attrib2 = "value2"
            };
        }
    }
}
