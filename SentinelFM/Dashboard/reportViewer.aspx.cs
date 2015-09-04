using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using VLF.CLS;
using VLF.Reports;
using System.Globalization;
using System.Data;
 
namespace SentinelFM
{
    public partial class DashBoard_reportViewer : SentinelFMBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            CrystalRpt.CrystalRpt cr = new CrystalRpt.CrystalRpt();
            string reportId = Request.QueryString["reportId"].ToString();
            string FleetId = "";
            string data = "";


            string ReportPath = "";
          


            DataSet dsFleets = new DataSet();
            dsFleets = sn.User.GetUserFleets(sn);
            int allVehiclesFleetId = 0;
            DataRow[] drArr = dsFleets.Tables[0].Select("FleetName='All Vehicles'");
            if (drArr.Length > 0)
                allVehiclesFleetId = Convert.ToInt32(drArr[0]["FleetId"]);
            else
                allVehiclesFleetId = Convert.ToInt32(dsFleets.Tables[0].Rows[0]["FleetId"]); 
            
            DateTime frmDate = DateTime.Now;
            DateTime toDate = DateTime.Now;

            switch (reportId)
            {

                case "1":

                       FleetId = Request.QueryString["fleetId"].ToString();
                       data = Request.QueryString["data"].ToString();
                       frmDate =   Convert.ToDateTime(System.DateTime.Now.ToShortDateString() + " 12:00 AM").AddHours(-Convert.ToInt32(data));
                       toDate =  Convert.ToDateTime(System.DateTime.Now.ToShortDateString() + " 12:00 AM");


                       if (sn.User.OrganizationId != 951)
                       {
                           if (objUtil.ErrCheck(cr.ViolationReport_NewTZ(sn.UserID, sn.SecId, Convert.ToInt32(FleetId), frmDate.ToString(), toDate.ToString(), "63*75", 1, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                               if (objUtil.ErrCheck(cr.ViolationReport_NewTZ(sn.UserID, sn.SecId, Convert.ToInt32(FleetId), frmDate.ToString(), toDate.ToString(), "63*75", 1, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                               {
                                   System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Violation failed. User:" + sn.UserID.ToString()));
                                   return;
                               }
                       }
                       else
                       {
                           if (objUtil.ErrCheck(cr.evtViolation_Report(sn.UserID, sn.SecId, Convert.ToInt32(FleetId), frmDate, toDate, 1, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                               if (objUtil.ErrCheck(cr.evtViolation_Report(sn.UserID, sn.SecId, Convert.ToInt32(FleetId), frmDate, toDate, 1, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                               {
                                   //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                                   System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Violation failed. User:" + sn.UserID.ToString()));
                                   return;
                               }
                       }

                    //if (objUtil.ErrCheck(cr.ViolationReportWithScore(sn.UserID, sn.SecId, Convert.ToInt32(FleetId), frmDate.ToString(), toDate.ToString(), "63;10;20;50;20;10;20;10;50", 1, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                    //    if (objUtil.ErrCheck(cr.ViolationReportWithScore(sn.UserID, sn.SecId, Convert.ToInt32(FleetId), frmDate.ToString(), toDate.ToString(), "63;10;20;50;20;10;20;10;50", 1, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                    //    {
                    //        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "ViolationSummary failed. User:" + sn.UserID.ToString()));
                    //        return;
                    //    }


                       //if (objUtil.ErrCheck(cr.GetExtendedSpeedSummaryViolationsReportForFleet(sn.UserID, sn.SecId, Convert.ToInt32(FleetId), frmDate.ToString(), toDate.ToString(), 2,1, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                       //    if (objUtil.ErrCheck(cr.GetExtendedSpeedSummaryViolationsReportForFleet(sn.UserID, sn.SecId, Convert.ToInt32(FleetId), frmDate.ToString(), toDate.ToString(), 2, 1, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true ))
                       //    {
                       //        //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                       //        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GeoZoneSummaryReport failed. User:" + sn.UserID.ToString()));
                       //        return;
                       //    }


                     

                    break;

                case "2":

                      if (sn.User.OrganizationId == 570)
                  {
                      if (objUtil.ErrCheck(cr.GetVehiclesStatusByDateReport_ActiveVehicles_NewTZ(sn.UserID, sn.SecId, sn.User.OrganizationId, allVehiclesFleetId, 1, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, 1, ref ReportPath), false))
                          if (objUtil.ErrCheck(cr.GetVehiclesStatusByDateReport_ActiveVehicles_NewTZ(sn.UserID, sn.SecId, sn.User.OrganizationId, allVehiclesFleetId, 1, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, 1, ref ReportPath), true))
                          {

                              System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Vehicle Status report failed. User:" + sn.UserID.ToString()));
                              return;
                          }
                  }
                  else
                  {
                      if (objUtil.ErrCheck(cr.GetVehiclesStatusByDateReport_NewTZ(sn.UserID, sn.SecId, sn.User.OrganizationId, allVehiclesFleetId, 1, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                          if (objUtil.ErrCheck(cr.GetVehiclesStatusByDateReport_NewTZ(sn.UserID, sn.SecId, sn.User.OrganizationId, allVehiclesFleetId, 1, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                          {
                              System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Vehicle Status report failed. User:" + sn.UserID.ToString()));
                              return;
                          }
                  }
                  break;



                case "3":
                     
                    FleetId = Request.QueryString["fleetId"].ToString();
                    data = Request.QueryString["data"].ToString();
                    
                    frmDate =   Convert.ToDateTime(System.DateTime.Now.ToShortDateString() + " 12:00 AM").AddHours(-Convert.ToInt32(data));
                    toDate =  Convert.ToDateTime(System.DateTime.Now.ToShortDateString() + " 12:00 AM");



                    if (objUtil.ErrCheck(cr.evtFactEvents_Report(sn.UserID, sn.SecId, Convert.ToInt32(FleetId), frmDate, toDate,  1, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                        if (objUtil.ErrCheck(cr.evtFactEvents_Report(sn.UserID, sn.SecId, Convert.ToInt32(FleetId), frmDate, toDate, 1, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true ))
                        {

                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Fact Events Report failed. User:" + sn.UserID.ToString()));
                            return;
                        }

                    //if (objUtil.ErrCheck(cr.GetIdlingDetailsReportNew(sn.UserID, sn.SecId, Convert.ToInt32(FleetId), frmDate.ToString() , toDate.ToString(), 3, 1, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                    //    if (objUtil.ErrCheck(cr.GetIdlingDetailsReportNew(sn.UserID, sn.SecId, Convert.ToInt32(FleetId), frmDate.ToString(), toDate.ToString(), 3, 1, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                    //  {
                         
                    //      System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GetActivitySummaryReportPerVehicle failed. User:" + sn.UserID.ToString()));
                    //      return;
                    //  }

                  break;



                case "4":

                  FleetId = Request.QueryString["fleetId"].ToString();
                  data = Request.QueryString["data"].ToString();

                  if (objUtil.ErrCheck(cr.AHA_Report_NewTZ(sn.UserID, sn.SecId, Convert.ToInt32(FleetId), Convert.ToInt32(data), 1, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                      if (objUtil.ErrCheck(cr.AHA_Report_NewTZ(sn.UserID, sn.SecId, Convert.ToInt32(FleetId), Convert.ToInt32(data), 1, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                      {

                          System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GetActivitySummaryReportPerVehicle failed. User:" + sn.UserID.ToString()));
                          return;
                      }

                  break;

                    
            }

            if (ReportPath!="")
                Response.Redirect(ReportPath);  
            else
                Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 180px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>No data for selected period<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");


        }
    }
}