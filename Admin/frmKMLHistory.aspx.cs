using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class frmKMLHistory : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            ddlFrom.Items.Add(new ListItem("12 AM", "12:00:00 AM"));
            ddlTo.Items.Add(new ListItem("12 AM", "12:00:00 AM"));

            for (int i = 1; i < 12; i++)
            {
                ddlFrom.Items.Add(new ListItem(i.ToString() + " AM", i.ToString() + ":00:00 AM"));
                ddlTo.Items.Add(new ListItem(i.ToString() + " AM", i.ToString() + ":00:00 AM"));
            }

            ddlFrom.Items.Add(new ListItem("12 PM", "12:00:00 PM"));
            ddlTo.Items.Add(new ListItem("12 PM", "12:00:00 PM"));

            for (int i = 1; i < 12; i++)
            {
                ddlFrom.Items.Add(new ListItem(i.ToString() + " PM", i.ToString() + ":00:00 PM"));
                ddlTo.Items.Add(new ListItem(i.ToString() + " PM", i.ToString() + ":00:00 PM"));
            }
        }
    }

    protected void btnGetKML_Click(object sender, EventArgs e)
    {
        short sMsgCount = 10;
        int intBoxID = VLF.CLS.Def.Const.unassignedIntValue;
        DateTime dtFrom, dtTo;
        SentinelFM.SentinelFMSession sn = null;
        SentinelFM.clsUtility objUtil;
        ServerDBHistory.DBHistory dbHistory = new ServerDBHistory.DBHistory();
        string strXML = "";
        StringReader srXML = null;
        DataSet dsHistory = new DataSet();
        FileStream fsKML = null;
        StreamWriter swKML = null;
        bool blnValidDataFound = false;

        lblStatus.ForeColor = System.Drawing.Color.Red;

        try
        {
            sMsgCount = short.Parse(txtMsgCount.Text);
        }

        catch
        {
            txtMsgCount.Text = "10";
        }

        try
        {
            intBoxID = int.Parse(txtBoxID.Text);
        }

        catch
        {
            txtBoxID.Text = "";
        }

        try
        {
            dtFrom = calFrom.SelectedDate.AddTicks(-1).AddTicks(1).AddHours(DateTime.Parse(ddlFrom.SelectedValue).Hour);
            dtTo = calTo.SelectedDate.AddTicks(-1).AddTicks(1).AddHours(DateTime.Parse(ddlTo.SelectedValue).Hour);
        }

        catch
        {
            lblStatus.Text = "Invalid date range.";
            return;
        }

        Response.Redirect("frmKML.aspx?count=" + HttpUtility.HtmlEncode(sMsgCount.ToString())
                          + "&boxid=" + HttpUtility.HtmlEncode(intBoxID.ToString())
                          + "&from=" + HttpUtility.HtmlEncode(dtFrom.ToString())
                          + "&to=" + HttpUtility.HtmlEncode(dtTo.ToString()));
    }
}