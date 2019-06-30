namespace SqlNaniteTest.Core30
{
    using System;
    using System.Data.Common;
    using System.Threading.Tasks;

    using Craig.Data.Extensions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

#if CORE30
    using SqlNaniteTest.Core30.Entities;
#endif

    /// <summary>
    /// The read data test.
    /// </summary>
    [TestClass]
    public class ReadDataTest
    {
        /// <summary>
        /// The initializing.
        /// </summary>
        [TestInitialize]
        public void Initializing()
        {
#if CORE30
            DbProviderFactories.RegisterFactory("System.Data.SqlClient", System.Data.SqlClient.SqlClientFactory.Instance);
            DbProviderFactories.RegisterFactory("MySql.Data.MySqlClient", MySql.Data.MySqlClient.MySqlClientFactory.Instance);
#endif
        }

        /// <summary>
        /// The execute scalar async test.
        /// </summary>
        /// <param name="factory">
        /// The factory.
        /// </param>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [DataTestMethod]
        public async Task ExecuteScalarAsyncTest(string factory, string connectionString)
        {
            using (var con = DbProviderFactories.GetFactory(factory).CreateConnection())
            {
                Assert.IsNotNull(con);

                con.ConnectionString = connectionString;

                con.Open();

                var now = await con.ExecuteScalarAsync<DateTime>("SELECT GETDATE();");

                con.Close();

                Assert.AreEqual(DateTime.Now.Date, now.Date);
            }
        }

#if CORE30

        /// <summary>
        /// The select from async test.
        /// </summary>
        /// <param name="factory">
        /// The factory.
        /// </param>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [DataTestMethod]
        public async Task SelectFromAsyncTest(string factory, string connectionString)
        {
            using (var con = DbProviderFactories.GetFactory(factory).CreateConnection())
            {
                Assert.IsNotNull(con);

                con.ConnectionString = connectionString;

                con.Open();

                var data = await con.SelectFromAsync<RetrieveEntity>();

                con.Close();

                Assert.IsNotNull(data);

                Assert.AreEqual(4, data.Count);
            }
        }

#endif
    }
}
