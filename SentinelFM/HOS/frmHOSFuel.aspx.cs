using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using SentinelFM;
using System.Data;
using System.IO;
public partial class HOS_frmHOSFuel : SentinelFMBasePage
{
    public string Error_Load = "";
    public string SelectVehicle = "Please select a vehicle";
    public string resizeScript = "";
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
        if (!Page.IsPostBack)
        {
            CboFleet_Fill();
            txtFrom.SelectedDate = System.DateTime.Now.Date;
            txtTo.SelectedDate = System.DateTime.Now.Date.AddDays(1);
        }
        Error_Load = (string)base.GetLocalResourceObject("Error_Load");
        //resizeScript = SetResizeScript(dgFuel.ClientID);
        dgFuel.RadAjaxManagerControl = RadAjaxManager1;

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
    protected void cboFleet_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (Convert.ToInt32(cboFleet.SelectedValue) != -1)
        {
            this.cboVehicle.Visible = true;
            this.lblVehicleName.Visible = true;
            CboVehicle_Fill(Convert.ToInt32(cboFleet.SelectedValue));
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
            //cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0_EntireFleet"), "0"));

            if ((sn.Report.VehicleId != 0) && (sn.Report.VehicleId != -1))
                cboVehicle.SelectedIndex = cboVehicle.Items.IndexOf(cboVehicle.Items.FindByValue(sn.Report.VehicleId.ToString()));


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
        if (cboVehicle.Items.Count == 0 || cboVehicle.SelectedIndex < 0)
        {
            lblMessage.Text = SelectVehicle;
            return;
        }
        lblMessage.Text = "";
        BindData(true);
    }
    protected void dgFuel_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        BindData(false);
    }

    DataTable CreateNullTable()
    { 
        DataTable dt = new DataTable();
        dt.Columns.Add("BoxId");
        dt.Columns.Add("EquipID");
        dt.Columns.Add("FuelTime");
        dt.Columns.Add("Odometer");
        dt.Columns.Add("DriverName");
        dt.Columns.Add("Address");
        dt.Columns.Add("Provider");
        dt.Columns.Add("Quantity");
        dt.Columns.Add("Unit");
        return dt;

    }
    void BindData(Boolean isBind)
    {
        try
        {
            if (cboVehicle.Items.Count > 0 && cboVehicle.SelectedIndex > 0)
            {
                int boxid = int.Parse(cboVehicle.SelectedValue);
                clsHOSManager hosManager = new clsHOSManager();
                DataSet dt = hosManager.Get_LogData_Fuel(boxid, txtFrom.SelectedDate.Value.Date, txtTo.SelectedDate.Value.Date);
                dgFuel.DataSource = dt;
            }
            else dgFuel.DataSource = CreateNullTable();
            if (isBind) dgFuel.DataBind();
        }
        catch (Exception Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            RadAjaxManager1.ResponseScripts.Add(string.Format("alert('{0}')", Error_Load));
        }



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