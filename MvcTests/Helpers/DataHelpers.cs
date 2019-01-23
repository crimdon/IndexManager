using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace MvcTests.Helpers
{
    public static class DataHelpers
    {
        public static string getConnectionString(string serverName, string authenticationType, string userName, string password)
        {
            try
            {
                SqlConnectionStringBuilder connectionString = new SqlConnectionStringBuilder();
                connectionString.DataSource = serverName;
                switch (authenticationType)
                {
                    case "Windows":
                        connectionString.IntegratedSecurity = true;
                        break;
                    case "SQL":
                        connectionString.UserID = userName;
                        connectionString.Password = password;
                        break;
                }
                connectionString.ApplicationName = "IndexManager";
                connectionString.InitialCatalog = "master";

                using (SqlConnection sqlConn = new SqlConnection())
                {
                    sqlConn.ConnectionString = connectionString.ConnectionString;
                    sqlConn.Open();
                    sqlConn.Close();
                    return connectionString.ConnectionString;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
