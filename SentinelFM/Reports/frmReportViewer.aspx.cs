using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Diagnostics;
using VLF.CLS;
using VLF.Reports;
 
namespace SentinelFM
{
    public partial class frmReportViewer : SentinelFMBasePage
   {

      //
      
      protected void Page_Load(object sender, EventArgs e)
      {
         CrystalRpt.CrystalRpt cr = new CrystalRpt.CrystalRpt();
         cr.Timeout = -1; // infinite time out.

        
         
         string ReportType = sn.Report.ReportType;
         string ReportPath = "";
         bool RequestOverflowed = false;
         bool OutMaxOverflowed = false;
         string strUrl = "";
         Stopwatch watch = new Stopwatch();

         int VehicleId = 0;
         Single Cost = 0;
         Int16 sensorNum = 0;
         string LicensePlate = "";
         int Type = 0;
         Int16 Summary = 0;
         Int16 tmpValue = 0;
         Int32 driverId = 0;
         string[] tmp ;
         string ColorFilter = "";
         Int16 IdlingThreshold = 0;
         Int16 PostedSpeedOnly = 0;
         //if (sn.Report.IsFleet)
         //    sn.Report.IsFleet = true;
         //else
         //    sn.Report.IsFleet = false;

         watch.Reset();
         watch.Start();

         try
         {
             switch (ReportType)
             {
                 # region Trip Details Report
                 case "0":
                     if (!sn.Report.IsFleet)
                     {


                         if (objUtil.ErrCheck(cr.TripDetailsReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref RequestOverflowed, ref OutMaxOverflowed, ref ReportPath), false))
                             if (objUtil.ErrCheck(cr.TripDetailsReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref RequestOverflowed, ref OutMaxOverflowed, ref ReportPath), true))
                             {
                                 //  Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                                 ReportFailed();
                                 System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "TripDetailsReport  failed. User:" + sn.UserID.ToString()));
                                 return;
                             }
                     }
                     else
                     {
                         if (objUtil.ErrCheck(cr.TripFleetDetailsReportOneByOne(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath, ref RequestOverflowed, ref OutMaxOverflowed), false))
                             if (objUtil.ErrCheck(cr.TripFleetDetailsReportOneByOne(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath, ref RequestOverflowed, ref OutMaxOverflowed), true))
                             {
                                 //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                                 ReportFailed();
                                 System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "TripFleetDetailsReport failed. User:" + sn.UserID.ToString()));
                                 return;
                             }
                     }
                     break;
                 # endregion
                 # region Trip Summary Report
                 case "1":
                     if (!sn.Report.IsFleet)
                     {
                         if (objUtil.ErrCheck(cr.TripSummaryReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath, ref RequestOverflowed, ref OutMaxOverflowed), false))
                             if (objUtil.ErrCheck(cr.TripSummaryReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath, ref RequestOverflowed, ref OutMaxOverflowed), true))
                             {
                                 //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                                 ReportFailed();
                                 System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "TripSummaryReport failed. User:" + sn.UserID.ToString()));
                                 return;
                             }
                     }
                     else
                     {
                         if (objUtil.ErrCheck(cr.TripFleetSummaryReportOneByOne(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath, ref RequestOverflowed, ref OutMaxOverflowed), false))
                             if (objUtil.ErrCheck(cr.TripFleetSummaryReportOneByOne(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath, ref RequestOverflowed, ref OutMaxOverflowed), true))
                             {
                                 // Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");

                                 ReportFailed();
                                 System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "TripFleetSummaryReport failed. User:" + sn.UserID.ToString()));
                                 return;
                             }
                     }
                     break;
                 # endregion
                 # region Alarms Report
                 case "2":

                     // Response.Redirect("Report_Alarms.aspx");

                     if (!sn.Report.IsFleet)
                     {
                         if (objUtil.ErrCheck(cr.AlarmsReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath, ref RequestOverflowed, ref OutMaxOverflowed), false))
                             if (objUtil.ErrCheck(cr.AlarmsReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath, ref RequestOverflowed, ref OutMaxOverflowed), true))
                             {
                                 //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                                 ReportFailed();
                                 System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "AlarmsReport failed. User:" + sn.UserID.ToString()));
                                 return;
                             }
                     }
                     else
                     {
                         if (objUtil.ErrCheck(cr.AlarmsFleetReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath, ref RequestOverflowed, ref OutMaxOverflowed), false))
                             if (objUtil.ErrCheck(cr.AlarmsFleetReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath, ref RequestOverflowed, ref OutMaxOverflowed), true))
                             {
                                 //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                                 ReportFailed();
                                 System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "AlarmsFleetReport failed. User:" + sn.UserID.ToString()));
                                 return;
                             }
                     }
                     break;
                 # endregion
                 # region History Report
                 case "3":
                     if (objUtil.ErrCheck(cr.HistoryReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, -1, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath, ref RequestOverflowed, ref OutMaxOverflowed), false))
                         if (objUtil.ErrCheck(cr.HistoryReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, -1, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath, ref RequestOverflowed, ref OutMaxOverflowed), true))
                         {
                             //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "HistoryReport failed. User:" + sn.UserID.ToString()));
                             return;
                         }

                     break;
                 # endregion
                 # region Stop Report
                 case "4":
                     if (!sn.Report.IsFleet)
                     {
                         if (objUtil.ErrCheck(cr.StopReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath, ref RequestOverflowed, ref OutMaxOverflowed), false))
                             if (objUtil.ErrCheck(cr.StopReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath, ref RequestOverflowed, ref OutMaxOverflowed), true))
                             {
                                 //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                                 ReportFailed();
                                 System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "StopReport failed. User:" + sn.UserID.ToString()));
                                 return;
                             }
                     }
                     else
                     {
                         if (objUtil.ErrCheck(cr.StopFleetReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath, ref RequestOverflowed, ref OutMaxOverflowed), false))
                             if (objUtil.ErrCheck(cr.StopFleetReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath, ref RequestOverflowed, ref OutMaxOverflowed), true))
                             {
                                 //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                                 ReportFailed();
                                 System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "StopFleetReport failed. User:" + sn.UserID.ToString()));
                                 return;
                             }
                     }
                     break;
                 # endregion
                 # region Messages Report
                 case "5":
                     if (!sn.Report.IsFleet)
                     {
                         if (objUtil.ErrCheck(cr.MessagesReport(sn.UserID, sn.SecId, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath, ref RequestOverflowed, ref OutMaxOverflowed), false))
                             if (objUtil.ErrCheck(cr.MessagesReport(sn.UserID, sn.SecId, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath, ref RequestOverflowed, ref OutMaxOverflowed), true))
                             {
                                 //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                                 ReportFailed();
                                 System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "MessagesReport failed. User:" + sn.UserID.ToString()));
                                 return;
                             }
                     }
                     else
                     {
                         if (objUtil.ErrCheck(cr.MessagesFleetReport(sn.UserID, sn.SecId, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath, ref RequestOverflowed, ref OutMaxOverflowed), false))
                             if (objUtil.ErrCheck(cr.MessagesFleetReport(sn.UserID, sn.SecId, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath, ref RequestOverflowed, ref OutMaxOverflowed), true))
                             {
                                 //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                                 ReportFailed();

                                 System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "StopFleetReport failed. User:" + sn.UserID.ToString()));
                                 return;
                             }
                     }
                     break;
                 # endregion
                 # region Exception Report
                 case "6":
                     if (!sn.Report.IsFleet)
                     {
                         if (objUtil.ErrCheck(cr.ExceptionReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath, ref RequestOverflowed, ref OutMaxOverflowed), false))
                             if (objUtil.ErrCheck(cr.ExceptionReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath, ref RequestOverflowed, ref OutMaxOverflowed), true))
                             {
                                 Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                                 System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "ExceptionReport failed. User:" + sn.UserID.ToString()));
                                 return;
                             }
                     }
                     else
                     {
                         if (objUtil.ErrCheck(cr.ExceptionFleetReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath, ref RequestOverflowed, ref OutMaxOverflowed), false))
                             if (objUtil.ErrCheck(cr.ExceptionFleetReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath, ref RequestOverflowed, ref OutMaxOverflowed), true))
                             {
                                 Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                                 System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "ExceptionFleetReport failed. User:" + sn.UserID.ToString()));
                                 return;
                             }
                     }
                     break;
                 # endregion
                 # region Off Hours Report
                 case "8":
                     if (!sn.Report.IsFleet)
                     {
                         if (objUtil.ErrCheck(cr.OffHoursReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath, ref RequestOverflowed, ref OutMaxOverflowed), false))
                             if (objUtil.ErrCheck(cr.OffHoursReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath, ref RequestOverflowed, ref OutMaxOverflowed), true))
                             {
                                 //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                                 ReportFailed();
                                 System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "OffHoursReport failed. User:" + sn.UserID.ToString()));
                                 return;
                             }
                     }
                     else
                     {
                         if (objUtil.ErrCheck(cr.OffHoursFleetReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath, ref RequestOverflowed, ref OutMaxOverflowed), false))
                             if (objUtil.ErrCheck(cr.OffHoursFleetReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath, ref RequestOverflowed, ref OutMaxOverflowed), true))
                             {
                                 //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                                 ReportFailed();
                                 System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "OffHoursFleetReport failed. User:" + sn.UserID.ToString()));
                                 return;
                             }
                     }
                     break;
                 # endregion
                 # region Landmark Activity Report
                 case "9":
                     if (!sn.Report.IsFleet)
                     {
                         if (objUtil.ErrCheck(cr.LandmarkActivityReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref RequestOverflowed, ref OutMaxOverflowed, ref ReportPath), false))
                             if (objUtil.ErrCheck(cr.LandmarkActivityReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref RequestOverflowed, ref OutMaxOverflowed, ref ReportPath), true))
                             {
                                 //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                                 ReportFailed();
                                 System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "LandmarkActivity failed. User:" + sn.UserID.ToString()));
                                 return;
                             }
                     }
                     else
                     {
                         if (objUtil.ErrCheck(cr.LandmarkFleetActivityReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref RequestOverflowed, ref OutMaxOverflowed, ref ReportPath), false))
                             if (objUtil.ErrCheck(cr.LandmarkFleetActivityReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref RequestOverflowed, ref OutMaxOverflowed, ref ReportPath), true))
                             {
                                 //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                                 ReportFailed();
                                 System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "LandmarkActivity failed. User:" + sn.UserID.ToString()));
                                 return;
                             }
                     }
                     break;
                 # endregion
                 # region Fleet Maintenance Report
                 case "10":
                     if (objUtil.ErrCheck(cr.FleetMaintenaceReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath, ref RequestOverflowed, ref OutMaxOverflowed), false))
                         if (objUtil.ErrCheck(cr.FleetMaintenaceReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath, ref RequestOverflowed, ref OutMaxOverflowed), true))
                         {
                             //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "LandmarkActivity failed. User:" + sn.UserID.ToString()));
                             return;
                         }
                     break;
                 # endregion
                 # region Utilization Report
                 case "11":
                     if (objUtil.ErrCheck(cr.UtilizationReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.UtilizationReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {

                             //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();

                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "UtilizationReport failed. User:" + sn.UserID.ToString()));
                             return;
                         }
                     break;
                 # endregion
                 # region Utilization Summary Report
                 case "12":
                     if (objUtil.ErrCheck(cr.UtilizationSummaryReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.UtilizationSummaryReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "UtilizationSummaryReport failed. User:" + sn.UserID.ToString()));
                             return;
                         }
                     break;
                 # endregion
                 # region Utilization Daily Fleet  Report
                 case "13":
                     if (objUtil.ErrCheck(cr.UtilizationDailyFleetReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.UtilizationDailyFleetReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "UtilizationSummaryReport failed. User:" + sn.UserID.ToString()));
                             return;
                         }
                     break;
                 # endregion
                 # region Utilization By VehicleType
                 case "14":
                     if (objUtil.ErrCheck(cr.UtilizationByVehicleTypeReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.UtilizationByVehicleTypeReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             // Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();

                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "UtilizationByVehicleTypeReport failed. User:" + sn.UserID.ToString()));
                             return;
                         }
                     break;
                 # endregion
                 # region Utilization Daily Detail
                 case "15":
                     if (objUtil.ErrCheck(cr.UtilizationDailyDetailReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.UtilizationDailyDetailReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");

                             ReportFailed();

                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "UtilizationDailyDetailReport failed. User:" + sn.UserID.ToString()));
                             return;
                         }
                     break;
                 # endregion
                 # region Weekly Utilization
                 case "16":
                     if (objUtil.ErrCheck(cr.UtilizationWeeklyReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.UtilizationWeeklyReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "UtilizationWeeklyReport failed. User:" + sn.UserID.ToString()));
                             return;
                         }
                     break;
                 # endregion
                 # region Violation details report
                 case "17":
                     //if (!sn.Report.OrganizationHierarchySelected)
                     //{
                         if (objUtil.ErrCheck(cr.ViolationReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                             if (objUtil.ErrCheck(cr.ViolationReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                             {
                                 //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                                 ReportFailed();
                                 System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "ViolationReport failed. User:" + sn.UserID.ToString()));
                                 return;
                             }
                     //}
                     //else
                     //{
                     //    if (objUtil.ErrCheck(cr.ViolationReport_Hierarchy(sn.UserID, sn.SecId, sn.Report.OrganizationHierarchyNodeCode, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                     //        if (objUtil.ErrCheck(cr.ViolationReport_Hierarchy(sn.UserID, sn.SecId, sn.Report.OrganizationHierarchyNodeCode, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                     //        {
                     //            //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                     //            ReportFailed();
                     //            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "ViolationReport failed. User:" + sn.UserID.ToString()));
                     //            return;
                     //        }
                     //}
                     break;
                 # endregion
                 # region Idling Details report
                 case "18":
                     if (objUtil.ErrCheck(cr.IdlingDetailReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.IdlingDetailReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Idling Details report failed. User:" + sn.UserID.ToString()));
                             return;
                         }
                     break;
                 # endregion
                 # region Idling Summary report
                 case "19":
                     //if (objUtil.ErrCheck(cr.IdlingSummaryOrgReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                     //    if (objUtil.ErrCheck(cr.IdlingSummaryOrgReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                     if (objUtil.ErrCheck(cr.IdlingSummaryReportByOrgId(sn.UserID, sn.SecId, sn.User.OrganizationId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.IdlingSummaryReportByOrgId(sn.UserID, sn.SecId, sn.User.OrganizationId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();

                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "IdlingSummaryReport failed. User:" + sn.UserID.ToString()));
                             return;
                         }
                     break;
                 # endregion
                 # region Violation Summary report
                 case "20":

                     //if (!sn.Report.OrganizationHierarchySelected)
                     //{
                         if (objUtil.ErrCheck(cr.ViolationReportWithScore(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                             if (objUtil.ErrCheck(cr.ViolationReportWithScore(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                             {
                                 //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                                 ReportFailed();
                                 System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "ViolationSummary failed. User:" + sn.UserID.ToString()));
                                 return;
                             }
                     //}
                     //else
                     //{
                     //    if (objUtil.ErrCheck(cr.ViolationReportWithScore_Hierarchy(sn.UserID, sn.SecId, sn.Report.OrganizationHierarchyNodeCode, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                     //        if (objUtil.ErrCheck(cr.ViolationReportWithScore_Hierarchy(sn.UserID, sn.SecId, sn.Report.OrganizationHierarchyNodeCode, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                     //        {
                     //            //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                     //            ReportFailed();
                     //            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "ViolationSummary failed. User:" + sn.UserID.ToString()));
                     //            return;
                     //        }
                     //}

                     //string xml = "";
                     //if (objUtil.ErrCheck(cr.ViolationsMonthlyData(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, ref xml), false))
                     //    if (objUtil.ErrCheck(cr.ViolationsMonthlyData(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, ref xml), true ))
                     //    {
                     //        //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                     //        ReportFailed();
                     //        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "ViolationSummary failed. User:" + sn.UserID.ToString()));
                     //        return;
                     //    }

                     break;
                 # endregion
                 # region Landmark summary
                 case "21":
                     if (sn.Report.IsFleet)
                     {
                         if (objUtil.ErrCheck(cr.LandmarkFleetReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                             if (objUtil.ErrCheck(cr.LandmarkFleetReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                             {
                                 //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                                 ReportFailed();
                                 System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "LandmarkSummaryReport failed. User:" + sn.UserID.ToString()));
                                 return;
                             }
                     }
                     else
                     {
                         if (objUtil.ErrCheck(cr.LandmarkVehicleReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                             if (objUtil.ErrCheck(cr.LandmarkVehicleReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                             {
                                 //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                                 ReportFailed();
                                 System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "LandmarkSummaryReport failed. User:" + sn.UserID.ToString()));
                                 return;
                             }
                     }
                     break;
                 # endregion
                 # region Geozone summary
                 case "22":
                     if (sn.Report.IsFleet)
                     {
                         if (objUtil.ErrCheck(cr.GeozoneFleetReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                             if (objUtil.ErrCheck(cr.GeozoneFleetReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                             {
                                 //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                                 ReportFailed();
                                 System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GeoZoneSummaryReport failed. User:" + sn.UserID.ToString()));
                                 return;
                             }
                     }
                     else
                     {
                         if (objUtil.ErrCheck(cr.GeozoneVehicleReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                             if (objUtil.ErrCheck(cr.GeozoneVehicleReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                             {
                                 //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                                 ReportFailed();
                                 System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GeoZoneSummaryReport failed. User:" + sn.UserID.ToString()));
                                 return;
                             }
                     }
                     break;
                 # endregion
                 # region Landmark Details Report
                 case "23":
                     if (sn.Report.IsFleet)
                     {
                         if (objUtil.ErrCheck(cr.LandmarkFleetDetailsReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                             if (objUtil.ErrCheck(cr.LandmarkFleetDetailsReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                             {
                                 //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                                 ReportFailed();
                                 System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "LandmarkDetailsReport failed. User:" + sn.UserID.ToString()));
                                 return;
                             }
                     }
                     else
                     {
                         if (objUtil.ErrCheck(cr.LandmarkVehicleDetailsReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                             if (objUtil.ErrCheck(cr.LandmarkVehicleDetailsReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                             {
                                 //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                                 ReportFailed();
                                 System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "LandmarkDetailsReport failed. User:" + sn.UserID.ToString()));
                                 return;
                             }
                     }
                     break;
                 # endregion
                 # region Inactivity Report
                 case "24":
                     if (sn.Report.IsFleet)
                     {
                         if (objUtil.ErrCheck(cr.InactivityReport4Fleet(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                             if (objUtil.ErrCheck(cr.InactivityReport4Fleet(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                             {
                                 //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                                 ReportFailed();
                                 System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "InactivityReport failed. User:" + sn.UserID.ToString()));
                                 return;
                             }
                     }
                     else
                     {
                         if (objUtil.ErrCheck(cr.InactivityReport4Vehicle(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                             if (objUtil.ErrCheck(cr.InactivityReport4Vehicle(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                             {
                                 //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                                 ReportFailed();
                                 System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "InactivityReport failed. User:" + sn.UserID.ToString()));
                                 return;
                             }
                     }
                     break;
                 # endregion
                 # region Driver Trip Details Report
                 case "25":
                     if (objUtil.ErrCheck(cr.DriverTripDetailsReportOneByOne(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref RequestOverflowed, ref OutMaxOverflowed, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.DriverTripDetailsReportOneByOne(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref RequestOverflowed, ref OutMaxOverflowed, ref ReportPath), true))
                         {
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "DriverTripDetailsReport failed. User:" + sn.UserID.ToString()));
                             //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             return;
                         }
                     break;
                 # endregion
                 # region Driver Trip Summary Report
                 case "26":
                     if (objUtil.ErrCheck(cr.DriverTripSummaryReportOneByOne(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath, ref RequestOverflowed, ref OutMaxOverflowed), false))
                         if (objUtil.ErrCheck(cr.DriverTripSummaryReportOneByOne(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath, ref RequestOverflowed, ref OutMaxOverflowed), true))
                         {
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "DriverTripDetailsReport failed. User:" + sn.UserID.ToString()));
                             //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             return;
                         }
                     break;
                 # endregion
                 # region Driver Violation Details Report
                 case "27":
                     if (objUtil.ErrCheck(cr.DriverViolationReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.DriverViolationReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "DriverTripDetailsReport failed. User:" + sn.UserID.ToString()));
                             //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             return;
                         }
                     break;
                 # endregion
                 # region Driver Violation Summary Report
                 case "28":
                     if (objUtil.ErrCheck(cr.DriverViolationSummaryReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.DriverViolationSummaryReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "DriverTripDetailsReport failed. User:" + sn.UserID.ToString()));
                             //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             return;
                         }
                     break;
                 # endregion
                 # region Fuel Transaction History
                 case "29":
                     if (objUtil.ErrCheck(cr.FuelTransactionHistReport(sn.UserID, sn.SecId, sn.User.OrganizationId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.FuelTransactionHistReport(sn.UserID, sn.SecId, sn.User.OrganizationId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "DriverTripDetailsReport failed. User:" + sn.UserID.ToString()));
                             // Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             return;
                         }
                     break;
                 #endregion
                 #region Geozone Details
                 case "30":
                     if (sn.Report.IsFleet)
                     {
                         if (objUtil.ErrCheck(cr.GeozoneFleetDetailsReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                             if (objUtil.ErrCheck(cr.GeozoneFleetDetailsReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                             {
                                 ReportFailed();
                                 //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                                 System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GeoZoneSummaryReport failed. User:" + sn.UserID.ToString()));
                                 return;
                             }
                     }
                     else
                     {
                         if (objUtil.ErrCheck(cr.GeozoneDetailsVehicleReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                             if (objUtil.ErrCheck(cr.GeozoneDetailsVehicleReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                             {
                                 ReportFailed();
                                 //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                                 System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GeoZoneSummaryReport failed. User:" + sn.UserID.ToString()));
                                 return;
                             }
                     }
                     break;
                 #endregion
                 #region ExtendedDailyUtilizationReport
                 case "31":
                     //if (sn.Report.IsFleet)
                     //{
                     //    if (objUtil.ErrCheck(cr.GetExtendedDailyFleetUtilizationReport (sn.UserID, sn.SecId, sn.Report.FleetId , sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                     //        if (objUtil.ErrCheck(cr.GetExtendedDailyFleetUtilizationReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                     //        {
                     //            Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                     //            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GeoZoneSummaryReport failed. User:" + sn.UserID.ToString()));
                     //            return;
                     //        }
                     //}
                     //else
                     //{


                     VehicleId = Convert.ToInt32(VLF.CLS.Util.PairFindValue(ReportTemplate.RptParamVehicleId, sn.Report.XmlParams));
                     Cost = Convert.ToSingle(Util.PairFindValue(ReportTemplate.RptParamCost, sn.Report.XmlParams));


                     if (objUtil.ErrCheck(cr.GetExtendedDailyVehicleUtilizationReport(sn.UserID, sn.SecId, VehicleId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, Cost, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.GetExtendedDailyVehicleUtilizationReport(sn.UserID, sn.SecId, VehicleId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, Cost, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             ReportFailed();
                             //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GeoZoneSummaryReport failed. User:" + sn.UserID.ToString()));
                             return;
                         }
                     //}
                     break;
                 #endregion
                 #region ExtendedDailyFleetUtilizationReport
                 case "32":
                     Cost = Convert.ToSingle(Util.PairFindValue(ReportTemplate.RptParamCost, sn.Report.XmlParams));

                     if (objUtil.ErrCheck(cr.GetExtendedDailyFleetUtilizationReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, Cost, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.GetExtendedDailyFleetUtilizationReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, Cost, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             // Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GeoZoneSummaryReport failed. User:" + sn.UserID.ToString()));
                             return;
                         }
                     break;
                 #endregion
                 #region ExtendedSpeedViolationReportReport
                 case "33":
                     tmp = sn.Report.XmlParams.Split(';');
                     Type = Convert.ToInt32(tmp[0]);
                     ColorFilter = tmp[1] == "" ? "%" : tmp[1] + "%";
                     //Type = Convert.ToInt32(Util.PairFindValue(ReportTemplate.RptParamIncludeSummary,sn.Report.XmlParams));

                     if (sn.Misc.DsReportSelectedFleets != null && sn.Misc.DsReportSelectedFleets.Tables.Count > 0 && sn.Misc.DsReportSelectedFleets.Tables[0].Rows.Count > 0)
                     {
                         int[] fleetIds = new int[sn.Misc.DsReportSelectedFleets.Tables[0].Rows.Count];
                         int i = 0;
                         foreach (DataRow dr in sn.Misc.DsReportSelectedFleets.Tables[0].Rows)
                         {
                             fleetIds[i] = Convert.ToInt32(dr["FleetId"]);
                             i++;
                         }

                         if (objUtil.ErrCheck(cr.GetExtendedSpeedViolationsReportForFleets(sn.UserID, sn.SecId, fleetIds, sn.Report.FromDate, sn.Report.ToDate, Type, ColorFilter, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                             if (objUtil.ErrCheck(cr.GetExtendedSpeedViolationsReportForFleets(sn.UserID, sn.SecId, fleetIds, sn.Report.FromDate, sn.Report.ToDate, Type, ColorFilter, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))

                             //if (objUtil.ErrCheck(cr.GetExtendedSpeedViolationsReportForFleets_ByRoadSpeed (sn.UserID, sn.SecId, fleetIds, sn.Report.FromDate, sn.Report.ToDate, Type,ColorFilter, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                             //    if (objUtil.ErrCheck(cr.GetExtendedSpeedViolationsReportForFleets_ByRoadSpeed(sn.UserID, sn.SecId, fleetIds, sn.Report.FromDate, sn.Report.ToDate, Type, ColorFilter, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                             {
                                 //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                                 ReportFailed();
                                 System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GeoZoneSummaryReport failed. User:" + sn.UserID.ToString()));
                                 return;
                             }

                     }

                     break;
                 #endregion
                 #region State Mileage Report
                 case "34":
                     Summary = Convert.ToInt16(Util.PairFindValue(ReportTemplate.RptParamIncludeSummary, sn.Report.XmlParams));

                     if (sn.Report.IsFleet)
                     {
                         if (objUtil.ErrCheck(cr.StateMileageReportPerFleet(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, Summary, sn.Report.ReportFormat, ref ReportPath), false))
                             if (objUtil.ErrCheck(cr.StateMileageReportPerFleet(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, Summary, sn.Report.ReportFormat, ref ReportPath), true))
                             {
                                 // Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                                 ReportFailed();
                                 System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GeoZoneSummaryReport failed. User:" + sn.UserID.ToString()));
                                 return;
                             }
                     }
                     else
                     {
                         LicensePlate = Util.PairFindValue(ReportTemplate.RptParamLicensePlate, sn.Report.XmlParams);

                         if (objUtil.ErrCheck(cr.StateMileageReportPerVehicle(sn.UserID, sn.SecId, LicensePlate, sn.Report.FromDate, sn.Report.ToDate, Summary, sn.Report.ReportFormat, ref ReportPath), false))
                             if (objUtil.ErrCheck(cr.StateMileageReportPerVehicle(sn.UserID, sn.SecId, LicensePlate, sn.Report.FromDate, sn.Report.ToDate, Summary, sn.Report.ReportFormat, ref ReportPath), true))
                             {
                                 //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                                 ReportFailed();
                                 System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GeoZoneSummaryReport failed. User:" + sn.UserID.ToString()));
                                 return;
                             }
                     }
                     break;
                 #endregion
                 #region ExtendedSpeedViolationDetailsReportReport
                 case "35":
                     // Type = Convert.ToInt32(Util.PairFindValue(ReportTemplate.RptParamIncludeSummary,sn.Report.XmlParams));

                     tmp = sn.Report.XmlParams.Split(';');
                     Type = Convert.ToInt32(tmp[0]);
                     ColorFilter = tmp[1] == "" ? "%" : tmp[1] + "%";

                     if (sn.Misc.DsReportSelectedFleets != null && sn.Misc.DsReportSelectedFleets.Tables.Count > 0 && sn.Misc.DsReportSelectedFleets.Tables[0].Rows.Count > 0)
                     {
                         int[] fleetIds = new int[sn.Misc.DsReportSelectedFleets.Tables[0].Rows.Count];
                         int i = 0;
                         foreach (DataRow dr in sn.Misc.DsReportSelectedFleets.Tables[0].Rows)
                         {
                             fleetIds[i] = Convert.ToInt32(dr["FleetId"]);
                             i++;
                         }

                         if (objUtil.ErrCheck(cr.GetExtendedSpeedViolationsDetailsReportForFleets(sn.UserID, sn.SecId, fleetIds, sn.Report.FromDate, sn.Report.ToDate, Type, ColorFilter, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                             if (objUtil.ErrCheck(cr.GetExtendedSpeedViolationsDetailsReportForFleets(sn.UserID, sn.SecId, fleetIds, sn.Report.FromDate, sn.Report.ToDate, Type, ColorFilter, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                             {

                                 //if (objUtil.ErrCheck(cr.GetExtendedSpeedViolationsDetailsReportForFleets_RoadSpeed(sn.UserID, sn.SecId, fleetIds, sn.Report.FromDate, sn.Report.ToDate, Type,ColorFilter, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                                 //   if (objUtil.ErrCheck(cr.GetExtendedSpeedViolationsDetailsReportForFleets_RoadSpeed(sn.UserID, sn.SecId, fleetIds, sn.Report.FromDate, sn.Report.ToDate, Type,ColorFilter, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                                 //   {
                                 //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                                 ReportFailed();
                                 System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GeoZoneSummaryReport failed. User:" + sn.UserID.ToString()));
                                 return;
                             }

                     }
                     //else
                     //{
                     //    if (objUtil.ErrCheck(cr.GetExtendedSpeedViolationsDetailsReportForFleet(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, Type, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                     //        if (objUtil.ErrCheck(cr.GetExtendedSpeedViolationsDetailsReportForFleet(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, Type, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                     //        {
                     //            //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                     //            ReportFailed();
                     //            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GeoZoneSummaryReport failed. User:" + sn.UserID.ToString()));
                     //            return;
                     //        }
                     //}
                     break;
                 #endregion
                 #region State Mileage Summary Report
                 case "36":
                     Summary = Convert.ToInt16(Util.PairFindValue(ReportTemplate.RptParamIncludeSummary, sn.Report.XmlParams));
                     if (objUtil.ErrCheck(cr.StateMileageSummaryReportPerFleet(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, Summary, sn.Report.ReportFormat, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.StateMileageSummaryReportPerFleet(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, Summary, sn.Report.ReportFormat, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GeoZoneSummaryReport failed. User:" + sn.UserID.ToString()));
                             return;
                         }

                     break;
                 #endregion
                 #region Fleet Utilization, Idling Details by Vehicle
                 case "37":
                     //Cost = Convert.ToSingle(Util.PairFindValue(ReportTemplate.RptParamCost,sn.Report.XmlParams));

                     tmp = sn.Report.XmlParams.Split(';');
                     Cost = Convert.ToSingle(tmp[0]);
                     ColorFilter = tmp[1] == "" ? "%" : tmp[1] + "%";


                     if (sn.Misc.DsReportSelectedFleets != null && sn.Misc.DsReportSelectedFleets.Tables.Count > 0 && sn.Misc.DsReportSelectedFleets.Tables[0].Rows.Count > 0)
                     {
                         int[] fleetIds = new int[sn.Misc.DsReportSelectedFleets.Tables[0].Rows.Count];
                         int i = 0;
                         foreach (DataRow dr in sn.Misc.DsReportSelectedFleets.Tables[0].Rows)
                         {
                             fleetIds[i] = Convert.ToInt32(dr["FleetId"]);
                             i++;
                         }


                         if (objUtil.ErrCheck(cr.GetCNFleetsUtilizationReport(sn.UserID, sn.SecId, fleetIds, sn.Report.FromDate, sn.Report.ToDate, Cost, ColorFilter, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                             if (objUtil.ErrCheck(cr.GetCNFleetsUtilizationReport(sn.UserID, sn.SecId, fleetIds, sn.Report.FromDate, sn.Report.ToDate, Cost, ColorFilter, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                             {
                                 //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                                 ReportFailed();
                                 System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GeoZoneSummaryReport failed. User:" + sn.UserID.ToString()));
                                 return;
                             }
                     }
                     //else
                     //{
                     //    if (objUtil.ErrCheck(cr.GetCNFleetUtilizationReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, Cost, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                     //        if (objUtil.ErrCheck(cr.GetCNFleetUtilizationReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, Cost, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                     //        {
                     //            //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                     //            ReportFailed();
                     //            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GeoZoneSummaryReport failed. User:" + sn.UserID.ToString()));
                     //            return;
                     //        }
                     //}
                     break;
                 #endregion
                 #region Activity Summary Report for Organization
                 case "38":

                     sensorNum = Convert.ToInt16(Util.PairFindValue(ReportTemplate.RptParamSensorId, sn.Report.XmlParams));

                     if (objUtil.ErrCheck(cr.GetActivitySummaryReportPerOrganization(sn.UserID, sn.SecId, sn.User.OrganizationId, sn.Report.FromDate, sn.Report.ToDate, sensorNum, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.GetActivitySummaryReportPerOrganization(sn.UserID, sn.SecId, sn.User.OrganizationId, sn.Report.FromDate, sn.Report.ToDate, sensorNum, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GetActivitySummaryReportPerOrganization failed. User:" + sn.UserID.ToString()));
                             return;
                         }

                     break;
                 #endregion
                 #region Activity Summary Report per Vehicle
                 case "39":
                     sensorNum = Convert.ToInt16(Util.PairFindValue(ReportTemplate.RptParamSensorId, sn.Report.XmlParams));

                     //if (!sn.Report.OrganizationHierarchySelected)
                     //{
                         if (objUtil.ErrCheck(cr.GetActivitySummaryReportPerVehicle(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sensorNum, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                             if (objUtil.ErrCheck(cr.GetActivitySummaryReportPerVehicle(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sensorNum, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                             {
                                 //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                                 ReportFailed();
                                 System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GetActivitySummaryReportPerVehicle failed. User:" + sn.UserID.ToString()));
                                 return;
                             }
                     //}
                     //else
                     //{
                     //    if (objUtil.ErrCheck(cr.GetActivitySummaryReportPerVehicle_Hierarchy(sn.UserID, sn.SecId, sn.Report.OrganizationHierarchyNodeCode, sn.Report.FromDate, sn.Report.ToDate, sensorNum, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                     //        if (objUtil.ErrCheck(cr.GetActivitySummaryReportPerVehicle_Hierarchy(sn.UserID, sn.SecId, sn.Report.OrganizationHierarchyNodeCode, sn.Report.FromDate, sn.Report.ToDate, sensorNum, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                     //        {
                     //            //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                     //            ReportFailed();
                     //            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GetActivitySummaryReportPerVehicle failed. User:" + sn.UserID.ToString()));
                     //            return;
                     //        }
                     //}

                     break;
                 #endregion
                 #region New trips Summary Report per Vehicle
                 case "40":

                     LicensePlate = Util.PairFindValue(ReportTemplate.RptParamLicensePlate, sn.Report.XmlParams);
                     sensorNum = Convert.ToInt16(Util.PairFindValue(ReportTemplate.RptParamSensorId, sn.Report.XmlParams));

                     if (objUtil.ErrCheck(cr.GetTripsSummaryReportByLicensePlate(sn.UserID, sn.SecId, LicensePlate, sn.Report.FromDate, sn.Report.ToDate, sensorNum, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.GetTripsSummaryReportByLicensePlate(sn.UserID, sn.SecId, LicensePlate, sn.Report.FromDate, sn.Report.ToDate, sensorNum, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GetActivitySummaryReportPerVehicle failed. User:" + sn.UserID.ToString()));
                             return;
                         }

                     break;
                 #endregion
                 #region Time At Landmark
                 case "41":
                     if (!sn.Report.IsFleet)
                     {
                         if (objUtil.ErrCheck(cr.GetTimeAtLandmarkReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                             if (objUtil.ErrCheck(cr.GetTimeAtLandmarkReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                             {
                                 //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                                 ReportFailed();
                                 System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GetActivitySummaryReportPerVehicle failed. User:" + sn.UserID.ToString()));
                                 return;
                             }
                     }
                     else
                     {
                         if (objUtil.ErrCheck(cr.GetTimeAtLandmarkFleetReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                             if (objUtil.ErrCheck(cr.GetTimeAtLandmarkFleetReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                             {
                                 //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                                 ReportFailed();
                                 System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GetActivitySummaryReportPerVehicle failed. User:" + sn.UserID.ToString()));
                                 return;
                             }
                     }
                     break;
                 #endregion
                 #region Monthly Utilization Report
                 case "42":

                     if (objUtil.ErrCheck(cr.GetExtendedMonthlyFleetSummary(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.GetExtendedMonthlyFleetSummary(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GetExtendedMonthlyFleetSummary failed. User:" + sn.UserID.ToString()));
                             return;
                         }

                     break;
                 #endregion
                 #region Average Days Utilized Per Vehicle
                 case "43":

                     if (objUtil.ErrCheck(cr.GetExtendedMonthlyAverageDaysUtilized(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.GetExtendedMonthlyAverageDaysUtilized(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GetExtendedMonthlyFleetSummary failed. User:" + sn.UserID.ToString()));
                             return;
                         }

                     break;
                 #endregion
                 #region Average Service Hrs for Utilized Vehicle
                 case "44":

                     if (objUtil.ErrCheck(cr.GetExtendedMonthlyServiceHrsUtilized(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.GetExtendedMonthlyServiceHrsUtilized(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GetExtendedMonthlyFleetSummary failed. User:" + sn.UserID.ToString()));
                             return;
                         }

                     break;
                 #endregion
                 #region Average Engine ON Hrs for Utilized Vehicle
                 case "45":

                     if (objUtil.ErrCheck(cr.GetExtendedMonthlyEngineOnHrsUtilized(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.GetExtendedMonthlyEngineOnHrsUtilized(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GetExtendedMonthlyFleetSummary failed. User:" + sn.UserID.ToString()));
                             return;
                         }

                     break;
                 #endregion
                 #region Unnecessary Idling Hrs. Per Vehicle Per Month
                 case "46":

                     if (objUtil.ErrCheck(cr.GetExtendedMonthlyUnnecessaryIdlingHrsUtilized(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.GetExtendedMonthlyUnnecessaryIdlingHrsUtilized(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GetExtendedMonthlyFleetSummary failed. User:" + sn.UserID.ToString()));
                             return;
                         }

                     break;
                 #endregion
                 #region Total Unnecessary Idling Fuel Costs
                 case "47":
                     Cost = Convert.ToSingle(Util.PairFindValue(ReportTemplate.RptParamCost, sn.Report.XmlParams));

                     if (objUtil.ErrCheck(cr.GetExtendedUnnecessaryIdlingFuelCosts(sn.UserID, sn.SecId, sn.Report.FromDate, Cost, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.GetExtendedUnnecessaryIdlingFuelCosts(sn.UserID, sn.SecId, sn.Report.FromDate, Cost, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GetExtendedMonthlyFleetSummary failed. User:" + sn.UserID.ToString()));
                             return;
                         }

                     break;
                 #endregion
                 #region Average Travelled Distance
                 case "48":


                     if (objUtil.ErrCheck(cr.GetExtendedAverageTravelledDistance(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.GetExtendedAverageTravelledDistance(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GetExtendedMonthlyFleetSummary failed. User:" + sn.UserID.ToString()));
                             return;
                         }

                     break;
                 #endregion
                 #region CN Vehicle Status
                 case "49":


                     if (objUtil.ErrCheck(cr.GetVehiclesStatusReport(sn.UserID, sn.SecId, sn.User.OrganizationId, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.GetVehiclesStatusReport(sn.UserID, sn.SecId, sn.User.OrganizationId, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GetExtendedMonthlyFleetSummary failed. User:" + sn.UserID.ToString()));
                             return;
                         }

                     break;
                 #endregion
                 #region Trip Summary Totals Report per Vehicle
                 case "50":
                     sensorNum = Convert.ToInt16(Util.PairFindValue(ReportTemplate.RptParamSensorId, sn.Report.XmlParams));

                     if (objUtil.ErrCheck(cr.GetTripSummaryTotalsReportperVehicle(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sensorNum, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.GetTripSummaryTotalsReportperVehicle(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sensorNum, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GetActivitySummaryReportPerVehicle failed. User:" + sn.UserID.ToString()));
                             return;
                         }

                     break;
                 #endregion
                 #region Trip Summary Totals Report per Organization
                 case "51":

                     sensorNum = Convert.ToInt16(Util.PairFindValue(ReportTemplate.RptParamSensorId, sn.Report.XmlParams));

                     if (objUtil.ErrCheck(cr.GetTripSummaryTotalsReportperOrganization(sn.UserID, sn.SecId, sn.User.OrganizationId, sn.Report.FromDate, sn.Report.ToDate, sensorNum, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.GetTripSummaryTotalsReportperOrganization(sn.UserID, sn.SecId, sn.User.OrganizationId, sn.Report.FromDate, sn.Report.ToDate, sensorNum, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GetActivitySummaryReportPerOrganization failed. User:" + sn.UserID.ToString()));
                             return;
                         }

                     break;
                 #endregion
                 #region Activity Summary Salt Spreader
                 case "52":


                     if (objUtil.ErrCheck(cr.GetActivitySummarySaltSpreader(sn.UserID, sn.SecId, sn.Report.VehicleId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.GetActivitySummarySaltSpreader(sn.UserID, sn.SecId, sn.Report.VehicleId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         {
                             //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GetActivitySummaryReportPerOrganization failed. User:" + sn.UserID.ToString()));
                             return;
                         }

                     break;
                 #endregion
                 #region HOS Details Report
                 case "53":

                     int DriverId = Convert.ToInt32(Util.PairFindValue(ReportTemplate.RptParamDriverId, sn.Report.XmlParams));
                     //Response.Redirect("..\\HOS\\HOS_Report_.aspx");

                     if (objUtil.ErrCheck(cr.GetHOSSummaryReport(sn.UserID, sn.SecId, DriverId, Convert.ToDateTime(sn.Report.FromDate), Convert.ToDateTime(sn.Report.ToDate), sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, sn.Report.GuiId, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.GetHOSSummaryReport(sn.UserID, sn.SecId, DriverId, Convert.ToDateTime(sn.Report.FromDate), Convert.ToDateTime(sn.Report.ToDate), sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, sn.Report.GuiId, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GetActivitySummaryReportPerOrganization failed. User:" + sn.UserID.ToString()));
                             return;
                         }

                     break;

                 #endregion
                 #region Vehicle Data Dump
                 case "54":


                     if (objUtil.ErrCheck(cr.VehicleInfoDataDump(sn.UserID, sn.SecId, sn.User.OrganizationId, sn.Report.ReportFormat, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.VehicleInfoDataDump(sn.UserID, sn.SecId, sn.User.OrganizationId, sn.Report.ReportFormat, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GetActivitySummaryReportPerOrganization failed. User:" + sn.UserID.ToString()));
                             return;
                         }

                     break;
                 #endregion
                 # region Fuel Transaction Report
                 case "55":
                     if (objUtil.ErrCheck(cr.FuelTransactionReport(sn.UserID, sn.SecId, sn.User.OrganizationId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.FuelTransactionReport(sn.UserID, sn.SecId, sn.User.OrganizationId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "DriverTripDetailsReport failed. User:" + sn.UserID.ToString()));
                             // Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             return;
                         }
                     break;
                 #endregion
                 #region Activity Summary Report per Vehicle (CP)
                 case "56":
                     sensorNum = Convert.ToInt16(Util.PairFindValue(ReportTemplate.RptParamSensorId, sn.Report.XmlParams));

                     if (objUtil.ErrCheck(cr.GetActivitySummaryReportPerVehicle_CP(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sensorNum, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.GetActivitySummaryReportPerVehicle_CP(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sensorNum, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GetActivitySummaryReportPerVehicle failed. User:" + sn.UserID.ToString()));
                             return;
                         }

                     break;
                 #endregion
                 # region Fleet Violation detail Report
                 case "57":
                     if (objUtil.ErrCheck(cr.ViolationReport_CP(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.ViolationReport_CP(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "ViolationReport failed. User:" + sn.UserID.ToString()));
                             return;
                         }
                     break;
                 # endregion
                 # region Fleet Membership Report
                 case "58":
                     if (objUtil.ErrCheck(cr.FleetMembershipReport(sn.UserID, sn.SecId, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.FleetMembershipReport(sn.UserID, sn.SecId, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Fleet Membership Report failed. User:" + sn.UserID.ToString()));
                             return;
                         }
                     break;
                 # endregion
                 # region Fleet Diagnostic Report
                 case "59":
                     if (objUtil.ErrCheck(cr.FleetDiagnosticReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.FleetDiagnosticReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Fleet Diagnostic Report failed. User:" + sn.UserID.ToString()));
                             return;
                         }
                     break;
                 # endregion
                 # region User Login Report
                 case "60":
                 case "112":
                     if (objUtil.ErrCheck(cr.UserLoginsReport(sn.UserID, sn.SecId, sn.User.OrganizationId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.UserLoginsReport(sn.UserID, sn.SecId, sn.User.OrganizationId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Fleet Diagnostic Report failed. User:" + sn.UserID.ToString()));
                             return;
                         }
                     break;
                 # endregion
                 # region State Milage Report Pivot
                 case "61":
                     if (objUtil.ErrCheck(cr.StateMileageReportPerFleet_StateBased(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.StateMileageReportPerFleet_StateBased(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "State Mileage Report Pivot - failed. User:" + sn.UserID.ToString()));
                             return;
                         }
                     break;
                 # endregion
                 #region Activity Summary Report per Vehicle - Daily
                 case "62":
                     sensorNum = Convert.ToInt16(Util.PairFindValue(ReportTemplate.RptParamSensorId, sn.Report.XmlParams));

                     if (objUtil.ErrCheck(cr.GetActivitySummaryReportPerVehicle_Daily(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sensorNum, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.GetActivitySummaryReportPerVehicle_Daily(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sensorNum, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GetActivitySummaryReportPerVehicle failed. User:" + sn.UserID.ToString()));
                             return;
                         }

                     break;
                 #endregion
                 #region TransportationMileageReport
                 case "63":
                     #region Get License Plate by Vehicle
                     //int vehicleId =Convert.ToInt32( sn.Report.XmlParams);
                     //ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();
                     //string xml="";  

                     //if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByVehicleId(sn.UserID,sn.SecId,vehicleId,ref xml)  , false))
                     //  if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByVehicleId(sn.UserID,sn.SecId,vehicleId,ref xml)  , true))
                     //   {
                     //       ReportFailed();
                     //       System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GetTripsSummaryReportByLicensePlate failed. User:" + sn.UserID.ToString()));
                     //       return;
                     //   }


                     //      if (xml == "")
                     //          {
                     //             return;
                     //          }
                     //      DataSet dsResult = new DataSet();

                     //      System.IO.StringReader strrXML = new System.IO.StringReader(xml);
                     //      dsResult.ReadXml(strrXML);

                     //      LicensePlate = dsResult.Tables[0].Rows[0]["LicensePlate"].ToString();   
                     #endregion

                     sensorNum = 3;

                     LicensePlate = Util.PairFindValue(ReportTemplate.RptParamLicensePlate, sn.Report.XmlParams);
                     // sensorNum = Convert.ToInt16(Util.PairFindValue(ReportTemplate.RptParamSensorId,sn.Report.XmlParams));



                     if (objUtil.ErrCheck(cr.GetTransportationMileageReport(sn.UserID, sn.SecId, LicensePlate, sn.Report.FromDate, sn.Report.ToDate, sensorNum, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.GetTransportationMileageReport(sn.UserID, sn.SecId, LicensePlate, sn.Report.FromDate, sn.Report.ToDate, sensorNum, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {

                             //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GetTripsSummaryReportByLicensePlate failed. User:" + sn.UserID.ToString()));
                             return;
                         }

                     break;
                 #endregion
                 #region Worksite Activity Report
                 case "64":
                     //#region Get License Plate by Vehicle
                     //int vehicleId = Convert.ToInt32(sn.Report.VehicleId);
                     //ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();
                     //string xml="";  

                     //if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByVehicleId(sn.UserID,sn.SecId,vehicleId,ref xml)  , false))
                     //  if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByVehicleId(sn.UserID,sn.SecId,vehicleId,ref xml)  , true))
                     //   {
                     //       ReportFailed();
                     //       System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Worksite Activity Report failed. User:" + sn.UserID.ToString()));
                     //       return;
                     //   }


                     //      if (xml == "")
                     //          {
                     //             return;
                     //          }
                     //      DataSet dsResult = new DataSet();

                     //      System.IO.StringReader strrXML = new System.IO.StringReader(xml);
                     //      dsResult.ReadXml(strrXML);

                     //      LicensePlate = dsResult.Tables[0].Rows[0]["LicensePlate"].ToString();   
                     //   #endregion


                     //LicensePlate = Util.PairFindValue(ReportTemplate.RptParamLicensePlate,sn.Report.XmlParams);


                     //if (objUtil.ErrCheck(cr.GetActivityAtLandmarkSummaryReportPerFleetOnTheFly(sn.UserID, sn.SecId, 1, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                     //    if (objUtil.ErrCheck(cr.GetActivityAtLandmarkSummaryReportPerFleetOnTheFly(sn.UserID, sn.SecId, 1, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                     //    {

                     if (objUtil.ErrCheck(cr.GetActivityAtLandmarkSummaryReportPerFleet_ActiveVehicles(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, Convert.ToInt16(sn.Report.XmlParams), sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.GetActivityAtLandmarkSummaryReportPerFleet_ActiveVehicles(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, Convert.ToInt16(sn.Report.XmlParams), sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {

                             //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GetActivityAtLandmarkSummaryReportPerFleet failed. User:" + sn.UserID.ToString()));
                             return;
                         }

                     break;
                 #endregion
                 # region Idling  Messages Details  report
                 case "65":
                     if (objUtil.ErrCheck(cr.IdlingMessagesDetailReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.IdlingMessagesDetailReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Idling Details report failed. User:" + sn.UserID.ToString()));
                             return;
                         }
                     break;
                 # endregion
                 # region Timesheet Validation Details
                 case "66":
                     tmpValue = Convert.ToInt16(sn.Report.XmlParams);
                     sn.Report.XmlParams = "Details";
                     //if (objUtil.ErrCheck(cr.GeozoneTimeSheetReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams  , sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                     //    if (objUtil.ErrCheck(cr.GeozoneTimeSheetReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true ))
                     if (objUtil.ErrCheck(cr.GeozoneTimeSheetReport_ActiveVehicles(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, tmpValue, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.GeozoneTimeSheetReport_ActiveVehicles(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, tmpValue, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GeoZone Time report failed. User:" + sn.UserID.ToString()));
                             return;
                         }
                     break;
                 #endregion
                 # region Timesheet Validation Summary
                 case "67":
                     tmpValue = Convert.ToInt16(sn.Report.XmlParams);
                     sn.Report.XmlParams = "Summary";
                     //if (objUtil.ErrCheck(cr.GeozoneTimeSheetReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                     //    if (objUtil.ErrCheck(cr.GeozoneTimeSheetReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                     if (objUtil.ErrCheck(cr.GeozoneTimeSheetReport_ActiveVehicles(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, tmpValue, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.GeozoneTimeSheetReport_ActiveVehicles(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, tmpValue, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GeoZone Time report failed. User:" + sn.UserID.ToString()));
                             return;
                         }
                     break;
                 #endregion
                 # region Vehicle WorkSite Activity Per Day
                 case "68":
                     if (objUtil.ErrCheck(cr.WorksiteActivityPerDay(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.WorksiteActivityPerDay(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GeoZone Time report failed. User:" + sn.UserID.ToString()));
                             return;
                         }
                     break;
                 #endregion
                 # region Vehicle Status by LastCommDate
                 case "69":
                     tmp = sn.Report.XmlParams.Split(';');
                     if (sn.User.OrganizationId == 570)
                     {
                         if (objUtil.ErrCheck(cr.GetVehiclesStatusByDateReport_ActiveVehicles(sn.UserID, sn.SecId, sn.User.OrganizationId, sn.Report.FleetId, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, Convert.ToInt16(tmp[0]), ref ReportPath), false))
                             if (objUtil.ErrCheck(cr.GetVehiclesStatusByDateReport_ActiveVehicles(sn.UserID, sn.SecId, sn.User.OrganizationId, sn.Report.FleetId, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, Convert.ToInt16(tmp[0]), ref ReportPath), true))
                             {
                                 //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                                 ReportFailed();
                                 System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Vehicle Status report failed. User:" + sn.UserID.ToString()));
                                 return;
                             }
                     }
                     else
                     {
                         if (objUtil.ErrCheck(cr.GetVehiclesStatusByDateReport(sn.UserID, sn.SecId, sn.User.OrganizationId, sn.Report.FleetId, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                             if (objUtil.ErrCheck(cr.GetVehiclesStatusByDateReport(sn.UserID, sn.SecId, sn.User.OrganizationId, sn.Report.FleetId, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                             {
                                 //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                                 ReportFailed();
                                 System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Vehicle Status report failed. User:" + sn.UserID.ToString()));
                                 return;
                             }
                     }
                     break;
                 #endregion
                 # region  SensorActivity
                 case "70":
                 case "90":
                 case "91":
                     //if (!sn.Report.OrganizationHierarchySelected)
                     //{
                         if (objUtil.ErrCheck(cr.GetSensorActivityReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                             if (objUtil.ErrCheck(cr.GetSensorActivityReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                             {
                                 //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                                 ReportFailed();
                                 System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Vehicle Status report failed. User:" + sn.UserID.ToString()));
                                 return;
                             }
                     //}
                     //else
                     //{
                     //    if (objUtil.ErrCheck(cr.GetSensorActivityReport_Hierarchy(sn.UserID, sn.SecId, sn.Report.OrganizationHierarchyNodeCode, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                     //        if (objUtil.ErrCheck(cr.GetSensorActivityReport_Hierarchy(sn.UserID, sn.SecId, sn.Report.OrganizationHierarchyNodeCode, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                     //        {
                     //            //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                     //            ReportFailed();
                     //            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Vehicle Status report failed. User:" + sn.UserID.ToString()));
                     //            return;
                     //        }
                     //}

                     break;
                 #endregion
                 #region Activity Summary Report per Vehicle Special
                 case "71":
                     sensorNum = 3;

                     //if (!sn.Report.OrganizationHierarchySelected)
                     //{
                         if (objUtil.ErrCheck(cr.GetActivitySummaryReportPerVehicle_Special(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sensorNum, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                             if (objUtil.ErrCheck(cr.GetActivitySummaryReportPerVehicle_Special(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sensorNum, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                             {
                                 //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                                 ReportFailed();
                                 System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GetActivitySummaryReportPerVehicle failed. User:" + sn.UserID.ToString()));
                                 return;
                             }
                     //}
                     //else
                     //{

                     //    if (objUtil.ErrCheck(cr.GetActivitySummaryReportPerVehicle_Special_Hierarchy(sn.UserID, sn.SecId, sn.Report.OrganizationHierarchyNodeCode, sn.Report.FromDate, sn.Report.ToDate, sensorNum, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                     //        if (objUtil.ErrCheck(cr.GetActivitySummaryReportPerVehicle_Special_Hierarchy(sn.UserID, sn.SecId, sn.Report.OrganizationHierarchyNodeCode, sn.Report.FromDate, sn.Report.ToDate, sensorNum, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                     //        {
                     //            //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                     //            ReportFailed();
                     //            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GetActivitySummaryReportPerVehicle failed. User:" + sn.UserID.ToString()));
                     //            return;
                     //        }
                     //}
                     break;
                 #endregion
                 # region Violation Summary report Special
                 case "72":
                     if (objUtil.ErrCheck(cr.ViolationReportWithScore_Special(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.ViolationReportWithScore_Special(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "ViolationSummary failed. User:" + sn.UserID.ToString()));
                             return;
                         }
                     break;
                 # endregion
                 # region Activity Outside Landmark
                 case "73":
                     tmp = sn.Report.XmlParams.Split(';');
                     //if (objUtil.ErrCheck(cr.ActivityOutsideLandmarkReport (sn.UserID, sn.SecId, sn.Report.FleetId,sn.Report.VehicleId,  sn.Report.FromDate, sn.Report.ToDate,  sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                     //    if (objUtil.ErrCheck(cr.ActivityOutsideLandmarkReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.VehicleId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                     if (objUtil.ErrCheck(cr.ActivityOutsideLandmarkReport_ActiveVehicles(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.VehicleId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, Convert.ToInt16(tmp[0]), ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.ActivityOutsideLandmarkReport_ActiveVehicles(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.VehicleId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, Convert.ToInt16(tmp[0]), ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "ViolationSummary failed. User:" + sn.UserID.ToString()));
                             return;
                         }
                     break;
                 # endregion
                 # region Worksite Details Report
                 case "74":
                     if (objUtil.ErrCheck(cr.ActivityInLandmarkReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.VehicleId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.ActivityInLandmarkReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.VehicleId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "ViolationSummary failed. User:" + sn.UserID.ToString()));
                             return;
                         }
                     break;
                 # endregion
                 #region ExtendedSpeedViolationDetailsReportReport
                 case "75":
                     // Type = Convert.ToInt32(Util.PairFindValue(ReportTemplate.RptParamIncludeSummary,sn.Report.XmlParams));

                     tmp = sn.Report.XmlParams.Split(';');
                     Type = Convert.ToInt32(tmp[0]);
                     ColorFilter = tmp[1] == "" ? "%" : tmp[1] + "%";



                     if (objUtil.ErrCheck(cr.GetExtendedSpeedSummaryViolationsReportForFleet(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, Type, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.GetExtendedSpeedSummaryViolationsReportForFleet(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, Type, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GeoZoneSummaryReport failed. User:" + sn.UserID.ToString()));
                             return;
                         }


                     //else
                     //{
                     //    if (objUtil.ErrCheck(cr.GetExtendedSpeedViolationsDetailsReportForFleet(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, Type, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                     //        if (objUtil.ErrCheck(cr.GetExtendedSpeedViolationsDetailsReportForFleet(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, Type, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                     //        {
                     //            //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                     //            ReportFailed();
                     //            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GeoZoneSummaryReport failed. User:" + sn.UserID.ToString()));
                     //            return;
                     //        }
                     //}
                     break;
                 #endregion
                 # region Fleet Membership Report By User
                 case "76":
                     if (objUtil.ErrCheck(cr.FleetMembershipReportUser(sn.UserID, sn.User.OrganizationId, sn.SecId, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.FleetMembershipReportUser(sn.UserID, sn.User.OrganizationId, sn.SecId, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Fleet Membership Report failed. User:" + sn.UserID.ToString()));
                             return;
                         }
                     break;
                 # endregion
                 # region Fleet Membership Report Special
                 case "77":
                     //if (objUtil.ErrCheck(cr.FleetMembershipReport_ActiveVehicles(sn.UserID, sn.SecId, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName,Convert.ToInt16(sn.Report.XmlParams),     ref ReportPath), false))
                     //    if (objUtil.ErrCheck(cr.FleetMembershipReport_ActiveVehicles(sn.UserID, sn.SecId, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, Convert.ToInt16(sn.Report.XmlParams), ref ReportPath), true))
                     if (objUtil.ErrCheck(cr.BrickmanFleetMembershipReport(sn.UserID, sn.SecId, Convert.ToInt32(sn.Report.XmlParams), sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.BrickmanFleetMembershipReport(sn.UserID, sn.SecId, Convert.ToInt32(sn.Report.XmlParams), sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Fleet Membership Report Special. User:" + sn.UserID.ToString()));
                             return;
                         }
                     break;
                 # endregion
                 #region Worksite Activity Report-Winter
                 case "79":
                     if (objUtil.ErrCheck(cr.GetActivityAtLandmarkSummaryReportPerFleetSnow_ActiveVehicles(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, Convert.ToInt16(sn.Report.XmlParams), ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.GetActivityAtLandmarkSummaryReportPerFleetSnow_ActiveVehicles(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, Convert.ToInt16(sn.Report.XmlParams), ref ReportPath), true))
                         {

                             //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GetActivityAtLandmarkSummaryReportPerFleet failed. User:" + sn.UserID.ToString()));
                             return;
                         }

                     break;
                 #endregion
                 #region ExtendedSpeedViolationDetailsReportReport_RoadSpeed
                 case "80":
                 case "84":
                     // Type = Convert.ToInt32(Util.PairFindValue(ReportTemplate.RptParamIncludeSummary,sn.Report.XmlParams));

                     tmp = sn.Report.XmlParams.Split(';');
                     Type = Convert.ToInt32(tmp[0]);
                     ColorFilter = tmp[1] == "" ? "%" : tmp[1] + "%";

                     if (sn.Misc.DsReportSelectedFleets != null && sn.Misc.DsReportSelectedFleets.Tables.Count > 0 && sn.Misc.DsReportSelectedFleets.Tables[0].Rows.Count > 0)
                     {
                         int[] fleetIds = new int[sn.Misc.DsReportSelectedFleets.Tables[0].Rows.Count];
                         int i = 0;
                         foreach (DataRow dr in sn.Misc.DsReportSelectedFleets.Tables[0].Rows)
                         {
                             fleetIds[i] = Convert.ToInt32(dr["FleetId"]);
                             i++;
                         }

                         //if (objUtil.ErrCheck(cr.GetExtendedSpeedViolationsDetailsReportForFleets(sn.UserID, sn.SecId, fleetIds, sn.Report.FromDate, sn.Report.ToDate, Type,ColorFilter, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         //    if (objUtil.ErrCheck(cr.GetExtendedSpeedViolationsDetailsReportForFleets(sn.UserID, sn.SecId, fleetIds, sn.Report.FromDate, sn.Report.ToDate, Type,ColorFilter, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         //    {

                         if (objUtil.ErrCheck(cr.GetExtendedSpeedViolationsDetailsReportForFleets_RoadSpeed(sn.UserID, sn.SecId, fleetIds, sn.Report.FromDate, sn.Report.ToDate, Type, ColorFilter, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                             if (objUtil.ErrCheck(cr.GetExtendedSpeedViolationsDetailsReportForFleets_RoadSpeed(sn.UserID, sn.SecId, fleetIds, sn.Report.FromDate, sn.Report.ToDate, Type, ColorFilter, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                             {
                                 //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                                 ReportFailed();
                                 System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GeoZoneSummaryReport failed. User:" + sn.UserID.ToString()));
                                 return;
                             }

                     }
                     //else
                     //{
                     //    if (objUtil.ErrCheck(cr.GetExtendedSpeedViolationsDetailsReportForFleet(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, Type, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                     //        if (objUtil.ErrCheck(cr.GetExtendedSpeedViolationsDetailsReportForFleet(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, Type, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                     //        {
                     //            //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                     //            ReportFailed();
                     //            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GeoZoneSummaryReport failed. User:" + sn.UserID.ToString()));
                     //            return;
                     //        }
                     //}
                     break;
                 #endregion
                 #region ExtendedSpeedViolationReportReport_RoadSpeed
                 case "81":
                 case "85":
                     tmp = sn.Report.XmlParams.Split(';');
                     Type = Convert.ToInt32(tmp[0]);
                     ColorFilter = tmp[1] == "" ? "%" : tmp[1] + "%";
                     //Type = Convert.ToInt32(Util.PairFindValue(ReportTemplate.RptParamIncludeSummary,sn.Report.XmlParams));

                     if (sn.Misc.DsReportSelectedFleets != null && sn.Misc.DsReportSelectedFleets.Tables.Count > 0 && sn.Misc.DsReportSelectedFleets.Tables[0].Rows.Count > 0)
                     {
                         int[] fleetIds = new int[sn.Misc.DsReportSelectedFleets.Tables[0].Rows.Count];
                         int i = 0;
                         foreach (DataRow dr in sn.Misc.DsReportSelectedFleets.Tables[0].Rows)
                         {
                             fleetIds[i] = Convert.ToInt32(dr["FleetId"]);
                             i++;
                         }

                         //if (objUtil.ErrCheck(cr.GetExtendedSpeedViolationsReportForFleets(sn.UserID, sn.SecId, fleetIds, sn.Report.FromDate, sn.Report.ToDate, Type, ColorFilter, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         //    if (objUtil.ErrCheck(cr.GetExtendedSpeedViolationsReportForFleets(sn.UserID, sn.SecId, fleetIds, sn.Report.FromDate, sn.Report.ToDate, Type, ColorFilter, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))

                         if (objUtil.ErrCheck(cr.GetExtendedSpeedViolationsReportForFleets_ByRoadSpeed(sn.UserID, sn.SecId, fleetIds, sn.Report.FromDate, sn.Report.ToDate, Type, ColorFilter, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                             if (objUtil.ErrCheck(cr.GetExtendedSpeedViolationsReportForFleets_ByRoadSpeed(sn.UserID, sn.SecId, fleetIds, sn.Report.FromDate, sn.Report.ToDate, Type, ColorFilter, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                             {
                                 //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                                 ReportFailed();
                                 System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GeoZoneSummaryReport failed. User:" + sn.UserID.ToString()));
                                 return;
                             }

                     }

                     break;
                 #endregion
                 # region Trip Details Report with Driver
                 case "82":
                 case "86":
                     if (!sn.Report.IsFleet)
                     {


                         if (objUtil.ErrCheck(cr.TripDetailsReportDriver(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref RequestOverflowed, ref OutMaxOverflowed, ref ReportPath), false))
                             if (objUtil.ErrCheck(cr.TripDetailsReportDriver(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref RequestOverflowed, ref OutMaxOverflowed, ref ReportPath), true))
                             {
                                 //  Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                                 ReportFailed();
                                 System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "TripDetailsReport  failed. User:" + sn.UserID.ToString()));
                                 return;
                             }
                     }
                     break;
                 # endregion
                 # region Fuel Summary Report
                 case "83":


                     if (objUtil.ErrCheck(cr.GetActivitySummaryReportPerVehicle_Fuel(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, 3, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.GetActivitySummaryReportPerVehicle_Fuel(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, 3, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             //  Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Fuel Summary Report failed. User:" + sn.UserID.ToString()));
                             return;
                         }

                     break;
                 # endregion
                 #region Idling Detail Report New
                 case "87":
                     sensorNum = Convert.ToInt16(Util.PairFindValue(ReportTemplate.RptParamSensorId, sn.Report.XmlParams));

                     if (objUtil.ErrCheck(cr.GetIdlingDetailsReportNew(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sensorNum, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.GetIdlingDetailsReportNew(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sensorNum, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GetActivitySummaryReportPerVehicle failed. User:" + sn.UserID.ToString()));
                             return;
                         }

                     break;
                 #endregion
                 #region Idling Summary Report New
                 case "88":

                     sensorNum = Convert.ToInt16(Util.PairFindValue(ReportTemplate.RptParamSensorId, sn.Report.XmlParams));


                     if (objUtil.ErrCheck(cr.GetIdlingSummaryReportPerOrganization(sn.UserID, sn.SecId, sn.User.OrganizationId, sn.Report.FromDate, sn.Report.ToDate, sensorNum, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.GetIdlingSummaryReportPerOrganization(sn.UserID, sn.SecId, sn.User.OrganizationId, sn.Report.FromDate, sn.Report.ToDate, sensorNum, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GetActivitySummaryReportPerVehicle failed. User:" + sn.UserID.ToString()));
                             return;
                         }

                     break;
                 #endregion
                 #region Trip Summary Report New
                 case "89":

                     tmp = sn.Report.XmlParams.Split(';');

                     if (objUtil.ErrCheck(cr.GetTripsSummaryReportNewStructure(sn.UserID, sn.SecId, sn.Report.LicensePlate, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, Convert.ToInt16(tmp[0]), Convert.ToBoolean(tmp[1]), sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.GetTripsSummaryReportNewStructure(sn.UserID, sn.SecId, sn.Report.LicensePlate, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, Convert.ToInt16(tmp[0]), Convert.ToBoolean(tmp[1]), sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GetActivitySummaryReportPerVehicle failed. User:" + sn.UserID.ToString()));
                             return;
                         }

                     break;
                 #endregion
                 #region Activity Summary and Green House Gas Report
                 case "92":
                     //sensorNum = Convert.ToInt16(Util.PairFindValue(ReportTemplate.RptParamSensorId, sn.Report.XmlParams));
                     sensorNum = 3;

                     if (objUtil.ErrCheck(cr.GetActivitySummaryReportPerVehicleGasType(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sensorNum, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.GetActivitySummaryReportPerVehicleGasType(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sensorNum, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GetActivitySummaryReportPerVehicleGasType failed. User:" + sn.UserID.ToString()));
                             return;
                         }

                     break;
                 #endregion
                 #region Green House Gas Emission Summary Report
                 case "93":
                     //sensorNum = Convert.ToInt16(Util.PairFindValue(ReportTemplate.RptParamSensorId, sn.Report.XmlParams));
                     sensorNum = 3;

                     if (objUtil.ErrCheck(cr.GetActivitySummaryReportPerVehicleGasTypeSummary(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sensorNum, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.GetActivitySummaryReportPerVehicleGasTypeSummary(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sensorNum, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GetActivitySummaryReportPerVehicleGasType failed. User:" + sn.UserID.ToString()));
                             return;
                         }

                     break;
                 #endregion
                 #region Maintenance Prev & Next
                 case "94":


                     if (objUtil.ErrCheck(cr.MaintenanceSpecialReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.MaintenanceSpecialReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Maintenance Special Report failed. User:" + sn.UserID.ToString()));
                             return;
                         }

                     break;
                 #endregion
                 # region  Off Road Highrail Mileage
                 case "95":
                     if (objUtil.ErrCheck(cr.GetHighRailMileageReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.GetHighRailMileageReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Vehicle Status report failed. User:" + sn.UserID.ToString()));
                             return;
                         }
                     break;
                 #endregion
                 #region Activity Summary Report per Vehicle Special
                 case "96":
                     sensorNum = 3;
                     //if (!sn.Report.OrganizationHierarchySelected)
                     //{
                         if (objUtil.ErrCheck(cr.GetActivitySummaryReportPerVehicle_BadGoodIdling(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sensorNum, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                             if (objUtil.ErrCheck(cr.GetActivitySummaryReportPerVehicle_BadGoodIdling(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sensorNum, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                             {
                                 //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                                 ReportFailed();
                                 System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GetActivitySummaryReportPerVehicle failed. User:" + sn.UserID.ToString()));
                                 return;
                             }

                     //}
                     //else
                     //{
                     //    if (objUtil.ErrCheck(cr.GetActivitySummaryReportPerVehicle_BadGoodIdling_Hierarchy(sn.UserID, sn.SecId, sn.Report.OrganizationHierarchyNodeCode, sn.Report.FromDate, sn.Report.ToDate, sensorNum, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                     //        if (objUtil.ErrCheck(cr.GetActivitySummaryReportPerVehicle_BadGoodIdling_Hierarchy(sn.UserID, sn.SecId, sn.Report.OrganizationHierarchyNodeCode, sn.Report.FromDate, sn.Report.ToDate, sensorNum, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                     //        {
                     //            //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                     //            ReportFailed();
                     //            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GetActivitySummaryReportPerVehicle failed. User:" + sn.UserID.ToString()));
                     //            return;
                     //        }
                     //}

                     break;
                 #endregion
                 #region Speed Violation Summary Report by Road Speed
                 case "97":
                     PostedSpeedOnly = Convert.ToInt16(sn.Report.XmlParams);

                     //if (!sn.Report.OrganizationHierarchySelected)
                     //{
                         if (objUtil.ErrCheck(cr.GetSpeedViolationSummaryReport_RoadSpeed(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, PostedSpeedOnly, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                             if (objUtil.ErrCheck(cr.GetSpeedViolationSummaryReport_RoadSpeed(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, PostedSpeedOnly, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                             {
                                 //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                                 ReportFailed();
                                 System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Speed Violation Summary Report by RoadSpeed failed. User:" + sn.UserID.ToString()));
                                 return;
                             }
                     //}
                     //else
                     //{
                     //    if (objUtil.ErrCheck(cr.GetSpeedViolationSummaryReport_RoadSpeed_Hierarchy(sn.UserID, sn.SecId, sn.Report.OrganizationHierarchyNodeCode, sn.Report.FromDate, sn.Report.ToDate, PostedSpeedOnly, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                     //        if (objUtil.ErrCheck(cr.GetSpeedViolationSummaryReport_RoadSpeed_Hierarchy(sn.UserID, sn.SecId, sn.Report.OrganizationHierarchyNodeCode, sn.Report.FromDate, sn.Report.ToDate, PostedSpeedOnly, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                     //        {
                     //            //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                     //            ReportFailed();
                     //            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Speed Violation Summary Report by RoadSpeed failed. User:" + sn.UserID.ToString()));
                     //            return;
                     //        }
                     //}

                     break;
                 #endregion
                 #region Electronic Invoice-Excel Dump
                 case "98":

                     if (objUtil.ErrCheck(cr.CNElectronicInvoice(sn.UserID, sn.SecId, sn.Report.ReportFormat, Convert.ToDateTime(sn.Report.FromDate), Convert.ToDateTime(sn.Report.ToDate), ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.CNElectronicInvoice(sn.UserID, sn.SecId, sn.Report.ReportFormat, Convert.ToDateTime(sn.Report.FromDate), Convert.ToDateTime(sn.Report.ToDate), ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GetSpeedViolationSummaryReport_RoadSpeed failed. User:" + sn.UserID.ToString()));
                             return;
                         }

                     break;
                 #endregion
                 #region Maintenance Services
                 case "99":

                     if (objUtil.ErrCheck(cr.MaintenanceVehicleReport(sn.UserID, sn.SecId, sn.Report.ReportFormat, sn.Report.FleetId, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.MaintenanceVehicleReport(sn.UserID, sn.SecId, sn.Report.ReportFormat, sn.Report.FleetId, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "MaintenanceVehicleReport failed. User:" + sn.UserID.ToString()));
                             return;
                         }

                     break;
                 #endregion
                 #region Mower Hours Report
                 case "100":

                     if (objUtil.ErrCheck(cr.GetMowerHoursReport_AciveVehicles(sn.UserID, sn.SecId, 4187, sn.Report.FromDate, sn.Report.ToDate, Convert.ToInt16(sn.Report.XmlParams), sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.GetMowerHoursReport_AciveVehicles(sn.UserID, sn.SecId, 4187, sn.Report.FromDate, sn.Report.ToDate, Convert.ToInt16(sn.Report.XmlParams), sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " Mower Hours Report. User:" + sn.UserID.ToString()));
                             return;
                         }

                     break;
                 #endregion
                 #region Vehicle Start End Odometer/ Eng Hrs Report
                 case "101":

                     if (objUtil.ErrCheck(cr.VehicleStartEndOdometerEngHrsReport(sn.UserID, sn.SecId, sn.Report.ReportFormat, sn.Report.FleetId, Convert.ToDateTime(sn.Report.FromDate), Convert.ToDateTime(sn.Report.ToDate), ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.VehicleStartEndOdometerEngHrsReport(sn.UserID, sn.SecId, sn.Report.ReportFormat, sn.Report.FleetId, Convert.ToDateTime(sn.Report.FromDate), Convert.ToDateTime(sn.Report.ToDate), ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Vehicle Start End Odometer/ Eng Hrs Report:" + sn.UserID.ToString()));
                             return;
                         }

                     break;
                 #endregion
                 #region Trucks Hours Report
                 case "102":

                     if (objUtil.ErrCheck(cr.GetTrucksHoursReport_AciveVehicles(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, Convert.ToInt16(sn.Report.XmlParams), sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.GetTrucksHoursReport_AciveVehicles(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, Convert.ToInt16(sn.Report.XmlParams), sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " Mower Hours Report. User:" + sn.UserID.ToString()));
                             return;
                         }

                     break;
                 #endregion
                 #region Audit Report
                 case "103":
                     //Devin changed 2014.01.27
                     System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");
                     DateTime from = Convert.ToDateTime(sn.Report.FromDate, ci);
                     DateTime to = Convert.ToDateTime(sn.Report.ToDate, ci);
                     //cr.GetHOSAuditReport(sn.UserID, sn.SecId, sn.User.OrganizationId, from, to, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath);
                     cr.GetHOSAuditReportNew(sn.UserID, sn.SecId, sn.User.OrganizationId, from, to,
                         sn.Report.FleetId, sn.Report.XmlParams,
                         sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath);
                     break;
                 #endregion
                 #region Speed Violation Details Report by Road Speed
                 case "104":
                     tmp = sn.Report.XmlParams.Split(';');
                     PostedSpeedOnly = Convert.ToInt16(tmp[0]);
                     Int32 Delta = Convert.ToInt32(tmp[1]);

                     if (objUtil.ErrCheck(cr.GetSpeedViolationsDetailsReport_RoadSpeed(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, PostedSpeedOnly, Delta, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.GetSpeedViolationsDetailsReport_RoadSpeed(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, PostedSpeedOnly, Delta, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " Mower Hours Report. User:" + sn.UserID.ToString()));
                             return;
                         }
                     break;
                 #endregion
                 #region Garmin Messages Report
                 case "105":

                     if (objUtil.ErrCheck(cr.GarminMessagesReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.GarminMessagesReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " Garmin Messages Report. User:" + sn.UserID.ToString()));
                             return;
                         }
                     break;
                 #endregion
                 #region Activity Summary Report with Cost
                 case "106":

                     if (objUtil.ErrCheck(cr.GetActivitySummaryReportPerVehicleWithCost(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, 3, Convert.ToSingle(sn.Report.XmlParams), sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.GetActivitySummaryReportPerVehicleWithCost(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, 3, Convert.ToSingle(sn.Report.XmlParams), sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " Activity Summary Report with Cost. User:" + sn.UserID.ToString()));
                             return;
                         }
                     break;
                 #endregion
                 #region Idling Details Driver Report (MTS)
                 case "107":
                     tmp = sn.Report.XmlParams.Split(';');
                     driverId = Convert.ToInt32(tmp[0]);
                     IdlingThreshold = Convert.ToInt16(tmp[1]);

                     if (objUtil.ErrCheck(cr.GetIdlingDriverReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, driverId, IdlingThreshold, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.GetIdlingDriverReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, driverId, IdlingThreshold, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Idling Details Driver Report. User:" + sn.UserID.ToString()));
                             return;
                         }
                     break;
                 #endregion
                 #region Get User Settings Report
                 case "108":
                     if (objUtil.ErrCheck(cr.GetUserSettingsReport(sn.UserID, sn.SecId, sn.User.OrganizationId, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.GetUserSettingsReport(sn.UserID, sn.SecId, sn.User.OrganizationId, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Idling Details Driver Report. User:" + sn.UserID.ToString()));
                             return;
                         }
                     break;
                 #endregion
                 #region Get Box Settings Report
                 case "109":
                     if (objUtil.ErrCheck(cr.GetBoxSettingsReport(sn.UserID, sn.SecId, sn.User.OrganizationId, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.GetBoxSettingsReport(sn.UserID, sn.SecId, sn.User.OrganizationId, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Idling Details Driver Report. User:" + sn.UserID.ToString()));
                             return;
                         }
                     break;
                 #endregion
                 #region Driver Violation Report
                 case "110":

                     tmp = sn.Report.XmlParams.Split(';');
                     driverId = Convert.ToInt32(tmp[0]);
                     Int16 speed = Convert.ToInt16(tmp[1]);

                     if (objUtil.ErrCheck(cr.DriverViolationReportNew(sn.UserID, sn.SecId, driverId, sn.Report.FromDate, sn.Report.ToDate, speed, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.DriverViolationReportNew(sn.UserID, sn.SecId, driverId, sn.Report.FromDate, sn.Report.ToDate, speed, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Idling Details Driver Report. User:" + sn.UserID.ToString()));
                             return;
                         }
                     break;
                 #endregion
                 #region Annual Mileage Report
                 case "111":

                     if (objUtil.ErrCheck(cr.YearOdomEngHoursReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.YearOdomEngHoursReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " Annual Mileage Report. User:" + sn.UserID.ToString()));
                             return;
                         }

                     break;
                 #endregion
                 #region Idling Details by Idling
                 case "113":
                     IdlingThreshold = Convert.ToInt16(sn.Report.XmlParams);

                     if (objUtil.ErrCheck(cr.evtFactEventsByIdling_Report(sn.UserID, sn.SecId, sn.Report.FleetId, Convert.ToDateTime(sn.Report.FromDate), Convert.ToDateTime(sn.Report.ToDate), IdlingThreshold, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.evtFactEventsByIdling_Report(sn.UserID, sn.SecId, sn.Report.FleetId, Convert.ToDateTime(sn.Report.FromDate), Convert.ToDateTime(sn.Report.ToDate), IdlingThreshold, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Idling Details Driver Report. User:" + sn.UserID.ToString()));
                             return;
                         }
                     break;
                 #endregion
                 #region Fuel Consumption Report by Driver
                 case "114":

                     if (objUtil.ErrCheck(cr.GetTripsSummaryReportNewStructure_ByDriver(sn.UserID, sn.SecId, "0", sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, 3, Convert.ToInt32(sn.Report.XmlParams), sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.GetTripsSummaryReportNewStructure_ByDriver(sn.UserID, sn.SecId, "0", sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, 3, Convert.ToInt32(sn.Report.XmlParams), sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GetActivitySummaryReportPerVehicle failed. User:" + sn.UserID.ToString()));
                             return;
                         }
                     break;
                 #endregion
                 #region Off Road Report
                 case "115":
                 case "116":

                     //if (!sn.Report.OrganizationHierarchySelected)
                     //{
                         if (objUtil.ErrCheck(cr.OnOffRoadMiles_Report(sn.UserID, sn.SecId, sn.Report.FleetId, Convert.ToDateTime(sn.Report.FromDate), Convert.ToDateTime(sn.Report.ToDate), sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                             if (objUtil.ErrCheck(cr.OnOffRoadMiles_Report(sn.UserID, sn.SecId, sn.Report.FleetId, Convert.ToDateTime(sn.Report.FromDate), Convert.ToDateTime(sn.Report.ToDate), sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                             {
                                 //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                                 ReportFailed();
                                 System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "OnOffRoadMiles_Report failed. User:" + sn.UserID.ToString()));
                                 return;
                             }
                     //}
                     //else
                     //{
                     //    if (objUtil.ErrCheck(cr.OnOffRoadMiles_Report_Hierarchy(sn.UserID, sn.SecId, sn.Report.OrganizationHierarchyNodeCode, Convert.ToDateTime(sn.Report.FromDate), Convert.ToDateTime(sn.Report.ToDate), sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                     //        if (objUtil.ErrCheck(cr.OnOffRoadMiles_Report_Hierarchy(sn.UserID, sn.SecId, sn.Report.OrganizationHierarchyNodeCode, Convert.ToDateTime(sn.Report.FromDate), Convert.ToDateTime(sn.Report.ToDate), sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                     //        {
                     //            //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                     //            ReportFailed();
                     //            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "OnOffRoadMiles_Report failed. User:" + sn.UserID.ToString()));
                     //            return;
                     //        }
                     //}
                     break;
                 #endregion
                 #region Fuel consumption while speeding report
                 case "117":

                     if (objUtil.ErrCheck(cr.evtViolationFuel_Report(sn.UserID, sn.SecId, sn.Report.FleetId, Convert.ToDateTime(sn.Report.FromDate), Convert.ToDateTime(sn.Report.ToDate), sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.evtViolationFuel_Report(sn.UserID, sn.SecId, sn.Report.FleetId, Convert.ToDateTime(sn.Report.FromDate), Convert.ToDateTime(sn.Report.ToDate), sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Fuel consumption report failed. User:" + sn.UserID.ToString()));
                             return;
                         }
                     break;
                 #endregion


                 #region Fuel Fraud Report
                 case "118":

                     if (objUtil.ErrCheck(cr.FuelFraudTransactionReport(sn.UserID, sn.SecId, sn.User.OrganizationId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.FuelFraudTransactionReport(sn.UserID, sn.SecId, sn.User.OrganizationId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Fuel Fraud Transaction Report failed. User:" + sn.UserID.ToString()));
                             // Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             return;
                         }
                     break;
                 #endregion


                 #region Speed Distribution Report
                 case "119":
                     //if (!sn.Report.OrganizationHierarchySelected)
                     //{
                         if (objUtil.ErrCheck(cr.SpeedDistributionReport(sn.UserID, sn.SecId, sn.Report.FleetId, Convert.ToDateTime(sn.Report.FromDate), Convert.ToDateTime(sn.Report.ToDate), sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                             if (objUtil.ErrCheck(cr.SpeedDistributionReport(sn.UserID, sn.SecId, sn.Report.FleetId, Convert.ToDateTime(sn.Report.FromDate), Convert.ToDateTime(sn.Report.ToDate), sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                             {
                                 ReportFailed();
                                 System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " Speed Distribution Report failed. User:" + sn.UserID.ToString()));
                                 // Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                                 return;
                             }
                     //}
                     //else
                     //{
                     //    if (objUtil.ErrCheck(cr.SpeedDistributionReport_Hierarchy(sn.UserID, sn.SecId, sn.Report.OrganizationHierarchyNodeCode, Convert.ToDateTime(sn.Report.FromDate), Convert.ToDateTime(sn.Report.ToDate), sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                     //        if (objUtil.ErrCheck(cr.SpeedDistributionReport_Hierarchy(sn.UserID, sn.SecId, sn.Report.OrganizationHierarchyNodeCode, Convert.ToDateTime(sn.Report.FromDate), Convert.ToDateTime(sn.Report.ToDate), sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                     //        {
                     //            ReportFailed();
                     //            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " Speed Distribution Report failed. User:" + sn.UserID.ToString()));
                     //            // Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                     //            return;
                     //        }
                     //}
                     break;
                 #endregion



                 #region Company property speed violation details report
                 case "120":
                     //if (!sn.Report.OrganizationHierarchySelected)
                     //{
                         if (objUtil.ErrCheck(cr.evtViolationSpeedInLandmark_Report(sn.UserID, sn.SecId, sn.Report.FleetId, Convert.ToDateTime(sn.Report.FromDate), Convert.ToDateTime(sn.Report.ToDate), sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                             if (objUtil.ErrCheck(cr.evtViolationSpeedInLandmark_Report(sn.UserID, sn.SecId, sn.Report.FleetId, Convert.ToDateTime(sn.Report.FromDate), Convert.ToDateTime(sn.Report.ToDate), sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                             {
                                 ReportFailed();
                                 System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " Company property speed violation details report failed. User:" + sn.UserID.ToString()));
                                 // Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                                 return;
                             }
                     //}
                     //else
                     //{
                     //    if (objUtil.ErrCheck(cr.evtViolationSpeedInLandmarkHierarchy_Report(sn.UserID, sn.SecId, sn.Report.OrganizationHierarchyNodeCode, Convert.ToDateTime(sn.Report.FromDate), Convert.ToDateTime(sn.Report.ToDate), sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                     //        if (objUtil.ErrCheck(cr.evtViolationSpeedInLandmarkHierarchy_Report(sn.UserID, sn.SecId, sn.Report.OrganizationHierarchyNodeCode, Convert.ToDateTime(sn.Report.FromDate), Convert.ToDateTime(sn.Report.ToDate), sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                     //        {
                     //            ReportFailed();
                     //            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " Company property speed violation details report failed. User:" + sn.UserID.ToString()));
                     //            // Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                     //            return;
                     //        }
                     //}
                     break;
                 #endregion



                 #region Box Geozones
                 case "121":
                     if (objUtil.ErrCheck(cr.ReportBoxGeozone(sn.UserID, sn.SecId, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.ReportBoxGeozone(sn.UserID, sn.SecId, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " Box Geozones report failed. User:" + sn.UserID.ToString()));
                             // Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             return;
                         }
                     break;
                 #endregion


                 #region Driver Scores by Fleet
                 case "123":
                     if (objUtil.ErrCheck(cr.evtDriverViolationsFleet_Report(sn.UserID, sn.SecId, sn.Report.FleetId, Convert.ToDateTime(sn.Report.FromDate), Convert.ToDateTime(sn.Report.ToDate), sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.evtDriverViolationsFleet_Report(sn.UserID, sn.SecId, sn.Report.FleetId, Convert.ToDateTime(sn.Report.FromDate), Convert.ToDateTime(sn.Report.ToDate), sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                         {
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " Box Geozones report failed. User:" + sn.UserID.ToString()));
                             // Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             return;
                         }
                     break;
                 #endregion



                 #region Worksite Activity Snow Report by Media Type
                 case "124":
                     //tmp = sn.Report.XmlParams.Split(';');
                     //Int16 ActiveVehicles = Convert.ToInt16(tmp[0]);
                     //Int16  MediaTypeId  = Convert.ToInt16(tmp[1]);

                  
                     Int16 ActiveVehicles = 1;
                     Int16  MediaTypeId  = 93;

                     if (objUtil.ErrCheck(cr.GetActivityAtLandmarkSummaryReportPerFleetSnow_MediaType_ActiveVehicles(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ActiveVehicles,MediaTypeId, ref ReportPath), false))
                         if (objUtil.ErrCheck(cr.GetActivityAtLandmarkSummaryReportPerFleetSnow_MediaType_ActiveVehicles(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ActiveVehicles, MediaTypeId, ref ReportPath), true ))
                         {

                             //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                             ReportFailed();
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GetActivityAtLandmarkSummaryReportPerFleet failed. User:" + sn.UserID.ToString()));
                             return;
                         }

                     break;
                 #endregion

                 #region Overtime duration in Landmark
                 //case "125":
                 //    if (objUtil.ErrCheck(cr.evtOverTimeDurationInLandmark_Report(sn.UserID, sn.SecId, sn.Report.FleetId, Convert.ToDateTime(sn.Report.FromDate), Convert.ToDateTime(sn.Report.ToDate), sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                 //        if (objUtil.ErrCheck(cr.evtOverTimeDurationInLandmark_Report(sn.UserID, sn.SecId, sn.Report.FleetId, Convert.ToDateTime(sn.Report.FromDate), Convert.ToDateTime(sn.Report.ToDate), sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                 //        {
                 //            ReportFailed();
                 //            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " Overtime Duration In Landmark Report Failed. User:" + sn.UserID.ToString()));
                 //            return;
                 //        }
                 //    break;
                 #endregion


		
                 #region Salt Spreader
                 case "126":
                     strUrl = "SSRSReports/frmSaltSpreaderSummaryReport.aspx";
                     CreateResponsePage(strUrl);
                     break;
                 #endregion


               //Devin Aecon Report
              //case "300":

              //    using (AeconReport.AeconXASClient ac = new AeconReport.AeconXASClient())
              //    {
              //        ci = new System.Globalization.CultureInfo("en-US");
              //        DateTime dtStart = Convert.ToDateTime(sn.Report.FromDate, ci);
              //        DateTime dtEnd = Convert.ToDateTime(sn.Report.ToDate, ci);
              //        ReportPath = ac.ProcessReport(int.Parse(sn.Report.XmlParams), dtStart, dtEnd, sn.UserID);
              //    }
              //    break;

                 #region Server Report Handle Section
                 default:
                                
                     int pi = this.StringToInt32(ReportType);

                     if (pi >= 10000)
                     {
                         ReportingServices.ReportingServices sr = new ReportingServices.ReportingServices();
                         
                         sr.Timeout = 1800000;

                         string message = "";
                         string parameters = sn.Report.XmlParams.ToString();

                         ReportPath = "";

                         try { 
                             if (!sr.RenderDirectReport(parameters, ref ReportPath))                // 1st
                             {
                                 if (!sr.RenderDirectReport(parameters, ref ReportPath))            // 2nd in case db server is busy.
                                 {
                                     message = sr.Message().ToString();
                                     ReportFailed();
                                     System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " Speed distribution report (Server) failed. User:" + sn.UserID.ToString()));
                                 }
                             }
                         }
                         catch(Exception ex){
                             message = ex.Message.ToString();
                             if (message.IndexOf("Time out") > 0)                                   // 3rd Web Service is busy
                             {
                                 if (!sr.RenderDirectReport(parameters, ref ReportPath))
                                 {
                                     message = sr.Message().ToString();
                                     ReportFailed();
                                     System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " Speed distribution report (Server) failed. User:" + sn.UserID.ToString()));
                                 }
                             }
                         }
                     }
                   
                  break;
                 #endregion

             }
           
            if (RequestOverflowed || OutMaxOverflowed)
            {
               //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>"+Resources.Const.Reports_ToManyRecords+"<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
               sn.MessageText = Resources.Const.Reports_ToManyRecords;
               strUrl = "frmReportError.aspx";
               //strUrl = "<html><frameset id=\"TopFrame\" rows=\"*,24px\" frameSpacing=\"0\" border=\"0\" bordercolor=\"gray\" frameBorder=\"0\">" +
               //"<frame name=\"report\" src='frmReportError.aspx' scrolling=auto  frameborder=\"0\" noresize>" +
               //"<frame name=\"reportback\" src=\"frmReportBack.aspx\" scrolling=\"no\" frameborder=\"1\" noresize></frameset>";
            }
            else
               if (!String.IsNullOrEmpty(ReportPath))
               {
                 // if (Request.IsSecureConnection)
                   //by devin
                   if (Request.Url.ToString().ToLower().Contains("https://"))
                     ReportPath = ReportPath.Replace("http", "https");
                   
                  strUrl = ReportPath;
                  //strUrl = "<html><frameset id=\"TopFrame\" rows=\"*,24px\" frameSpacing=\"0\" border=\"0\" bordercolor=\"gray\" frameBorder=\"0\">" +
                  //"<frame name=\"report\" src='" + ReportPath + "' scrolling=auto  frameborder=\"0\" noresize>" +
                  //"<frame name=\"reportback\" src=\"frmReportBack.aspx\" scrolling=\"no\" frameborder=\"1\" noresize></frameset>";
               }
               else
               {
                  //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>"+ Resources.Const.Reports_NoData +"<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                  sn.MessageText = Resources.Const.Reports_NoData;
                  strUrl = "frmReportError.aspx";
                  //strUrl = "<html><frameset id=\"TopFrame\" rows=\"*,24px\" frameSpacing=\"0\" border=\"0\" bordercolor=\"gray\" frameBorder=\"0\">" +
                  //"<frame name=\"report\" src='frmReportError.aspx' scrolling=auto  frameborder=\"0\" noresize>" +
                  //"<frame name=\"reportback\" src=\"frmReportBack.aspx\" scrolling=\"no\" frameborder=\"1\" noresize></frameset>";
               }

            CreateResponsePage(strUrl);
         }
         catch (Exception ex)
         {
             ReportFailed();
            //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
            //sn.MessageText = Resources.Const.Reports_LoadFailed;
            //CreateResponsePage("frmReportError.aspx");
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "ReportViewer failed. Ex:" + ex.Message));
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
            ExceptionLogger(trace);
            return;
         }
      }

      /// <summary>
      /// Creates frameset page with url and back button
      /// </summary>
      /// <param name="url"></param>
      private void CreateResponsePage(string url)
      {
         StringBuilder pageBuilder = 
            new StringBuilder("<html><frameset id=\"TopFrame\" rows=\"*,24px\" frameSpacing=\"0\" border=\"0\" bordercolor=\"gray\" frameBorder=\"0\">");
         pageBuilder.AppendFormat("<frame name=\"report\" src='{0}' scrolling=auto  frameborder=\"0\" noresize />", url);
         pageBuilder.AppendLine("<frame name=\"reportback\" src=\"frmReportBack.aspx\" scrolling=\"no\" frameborder=\"1\" noresize /></frameset></html>");
         
         Response.Write(pageBuilder.ToString());
         //return pageBuilder.ToString();
      }

        private void ReportFailed()
        {
            sn.MessageText = Resources.Const.Reports_LoadFailed;
            CreateResponsePage("frmReportError.aspx");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Int32 StringToInt32(string value)
        {
            Int32 i = 0;
            if (Int32.TryParse(value, out i))
                return i;
            else
                return 0;
        }
    }
}
