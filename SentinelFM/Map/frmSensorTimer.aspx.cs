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
using VLF.CLS.Interfaces;
using System.Configuration;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace SentinelFM.Map
{
	/// <summary>
	/// Summary description for frmSensorTimer.
	/// </summary>
	public partial class frmSensorTimer : SentinelFMBasePage
	{

		
		protected bool reloadMap ;
        protected string SensorPageUrl;
		


		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				if (sn.Map.TimerStatus==false) 
					return;

				int cmdStatus = 0;
		
				LocationMgr.Location dbl=new LocationMgr.Location();

                SensorPageUrl = "frmSensorsInfo.aspx";
                string ReeferOrganizationId = ConfigurationManager.AppSettings["ReeferOrganizationId"];
                if (!String.IsNullOrEmpty(ReeferOrganizationId))
                {
                    char[] delimiters = new char[] { ',', ';' };
                    List<int> organizations = ReeferOrganizationId.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Where(x => Regex.IsMatch(x, @"\d")).Select(n => int.Parse(n)).ToList();

                    if (organizations.Contains(sn.User.OrganizationId))
                    {
                        SensorPageUrl = "frmSensorsInfo_Reefer.aspx";
                    }
                }
                

				if( objUtil.ErrCheck( dbl.GetCommandStatus(  sn.UserID ,sn.SecId  ,Convert.ToInt32(sn.Cmd.BoxId),sn.Cmd.ProtocolTypeId,    ref cmdStatus ),false)) 
					if( objUtil.ErrCheck( dbl.GetCommandStatus(  sn.UserID ,sn.SecId  ,Convert.ToInt32(sn.Cmd.BoxId),sn.Cmd.ProtocolTypeId,  ref cmdStatus ),true)) 
					{
						sn.Map.TimerStatus=false;
                        sn.Cmd.SendCommandMessage = (string)base.GetLocalResourceObject("msgGetCommanStatusFailed"); 
						//Response.Write("<script language='javascript'> clearTimeout();  parent.frmSensorsInfo.location.href='frmSensorsInfo.aspx'; </script>") ; 
                        Response.Write("<script language='javascript'> try {clearTimeout();} catch(err) {}  parent.frmSensorsInfo.location.href='" + SensorPageUrl + "'; </script>"); 
						return;
					}
						
				sn.Cmd.Status=(CommandStatus) cmdStatus; 
					  
				if( (cmdStatus == (int)CommandStatus.Ack) || (cmdStatus == (int)CommandStatus.CommTimeout) || (cmdStatus == (int)CommandStatus.Pending) )
				{
					//Dual Communication mode

					if (cmdStatus == (int)CommandStatus.CommTimeout  && sn.Cmd.DualComm==true)  
					{
						
						short CommModeId=0;
						short AlternativeProtocol=Convert.ToInt16(sn.Cmd.DsProtocolTypes.Tables[0].Rows[1]["BoxProtocolTypeId"]);
						sn.Cmd.ProtocolTypeId=AlternativeProtocol;
						CommModeId=Convert.ToInt16(sn.Cmd.DsProtocolTypes.Tables[0].Rows[1]["CommModeId"]);
						sn.Cmd.CommModeId=CommModeId;
						
						bool cmdSent = false;

						Int64 sessionTimeOut=0;
						if (objUtil.ErrCheck( dbl.SendCommand     (    sn.UserID , sn.SecId ,DateTime.Now  ,sn.Cmd.BoxId,sn.Cmd.CommandId  ,sn.Cmd.CommandParams,ref AlternativeProtocol,ref CommModeId,ref cmdSent,ref sessionTimeOut ),false ) )
							if (objUtil.ErrCheck( dbl.SendCommand     (    sn.UserID , sn.SecId ,DateTime.Now  ,sn.Cmd.BoxId,sn.Cmd.CommandId  ,sn.Cmd.CommandParams,ref AlternativeProtocol,ref CommModeId,ref cmdSent,ref sessionTimeOut ),true ) )
							{
								sn.Cmd.DualComm=false;
								sn.Map.TimerStatus=false;
								System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,"Sent command failed. Communication mode:"+ Convert.ToString((VLF.CLS.Def.Enums.CommMode) CommModeId)+ ". User:"+sn.UserID.ToString()+" Form:"+Page.GetType().Name));    			
								//Response.Write("<script language='javascript'> clearTimeout();  parent.frmSensorsInfo.location.href='frmSensorsInfo.aspx'; </script>") ; 
                                Response.Write("<script language='javascript'> try {clearTimeout();} catch(err) {}  parent.frmSensorsInfo.location.href='" + SensorPageUrl + "'; </script>"); 
								return;
								
							}


						if (cmdSent)
						{
							sn.Cmd.DualComm=false;
							sn.Map.TimerStatus=true;

							if (sessionTimeOut>0)
								sn.Cmd.GetCommandStatusRefreshFreq = Convert.ToInt64(Math.Round(sessionTimeOut/15.0))*1000; 	
							else
								sn.Cmd.GetCommandStatusRefreshFreq=2000;

							Response.Write("<script language='javascript'>window.setTimeout('location.reload(true)',"+sn.Cmd.GetCommandStatusRefreshFreq.ToString() +")</script>") ;
							return;
						}
					}
					
					// Scheduled Task
                    if ((sn.Cmd.SchCommand) && (cmdStatus == (int)CommandStatus.CommTimeout)) 
                    {
                        if (sn.Cmd.ProtocolTypeId==-1 || sn.Cmd.CommModeId==-1)
                        {
                            sn.Map.TimerStatus=false;
                            sn.Cmd.SendCommandMessage = (string)base.GetLocalResourceObject("msgScheduledCommandWrongConf") + sn.Cmd.BoxId; 
                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,"Sent AutomaticCommand failed. User:"+sn.UserID.ToString()+" Form:"+Page.GetType().Name));    			
                            //Response.Write("<script language='javascript'> clearTimeout();  parent.frmSensorsInfo.location.href='frmSensorsInfo.aspx'; </script>") ; 
                            Response.Write("<script language='javascript'> try {clearTimeout();} catch(err) {}  parent.frmSensorsInfo.location.href='" + SensorPageUrl + "'; </script>"); 
                            return;
                        }
						
                        Int16 ProtocolId=sn.Cmd.ProtocolTypeId;
                        Int16 CommModeId=sn.Cmd.CommModeId;
                        Int64 TaskId=0;


                        
                        if (objUtil.ErrCheck( dbl.SendAutomaticCommand      (    sn.UserID , sn.SecId ,sn.Cmd.BoxId,sn.Cmd.CommandId  ,DateTime.Now, sn.Cmd.CommandParams,ref ProtocolId,ref CommModeId,sn.Cmd.SchPeriod    ,sn.Cmd.SchInterval ,false,ref TaskId),false )  )
                            if (objUtil.ErrCheck( dbl.SendAutomaticCommand      (    sn.UserID , sn.SecId ,sn.Cmd.BoxId,sn.Cmd.CommandId  ,DateTime.Now, sn.Cmd.CommandParams,ref ProtocolId,ref CommModeId,sn.Cmd.SchPeriod    ,sn.Cmd.SchInterval ,false,ref TaskId),true )  )
                            {
                                sn.Map.TimerStatus=false;
                                sn.Cmd.SendCommandMessage = (string)base.GetLocalResourceObject("msgScheduledCommandFailed"); 
                                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,"Sent AutomaticCommand failed. User:"+sn.UserID.ToString()+" Form:"+Page.GetType().Name));    			
                                //Response.Write("<script language='javascript'> clearTimeout();  parent.frmSensorsInfo.location.href='frmSensorsInfo.aspx'; </script>") ; 
                                Response.Write("<script language='javascript'> try {clearTimeout();} catch(err) {}  parent.frmSensorsInfo.location.href='" + SensorPageUrl + "'; </script>"); 
                                return;
                            }


                         sn.Cmd.SendCommandMessage = (string)base.GetLocalResourceObject("msgScheduledCommandSend");

                    }

					
					sn.Map.TimerStatus=false;
					//Response.Write("<script language='javascript'> clearTimeout();  parent.frmSensorsInfo.location.href='frmSensorsInfo.aspx'; </script>") ; 
                    Response.Write("<script language='javascript'> try {clearTimeout();} catch(err) {}  parent.frmSensorsInfo.location.href='" + SensorPageUrl + "'; </script>");                     
								
				}	
				else
				{
					sn.Map.TimerStatus=true;
					Response.Write("<script language='javascript'>window.setTimeout('location.reload(true)',"+sn.Cmd.GetCommandStatusRefreshFreq.ToString() +")</script>") ;
				}
			}
			catch(Exception Ex)
			{
				sn.Map.TimerStatus=false;
				//Response.Write("<script language='javascript'> clearTimeout();  parent.frmSensorsInfo.location.href='frmSensorsInfo.aspx'; </script>") ; 
                Response.Write("<script language='javascript'> try {clearTimeout();} catch(err) {}  parent.frmSensorsInfo.location.href='" + SensorPageUrl + "'; </script>"); 
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.Message.ToString()+" User:"+sn.UserID.ToString()+" Form:"+Page.GetType().Name));    
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
