namespace System.Configuration
{
    using System.Data.Common;

    /// <summary>
    /// The connection string settings extension.
    /// </summary>
    public static class ConnectionStringSettingsExtension
    {
        /// <summary>
        /// The create connection.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <returns>
        /// The <see cref="DbConnection"/>.
        /// </returns>
        public static DbConnection CreateConnection(this ConnectionStringSettings settings)
        {
            var factory = DbProviderFactories.GetFactory(settings.ProviderName);
            var connection = factory.CreateConnection();

            if (connection != null)
            {
                connection.ConnectionString = settings.ConnectionString;
            }

            return connection;
        }
    }
}
