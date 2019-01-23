using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Index_Manager_Web.Models
{
    public class UnusedIndexes
    {
        public string TableName { get; set; }

        public string IndexName { get; set; }

        public string Type { get; set; }

        public int PageCount { get; set; }

        public decimal IndexSize { get; set; }
    }
}