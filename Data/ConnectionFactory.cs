using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperHelper.Data
{
    public class ConnectionFactory
    {
        /// <summary>
        /// Dapper için sql ile bağlantı açar.
        /// </summary>
        /// <returns>Sql ile açık bağlantı döner. using() ile kullanmazsanız "Dispose" edin</returns>
        public static DbConnection GetOpenConnection(string connectionString)
        {
            var connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;
        }
    }
}
