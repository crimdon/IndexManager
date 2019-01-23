using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Index_Manager_Web.Helpers;
using Index_Manager_Web.Models;
using System.Data;

namespace Index_Manager_Web.Views.Connected
{
    public class MissingIndexesController : Controller
    {
        public ActionResult MissingIndexes(string databaseName)
        {
                DAL dal = new DAL();
                List<MissingIndexes> mi = new List<MissingIndexes>();
                string connectionString = (string)(Session["connectionstring"]);
                string strSQLCommand = "SELECT DB_NAME(mid.database_id) AS [db_name], SUBSTRING(mid.[statement], " +
                    "CHARINDEX('].[', mid.[statement], 1) + 2, LEN(mid.[statement])) AS[TableName] , " +
                    "mid.equality_columns , mid.inequality_columns , mid.included_columns , migs.unique_compiles , " +
                    "CAST(user_seeks * avg_total_user_cost * (avg_user_impact * 0.01) AS INT) AS[index_advantage] , " +
                    "migs.last_user_seek , migs.user_seeks , CAST(migs.avg_total_user_cost AS INT) AS avg_total_user_cost, migs.avg_user_impact " +
                    "FROM sys.dm_db_missing_index_group_stats AS migs WITH (NOLOCK) " +
                    "INNER JOIN sys.dm_db_missing_index_groups AS mig WITH(NOLOCK) ON migs.group_handle = mig.index_group_handle " +
                    "INNER JOIN sys.dm_db_missing_index_details AS mid WITH(NOLOCK) ON mig.index_handle = mid.index_handle " +
                    "WHERE DB_NAME(mid.database_id) = '" + databaseName + "'" +
                    "ORDER BY index_advantage DESC";
                DataTable dt = dal.fillDataTable(connectionString, strSQLCommand, databaseName);
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    mi.Add(new MissingIndexes
                    {
                        DatabaseName = row[0].ToString(),
                        TableName = row[1].ToString(),
                        EqualityColumns = row[2].ToString(),
                        InqualiityColumns = row[3].ToString(),
                        IncludedColumns = row[4].ToString(),
                        UniqueCompiles = float.Parse(row[5].ToString()),
                        IndexAdvantage = float.Parse(row[6].ToString()),
                        LastUserSeek = DateTime.Parse(row[7].ToString()),
                        UserSeeks = float.Parse(row[8].ToString()),
                        AvgTotalUserCost = float.Parse(row[9].ToString()),
                        AvgUserImpact = float.Parse(row[10].ToString())
                    });
                }
            }
                ViewBag.DatabaseName = databaseName;
                return View(mi);
        }

        public ActionResult CreateIndex(string databaseName, string fullTableName, string equalityColumns, string inequalityColumns, string includedColumns)
        {
            DAL dal = new DAL();
            StringHandler sh = new StringHandler();
            string connectionString = (string)(Session["connectionstring"]);
            string sqlStatement = sh.buildIndexStatement(databaseName, fullTableName, equalityColumns, inequalityColumns, includedColumns);
            dal.runQuery(connectionString, sqlStatement);
            return RedirectToAction("MissingIndexes", "MissingIndexes", new { databaseName = databaseName });
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            if (Session["connectionstring"] == null)
            {
                filterContext.ExceptionHandled = true;
                filterContext.Result = RedirectToAction("Index", "Home");
            }
            else
            {
                filterContext.ExceptionHandled = true;
                TempData["ErrorMessage"] = filterContext.Exception.Message;
                filterContext.Result = RedirectToAction("Error", "Home");
            }
        }
    }
}