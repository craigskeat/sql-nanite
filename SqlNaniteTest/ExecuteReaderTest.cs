using Microsoft.VisualStudio.TestTools.UnitTesting;

using SqlNaniteTest.Entities;

using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

using MicrosoftSqlClientFactory = Microsoft.Data.SqlClient.SqlClientFactory;
using SystemSqlClientFactory = System.Data.SqlClient.SqlClientFactory;

namespace SqlNaniteTest
{
    [TestClass]
    public class ExecuteReaderTest
    {
        static string connectionString = $@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=sql-nanite;Integrated Security=True;Connect Timeout=30;Encrypt=False;";
        static string sql = "SELECT NO FROM TEST1 WHERE NO = @p1";

        [TestMethod]
        public async Task TestMethod0()
        {
            using (var connection = MicrosoftSqlClientFactory.Instance.CreateConnection(connectionString))
            {
                var parameter = MicrosoftSqlClientFactory.Instance.CreateParameter();
                parameter.ParameterName = "@p1";
                parameter.Value = 1;

                var data = await connection.ExecuteReaderForDataAsync<RetrieveEntity>(sql, parameter);
                Assert.AreEqual(1, data.FirstOrDefault()?.Index);
            }
        }
    }
}
