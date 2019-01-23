using System.Collections.Generic;
using System.Web.Mvc;
using Index_Manager_Web.Helpers;
using Index_Manager_Web.Models;
using System.Data;

namespace Index_Manager_Web.Controllers
{
    public class OverLappingIndexesController : Controller
    {
        public ActionResult OverLappingIndexes(string databaseName)
        {
            DAL dal = new DAL();
            List<OverLappingIndexes> idx = new List<OverLappingIndexes>();
            string connectionString = (string)(Session["connectionstring"]);
            string strSQLCommand = ";WITH CTE_INDEX_DATA "
                        + "AS(SELECT   SCHEMA_DATA.name AS schema_name, "
                        + "TABLE_DATA.name AS table_name, "
                        + "INDEX_DATA.name AS index_name, "
                        + "STUFF((SELECT  ', ' + COLUMN_DATA_KEY_COLS.name + ' ' "
                                        + "+ CASE WHEN INDEX_COLUMN_DATA_KEY_COLS.is_descending_key = 1 "
                                               + "THEN 'DESC' "
                                               + "ELSE 'ASC' "
                                          + "END "
                                + "FROM    sys.tables AS T "
                                        + "INNER JOIN sys.indexes INDEX_DATA_KEY_COLS ON T.object_id = INDEX_DATA_KEY_COLS.object_id "
                                        + "INNER JOIN sys.index_columns INDEX_COLUMN_DATA_KEY_COLS ON INDEX_DATA_KEY_COLS.object_id = INDEX_COLUMN_DATA_KEY_COLS.object_id "
                                                              + "AND INDEX_DATA_KEY_COLS.index_id = INDEX_COLUMN_DATA_KEY_COLS.index_id " 
                                        + "INNER JOIN sys.columns COLUMN_DATA_KEY_COLS ON T.object_id = COLUMN_DATA_KEY_COLS.object_id " 
                                                              + "AND INDEX_COLUMN_DATA_KEY_COLS.column_id = COLUMN_DATA_KEY_COLS.column_id "
                                + "WHERE   INDEX_DATA.object_id = INDEX_DATA_KEY_COLS.object_id "
                                        + "AND INDEX_DATA.index_id = INDEX_DATA_KEY_COLS.index_id "
                                        + "AND INDEX_COLUMN_DATA_KEY_COLS.is_included_column = 0 "
                                + "ORDER BY INDEX_COLUMN_DATA_KEY_COLS.key_ordinal "
                              + "FOR  XML PATH('') "
                              + "), 1, 2, '') AS key_column_list, "
                        + "STUFF((SELECT  ', ' + COLUMN_DATA_INC_COLS.name "
                                + "FROM    sys.tables AS T "
                                        + "INNER JOIN sys.indexes INDEX_DATA_INC_COLS ON T.object_id = INDEX_DATA_INC_COLS.object_id "
                                        + "INNER JOIN sys.index_columns INDEX_COLUMN_DATA_INC_COLS ON INDEX_DATA_INC_COLS.object_id = INDEX_COLUMN_DATA_INC_COLS.object_id "
                                                              + "AND INDEX_DATA_INC_COLS.index_id = INDEX_COLUMN_DATA_INC_COLS.index_id "
                                        + "INNER JOIN sys.columns COLUMN_DATA_INC_COLS ON T.object_id = COLUMN_DATA_INC_COLS.object_id "
                                                              + "AND INDEX_COLUMN_DATA_INC_COLS.column_id = COLUMN_DATA_INC_COLS.column_id "
                                + "WHERE   INDEX_DATA.object_id = INDEX_DATA_INC_COLS.object_id "
                                        + "AND INDEX_DATA.index_id = INDEX_DATA_INC_COLS.index_id "
                                        + "AND INDEX_COLUMN_DATA_INC_COLS.is_included_column = 1 "
                                + "ORDER BY INDEX_COLUMN_DATA_INC_COLS.key_ordinal "
                              + "FOR XML PATH('') "
                              + "), 1, 2, '') AS include_column_list, "
                        + "INDEX_DATA.is_disabled "
                + "FROM sys.indexes INDEX_DATA "
                        + "INNER JOIN sys.tables TABLE_DATA ON TABLE_DATA.object_id = INDEX_DATA.object_id "
                        + "INNER JOIN sys.schemas SCHEMA_DATA ON SCHEMA_DATA.schema_id = TABLE_DATA.schema_id "
               + "WHERE TABLE_DATA.is_ms_shipped = 0 "
                        + "AND INDEX_DATA.type_desc IN ('NONCLUSTERED', 'CLUSTERED' )) "
            + "SELECT * "
            + "FROM    CTE_INDEX_DATA DUPE1 "
            + "WHERE EXISTS (SELECT * "
                     + "FROM   CTE_INDEX_DATA DUPE2 "
                     + "WHERE  DUPE1.schema_name = DUPE2.schema_name "
                            + "AND DUPE1.table_name = DUPE2.table_name "
                            + "AND(DUPE1.key_column_list LIKE LEFT(DUPE2.key_column_list, "
                                                              + "LEN(DUPE1.key_column_list)) "
                                  + "OR DUPE2.key_column_list LIKE LEFT(DUPE1.key_column_list, "
                                                              + "LEN(DUPE2.key_column_list))) "
                            + "AND DUPE1.index_name <> DUPE2.index_name ); ";
            DataTable dt = dal.fillDataTable(connectionString, strSQLCommand, databaseName);
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    idx.Add(new OverLappingIndexes
                    {
                        SchemaName = row[0].ToString(),
                        TableName = row[1].ToString(),
                        IndexName = row[2].ToString(),
                        KeyColumnList = row[3].ToString(),
                        IncludeColumnList = row[4].ToString(),
                        IsDisabled = bool.Parse(row[5].ToString())
                    });
                }
            }
            ViewBag.DatabaseName = databaseName;
            return View(idx);
        }

        public ActionResult DeleteIndex(string databaseName, string schemaName, string tableName, string indexName)
        {
            DAL dal = new DAL();
            string connectionString = (string)(Session["connectionstring"]);
            string strSQLCommand = "DROP INDEX " + indexName + " ON " + "[" + databaseName + "].[" + schemaName + "].[" +  tableName + "]";
            dal.runQuery(connectionString, strSQLCommand);
            return RedirectToAction("OverLappingIndexes", "OverLappingIndexes", new { databaseName = databaseName });
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