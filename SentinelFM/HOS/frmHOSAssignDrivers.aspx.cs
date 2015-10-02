using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SentinelFM;
using System.Data;
using System.IO;
using VLF.CLS;
using System.Data.SqlClient;
using System.Configuration;

public partial class HOS_frmHOSAssignDrivers : SentinelFMBasePage
{
    private string hosConnectionString =
        ConfigurationManager.ConnectionStrings["SentinelHOSConnectionString"].ConnectionString;
    protected void Page_Load(object sender, EventArgs e)
    {
         try
         {
             if (!Page.IsPostBack)
             {
                 LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref form1, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                 GuiSecurity(this);
                 LoadDrivers();
                 //LoadAssignedVehicles();
                 //LoadUnassignedVehicles();
                 cmdAssign.Enabled = false;
                 cmdAssignAll.Enabled = false;
                 cmdUnAssign.Enabled = false;
                 cmdUnAssignAll.Enabled = false;

             }

         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
               Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            //lblMessage.Text = Ex.Message;
           
         }
         lblError.Text = string.Empty;
    }

    private void LoadDrivers()
    {
        DataSet dsDrivers = new DataSet();
        string xmlResult = "";

        using (SentinelFM.ServerDBDriver.DBDriver drv = new global::SentinelFM.ServerDBDriver.DBDriver())
        {

            if (objUtil.ErrCheck(drv.GetAllDrivers(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xmlResult), false))
                if (objUtil.ErrCheck(drv.GetAllDrivers(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xmlResult), true))
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " No Drivers for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    //RedirectToLogin();
                    return;
                }
        }

        if (String.IsNullOrEmpty(xmlResult))
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " No Drivers for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            return;
        }

        dsDrivers.ReadXml(new StringReader(xmlResult));

        this.ddlDrivers.Items.Clear();

        if (Util.IsDataSetValid(dsDrivers))
        {
            this.ddlDrivers.DataSource = dsDrivers;
            this.ddlDrivers.DataBind();
            this.ddlDrivers.Items.Insert(0, new ListItem("Select a Driver", "-1"));
        }
        else
            this.ddlDrivers.Items.Insert(0, new ListItem("No Available Drivers", "-100"));



    }


    private void LoadUnassignedVehicles()
    {
        try
        {
            DataSet dataSet = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter();
            try
            {
                SqlConnection connection = new SqlConnection(hosConnectionString);
                adapter.SelectCommand = new SqlCommand();
                adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                adapter.SelectCommand.CommandText = "Get_UnassignAssignedVehicleByDriverID";
                adapter.SelectCommand.Connection = connection;

                SqlParameter sqlPara = new SqlParameter("@SentinelOrganizationId", SqlDbType.Int);
                sqlPara.Value = sn.User.OrganizationId;
                adapter.SelectCommand.Parameters.Add(sqlPara);
                sqlPara = new SqlParameter("@SentinelDrId", SqlDbType.Int);
                sqlPara.Value = int.Parse(ddlDrivers.SelectedValue.ToString());
                adapter.SelectCommand.Parameters.Add(sqlPara);

                adapter.Fill(dataSet);
                lboUnassigned.DataSource= dataSet.Tables[0];
                lboUnassigned.DataBind();

                if (dataSet.Tables[0] != null && dataSet.Tables[0].Rows.Count > 0)
                {
                    cmdAssign.Enabled = true;
                    cmdAssignAll.Enabled = true;
                }
                else
                {
                    cmdAssign.Enabled = false;
                    cmdAssignAll.Enabled = false;
                }
            }
            catch (Exception e)
            {
                lblError.Text = "Falied to load data.";
                cmdAssign.Enabled = false;
                cmdAssignAll.Enabled = false;
                cmdUnAssign.Enabled = false;
                cmdUnAssignAll.Enabled = false;
            }
        }
        catch(Exception ex){}
    }

    private void LoadAssignedVehicles()
    {
        try
        {
            DataSet dataSet = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter();
            try
            {
                SqlConnection connection = new SqlConnection(hosConnectionString);
                adapter.SelectCommand = new SqlCommand();
                adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                adapter.SelectCommand.CommandText = "Get_AssignedVehicleByDriverID";
                adapter.SelectCommand.Connection = connection;
                SqlParameter sqlPara = new SqlParameter("@SentinelOrganizationId", SqlDbType.Int);
                sqlPara.Value = sn.User.OrganizationId;
                adapter.SelectCommand.Parameters.Add(sqlPara);
                sqlPara = new SqlParameter("@SentinelDrId", SqlDbType.Int);
                sqlPara.Value = int.Parse(ddlDrivers.SelectedValue.ToString());
                adapter.SelectCommand.Parameters.Add(sqlPara);

                adapter.Fill(dataSet);
                lboassigned.DataSource = dataSet.Tables[0];
                lboassigned.DataBind();

                if (dataSet.Tables[0] != null && dataSet.Tables[0].Rows.Count > 0)
                {
                    cmdUnAssign.Enabled = true;
                    cmdUnAssignAll.Enabled = true;
                }
                else
                {
                    cmdUnAssign.Enabled = false;
                    cmdUnAssignAll.Enabled = false;
                }

            }
            catch (Exception e)
            {
                lblError.Text = "Falied to load data.";
                cmdAssign.Enabled = false;
                cmdAssignAll.Enabled = false;
                cmdUnAssign.Enabled = false;
                cmdUnAssignAll.Enabled = false;
            }
        }
        catch (Exception ex) { }
    }

    protected void ddlDrivers_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlDrivers.SelectedIndex == 0)
        {
            lboassigned.Items.Clear();
            lboUnassigned.Items.Clear();
            cmdAssign.Enabled = false;
            cmdAssignAll.Enabled = false;
            cmdUnAssign.Enabled = false;
            cmdUnAssignAll.Enabled = false;
        }
        else
        {
            LoadAssignedVehicles();
            LoadUnassignedVehicles();
        }
    }
    protected void cmdAssign_Click(object sender, EventArgs e)
    {
        if (ddlDrivers.SelectedIndex < 0)
        {
            lblError.Text = "Please select a driver.";
            return;
        }

        string selectVehicles = "";
        foreach(ListItem lstItem in lboUnassigned.Items)
        {
            if (lstItem.Selected)
            {
                if (selectVehicles == "") selectVehicles = lstItem.Value;
                else selectVehicles = selectVehicles + "," + lstItem.Value;
            }
        }
        if (selectVehicles == "")
        {
            lblError.Text = "Please select a vehicle.";
            return;
        }

        if (AssignVehicleToDrivers(selectVehicles)) ResetList();
    }

    private Boolean UnAssignDriverToVehicles(string selectVehicles)
    {
        int driverID = int.Parse(ddlDrivers.SelectedValue);
        Boolean ret = true;
        SqlConnection connection;
        using (connection = new SqlConnection(hosConnectionString))
        {
            SqlCommand sqlCmd = new SqlCommand();
            try
            {
                sqlCmd.Connection = connection;
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandText = "UnAssignDriverToVehicles";
                sqlCmd.Connection = connection;
                SqlParameter sqlPara = new SqlParameter("@UnAssignedBy", SqlDbType.Int);
                sqlPara.Value = sn.UserID;
                sqlCmd.Parameters.Add(sqlPara);

                sqlPara = new SqlParameter("@SentinelDrId", SqlDbType.Int);
                sqlPara.Value = driverID;
                sqlCmd.Parameters.Add(sqlPara);


                sqlPara = new SqlParameter("@VehicleIDs", SqlDbType.VarChar, -1);
                sqlPara.Value = selectVehicles;
                sqlCmd.Parameters.Add(sqlPara);

                sqlPara = new SqlParameter("@OrganizationID", SqlDbType.Int);
                sqlPara.Value = sn.User.OrganizationId;
                sqlCmd.Parameters.Add(sqlPara);
                connection.Open();
                sqlCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ret = false;
                lblError.Text = "Falied to unassign.";
            }
            finally
            {
                if (connection.State == ConnectionState.Open) connection.Close();
            }
        }
        return ret;

    }

    private Boolean AssignVehicleToDrivers(string selectVehicles)
    {
        int driverID = int.Parse(ddlDrivers.SelectedValue);
        Boolean ret = true;
        SqlConnection connection ;
        using (connection = new SqlConnection(hosConnectionString))
        {
            SqlCommand sqlCmd = new SqlCommand();
            try
            {
                sqlCmd.Connection = connection;
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandText = "AssignDriverToVehicles";
                sqlCmd.Connection = connection;
                SqlParameter sqlPara = new SqlParameter("@AssignedBy", SqlDbType.Int);
                sqlPara.Value = sn.UserID;
                sqlCmd.Parameters.Add(sqlPara);

                sqlPara = new SqlParameter("@SentinelDrId", SqlDbType.Int);
                sqlPara.Value = driverID;
                sqlCmd.Parameters.Add(sqlPara);


                sqlPara = new SqlParameter("@VehicleIDs", SqlDbType.VarChar, -1);
                sqlPara.Value = selectVehicles;
                sqlCmd.Parameters.Add(sqlPara);

                sqlPara = new SqlParameter("@OrganizationID", SqlDbType.Int);
                sqlPara.Value = sn.User.OrganizationId;
                sqlCmd.Parameters.Add(sqlPara);
                connection.Open();
                sqlCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ret =  false;
                lblError.Text = "Falied to assign.";
            }
            finally {
                if (connection.State == ConnectionState.Open) connection.Close();
            }
        }
        return ret;

    }

    protected void cmdAssignAll_Click(object sender, EventArgs e)
    {
        if (ddlDrivers.SelectedIndex < 0)
        {
            lblError.Text = "Please select a driver.";
            return;
        }

        string selectVehicles = "";
        foreach (ListItem lstItem in lboUnassigned.Items)
        {
            if (selectVehicles == "") selectVehicles = lstItem.Value;
            else selectVehicles = selectVehicles + "," + lstItem.Value;
        }
        if (selectVehicles == "")
        {
            lblError.Text = "Please select a vehicle.";
            return;
        }

        if (AssignVehicleToDrivers(selectVehicles))
            ResetList();

    }

    private void ResetList()
    {
        lboassigned.ClearSelection();
        lboassigned.Items.Clear();

        lboUnassigned.ClearSelection();
        lboUnassigned.Items.Clear();

        LoadAssignedVehicles();
        LoadUnassignedVehicles();
    }

    protected void cmdUnAssign_Click(object sender, EventArgs e)
    {
        if (ddlDrivers.SelectedIndex < 0)
        {
            lblError.Text = "Please select a driver.";
            return;
        }

        string selectVehicles = "";
        foreach (ListItem lstItem in lboassigned.Items)
        {
            if (lstItem.Selected)
            {
                if (selectVehicles == "") selectVehicles = lstItem.Value;
                else selectVehicles = selectVehicles + "," + lstItem.Value;
            }
        }
        if (selectVehicles == "")
        {
            lblError.Text = "Please select a vehicle.";
            return;
        }

        if (UnAssignDriverToVehicles(selectVehicles)) ResetList();

    }
    protected void cmdUnAssignAll_Click(object sender, EventArgs e)
    {
        if (ddlDrivers.SelectedIndex < 0)
        {
            lblError.Text = "Please select a driver.";
            return;
        }

        string selectVehicles = "";
        foreach (ListItem lstItem in lboassigned.Items)
        {
                if (selectVehicles == "") selectVehicles = lstItem.Value;
                else selectVehicles = selectVehicles + "," + lstItem.Value;
        }
        if (selectVehicles == "")
        {
            lblError.Text = "Please select a vehicle.";
            return;
        }

        if (UnAssignDriverToVehicles(selectVehicles)) ResetList();

    }
}