// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using YAXLib;
using YAXLib.Attributes;

namespace YAXLibTests.SampleClasses
{
    [ShowInDemoApplication]
    public class YAXLibMetadataOverriding
    {
        public int[,] IntArray { get; set; }

        public object Obj { get; set; }


        public override string ToString()
        {
            return GeneralToStringProvider.GeneralToString(this);
        }

        public static YAXLibMetadataOverriding GetSampleInstance()
        {
            var instance = new YAXLibMetadataOverriding();
            instance.SetSampleData();
            return instance;
        }

        protected void SetSampleData()
        {
            IntArray = new int[2, 3];

            for (var i = 0; i < 2; i++)
            for (var j = 0; j < 3; j++)
                IntArray[i, j] = i + j + 1;

            Obj = "Hello, World!";
        }
    }

    [ShowInDemoApplication]
    [YAXNamespace("http://namespace.org/sample")]
    public class YAXLibMetadataOverridingWithNamespace : YAXLibMetadataOverriding
    {
        public new static YAXLibMetadataOverridingWithNamespace GetSampleInstance()
        {
            var instance = new YAXLibMetadataOverridingWithNamespace();
            instance.SetSampleData();
            return instance;
        }
    }
}