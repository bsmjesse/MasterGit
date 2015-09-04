using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class Configuration_frmvehicle_workinghours : SentinelFMBasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            clsMisc.cboHoursFill(ref cboFromDayH);
            clsMisc.cboHoursFill(ref cboToDayH);
            clsMisc.cboHoursFill(ref cboWeekEndFromH);
            clsMisc.cboHoursFill(ref cboWeekEndToH);
            //System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");
            //System.Threading.Thread.CurrentThread.CurrentUICulture = ci ;
            //this.txtFrom.Text=DateTime.Now.AddDays(-1).ToString("MM/dd/yyyy hh:mm");
            //this.txtTo.Text=DateTime.Now.AddDays(1).ToString("MM/dd/yyyy hh:mm");
        }
    }
    protected void cmdInfo_Click(object sender, EventArgs e)
    {
        Response.Redirect("frmvehicle_add_edit.aspx");
    }
    protected void cmdCustomFields_Click(object sender, EventArgs e)
    {
        Response.Redirect("frmvehicle_customfields.aspx");
    }
    protected void cmdSave_Click(object sender, EventArgs e)
    {

    }
    protected void optFromToWeekDay_CheckedChanged(object sender, EventArgs e)
    {
        if (optFromToWeekDay.Checked)
        {
            SetFromToWeekDay(true);
        }
        else
        {
            SetFromToWeekDay(false);
            optWeekDay24.Checked = true;
        }
    }

    private void SetFromToWeekDay(bool bValue)
    {
        this.cboFromDayH.Enabled = bValue;
        this.cboFromDayM.Enabled = bValue;
        this.cboToDayH.Enabled = bValue;
        this.cboToDayM.Enabled = bValue;
    }


    private void SetFromToWeekEnd(bool bValue)
    {
        this.cboWeekEndFromH.Enabled = bValue;
        this.cboWeekEndFromM.Enabled = bValue;
        this.cboWeekEndToH.Enabled = bValue;
        this.cboWeekEndToM.Enabled = bValue;
    }
    protected void optFromToWeekEnd_CheckedChanged(object sender, EventArgs e)
    {
        SetFromToWeekEnd(optFromToWeekEnd.Checked);
    }
    protected void optWeekDay24_CheckedChanged(object sender, EventArgs e)
    {
        if (optWeekDay24.Checked)
        {
            SetFromToWeekDay(false);
            optFromToWeekDay.Checked = false;
        }
        else
        {
            SetFromToWeekDay(true);
        }

    }
    protected void optWeekEnd24_CheckedChanged(object sender, EventArgs e)
    {
        if (optWeekEnd24.Checked)
        {
            SetFromToWeekEnd(false);
            optFromToWeekEnd.Checked = true;
        }
        else
        {
            optFromToWeekEnd.Checked = true;
        }
    }
}
