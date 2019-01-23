using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Index_Manager_Web.Models
{
    public class MissingIndexes
    {
        public string DatabaseName { get; set; }
        public string TableName { get; set; }
        public string EqualityColumns { get; set; }
        public string InqualiityColumns { get; set; }
        public string IncludedColumns { get; set; }
        public float UniqueCompiles { get; set; }
        public float IndexAdvantage { get; set; }
        public DateTime LastUserSeek { get; set; }
        public float UserSeeks { get; set; }
        public float AvgTotalUserCost { get; set; }
        public float AvgUserImpact { get; set; }

    }
}