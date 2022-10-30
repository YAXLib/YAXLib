// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Xml.Linq;
using YAXLib;
using YAXLib.Attributes;
using YAXLib.Customization;
using YAXLib.Exceptions;

namespace YAXLibTests.SampleClasses;

[YAXSerializeAs("root")]
public class Code4PublicThemesCollection : List<Theme>
{
    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }

    public static Code4PublicThemesCollection GetSampleInstance()
    {
        var codeFont = new FontInfo(Color.Black, false, false);
        var headerBorder = new BorderInfo(true, false, true, true, Color.Blue);
        var headerFont = new FontInfo(Color.Black, true, false);

        var theme = new Theme {
            Name = "SampleTheme",
            CodeContentBackColor = Color.White,
            CodeContentFont = codeFont,
            HeaderBackColor = Color.Aqua,
            HeaderBorder = headerBorder,
            HeaderFont = headerFont,
            HeaderFontSize = 10,
            LineNumbersBackColor = Color.White,
            LineNumbersFont = codeFont,
            LineNumbersSeperatorLineColor = Color.Black,
            LineNumbersShowSeperatorLine = true,
            LineNumbersShowStringAfter = true,
            LineNumbersShowZeros = true,
            LineNumbersSpacesAfter = 2,
            LineNumbersSpacesBefore = 3,
            LineNumbersStringAfter = ":",
            MainContentBorder = headerBorder,
            MainContentFontSize = 9
        };

        var themeCol = new Code4PublicThemesCollection { theme };

        return themeCol;
    }
}

public class Theme
{
    [YAXAttributeFor(".#name")] public string? Name { get; set; }

    [YAXElementFor("Header#Border")] public BorderInfo HeaderBorder { get; set; } = new();

    [YAXElementFor("Header#Font")] public FontInfo HeaderFont { get; set; } = new();

    [YAXAttributeFor("Header/BackColor#value")]
    [YAXCustomSerializer(typeof(ColorSerializer))]
    public Color HeaderBackColor { get; set; }

    [YAXAttributeFor("Header/FontSize#value")]
    public int HeaderFontSize { get; set; }

    [YAXElementFor("MainContent#Border")] public BorderInfo MainContentBorder { get; set; } = new();

    [YAXAttributeFor("MainContent/FontSize#value")]
    public int MainContentFontSize { get; set; }

    [YAXAttributeFor("LineNumbers/SeperatorLine#show")]
    public bool LineNumbersShowSeperatorLine { get; set; }

    [YAXAttributeFor("LineNumbers/SeperatorLine#color")]
    [YAXCustomSerializer(typeof(ColorSerializer))]
    public Color LineNumbersSeperatorLineColor { get; set; }

    [YAXElementFor("LineNumbers#Font")] public FontInfo LineNumbersFont { get; set; } = new();

    [YAXCustomSerializer(typeof(ColorSerializer))]
    [YAXAttributeFor("LineNumbers/BackColor#value")]
    public Color LineNumbersBackColor { get; set; }

    [YAXAttributeFor("LineNumbers/ShowZeros#value")]
    public bool LineNumbersShowZeros { get; set; }

    [YAXAttributeFor("LineNumbers/CharAfterLineNo#show")]
    public bool LineNumbersShowStringAfter { get; set; }

    [YAXAttributeFor("LineNumbers/CharAfterLineNo#value")]
    public string? LineNumbersStringAfter { get; set; }

    [YAXAttributeFor("LineNumbers/SpacesAfter#count")]
    public int LineNumbersSpacesAfter { get; set; }

    [YAXAttributeFor("LineNumbers/SpacesBefore#count")]
    public int LineNumbersSpacesBefore { get; set; }

    [YAXCustomSerializer(typeof(ColorSerializer))]
    [YAXAttributeFor("CodeContent/BackColor#value")]
    public Color CodeContentBackColor { get; set; }

    [YAXElementFor("CodeContent#Font")] public FontInfo CodeContentFont { get; set; } = new();
}

public class BorderInfo
{
    public BorderInfo()
    {
    }

    public BorderInfo(bool top, bool bottom, bool left, bool right, Color color)
    {
        Top = top;
        Bottom = bottom;
        Left = left;
        Right = right;
        Color = color;
    }

    [YAXAttributeFor(".#top")] public bool Top { get; set; }

    [YAXAttributeFor(".#bottom")] public bool Bottom { get; set; }

    [YAXAttributeFor(".#left")] public bool Left { get; set; }

    [YAXAttributeFor(".#right")] public bool Right { get; set; }

    [YAXAttributeFor(".#color")]
    [YAXCustomSerializer(typeof(ColorSerializer))]
    public Color Color { get; set; }
}

public class FontInfo
{
    public FontInfo()
    {
    }

    public FontInfo(Color color, bool bold, bool italic)
    {
        Bold = bold;
        Italic = italic;
        Color = color;
    }

    [YAXAttributeFor(".#bold")] public bool Bold { get; set; }

    [YAXAttributeFor(".#italic")] public bool Italic { get; set; }

    [YAXCustomSerializer(typeof(ColorSerializer))]
    [YAXAttributeFor(".#color")]
    public Color Color { get; set; }
}

internal class ColorSerializer : ICustomSerializer<Color>
{
    public void SerializeToAttribute(Color objectToSerialize, XAttribute attrToFill,
        ISerializationContext serializationContext)
    {
        attrToFill.Value = ColorTo6CharHtmlString(objectToSerialize);
    }

    public void SerializeToElement(Color objectToSerialize, XElement elemToFill,
        ISerializationContext serializationContext)
    {
        elemToFill.Value = ColorTo6CharHtmlString(objectToSerialize);
    }

    public string SerializeToValue(Color objectToSerialize, ISerializationContext serializationContext)
    {
        return ColorTo6CharHtmlString(objectToSerialize);
    }

    public Color DeserializeFromAttribute(XAttribute attribute, ISerializationContext serializationContext)
    {
        if (TryParseColor(attribute.Value, out var color))
            return color;

        throw new YAXBadlyFormedInput(attribute.Name.ToString(), attribute.Value, attribute);
    }

    public Color DeserializeFromElement(XElement element, ISerializationContext serializationContext)
    {
        if (TryParseColor(element.Value, out var color))
            return color;

        throw new YAXBadlyFormedInput(element.Name.ToString(), element.Value, element);
    }

    public Color DeserializeFromValue(string value, ISerializationContext serializationContext)
    {
        if (TryParseColor(value, out var color))
            return color;

        throw new YAXBadlyFormedInput("[SomeValue]", value, null);
    }

    public static string ColorTo8CharString(Color color)
    {
        var str = string.Format("{0:X}", color.ToArgb());

        var sb = new StringBuilder();
        for (var i = 0; i < 8 - str.Length; ++i) sb.Append('0');

        return sb + str;
    }

    public static string ColorTo6CharString(Color color)
    {
        return ColorTo8CharString(color).Substring(2);
    }

    public static string ColorTo6CharHtmlString(Color color)
    {
        return "#" + ColorTo6CharString(color);
    }

    public static bool TryParseColor(string strColor, out Color color)
    {
        color = Color.White;

        strColor = strColor.Trim();
        if (strColor.StartsWith("#")) // remove leading # if any
            strColor = strColor.Substring(1);

        int n;
        if (int.TryParse(strColor, NumberStyles.HexNumber, null, out n))
        {
            color = Color.FromArgb(n);
            // sets the alpha value to 255
            color = Color.FromArgb(255, color.R, color.G, color.B);
            return true;
        }

        return false;
    }
}