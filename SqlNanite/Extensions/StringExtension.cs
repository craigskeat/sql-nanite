using System;
using System.Collections.Generic;

/// <summary>
/// The string extension.
/// </summary>
public static class StringExtension
{
    /// <summary>
    /// The join.
    /// </summary>
    /// <param name="values">
    /// The values.
    /// </param>
    /// <param name="separator">
    /// The separator.
    /// </param>
    /// <typeparam name="T">
    /// The target <see cref="Type"/>.
    /// </typeparam>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public static string Join<T>(this IEnumerable<T> values, string separator)
    {
        return string.Join(separator, values);
    }
}