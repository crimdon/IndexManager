using System.Collections.Generic;
using System.Web.Mvc;
using Index_Manager_Web.Models;
using System;

namespace Index_Manager_Web.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            var dropdown = new List<SelectListItem> { new SelectListItem { Text = "Windows", Value = "Windows" }, new SelectListItem { Text = "SQL", Value = "SQL" } };
            ViewBag.dropdown = dropdown;
            return View();
        }

        [HttpPost]
        public ActionResult Index(LoginModel logindetails)
        {
            if (ModelState.IsValid)
            {
                Tuple<bool, string, string> tuple = logindetails.IsValid(logindetails.Servername, logindetails.Authentication, logindetails.Username, logindetails.Password, "master");
                if (tuple.Item1)
                {
                    (Session["connectionstring"]) = tuple.Item2;
                    (Session["servername"]) = tuple.Item3;
                    return RedirectToAction("Databases", "Connected");
                }
                else
                {
                    ModelState.AddModelError("LoginError", "Login to SQL Server Failed!");
                }
            }
            var dropdown = new List<SelectListItem> { new SelectListItem { Text = "Windows", Value = "Windows" }, new SelectListItem { Text = "SQL", Value = "SQL" } };
            ViewBag.dropdown = dropdown;
            return View(logindetails);
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Error()
        {
            return View();
        }
    }
}