using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Configuration;
using System.IO;
using VLF.MAP;
using VLF.CLS.Interfaces;
using System.Globalization;



namespace SentinelFM.Map
{
    /// <summary>
    /// Summary description for frmBigDetailsVehicleSearch.
    /// </summary>
    public partial class frmBigDetailsVehicleSearch : SentinelFMBasePage
    {




        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dgSingleVehicle.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dgSingleVehicle_PageIndexChanged);
            this.dgMultiplyVehicle.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dgMultiplyVehicle_PageIndexChanged);

        }
        #endregion

        protected System.Web.UI.WebControls.DropDownList DropDownList1;
        
        
        public VLF.MAP.ClientMapProxy map;


        protected void Page_Load(object sender, System.EventArgs e)
        {

            

            if (!Page.IsPostBack)
            {
                this.tblLandmark.Visible = false;
                this.tblAddress.Visible = false;
            }
        }



        protected void cboSingleSearchType_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.cmdSearch.Visible = true;
            this.tblAddress.Visible = false;

            if ((this.cboSearchType.SelectedItem.Value == "0") || (this.cboSearchType.SelectedItem.Value == "1") || (this.cboSearchType.SelectedItem.Value == "3"))
            {
                this.txtSearchParam.Text = "";
                this.txtSearchParam.Visible = true;
                this.tblLandmark.Visible = false;
            }

            if (this.cboSearchType.SelectedItem.Value == "2")
            {
                this.txtSearchParam.Visible = false;
                this.tblLandmark.Visible = true;
                CboFleet_Fill();
                CboLandmarks_Fill();
            }

            if (this.cboSearchType.SelectedItem.Value == "4")
            {
                CboFleet_Fill();
                this.txtSearchParam.Text = "";
                this.txtSearchParam.Visible = false;
                this.tblAddress.Visible = true;
                this.tblLandmark.Visible = false;
                this.cmdSearch.Visible = false;
            }

            this.dgMultiplyVehicle.DataSource = null;
            this.dgMultiplyVehicle.DataBind();

            this.dgSingleVehicle.DataSource = null;
            this.dgSingleVehicle.DataBind();
            this.txtSearchParam.Text = "";
            this.lblMessage.Text = "";
            this.cmdGoTo.Visible = false;


        }

        protected void cmdSearch_Click(object sender, System.EventArgs e)
        {
            try
            {
                this.lblMessage.Text = "";

                if ((this.cboSearchType.SelectedItem.Value == "0") || (this.cboSearchType.SelectedItem.Value == "1") || (this.cboSearchType.SelectedItem.Value == "3"))
                {

                    if (cboSearchType.SelectedItem.Value == "3")
                    {
                        if (!clsUtility.IsNumeric(this.txtSearchParam.Text))
                        {
                            this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_InvalidInputError");
                            this.lblMessage.Visible = true;
                            return;
                        }
                    }

                    DgSingleVehicle_Fill();
                }

                // Search by Landmark

                if (this.cboSearchType.SelectedItem.Value == "2")
                {
                    DgMultiplyVehicle_Fill();
                }

                if (this.cboSearchType.SelectedItem.Value == "4")
                {
                    DgByAddress_Fill();
                }

            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }

        private void CboFleet_Fill()
        {
            try
            {
                
                DataSet dsFleets = new DataSet();
                StringReader strrXML = null;


                string xml = "";
                ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();

                if (objUtil.ErrCheck(dbf.GetFleetsInfoXMLByUserIdByLang(sn.UserID, sn.SecId, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                    if (objUtil.ErrCheck(dbf.GetFleetsInfoXMLByUserIdByLang(sn.UserID, sn.SecId, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                    {
                        cboAdvanceFleet.DataSource = null;
                        cboAdvanceFleet.DataBind();
                        return;
                    }
                strrXML = new StringReader(xml);
                dsFleets.ReadXml(strrXML);

                cboAdvanceFleet.DataSource = dsFleets;
                cboAdvanceFleet.DataBind();
                cboAdvanceFleet.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboAdvanceFleet_Item_0"), "-1"));


                cboFleetAdrSrch.DataSource = dsFleets;
                cboFleetAdrSrch.DataBind();
                cboFleetAdrSrch.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboFleetAdrSrch_Item_0"), "-1"));



            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }

        }


        private void CboLandmarks_Fill()
        {
            try
            {
                StringReader strrXML = null;
                DataSet dsLandmarks = new DataSet();
                
                string xml = "";
                ServerDBOrganization.DBOrganization dbo = new ServerDBOrganization.DBOrganization();

                if (objUtil.ErrCheck(dbo.GetOrganizationLandmarksXMLByOrganizationId(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), false))
                    if (objUtil.ErrCheck(dbo.GetOrganizationLandmarksXMLByOrganizationId(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), true))
                    {
                        return;
                    }

                if (xml == "")
                {
                    this.cboAdvanceLandmarks.DataSource = null;
                    this.cboAdvanceLandmarks.DataBind();
                    return;
                }


                strrXML = new StringReader(xml);
                dsLandmarks.ReadXml(strrXML);
                this.cboAdvanceLandmarks.DataSource = dsLandmarks;
                this.cboAdvanceLandmarks.DataBind();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }


        private void MapIt(string VehicleId)
        {
            try
            {
                
                DataSet ds = new DataSet();
                ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();
                StringReader strrXML = null;
                string xml = "";

                if (objUtil.ErrCheck(dbf.GetFleetsInfoXMLByVehicleId(sn.UserID, sn.SecId, Convert.ToInt64(VehicleId), ref xml), false))
                    if (objUtil.ErrCheck(dbf.GetFleetsInfoXMLByVehicleId(sn.UserID, sn.SecId, Convert.ToInt64(VehicleId), ref xml), true))
                    {
                        return;
                    }


                strrXML = new StringReader(xml);


                if (xml == "")
                {
                    return;
                }

                ds.ReadXml(strrXML);


                if (ds.Tables.Count == 0)
                    return;

                // Set FleetId
                sn.Map.SelectedFleetID = Convert.ToInt32(ds.Tables[0].Rows[0]["FleetId"]);
                DsFleetInfo_Fill(sn.Map.SelectedFleetID);

                // Set checkbox for vehicle
                foreach (DataRow rowItem in sn.Map.DsFleetInfo.Tables[0].Rows)
                {
                    if (VehicleId == rowItem["VehicleId"].ToString())
                        rowItem["chkBoxShow"] = true;
                    else
                        rowItem["chkBoxShow"] = false;

                }
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }

        }

        private void DsFleetInfo_Fill(int fleetId)
        {
            try
            {

                DataSet dsFleetInfo = new DataSet();

                
                StringReader strrXML = null;


                string xml = "";
                ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();

                if (objUtil.ErrCheck(dbf.GetVehiclesLastKnownPositionInfoByLang(sn.UserID, sn.SecId, fleetId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                    if (objUtil.ErrCheck(dbf.GetVehiclesLastKnownPositionInfoByLang(sn.UserID, sn.SecId, fleetId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                    {
                        return;
                    }
                if (xml == "")
                {
                    sn.Map.DsFleetInfo = null;
                    return;
                }

                strrXML = new StringReader(xml);
                dsFleetInfo.ReadXml(strrXML);


                // Show Combobox
                DataColumn dc = new DataColumn();
                dc.ColumnName = "chkBoxShow";
                dc.DataType = Type.GetType("System.Boolean");
                dc.DefaultValue = false;
                dsFleetInfo.Tables[0].Columns.Add(dc);


                // Command Status (Update Position)
                dc = new DataColumn();
                dc.ColumnName = "Updated";

                dc.DataType = Type.GetType("System.Int16");
                dc.DefaultValue = false;
                dsFleetInfo.Tables[0].Columns.Add(dc);


                //Last Communicated
                DataColumn colDateTime = new DataColumn("MyDateTime", Type.GetType("System.DateTime"));
                dsFleetInfo.Tables[0].Columns.Add(colDateTime);


                // Vehicle Status
                dc = new DataColumn();
                dc.ColumnName = "VehicleStatus";
                dc.DataType = Type.GetType("System.String");
                dc.DefaultValue = "";
                dsFleetInfo.Tables[0].Columns.Add(dc);

                //Set show checkbox

                if (sn.Map.DsFleetInfo != null)
                {

                    GeoPoint geoPoint = new GeoPoint();
                    if (dsFleetInfo.Tables[0].Columns.IndexOf("StreetAddress") == -1)
                    {
                        DataColumn colStreetAddress = new DataColumn("StreetAddress", Type.GetType("System.String"));
                        dsFleetInfo.Tables[0].Columns.Add(colStreetAddress);
                    }

                    string streetAdr = "";

                    foreach (DataRow rowSN in sn.Map.DsFleetInfo.Tables[0].Rows)
                    {
                        if (rowSN["chkBoxShow"].ToString().ToLower() == "true")
                        {

                            foreach (DataRow rowLast in dsFleetInfo.Tables[0].Rows)
                            {
                                if (rowLast["VehicleId"].ToString() == rowSN["VehicleId"].ToString())
                                {
                                    rowLast["chkBoxShow"] = "true";

                                    streetAdr = rowLast["StreetAddress"].ToString().TrimEnd();

                                    // Get Street address
                                    if ((streetAdr == "") ||
                                        (streetAdr == "0") ||
                                        (streetAdr == VLF.CLS.Def.Const.addressNA))
                                    {
                                        geoPoint.Latitude = Convert.ToDouble(rowLast["Latitude"]);
                                        geoPoint.Longitude = Convert.ToDouble(rowLast["Longitude"]);

                                        // create ClientMapProxy only for geocoding
                                        VLF.MAP.ClientMapProxy geoCode = new VLF.MAP.ClientMapProxy(sn.User.GeoCodeEngine);
                                        if (geoCode == null)
                                        {
                                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Can't create map " + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                                            return;
                                        }
                                        rowLast["StreetAddress"] = geoCode.GetStreetAddress(geoPoint);

                                        

                                        ServerDBSystem.DBSystem dbs = new ServerDBSystem.DBSystem();
                                        if (objUtil.ErrCheck(dbs.AddMapUserUsage(sn.UserID, sn.SecId, Convert.ToInt16(VLF.CLS.Def.Enums.MapTypes.StreetAddress), (short)geoCode.LastUsedGeoCodeID), false))
                                            if (objUtil.ErrCheck(dbs.AddMapUserUsage(sn.UserID, sn.SecId, Convert.ToInt16(VLF.CLS.Def.Enums.MapTypes.StreetAddress), (short)geoCode.LastUsedGeoCodeID), true))
                                            {
                                                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, "Update user map usage info failed."));
                                            }
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }


                // Show Armed checkbox

                dc = new DataColumn();
                dc.ColumnName = "chkArm";
                dc.DataType = Type.GetType("System.Boolean");
                dc.DefaultValue = false;
                dsFleetInfo.Tables[0].Columns.Add(dc);



                // Show Heading

                dc = new DataColumn();
                dc.ColumnName = "MyHeading";
                dc.DataType = Type.GetType("System.String");
                dc.DefaultValue = "";
                dsFleetInfo.Tables[0].Columns.Add(dc);


                foreach (DataRow rowItem in dsFleetInfo.Tables[0].Rows)
                {

                    // Armed check
                    rowItem["chkArm"] = rowItem["BoxArmed"];

                    // Last Comunicated

                    rowItem["MyDateTime"] = Convert.ToDateTime(rowItem["OriginDateTime"].ToString());


                    // Heading
                    if (Convert.ToDouble(rowItem["Speed"]) != 0)
                    {

                        if ((rowItem["Heading"] != null) &&
                            (rowItem["Heading"].ToString() != ""))
                        {
                            rowItem["MyHeading"] = Heading(rowItem["Heading"].ToString());
                        }
                    }




                    // Vehicle Status
                    string SensorMask = rowItem["SensorMask"].ToString();
                    string Speed = rowItem["Speed"].ToString();
                    if (SensorMask != "")
                    {
                        Int64 snsMask = Convert.ToInt64(SensorMask);
                        if ((snsMask & 4) != 0)
                        {
                            // Ignition sensor status is ON

                            if ((Speed != "0") && (Speed != ""))
                            {
                                rowItem["VehicleStatus"] = Resources.Const.VehicleStatus_Moving ;
                            }
                            else if (Speed == "0")
                            {
                                rowItem["VehicleStatus"] = Resources.Const.VehicleStatus_Idling ;
                            }
                        }
                        else
                        {
                            // Ignition sensor status is OFF
                            if (Speed == "0")
                            {
                                rowItem["VehicleStatus"] = Resources.Const.VehicleStatus_Parked ;
                            }

                            if (Speed != "0")
                            {
                                rowItem["VehicleStatus"] = Resources.Const.VehicleStatus_Moving;
                            }
                        }
                    }
                }

                dsFleetInfo.Tables[0].DefaultView.Sort = "chkBoxShow" + " DESC";
                sn.Map.DsFleetInfo = dsFleetInfo;

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }


        private string Heading(string heading)
        {
            return sn.Map.Heading(heading);
        }

        protected void cmdGoTo_Click(object sender, System.EventArgs e)
        {

            if ((this.cboSearchType.SelectedItem.Value == "0") || (this.cboSearchType.SelectedItem.Value == "1") || (this.cboSearchType.SelectedItem.Value == "3"))
            {
                if ((ViewState["VehicleId"] != null) && (ViewState["VehicleId"].ToString() != ""))
                {
                    this.lblMessage.Text = "";
                    MapIt(ViewState["VehicleId"].ToString());
                    Response.Write("<SCRIPT Language='javascript'>window.opener.location.href='frmBigDetails.aspx'; </SCRIPT>");
                    Response.Write("<script language='javascript'>window.close()</script>");
                }
                else
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_SelectVehicle");
                }
            }

            if (this.cboSearchType.SelectedItem.Value == "2" || (this.cboSearchType.SelectedItem.Value == "4"))
            {

                if (this.cboSearchType.SelectedItem.Value == "2")
                    sn.Map.SelectedFleetID = Convert.ToInt32(this.cboAdvanceFleet.SelectedItem.Value);
                else if (this.cboSearchType.SelectedItem.Value == "4")
                    sn.Map.SelectedFleetID = Convert.ToInt32(this.cboFleetAdrSrch.SelectedItem.Value);




                DsFleetInfo_Fill(Convert.ToInt32(sn.Map.SelectedFleetID));

                for (int i = 0; i < dgMultiplyVehicle.Items.Count; i++)
                {
                    CheckBox ch = (CheckBox)(dgMultiplyVehicle.Items[i].Cells[1].Controls[1]);
                    foreach (DataRow rowItem in sn.Map.DsFleetInfo.Tables[0].Rows)
                    {
                        if (dgMultiplyVehicle.Items[i].Cells[0].Text.ToString() == rowItem["VehicleId"].ToString())
                            rowItem["chkBoxShow"] = ch.Checked;
                    }
                }

                Response.Write("<SCRIPT Language='javascript'>window.opener.location.href='frmBigDetails.aspx'; </SCRIPT>");
                Response.Write("<script language='javascript'>window.close()</script>");


            }
        }

        protected void dgSingleVehicle_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            ViewState["VehicleId"] = dgSingleVehicle.DataKeys[dgSingleVehicle.SelectedIndex];
        }



        private void DgSingleVehicle_Fill()
        {
            DataSet dsSearch = new DataSet();
            string xml = "";
            
            StringReader strrXML = null;


            try
            {

                ServerDBOrganization.DBOrganization dbo = new ServerDBOrganization.DBOrganization();
                if (objUtil.ErrCheck(dbo.GetVehiclesLastKnownPositionInfo(sn.UserID, sn.SecId, sn.User.OrganizationId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                    if (objUtil.ErrCheck(dbo.GetVehiclesLastKnownPositionInfo(sn.UserID, sn.SecId, sn.User.OrganizationId,CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                    {
                        dgSingleVehicle.DataSource = null;
                        dgSingleVehicle.DataBind();
                        return;
                    }

                strrXML = new StringReader(xml);
                dsSearch.ReadXml(strrXML);

                // Vehicle Status
                DataColumn dc = new DataColumn();
                dc.ColumnName = "VehicleStatus";
                dc.DataType = Type.GetType("System.String");
                dc.DefaultValue = "";
                dsSearch.Tables[0].Columns.Add(dc);


                foreach (DataRow rowItem in dsSearch.Tables[0].Rows)
                {
                    // Vehicle Status
                    string SensorMask = rowItem["SensorMask"].ToString();
                    string Speed = rowItem["Speed"].ToString();
                    //if(SensorMask != "")
                    //{
                    //    Int64 snsMask = Convert.ToInt64(SensorMask);
                    //    if( (snsMask & 4) != 0)
                    //    {
                    //        // Ignition sensor status is ON

                    //        if ((Speed!="0") && (Speed!=""))
                    //        {
                    //            rowItem["VehicleStatus"]="Moving";
                    //        }
                    //        else if (Speed=="0")
                    //        {
                    //            rowItem["VehicleStatus"]="Idling";
                    //        }
                    //    }
                    //    else
                    //    {
                    //        // Ignition sensor status is OFF
                    //        if (Speed=="0")
                    //        {
                    //            rowItem["VehicleStatus"]="Parked";
                    //        }

                    //        if (Speed!="0")
                    //        {
                    //            rowItem["VehicleStatus"]="Moving";
                    //        }
                    //    }
                    //}


                    if (!Convert.ToBoolean(rowItem["LastStatusSensor"])) // Ignition OFF /Untethered
                    {
                        // Ignition sensor status is OFF
                        if (Convert.ToInt16(rowItem["VehicleTypeId"].ToString().TrimEnd()) == Convert.ToInt16(VLF.CLS.Def.Enums.VehicleType.Trailer))
                            rowItem["VehicleStatus"] = Resources.Const.VehicleStatus_Untethered ;
                        else
                            rowItem["VehicleStatus"] = Resources.Const.VehicleStatus_Parked ;
                    }
                    else if (Convert.ToBoolean(rowItem["LastStatusSensor"])) // Ignition ON / Tethered
                    {
                        if (Convert.ToDateTime(rowItem["LastCommunicatedDateTime"]) > Convert.ToDateTime(rowItem["OriginDateTime"]))
                        {
                            if (Convert.ToInt16(rowItem["VehicleTypeId"].ToString().TrimEnd()) == Convert.ToInt16(VLF.CLS.Def.Enums.VehicleType.Trailer))
                                rowItem["VehicleStatus"] = Resources.Const.VehicleStatus_Tethered ;
                            else
                                rowItem["VehicleStatus"] = Resources.Const.VehicleStatus_Ignition_ON ;
                        }
                        else
                        {
                            if ((Speed != "0") && (Speed != ""))
                                rowItem["VehicleStatus"] = Resources.Const.VehicleStatus_Moving;
                            else if (Speed == "0")
                            {

                                if (Convert.ToInt16(rowItem["VehicleTypeId"].ToString().TrimEnd()) == Convert.ToInt16(VLF.CLS.Def.Enums.VehicleType.Trailer))
                                    rowItem["VehicleStatus"] = Resources.Const.VehicleStatus_Tethered ;
                                else
                                    rowItem["VehicleStatus"] = Resources.Const.VehicleStatus_Idling ;
                            }
                            else // Speed==""
                            {
                                rowItem["VehicleStatus"] = "";
                            }
                        }
                    }
                    else // Speed and Status are not set
                    {
                        rowItem["VehicleStatus"] = "";
                    }




                    try
                    {
                        if (Convert.ToDateTime(rowItem["OriginDateTime"]) < System.DateTime.Now.AddMinutes(-sn.User.PositionExpiredTime))
                            rowItem["VehicleStatus"] = Resources.Const.VehicleStatus_NA ;

                    }
                    catch
                    {
                    }


                    try
                    {
                        if (Convert.ToDateTime(rowItem["LastCommunicatedDateTime"]) > Convert.ToDateTime(rowItem["OriginDateTime"]))
                        {

                            rowItem["VehicleStatus"] += "*";
                        }
                    }
                    catch
                    {
                    }


                    if (Convert.ToBoolean(rowItem["Dormant"]))
                    {
                        rowItem["VehicleStatus"] = Resources.Const.VehicleStatus_PowerSave ;
                    }
                }

                DataRow[] drCollections = null;


                DataTable dt = new DataTable();
                DataColumn colVehicle = new DataColumn("VehicleId", Type.GetType("System.Double"));
                dt.Columns.Add(colVehicle);
                DataColumn colDescription = new DataColumn("Description", Type.GetType("System.String"));
                dt.Columns.Add(colDescription);
                DataColumn colStatus = new DataColumn("VehicleStatus", Type.GetType("System.String"));
                dt.Columns.Add(colStatus);


                if (this.cboSearchType.SelectedItem.Value == "0")
                    drCollections = dsSearch.Tables[0].Select("Description like '" + this.txtSearchParam.Text.Replace("'", "''") + "%'", "Description", DataViewRowState.CurrentRows);


                if (this.cboSearchType.SelectedItem.Value == "1")
                    drCollections = dsSearch.Tables[0].Select("LicensePlate like '" + this.txtSearchParam.Text + "%'", "Description", DataViewRowState.CurrentRows);

                if (drCollections != null)
                {
                    foreach (DataRow rowItem in drCollections)
                    {
                        DataRow dr = dt.NewRow();
                        dr["VehicleId"] = rowItem["VehicleId"];
                        dr["Description"] = rowItem["Description"];
                        dr["VehicleStatus"] = rowItem["VehicleStatus"];

                        dt.Rows.Add(dr);
                    }
                }


                if (this.cboSearchType.SelectedItem.Value == "3")
                {
                    //drCollections=dsSearch.Tables[0].Select("BoxId=" + this.txtSearchParam.Text ,"Description");
                    foreach (DataRow rowItem in dsSearch.Tables[0].Rows)
                    {
                        if (Convert.ToInt64(rowItem["BoxId"]) == Convert.ToInt64(this.txtSearchParam.Text))
                        {
                            DataRow dr = dt.NewRow();
                            dr["VehicleId"] = rowItem["VehicleId"];
                            dr["Description"] = rowItem["Description"];
                            dr["VehicleStatus"] = rowItem["VehicleStatus"];

                            dt.Rows.Add(dr);
                            break;
                        }
                    }
                }




                this.dgSingleVehicle.DataSource = dt;
                this.dgSingleVehicle.DataBind();

                if (dt.Rows.Count > 0)
                {
                    this.lblMessage.Text = "";
                    this.cmdGoTo.Visible = true;
                }
                else
                {
                    this.cmdGoTo.Visible = false;
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_NoDataFound");
                }
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }

        private void DgMultiplyVehicle_Fill()
        {
            try
            {

                DataSet dsSearch = new DataSet();
                string xml = "";
                
                StringReader strrXML = null;

                this.lblMessage.Visible = true;
                if (this.cboAdvanceFleet.SelectedItem.Value == "-1")
                {
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_SelectFleet");
                    return;
                }


                ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();
                if (objUtil.ErrCheck(dbf.GetVehiclesLastKnownPositionInfoNearestToLandmark(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt32(this.cboAdvanceFleet.SelectedItem.Value), Convert.ToInt32(this.cboAdvanceDist.SelectedItem.Value), this.cboAdvanceLandmarks.SelectedItem.Value, ref xml), false))
                    if (objUtil.ErrCheck(dbf.GetVehiclesLastKnownPositionInfoNearestToLandmark(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt32(this.cboAdvanceFleet.SelectedItem.Value), Convert.ToInt32(this.cboAdvanceDist.SelectedItem.Value), this.cboAdvanceLandmarks.SelectedItem.Value, ref xml), true))
                    {
                        dgMultiplyVehicle.DataSource = null;
                        dgMultiplyVehicle.DataBind();
                        return;
                    }

                strrXML = new StringReader(xml);
                dsSearch.ReadXml(strrXML);

                if (dsSearch.Tables.Count == 0)
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_NoDataFound");
                    this.dgMultiplyVehicle.DataSource = null;
                    this.dgMultiplyVehicle.DataBind();
                    return;
                }

                // Vehicle Status
                DataColumn dc = new DataColumn();
                dc.ColumnName = "VehicleStatus";
                dc.DataType = Type.GetType("System.String");
                dc.DefaultValue = "";
                dsSearch.Tables[0].Columns.Add(dc);

                // Show Combobox
                dc = new DataColumn();
                dc.ColumnName = "chkBoxShow";
                dc.DataType = Type.GetType("System.Boolean");
                dc.DefaultValue = false;
                dsSearch.Tables[0].Columns.Add(dc);

                foreach (DataRow rowItem in dsSearch.Tables[0].Rows)
                {
                    // Vehicle Status
                    string SensorMask = rowItem["SensorMask"].ToString();
                    string Speed = rowItem["Speed"].ToString();
                    if (SensorMask != "")
                    {
                        Int64 snsMask = Convert.ToInt64(SensorMask);
                        if ((snsMask & 4) != 0)
                        {
                            // Ignition sensor status is ON

                            if ((Speed != "0") && (Speed != ""))
                            {
                                rowItem["VehicleStatus"] = Resources.Const.VehicleStatus_Moving;
                            }
                            else if (Speed == "0")
                            {
                                rowItem["VehicleStatus"] = Resources.Const.VehicleStatus_Idling ;
                            }
                        }
                        else
                        {
                            // Ignition sensor status is OFF
                            if (Speed == "0")
                            {
                                rowItem["VehicleStatus"] = Resources.Const.VehicleStatus_Parked ;
                            }

                            if (Speed != "0")
                            {
                                rowItem["VehicleStatus"] = Resources.Const.VehicleStatus_Moving;
                            }
                        }
                    }
                }

                dgMultiplyVehicle.DataSource = dsSearch;
                dgMultiplyVehicle.DataBind();

                if (dsSearch.Tables[0].Rows.Count > 0)
                {
                    this.lblMessage.Text = "";
                    this.cmdGoTo.Visible = true;
                }
                else
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_NoDataFound");
                    this.cmdGoTo.Visible = false;
                }

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }


        private void DgByAddress_Fill()
        {
            try
            {
                this.lblMessage.Text = "";

                DataSet dsSearch = new DataSet();
                string xml = "";
                
                StringReader strrXML = null;



                ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();
                if (objUtil.ErrCheck(dbf.GetVehiclesLastKnownPositionInfoNearestToLatLon(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt32(this.cboFleetAdrSrch.SelectedItem.Value), Convert.ToInt32(this.cboDistanceAdrSrc.SelectedItem.Value), Convert.ToDouble(this.txtY.Text), Convert.ToDouble(this.txtX.Text), ref xml), false))
                    if (objUtil.ErrCheck(dbf.GetVehiclesLastKnownPositionInfoNearestToLatLon(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt32(this.cboFleetAdrSrch.SelectedItem.Value), Convert.ToInt32(this.cboDistanceAdrSrc.SelectedItem.Value), Convert.ToDouble(this.txtY.Text), Convert.ToDouble(this.txtX.Text), ref xml), true))
                    {
                        dgMultiplyVehicle.DataSource = null;
                        dgMultiplyVehicle.DataBind();
                        return;
                    }

                strrXML = new StringReader(xml);
                dsSearch.ReadXml(strrXML);

                if (dsSearch.Tables.Count == 0)
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_NoDataFound");
                    this.dgMultiplyVehicle.DataSource = null;
                    this.dgMultiplyVehicle.DataBind();
                    return;
                }



                // Vehicle Status
                DataColumn dc = new DataColumn();
                dc.ColumnName = "VehicleStatus";
                dc.DataType = Type.GetType("System.String");
                dc.DefaultValue = "";
                dsSearch.Tables[0].Columns.Add(dc);

                // Show Combobox
                dc = new DataColumn();
                dc.ColumnName = "chkBoxShow";
                dc.DataType = Type.GetType("System.Boolean");
                dc.DefaultValue = false;
                dsSearch.Tables[0].Columns.Add(dc);

                foreach (DataRow rowItem in dsSearch.Tables[0].Rows)
                {
                    // Vehicle Status
                    string SensorMask = rowItem["SensorMask"].ToString();
                    string Speed = rowItem["Speed"].ToString();
                    if (SensorMask != "")
                    {
                        Int64 snsMask = Convert.ToInt64(SensorMask);
                        if ((snsMask & 4) != 0)
                        {
                            // Ignition sensor status is ON

                            if ((Speed != "0") && (Speed != ""))
                            {
                                rowItem["VehicleStatus"] = Resources.Const.VehicleStatus_Moving;
                            }
                            else if (Speed == "0")
                            {
                                rowItem["VehicleStatus"] = Resources.Const.VehicleStatus_Idling ;
                            }
                        }
                        else
                        {
                            // Ignition sensor status is OFF
                            if (Speed == "0")
                            {
                                rowItem["VehicleStatus"] = Resources.Const.VehicleStatus_Parked ;
                            }

                            if (Speed != "0")
                            {
                                rowItem["VehicleStatus"] = Resources.Const.VehicleStatus_Moving;
                            }
                        }
                    }
                }

                dgMultiplyVehicle.DataSource = dsSearch;
                dgMultiplyVehicle.DataBind();

                if (dsSearch.Tables[0].Rows.Count > 0)
                {
                    this.lblMessage.Text = "";
                    this.cmdGoTo.Visible = true;
                }
                else
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_NoDataFound");
                    this.cmdGoTo.Visible = false;
                }

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }


        protected void dgSingleVehicle_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
        {
            dgSingleVehicle.CurrentPageIndex = e.NewPageIndex;
            DgSingleVehicle_Fill();
            dgSingleVehicle.SelectedIndex = -1;
        }

        protected void dgMultiplyVehicle_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
        {
            dgMultiplyVehicle.CurrentPageIndex = e.NewPageIndex;
            DgMultiplyVehicle_Fill();
            dgMultiplyVehicle.SelectedIndex = -1;
        }

        protected void cboAdvanceFleet_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.dgMultiplyVehicle.DataSource = null;
            this.dgMultiplyVehicle.DataBind();
        }

   

        protected void cmdFindAddress_Click(object sender, EventArgs e)
        {


            this.lblMessage.Text = "";

            if (this.cboFleetAdrSrch.SelectedItem.Value == "-1")
            {
                this.lblMessage.Visible = true;
                this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_SelectFleet");
                return;
            }


            this.cmdSearch.Visible = false;
            //Get Coordinates by street address
            string xml = "";
            string strAddress = "";
            DataSet ds = new DataSet();
            strAddress = this.txtStreet.Text + "|" + this.txtCity.Text + ", " + this.txtState.Text + ", " + this.cboCountry.SelectedItem.Value;
            // create ClientMapProxy only for geocoding
            VLF.MAP.ClientMapProxy geoMap = new VLF.MAP.ClientMapProxy(sn.User.GeoCodeEngine);
            if (geoMap == null)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Can't create map " + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                return;
            }
            xml = geoMap.GetAddressMatches(strAddress);

            

            ServerDBSystem.DBSystem dbs = new ServerDBSystem.DBSystem();
            if (objUtil.ErrCheck(dbs.AddMapUserUsage(sn.UserID, sn.SecId, Convert.ToInt16(VLF.CLS.Def.Enums.MapTypes.StreetAddress), (short)geoMap.LastUsedGeoCodeID), false))
                if (objUtil.ErrCheck(dbs.AddMapUserUsage(sn.UserID, sn.SecId, Convert.ToInt16(VLF.CLS.Def.Enums.MapTypes.StreetAddress), (short)geoMap.LastUsedGeoCodeID), true))
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, "Update user map usage info failed."));
                }

            if (xml != "")
            {
                ds = new DataSet();
                ds.ReadXml(new StringReader(xml));
            }

            if ((xml == "") || (ds.Tables.Count == 0))
            {
                this.lblMessage.Visible = true;
                this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_AddressNotFound");
                this.dgAddress.DataSource = null;
                this.dgAddress.DataBind();
                return;
            }
            else
            {
                this.lblMessage.Visible = true;
                this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_SelectAddress");
            }

            this.dgAddress.DataSource = ds;
            this.dgAddress.DataBind();
        }
        protected void dgAddress_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.txtX.Text = dgAddress.SelectedItem.Cells[2].Text;
            this.txtY.Text = dgAddress.SelectedItem.Cells[1].Text;
            this.cmdSearch.Visible = true;
        }

        protected void cmdClose_Click(object sender, EventArgs e)
        {
            Response.Write("<script language='javascript'>window.close()</script>");
        }
    }
}