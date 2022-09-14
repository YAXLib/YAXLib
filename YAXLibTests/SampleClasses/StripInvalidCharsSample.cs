// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YAXLib.Attributes;

namespace YAXLibTests.SampleClasses
{
    internal class StripInvalidCharsSample
    {
        [YAXValueForClass]
        public string ValueForClass { get; set; } = nameof(ValueForClass);

        public string ValueOfElement { get; set; } = nameof(ValueOfElement);

        [YAXAttributeFor(nameof(ValueOfElement))]
        public string ValueForAttribute { get; set; } = nameof(ValueForAttribute);

        public List<string> TheList { get; set; } = new();

        public Dictionary<string, string> TheDictionary { get; set; } = new();
    }
}
