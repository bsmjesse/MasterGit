using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using SentinelFM;
using System.Data;
using System.IO;
using HOS_DBTableAdapters;
using Telerik.Web.UI;

public partial class HOS_frmHOSHistory : SentinelFMBasePage
{
    public string Error_Load = "";
    string SelectVehiclePrompt = "";
    public string resizeScript = "";
    string DateError1 = "";
    string DateError2 = "";
    string DateError3 = "";
    string SelectVehicle = "";
    string SelectDriverPrompt = "";
    string SelectDriver = "";
    string DownLoadsuccess = "Download successfully.";
    public string showFilter = "Show Filter";
    public string hideFilter = "Hide Filter";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (sn.User.LoadVehiclesBasedOn == "hierarchy")
        {
            lblFleet.Visible = false;
            FleetColumn.Visible = false;
            cboFleet.Visible = false;
            FleetVehicle1.Visible = true;
        }
        else
        {
            lblFleet.Visible = true;
            FleetColumn.Visible = true;
            cboFleet.Visible = true;
            FleetVehicle1.Visible = false;
        }
        SelectVehicle = (string)base.GetLocalResourceObject("SelectVehicle");
        DateError1 = (string)base.GetLocalResourceObject("DateError1");
        DateError2 = (string)base.GetLocalResourceObject("DateError2");
        DateError3 = (string)base.GetLocalResourceObject("DateError3");
        SelectVehiclePrompt = (string)base.GetLocalResourceObject("SelectVehiclePrompt");
        SelectDriverPrompt = (string)base.GetLocalResourceObject("SelectDriverPrompt");
        SelectDriver = (string)base.GetLocalResourceObject("SelectDriver");

        if (!Page.IsPostBack)
        {
            CboFleet_Fill();
            txtFrom.SelectedDate = System.DateTime.Now.Date;
            txtTo.SelectedDate = System.DateTime.Now.Date.AddDays(1);

            GetDriversTableAdapter getDrivers = new GetDriversTableAdapter();
            cboDriver.DataSource = getDrivers.GetDrivers(sn.User.OrganizationId);
            //getDrivers.GetDrivers(134);            
            cboDriver.DataBind();
            cboDriver.Items.Insert(0, new ListItem(SelectDriver, "-1"));
        }
        Error_Load = (string)base.GetLocalResourceObject("Error_Load");
        resizeScript = SetResizeScript(dgHistory.ClientID);
    }


    private void CboFleet_Fill()
    {
        try
        {

            DataSet dsFleets = new DataSet();
            dsFleets = sn.User.GetUserFleets(sn);
            cboFleet.DataSource = dsFleets;
            cboFleet.DataBind();
            cboFleet.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboFleet_Item_0"), "-1"));
        }
        catch (NullReferenceException Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
        }
        catch (Exception Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
            ExceptionLogger(trace);

        }
    }
    private void CboVehicle_Fill(int fleetId)
    {
        try
        {
            cboVehicle.Items.Clear();

            DataSet dsVehicle = new DataSet();

            string xml = "";

            using (SentinelFM.ServerDBFleet.DBFleet dbf = new SentinelFM.ServerDBFleet.DBFleet())
            {
                if (objUtil.ErrCheck(dbf.GetVehiclesShortInfoXMLByFleetId(sn.UserID, sn.SecId, fleetId, ref xml), false))
                    if (objUtil.ErrCheck(dbf.GetVehiclesShortInfoXMLByFleetId(sn.UserID, sn.SecId, fleetId, ref xml), true))
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "No vehicles for fleet:" + fleetId.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                        //cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0_EntireFleet"), "0"));
                        return;
                    }
            }

            if (String.IsNullOrEmpty(xml))
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "No vehicles for fleet:" + fleetId.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                //cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0_EntireFleet"), "0"));
                return;
            }

            dsVehicle.ReadXml(new StringReader(xml));

            cboVehicle.DataSource = dsVehicle;
            cboVehicle.DataBind();
            cboVehicle.SelectedIndex = 0;
            cboVehicle.Items.Insert(0, new ListItem(SelectVehicle, "-1"));

            //if ((sn.Report.VehicleId != 0) && (sn.Report.VehicleId != -1))
                //cboVehicle.SelectedIndex = cboVehicle.Items.IndexOf(cboVehicle.Items.FindByValue(sn.Report.VehicleId.ToString()));


        }
        catch (NullReferenceException Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
        }
        catch (Exception Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
            ExceptionLogger(trace);

        }
    }
    protected void cmdViewAllData_Click(object sender, EventArgs e)
    {

        if ((cboVehicle.Items.Count == 0 || cboVehicle.SelectedIndex <= 0) &&
            (cboDriver.Items.Count == 0 || cboDriver.SelectedIndex <= 0))
        {
            lblMessage.Text = SelectVehiclePrompt;
            return;
        }
        DateTime dtFrom = txtFrom.SelectedDate.Value.Date;
        DateTime dtTo = txtTo.SelectedDate.Value.Date;
        if (dtFrom > dtTo)
        {
            lblMessage.Text = DateError1;
            return;
        }
        if (dtFrom.AddDays(30) < dtTo)
        {
            lblMessage.Text = DateError2;
            return;
        }

        lblMessage.Text = "";
        BindData_NewTZ(true);

    }

    DataTable CreateNullTable()
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("BoxId");
        dt.Columns.Add("EventDateTime");
        dt.Columns.Add("MdtDateTime");
        dt.Columns.Add("DriverName");
        dt.Columns.Add("Description");
        dt.Columns.Add("Odometer");
        return dt;

    }

    // Changes for TimeZone Feature start

    void BindData_NewTZ(Boolean isBind)
    {
        try
        {
            if ((cboVehicle.Items.Count > 0 && cboVehicle.SelectedIndex > 0) ||
                (cboDriver.Items.Count > 0 && cboDriver.SelectedIndex > 0))
            {
                int boxid = -1;
                if (cboVehicle.SelectedIndex > 0)
                    boxid = int.Parse(cboVehicle.SelectedValue);
                string driver = string.Empty;

                if (cboDriver.SelectedIndex > 0)
                    driver = cboDriver.SelectedValue.ToString();

                clsHOSManager hosManager = new clsHOSManager();
                float timezone = sn.User.NewFloatTimeZone + sn.User.DayLightSaving;
                DateTime dtFrom = txtFrom.SelectedDate.Value.Date.AddHours(-1 * timezone);
                DateTime dtTo = txtTo.SelectedDate.Value.Date.AddHours(-1 * timezone);
                dtTo = dtTo.AddHours(24);
                DataSet dt = hosManager.GetLogData_Event_NewTZ(boxid, dtFrom, dtTo, timezone, driver);
                dgHistory.DataSource = dt;
            }
            else dgHistory.DataSource = CreateNullTable();
            if (isBind) dgHistory.DataBind();
        }
        catch (Exception Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            RadAjaxManager1.ResponseScripts.Add(string.Format("alert('{0}')", Error_Load));
        }
    }

    // Changes for TimeZone Feature end

    void BindData(Boolean isBind)
    {
        try
        {
            if ((cboVehicle.Items.Count > 0 && cboVehicle.SelectedIndex > 0) || 
                (cboDriver.Items.Count > 0 && cboDriver.SelectedIndex > 0))
            {
                int boxid = -1;
                if (cboVehicle.SelectedIndex > 0)
                    boxid = int.Parse(cboVehicle.SelectedValue);
                string driver = string.Empty;

                if (cboDriver.SelectedIndex > 0)
                    driver = cboDriver.SelectedValue.ToString();
                        
                clsHOSManager hosManager = new clsHOSManager();
                int timezone = sn.User.TimeZone + sn.User.DayLightSaving;
                DateTime dtFrom = txtFrom.SelectedDate.Value.Date.AddHours(-1 * timezone);
                DateTime dtTo = txtTo.SelectedDate.Value.Date.AddHours(-1 * timezone);
                dtTo = dtTo.AddHours(24);
                DataSet dt = hosManager.GetLogData_Event(boxid, dtFrom, dtTo, timezone, driver);
                dgHistory.DataSource = dt;
            }
            else dgHistory.DataSource = CreateNullTable();
            if (isBind) dgHistory.DataBind();
        }
        catch (Exception Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            RadAjaxManager1.ResponseScripts.Add(string.Format("alert('{0}')", Error_Load));
        }
    }
    protected void dgHistory_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        BindData_NewTZ(false);
    }
    protected void cboFleet_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (Convert.ToInt32(cboFleet.SelectedValue) != -1)
        {
            this.cboVehicle.Visible = true;
            this.lblVehicleName.Visible = true;
            CboVehicle_Fill(Convert.ToInt32(cboFleet.SelectedValue));
        }
    }
    protected void cmdFlatFile_Click(object sender, EventArgs e)
    {
        if (cboDriver.Items.Count == 0 || cboDriver.SelectedIndex <= 0)
        {
            lblMessage.Text = SelectDriverPrompt ;
            return;
        }
        DateTime dtFrom = txtFrom.SelectedDate.Value.Date;
        DateTime dtTo = txtTo.SelectedDate.Value.Date;
        if (dtFrom > dtTo)
        {
            lblMessage.Text = DateError1;
            return;
        }
        if (dtFrom.AddDays(14) < dtTo)
        {
            lblMessage.Text = string.Format("{0}", DateError3, 14);
            return;
        }

        lblMessage.Text = "";
       // try
        {
            clsHOSManager clsManager = new clsHOSManager();
            string ds = clsManager.GetDataByDaysForFlatFile(dtFrom, dtTo.AddDays(1), cboDriver.SelectedValue.ToString(), chkFlatIncludeName.Checked, sn);
            string destFileName = "log.txt";
            //destFileName = Server.MapPath(".") + "\\" + destFileName;
            destFileName = Server.UrlDecode(destFileName);
            Response.Clear();
            Response.ClearHeaders();
            Response.ContentType = "text/plain";
            Response.AddHeader("Content-Disposition", "attachment; filename=" + destFileName);
            Response.AddHeader("Content-Length", ds.Length.ToString());
            Response.Write(ds);
            Response.ContentType = "application/octet-stream";
            Response.Flush();
            Response.End();
            //RadAjaxManager1.ResponseScripts.Add(string.Format("", DownLoadsuccess));
            //if (File.Exists(destFileName))
            //{
                //FileInfo fi = new FileInfo(destFileName);
                //Response.Clear();
                //Response.ClearHeaders();
                //Response.Buffer = false;
                //Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(destFileName, System.Text.Encoding.UTF8));
                //Response.AppendHeader("Content-Length", ds);
                //Response.ContentType = "application/octet-stream";
                //Response.Write(ds);
                //Response.Flush();
                //Response.End();
            //}  
        }
        //catch (Exception Ex)
       // {
           // System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            //RadAjaxManager1.ResponseScripts.Add(string.Format("alert('{0}')", Error_Load));
       // }
    }

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        FleetVehicle1.OnOkClick += ActionControl_OnOkClick;
    }

    protected void ActionControl_OnOkClick()
    {
        if (Convert.ToInt32(FleetVehicle1.GetSelectedFleet()) != -1)
        {
            this.cboVehicle.Visible = true;
            this.lblVehicleName.Visible = true;
            CboVehicle_Fill(Convert.ToInt32(FleetVehicle1.GetSelectedFleet()));
        }
    }
    protected void txtFrom_Load(object sender, EventArgs e)
    {
        txtFrom.DateInput.DateFormat = sn.User.DateFormat;
        txtTo.DateInput.DateFormat = sn.User.DateFormat;
    }
}