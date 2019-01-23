using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Index_Manager_Web.Helpers;
using Index_Manager_Web.Models;
using System.Data;

namespace Index_Manager_Web.Controllers
{
    public class ConnectedController : Controller
    {
        public ActionResult Databases()
        {
            DAL dal = new DAL();
            var db = new List<IDatabasesModel>();
            string connectionString = (string)(Session["connectionstring"]);
            string strSQLCommand = "SELECT [name] FROM master.dbo.sysdatabases WHERE dbid > 4 and [name] <> 'ReportServer' and [name] <> 'ReportServerTempDB' ORDER BY [name]";
            DataTable dt = dal.fillDataTable(connectionString, strSQLCommand);
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    db.Add(new DatabasesModel
                    {
                        ServerName = (string)(Session["servername"]),
                        DatabaseName = row[0].ToString()
                    });
                }
            }
            ViewBag.Databases = db;
            return View();
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