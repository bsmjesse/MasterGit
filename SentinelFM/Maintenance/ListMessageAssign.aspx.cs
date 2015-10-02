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
using System.Drawing;
using System.Text;
using VLF.CLS;
using System.IO;
using System.Globalization;
using Telerik.Web.UI;
using VLF.DAS.Logic;
using System.Web.Services;
using System.Collections.Generic;
using System.Web.Script.Serialization;


namespace SentinelFM
{
    public partial class Maintenance_ListMessageAssign : SentinelFMBasePage
    {
        public string assignStr1 = "This action will assign service for (1) box, continue?";
        public string assignStr2 = "This action will assign service for (n) boxes, continue?";
        public string deleteStr1 = "This action will delete service assignments for (1) box, continue?";
        public string deleteStr2 = "This action will delete service assignments for (n) boxes, continue?";
        public long mccID = -1;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                if (Request.QueryString["m"] != null)
                    mccID = long.Parse(Request.QueryString["m"]);
                if (Request.QueryString["t"] != null)
                {
                    gdVehicle.Columns.FindByUniqueName("ColcalValue").Visible = true;
                }
                else gdVehicle.Columns.FindByUniqueName("ColcalValue").Visible = false;

                if (Session["SelectedMaintenances"] != null)
                {

                    Session["SelectedMaintenances"] = null;
                }
            }
        }
    }
}