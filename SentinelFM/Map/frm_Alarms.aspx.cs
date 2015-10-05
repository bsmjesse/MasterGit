using System;
using System.Globalization;
using System.Threading;
using System.Web.UI;
using VLF.CLS;
using VLF.CLS.Def;
using VLF.MAP;

namespace SentinelFM
{
    public partial class frm_Alarms : Page
    {
        public string alarmDetailPage = "frmAlarmInfo.aspx";
        public string errorCancel = "Failed to cancel.";
        public string errorLoad = "Failed to load data.";
        public string headerColor = "#009933";
        protected clsUtility objUtil;
        protected SentinelFMSession sn;
        public string SourcePage = "";
        public string strAlarms;
        public int windowHeight = 550;
        public int windowWidth = 400;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                ////Clear IIS cache
                //Response.Cache.SetCacheability(HttpCacheability.NoCache);
                //Response.Cache.SetExpires(DateTime.Now);
                //if (sn.UserName.ToLower().Contains("g4s") || sn.UserName.ToLower().Contains("sfm2000"))

                if (Request["s"] != null && Request["s"].Trim() != "")
                    SourcePage = Request["s"].Trim();

                if (sn.User.OrganizationId == 952 || sn.User.OrganizationId == 480 || sn.User.OrganizationId == 1000092 ||
                    sn.User.OrganizationId == 999991)
                {
                    alarmDetailPage = "frmAlarmInfo_G4S.aspx";
                    windowWidth = 525;
                    windowHeight = 410;
                }

                if (!Page.IsPostBack)
                {
                    if (sn.Map.ReloadMap)
                    {
                        if (sn.User.MapType == MapType.LSD)
                            Response.Write(
                                "<SCRIPT Language='javascript'>parent.frmFleetInfo.location.href='frmFleetInfoNew.aspx'; parent.frmVehicleMap.location.href='frmLSDmap.aspx';</SCRIPT>");
                        else if (sn.User.MapType == MapType.VirtualEarth)
                            Response.Write(
                                "<SCRIPT Language='javascript'>parent.frmFleetInfo.location.href='frmFleetInfoNew.aspx'; parent.frmVehicleMap.location.href='../MapVE/VEMap.aspx';</SCRIPT>");
                        else
                            Response.Write(
                                "<SCRIPT Language='javascript'>parent.frmFleetInfo.location.href='frmFleetInfoNew.aspx'; parent.frmVehicleMap.location.href='frmvehiclemap.aspx';</SCRIPT>");

                        sn.Map.ReloadMap = false;
                    }
                }
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
                    Util.TraceFormat(Enums.TraceSeverity.Error, Ex.StackTrace));
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
                    Util.TraceFormat(Enums.TraceSeverity.Error,
                        Ex.Message + " User:" + sn.UserID + " Form:" + Page.GetType().Name));
            }
        }

        //By Devin
        /// <summary>
        ///     Get String for javascript, convert quotes character
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string GetScriptEscapeString(string str)
        {
            if (string.IsNullOrEmpty(str)) return string.Empty;
            var newQuote = @"\" + @"""";
            var newQuote1 = @"\" + @"'";
            return str.Replace("\"", newQuote).Replace("'", newQuote1);
        }


        protected override void InitializeCulture()
        {
            if (Session["PreferredCulture"] != null)
            {
                var UserCulture = Session["PreferredCulture"].ToString();
                if (UserCulture != "")
                {
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo(UserCulture);
                }
            }
        }


        protected void Page_Init(object sender, EventArgs e)
        {
            sn = (SentinelFMSession) Session["SentinelFMSession"];
            if (sn == null || sn.User == null || string.IsNullOrEmpty(sn.UserName))
            {
                RedirectToLogin();
                return;
            }

            if (sn.User.MenuColor != "")
                headerColor = sn.User.MenuColor;
        }


        public void RedirectToLogin()
        {
            Session.Abandon();
            Response.Write(
                "<SCRIPT Language='javascript'>top.document.all('TopFrame').cols='0,*';window.open('../Login.aspx','_top')</SCRIPT>");
        }

        #region Web Form Designer generated code

        protected override void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        ///     Required method for Designer support - do not modify
        ///     the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        }

        #endregion
    }
}