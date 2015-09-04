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

namespace SentinelFM
{
	/// <summary>
	/// Summary description for frmMain.
	/// </summary>
	public partial class frmMain : SentinelFMBasePage
	{
      public string strMapForm = "";
	public string alarmPage = "";
	public string mainFrameWidth = "*,220";
      
      //public string MDTMessagesScrollingHight = "45%";
      //public string AlarmsScrollingHight = "46%";
      //public string MDTMessageButtonHight = "*"; 

      public string MDTMessagesScrollingHight = "90%";
      public string AlarmsScrollingHight = "0%";
      public string MDTMessageButtonHight = "*"; 
		protected void Page_Load(object sender, System.EventArgs e)
		{
            alarmPage = "frmAlarmRotatingClient.aspx";
            alarmPage = "";
            mainFrameWidth = "*,1";

            //if (sn.UserName.Contains("g4s") || sn.UserName.Contains("sfm2000"))
            if (sn.User.ControlEnable(sn, 115))
            {
                MDTMessagesScrollingHight = "0%";
                AlarmsScrollingHight = "100%";
                MDTMessageButtonHight = "0%";
                alarmPage = "frm_Alarms.aspx";
                mainFrameWidth = "*,525";
            }
            else
            {
                if (sn.User.ViewMDTMessagesScrolling == 1 && sn.User.ViewAlarmScrolling == 1)
                {
                    MDTMessagesScrollingHight = "45%";
                    AlarmsScrollingHight = "46%";
                    MDTMessageButtonHight = "*"; 
                }
                else if (sn.User.ViewMDTMessagesScrolling == 0 && sn.User.ViewAlarmScrolling == 1)
                {
                    MDTMessagesScrollingHight = "0%";
                    AlarmsScrollingHight = "94%";
                    MDTMessageButtonHight = "0%"; 
                }
            }


         if (sn.User.MapType == VLF.MAP.MapType.MapsoluteWeb)
             strMapForm = "frmMapsoluteMap.aspx";
         else if (sn.User.MapType == VLF.MAP.MapType.LSD)
             strMapForm = "frmLSDmap.aspx";
         else if (sn.User.MapType == VLF.MAP.MapType.VirtualEarth)
             //strMapForm = "frmVehicleMapVE.aspx";
             strMapForm = "../MapVE/VEMap.aspx";
         else
            {
                //strMapForm = "frmvehiclemap.aspx"; 
                if (!sn.UserName.ToLower().Contains("g4s"))
                {
                    //Response.Redirect("../Map2012/frmassetmap.aspx");
                    //strMapForm = "../Map2012/frmassetmap.aspx";
		 strMapForm = "../MapNew/frmvehiclemap.aspx"; 
                }
                else
                {
                    // Response.Redirect("../MapNew/frmvehiclemap.aspx");
                    strMapForm = "frmvehiclemap.aspx";
			
                }
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
		}
		#endregion
	}
}
