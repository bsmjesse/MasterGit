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
using System.IO;
using VLF.MAP;
using VLF.CLS.Interfaces;
using System.Globalization;
using VLF.CLS.Def;
using System.Configuration;

namespace SentinelFM
{
    /// <summary>
    /// Summary description for frmGeoZone.
    /// </summary>
    public partial class frmGeoZones : SentinelFMBasePage
    {
        protected System.Web.UI.WebControls.Label lblY;
        protected System.Web.UI.WebControls.Label lblX;
        protected System.Web.UI.WebControls.Label lblUnit;
        protected DataSet dsSeverity = new DataSet();
        protected DataSet dsGeoZoneDirections = new DataSet();
        
        protected System.Web.UI.WebControls.TextBox txtWidth;
        protected System.Web.UI.WebControls.TextBox txtHeight;
        protected System.Web.UI.WebControls.Label lblUnit1;
        
        protected ServerDBOrganization.DBOrganization dbo;
        protected System.Web.UI.WebControls.RangeValidator valWidth;
        protected System.Web.UI.WebControls.RangeValidator valHeight;
        protected System.Web.UI.WebControls.RadioButton optByAddress;
        protected System.Web.UI.WebControls.RadioButton optByLandmark;
        protected System.Web.UI.HtmlControls.HtmlTable tblGeoZoneCoord;
        protected System.Web.UI.WebControls.RadioButton optByCoordinates;
        protected System.Web.UI.WebControls.TextBox txtStreet;
        protected System.Web.UI.WebControls.TextBox txtCity;
        protected System.Web.UI.WebControls.TextBox txtState;
        protected System.Web.UI.HtmlControls.HtmlTable tblGeoZoneAddr;
        protected System.Web.UI.WebControls.TextBox txtX;
        protected System.Web.UI.WebControls.TextBox txtY;
        protected System.Web.UI.WebControls.DropDownList cboCountry;
        string confirm;
        public string GeozoneMap="";
        public string ViewGeozoneMap = "";

        public bool enableAssignment = false;

        public Int16 GeoZoneId = 0;

        public bool isGeoZoneUpdatableByUser = true;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                //Clear IIS cache
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetExpires(DateTime.Now);


                if ((sn == null) || (sn.UserName == ""))
                {
                    RedirectToLogin();
                    return;
                }

                if ((sn.User.UserGroupId == 1 || sn.User.UserGroupId == 2 || sn.User.UserGroupId == 7) && (sn.User.OrganizationId == 480 || sn.User.OrganizationId == 952 || sn.User.OrganizationId == 999952 || sn.User.OrganizationId == 999954)) //Hgi and Security Administrator user 
                {
                    enableAssignment = true;
                    trAssignment.Visible = true;
                    if (!Page.IsPostBack)
                        CboFleet_Fill();
                }

                if (sn.User.UserMapType == (Int16)clsMap.MapEngineType.VirtualEarth)
                {
                    //GeozoneMap = "frmLandmarkGeoZoneVE.aspx?FormName=Geozone";
                    //ViewGeozoneMap = "frmLandmarkGeoZoneVE.aspx";

                    GeozoneMap = "../MapVE/VEGeoZone.aspx?FormName=Geozone";
                    ViewGeozoneMap = "../MapVE/VELandmarks_GeoZones.aspx";
                }
                else
                {
                    //GeozoneMap = "frmMap.aspx?FormName=Geozone";
                    //ViewGeozoneMap = "frmViewGeoZone.aspx";

                    //GeozoneMap = "../MapNew/frmDrawLandmarkGeozone.aspx?FormName=Geozone";
                    if (sn.GeoZone.EditMode)
                    {
                        dbo = new ServerDBOrganization.DBOrganization();
                        dbo.IsGeoZoneUpdatableByUser(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt16(this.lblGeoZoneId.Text), ref isGeoZoneUpdatableByUser);
                        //DisableOrLockControls(isGeoZoneUpdatableByUser);
                        GeozoneMap = "../MapNew/frmDrawLandmarkGeozone.aspx?FormName=Geozone&ShowControl=" + isGeoZoneUpdatableByUser.ToString();
                    }
                    ViewGeozoneMap = "../MapNew/frmViewLandmarkGeozone.aspx";
                }


                if (!Page.IsPostBack)
                {
                    LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref frmGeozoneForm, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);

                    if (sn.User.SmsSupport)
                        txtPhone.Enabled = true;   

                    GuiSecurity(this);
                    this.cboLandmarks.Visible = false;
                    this.tblGeoZoneMap.Visible = true;

                    this.tblGeoZoneAdd.Visible = false;
                    ViewState["ConfirmDelete"] = "0";
                    this.tblGeoZoneCoordinates.Visible = false;  

                    CboLandmarks_Fill();
                    DsSeverity_Fill();
                    DsGeoZoneDirection_Fill();
                    DgGeoZone_Fill_NewTZ();
                    DisableOptSelection();

                    if ((sn.GeoZone.AddMode) || (sn.GeoZone.EditMode))
                    {

                        this.tblGeoZoneAdd.Visible = true;
                        this.tblGeoZones.Visible = false;

                        cboDirection.SelectedIndex = -1;
                        cboDirection.Items.FindByValue(sn.GeoZone.Direction.ToString()).Selected = true;

                        cboGeoZoneSeverity.SelectedIndex = -1;
                        cboGeoZoneSeverity.Items.FindByValue(sn.GeoZone.Severity.ToString()).Selected = true;
                    }
                    else
                    {
                        this.tblGeoZoneAdd.Visible = false;
                        this.tblGeoZones.Visible = true;
                    }

                }
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }

        private void DsSeverity_Fill()
        {
            try
            {
                StringReader strrXML = null;
                dsSeverity.Tables.Clear();
                
                string xml = "";
                ServerAlarms.Alarms dba = new ServerAlarms.Alarms();



                if (objUtil.ErrCheck(dba.GetAlarmSeverityXMLByLang(sn.UserID, sn.SecId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                    if (objUtil.ErrCheck(dba.GetAlarmSeverityXMLByLang(sn.UserID, sn.SecId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                    {
                        return;
                    }

                if (xml == "")
                {
                    return;
                }


                strrXML = new StringReader(xml);
                dsSeverity.ReadXml(strrXML);

                this.cboGeoZoneSeverity.DataSource = dsSeverity;
                this.cboGeoZoneSeverity.DataBind();
            }

            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }

        public int GetSeverity(int SeverityId)
        {
            try
            {
                DropDownList cboSeverity = new DropDownList();
                cboSeverity.DataValueField = "SeverityId";
                cboSeverity.DataTextField = "SeverityName";
                DsSeverity_Fill();
                cboSeverity.DataSource = dsSeverity;
                cboSeverity.DataBind();

                cboSeverity.SelectedIndex = -1;
                cboSeverity.Items.FindByValue(SeverityId.ToString()).Selected = true;
                return cboSeverity.SelectedIndex;
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                return 0;
            }

        }

        private void DsGeoZoneDirection_Fill()
        {
            try
            {
                DataTable tblTypes = dsGeoZoneDirections.Tables.Add("Types");
                tblTypes.Columns.Add("DirectionId", typeof(short));
                tblTypes.Columns.Add("DirectionName", typeof(string));

                Array enmArr = Enum.GetValues(typeof(Enums.GeoZoneDirection));
                string TypeName;
                object[] objRow;
                string[] geoDirectionFrench = new string[4] { "Désactivé", "Entrant", "Sortant", "EntrantSortant" };
                Int16 i = 0;
                foreach (Enums.GeoZoneDirection ittr in enmArr)
                {
                    TypeName = Enum.GetName(typeof(Enums.GeoZoneDirection), ittr);
                    objRow = new object[2];
                    objRow[0] = Convert.ToInt16(ittr);
                    objRow[1] = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "en" ? TypeName : geoDirectionFrench[i];
                    dsGeoZoneDirections.Tables[0].Rows.Add(objRow);
                    i++;

                }

                this.cboDirection.DataSource = dsGeoZoneDirections;
                this.cboDirection.DataBind();

            }

            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }

        public int GetGeoZoneDirection(int DirectionId)
        {
            try
            {
                DropDownList cboGeoZoneDirection = new DropDownList();
                cboGeoZoneDirection.DataValueField = "DirectionId";
                cboGeoZoneDirection.DataTextField = "DirectionName";
                DsGeoZoneDirection_Fill();
                cboGeoZoneDirection.DataSource = dsGeoZoneDirections;
                cboGeoZoneDirection.DataBind();

                cboGeoZoneDirection.SelectedIndex = -1;
                cboGeoZoneDirection.Items.FindByValue(DirectionId.ToString()).Selected = true;
                return cboGeoZoneDirection.SelectedIndex;
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                return 0;
            }

        }

        private void CboLandmarks_Fill()
        {
            try
            {
                StringReader strrXML = null;
                DataSet dsLandmarks = new DataSet();
                
                string xml = "";
                dbo = new ServerDBOrganization.DBOrganization();

                if (objUtil.ErrCheck(dbo.GetOrganizationLandmarksXMLByOrganizationId(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), false))
                    if (objUtil.ErrCheck(dbo.GetOrganizationLandmarksXMLByOrganizationId(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), true))
                    {
                        return;
                    }

                if (xml == "")
                {
                    this.cboLandmarks.DataSource = null;
                    this.cboLandmarks.DataBind();
                    return;
                }


                strrXML = new StringReader(xml);
                dsLandmarks.ReadXml(strrXML);
                this.cboLandmarks.DataSource = dsLandmarks;
                this.cboLandmarks.DataBind();
                cboLandmarks.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboLandmarks_Item_0"), "-1"));
                sn.DsLandMarks = dsLandmarks;
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }

        // Changes for TimeZone Feature start

        private void DgGeoZone_Fill_NewTZ()
        {
            DataRow[] drCollections = null;
            StringReader strrXML = null;
            DataSet dsGeoZone = new DataSet();
            string xml = "";
            short enumId = 0;

            try
            {
                dbo = new ServerDBOrganization.DBOrganization();

                DataSet _geozones = null;
                string sConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                using (VLF.PATCH.Logic.PatchOrganizationGeozone pog = new VLF.PATCH.Logic.PatchOrganizationGeozone(sConnectionString))
                {
                    _geozones = pog.PatchGetOrganizationGeozonesWithStatus_NewTZ(sn.User.OrganizationId, sn.UserID);

                    if (VLF.CLS.Util.IsDataSetValid(_geozones))
                    {
                        xml = clsXmlUtil.GetXmlIncludingNull(_geozones);
                    }
                }

                if (xml == "")
                {
                    this.dgGeoZone.DataSource = null;
                    this.dgGeoZone.DataBind();
                    return;
                }

                strrXML = new StringReader(xml);
                dsGeoZone.ReadXml(strrXML);

                // Show DirectionName
                DataColumn dc = new DataColumn();
                dc.ColumnName = "DirectionName";
                dc.DataType = Type.GetType("System.String");
                dc.DefaultValue = "";

                DataTable dt = new DataTable();

                if (sn.User.ControlEnable(sn, 93))
                {
                    dt = dsGeoZone.Tables[0].Clone();

                    string filter = String.Format("Public=false AND CreateUserID={0}", sn.UserID.ToString());
                    drCollections = dsGeoZone.Tables[0].Select(filter);

                    if (drCollections != null && drCollections.Length > 0)
                    {
                        foreach (DataRow dr in drCollections)
                            dt.ImportRow(dr);
                    }

                    if (dt.Columns.IndexOf("DirectionName") == -1)
                    {
                        dt.Columns.Add(dc);

                        foreach (DataRow rowItem in dt.Rows)
                        {
                            enumId = Convert.ToInt16(rowItem["Type"]);
                            rowItem["DirectionName"] = Enum.GetName(typeof(Enums.GeoZoneDirection), (Enums.GeoZoneDirection)enumId);
                        }
                    }

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);
                    sn.GeoZone.DsGeoZone = ds;

                    this.dgGeoZone.DataSource = ds;
                    this.dgGeoZone.DataBind();
                }
                else
                {
                    if (dsGeoZone.Tables[0].Columns.IndexOf("DirectionName") == -1)
                    {
                        dsGeoZone.Tables[0].Columns.Add(dc);

                        foreach (DataRow rowItem in dsGeoZone.Tables[0].Rows)
                        {
                            enumId = Convert.ToInt16(rowItem["Type"]);
                            rowItem["DirectionName"] = Enum.GetName(typeof(Enums.GeoZoneDirection), (Enums.GeoZoneDirection)enumId);
                        }
                    }

                    this.dgGeoZone.DataSource = dsGeoZone;
                    this.dgGeoZone.DataBind();
                    sn.GeoZone.DsGeoZone = dsGeoZone;
                }
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

        // Changes for TimeZone Feature end

        private void DgGeoZone_Fill()
        {
            DataRow[] drCollections = null;
            StringReader strrXML = null;
            DataSet dsGeoZone = new DataSet();
            string xml = "";
            short enumId = 0;

            try
            {
                dbo = new ServerDBOrganization.DBOrganization();

                DataSet _geozones = null;
                string sConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                using (VLF.PATCH.Logic.PatchOrganizationGeozone pog = new VLF.PATCH.Logic.PatchOrganizationGeozone(sConnectionString))
                {
                    _geozones = pog.PatchGetOrganizationGeozonesWithStatus(sn.User.OrganizationId, sn.UserID);

                    if (VLF.CLS.Util.IsDataSetValid(_geozones))
                    {
                        xml = clsXmlUtil.GetXmlIncludingNull(_geozones);
                    }
                }

                if (xml == "")
                {
                    this.dgGeoZone.DataSource = null;
                    this.dgGeoZone.DataBind();
                    return;
                }

                strrXML = new StringReader(xml);
                dsGeoZone.ReadXml(strrXML);

                // Show DirectionName
                DataColumn dc = new DataColumn();
                dc.ColumnName = "DirectionName";
                dc.DataType = Type.GetType("System.String");
                dc.DefaultValue = "";

                DataTable dt = new DataTable();

                if (sn.User.ControlEnable(sn, 93))
                {
                    dt = dsGeoZone.Tables[0].Clone();

                    string filter = String.Format("Public=false AND CreateUserID={0}", sn.UserID.ToString());
                    drCollections = dsGeoZone.Tables[0].Select(filter);

                    if (drCollections != null && drCollections.Length > 0)
                    {
                        foreach (DataRow dr in drCollections)
                            dt.ImportRow(dr);
                    }

                    if (dt.Columns.IndexOf("DirectionName") == -1)
                    { 
                        dt.Columns.Add(dc);

                        foreach (DataRow rowItem in dt.Rows)
                        {
                            enumId = Convert.ToInt16(rowItem["Type"]);
                            rowItem["DirectionName"] = Enum.GetName(typeof(Enums.GeoZoneDirection), (Enums.GeoZoneDirection)enumId);
                        }
                    }

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);
                    sn.GeoZone.DsGeoZone = ds;

                    this.dgGeoZone.DataSource = ds;
                    this.dgGeoZone.DataBind();
                }
                else
                {
                    if (dsGeoZone.Tables[0].Columns.IndexOf("DirectionName") == -1)
                    {
                        dsGeoZone.Tables[0].Columns.Add(dc);

                        foreach (DataRow rowItem in dsGeoZone.Tables[0].Rows)
                        {
                            enumId = Convert.ToInt16(rowItem["Type"]);
                            rowItem["DirectionName"] = Enum.GetName(typeof(Enums.GeoZoneDirection), (Enums.GeoZoneDirection)enumId);
                        }
                    }

                    this.dgGeoZone.DataSource = dsGeoZone;
                    this.dgGeoZone.DataBind();
                    sn.GeoZone.DsGeoZone = dsGeoZone;
                }   
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }

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
            this.dgGeoZone.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgGeoZone_ItemCommand);
            this.dgGeoZone.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dgGeoZone_PageIndexChanged);
            this.dgGeoZone.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgGeoZone_DeleteCommand);
            this.ID = "frmGeoZones";

        }
        #endregion

        protected void cmdMap_Click(object sender, System.EventArgs e)
        {
            try
            {
                sn.Landmark.AddLandmarkMode = false;
                sn.Landmark.EditLandmarkMode = false;
               sn.GeoZone.AddMode = false;
               sn.GeoZone.EditMode = false;
                sn.Map.XInCoord = 0;
                sn.Map.YInCoord = 0;

                //if (sn.User.MapType == VLF.MAP.MapType.MapsoluteWeb)
                //{
                //   sn.MapSolute.LoadDefaultMapWithoutLandmarks(sn);  
                //   Response.Redirect("frmMap_land_geo_mapsolute.aspx");
                //}
                if (sn.User.UserMapType == (Int16)clsMap.MapEngineType.VirtualEarth)
                {
                    Response.Redirect("frmMap_Landmark_GeoZoneVE.aspx");
                }
                else
                    Response.Redirect("frmMap_Landmark_GeoZone.aspx");                
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
        }

        private void cboDirection_SelectedIndexChanged(object sender, System.EventArgs e)
        {
           sn.GeoZone.Direction = Convert.ToInt16(cboDirection.SelectedItem.Value);
        }

        private void cboGeoZoneSeverity_SelectedIndexChanged(object sender, System.EventArgs e)
        {
           sn.GeoZone.Severity = Convert.ToInt16(cboGeoZoneSeverity.SelectedItem.Value);
        }

        protected void cmdGeoZoneAdd_Click(object sender, System.EventArgs e)
        {
            this.txtMapMessage.Text = "";
           sn.GeoZone.SetGeoZone = false;
           sn.GeoZone.ImgConfPath = "";
           sn.GeoZone.ShowEditGeoZoneTable = true;
            this.tblGeoZones.Visible = false;
            this.tblGeoZoneAdd.Visible = true;
           sn.GeoZone.AddMode = true;
           sn.GeoZone.EditMode = false;
            ClearFields();
            cboTimeZone_Fill();
            EnableEditFields(true);
           sn.GeoZone.IsGeoZoneComplete = false;
           sn.GeoZone.DsGeoDetails.Tables[0].Rows.Clear();
            sn.Map.LandmarksMapVE = "";
            sn.Map.GeozonesMapVE = "";
            sn.Map.EditMapVE = "false";

            DisableOptSelection();
            DisableOrLockControls(true);      
        }

        private void ClearFields()
        {
            this.lblAddMessage.Text = "";
            this.lblMessage.Text = "";
            this.txtGeoZoneName.Text = "";
            this.txtGeoZoneDesc.Text = "";
            this.lblGeoZoneId.Text = "";
            this.lblOldGeoZoneName.Text = "";
            this.cboLandmarks.SelectedIndex = -1;
            this.txtPhone.Text = "";
            this.txtEmail.Text = "";
        }


        private void DisableOptSelection()
        {
            if (sn.User.ControlEnable(sn, 93))
            {
                this.optGeozonePublicPrivate.SelectedValue = "0";
                this.optGeozonePublicPrivate.Visible = false;
            }
            //foreach (DataRow rowItem in sn.User.DsGUIControls.Tables[0].Rows)
            //{
            //    if (Convert.ToInt32(rowItem["ControlId"]) == 93) //Disable public geozone option 
            //    {
            //        this.optGeozonePublicPrivate.SelectedValue = "0";
            //        this.optGeozonePublicPrivate.Enabled = false;
            //        break;
            //    }
            //}
        }

        private void DisableOrLockControls(bool status)
        {
            this.txtGeoZoneName.Enabled = status;
            this.txtGeoZoneDesc.Enabled = status;
            this.cboDirection.Enabled = status;
            this.cboGeoZoneSeverity.Enabled = status;
            this.cboFleet.Enabled = status;
            this.cboLandmarks.Enabled = status;
            this.optGeoZoneType.Enabled = status;
            this.cmdClearAllPoints.Enabled = status;
            this.txtLatitude.Enabled = status;
            this.txtLongitude.Enabled = status;
            this.cmdAddGeoZonePoint.Enabled = status;
            this.txtEmail.Enabled = status;
            this.txtPhone.Enabled = status;
            this.cboTimeZone.Enabled = status;
            this.chkCritical.Enabled = status;
            this.chkWarning.Enabled = status;
            this.chkNotify.Enabled = status;
            this.chkDayLight.Enabled = status;
            this.cmdSaveGeoZone.Enabled = status;
            GeozoneMap = "../MapNew/frmDrawLandmarkGeozone.aspx?FormName=Geozone&ShowControl=" + isGeoZoneUpdatableByUser.ToString();
            
            if (!status)
            {
                lblMessage.Visible = true;
                lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_GeozoneReadOnlyErrorSecurity");
            }
            else
            {
                lblMessage.Visible = false;
            }
        }

        protected void cmdCancelLandmark_Click(object sender, System.EventArgs e)
        {
            this.tblGeoZones.Visible = true;
            this.tblGeoZoneAdd.Visible = false;
           sn.GeoZone.AddMode = false;
           sn.GeoZone.EditMode = false;
            ClearFields();
        }

        protected void dgGeoZone_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            try
            {
                this.lblMessage.Text = "";
                this.txtMapMessage.Text = "";
               sn.GeoZone.SetGeoZone = false;

                if (e.CommandName == "cmdEdit")
                {
                    trAssignment.Visible = false;
                    this.tblGeoZoneCoordinates.Visible = false;
                   sn.GeoZone.ImgConfPath = "";
                    GeoZoneId = Convert.ToInt16(dgGeoZone.DataKeys[e.Item.ItemIndex]);

                    //Check if Geozone Assigned to Vehicle
                    
                    dbo = new ServerDBOrganization.DBOrganization();
                    bool GeoZoneAssigned = false;
                    if (objUtil.ErrCheck(dbo.IsGeozoneAssigned(sn.UserID, sn.SecId, sn.User.OrganizationId, GeoZoneId, ref GeoZoneAssigned), false))
                        if (objUtil.ErrCheck(dbo.IsGeozoneAssigned(sn.UserID, sn.SecId, sn.User.OrganizationId, GeoZoneId, ref GeoZoneAssigned), true))
                        {
                            return;
                        }


                    foreach (DataRow dr in sn.GeoZone.DsGeoZone.Tables[0].Rows)
                    {
                        if (dgGeoZone.DataKeys[e.Item.ItemIndex].ToString().TrimEnd() == dr["GeoZoneId"].ToString().TrimEnd())
                        {

                           sn.GeoZone.EditMode = true;
                           sn.GeoZone.AddMode = false;

                            this.lblAddMessage.Text = "";
                            this.lblMessage.Text = "";

                            StringReader strrXML = null;
                            DataSet dsGeoZone = new DataSet();
                            
                            string xml = "";
                            dbo = new ServerDBOrganization.DBOrganization();
                           sn.GeoZone.DsGeoDetails.Tables[0].Rows.Clear();

                            if (objUtil.ErrCheck(dbo.GetOrganizationGeozoneInfo(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt16(dr["GeoZoneId"]), ref xml), false))
                                if (objUtil.ErrCheck(dbo.GetOrganizationGeozoneInfo(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt16(dr["GeoZoneId"]), ref xml), true))
                                {
                                    return;
                                }

                            if (xml == "")
                            {
                               sn.GeoZone.DsGeoDetails.Tables[0].Rows.Clear();
                                return;
                            }

                            strrXML = new StringReader(xml);
                            dsGeoZone.ReadXml(strrXML);


                            foreach (DataRow rowItem in dsGeoZone.Tables[0].Rows)
                            {
                                DataRow drGeo =sn.GeoZone.DsGeoDetails.Tables[0].NewRow();
                                drGeo["Latitude"] = rowItem["Latitude"];
                                drGeo["Longitude"] = rowItem["Longitude"];
                                drGeo["SequenceNum"] = rowItem["SequenceNum"];
                               sn.GeoZone.DsGeoDetails.Tables[0].Rows.Add(drGeo);
                            }

                            this.cboDirection.SelectedIndex = cboDirection.Items.IndexOf(cboDirection.Items.FindByValue(dr["Type"].ToString()));
                            this.cboGeoZoneSeverity.SelectedIndex = cboGeoZoneSeverity.Items.IndexOf(cboGeoZoneSeverity.Items.FindByValue(dr["SeverityId"].ToString()));
                            this.txtGeoZoneName.Text = dr["GeozoneName"].ToString().TrimEnd();
                           sn.GeoZone.Name = dr["GeozoneName"].ToString().TrimEnd(); 
                            this.txtGeoZoneDesc.Text = dr["Description"].ToString().TrimEnd();
                           sn.GeoZone.Severity =Convert.ToInt16(dr["SeverityId"].ToString().TrimEnd());
                           sn.GeoZone.Description = dr["Description"].ToString().TrimEnd(); 
                           sn.GeoZone.GeozoneTypeId = (VLF.CLS.Def.Enums.GeozoneType)Convert.ToInt16(dr["GeozoneType"].ToString().TrimEnd());
                            this.lblGeoZoneId.Text = dr["GeoZoneId"].ToString().TrimEnd();
                            this.lblOldGeoZoneName.Text = dr["GeozoneName"].ToString().TrimEnd();
                            this.tblGeoZoneAdd.Visible = true;
                            this.tblGeoZones.Visible = false;
                           sn.GeoZone.ShowEditGeoZoneTable = false;



                            cboTimeZone_Fill();
                            this.cboTimeZone.SelectedIndex = -1;
                            for (int i = 0; i < cboTimeZone.Items.Count; i++)
                            {
                                // Changes for TimeZone Feature start
                                //if (Convert.ToInt16(cboTimeZone.Items[i].Value.TrimEnd()) == Convert.ToInt32(dr["TimeZone"].ToString().TrimEnd()))
                                if (Convert.ToSingle(cboTimeZone.Items[i].Value.TrimEnd()) == Convert.ToSingle(dr["TimeZoneNew"].ToString().TrimEnd()))// Changes for TimeZone Feature end
                                {
                                    cboTimeZone.SelectedIndex = i;
                                    break;
                                }
                            }

                            this.txtEmail.Text = dr["Email"].ToString().TrimEnd();
                            this.txtPhone.Text = dr["Phone"].ToString().TrimEnd();
                            this.chkDayLight.Checked = Convert.ToBoolean(dr["AutoAdjustDayLightSaving"].ToString().TrimEnd());
                            this.chkNotify.Checked = Convert.ToBoolean(dr["Notify"].ToString().TrimEnd());
                            this.chkWarning.Checked = Convert.ToBoolean(dr["Warning"].ToString().TrimEnd());
                            this.chkCritical.Checked = Convert.ToBoolean(dr["Critical"].ToString().TrimEnd());

                           sn.GeoZone.IsAssigned = GeoZoneAssigned;
                           sn.GeoZone.IsGeoZoneComplete = true;
                            //Enable fields for Assigned Geozone
                            if (GeoZoneAssigned)
                                EnableEditFields(false);
                            else
                                EnableEditFields(true);
                            //return;


                            if (Convert.ToBoolean(dr["Public"].ToString()))
                                this.optGeozonePublicPrivate.SelectedValue = "1";
                            else
                                this.optGeozonePublicPrivate.SelectedValue = "0";

                            DisableOptSelection();
                            if (sn.GeoZone.EditMode)
                            {
                                dbo = new ServerDBOrganization.DBOrganization();
                                dbo.IsGeoZoneUpdatableByUser(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt16(this.lblGeoZoneId.Text), ref isGeoZoneUpdatableByUser);
                                DisableOrLockControls(isGeoZoneUpdatableByUser);
                            }
                        }
                    }
                }
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }

        private void txtGeoZoneDesc_TextChanged(object sender, System.EventArgs e)
        {
           sn.GeoZone.Description = txtGeoZoneDesc.Text;
        }

        //		private void cboLandmarks_SelectedIndexChanged(object sender, System.EventArgs e)
        //		{
        //			try
        //			{
        //				if (this.cboLandmarks.SelectedItem.Value.ToString()!="-1")
        //				{
        //					foreach(DataRow rowItem in sn.DsLandMarks.Tables[0].Rows)
        //					{
        //						if  (cboLandmarks.SelectedItem.Value.ToString().TrimEnd()== rowItem["LandmarkName"].ToString().TrimEnd())
        //						{
        //							this.txtY.Text=rowItem["Latitude"].ToString();
        //							this.txtX.Text=rowItem["Longitude"].ToString();
        //							
        //							return;
        //						}
        //						else
        //						{
        //							this.txtY.Text="0";
        //							this.txtX.Text="0";
        //						}
        //				
        //					}
        //				}
        //			}
        //			catch(NullReferenceException)
        //			{
        //				RedirectToLogin();
        //			}
        //			catch(Exception Ex)
        //			{
        //				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.Message.ToString()+" User:"+sn.UserID.ToString()+" Form:"+Page.GetType().Name));    
        //			}
        //		}

        protected void cmdSaveGeoZone_Click(object sender, System.EventArgs e)
        {
            try
            {
                Page.Validate();
                if (!Page.IsValid)
                    return;

                this.lblAddMessage.Text = "";
                
                dbo = new ServerDBOrganization.DBOrganization();


                lblMessage.Visible = true;
                this.lblAddMessage.Text = "";
                this.lblMessage.Text = "";
                txtMapMessage.Text = "";

                bool DayLightSaving = Convert.ToBoolean(objUtil.IsDayLightSaving(chkDayLight.Checked));
                bool geozonePublic = this.optGeozonePublicPrivate.SelectedValue == "1" ? true : false;

                

                if (sn.GeoZone.DsGeoDetails.Tables.Count == 0 ||sn.GeoZone.DsGeoDetails.Tables[0].Rows.Count == 0)
                {
                   this.lblMessage.Visible = true;
                   this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_AddGeozoneFailedError");
                   return;
                }

    
                if (sn.GeoZone.AddMode)
                {
                    if (objUtil.ErrCheck(dbo.AddGeozone_NewTZ(sn.UserID, sn.SecId, sn.User.OrganizationId, txtGeoZoneName.Text, Convert.ToInt16(cboDirection.SelectedItem.Value), Convert.ToInt16(sn.GeoZone.GeozoneTypeId), sn.GeoZone.DsGeoDetails.GetXml(), Convert.ToInt16(cboGeoZoneSeverity.SelectedItem.Value), txtGeoZoneDesc.Text, this.txtEmail.Text, this.txtPhone.Text, Convert.ToSingle(this.cboTimeZone.SelectedItem.Value), DayLightSaving, 0, this.chkNotify.Checked, this.chkWarning.Checked, this.chkCritical.Checked, chkDayLight.Checked, 0, geozonePublic), false))
                        if (objUtil.ErrCheck(dbo.AddGeozone_NewTZ(sn.UserID, sn.SecId, sn.User.OrganizationId, txtGeoZoneName.Text, Convert.ToInt16(cboDirection.SelectedItem.Value), Convert.ToInt16(sn.GeoZone.GeozoneTypeId), sn.GeoZone.DsGeoDetails.GetXml(), Convert.ToInt16(cboGeoZoneSeverity.SelectedItem.Value), txtGeoZoneDesc.Text, this.txtEmail.Text, this.txtPhone.Text, Convert.ToSingle(this.cboTimeZone.SelectedItem.Value), DayLightSaving, 0, this.chkNotify.Checked, this.chkWarning.Checked, this.chkCritical.Checked, chkDayLight.Checked, 0, geozonePublic), true))
                        {
                            this.lblMessage.Visible = true;
                            this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_AddGeozoneFailedError");
                            return;
                        }
                }

                if (sn.GeoZone.EditMode)
                {
                    if (objUtil.ErrCheck(dbo.UpdateGeozone_NewTZ(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt16(this.lblGeoZoneId.Text), txtGeoZoneName.Text, Convert.ToInt16(cboDirection.SelectedItem.Value), Convert.ToInt16(sn.GeoZone.GeozoneTypeId), sn.GeoZone.DsGeoDetails.GetXml(), Convert.ToInt16(cboGeoZoneSeverity.SelectedItem.Value), txtGeoZoneDesc.Text, this.txtEmail.Text, this.txtPhone.Text, Convert.ToSingle(this.cboTimeZone.SelectedItem.Value), DayLightSaving, 0, this.chkNotify.Checked, this.chkWarning.Checked, this.chkCritical.Checked, chkDayLight.Checked, geozonePublic), false))
                        if (objUtil.ErrCheck(dbo.UpdateGeozone_NewTZ(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt16(this.lblGeoZoneId.Text), txtGeoZoneName.Text, Convert.ToInt16(cboDirection.SelectedItem.Value), Convert.ToInt16(sn.GeoZone.GeozoneTypeId), sn.GeoZone.DsGeoDetails.GetXml(), Convert.ToInt16(cboGeoZoneSeverity.SelectedItem.Value), txtGeoZoneDesc.Text, this.txtEmail.Text, this.txtPhone.Text, Convert.ToSingle(this.cboTimeZone.SelectedItem.Value), DayLightSaving, 0, this.chkNotify.Checked, this.chkWarning.Checked, this.chkCritical.Checked, chkDayLight.Checked, geozonePublic), true))
                        {
                            this.lblMessage.Visible = true;
                            this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_UpdateGeozoneFailedError");
                            return;
                        }
                }


                DgGeoZone_Fill_NewTZ();

                if (enableAssignment && trAssignment.Visible)
                {
                    int fleetId = Convert.ToInt32(cboFleet.SelectedItem.Value);
                    if (fleetId != -1)
                    {
                        string sConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                        VLF.PATCH.Logic.PatchOrganizationGeozone _geozone = new VLF.PATCH.Logic.PatchOrganizationGeozone(sConnectionString);
                        _geozone.AssignGeozoneToFleetByGeozoneName(sn.User.OrganizationId, this.txtGeoZoneName.Text, fleetId);
                    }

                }

                this.lblMessage.Visible = true;

                if (sn.GeoZone.EditMode)
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_UpdateGeozoneSuccess");
                else
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_AddGeozoneSuccess");

                this.tblGeoZoneAdd.Visible = false;
                this.tblGeoZones.Visible = true;
               sn.GeoZone.AddMode = false;
               sn.GeoZone.EditMode = false;
               sn.GeoZone.DsGeoDetails.Tables[0].Rows.Clear();



            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }

        bool CheckIfNumberic(string myNumber)
        {
            try
            {
                double dbl = Convert.ToDouble(myNumber);
                return true;
            }
            catch
            {
                return false;
            }
        }

        protected void dgGeoZone_DeleteCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            try
            {


                lblMessage.Visible = true;


                if (confirm == "")
                    return;

                dbo = new ServerDBOrganization.DBOrganization();
                dbo.IsGeoZoneUpdatableByUser(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt16(dgGeoZone.DataKeys[e.Item.ItemIndex].ToString()), ref isGeoZoneUpdatableByUser);

                if (!isGeoZoneUpdatableByUser)
                {
                    lblMessage.Visible = true;
                    lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_DeleteGeozoneFailedErrorSecurity");
                    return;
                }

                //dbo = new ServerDBOrganization.DBOrganization();
                


                if (objUtil.ErrCheck(dbo.DeleteGeozoneFromOrganization(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt16(dgGeoZone.DataKeys[e.Item.ItemIndex].ToString())), false))
                    if (objUtil.ErrCheck(dbo.DeleteGeozoneFromOrganization(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt16(dgGeoZone.DataKeys[e.Item.ItemIndex].ToString())), true))
                    {
                        lblMessage.Visible = true;
                        lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_DeleteGeozoneFailedError");
                        return;
                    }

                lblMessage.Visible = false;
                lblMessage.Text = "";
                dgGeoZone.SelectedIndex = -1;
                lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_DeleteGeozoneSuccess");
                this.dgGeoZone.CurrentPageIndex = 0;
                DgGeoZone_Fill_NewTZ();
                ViewState["ConfirmDelete"] = "0";
                confirm = "";
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }

        }


        private void EnableEditFields(bool status)
        {
            this.cboDirection.Enabled = status;
            tblGeoZoneMap.Visible = status;
            if (!status)
            {
                this.lblMessage.Visible = true;
                this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_GeozoneReadOnlyError");
            }

        }

        private void dgGeoZone_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
        {
            dgGeoZone.CurrentPageIndex = e.NewPageIndex;
            DgGeoZone_Fill_NewTZ();
            dgGeoZone.SelectedIndex = -1;
            this.lblAddMessage.Text = "";
            this.lblAddMessage.Visible = false;
        }

        private void dgGeoZone_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
        {
            if ((e.Item.ItemType == ListItemType.AlternatingItem) || (e.Item.ItemType == ListItemType.Item))
            {
                e.Item.Cells[8].ToolTip = (string)base.GetLocalResourceObject("dgGeozone_Item_Edit");
                e.Item.Cells[9].ToolTip = (string)base.GetLocalResourceObject("dgGeozone_Item_Delete");
            }

            if (e.Item.ItemType == ListItemType.Header)
            {

                if (sn.User.UnitOfMes == 1)
                {
                    e.Item.Cells[4].Text = (string)base.GetLocalResourceObject("dgGeozone_Item_WidthKM");
                    e.Item.Cells[5].Text = (string)base.GetLocalResourceObject("dgGeozone_Item_HeightKM");
                }
                else if (sn.User.UnitOfMes == 0.6214)
                {
                    e.Item.Cells[4].Text = (string)base.GetLocalResourceObject("dgGeozone_Item_WidthMI");
                    e.Item.Cells[5].Text = (string)base.GetLocalResourceObject("dgGeozone_Item_HeightMI");
                }
            }
        }

        private void txtStreet_TextChanged(object sender, System.EventArgs e)
        {
            this.txtY.Text = "0";
            this.txtX.Text = "0";
        }

        private void txtState_TextChanged(object sender, System.EventArgs e)
        {
            this.txtY.Text = "0";
            this.txtX.Text = "0";
        }

        private void txtCity_TextChanged(object sender, System.EventArgs e)
        {
            this.txtY.Text = "0";
            this.txtX.Text = "0";
        }

        private void cboCountry_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.txtY.Text = "0";
            this.txtX.Text = "0";
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
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }

        }


        protected void dgGeoZone_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            if ((e.Item.ItemType == ListItemType.AlternatingItem) || (e.Item.ItemType == ListItemType.Item))
            {
                confirm = "return confirm('" + (string)base.GetLocalResourceObject("dgGeozone_DeleteConfirmation") + "')";
                LinkButton deleteBtn = (LinkButton)e.Item.Cells[5].Controls[0];
                deleteBtn.Attributes.Add("onclick", confirm);
            }
        }
        protected void lnkByCoordinates_Click(object sender, EventArgs e)
        {
            this.tblGeoZoneCoordinates.Visible = true;
            this.dgGeoZonesCoordinates.DataSource =sn.GeoZone.DsGeoDetails;
            this.dgGeoZonesCoordinates.DataBind();
            optGeoZoneType.SelectedIndex = optGeoZoneType.Items.IndexOf(optGeoZoneType.Items.FindByValue(Convert.ToInt16(sn.GeoZone.GeozoneTypeId).ToString()));
            if (sn.GeoZone.GeozoneTypeId==0) 
               sn.GeoZone.GeozoneTypeId = VLF.CLS.Def.Enums.GeozoneType.Rectangle;
            GeozoneMap = "../MapNew/frmDrawLandmarkGeozone.aspx?FormName=Geozone&ShowControl=" + isGeoZoneUpdatableByUser.ToString();
        }
        protected void cmdAddGeoZonePoint_Click(object sender, EventArgs e)
        {
            if (!clsUtility.IsNumeric(this.txtLongitude.Text) || !clsUtility.IsNumeric(this.txtLatitude.Text))
            {
                this.lblAddMessage.Visible = true; 
                this.lblAddMessage.Text = "Longitude or Latitude should be numeric!";  
                return; 
            }

            if (sn.GeoZone.DsGeoDetails.Tables[0].Rows.Count == 2 && this.optGeoZoneType.SelectedItem.Value == "1")
            {
                this.lblAddMessage.Visible = true;
                this.lblAddMessage.Text = "The rectangle geozone is based on 2 points. Top left and bottom right.";
                return; 
            }


            if (sn.GeoZone.DsGeoDetails.Tables[0].Rows.Count ==10 && this.optGeoZoneType.SelectedItem.Value == "2")
            {
                this.lblAddMessage.Visible = true;
                this.lblAddMessage.Text = "The polygon geozone shound be based not more than 10 points.";
                return;
            }

           
            this.lblAddMessage.Text = "";
            DataRow dr =sn.GeoZone.DsGeoDetails.Tables[0].NewRow();
            dr["Latitude"] = this.txtLatitude.Text ;
            dr["Longitude"] = this.txtLongitude.Text;
            dr["SequenceNum"] =sn.GeoZone.DsGeoDetails.Tables[0].Rows.Count + 1;
           sn.GeoZone.DsGeoDetails.Tables[0].Rows.Add(dr);
            this.dgGeoZonesCoordinates.DataSource =sn.GeoZone.DsGeoDetails;
            this.dgGeoZonesCoordinates.DataBind();
            this.txtLatitude.Text = "";
            this.txtLongitude.Text = "";

        }
        protected void cmdClearAllPoints_Click(object sender, EventArgs e)
        {
           sn.GeoZone.DsGeoDetails.Tables[0].Rows.Clear();
            this.dgGeoZonesCoordinates.DataSource =sn.GeoZone.DsGeoDetails;
            this.dgGeoZonesCoordinates.DataBind();
        }
        protected void optGeoZoneType_SelectedIndexChanged(object sender, EventArgs e)
        {
           sn.GeoZone.GeozoneTypeId = (VLF.CLS.Def.Enums.GeozoneType)Convert.ToInt16(this.optGeoZoneType.SelectedItem.Value);
        }

        private void CboFleet_Fill()
        {
            try
            {
                DataSet dsFleets = new DataSet();
                dsFleets = sn.User.GetUserFleets(sn);
                cboFleet.DataSource = dsFleets;
                cboFleet.DataBind();

                cboFleet.Items.Insert(0, new ListItem((string)this.GetLocalResourceObject("cboFleet_Item_0"), "-1"));


                foreach (ListItem li in cboFleet.Items)
                {
                    if (li.Text == "All Vehicles" || li.Text == "Tous les véhicules" || li.Text == "Tous les vehicules")
                    {
                        cboFleet.Items.Remove(li);
                        break;
                    }

                }
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

        public string GetDirectionName(object direction)
        {
            string[] geoDirectionFrench = new string[4] { "Désactivé", "Entrant", "Sortant", "EntrantSortant" };
            string[] geoDirectionEnglish = new string[4] { "Disable", "Out", "In", "InOut" };
            if (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "fr")
            {
                return geoDirectionFrench[Array.IndexOf(geoDirectionEnglish, (string)direction)];
            }
            else
                return (string)direction;
        }

        public string GetSeverityName(object severity)
        {          
            string[] geoSeverityFrench = new string[4] { "Alarme mineure", "Alarme majeure", "Alarme critique", "Aucune alarme" };
            string[] geoSeverityEnglish = new string[4] { "Notify", "Warning", "Critical", "NoAlarm" };
            if (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "fr")
            {
                return geoSeverityFrench[Array.IndexOf(geoSeverityEnglish, (string)severity)];
            }
            else
                return (string)severity;
        }
        
}
}
