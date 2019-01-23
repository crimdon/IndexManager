using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Index_Manager_Web.Models
{
    public interface IDatabasesModel
    {
        string ServerName { get; set; }
        string DatabaseName { get; set; }
    }
}