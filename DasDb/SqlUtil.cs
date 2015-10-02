using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace VLF.DAS.DB
{
   public class SqlUtil
   {
      public static string PairValue(string columnName, object value)
      {
         Type type = value.GetType();
         if (type == typeof(sbyte) || type == typeof(char) || type == typeof(uint) || type == typeof(ulong) || type == typeof(ushort) ||
             type == typeof(float) || type == typeof(double) || type == typeof(Double) ||
             type == typeof(UInt16) || type == typeof(UInt32) || type == typeof(UInt64) ||
             type == typeof(Int16) || type == typeof(Int32) || type == typeof(Int64))
            return string.Format(" {0}={1} ", columnName, value.ToString());
         else
            if (type == typeof(string))
               return string.Format(" {0}='{1}' ", columnName, value.ToString().Replace("'", "''"));
            else
               if (type == typeof(DateTime))
                  return string.Format(" {0}='{1}' ", columnName, ((DateTime)value).ToString("MM/dd/yyyy HH:mm:ss.fff"));

         return "";
      }

      /** \fn        public static string PairSQLValue(string ColumnName, object value)
       *  \brief     returns the string formatted for the type of the data you have
       *             the only difference is the type of the object is in fact an SQL type
       *  \comment   what is missing:
       *             - Variant
       *             - VarBinary
       *             - Timestamp
       *             - SmallDateTime
       *             - Image
       *             - Binary
       */
      public static string PairSQLValue(System.Data.SqlClient.SqlParameter param)
      {
         string columnName = param.SourceColumn;
         SqlDbType type = param.SqlDbType;
         if (type == SqlDbType.Int || type == SqlDbType.Decimal ||
             type == SqlDbType.BigInt || type == SqlDbType.Bit ||
             type == SqlDbType.Float || type == SqlDbType.Money ||
             type == SqlDbType.Real || type == SqlDbType.SmallInt ||
             type == SqlDbType.SmallMoney || type == SqlDbType.TinyInt)
            return string.Format(" {0}={1} ", columnName, param.Value.ToString());
         else
            if (type == SqlDbType.Char || type == SqlDbType.VarChar ||
                type == SqlDbType.Text || type == SqlDbType.NVarChar ||
                type == SqlDbType.NText || type == SqlDbType.NChar ||
                type == SqlDbType.Char || type == SqlDbType.Xml)
               return string.Format(" {0}='{1}' ", columnName, param.Value.ToString().Replace("'", "''"));
            else
               if (type == SqlDbType.DateTime || type == SqlDbType.SmallDateTime)
                  return string.Format(" {0}='{1}' ", columnName, ((DateTime)param.Value).ToString("MM/dd/yyyy HH:mm:ss.fff"));

         return "";
      }

      public static string PairValueAnd(string columnName1, object value1,
                                         string columnName2, object value2)
      {
         if (false == string.IsNullOrEmpty(columnName1) &&
             false == string.IsNullOrEmpty(columnName2))
         {
            return string.Format(" {0} AND {1}", PairValue(columnName1, value1),
                                                       PairValue(columnName2, value2));
         }
         return "";
      }

      public static string PairValueAnd(System.Data.SqlClient.SqlParameter[] parameters)
      {
         if (null != parameters)
         {
            StringBuilder ret = new StringBuilder(" ");
            foreach (System.Data.SqlClient.SqlParameter param in parameters)
               ret.Append(PairSQLValue(param)).Append(" AND ");

            // discard the last AND
            int lastAnd = ret.ToString().LastIndexOf("AND");
            return " WHERE " + ret.ToString().Remove(lastAnd);
         }
         return "";

      }

      public static string WhereOr(System.Data.SqlClient.SqlParameter[] parameters)
      {
         if (null != parameters)
         {
            StringBuilder ret = new StringBuilder(" ");
            foreach (System.Data.SqlClient.SqlParameter param in parameters)
               ret.Append(param.ParameterName).Append("=").Append(param.SqlValue.ToString()).Append(" OR ");

            // discard the last AND
            int lastAnd = ret.ToString().LastIndexOf("OR");
            return " WHERE " + ret.ToString().Remove(lastAnd);
         }
         return "";
      }

      /**
       *    No you cannot create a dataview with less columns than its underlying datatable.
       *    A dataview is only a view on a dataset. Nothing more. It has a rowfilter
       *    however not a columnfilter.
       */
      public static DataTable NewDataTable(DataTable table, string[] columnsName, string selection, string sortOrder)
      {
         if (false == string.IsNullOrEmpty(selection))
         {
            //Clone the old DataTable structure to the new one
            DataTable dt = table.Clone();

            DataRow[] dr = null;
            //Select a specific rows from the old DataTable
            if (string.IsNullOrEmpty(sortOrder))
               dr = table.Select(selection);
            else
               dr = table.Select(selection, sortOrder);  // "EmployeeID > 5"

            //loop on the selected rows and import it to the new datatable using the ImportRow method
            foreach (DataRow drEmployee in (dr))
               dt.ImportRow(drEmployee);

            if (null != columnsName && columnsName.Length > 0)
            {
               // remove columns not present in columnsName
               foreach (DataColumn dc in dt.Columns)
                  foreach (string column2Keep in columnsName)
                     if (!dc.ColumnName.ToUpper().Equals(column2Keep.ToUpper()))
                        dt.Columns.Remove(dc.ColumnName);
            }

            return dt;
         }
         else
            return table.Copy();

      }

   }
}
