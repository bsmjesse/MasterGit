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
namespace SentinelFM.Map
{
	/// <summary>
	/// Summary description for frmBigMapOptions.
	/// </summary>
	public partial class frmBigMapOptions : SentinelFMBasePage
	{
		
		

		protected void Page_Load(object sender, System.EventArgs e)
		{
			//Clear IIS cache
			Response.Cache.SetCacheability(HttpCacheability.NoCache);
			Response.Cache.SetExpires(DateTime.Now);
	
			sn = (SentinelFMSession) Session["SentinelFMSession"] ;

			if (!Page.IsPostBack)
			{
				this.chkShowLandmark.Checked=sn.Map.ShowLandmark;
				this.chkShowLandmarkname.Checked=sn.Map.ShowLandmarkname;
				this.chkShowVehicleName.Checked=sn.Map.ShowVehicleName;     
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

		protected void cmdSave_Click(object sender, System.EventArgs e)
		{
			sn.Map.ShowLandmark=this.chkShowLandmark.Checked;
			sn.Map.ShowLandmarkname=this.chkShowLandmarkname.Checked;
			sn.Map.ShowVehicleName=this.chkShowVehicleName.Checked;     

			string ShowLandmark="";
			string ShowLandmarkname="";
			string ShowVehicleName="";
 

			if (sn.Map.ShowLandmark)
				ShowLandmark="1";
			else
				ShowLandmark="0";

			if (sn.Map.ShowLandmarkname)
				ShowLandmarkname="1";
			else
				ShowLandmarkname="0";


			if (sn.Map.ShowVehicleName)
				ShowVehicleName="1";
			else
				ShowVehicleName="0";


    
			ServerDBUser.DBUser dbu = new ServerDBUser.DBUser()  ;
			

			//ShowLandmark

            if (objUtil.ErrCheck(dbu.UserPreference_Add_Update(sn.UserID, sn.SecId, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.MapOptShowLandmark), ShowLandmark), false))
                if (objUtil.ErrCheck(dbu.UserPreference_Add_Update(sn.UserID, sn.SecId, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.MapOptShowLandmark), ShowLandmark), true))
				{
					System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error," MapOptShowLandmark . User:"+sn.UserID.ToString()+" Form:"+Page.GetType().Name));    
				}

			//ShowLandmarkName

            if (objUtil.ErrCheck(dbu.UserPreference_Add_Update(sn.UserID, sn.SecId, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.MapOptShowLandmarkName), ShowLandmarkname), false))
                if (objUtil.ErrCheck(dbu.UserPreference_Add_Update(sn.UserID, sn.SecId, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.MapOptShowLandmarkName), ShowLandmarkname), true))
				{
					System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error," MapOptShowLandmark . User:"+sn.UserID.ToString()+" Form:"+Page.GetType().Name));    
				}

			//ShowVehicleName

            if (objUtil.ErrCheck(dbu.UserPreference_Add_Update(sn.UserID, sn.SecId, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.MapOptShowVehicleName), ShowVehicleName), false))
                if (objUtil.ErrCheck(dbu.UserPreference_Add_Update(sn.UserID, sn.SecId, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.MapOptShowVehicleName), ShowVehicleName), true))
				{
					System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error," MapOptShowLandmark . User:"+sn.UserID.ToString()+" Form:"+Page.GetType().Name));    
				}


			Response.Write("<script language='javascript'>window.opener.location.href='frmBigMap.aspx'; window.close()</script>"); 
		}

		protected void chkShowLandmarkname_CheckedChanged(object sender, System.EventArgs e)
		{
			if (chkShowLandmarkname.Checked)
			{
				this.chkShowLandmark.Checked=true;  
				this.chkShowLandmark.Enabled=false;  
			}
			else
			{
				this.chkShowLandmark.Enabled=true;  
			}
		}

		protected void chkShowLandmark_CheckedChanged(object sender, System.EventArgs e)
		{
			if (chkShowLandmark.Checked)
			{
				this.chkShowLandmarkname.Enabled=true;   
			}
			else
			{
				this.chkShowLandmarkname.Enabled=false;
				this.chkShowLandmarkname.Checked=false; 
			}
		}
	}
}
