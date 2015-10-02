using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.SessionState;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Globalization;

namespace SentinelFM
{
    public partial class Configuration_frmVehicleStatus : SentinelFMBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string VehicleId = String.Empty;
            int iVehicleId = 0;
            string LicensePlate = String.Empty;

            VehicleId = Request.QueryString["VehicleId"];
            LicensePlate = Request.QueryString["LicensePlate"];

            if (!String.IsNullOrEmpty(VehicleId))
            {
                lblVehicleId.Text = VehicleId;
                iVehicleId = Convert.ToInt32(VehicleId);
            }
            lblLicensePlate.Text = LicensePlate;

            if (!IsPostBack)
            {
                if (!sn.User.ControlEnable(sn, 114))
                    cmdVehicleStatus.Visible = false;
                SetDropdowns(iVehicleId);
            }
        }

        private void SetDropdowns(int VehicleId)
        {
            DataSet dsVehicleDeviceStatuses = new DataSet();
            StringReader strrXML = null;

            string xml = "";

            ServerDBVehicle.DBVehicle dbu = new ServerDBVehicle.DBVehicle();
            //userId, string SID, int VehicleId
            if (objUtil.ErrCheck(dbu.GetVehicleDeviceStatuses(sn.UserID, sn.SecId, VehicleId, ref xml), false))
                if (objUtil.ErrCheck(dbu.GetVehicleDeviceStatuses(sn.UserID, sn.SecId, VehicleId, ref xml), true))
                {
                    this.cboVehicleStatus.DataSource = null;
                    this.cboVehicleStatus.DataBind();
                    return;
                }

            if (xml == "")
            {
                this.cboVehicleStatus.DataSource = null;
                this.cboVehicleStatus.DataBind();
                return;
            }

            cboVehicleStatus.Items.Clear();

            strrXML = new StringReader(xml);
            dsVehicleDeviceStatuses.ReadXml(strrXML);

            cboVehicleStatus.DataSource = dsVehicleDeviceStatuses;
            cboVehicleStatus.DataBind();

            cboVehicleStatus.Items.Insert(0, new ListItem(String.Empty, String.Empty));
            cboVehicleStatus.SelectedIndex = 0;
        }

        protected void cmdInfo_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmvehicle_add_edit.aspx?LicensePlate=" + lblLicensePlate.Text);
        }

        protected void cmdCustomFields_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmvehicle_customfields.aspx?VehicleId=" + lblVehicleId.Text + "&LicensePlate=" + lblLicensePlate.Text);
        }

        protected void cmdWorkingHours_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmvehicle_workinghours.aspx");
        }

        protected void cmdUpdate_Click(object sender, EventArgs e)
        {
            int VehicleDeviceStatusID = 0;
            string StatusDate = String.Empty;
            string AuthorizationNo = String.Empty;
            int VehicleId = 0;
            string Address = String.Empty;
            string refAddress = String.Empty;
            double Latitude = 0.0;
            double refLatitude = 0.0;
            double Longitude = 0.0;
            double refLongitude = 0.0;
            bool result = false;

            if (!String.IsNullOrEmpty(cboVehicleStatus.SelectedValue))
                VehicleDeviceStatusID = Convert.ToInt32(cboVehicleStatus.SelectedValue);

            Address = txtAddress.Text.Trim();

            if (!String.IsNullOrEmpty(Address))
            {
                Resolver.Resolver resolver = new Resolver.Resolver();
                result = resolver.Location(Address, ref refLatitude, ref refLongitude, ref refAddress);
                if (result)
                {
                    Address = refAddress;
                    Latitude = refLatitude;
                    Longitude = refLongitude;
                }
                else
                {
                    lblMsg.Text = "Address is invalid. Please check it and try again.";
                    return;
                }
            }

            StatusDate = txtDate.Text;
            AuthorizationNo = txtAuthorizationNo.Text;
            VehicleId = Convert.ToInt32(lblVehicleId.Text);

            ServerDBVehicle.DBVehicle dbu = new ServerDBVehicle.DBVehicle();

            int updateResult = dbu.UpdateVehicleDeviceStatus(sn.UserID, sn.LoginUserID, sn.SecId, VehicleDeviceStatusID, StatusDate, AuthorizationNo, VehicleId, Address, Latitude, Longitude);
            if (updateResult == 0)
                lblMsg.Text = "Error to update vehicle device status.";
            else
            {
                lblMsg.Text = "Vehicle status device updated successfully.";
                SetDropdowns(VehicleId);
            }
        }
    }
}