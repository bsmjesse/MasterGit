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

namespace SentinelFM.Map
{
	/// <summary>
	/// Summary description for frmTimerPosition.
	/// </summary>
	public partial class frmTimerPosition : SentinelFMBasePage
	{
	
			
			protected bool reloadMap ;
			protected void Page_Load(object sender, System.EventArgs e)
			{

				try
				{

					if (sn.Map.TimerStatus==false) 
						return;

					int cmdStatus = 0;
					if(sn==null || sn.Map.DsFleetInfo==null)
					{
						sn.Map.TimerStatus=false;
						string str="";
						str="top.document.all('TopFrame').cols='0,*';";
						Response.Write("<SCRIPT Language='javascript'>" + str + "window.open('../Login.aspx','_top') </SCRIPT>");
						return;
					}

		

					LocationMgr.Location dbl=new LocationMgr.Location();
					bool blnReload=false;
                    
#if NEW_SLS  
                    int[] ArrCmdStatus =new int[sn.Cmd.ArrBoxId.Count];
                    int[] ArrBoxId = sn.Cmd.ArrBoxId.ToArray();
                    short [] ArrProtocolType = sn.Cmd.ArrProtocolType.ToArray();

                    if (objUtil.ErrCheck(dbl.GetCommandStatusFromMultipleVehicles(sn.UserID, sn.SecId, ArrBoxId, ArrProtocolType, ref ArrCmdStatus), false))
                        if (objUtil.ErrCheck(dbl.GetCommandStatusFromMultipleVehicles(sn.UserID, sn.SecId, ArrBoxId, ArrProtocolType, ref ArrCmdStatus), true))
                     {
                     }

                    sn.Cmd.ArrCmdStatus.Clear();
                    sn.Cmd.ArrBoxId.Clear();
                    sn.Cmd.ArrProtocolType.Clear(); 

                    for (int i = 0; i < ArrCmdStatus.Length; i++)
                    {
                        cmdStatus = ArrCmdStatus[i];
                        if ((cmdStatus == (int)CommandStatus.Ack) || (cmdStatus == (int)CommandStatus.CommTimeout) || (cmdStatus == (int)CommandStatus.Pending))
                        {
                            if ((cmdStatus == (int)CommandStatus.Pending) || (cmdStatus == (int)CommandStatus.CommTimeout))
                            {
                                DataRow dr;
                                dr = sn.Cmd.DtUpdatePositionFails.NewRow();
                                dr["VehicleDesc"] = sn.Cmd.ArrVehicle[i];
                                dr["Status"] = cmdStatus.ToString();
                                sn.Cmd.DtUpdatePositionFails.Rows.Add(dr);

                                // Clear flag GPS not valid for time-out vechicles.
                                //rowItem["LastCommunicatedDateTime"] = rowItem["OriginDateTime"];
                            }
                        }
                        else
                        {
                            sn.Cmd.ArrBoxId.Add(ArrBoxId[i]);
                            sn.Cmd.ArrProtocolType.Add(ArrProtocolType[i]);
                            sn.Cmd.ArrCmdStatus.Add(ArrCmdStatus[i]);   
                            blnReload = true;
                        }
                    }


#else	            

                    foreach (DataRow rowItem in sn.Map.DsFleetInfo.Tables[0].Rows)
                    {
                        if (rowItem["chkBoxShow"].ToString().ToLower() == "true" && !Convert.ToBoolean(rowItem["Updated"]))
                        {
                            if (objUtil.ErrCheck(dbl.GetCommandStatus(sn.UserID, sn.SecId, Convert.ToInt32(rowItem["BoxId"]), Convert.ToInt16(rowItem["ProtocolId"]), ref cmdStatus), false))
                                if (objUtil.ErrCheck(dbl.GetCommandStatus(sn.UserID, sn.SecId, Convert.ToInt32(rowItem["BoxId"]), Convert.ToInt16(rowItem["ProtocolId"]), ref cmdStatus), true))
                                {
                                    cmdStatus = (int)CommandStatus.CommTimeout;
                                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Session for Box: " + rowItem["BoxId"].ToString() + " and User: " + sn.UserID.ToString() + " does not exist. Form:frmTimerPosition.aspx"));
                                }

                            sn.Cmd.Status = (CommandStatus)cmdStatus;

                            if ((cmdStatus == (int)CommandStatus.Ack) || (cmdStatus == (int)CommandStatus.CommTimeout) || (cmdStatus == (int)CommandStatus.Pending))
                            {
                                if ((cmdStatus == (int)CommandStatus.Pending) || (cmdStatus == (int)CommandStatus.CommTimeout))
                                {
                                    DataRow dr;
                                    dr = sn.Cmd.DtUpdatePositionFails.NewRow();
                                    dr["VehicleDesc"] = rowItem["Description"];
                                    dr["Status"] = cmdStatus.ToString();
                                    sn.Cmd.DtUpdatePositionFails.Rows.Add(dr);

                                    // Clear flag GPS not valid for time-out vechicles.
                                    rowItem["LastCommunicatedDateTime"] = rowItem["OriginDateTime"];
                                }

                                rowItem["Updated"] = (int)cmdStatus;
                            }
                            else
                            {
                                blnReload = true;
                            }
                        }
                    }

#endif
					if (blnReload)
					{
						sn.Map.TimerStatus=true;
						Response.Write("<script language='javascript'>window.setTimeout('location.reload(true)',"+sn.Cmd.GetCommandStatusRefreshFreq.ToString()+")</script>") ;
					}
					else
					{
				
    			      sn.Map.TimerStatus=false;

						

                    //Changed by Devin Begin

                    //if (sn.User.MapType == VLF.MAP.MapType.LSD)
                    //    Response.Write("<script language='javascript'> clearTimeout();  parent.frmFleetInfo.location.href='frmFleetInfoNew.aspx'; parent.frmVehicleMap.location.href='frmLSDmap.aspx'; </script>");
                    //else if (sn.User.MapType == VLF.MAP.MapType.VirtualEarth)
                    //    Response.Write("<script language='javascript'> clearTimeout();  parent.frmFleetInfo.location.href='frmFleetInfoNew.aspx'; parent.frmVehicleMap.location.href='../MapVE/VEMap.aspx'; </script>");
                    //else
                    //    Response.Write("<script language='javascript'> clearTimeout();  parent.frmFleetInfo.location.href='frmFleetInfoNew.aspx'; parent.frmVehicleMap.location.href='frmVehicleMap.aspx'; </script>");
                    
                    //Devin
                    if (sn.User.MapType == VLF.MAP.MapType.LSD)
                        Response.Write("<script language='javascript'> clearTimeout();  var loc=parent.frmFleetInfo.location.toString().toLowerCase(); if (loc.indexOf('/new/frmfleetinfonew.aspx')>0) parent.frmFleetInfo.UpdatePositionResult(); else parent.frmFleetInfo.location.href='frmFleetInfoNew.aspx';  parent.frmVehicleMap.location.href='frmLSDmap.aspx'; </script>");
                    else if (sn.User.MapType == VLF.MAP.MapType.VirtualEarth)
                        Response.Write("<script language='javascript'> clearTimeout();  var loc=parent.frmFleetInfo.location.toString().toLowerCase(); if (loc.indexOf('/new/frmfleetinfonew.aspx')>0) parent.frmFleetInfo.UpdatePositionResult(); else parent.frmFleetInfo.location.href='frmFleetInfoNew.aspx';  parent.frmVehicleMap.location.href='../MapVE/VEMap.aspx'; </script>");
                    else
                        Response.Write("<script language='javascript'> clearTimeout();  var loc=parent.frmFleetInfo.location.toString().toLowerCase(); if (loc.indexOf('/new/frmfleetinfonew.aspx')>0) parent.frmFleetInfo.UpdatePositionResult(); else parent.frmFleetInfo.location.href='frmFleetInfoNew.aspx';  parent.frmVehicleMap.location.href='frmVehicleMap.aspx'; </script>");

                    //End


					}
				}

				catch(Exception Ex)
				{
					sn.Map.TimerStatus=false;
					System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.Message.ToString()+" User:"+sn.UserID.ToString()+" Form:"+Page.GetType().Name));    
                    System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                    ExceptionLogger(trace);
     
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
