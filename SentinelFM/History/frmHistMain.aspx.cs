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

namespace SentinelFM.History
{
	/// <summary>
	/// Summary description for frmHistMain.
	/// </summary>
	public partial class frmHistMain : SentinelFMBasePage
	{
        public string strHistoryForm = "frmHistMap.aspx";
    
	    protected void Page_Load(object sender, System.EventArgs e)
		{
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
