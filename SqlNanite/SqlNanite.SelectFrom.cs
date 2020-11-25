namespace Craig.Data
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Common;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// The SQL Nanite.
    /// </summary>
    /// <typeparam name="T">
    /// The target <see cref="Type"/>.
    /// </typeparam>
    public class SqlNanite<T> where T : new()
    {
        /// <summary>
        /// The select from.
        /// </summary>
        /// <returns>
        /// The <see cref="ValueTuple{T}"/>.
        /// </returns>
        internal static (string CommandText, Func<DbDataReader, T> ReadResult) SelectFrom()
        {
            return (GetSelectFromCommandText(), GetSelectFromResultReader);
        }

        /// <summary>
        /// The get select from command text.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetSelectFromCommandText()
        {
            return $@"SELECT {typeof(T).GetOrderedProperties().Select(pi => pi.GetName<PropertyInfo, ColumnAttribute>(col => col?.Name)).Join($",{Environment.NewLine}       ")}{Environment.NewLine}  FROM {typeof(T).GetName<Type, TableAttribute>(attr => attr?.Name)}";
        }

        /// <summary>
        /// The get select from result reader.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        private static T GetSelectFromResultReader(DbDataReader reader)
        {
            var item = new T();

            foreach (var orderedProperty in typeof(T).GetOrderedProperties())
            {
                var type = orderedProperty.PropertyType;
                var name = orderedProperty.GetName<PropertyInfo, ColumnAttribute>(col => col?.Name);

                var valueTask = typeof(DbDataReaderExtension).GetMethod("GetFieldValueAsync")
                    ?.MakeGenericMethod(type).Invoke(reader, new object[] { reader, name, null });

                var value = valueTask?.InvokeMethod("GetAwaiter")?.InvokeMethod("GetResult");

                orderedProperty.SetValue(item, value);
            }

            return item;
        }
    }
}
