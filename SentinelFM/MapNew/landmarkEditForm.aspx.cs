using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;
using VLF.PATCH.Logic;
using System.Globalization;

namespace SentinelFM
{
    public partial class MapNew_landmarkEditForm : System.Web.UI.Page
    {
        public bool Show_Header = true;
        public LandmarkMeta LANDMARK;
        public LandmarkMeta_NewTZ LANDMARK_NewTZ;
        public string GeoType = "landmark";
        public string GeoAssetName;
        public bool ISNEW;

        public SentinelFMSession sn = null;
        public bool Assigned = false;
        public string ALL_SERVICES = string.Empty;
        public string APPLIED_SERVICES = string.Empty;
        public string HELPTIP = "Tip: click to view, double click to (un)apply the services.";
        public bool ShowAssignToFleet = false;

        public int landmarkId;

        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
        private PatchServices _ps = null;

        public bool ShowCallTimer = false;
        public bool ShowSearchVehicleInLandmark = false;

        //Resorce Reader
        public string Res_btnCancel = string.Empty;
        public string Res_btnDelete = string.Empty;
        public string Res_btnSave = string.Empty;
        public string Res_PageTitleText = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            _ps = new VLF.PATCH.Logic.PatchServices(sConnectionString);

            sn = (SentinelFMSession)Session["SentinelFMSession"];
            if (sn.DsLandMarks == null)
                DgLandmarks_Fill(sn);

            clsUtility objUtil;
            objUtil = new clsUtility(sn);

            string displayExtendedAttributesAtNewmapLandmark = ConfigurationManager.AppSettings["displayExtendedAttributesAtNewmapLandmark"] ?? "false";
            
            if (displayExtendedAttributesAtNewmapLandmark.ToLower() == "true")
                lnkExtendedAttributes.Visible = true;
            else
                lnkExtendedAttributes.Visible = false;

            ShowAssignToFleet = clsPermission.FeaturePermissionCheck(sn, "LandmarkFleetAssignment");
            //if ((sn.User.UserGroupId == 1 || sn.User.UserGroupId == 2 || sn.User.UserGroupId == 7) && (sn.User.OrganizationId == 480 || sn.User.OrganizationId == 952 || sn.User.OrganizationId == 999952 || sn.User.OrganizationId == 999954)) //Hgi and Security Administrator user 
            //{
            //    ShowAssignToFleet = true;                
            //}

            if ((sn.User.UserGroupId == 1 || sn.User.UserGroupId == 2 || sn.User.UserGroupId == 7 || sn.UserID == 11967) && (sn.User.OrganizationId == 480 || sn.User.OrganizationId == 952 || sn.User.OrganizationId == 999952 || sn.User.OrganizationId == 999954)) //Hgi and Security Administrator user 
            {
                //enableTimer = true;
                //trCallTimer.Visible = true;
                ShowCallTimer = true;
                if (!Page.IsPostBack)
                    CboServices_Fill();
            }

            //if (sn.UserID == 11967)
            //{
            //    ShowAssignToFleet = true;  
            //}



            if (Request["showheader"] == "false")
                Show_Header = false;

            string landmarkName = string.Empty;
            if (Request["landmarkName"] != "" && Request["landmarkName"] != null)
            {
                landmarkName = Request["landmarkName"];
            }

            double lon = 0;
            double lat = 0;
            if (Request["lon"] != "" && Request["lon"] != null)
            {
                lon = double.Parse(Request["lon"].ToString());
            }
            if (Request["lat"] != "" && Request["lat"] != null)
            {
                lat = double.Parse(Request["lat"].ToString());
            }

            geoassettype.Value = "landmark";
            
            cboTimeZone_Fill();
            cboRuleName_Fill();

            ddlCategory_Fill();

            DataSet _as = null;
            landmarkId = 0;

            helptip.Value = HELPTIP;
            // Changes for TimeZone Feature start
            LANDMARK_NewTZ = new LandmarkMeta_NewTZ(); // Changes for TimeZone Feature end
            foreach (DataRow dr in sn.DsLandMarks.Tables[0].Rows)
            {
                if (landmarkName.TrimEnd() == dr["LandmarkName"].ToString().TrimEnd() && landmarkName != string.Empty)
                {
                    /*LANDMARK.LandmarkId = Convert.ToInt32(dr["LandmarkId"].ToString().TrimEnd());
                    LANDMARK.OrganizationId = Convert.ToInt32(dr["OrganizationId"].ToString().TrimEnd());
                    LANDMARK.LandmarkName = dr["LandmarkName"].ToString().TrimEnd();
                    LANDMARK.Latitude = Convert.ToDouble(dr["Latitude"].ToString().TrimEnd());
                    LANDMARK.Longitude = Convert.ToDouble(dr["Longitude"].ToString().TrimEnd());
                    LANDMARK.Description = dr["Description"].ToString().TrimEnd();
                    LANDMARK.ContactPersonName = dr["ContactPersonName"].ToString().TrimEnd();
                    LANDMARK.ContactPhoneNum = dr["ContactPhoneNum"].ToString().TrimEnd();
                    LANDMARK.Radius = Convert.ToInt32(dr["Radius"].ToString().TrimEnd());
                    LANDMARK.Email = dr["Email"].ToString().TrimEnd();
                    LANDMARK.Phone = dr["Phone"].ToString().TrimEnd();
                    LANDMARK.TimeZone = Convert.ToInt32(dr["TimeZone"].ToString().TrimEnd());
                    LANDMARK.DayLightSaving = Convert.ToBoolean(dr["DayLightSaving"].ToString().TrimEnd());
                    LANDMARK.AutoAdjustDayLightSaving = Convert.ToBoolean(dr["AutoAdjustDayLightSaving"].ToString().TrimEnd());
                    LANDMARK.StreetAddress = dr["StreetAddress"].ToString().TrimEnd();
                    LANDMARK.IsPublic = Convert.ToBoolean(dr["Public"].ToString().TrimEnd());*/
                    LandmarkNameLabel.Text = landmarkName;
                    GeoAssetName = landmarkName.Replace("\"", "\\\"");
                    geoassetname.Value = landmarkName;
                    //FormTitle.Text = "Edit Landmark";
                    if (double.Parse(dr["Longitude"].ToString().TrimEnd()) == 0 && lon != 0)
                        txtX.Value = lon.ToString();
                    else
                        txtX.Value = dr["Longitude"].ToString().TrimEnd();

                    if (double.Parse(dr["Latitude"].ToString().TrimEnd()) == 0 && lat != 0)
                        txtY.Value = lat.ToString();
                    else
                        txtY.Value = dr["Latitude"].ToString().TrimEnd();
                    oldLandmarkName.Value = landmarkName; // Replace landmark name: ' -> &#39; " -> &quot;
                    txtLandmarkName.Value = landmarkName;
                    txtContactName.Value = dr["ContactPersonName"].ToString().TrimEnd();
                    txtLandmarkDesc.Value = dr["Description"].ToString().TrimEnd();
                    txtPhone.Value = dr["ContactPhoneNum"].ToString().TrimEnd();
                    if (Convert.ToInt32(dr["Radius"].ToString().TrimEnd()) > 0)
                    {
                        radiusRow.Visible = true;
                        txtRadius.Value = dr["Radius"].ToString().TrimEnd();
                    }
                    lstPublicPrivate.SelectedValue = (Convert.ToBoolean(dr["Public"].ToString().TrimEnd()) ? 1 : 0).ToString();
                    txtEmail.Value = dr["Email"].ToString().TrimEnd();
                    txtPhoneSMS.Value = dr["Phone"].ToString().TrimEnd();
                    this.cboTimeZone.SelectedIndex = -1;
                    for (int i = 0; i < cboTimeZone.Items.Count; i++)
                    {
                        if (Convert.ToSingle(cboTimeZone.Items[i].Value.TrimEnd()) == Convert.ToSingle(dr["TimeZone"].ToString().TrimEnd()))
                        {
                            cboTimeZone.SelectedIndex = i;
                            break;
                        }
                    }
                    chkDayLight.Checked = Convert.ToBoolean(dr["AutoAdjustDayLightSaving"].ToString().TrimEnd());

                    landmarkId = Convert.ToInt32(dr["lid"].ToString().TrimEnd());

                    _as = _ps.GetAppliedServicesByLandmarkId(sn.User.OrganizationId, landmarkId);
                    string ids = string.Empty;
                    foreach (DataRow r in _as.Tables[0].Rows)
                    {
                        APPLIED_SERVICES += string.Format(@"<li id='{0}' name='{4}' rel='{1}' recepients='{2}' subjects='{3}' onclick='onServiceClick(this);' ondblclick='onServiceDblclick(this);'>{4}</li>",
                            r["ServiceConfigId"].ToString(), r["RulesApplied"].ToString(), (r["Recepients"] ?? string.Empty).ToString(), (r["Subjects"] ?? string.Empty).ToString(), HttpUtility.UrlEncode(r["ServiceName"].ToString()));
                        ids += r["ServiceConfigId"].ToString() + ",";
                    }
                    orginalAppliedServices.Value = ids.TrimEnd(',');
                    break;
                }
            }

            if (landmarkId > 0)
            {
                setDDLCurrentLandmarkCategoryValue(landmarkId);
                _as = _ps.GetAllAvailableServices(sn.User.OrganizationId, landmarkId);
            }
            else
            { 
                _as = _ps.GetAllServices();
            }

            foreach (DataRow r in _as.Tables[0].Rows)
            {
                ALL_SERVICES += string.Format(@"<li id='{0}' name='{1}' rel='{2}' recepients='' subjects='' onclick='onServiceClick(this);' ondblclick='onServiceDblclick(this);'>{1}</li>",
                    r["ServiceConfigId"].ToString(), HttpUtility.UrlEncode(r["ServiceName"].ToString()), r["RulesApplied"].ToString());
            }

            CheckBoxFleet_Fill();

            ALL_SERVICES = "<ul>" + ALL_SERVICES + "</ul>";

            string xml = "";
            using (ServerDBOrganization.DBOrganization dbOrganization = new ServerDBOrganization.DBOrganization())
            {
                if (objUtil.ErrCheck(dbOrganization.GetOrganizationSettings(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), false))
                    if (objUtil.ErrCheck(dbOrganization.GetOrganizationSettings(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), true))
                    {
                        return;
                    }
                if (xml != "")
                {
                    DataSet dsPref = new DataSet();
                    System.IO.StringReader strrXML = new System.IO.StringReader(xml);
                    dsPref.ReadXml(strrXML);
                    foreach (DataRow rowItem in dsPref.Tables[0].Rows)
                    {

                        if (Convert.ToInt16(rowItem["OrgPreferenceId"]) == 27)// 27 SearchVehiclesInLandmark
                        {
                            if (rowItem["PreferenceValue"].ToString().Trim() == "1")
                            {
                                ShowSearchVehicleInLandmark = true;
                            }
                        }

                    }
                }
            }

            //REsource Reading/binding
            Res_btnCancel = (string)base.GetLocalResourceObject("btnCancel.Text");
            Res_btnDelete = (string)base.GetLocalResourceObject("btnDelete.Text");
            Res_btnSave = (string)base.GetLocalResourceObject("btnSave.Text");
            Res_PageTitleText = (string)base.GetLocalResourceObject("PageTitle.Text");
        }

        // Changes for TimeZone Feature start
        private void DgLandmarks_Fill_NewTZ(SentinelFMSession sn)
        {
            try
            {
                sn.Landmark.DgLandmarks_Fill_NewTZ(sn);
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " From:landmarkEditForm"));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);

            }
        }


        // Changes for TimeZone Feature end

        private void DgLandmarks_Fill(SentinelFMSession sn)
        {
            try
            {
                sn.Landmark.DgLandmarks_Fill(sn);
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " From:landmarkEditForm"));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);

            }
        }
        // Changes for TimeZone Feature start

        public class LandmarkMeta_NewTZ
        {
            public int LandmarkId;
            public int OrganizationId;
            public string LandmarkName;
            public double Latitude;
            public double Longitude;
            public string Description;
            public string ContactPersonName;
            public string ContactPhoneNum;
            public int Radius;
            public string Email;
            public string Phone;
            public float? TimeZone;
            public bool DayLightSaving;
            public bool AutoAdjustDayLightSaving;
            public string StreetAddress;
            public bool IsPublic;
        }

        // Changes for TimeZone Feature end

        public class LandmarkMeta
        {
            public int LandmarkId;
            public int OrganizationId;
            public string LandmarkName;
            public double Latitude;
            public double Longitude;
            public string Description;
            public string ContactPersonName;
            public string ContactPhoneNum;
            public int Radius;
            public string Email;
            public string Phone;
            public int? TimeZone;
            public bool DayLightSaving;
            public bool AutoAdjustDayLightSaving;
            public string StreetAddress;
            public bool IsPublic;
        }
       


        private void cboTimeZone_Fill()
        {
            try
            {
                cboTimeZone.SelectedIndex = -1;
                cboTimeZone.DataSource = null;

                DataTable tblTimeZone = new DataTable();
                tblTimeZone.Columns.Add("TimeZoneId", typeof(float));
                tblTimeZone.Columns.Add("TimeZoneName", typeof(string));

                object[] objRow;
                for (int i = -12; i < 14; i++)
                {

                    objRow = new object[2];
                    objRow[0] = i;
                    if (i != 0)
                    {
                        if (i < 0)
                        {
                            objRow[1] = "GMT" + i.ToString();
                        }
                        else
                        {
                            objRow[1] = "GMT+" + i.ToString();
                        }
                    }
                    else
                    {
                        objRow[1] = "GMT";
                    }

                    tblTimeZone.Rows.Add(objRow);
                    this.cboTimeZone.DataSource = tblTimeZone;
                    this.cboTimeZone.DataBind();
                }

                //  Changes For TimeZone Feature start
                //  Adding New Time Zone GMT - 3:30 (-3.5)  | NewFoundLand

                objRow = new object[2];
                DataRow dtRow = tblTimeZone.NewRow();
                dtRow[0] = -3.5;
                dtRow[1] = "GMT-3:30";
                tblTimeZone.Rows.InsertAt(dtRow, 9);
                this.cboTimeZone.DataSource = tblTimeZone;
                this.cboTimeZone.DataBind();
                //  Changes For TimeZone Feature end
                cboTimeZone.SelectedIndex = cboTimeZone.Items.IndexOf(cboTimeZone.Items.FindByText(string.Format("GMT{0}", sn.User.TimeZone)));
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }

        }

        private void setDDLCurrentLandmarkCategoryValue(long landmarkId)
        {
            try
            {
                VLF.DAS.Logic.Organization org = new VLF.DAS.Logic.Organization(this.sConnectionString);
                DataSet dsOrganization = org.GetLandmarkCategory(sn.UserID, sn.User.OrganizationId, landmarkId);
                DataTable dsLandmarkCategory = dsOrganization.Tables["LandmarkCategory"];
                string landmarkCategoryMappingId = "0";
                string landmarkCategoryId = "0";
                if (dsLandmarkCategory.Rows.Count > 0)
                {
                    DataRow oneRow = dsLandmarkCategory.Rows[0];
                    landmarkCategoryMappingId = oneRow["LandmarkCategoryMappingId"].ToString();
                    landmarkCategoryId = oneRow["LandmarkCategoryId"].ToString();
                }

                this.ddlCategory.SelectedValue = landmarkCategoryId;
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }

        }

        private void ddlCategory_Fill()
        {
            try
            {
                VLF.DAS.Logic.Organization org = new VLF.DAS.Logic.Organization(this.sConnectionString);
                DataSet dsOrganization = org.ListOrganizationLandmarkCategory(sn.UserID, sn.User.OrganizationId);
                DataTable dsLandmarkCategory = dsOrganization.Tables["LandmarkCategory"];

                ListItem oneItem = null;
                this.ddlCategory.Items.Clear();
                this.ddlCategory.Items.Add(new ListItem("Select a category", "0"));
                foreach (DataRow oneRow in dsLandmarkCategory.Rows)
                {
                    //DomainMetadataId], [MetadataValue
                    oneItem = new ListItem(oneRow["MetadataValue"].ToString(), oneRow["DomainMetadataId"].ToString());
                    this.ddlCategory.Items.Add(oneItem);
                }

                //Dictionary<string, string> listCategory = new Dictionary<string, string>();
                //listCategory.Add("100", "Generic");
                //listCategory.Add("101", "Parking Lots");
                //listCategory.Add("102", "Garages");
                //listCategory.Add("103", "Service Areas");

                //ListItem oneItem = null;
                //this.ddlCategory.Items.Clear();
                //foreach (KeyValuePair<string, string> oneCategory in listCategory)
                //{
                //    oneItem = new ListItem(oneCategory.Value, oneCategory.Key);
                //    this.ddlCategory.Items.Add(oneItem);

                //}

            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }

        }

        private void cboRuleName_Fill()
        {
            try
            {
                cboRuleName.SelectedIndex = -1;
                cboRuleName.DataSource = null;

                DataTable tblRule = _ps.GetAllRules().Tables[0];
                this.cboRuleName.DataSource = tblRule;
                this.cboRuleName.DataBind();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }

        }

        private void CheckBoxFleet_Fill()
        {
            try
            {
                DataSet dsFleets = new DataSet();
                dsFleets = sn.User.GetUserFleets(sn);
                CheckBoxFleet.DataSource = dsFleets;
                CheckBoxFleet.DataBind();

                //cboFleet.Items.Insert(0, new ListItem((string)this.GetLocalResourceObject("cboFleet_Item_0"), "-1"));


                foreach (ListItem li in CheckBoxFleet.Items)
                {
                    if (li.Text == "All Vehicles" || li.Text == "Tous les véhicules" || li.Text == "Tous les vehicules")
                    {
                        CheckBoxFleet.Items.Remove(li);
                        break;
                    }

                }
            }

            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                //RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }

        }

        private void CboServices_Fill()
        {
            try
            {
                DataSet dsServices = new DataSet();
                dsServices = _ps.GetHardcodedCallTimerServices();
                cboServices.DataSource = dsServices;
                cboServices.DataBind();

                //cboFleet.Items.Insert(0, new ListItem((string)this.GetLocalResourceObject("cboFleet_Item_0"), "-1"));
                cboServices.Items.Insert(0, new ListItem("Select a service", "-1"));

                setDefaultCallTimer();


            }

            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));                
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }

        }

        private void setDefaultCallTimer()
        {
            string landmarkName = string.Empty;
            if (Request["landmarkName"] != "" && Request["landmarkName"] != null)
            {
                landmarkName = Request["landmarkName"];
            }

            VLF.PATCH.Logic.PatchLandmark _lankmark = new VLF.PATCH.Logic.PatchLandmark(sConnectionString);
            int landmarkId = _lankmark.GetLandmarkIdByLandmarkName(sn.User.OrganizationId, landmarkName);
            int serviceId = _ps.GetHardcodedTimerServiceId(sn.User.OrganizationId, landmarkId);
            if (serviceId > 0)
            {
                cboServices.SelectedValue = serviceId.ToString();
            }
            else
            {
                cboServices.SelectedValue = "-1";
            }
        }

        protected override void InitializeCulture()
        {
            SentinelFMSession snMain = (SentinelFMSession)Session["SentinelFMSession"];
            if (snMain == null || snMain.User == null || String.IsNullOrEmpty(snMain.UserName))
            {
                return;
            }

            if (snMain.SelectedLanguage != null)
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new
                    CultureInfo(snMain.SelectedLanguage);

                System.Threading.Thread.CurrentThread.CurrentCulture = new
                 CultureInfo("en-US");

                base.InitializeCulture();
            }
            else
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new
                   CultureInfo("en-US");

                base.InitializeCulture();
            }
        }
        
    }
}