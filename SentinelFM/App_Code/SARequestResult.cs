using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// Summary description for SARequestResult
/// </summary>
public class SARequestResult
{
    private static readonly DateTime baseDateTime = new DateTime(1970, 1, 1, 0, 0, 0);
    public bool Result;
    public string Message;
    public Object Object;

    public SARequestResult()
    {
    }
    public static SARequestResult GetFailResult(string message)
    {
        SARequestResult r = new SARequestResult();
        r.Result = false;
        r.Message = message;
        return r;
    }
    public static SARequestResult GetOKResult()
    {
        return GetOKResult(null);
    }

    public static SARequestResult GetOKResult(object obj)
    {
        SARequestResult r = new SARequestResult();
        r.Result = true;
        if (obj == null) return r;
        if (obj is DataTable)
            r.Object = r.ToDictionary(obj as DataTable);
        else if (obj is DataView)
            r.Object = r.ToDictionary(obj as DataView);
        else if (obj is DataSet)
        {
            DataSet ds = obj as DataSet;
            r.Object = r.ToDictionary(ds.Tables[0]);
        }
        else
            r.Object = obj;
        return r;
    }
    public string ToJson()
    {
        System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        return serializer.Serialize(this);
    }

    private List<Dictionary<string, object>> ToDictionary(DataView dv)
    {
        DataTable dt = dv.ToTable();
        return ToDictionary(dt);
    }

    private List<Dictionary<string, object>> ToDictionary(DataTable dt)
    {
        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
        Dictionary<string, object> row = null;
        Type dateType = DateTime.MinValue.GetType();

        foreach (DataRow dr in dt.Rows)
        {
            row = new Dictionary<string, object>();
            foreach (DataColumn col in dt.Columns)
            {
                if (col.DataType == dateType && dr[col] != DBNull.Value)
                {
                    DateTime date = DateTime.Parse(dr[col].ToString());
                    row.Add(col.ColumnName.Trim(), date.ToString("yyyy-MM-ddTHH:mm:ss"));
                }
                else
                    row.Add(col.ColumnName.Trim(), dr[col]);
            }
            rows.Add(row);
        }
        return rows;
    }
}