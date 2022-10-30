// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Drawing;
using System.Globalization;
using YAXLib.Attributes;

namespace YAXLibTests.SampleClasses;

[ShowInDemoApplication]
[YAXComment("This example shows a technique for serializing classes without a default constructor")]
public class ColorExample
{
    private Color _color = Color.Blue;

    public string TheColor
    {
        get { return string.Format("#{0:X}", _color.ToArgb()); }

        set
        {
            _color = Color.White;

            value = value.Trim();
            if (value.StartsWith("#")) // remove leading # if any
                value = value.Substring(1);

            int n;
            if (int.TryParse(value, NumberStyles.HexNumber, null, out n)) _color = Color.FromArgb(n);
        }
    }

    public override string ToString()
    {
        //return GeneralToStringProvider.GeneralToString(this);
        return string.Format("TheColor: {0}", _color.ToString());
    }

    public static ColorExample GetSampleInstance()
    {
        return new ColorExample();
    }
}