namespace Craig.Data.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The db connection extension.
    /// </summary>
    public static partial class DbConnectionExtension
    {
        /// <summary>
        /// The execute non query.
        /// </summary>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <param name="commandText">
        /// The command text.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int ExecuteNonQuery(this DbConnection connection, string commandText)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = commandText;
                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// The execute non query async.
        /// </summary>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <param name="commandText">
        /// The command text.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static async Task<int> ExecuteNonQueryAsync(this DbConnection connection, string commandText, CancellationToken? cancellationToken = null)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = commandText;
                return await command.ExecuteNonQueryAsync(cancellationToken ?? CancellationToken.None);
            }
        }

        /// <summary>
        /// The execute scalar.
        /// </summary>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <param name="commandText">
        /// The command text.
        /// </param>
        /// <typeparam name="T">
        /// The target <see cref="Type"/>.
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public static T ExecuteScalar<T>(this DbConnection connection, string commandText)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = commandText;
                return (T)command.ExecuteScalar();
            }
        }

        /// <summary>
        /// The execute scalar async.
        /// </summary>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <param name="commandText">
        /// The command text.
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
        public static async Task<T> ExecuteScalarAsync<T>(this DbConnection connection, string commandText, CancellationToken? cancellationToken = null)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = commandText;
                return (T)await command.ExecuteScalarAsync(cancellationToken ?? CancellationToken.None);
            }
        }

        /// <summary>
        /// The execute reader.
        /// </summary>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <param name="commandText">
        /// The command text.
        /// </param>
        /// <param name="commandBehavior">
        /// The command behavior.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        public static IEnumerable<DbDataReader> ExecuteReader(this DbConnection connection, string commandText, CommandBehavior commandBehavior = CommandBehavior.Default)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = commandText;

                var reader = command.ExecuteReader(commandBehavior);

                while (reader.Read())
                {
                    yield return reader;
                }
            }
        }

        #if STD21

        /// <summary>
        /// The execute reader async.
        /// </summary>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <param name="commandText">
        /// The command text.
        /// </param>
        /// <param name="commandBehavior">
        /// The command behavior.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token.
        /// </param>
        /// <returns>
        /// The <see cref="IAsyncEnumerable{T}"/>.
        /// </returns>
        public static async IAsyncEnumerable<DbDataReader> ExecuteReaderAsync(this DbConnection connection, string commandText, CommandBehavior commandBehavior = CommandBehavior.Default, CancellationToken? cancellationToken = null)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = commandText;

                var reader = await command.ExecuteReaderAsync(commandBehavior, cancellationToken ?? CancellationToken.None);

                while (await reader.ReadAsync(cancellationToken ?? CancellationToken.None))
                {
                    yield return reader;
                }
            }
        }

        #endif
    }
}
