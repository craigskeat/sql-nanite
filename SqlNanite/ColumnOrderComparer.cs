using System.Collections.Generic;

/// <summary>
/// The column order comparer.
/// </summary>
internal class ColumnOrderComparer : IComparer<int?>
{
    /// <summary>
    /// The compare.
    /// </summary>
    /// <param name="x">
    /// The x.
    /// </param>
    /// <param name="y">
    /// The y.
    /// </param>
    /// <returns>
    /// The <see cref="int"/>.
    /// </returns>
    public int Compare(int? x, int? y)
    {
        if (x == null)
        {
            return 1;
        }

        if (y == null)
        {
            return -1;
        }

        if (x == y)
        {
            return 0;
        }

        return x > y ? -1 : 1;
    }
}