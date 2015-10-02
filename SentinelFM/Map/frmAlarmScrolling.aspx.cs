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

namespace SentinelFM.Map
{
	

	public partial class frmAlarmScrolling : SentinelFMBasePage
	{
		public string AlarmList="";

		
		protected System.Web.UI.WebControls.Label lblTotalAlarms;
		

		protected void Page_Load(object sender, System.EventArgs e)
		{

			

			if (!Page.IsPostBack)
			{
                AlarmsList_Fill_NewTZ();
			}
		}

        // Changes for TimeZone Feature start

        private void AlarmsList_Fill_NewTZ()
        {
            try
            {


                string xml = "";
                ServerAlarms.Alarms alarms = new ServerAlarms.Alarms();
                float timeZone = Convert.ToSingle(sn.User.NewFloatTimeZone + sn.User.DayLightSaving);

                if (objUtil.ErrCheck(alarms.GetCurrentAlarmsXMLByLang_NewTZ(sn.UserID, sn.SecId, timeZone, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                    if (objUtil.ErrCheck(alarms.GetCurrentAlarmsXMLByLang_NewTZ(sn.UserID, sn.SecId, timeZone, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                    {
                        return;
                    }

                if (xml == "")
                    return;

                StringReader strrXML = new StringReader(xml);

                DataSet ds = new DataSet();
                ds.ReadXml(strrXML);

                string str = "";
                string strStyle = "";
                int AlarmCount = 0;


                for (int i = 0; i < ds.Tables[0].Rows.Count; ++i)
                {
                    string state = ds.Tables[0].Rows[i]["AlarmState"].ToString();

                    switch (state)
                    {
                        case "Accepted":
                            strStyle = "style='{font-family: Arial, Helvetica, sans-serif; font-size: 13px;color:green;}'";
                            break;
                        case "Closed":
                            strStyle = "style='{font-family: Arial, Helvetica, sans-serif; font-size: 13px;color:#C0C0C0;}'";
                            str = str + "_Items[" + AlarmCount.ToString() + "]=\"<a " + strStyle + " class='infoevt'>" + Convert.ToDateTime(ds.Tables[0].Rows[i]["TimeCreated"].ToString().TrimEnd()).ToLongTimeString() + " " + ds.Tables[0].Rows[i]["vehicleDescription"].ToString().TrimEnd() + "[" + ds.Tables[0].Rows[i]["AlarmDescription"].ToString().TrimEnd() + "]" + "</a>\";";
                            AlarmCount++;
                            break;
                        case "New":
                            string level = ds.Tables[0].Rows[i]["AlarmLevel"].ToString();
                            if ("Warning" == level)
                                strStyle = "style='{font-family: Arial, Helvetica, sans-serif; font-size: 13px;color:DarkGoldenrod;}'";
                            else
                                if ("Critical" == level)
                                    strStyle = "style='{font-family: Arial, Helvetica, sans-serif; font-size: 13px;color:red;}'";
                            break;
                    }
                    /*
                                        if (ds.Tables[0].Rows[i]["AlarmState"].ToString() =="Accepted")
                                        {
                                            strStyle="style='{font-family: Arial, Helvetica, sans-serif; font-size: 13px;color:green;}'";
                                        }	

                                        if (ds.Tables[0].Rows[i]["AlarmState"].ToString() =="Closed")
                                        {
                                            strStyle="style='{font-family: Arial, Helvetica, sans-serif; font-size: 13px;color:#C0C0C0;}'";
                                        }	


                                        if (ds.Tables[0].Rows[i]["AlarmState"].ToString() =="New" && ds.Tables[0].Rows[i]["AlarmLevel"].ToString() =="Warning")
                                        {
                                            strStyle="style='{font-family: Arial, Helvetica, sans-serif; font-size: 13px;color:DarkGoldenrod;}'";
                                        }	


                                        if (ds.Tables[0].Rows[i]["AlarmState"].ToString() =="New" && ds.Tables[0].Rows[i]["AlarmLevel"].ToString() =="Critical")
                                        {
                                            strStyle="style='{font-family: Arial, Helvetica, sans-serif; font-size: 13px;color:red;}'";
                                        }	



                                        // --- Hide closed alarms
					
                                        if (ds.Tables[0].Rows[i]["AlarmState"].ToString() !="Closed")
                                        {
                                            str=str+"_Items["+AlarmCount.ToString()+"]=\"<a " +strStyle+ " class='infoevt'>"+Convert.ToDateTime(ds.Tables[0].Rows[i]["TimeCreated"].ToString().TrimEnd()).ToLongTimeString()+" "+ds.Tables[0].Rows[i]["vehicleDescription"].ToString().TrimEnd() +"["+ds.Tables[0].Rows[i]["AlarmDescription"].ToString().TrimEnd()+"]" +"</a>\";";
						
                                            AlarmCount++;
                                        }
                     */
                    //----------------------

                }

                AlarmList = str;

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:frmAlarmScrolling.aspx"));
            }
        }

        // Changes for TimeZone Feature end

		private void AlarmsList_Fill()
		{
			try
			{
				
							
				string xml = "" ;
				ServerAlarms.Alarms alarms = new ServerAlarms.Alarms() ;
				Int16 timeZone=Convert.ToInt16(sn.User.TimeZone+sn.User.DayLightSaving);

                if (objUtil.ErrCheck(alarms.GetCurrentAlarmsXMLByLang(sn.UserID, sn.SecId, timeZone, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                    if (objUtil.ErrCheck(alarms.GetCurrentAlarmsXMLByLang(sn.UserID, sn.SecId, timeZone, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
					{
						return;
					}

				if (xml == "")
					return;

				StringReader strrXML = new StringReader( xml ) ;

				DataSet ds=new DataSet();
				ds.ReadXml(strrXML);
		
				string str="";
				string strStyle="";
				int AlarmCount=0;

				
				for (int i=0; i < ds.Tables[0].Rows.Count ; ++i)
				{
               string state = ds.Tables[0].Rows[i]["AlarmState"].ToString();

               switch (state)
               {
                  case "Accepted":
                     strStyle = "style='{font-family: Arial, Helvetica, sans-serif; font-size: 13px;color:green;}'";
                     break;
                  case "Closed":
                     strStyle = "style='{font-family: Arial, Helvetica, sans-serif; font-size: 13px;color:#C0C0C0;}'";
                     str = str + "_Items[" + AlarmCount.ToString() + "]=\"<a " + strStyle + " class='infoevt'>" + Convert.ToDateTime(ds.Tables[0].Rows[i]["TimeCreated"].ToString().TrimEnd()).ToLongTimeString() + " " + ds.Tables[0].Rows[i]["vehicleDescription"].ToString().TrimEnd() + "[" + ds.Tables[0].Rows[i]["AlarmDescription"].ToString().TrimEnd() + "]" + "</a>\";";
                     AlarmCount++;
                     break;
                  case "New":
                     string level = ds.Tables[0].Rows[i]["AlarmLevel"].ToString();
                     if ("Warning" == level)
                        strStyle = "style='{font-family: Arial, Helvetica, sans-serif; font-size: 13px;color:DarkGoldenrod;}'";
                     else
                        if ("Critical" == level)
                           strStyle = "style='{font-family: Arial, Helvetica, sans-serif; font-size: 13px;color:red;}'";
                     break;
               }
/*
					if (ds.Tables[0].Rows[i]["AlarmState"].ToString() =="Accepted")
					{
						strStyle="style='{font-family: Arial, Helvetica, sans-serif; font-size: 13px;color:green;}'";
					}	

					if (ds.Tables[0].Rows[i]["AlarmState"].ToString() =="Closed")
					{
						strStyle="style='{font-family: Arial, Helvetica, sans-serif; font-size: 13px;color:#C0C0C0;}'";
					}	


					if (ds.Tables[0].Rows[i]["AlarmState"].ToString() =="New" && ds.Tables[0].Rows[i]["AlarmLevel"].ToString() =="Warning")
					{
						strStyle="style='{font-family: Arial, Helvetica, sans-serif; font-size: 13px;color:DarkGoldenrod;}'";
					}	


					if (ds.Tables[0].Rows[i]["AlarmState"].ToString() =="New" && ds.Tables[0].Rows[i]["AlarmLevel"].ToString() =="Critical")
					{
						strStyle="style='{font-family: Arial, Helvetica, sans-serif; font-size: 13px;color:red;}'";
					}	



					// --- Hide closed alarms
					
					if (ds.Tables[0].Rows[i]["AlarmState"].ToString() !="Closed")
					{
						str=str+"_Items["+AlarmCount.ToString()+"]=\"<a " +strStyle+ " class='infoevt'>"+Convert.ToDateTime(ds.Tables[0].Rows[i]["TimeCreated"].ToString().TrimEnd()).ToLongTimeString()+" "+ds.Tables[0].Rows[i]["vehicleDescription"].ToString().TrimEnd() +"["+ds.Tables[0].Rows[i]["AlarmDescription"].ToString().TrimEnd()+"]" +"</a>\";";
						
						AlarmCount++;
					}
 */ 
					//----------------------
				
				}

				AlarmList=str;
				 
			}
			catch(Exception Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.Message.ToString()+" User:"+sn.UserID.ToString()+" Form:frmAlarmScrolling.aspx"));    
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
