using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.IO;
using VLF.DAS.DB;   

namespace SentinelFM
{
    public partial class Messages_frmNewLocation : SentinelFMBasePage
    {
        
        public string strAddress = "";

        public bool ShowOrganizationHierarchy;
        public int DefaultOrganizationHierarchyFleetId = 0;
        public string DefaultOrganizationHierarchyFleetName = string.Empty;
        public string DefaultOrganizationHierarchyNodeCode = string.Empty;
        public string LoadVehiclesBasedOn = "fleet";
        public string OrganizationHierarchyPath = "";
        VLF.PATCH.Logic.PatchOrganizationHierarchy poh;

        protected void Page_Load(object sender, EventArgs e)
        {
            string defaultnodecode = string.Empty;

            ServerDBUser.DBUser dbu = new ServerDBUser.DBUser();

            string xml = "";
            if (objUtil.ErrCheck(dbu.GetUserPreferencesXML(sn.UserID, sn.SecId, ref xml), false))
                if (objUtil.ErrCheck(dbu.GetUserPreferencesXML(sn.UserID, sn.SecId, ref xml), true))
                {

                }

            StringReader strrXML = new StringReader(xml);
            DataSet dsPref = new DataSet();
            dsPref.ReadXml(strrXML);

            foreach (DataRow rowItem in dsPref.Tables[0].Rows)
            {
                if (Convert.ToInt16(rowItem["PreferenceId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.Preference.DefaultOrganizationHierarchyNodeCode))
                {
                    defaultnodecode = rowItem["PreferenceValue"].ToString().TrimEnd();

                }

                if (Convert.ToInt16(rowItem["PreferenceId"]) == 36)
                {
                    string d = rowItem["PreferenceValue"].ToString().ToLower();
                    if (d == "hierarchy")
                        LoadVehiclesBasedOn = "hierarchy";
                    else
                        LoadVehiclesBasedOn = "fleet";
                }
            }

            string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;

            poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);
            if (poh.HasOrganizationHierarchy(sn.User.OrganizationId))
                ShowOrganizationHierarchy = true;
            else
            {
                ShowOrganizationHierarchy = false;
                LoadVehiclesBasedOn = "fleet";
            }

            if (LoadVehiclesBasedOn == "hierarchy")
            {
                defaultnodecode = defaultnodecode ?? string.Empty;
                if (defaultnodecode == string.Empty)
                {
                    if (sn.User.OrganizationId == 999763 || sn.User.OrganizationId == 999955 || sn.User.OrganizationId == 999956 || sn.User.OrganizationId == 999957 || sn.User.OrganizationId == 480)
                        defaultnodecode = poh.GetOrganizationHierarchyRootNodeCodeUserID(sn.User.OrganizationId, sn.UserID);
                    else
                        defaultnodecode = poh.GetRootNodeCode(sn.User.OrganizationId);

                }

                if (!IsPostBack)
                {
                    DefaultOrganizationHierarchyNodeCode = defaultnodecode;
                    DefaultOrganizationHierarchyFleetId = poh.GetFleetIdByNodeCode(sn.User.OrganizationId, defaultnodecode);
                    DefaultOrganizationHierarchyFleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, DefaultOrganizationHierarchyFleetId);
                    OrganizationHierarchyPath = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, defaultnodecode);
                    hidOrganizationHierarchyNodeCode.Value = DefaultOrganizationHierarchyNodeCode;
                }
                else
                {
                    DefaultOrganizationHierarchyNodeCode = hidOrganizationHierarchyNodeCode.Value.ToString();
                    DefaultOrganizationHierarchyFleetId = poh.GetFleetIdByNodeCode(sn.User.OrganizationId, DefaultOrganizationHierarchyNodeCode);
                    DefaultOrganizationHierarchyFleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, DefaultOrganizationHierarchyFleetId);
                }

                btnOrganizationHierarchyNodeCode.Text = DefaultOrganizationHierarchyFleetName == string.Empty ? DefaultOrganizationHierarchyNodeCode : DefaultOrganizationHierarchyFleetName;
                lblFleetTitle.Visible = false;
                cboFleet.Visible = false;
                valFleet.Enabled = false;
                lblOhTitle.Visible = true;
                btnOrganizationHierarchyNodeCode.Visible = true;
                hidOrganizationHierarchyPath.Value = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, DefaultOrganizationHierarchyNodeCode);
            }
         
            if (!Page.IsPostBack)
            {
                CboFleet_Fill();
                this.tblLandmarks.Visible = false;   
                this.tblCoordinates.Visible = false;

                if (LoadVehiclesBasedOn == "hierarchy")
                {
                    hidOrganizationHierarchyFleetId.Value = DefaultOrganizationHierarchyFleetId.ToString();
                    refillCboVehicle();
                }
            }
        }
        protected void lstAddOptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.tblStreet.Visible = false;
            this.tblCoordinates.Visible = false;
            this.tblLandmarks.Visible = false;
   
            if (this.lstAddOptions.SelectedItem.Value == "0")
                this.tblStreet.Visible = true;
            else if (this.lstAddOptions.SelectedItem.Value == "1")
                this.tblCoordinates.Visible = true;
            else if (this.lstAddOptions.SelectedItem.Value == "2")
            {
                tblLandmarks.Visible = true;
                CboLandmarks_Fill();
            }
        }
        protected void cmdSaveLandmark_Click(object sender, EventArgs e)
        {
            if (this.lstAddOptions.SelectedItem.Value == "0")
            {

                //Get Coordinates by street address
                string xml = "";
                
                DataSet ds = new DataSet();
                //strAddress = this.txtStreet.Text + "|" + this.txtCity.Text + "|" + this.txtState.Text + "|" + this.cboCountry.SelectedItem.Value;

                ////strAddress = this.txtStreet.Text + "|" + this.txtCity.Text + ", " + this.txtState.Text + ", " + this.cboCountry.SelectedItem.Value;

                ////// create ClientMapProxy only for geocoding
                ////VLF.MAP.ClientMapProxy geoMap = new VLF.MAP.ClientMapProxy(sn.User.GeoCodeEngine);
                ////if (geoMap == null)
                ////{
                ////    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Can't create map " + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                ////    return;
                ////}
                ////xml = geoMap.GetAddressMatches(strAddress);

                ////if (xml != "")
                ////{
                ////    ds = new DataSet();
                ////    ds.ReadXml(new StringReader(xml));
                ////    this.dgAddress.DataSource = ds;
                ////    this.dgAddress.DataBind();
                ////    this.lblMessage.Text = "Please select an Address!";  
                ////}
                ////else
                ////{
                ////    this.lblMessage.Text = "Address not found";  
                ////    this.dgAddress.DataSource = null;
                ////    this.dgAddress.DataBind();
                ////    return;
                ////}
                


            string strAddress = "";
            string resolvedAddress = "";
            clsMap mp = new clsMap();
            strAddress = this.txtStreet.Text + "," + this.txtCity.Text + "," + this.txtState.Text + "," + this.cboCountry.SelectedItem.Value + "|;";
            double X = 0; double Y = 0;
            mp.ResolveCooridnatesByAddressTelogis(strAddress, ref X, ref Y, ref resolvedAddress);
            ViewState["streetAddress"] = resolvedAddress;
                
            }
        }

      
        protected void dgAddress_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.txtLongitude.Text = dgAddress.SelectedItem.Cells[2].Text;
            this.txtLatitude.Text = dgAddress.SelectedItem.Cells[1].Text;
            ViewState["streetAddress"] = dgAddress.SelectedItem.Cells[0].Text; 

        }

        protected void cmdSent_Click(object sender, EventArgs e)
        {
            try
            {

            string strLandmark = "";
            strAddress = "";
            if (this.lstAddOptions.SelectedItem.Value == "2")// Location by Landmark
            {
                strLandmark = this.cboLandmarks.SelectedItem.Text;
                DataRow[] drArr = sn.DsLandMarks.Tables[0].Select("LandmarkName='" + this.cboLandmarks.SelectedItem.Value.Replace("'","''") +"'");
                if (drArr == null || drArr.Length == 0)
                {
                    this.lblMessage.Text = "Invalid landmark";
                    return; 
                }

                this.txtLatitude.Text = drArr[0]["Latitude"].ToString();
                this.txtLongitude.Text = drArr[0]["Longitude"].ToString();
            }
            else if (this.lstAddOptions.SelectedItem.Value == "0")// Location by Address
            {

                string resolvedAddress = "";
                clsMap mp = new clsMap();
                strAddress = this.txtStreet.Text + "," + this.txtCity.Text + "," + this.txtState.Text + "," + this.cboCountry.SelectedItem.Value + "|;";
                double X = 0; double Y = 0;
                mp.ResolveCooridnatesByAddressTelogis(strAddress, ref X, ref Y, ref resolvedAddress);
                ViewState["streetAddress"] = resolvedAddress;
                this.txtLatitude.Text=Y.ToString();
                this.txtLongitude.Text = X.ToString();  

                if  (ViewState["streetAddress"]==null || ViewState["streetAddress"].ToString().TrimEnd() =="")
                {
                        this.lblMessage.Text = "Invalid Address";
                        return; 
                }

                strAddress = ViewState["streetAddress"].ToString(); 
            }

            if (this.txtLatitude.Text == "0" || this.txtLongitude.Text == "")
            {
                this.lblMessage.Text = "Please select location";
                return; 
            }

            //string paramList = VLF.CLS.Util.MakePair("Txt", this.txtMessageBody.Text);
            //paramList += VLF.CLS.Util.MakePair("LAT",this.txtY.Text );
            //paramList += VLF.CLS.Util.MakePair("LON", this.txtX.Text);


            //string paramList = VLF.CLS.Util.MakePair(GarminFMI.JKeyWords.TXT.ToString(), this.txtMessageBody.Text);
            //paramList += VLF.CLS.Util.MakePair(GarminFMI.JKeyWords.RETRYINTERVAL.ToString(), "5");
            //paramList += VLF.CLS.Util.MakePair(GarminFMI.JKeyWords.LIFETIME.ToString(), "300");



            

            using (DBGarmin.Garmin garmin = new DBGarmin.Garmin())
            {
                if (Convert.ToInt32(this.cboVehicle.SelectedItem.Value) != 0)
                {
                    if (objUtil.ErrCheck(garmin.SendLocationMessage(sn.UserID, sn.SecId, Convert.ToInt32(cboVehicle.SelectedItem.Value), Convert.ToInt32(VLF.DAS.DB.GarminMessageType.A603_Stop_Data_Type), this.txtMessageBody.Text, Convert.ToDouble(this.txtLatitude.Text), Convert.ToDouble(this.txtLongitude.Text), strAddress, strLandmark), false))
                        if (objUtil.ErrCheck(garmin.SendLocationMessage(sn.UserID, sn.SecId, Convert.ToInt32(cboVehicle.SelectedItem.Value), Convert.ToInt32(VLF.DAS.DB.GarminMessageType.A603_Stop_Data_Type), this.txtMessageBody.Text, Convert.ToDouble(this.txtLatitude.Text), Convert.ToDouble(this.txtLongitude.Text), strAddress, strLandmark), true))
                        {
                            this.lblMessage.Text = "Send location failed";
                            return;
                        }
                }
                else
                {
                    foreach (DataRow dr in sn.Message.DsVehicles.Tables[0].Rows)
                    {
                        if (objUtil.ErrCheck(garmin.SendLocationMessage(sn.UserID, sn.SecId, Convert.ToInt32(dr["boxId"]), Convert.ToInt32(VLF.DAS.DB.GarminMessageType.A603_Stop_Data_Type), this.txtMessageBody.Text, Convert.ToDouble(this.txtLatitude.Text), Convert.ToDouble(this.txtLongitude.Text), strAddress, strLandmark), false))
                            if (objUtil.ErrCheck(garmin.SendLocationMessage(sn.UserID, sn.SecId, Convert.ToInt32(dr["boxId"]), Convert.ToInt32(VLF.DAS.DB.GarminMessageType.A603_Stop_Data_Type), this.txtMessageBody.Text, Convert.ToDouble(this.txtLatitude.Text), Convert.ToDouble(this.txtLongitude.Text), strAddress, strLandmark), true))
                            {
                                continue;
                            }
                    }

                }
            }

            //this.lblMessage.Text = "Location queued successfully";
            //this.cmdSent.Enabled = false;    
            Response.Write("<script language='javascript'>window.close()</script>");

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                Response.Write("<script language='javascript'>window.close()</script>");
            }

            
        }


        private void CboLandmarks_Fill()
        {
            try
            {
                //StringReader strrXML = null;
                //DataSet dsLandmarks = new DataSet();

                //string xml = "";
                //ServerDBOrganization.DBOrganization dbo = new ServerDBOrganization.DBOrganization();

                //if (objUtil.ErrCheck(dbo.GetOrganizationLandmarksXMLByOrganizationId(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), false))
                //    if (objUtil.ErrCheck(dbo.GetOrganizationLandmarksXMLByOrganizationId(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), true))
                //    {
                //        return;
                //    }

                //if (xml == "")
                //{
                //    this.cboLandmarks.DataSource = null;
                //    this.cboLandmarks.DataBind();
                //    return;
                //}


                //strrXML = new StringReader(xml);
                //dsLandmarks.ReadXml(strrXML);
                //this.cboLandmarks.DataSource = dsLandmarks;
                this.cboLandmarks.DataSource = sn.DsLandMarks; 
                this.cboLandmarks.DataBind();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }



        private void CboFleet_Fill()
        {
            try
            {

                DataSet dsFleets = new DataSet();
                dsFleets = sn.User.GetUserFleets(sn);
                cboFleet.DataSource = dsFleets;
                cboFleet.DataBind();

                cboFleet.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("valFleetResource1.ErrorMessage"), "-1"));


                
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }


        }


        private void CboVehicle_Fill(int fleetId)
        {
            try
            {
                DataSet dsVehicle;
                dsVehicle = new DataSet();

                StringReader strrXML = null;


                string xml = "";
                ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();

                if (objUtil.ErrCheck(dbf.GetVehiclesInfoXMLByFleetId(sn.UserID, sn.SecId, fleetId, ref xml), false))
                    if (objUtil.ErrCheck(dbf.GetVehiclesInfoXMLByFleetId(sn.UserID, sn.SecId, fleetId, ref xml), true))
                    {
                        cboVehicle.Items.Clear();
                        cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0"), "0"));
                        return;
                    }
                if (xml == "")
                {
                    cboVehicle.Items.Clear();
                    cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0"), "0"));
                    return;
                }

                strrXML = new StringReader(xml);
                dsVehicle.ReadXml(strrXML);

                cboVehicle.DataSource = dsVehicle;
                cboVehicle.DataBind();
                sn.Message.DsVehicles = dsVehicle;

                //cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0"), "0"));
                cboVehicle.Items.Insert(0, new ListItem("Entire Fleet", "0"));

            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }

        }

        protected void cboFleet_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (Convert.ToInt32(cboFleet.SelectedItem.Value) != -1)
            //{
            //    this.cboVehicle.Visible = true;
            //    this.lblVehicleName.Visible = true;
            //    CboVehicle_Fill(Convert.ToInt32(cboFleet.SelectedItem.Value));
            //}
            refillCboVehicle();
        }

        private void refillCboVehicle()
        {
            int fleetId = -1;
            if (LoadVehiclesBasedOn == "hierarchy")
            {
                int.TryParse(hidOrganizationHierarchyFleetId.Value.ToString(), out fleetId);
            }
            else
            {
                fleetId = Convert.ToInt32(cboFleet.SelectedItem.Value);
            }

            if (fleetId != -1)
            {
                CboVehicle_Fill(fleetId);
                this.lblVehicleName.Visible = true;
                this.cboVehicle.Visible = true;
            }
            else
                this.cboVehicle.Items.Clear();
        }

        protected void hidOrganizationHierarchyPostBack_Click(object sender, EventArgs e)
        {
            refillCboVehicle();
        }
       
}
}
