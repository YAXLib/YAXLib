// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Globalization;
using YAXLib.Enums;
using YAXLib.Exceptions;
using YAXLib.Pooling.SpecializedPools;

namespace YAXLib;

/// <summary>
/// Holds list of exception occurred during serialization/deserialization.
/// </summary>
public class YAXParsingErrors
{
    #region Fields

    /// <summary>
    /// The list of exception occurred during serialization/deserialization.
    /// </summary>
    private readonly List<KeyValuePair<YAXException, YAXExceptionTypes>> _listExceptions = new();

    #endregion

    #region Indexers

    /// <summary>
    /// Gets the the pair of Exception and its corresponding exception-type with the specified n.
    /// </summary>
    /// <param name="n">The index of the exception/exception type pair in the error list to return.</param>
    /// <value></value>
    public KeyValuePair<YAXException, YAXExceptionTypes> this[int n] => _listExceptions[n];

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether the list of errors is empty or not.
    /// </summary>
    /// <value><c>true</c> if the list is not empty; otherwise, <c>false</c>.</value>
    public bool ContainsAnyError => _listExceptions.Count > 0;

    /// <summary>
    /// Gets the number of errors within the list of errors.
    /// </summary>
    /// <value>the number of errors within the list of errors.</value>
    public int Count => _listExceptions.Count;

    #endregion

    #region Methods

    /// <summary>
    /// Adds an exception with the corresponding type.
    /// </summary>
    /// <param name="exception">The exception to add.</param>
    /// <param name="exceptionType">Type of the exception added.</param>
    public void AddException(YAXException exception, YAXExceptionTypes exceptionType)
    {
        _listExceptions.Add(new KeyValuePair<YAXException, YAXExceptionTypes>(exception, exceptionType));
    }

    /// <summary>
    /// Clears the list of errors.
    /// </summary>
    public void ClearErrors()
    {
        _listExceptions.Clear();
    }

    /// <summary>
    /// Adds the list of errors within another instance of <c>YAXParsingErrors</c>.
    /// </summary>
    /// <param name="parsingErrors">The parsing errors to add its content.</param>
    public void AddRange(YAXParsingErrors parsingErrors)
    {
        parsingErrors._listExceptions.ForEach(_listExceptions.Add);
    }

    /// <summary>
    /// Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
    /// </returns>
    public override string ToString()
    {
        using var pooledObject = StringBuilderPool.Instance.Get(out var sb);

        _listExceptions.ForEach(pair =>
        {
            sb.AppendLine(string.Format(CultureInfo.CurrentCulture, "{0,-8} : {1}", pair.Value, pair.Key.Message));
            sb.AppendLine();
        });

        return sb.ToString();
    }

    #endregion
}