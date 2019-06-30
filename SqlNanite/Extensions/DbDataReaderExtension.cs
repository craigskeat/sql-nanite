using System;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// The db data reader extension.
/// </summary>
public static class DbDataReaderExtension
{
    /// <summary>
    /// The get field value async.
    /// </summary>
    /// <param name="reader">
    /// The reader.
    /// </param>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token.
    /// </param>
    /// <typeparam name="T">
    /// The target <see cref="Type"/>.
    /// </typeparam>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public static async Task<T> GetFieldValueAsync<T>(this DbDataReader reader, string name, CancellationToken? cancellationToken = null)
    {
        try
        {
            int ordinal = reader.GetOrdinal(name);
            T value = await reader.GetFieldValueAsync<T>(ordinal, cancellationToken ?? CancellationToken.None);
            return value;
        }
        catch (SqlNullValueException)
        {
            return default;
        }
    }
}