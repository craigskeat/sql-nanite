using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Data.Common;
using System.Threading.Tasks;

using MicrosoftSqlClientFactory = Microsoft.Data.SqlClient.SqlClientFactory;
using SystemSqlClientFactory = System.Data.SqlClient.SqlClientFactory;

namespace SqlNaniteTest
{
    [TestClass]
    public class ExecuteScalarTest
    {
        static string connectionString = $@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=sql-nanite;Integrated Security=True;Connect Timeout=30;Encrypt=False;";
        static string sql = "SELECT NO FROM TEST1 WHERE NO = @p1";

        [TestMethod]
        public async Task TestMethod0()
        {
            using (var connection = MicrosoftSqlClientFactory.Instance.CreateConnection(connectionString))
            {
                var dt = await connection.ExecuteScalarAsync<DateTime>("SELECT GETDATE();");
                Assert.AreEqual(DateTime.Now.Date, dt.Date);
            }
        }

        [TestMethod]
        public void TestMethod1()
        {
            using (var connection = MicrosoftSqlClientFactory.Instance.CreateConnection(connectionString))
            {
                var parameter = MicrosoftSqlClientFactory.Instance.CreateParameter();
                parameter.ParameterName = "@p1";
                parameter.Value = 1;

                var no = connection.ExecuteScalar<int>(sql, parameter);

                Assert.AreEqual(1, no);
            }
        }

        [TestMethod]
        public void TestMethod2()
        {
            using (var connection = SystemSqlClientFactory.Instance.CreateConnection(connectionString))
            {
                var parameter = SystemSqlClientFactory.Instance.CreateParameter();
                parameter.ParameterName = "@p1";
                parameter.Value = 2;

                var no = connection.ExecuteScalar<int>(sql, parameter);

                Assert.AreEqual(2, no);
            }
        }
    }
}
