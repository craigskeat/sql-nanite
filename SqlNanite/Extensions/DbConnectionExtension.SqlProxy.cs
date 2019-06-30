namespace Craig.Data.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;

    /// <summary>
    /// The db connection extension.
    /// </summary>
    public static partial class DbConnectionExtension
    {
        /// <summary>
        /// The select from.
        /// </summary>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <typeparam name="T">
        /// The target <see cref="Type"/>.
        /// </typeparam>
        /// <returns>
        /// The <see cref="List{T}"/>.
        /// </returns>
        public static List<T> SelectFrom<T>(this DbConnection connection) where T : new()
        {
            var nanite = SqlNanite<T>.SelectFrom();

            var liveReader = connection.ExecuteReader(nanite.CommandText);

            var list = new List<T>();

            foreach (var read in liveReader)
            {
                list.Add(nanite.ReadResult(read));
            }

            return list;
        }

#if STD21

        /// <summary>
        /// The select from async.
        /// </summary>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <typeparam name="T">
        /// The target <see cref="Type"/>.
        /// </typeparam>
        /// <returns>
        /// The <see cref="System.Threading.Tasks.Task{T}"/>.
        /// </returns>
        public static async System.Threading.Tasks.Task<List<T>> SelectFromAsync<T>(this DbConnection connection) where T : new()
        {
            var nanite = SqlNanite<T>.SelectFrom();

            var liveReader = connection.ExecuteReaderAsync(nanite.CommandText);

            var list = new List<T>();

            await foreach (var read in liveReader)
            {
                list.Add(nanite.ReadResult(read));
            }

            return list;
        }

#endif
    }
}
