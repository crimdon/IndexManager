using System.Collections.Generic;
using System.Web.Mvc;
using Index_Manager_Web.Helpers;
using Index_Manager_Web.Models;
using System.Data;

namespace Index_Manager_Web.Controllers
{
    public class FragmentedIndexesController : Controller
    {
        public ActionResult FragmentedIndexes(string databaseName)
        {
            DAL dal = new DAL();
            List<FragmentedIndexes> pi = new List<FragmentedIndexes>();
            string connectionString = (string)(Session["connectionstring"]);
            string strSQLCommand = "SELECT '[' + dbschemas.[name] + '].[' + dbtables.[name] + ']' as 'TableName', "
                    + "dbindexes.[name] as 'IndexName', CONVERT(DECIMAL(5, 2), indexstats.avg_fragmentation_in_percent) AS avg_fragmentation_in_percent, "
                    + "indexstats.page_count, CASE WHEN indexstats.avg_fragmentation_in_percent > 30 THEN 'Rebuild' "
                    + "WHEN indexstats.avg_fragmentation_in_percent BETWEEN 15 AND 29 THEN 'Defrag' ELSE '' END AS[Action] "
                    + "FROM sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, NULL) AS indexstats "
                    + "INNER JOIN sys.tables dbtables on dbtables.[object_id] = indexstats.[object_id] "
                    + "INNER JOIN sys.schemas dbschemas on dbtables.[schema_id] = dbschemas.[schema_id] "
                    + "INNER JOIN sys.indexes AS dbindexes ON dbindexes.[object_id] = indexstats.[object_id] AND indexstats.index_id = dbindexes.index_id "
                    + "WHERE indexstats.database_id = DB_ID() AND dbtables.is_ms_shipped = 0 AND dbindexes.[name] IS NOT NULL "
                    + "AND CASE WHEN indexstats.avg_fragmentation_in_percent > 30 THEN 'Rebuild' "
                    + "WHEN indexstats.avg_fragmentation_in_percent BETWEEN 15 AND 29 THEN 'Defrag' ELSE NULL END IS NOT NULL "
                    + "ORDER BY dbschemas.name, dbtables.name, dbindexes.name";
            DataTable dt = dal.fillDataTable(connectionString, strSQLCommand, databaseName);
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    pi.Add(new FragmentedIndexes
                    {
                        TableName = row[0].ToString(),
                        IndexName = row[1].ToString(),
                        AvgFragmentation = decimal.Parse(row[2].ToString()),
                        PageCount = int.Parse(row[3].ToString()),
                        Action = row[4].ToString()
                    });
                }
            }
            ViewBag.DatabaseName = databaseName;
            return View(pi);
        }
        
        public ActionResult Rebuild(string databaseName, string tableName, string indexName)
        { 
            DAL dal = new DAL();
            string connectionString = (string)(Session["connectionstring"]);
            string strSQLCommand = "ALTER INDEX [" + indexName + "] ON " + "[" + databaseName + "]." + tableName + " REBUILD";
            dal.runQuery(connectionString, strSQLCommand);
            return RedirectToAction("FragmentedIndexes", "FragmentedIndexes", new { databaseName = databaseName });
        }

        public ActionResult Defrag(string databaseName, string tableName, string indexName)
        {
            DAL dal = new DAL();
            string connectionString = (string)(Session["connectionstring"]);
            string strSQLCommand = "ALTER INDEX [" + indexName + "] ON " + "[" + databaseName + "]." + tableName + " REORGANIZE";
            dal.runQuery(connectionString, strSQLCommand);
            return RedirectToAction("FragmentedIndexes", "FragmentedIndexes", new { databaseName = databaseName });
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