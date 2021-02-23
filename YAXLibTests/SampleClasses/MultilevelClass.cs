﻿// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Collections.Generic;
using YAXLib;

namespace YAXLibTests.SampleClasses
{
    [ShowInDemoApplication]
    [YAXComment(@"This example shows a multi-level class, which helps to test 
      the null references identity problem. 
      Thanks go to Anton Levshunov for proposing this example,
      and a discussion on this matter.")]
    public class MultilevelClass
    {
        public List<FirstLevelClass> items { get; set; }

        public override string ToString()
        {
            return GeneralToStringProvider.GeneralToString(this);
        }

        public static MultilevelClass GetSampleInstance()
        {
            var obj = new MultilevelClass();
            obj.items = new List<FirstLevelClass>();
            obj.items.Add(new FirstLevelClass());
            obj.items.Add(new FirstLevelClass());

            obj.items[0].Second = new SecondLevelClass();
            obj.items[0].ID = "1";
            obj.items[0].Second.SecondID = "1-2";

            obj.items[1].ID = "2";
            obj.items[1].Second = null;
            return obj;
        }
    }

    public class FirstLevelClass
    {
        public string ID { get; set; }

        public SecondLevelClass Second { get; set; }
    }

    public class SecondLevelClass
    {
        public string SecondID { get; set; }
    }
}