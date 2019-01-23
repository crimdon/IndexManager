using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Index_Manager_Web.Models
{
    public class DatabasesModel : IDatabasesModel
    {
        public string ServerName { get; set; }
        public string DatabaseName { get; set; }
    }
}