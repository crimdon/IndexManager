using System.Collections.Generic;
using System.Web.Mvc;
using Index_Manager_Web.Helpers;
using Index_Manager_Web.Models;
using System.Data;

namespace Index_Manager_Web.Controllers
{
    public class UnusedIndexesController : Controller
    {
        public ActionResult UnusedIndexes(string databaseName)
        {
            DAL dal = new DAL();
            List<UnusedIndexes> idx = new List<UnusedIndexes>();
            string connectionString = (string)(Session["connectionstring"]);
            string strSQLCommand = "SELECT DISTINCT '[' + OBJECT_SCHEMA_NAME(S.[object_id]) + '].' + '[' + OBJECT_NAME(S.[object_id]) + ']' AS [TableName] , '[' + I.[name] + ']' AS[IndexName] , type_desc AS[Type], PhysicalStats.page_count AS[PageCount], CONVERT(DECIMAL(18, 2), PhysicalStats.page_count * 8 / 1024.0) AS[IndexSize] "
                    + "FROM sys.dm_db_index_usage_stats AS S "
                    + "INNER JOIN sys.indexes AS I ON I.[object_id] = S.[object_id] AND I.index_id = S.index_id "
                    + "LEFT JOIN sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, NULL) PhysicalStats ON PhysicalStats.object_id = S.object_id AND PhysicalStats.index_id = S.index_id "
                    + "WHERE OBJECTPROPERTY(S.[object_id], 'IsUserTable') = 1 AND type_desc NOT IN ('CLUSTERED', 'HEAP') AND COALESCE(last_user_seek, last_user_scan, last_user_lookup) IS NULL "
                    + "ORDER BY PhysicalStats.page_count DESC;";
            DataTable dt = dal.fillDataTable(connectionString, strSQLCommand, databaseName);
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    idx.Add(new UnusedIndexes
                    {
                        TableName = row[0].ToString(),
                        IndexName = row[1].ToString(),
                        Type = row[2].ToString(),
                        PageCount = int.Parse(row[3].ToString()),
                        IndexSize = decimal.Parse(row[4].ToString())
                    });
                }
            }
            ViewBag.DatabaseName = databaseName;
            return View(idx);
        }

        public ActionResult DeleteIndex(string databaseName, string tableName, string indexName)
        {
            DAL dal = new DAL();
            string connectionString = (string)(Session["connectionstring"]);
            string strSQLCommand = "DROP INDEX " + indexName + " ON " + "[" + databaseName + "]." + tableName;
            dal.runQuery(connectionString, strSQLCommand);
            return RedirectToAction("UnusedIndexes", "UnusedIndexes", new { databaseName = databaseName });
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