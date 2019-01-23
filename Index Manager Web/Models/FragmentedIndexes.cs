using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Index_Manager_Web.Models
{
    public class FragmentedIndexes
    {
        public string TableName { get; set; }
        public string IndexName { get; set; }
        public decimal AvgFragmentation { get; set; }
        public int PageCount { get; set; }
        public string Action { get; set; }
    }
}