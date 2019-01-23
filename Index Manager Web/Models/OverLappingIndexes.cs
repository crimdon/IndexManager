using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Index_Manager_Web.Models
{
    public class OverLappingIndexes
    {
        public string SchemaName { get; set; }

        public string TableName { get; set; }

        public string IndexName { get; set; }

        public string KeyColumnList { get; set; }

        public string IncludeColumnList { get; set; }

        public bool IsDisabled { get; set; }
    }
}