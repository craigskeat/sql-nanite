namespace Craig.Data.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Reflection;

    using static System.Reflection.BindingFlags;

    /// <summary>
    /// The property info extension.
    /// </summary>
    public static class PropertyInfoExtension
    {
        /// <summary>
        /// The get custom attribute.
        /// </summary>
        /// <param name="t">
        /// The t.
        /// </param>
        /// <param name="inherit">
        /// The inherit.
        /// </param>
        /// <typeparam name="TAttribute">
        /// The target <see cref="Attribute"/>.
        /// </typeparam>
        /// <returns>
        /// The <typeparamref name="TAttribute"/>.
        /// </returns>
        public static TAttribute GetCustomAttribute<TAttribute>(this ICustomAttributeProvider t, bool inherit = true) where TAttribute : Attribute
        {
            return (TAttribute)t.GetCustomAttributes(typeof(TAttribute), inherit).FirstOrDefault();
        }

        /// <summary>
        /// The get custom attributes.
        /// </summary>
        /// <param name="t">
        /// The t.
        /// </param>
        /// <param name="inherit">
        /// The inherit.
        /// </param>
        /// <typeparam name="TAttribute">
        /// The target <see cref="Attribute"/>.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        public static IEnumerable<TAttribute> GetCustomAttributes<TAttribute>(this ICustomAttributeProvider t, bool inherit = true) where TAttribute : Attribute
        {
            return t.GetCustomAttributes(typeof(TAttribute), inherit).OfType<TAttribute>();
        }

        /// <summary>
        /// The get ordered properties.
        /// </summary>
        /// <param name="t">
        /// The t.
        /// </param>
        /// <param name="bindingFlags">
        /// The binding flags.
        /// </param>
        /// <returns>
        /// The <see cref="IOrderedEnumerable{T}"/>.
        /// </returns>
        public static IOrderedEnumerable<PropertyInfo> GetOrderedProperties(this Type t, BindingFlags bindingFlags = Instance | Static | Public)
        {
            return t.GetProperties(bindingFlags).OrderBy(pi => pi.GetCustomAttribute<ColumnAttribute>()?.Order, new ColumnOrderComparer());
        }

        /// <summary>
        /// The invoke method.
        /// </summary>
        /// <param name="t">
        /// The t.
        /// </param>
        /// <param name="methodName">
        /// The method name.
        /// </param>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static object InvokeMethod(this object t, string methodName, params object[] arguments)
        {
            return t.GetType().InvokeMember(methodName, BindingFlags.InvokeMethod, null, t, arguments);
        }

        /// <summary>
        /// The get name.
        /// </summary>
        /// <param name="t">
        /// The t.
        /// </param>
        /// <param name="picker">
        /// The picker.
        /// </param>
        /// <param name="inherit">
        /// The inherit.
        /// </param>
        /// <typeparam name="T">
        /// The target <see cref="Type"/>.
        /// </typeparam>
        /// <typeparam name="TAttribute">
        /// The target <see cref="Attribute"/>.
        /// </typeparam>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetName<T, TAttribute>(this T t, Func<TAttribute, string> picker, bool inherit = true)
            where T : MemberInfo
            where TAttribute : Attribute
        {
            var attribute = t.GetCustomAttribute<TAttribute>(inherit);

            var columnName = picker?.Invoke(attribute);

            if (string.IsNullOrEmpty(columnName))
            {
                columnName = t.Name;
            }

            return columnName;
        }
    }
}
