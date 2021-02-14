// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Drawing;
using System.Globalization;
using YAXLib;

namespace YAXLibTests.SampleClasses
{
#if !NETSTANDARD
    [ShowInDemoApplication]
    [YAXComment("This example shows a technique for serializing classes without a default constructor")]
    public class ColorExample
    {
        private Color m_color = Color.Blue;

        public string TheColor
        {
            get { return string.Format("#{0:X}", m_color.ToArgb()); }

            set
            {
                m_color = Color.White;

                value = value.Trim();
                if (value.StartsWith("#")) // remove leading # if any
                    value = value.Substring(1);

                int n;
                if (int.TryParse(value, NumberStyles.HexNumber, null, out n)) m_color = Color.FromArgb(n);
            }
        }

        public override string ToString()
        {
            //return GeneralToStringProvider.GeneralToString(this);
            return string.Format("TheColor: {0}", m_color.ToString());
        }

        public static ColorExample GetSampleInstance()
        {
            return new ColorExample();
        }
    }
#endif
}