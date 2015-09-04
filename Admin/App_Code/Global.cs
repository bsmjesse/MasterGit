using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Contains useful static functions.
/// </summary>
public static class Global
{
    public static void FillTimeDDLs(ref DropDownList ddlFrom, ref DropDownList ddlTo)
    {
        ddlFrom.Items.Add(new ListItem("12 AM", "0:00:00"));
        ddlTo.Items.Add(new ListItem("12 AM", "0:00:00"));

        for (int i = 1; i <= 11; i++)
        {
            ddlFrom.Items.Add(new ListItem(i.ToString() + " AM", i.ToString() + ":00:00"));
            ddlTo.Items.Add(new ListItem(i.ToString() + " AM", i.ToString() + ":00:00"));
        }

        ddlFrom.Items.Add(new ListItem("12 PM", "12:00:00"));
        ddlTo.Items.Add(new ListItem("12 PM", "12:00:00"));

        for (int i = 1; i <= 11; i++)
        {
            ddlFrom.Items.Add(new ListItem(i.ToString() + " PM", ((int)(i + 12)).ToString() + ":00:00"));
            ddlTo.Items.Add(new ListItem(i.ToString() + " PM", ((int)(i + 12)).ToString() + ":00:00"));
        }
    }

    public static string GetExceptions(Exception ex, string separator)
    {
        if (ex.InnerException == null)
            return ex.Message;

        else
            return ex.Message + separator + GetExceptions(ex.InnerException, separator);
    }
}
