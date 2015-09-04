using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO; 
using System.Configuration;
 
namespace SentinelFM
{
	/// <summary>
	/// Summary description for WebForm1.
	/// </summary>
	public partial class frmAlarmRotating : SentinelFMBasePage
	{

		public string strAlarms;
		

		
		

		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				

				if (!Page.IsPostBack)
				{
					///AlarmsList_Fill();


               strAlarms = sn.Map.AlarmsHTML;
               this.lblTotalAlarms.Text = sn.Map.AlarmCount.ToString();   
 
					if (sn.Map.ReloadMap)
					{
                      //string strMapForm = "";
                      //if (sn.User.MapType == VLF.MAP.MapType.MapsoluteWeb)
                      //{
                      //   sn.MapSolute.LoadVehicles(sn, sn.Map.DsFleetInfo.Tables[0], "Map");
                      //   Response.Write("<SCRIPT Language='javascript'>parent.parent.frmFleetInfo.location.href='frmFleetInfoNew.aspx';</SCRIPT>");
                      //}
                      if (sn.User.MapType == VLF.MAP.MapType.LSD)
                          Response.Write("<SCRIPT Language='javascript'>parent.frmFleetInfo.location.href='frmFleetInfoNew.aspx'; parent.frmVehicleMap.location.href='frmLSDmap.aspx';</SCRIPT>");
                      else if (sn.User.MapType == VLF.MAP.MapType.VirtualEarth)
                         // Response.Write("<SCRIPT Language='javascript'>parent.parent.frmFleetInfo.location.href='frmFleetInfoNew.aspx'; parent.frmVehicleMap.location.href='frmVehicleMapVE.aspx';</SCRIPT>");
                          Response.Write("<SCRIPT Language='javascript'>parent.parent.frmFleetInfo.location.href='frmFleetInfoNew.aspx'; parent.frmVehicleMap.location.href='../MapVE/VEMap.aspx';</SCRIPT>");
                          
                      else
                         Response.Write("<SCRIPT Language='javascript'>parent.parent.frmFleetInfo.location.href='frmFleetInfoNew.aspx'; parent.frmVehicleMap.location.href='frmvehiclemap.aspx';</SCRIPT>");
    						
					     sn.Map.ReloadMap=false; 
					}
				}
			}
			catch(NullReferenceException Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.StackTrace.ToString()));
				RedirectToLogin();
			}
			catch(Exception Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.Message.ToString()+" User:"+sn.UserID.ToString()+" Form:"+Page.GetType().Name));    
			}

		
		}


      //private void AlarmsList_Fill()
      //{
      //   try
      //   {
      //      
											
      //      string xml = "" ;
      //      this.lblTotalAlarms.Text="0";

      //      ServerAlarms.Alarms alarms = new ServerAlarms.Alarms() ;
      //      Int16 timeZone=Convert.ToInt16(sn.User.TimeZone+sn.User.DayLightSaving);

      //      if( objUtil.ErrCheck( alarms.GetCurrentAlarmsXMLByLang( sn.UserID , sn.SecId ,timeZone, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml ),false ) )
      //         if( objUtil.ErrCheck( alarms.GetCurrentAlarmsXMLByLang( sn.UserID , sn.SecId,timeZone, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml ), true ) )
      //         {
      //            return;
      //         }

      //      if (xml == "")
      //      {
      //         return;
      //      }

      //      StringReader strrXML = new StringReader(xml);

      //      DataSet ds=new DataSet();
      //      ds.ReadXml(strrXML);
		
      //      string str="";
      //      string strStyle="";
      //      int AlarmCount=0;

				
      //      for (int i=0 ; i < ds.Tables[0].Rows.Count ;++i)
      //      {
      //              /*
      //         string alarmState = ds.Tables[0].Rows[i]["AlarmState"].ToString();
      //         switch (alarmState)
      //         {
      //            case VLF.CLS.Def.Enums.AlarmState.Accepted.ToString():
      //               strStyle = "style='{color:green;}'";
      //               str = str + ("<p><a href='#' " + strStyle + " onclick=NewWindow('" + ds.Tables[0].Rows[i]["AlarmId"].ToString().TrimEnd() + "') >" + Convert.ToDateTime(ds.Tables[0].Rows[i]["TimeCreated"].ToString().TrimEnd()).ToLongTimeString() + " " + ds.Tables[0].Rows[i]["vehicleDescription"].ToString().TrimEnd() + "<br>" + "[" + ds.Tables[0].Rows[i]["AlarmDescription"].ToString().TrimEnd() + "]" + "</u></a></p><br>");
      //               break;

      //            case VLF.CLS.Def.Enums.AlarmState.Closed.ToString():
      //               strStyle = "style='{color:#C0C0C0;}'";
      //               break;

      //            case VLF.CLS.Def.Enums.AlarmState.New.ToString():
      //               string alarmLevel = ds.Tables[0].Rows[i]["AlarmLevel"].ToString();
      //               if (VLF.CLS.Def.Enums.AlarmSeverity.Warning.ToString() == alarmLevel)
      //               {
      //                  strStyle = "style='{color:DarkGoldenrod;}'";
      //                  str = str + ("<p><a href='#' " + strStyle + " onclick=NewWindow('" + ds.Tables[0].Rows[i]["AlarmId"].ToString().TrimEnd() + "') >" + Convert.ToDateTime(ds.Tables[0].Rows[i]["TimeCreated"].ToString().TrimEnd()).ToLongTimeString() + " " + ds.Tables[0].Rows[i]["vehicleDescription"].ToString().TrimEnd() + "<br>" + "[" + ds.Tables[0].Rows[i]["AlarmDescription"].ToString().TrimEnd() + "]" + "</u></a></p><br>");
      //               }
      //               else
      //                  if (VLF.CLS.Def.Enums.AlarmSeverity.Critical.ToString() == alarmLevel)
      //                  {
      //                     strStyle = "style='{color:red;}'";
      //                     str = str + ("<p><a href='#' " + strStyle + " onclick=NewWindow('" + ds.Tables[0].Rows[i]["AlarmId"].ToString().TrimEnd() + "') >" + "<IMG alt='' border=0  src='../images/exclameanim.gif'> " + Convert.ToDateTime(ds.Tables[0].Rows[i]["TimeCreated"].ToString().TrimEnd()).ToLongTimeString() + " " + ds.Tables[0].Rows[i]["vehicleDescription"].ToString().TrimEnd() + "<br>" + "[" + ds.Tables[0].Rows[i]["AlarmDescription"].ToString().TrimEnd() + "]" + "</u></a></p><br>");
      //                  }
      //               break;
      //         }
      //               */

      //         if (ds.Tables[0].Rows[i]["AlarmState"].ToString() == VLF.CLS.Def.Enums.AlarmState.Accepted.ToString())
      //         {
      //            strStyle="style='{color:green;}'";
      //         }	

      //         if (ds.Tables[0].Rows[i]["AlarmState"].ToString() ==VLF.CLS.Def.Enums.AlarmState.Closed.ToString())
      //         {
      //            strStyle="style='{color:#C0C0C0;}'";
      //         }	


      //         if (ds.Tables[0].Rows[i]["AlarmState"].ToString() ==VLF.CLS.Def.Enums.AlarmState.New.ToString() 
      //            && ds.Tables[0].Rows[i]["AlarmLevel"].ToString() ==VLF.CLS.Def.Enums.AlarmSeverity.Warning.ToString())
      //         {
      //            strStyle="style='{color:DarkGoldenrod;}'";
      //         }	


      //         if (ds.Tables[0].Rows[i]["AlarmState"].ToString() ==VLF.CLS.Def.Enums.AlarmState.New.ToString() 
      //            && ds.Tables[0].Rows[i]["AlarmLevel"].ToString() ==VLF.CLS.Def.Enums.AlarmSeverity.Critical.ToString())
      //         {
      //            strStyle="style='{color:red;}'";
      //         }	


      //         // --- Hide closed alarms
      //         if (ds.Tables[0].Rows[i]["AlarmState"].ToString() !=VLF.CLS.Def.Enums.AlarmState.Closed.ToString())
      //         {
      //            //str=str+("<p><a href='#' "+ strStyle +" onclick=NewWindow('"+ ds.Tables[0].Rows[i]["AlarmId"].ToString().TrimEnd() + "') >"+ds.Tables[0].Rows[i]["TimeCreated"].ToString().TrimEnd()+" "+ds.Tables[0].Rows[i]["vehicleDescription"].ToString().TrimEnd() +"</u></a></p><br>");
					
      //            if (ds.Tables[0].Rows[i]["AlarmState"].ToString() ==VLF.CLS.Def.Enums.AlarmState.New.ToString() 
      //               && ds.Tables[0].Rows[i]["AlarmLevel"].ToString() ==VLF.CLS.Def.Enums.AlarmSeverity.Critical.ToString())
      //               str=str+("<p><a href='#' "+ strStyle +" onclick=NewWindow('"+ ds.Tables[0].Rows[i]["AlarmId"].ToString().TrimEnd() + "') >"+"<IMG alt='' border=0  src='../images/exclameanim.gif'> "+Convert.ToDateTime(ds.Tables[0].Rows[i]["TimeCreated"].ToString().TrimEnd(), new CultureInfo("en-US")).ToLongTimeString()+" "+ds.Tables[0].Rows[i]["vehicleDescription"].ToString().TrimEnd()+"<br>"+"["+ds.Tables[0].Rows[i]["AlarmDescription"].ToString().TrimEnd()+"]" +"</u></a></p><br>");
      //            else
      //                      str = str + ("<p><a href='#' " + strStyle + " onclick=NewWindow('" + ds.Tables[0].Rows[i]["AlarmId"].ToString().TrimEnd() + "') >" + Convert.ToDateTime(ds.Tables[0].Rows[i]["TimeCreated"].ToString().TrimEnd(), new CultureInfo("en-US")).ToLongTimeString() + " " + ds.Tables[0].Rows[i]["vehicleDescription"].ToString().TrimEnd() + "<br>" + "[" + ds.Tables[0].Rows[i]["AlarmDescription"].ToString().TrimEnd() + "]" + "</u></a></p><br>");

      //            AlarmCount++;
      //         }
  
      //         //----------------------
				
      //      }

      //      strAlarms=str;
      //      this.lblTotalAlarms.Text=AlarmCount.ToString();   
      //   }
      //   catch(NullReferenceException Ex)
      //   {
      //      System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.StackTrace.ToString()));
      //      RedirectToLogin();
      //   }
      //   catch(Exception Ex)
      //   {
      //      System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.Message.ToString()+" User:"+sn.UserID.ToString()+" Form:"+Page.GetType().Name));    
      //   }
      //}

		

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
