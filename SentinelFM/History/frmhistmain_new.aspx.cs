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
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;
using Telerik.Web.UI;
using System.Web.Services;
using System.Text;
using System.Web.Script.Serialization;

namespace SentinelFM
{
    public partial class History_frmhistmain_new : SentinelFMBasePage
    {
        public string strHistoryForm = "frmHistMap.aspx";

        public string FromMapScreen = string.Empty;
        protected void Page_Load(object sender, System.EventArgs e)
        {

	    try
            {
                string vehicleID = Request.QueryString["VehicleId"];
                if (!string.IsNullOrEmpty(vehicleID))
                {
                    sn.History.VehicleId = Int64.Parse(vehicleID);
                    sn.History.FromDate = "";
                    sn.History.ToDate = "";
                    sn.History.FromHours = "";
                    sn.History.ToHours = "";
                    sn.History.RedirectFromMapScreen = true;
                }
            }
            catch (Exception)
            {
                //Do nothing if this doesn't work.
            }

            if (sn.History.RedirectFromMapScreen) FromMapScreen = "?FromMapScreen=1";
            if (sn.User.MapType == VLF.MAP.MapType.MapsoluteWeb)
                strHistoryForm = "frmHistMapSoluteMap.aspx";
            else if (sn.User.MapType == VLF.MAP.MapType.LSD)
                strHistoryForm = "frmHistoryLSDMap.aspx";
            else if (sn.User.MapType == VLF.MAP.MapType.VirtualEarth)
                //strHistoryForm = "frmHistoryMapVE.aspx";
                strHistoryForm = "../MapVE/VEHistory.aspx";
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
        }
        #endregion
    }
}