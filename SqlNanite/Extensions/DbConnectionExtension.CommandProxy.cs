using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Craig.Data.Extensions
{
    public static partial class DbConnectionExtension
    {
        public static int ExecuteNonQuery(this DbConnection connection, string sql, params object[] parameters)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = sql;
                command.Parameters.AddRange(parameters);
                return command.ExecuteNonQuery();
            }
        }

        public static async Task<int> ExecuteNonQueryAsync(this DbConnection connection, string sql, params object[] parameters)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = sql;
                command.Parameters.AddRange(parameters);
                return await command.ExecuteNonQueryAsync();
            }
        }

        public static T ExecuteScalar<T>(this DbConnection connection, string sql, params object[] parameters)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = sql;
                command.Parameters.AddRange(parameters.ToArray());
                return (T)command.ExecuteScalar();
            }
        }

        public static async Task<T> ExecuteScalarAsync<T>(this DbConnection connection, string sql, params object[] parameters)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = sql;
                command.Parameters.AddRange(parameters);
                return (T)await command.ExecuteScalarAsync();
            }
        }
    }
}
