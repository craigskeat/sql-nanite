using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace System.Data.Common
{
    public static partial class DbConnectionExtension
    {
        #region CreateCommand

        public static DbCommand CreateCommand(this DbConnection connection, string sql, params object[] parameters)
        {
            var command = connection.CreateCommand();
            command.CommandText = sql;
            command.Parameters.AddRange(parameters);
            return command;
        }

        #endregion

        #region ExecuteNonQuery

        public static int ExecuteNonQuery(this DbConnection connection, string sql, params object[] parameters)
        {
            using (var command = connection.CreateCommand(sql, parameters))
            {
                return command.ExecuteNonQuery();
            }
        }

        public static async Task<int> ExecuteNonQueryAsync(this DbConnection connection, string sql, params object[] parameters)
        {
            using (var command = connection.CreateCommand(sql, parameters))
            {
                return await command.ExecuteNonQueryAsync();
            }
        }

        #endregion

        #region ExecuteScalar

        public static T ExecuteScalar<T>(this DbConnection connection, string sql, params object[] parameters)
        {
            using (var command = connection.CreateCommand(sql, parameters))
            {
                return (T)command.ExecuteScalar();
            }
        }

        public static async Task<T> ExecuteScalarAsync<T>(this DbConnection connection, string sql, params object[] parameters)
        {
            using (var command = connection.CreateCommand(sql, parameters))
            {
                return (T)await command.ExecuteScalarAsync();
            }
        }

        #endregion

        #region ExecuteReader

        internal static List<(int ColumnOrdinal, PropertyInfo SettingProperty)> ReadColumnProperties<T>(this DbDataReader reader) where T : class, new()
        {
            var columnNames = reader.GetSchemaTable().Select().Select(row => $"{row[0]}").Distinct().ToList();

            var columnProperties = typeof(T).GetOrderedProperties().SelectMany(property =>
            {
                var columnName = property.GetCustomAttribute<ColumnAttribute>()?.Name ?? property.Name;

                return columnNames.Contains(columnName)
                    ? new[] { (ColumnOrdinal: reader.GetOrdinal(columnName), SettingProperty: property) }
                    : Array.Empty<(int ColumnOrdinal, PropertyInfo SettingProperty)>();
            }).ToList();

            return columnProperties;
        }

        internal static T ReadDatum<T>(this DbDataReader reader, ref List<(int ColumnOrdinal, PropertyInfo SettingProperty)> columnProperties) where T : class, new()
        {
            var datum = new T();

            foreach (var (ColumnOrdinal, SettingProperty) in columnProperties)
            {
                SettingProperty.SetValue(datum, reader.GetValue(ColumnOrdinal));
            }

            return datum;
        }

        public static IEnumerable<T> ExecuteReaderForData<T>(this DbConnection connection, string sql, params object[] parameters) where T : class, new()
        {
            using (var command = connection.CreateCommand(sql, parameters))
            using (var reader = command.ExecuteReader())
            {
                var columnProperties = reader.ReadColumnProperties<T>();

                while (reader.Read())
                {
                    var datum = reader.ReadDatum<T>(ref columnProperties);
                    yield return datum;
                }
            }

        }

        public static async Task<List<T>> ExecuteReaderForDataAsync<T>(this DbConnection connection, string sql, params object[] parameters) where T : class, new()
        {
            using (var command = connection.CreateCommand(sql, parameters))
            using (var reader = await command.ExecuteReaderAsync())
            {
                var columnProperties = reader.ReadColumnProperties<T>();

                var data = new List<T>();

                while (await reader.ReadAsync())
                {
                    var datum = reader.ReadDatum<T>(ref columnProperties);
                    data.Add(datum);
                }

                return data;
            }
        }

        #endregion
    }
}
