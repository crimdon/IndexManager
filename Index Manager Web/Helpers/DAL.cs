using System;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace Index_Manager_Web.Helpers
{
    public class DAL
    {
        public DataTable fillDataTable(string conSTR, string query, string database = null)
        {
            try
            {
                using (SqlConnection sqlConn = new SqlConnection(conSTR))
                using (SqlCommand cmd = new SqlCommand(query, sqlConn))
                {
                    sqlConn.Open();
                    if (String.IsNullOrEmpty(database) != true)
                    {
                        sqlConn.ChangeDatabase(database);
                    }
                    cmd.CommandTimeout = 120;
                    DataTable dt = new DataTable();
                    dt.Load(cmd.ExecuteReader());
                    sqlConn.Close();
                    return dt;
                }
            }
            catch (Exception ex)
            {
                ArgumentException argEx = new ArgumentException(ex.Message, ex);
                throw argEx;
            }
        }
        public void runQuery(string conSTR, string query, string database = null)
        {
            try
            {
                using (SqlConnection sqlConn = new SqlConnection(conSTR))
                using (SqlCommand cmd = new SqlCommand(query, sqlConn))
                {
                    sqlConn.Open();
                    if (String.IsNullOrEmpty(database) != true)
                    {
                        sqlConn.ChangeDatabase(database);
                    }
                    cmd.CommandText = query;
                    cmd.Connection = sqlConn;
                    cmd.CommandTimeout = 120;
                    cmd.ExecuteNonQuery();
                    sqlConn.Close();
                }
            }
            catch (Exception ex)
            {
                ArgumentException argEx = new ArgumentException(ex.Message, ex);
                throw argEx;
            }
        }
    }
}