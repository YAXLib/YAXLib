// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

#nullable enable

namespace YAXLib;

internal interface IRecursionCounter
{
    /// <summary>
    ///     Gets or sets the number of recursions (number of total created <see cref="YAXSerializer"/> instances).
    /// </summary>
    int RecursionCount { get; set; }
}
