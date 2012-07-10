﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YAXLib;

namespace YAXLibTests.SampleClasses
{
    [YAXComment("This example shows usage of a number of custom namespaces")]
    [YAXNamespace("ns1", "http://namespaces.org/ns1")]
    public class MultipleNamespaceSample
    {
        public static MultipleNamespaceSample GetInstance()
        {
            return new MultipleNamespaceSample()
            {
                BoolItem = true,
                StringItem = "This is a test string",
                IntItem = 10
            };
        }

        public bool BoolItem
        { get; set; }

        [YAXNamespace("ns2", "http://namespaces.org/ns2")]
        public string StringItem
        { get; set; }

        [YAXNamespace("ns3", "http://namespaces.org/ns3")]
        public int IntItem
        { get; set; }
    }    
}
