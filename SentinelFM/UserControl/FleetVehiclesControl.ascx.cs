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
using VLF.DAS.Logic;
using VLF.CLS;
using System.IO;

namespace SentinelFM
{
    public partial class UserControl_FleetVehiclesControl : System.Web.UI.UserControl
    {
        ServerDBFleet.DBFleet dbFleet = new ServerDBFleet.DBFleet();
        private SentinelFMSession sn = null;
        protected clsUtility objUtil;

        private string _width = "560px";
        private string _height = "50px";
        private string _border = "none"; // 'solid 1px color'
        private string _fleetWidth = "160px";
        private string _vehicleWidth = "160px";
        private string _backColor = "#fffff0";
        private static long _vehicleId = -1;
        private static int _fleetId = -1;
        private static int _boxId = -1;
        private static bool _showEntireFleet = false;

        // drop down vehicle index changed
        public event EventHandler VehicleChanged;

        protected void OnVehicleChanged(EventArgs e)
        {
            if (VehicleChanged != null)
                VehicleChanged(this, e);
        }




        // drop down vehicle index changed
       
        # region Properties
        public bool Enabled
        {
            get { return this.ddlFleets.Enabled && this.ddlVehicles.Enabled; }
            set { this.ddlFleets.Enabled = value; this.ddlVehicles.Enabled = value; }
        }



        /// <summary>
        /// ShowEnireFleet
        /// </summary>
        public bool ShowEntireFleet
        {
            get { return _showEntireFleet; }
            set { _showEntireFleet = value; }
        }

        /// <summary>
        /// Control Background Color
        /// </summary>
        public string BackColor
        {
            get { return _backColor; }
            set { _backColor = value; }
        }

        /// <summary>
        /// Vehicle drop down liat AutoPostBack
        /// </summary>
        public bool VehicleAutoPostBack
        {
            get { return this.ddlVehicles.AutoPostBack; }
            set { this.ddlVehicles.AutoPostBack = value; }
        }

        /// <summary>
        /// Vehicle drop down liat width
        /// </summary>
        public string VehicleWidth
        {
            get { return _vehicleWidth; }
            set { _vehicleWidth = value; }
        }

        /// <summary>
        /// Fleet drop down liat width
        /// </summary>
        public string FleetWidth
        {
            get { return _fleetWidth; }
            set { _fleetWidth = value; }
        }

        /// <summary>
        /// Control border
        /// </summary>
        public string Border
        {
            get { return _border; }
            set { _border = value; }
        }

        /// <summary>
        /// Control width
        /// </summary>
        public string Width
        {
            get { return _width; }
            set { _width = value; }
        }

        /// <summary>
        /// Control height
        /// </summary>
        public string Height
        {
            get { return _height; }
            set { _height = value; }
        }

        /// <summary>
        /// Fleets label text, default: 'Fleets:'
        /// </summary>
        public string FleetsCaption
        {
            get { return this.lblFleets.Text; }
            set { this.lblFleets.Text = value; }
        }

        /// <summary>
        /// Vehicles label text, default: 'Vehicles'
        /// </summary>
        public string VehiclesCaption
        {
            get { return this.lblVehicles.Text; }
            set { this.lblVehicles.Text = value; }
        }

        /// <summary>
        /// Fleets label CssClass
        /// </summary>
        public string FleetsCaptionCssClass
        {
            get { return this.lblFleets.CssClass; }
            set { this.lblFleets.CssClass = value; }
        }

        /// <summary>
        /// Fleet drop down list CssClass
        /// </summary>
        public string FleetsCssClass
        {
            get { return this.ddlFleets.CssClass; }
            set { this.ddlFleets.CssClass = value; }
        }

        /// <summary>
        /// Vehicles drop down list CssClass
        /// </summary>
        public string VehiclesCaptionCssClass
        {
            get { return this.lblVehicles.CssClass; }
            set { this.lblVehicles.CssClass = value; }
        }

        /// <summary>
        /// Vehicles label CssClass
        /// </summary>
        public string VehiclesCssClass
        {
            get { return this.ddlVehicles.CssClass; }
            set { this.ddlVehicles.CssClass = value; }
        }

        /// <summary>
        /// Fleet drop down list Selected fleet ID
        /// </summary>
        public int SelectedFleet
        {
            get { 
                   // return Convert.ToInt32(this.ddlFleets.SelectedValue); 
                return (_fleetId);
                }
            set
            {
                
                if (ddlFleets.Items.Count>0 && this.ddlFleets.Items.FindByValue(value.ToString()) != null)
                    this.ddlFleets.SelectedValue = value.ToString();

                _fleetId = value;
            }
        }

        /// <summary>
        /// Vehicles drop down list selected vehicle ID
        /// </summary>
        public long SelectedVehicle
        {
            get
            {
                return _vehicleId;
            }
            set
            {
                //if (!this.ddlVehicles.Enabled) return;

                if (ddlVehicles.Items.Count > 0 && this.ddlVehicles.Items.FindByValue(value.ToString()) != null)
                     this.ddlVehicles.SelectedValue = value.ToString();

                    _vehicleId = value;

                
            }
        }

        /// <summary>
        /// Vehicles drop down list selected box ID
        /// </summary>
        public int SelectedBox
        {
            get
            {
                return _boxId;
            }
            set
            {
                if (!this.ddlVehicles.Enabled) return;
                if (this.ddlVehicles.Items.FindByValue(value.ToString()) != null)
                {
                    _boxId = value;
                    this.ddlVehicles.SelectedValue = value.ToString();
                }
            }
        }

        /// <summary>
        /// Selected string value of vehicles drop down list
        /// </summary>
        public string SelectedVehicleValue
        {
            get { return this.ddlVehicles.SelectedValue; }
            set { this.ddlVehicles.SelectedValue = value; }
        }

        /// <summary>
        /// Vehicle DataValueField: VehicleId, BoxId, LicensePlate, Description
        /// </summary>
        public string VehicleDataValueField
        {
            get { return this.ddlVehicles.DataValueField; }
            set { this.ddlVehicles.DataValueField = value; }
        }

        # endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //Clear IIS cache
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetExpires(DateTime.Now);
                sn = (SentinelFMSession)Session["SentinelFMSession"];
                if (sn == null || sn.UserName == "")
                {
                    return;
                }
                objUtil = new clsUtility(sn);

                if (!IsPostBack)
                {
                    GetFleets();

                    if (_fleetId>0)
                        ddlFleets.SelectedIndex = ddlFleets.Items.IndexOf(ddlFleets.Items.FindByValue(_fleetId.ToString()));

                    if (this.ddlFleets.SelectedIndex > 0)
                    {
                        
                        GetVehicles(_fleetId);
                        this.ddlVehicles.Enabled = true;

                        if (_vehicleId != -1)
                            ddlVehicles.SelectedIndex = ddlVehicles.Items.IndexOf(ddlVehicles.Items.FindByValue(_vehicleId.ToString()));

                    }

                }
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
                   VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
                   VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " FleetVehiclesControl.ascx"));
            }

        }

        /// <summary>
        /// Get fleets data and fill drop down list
        /// </summary>
        protected void GetFleets()
        {
            string xml = "";
            try
            {
                //if (objUtil.ErrCheck(dbFleet.GetFleetsInfoXMLByUserId(sn.UserID, sn.SecId, ref xml), false))
                //    if (objUtil.ErrCheck(dbFleet.GetFleetsInfoXMLByUserId(sn.UserID, sn.SecId, ref xml), true))
                //    {
                //        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
                //           VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GetFleetsInfoXMLByUserId Error : User:" +
                //           sn.UserID.ToString() + " FleetVehiclesControl.ascx"));
                //        return;
                //    }
                //if (String.IsNullOrEmpty(xml))
                //    return;


                DataSet dsFleets = new DataSet();
                dsFleets = sn.User.GetUserFleets(sn);
                if (Util.IsDataSetValid(dsFleets))
                {
                    ddlFleets.DataSource = dsFleets;
                    ddlFleets.DataBind();
                    ddlFleets.Items.Insert(0, new ListItem(base.GetLocalResourceObject("StringSelectFleet").ToString(), "-1"));
                }
                else
                {
                    ddlFleets.Items.Insert(0, new ListItem(base.GetLocalResourceObject("StringNoFleets").ToString(), "-1"));
                }
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
                   VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
                   VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " FleetVehiclesControl.ascx"));
            }
        }

        protected void ddlFleets_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.ddlFleets.SelectedIndex > 0)
            {
                _fleetId = Convert.ToInt32(ddlFleets.SelectedValue);
                this.ddlVehicles.Enabled = true;
                GetVehicles(_fleetId);
            }
            else
            {
                this.ddlVehicles.SelectedIndex = 0;
                this.ddlVehicles.Enabled = false;
            }

            
        }

        protected void ddlVehicles_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.ddlVehicles.DataValueField)
            {
                case "VehicleId":
                    _vehicleId = Convert.ToInt64(this.ddlVehicles.SelectedValue);
                    break;
                case "BoxId":
                    _boxId = Convert.ToInt32(this.ddlVehicles.SelectedValue);
                    break;
                default:
                    break;
            }
            OnVehicleChanged(e);
        }

        /// <summary>
        /// Get vehicles data and fill drop down list
        /// </summary>
        /// <param name="fleetId"></param>
        protected void GetVehicles(int fleetId)
        {
            string xml = "";
            if (objUtil.ErrCheck(dbFleet.GetVehiclesShortInfoXMLByFleetId(sn.UserID, sn.SecId, this.SelectedFleet, ref xml), false))
                if (objUtil.ErrCheck(dbFleet.GetVehiclesShortInfoXMLByFleetId(sn.UserID, sn.SecId, this.SelectedFleet, ref xml), true))
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
                       VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GetVehiclesShortInfoXMLByFleetId Error"));
                    return;
                }
            if (String.IsNullOrEmpty(xml))
                return;

            DataSet dsVehicles = new DataSet();
            dsVehicles.ReadXml(new StringReader(xml));
            if (Util.IsDataSetValid(dsVehicles))
            {
                ddlVehicles.DataSource = dsVehicles;
                ddlVehicles.DataBind();

                ddlVehicles.Items.Insert(0, new ListItem(base.GetLocalResourceObject("StringSelectVehicle").ToString(), "-1"));

                if (_showEntireFleet)
                    ddlVehicles.Items.Insert(1, new ListItem(base.GetLocalResourceObject("EntireFleet").ToString(), "-999"));

                ddlVehicles.SelectedIndex = 0;

            }
            else
            {
                ddlVehicles.Items.Insert(0, new ListItem(base.GetLocalResourceObject("StringNoVehicles").ToString(), "-1"));
            }

        }
    }
}