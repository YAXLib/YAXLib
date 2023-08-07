// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Text.RegularExpressions;

namespace YAXLibTests;

public static class StringExtensionsForTest
{
    public static string StripTypeAssemblyVersion(this string str)
    {
        const string pattern =
            """\,\s+(mscorlib|System\.Private\.CoreLib)\,\s+Version\=\d+(\.\d+)*\,\s+Culture=\b\w+\b\,\s+PublicKeyToken\=\b\w+\b""";
        return Regex.Replace(str, pattern, string.Empty);
    }
}