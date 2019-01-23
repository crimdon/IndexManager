using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Web;

namespace Index_Manager_Web.Models
{
    public class LoginModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a SQL Server Name")]
        public string Servername { get; set; }

        [DefaultValue("Windows")]
        public string Authentication { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public Tuple<bool, string, string> IsValid(string serverName, string authenticationType, string userName, string password, string databaseName)
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
                connectionString.InitialCatalog = databaseName;
                connectionString.ApplicationName = "IndexManager";

                using (SqlConnection sqlConn = new SqlConnection())
                {
                    sqlConn.ConnectionString = connectionString.ConnectionString;
                    sqlConn.Open();
                    sqlConn.Close();
                    return Tuple.Create(true, connectionString.ConnectionString, connectionString.DataSource);
                }
            }
            catch
            {
                return Tuple.Create(false, serverName, authenticationType);
            }
        }
    }
}