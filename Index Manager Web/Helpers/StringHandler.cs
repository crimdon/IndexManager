using System;
using System.Text;

namespace Index_Manager_Web.Helpers
{
    public class StringHandler
    {
        public string buildIndexStatement(string database, string fullTableName, string equalitycolumns, string inequalitycolumns, string includedcolumns)
        {
            string indexStatement;
            StringBuilder sbIndexStatement = new StringBuilder();
            sbIndexStatement.Append("CREATE NONCLUSTERED INDEX ");
            sbIndexStatement.Append(buildIndexName(database, fullTableName, equalitycolumns, inequalitycolumns, includedcolumns));
            sbIndexStatement.Append(" ON [" + database + "]." + fullTableName);
            if (equalitycolumns != null)
            {
                sbIndexStatement.Append("(" + equalitycolumns);
                if (String.IsNullOrEmpty(inequalitycolumns) != true)
                {
                    sbIndexStatement.Append("," + inequalitycolumns + ")");
                }
                else
                {
                    sbIndexStatement.Append(")");
                }
            }
            else
            {
                sbIndexStatement.Append("(" + inequalitycolumns + ")");
            }
            if (String.IsNullOrEmpty(includedcolumns) != true)
            {
                sbIndexStatement.Append(" INCLUDE (" + includedcolumns + ")");
            }
            indexStatement = sbIndexStatement.ToString();
            return indexStatement;
        }

        private string getTableName(string fullTableName)
        {
            // Gets table name from string "[dbo].[tablename]"
            var pos = fullTableName.IndexOf("].[");
            var tableName = fullTableName.Substring(pos + 3, fullTableName.Length - pos - 4);
            return tableName;
        }

        private string buildIndexName(string database, string fullTableName, string equalitycolumns, string inequalitycolumns, string includedcolumns)
        {
            string indexName;
            StringBuilder sbIndexName = new StringBuilder();
            sbIndexName.Append("[IX_");
            //Get the table name
            string tableName = getTableName(fullTableName);
            sbIndexName.Append(tableName);
            if (String.IsNullOrEmpty(equalitycolumns) != true)
            {
                sbIndexName.Append("_" + equalitycolumns.Replace(",", "_").Replace("[", "").Replace("]", ""));
            }
            if (String.IsNullOrEmpty(inequalitycolumns) != true)
            {
                sbIndexName.Append("_" + inequalitycolumns.Replace(",", "_").Replace("[", "").Replace("]", ""));
            }
            if (String.IsNullOrEmpty(includedcolumns) != true)
            {
                sbIndexName.Append("_Includes");
            }
            sbIndexName.Append("]");
            indexName = sbIndexName.ToString();
            return indexName;
        }
    }
}