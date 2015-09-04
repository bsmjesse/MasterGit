using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.IO;
using System.Web.Services;
using VLF.CLS.Interfaces ;
using VLF.ERRSecurity ;
using VLF.CLS;
using VLF.CLS.Def ;
using VLF.Reports;
using VLF.DAS.Logic;


namespace VLF.ASI.Interfaces
{

   [WebService(Namespace = "http://www.sentinelfm.com")]

   /// <summary>
   /// Summary description for Reports.
   /// </summary>
   public class Reports : System.Web.Services.WebService
   {
      public Reports()
      {
         //CODEGEN: This call is required by the ASP.NET Web Services Designer
         InitializeComponent();
      }

      #region Component Designer generated code

      //Required by the Web Services Designer 
      private IContainer components = null;

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
      }

      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      protected override void Dispose(bool disposing)
      {
         if (disposing && components != null)
         {
            components.Dispose();
         }
         base.Dispose(disposing);
      }

      #endregion

      [WebMethod]
      public int GetXml(int userId, string SID, ReportTemplate.ReportTypes repType, string xmlParams, ref string xml,
                     ref bool requestOverflowed, ref int totalSqlRecords,
                     ref bool outMaxOverflowed, ref int outMaxRecords)
      {
         try
         {
            Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceVerbose, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces, string.Format("GetXml( userId = {0}, repType = {1}, xmlParams = {2} )", userId, repType, xmlParams)));

            Trace.WriteLineIf(AppConfig.tsStat.Enabled | AppConfig.tsMain.TraceInfo, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Info, "Report: " + repType.ToString() + " started."));

            // Authenticate 
            LoginManager.GetInstance().SecurityCheck(userId, SID);

            // Authorize
            LoginManager.GetInstance().AuthorizeOperation(userId, VLF.CLS.Def.Enums.OperationType.Repors, Convert.ToInt32(repType));

            xml = "";

            using (ReportGenerator dbReport = new ReportGenerator(LoginManager.GetConnnectionString(userId)))
            {
               xml = dbReport.GetXml(repType, xmlParams, userId, "en", ref requestOverflowed, ref totalSqlRecords, ref outMaxOverflowed, ref outMaxRecords);

            }

            return (int)InterfaceError.NoError;
         }
         catch (Exception Ex)
         {
            return (int)ASIErrorCheck.CheckError(Ex);
         }
         finally
         {
            Trace.WriteLineIf(AppConfig.tsStat.Enabled | AppConfig.tsMain.TraceInfo, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Info, "Report: " + repType.ToString() + " finished."));
         }
      }

      [WebMethod]
      public int GetXmlByLang(int userId, string SID, ReportTemplate.ReportTypes repType, string xmlParams,
                              string lang, ref string xml, ref bool requestOverflowed, ref int totalSqlRecords,
                              ref bool outMaxOverflowed, ref int outMaxRecords)
      {
         try
         {
            Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceVerbose, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces, string.Format("GetXml( userId = {0}, repType = {1}, xmlParams = {2} )", userId, repType, xmlParams)));

            Trace.WriteLineIf(AppConfig.tsStat.Enabled | AppConfig.tsMain.TraceInfo, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Info, "Report: " + repType.ToString() + " started."));

            // Authenticate 
            LoginManager.GetInstance().SecurityCheck(userId, SID);

            // Authorize
            LoginManager.GetInstance().AuthorizeOperation(userId, VLF.CLS.Def.Enums.OperationType.Repors, Convert.ToInt32(repType));

            xml = "";

            using (ReportGenerator dbReport = new ReportGenerator(LoginManager.GetConnnectionString(userId)))
            {
               xml = dbReport.GetXml(repType, xmlParams, userId, lang, ref requestOverflowed, ref totalSqlRecords, ref outMaxOverflowed, ref outMaxRecords);
               xml = LocalizationLayer.GUILocalizationLayer.LocalizeReportData(xml, lang);

               DataSet ds = new DataSet();
               StringReader strrXML = new StringReader(xml);
               ds.ReadXml(strrXML);

//               if (lang == "fr" && lang != null)
               if (ASIErrorCheck.IsLangSupported(lang))
               {
                  //Alarm Report
                  if ( repType == ReportTemplate.ReportTypes.Alarm ||
                       repType == ReportTemplate.ReportTypes.FleetAlarms)
                  {
                     foreach (DataRow dr in ds.Tables[0].Rows)
                        dr["Description"] = LocalizationLayer.GUILocalizationLayer.LocalizeAlarms(dr["Description"].ToString(), lang);
                  }
               }

               xml = ds.GetXml();
            }

            return (int)InterfaceError.NoError;
         }
         catch (Exception Ex)
         {
            return (int)ASIErrorCheck.CheckError(Ex);
         }
         finally
         {
            Trace.WriteLineIf(AppConfig.tsStat.Enabled | AppConfig.tsMain.TraceInfo, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Info, "Report: " + repType.ToString() + " finished."));
         }
      }
      [WebMethod]
      public int GetSystemReport(int userId, string SID, ReportTemplate.ReportTypes repType, string xmlParams, ref string xml)
      {
         try
         {
            Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceVerbose, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces, string.Format("GetSystemReport( userId = {0}, repType = {1}, xmlParams = {2} )", userId, repType, xmlParams)));

            Trace.WriteLineIf(AppConfig.tsStat.Enabled | AppConfig.tsMain.TraceInfo, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Info, "Report: " + repType.ToString() + " started."));

            // Authenticate 
            LoginManager.GetInstance().SecurityCheck(userId, SID);

            xml = "";

            using (ReportGenerator dbReport = new ReportGenerator(LoginManager.GetConnnectionString(userId)))
            {
               xml = dbReport.GetSystemReport(repType, xmlParams, userId);
            }

            return (int)InterfaceError.NoError;
         }
         catch (Exception Ex)
         {
            return (int)ASIErrorCheck.CheckError(Ex);
         }
         finally
         {
            Trace.WriteLineIf(AppConfig.tsStat.Enabled | AppConfig.tsMain.TraceInfo, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Info, "Report: " + repType.ToString() + " finished."));
         }
      }

      /** \comment the function is 
       */
      [WebMethod]
      public int GetSensorReportForFleet(int userId, string SID, int fleetId, int sensorId,
                                         DateTime dtFrom, DateTime dtTo, ref string xml)
      {
         try
         {
            Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceVerbose,
                     CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                     string.Format("GetSensorReportForFleet( userId = {0}, fleetId = {1}, sensorId = {2}, from = {3}, to = {4} )", userId, fleetId, sensorId, dtFrom, dtTo)));

            Trace.WriteLineIf(AppConfig.tsStat.Enabled | AppConfig.tsMain.TraceInfo,
                     CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Info,
                     "GetSensorReportForFleet: started."));

            // Authenticate 
            LoginManager.GetInstance().SecurityCheck(userId, SID);

            xml = "";
            using (ReportGenerator dbReport = new ReportGenerator(LoginManager.GetConnnectionString(userId)))
            {
               xml = dbReport.GetSensorReportForFleet(fleetId, userId, sensorId, dtFrom, dtTo);
            }

            return (int)InterfaceError.NoError;
         }
         catch (Exception Ex)
         {
            return (int)ASIErrorCheck.CheckError(Ex);
         }
         finally
         {
            Trace.WriteLineIf(AppConfig.tsStat.Enabled | AppConfig.tsMain.TraceInfo,
               CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Info,
               "GetSensorReportForFleet finished."));
         }
      }


      [WebMethod]
      public int GetActivityReportForFleet(int sensorId, int userId, string SID, int fleetId,
                                         DateTime dtFrom, DateTime dtTo, ref string xml)
      {
         try
         {
            Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceVerbose,
                     CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                     string.Format("GetActivityReportForFleet( userId = {0}, fleetId = {1}, from = {2}, to = {3} )", userId, fleetId, dtFrom, dtTo)));

            Trace.WriteLineIf(AppConfig.tsStat.Enabled | AppConfig.tsMain.TraceInfo,
                     CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Info,
                     "GetActivityReportForFleet: started."));

            // Authenticate 
            LoginManager.GetInstance().SecurityCheck(userId, SID);

            xml = "";
            using (ReportGenerator dbReport = new ReportGenerator(LoginManager.GetConnnectionString(userId)))
            {
               xml = dbReport.GetActivityReportForFleet(sensorId, fleetId, userId, dtFrom, dtTo);
            }

            return (int)InterfaceError.NoError;
         }
         catch (Exception Ex)
         {
            return (int)ASIErrorCheck.CheckError(Ex);
         }
         finally
         {
            Trace.WriteLineIf(AppConfig.tsStat.Enabled | AppConfig.tsMain.TraceInfo,
               CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Info,
               "GetActivityReportForFleet finished."));
         }
      }

      [WebMethod]
      public string GetReportsName(int guiId)
      {
         string tmp;
         //xml = "";
         using (VLF.DAS.Logic.Report dbReport = new VLF.DAS.Logic.Report(Application["ConnectionString"].ToString()))
         {
            tmp = dbReport.GetReportsName(guiId);
         }
         return tmp;
      }

      [WebMethod]
      [Obsolete("Use method: AddReportSchedule")]
      public int InsertSecheduledReport(int userID, DateTime periodStart, DateTime periodEnd,
                                       DateTime deliveryDeadLine, string xmlParams, string emails,
                                       string url, int reportType, DateTime statusDate, string deliveryPeriod, bool fleet)
      {
         int tmp;
         //xml = "";
         using (VLF.DAS.Logic.Report dbReport = new VLF.DAS.Logic.Report(LoginManager.GetConnnectionString(userID)))
         {
            tmp = dbReport.InsertSecheduledReport(userID, periodStart, periodEnd,
                                                 deliveryDeadLine, xmlParams, emails,
                                                 url, reportType, statusDate, deliveryPeriod, fleet);

         }
         return tmp;
      }

      [WebMethod]
      [Obsolete("Use method: GetScheduledReportsByUserID")]
      public DataSet GetSecheduledReports(int userId)
      {
         DataSet tmp;
         //xml = "";
         using (VLF.DAS.Logic.Report dbReport = new VLF.DAS.Logic.Report(LoginManager.GetConnnectionString(userId)))
         {
            tmp = dbReport.GetScheuledReportsByUser(userId);

         }
         return tmp;
      }

      [WebMethod]
      [Obsolete("Use method: GetScheduledReportsByUserID")]
      public DataSet GetSecheduledReportsByStatus(int userId)
      {
         DataSet tmp;
         //xml = "";
         using (VLF.DAS.Logic.Report dbReport = new VLF.DAS.Logic.Report(LoginManager.GetConnnectionString(userId)))
         {
            tmp = dbReport.GetScheuledReportsByStatus(userId);

         }
         return tmp;
      }

      [WebMethod]
      [Obsolete("Use method: DeleteScheduledReportByReportID")]
      public int DeleteSecheduledReport(int reportID)
      {
         int tmp;
         //xml = "";
         using (VLF.DAS.Logic.Report dbReport = new VLF.DAS.Logic.Report(Application["ConnectionString"].ToString()))
         {
            tmp = dbReport.DeleteSecheduledReport(reportID);

         }
         return tmp;
      }

      [WebMethod]
      [Obsolete("Not Used")]
      public int UpdateScheuledReportDatesAndURL(int reportID, DateTime newDelivery, DateTime newFrom, DateTime newTo, string url, int status)
      {
         int tmp = 0;
         //xml = "";
         using (VLF.DAS.Logic.Report dbReport = new VLF.DAS.Logic.Report(Application["ConnectionString"].ToString()))
         {
            tmp = dbReport.UpdateScheuledReportDatesAndURL(reportID, newDelivery, newFrom, newTo, url, status);
         }
         return tmp;
      }

      [WebMethod]
      public int GetViolationReportForFleet(int userId, string SID, int fleetId,
                                         int maskViolations, DateTime dtFrom, DateTime dtTo, int speed, ref string xml)
      {
         try
         {
            Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceVerbose,
                     CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                     string.Format("GetViolationReportForFleet( userId = {0}, fleetId = {1}, maskViolations={2}, from = {3}, to = {4} )", userId, fleetId, maskViolations, dtFrom, dtTo)));

            Trace.WriteLineIf(AppConfig.tsStat.Enabled | AppConfig.tsMain.TraceInfo,
                     CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Info,
                     "GetViolationReportForFleet: started."));

            // Authenticate 
            LoginManager.GetInstance().SecurityCheck(userId, SID);

            xml = "";
            using (ReportGenerator dbReport = new ReportGenerator(LoginManager.GetConnnectionString(userId)))
            {
               xml = dbReport.GetViolationReportForFleet(fleetId, userId, maskViolations, dtFrom, dtTo, speed);
            }

            return (int)InterfaceError.NoError;
         }
         catch (Exception Ex)
         {
            return (int)ASIErrorCheck.CheckError(Ex);
         }
         finally
         {
            Trace.WriteLineIf(AppConfig.tsStat.Enabled | AppConfig.tsMain.TraceInfo,
               CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Info,
               "GetViolationReportForFleet finished."));
         }
      }

      [WebMethod]
      public int GetViolationReportForFleetByLang(int userId, string SID, int fleetId, int maskViolations,
                                                  DateTime dtFrom, DateTime dtTo, string lang, int speed, ref string xml)
      {
         try
         {
            Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceVerbose,
                     CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                     string.Format("GetViolationReportForFleet( userId = {0}, fleetId = {1}, maskViolations={2}, from = {3}, to = {4} )", userId, fleetId, maskViolations, dtFrom, dtTo)));

            Trace.WriteLineIf(AppConfig.tsStat.Enabled | AppConfig.tsMain.TraceInfo,
                     CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Info,
                     "GetViolationReportForFleet: started."));

            // Authenticate 
            LoginManager.GetInstance().SecurityCheck(userId, SID);

            xml = "";
            using (ReportGenerator dbReport = new ReportGenerator(LoginManager.GetConnnectionString(userId)))
            {
               xml = dbReport.GetViolationReportForFleet(fleetId, userId, maskViolations, dtFrom, dtTo, speed);

               DataSet ds = new DataSet();
               StringReader sr = new StringReader(xml);

               ds.ReadXml(sr);

               foreach (DataRow dr in ds.Tables[0].Rows)
                  dr["Description"] = LocalizationLayer.GUILocalizationLayer.LocalizeViolations(dr["Description"].ToString(), lang);

               xml = ds.GetXml();
            }

            return (int)InterfaceError.NoError;
         }
         catch (Exception Ex)
         {
            return (int)ASIErrorCheck.CheckError(Ex);
         }
         finally
         {
            Trace.WriteLineIf(AppConfig.tsStat.Enabled | AppConfig.tsMain.TraceInfo,
               CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Info,
               "GetViolationReportForFleet finished."));
         }
      }


      [WebMethod]
      public int GetActiveVehiclesPerDay(int userId, string SID, int fleetId,
                                         DateTime dtFrom, DateTime dtTo, ref string xml)
      {
         try
         {
            Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceVerbose,
                     CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                     string.Format("GetActiveVehiclesPerDay( userId = {0}, fleetId = {1}, from = {2}, to = {3} )", userId, fleetId, dtFrom, dtTo)));

            Trace.WriteLineIf(AppConfig.tsStat.Enabled | AppConfig.tsMain.TraceInfo,
                     CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Info,
                     "GetActiveVehiclesPerDay: started."));

            // Authenticate 
            LoginManager.GetInstance().SecurityCheck(userId, SID);

            xml = "";
            using (ReportGenerator dbReport = new ReportGenerator(LoginManager.GetConnnectionString(userId)))
            {
               xml = dbReport.GetActiveVehiclesPerDay(fleetId, userId, dtFrom, dtTo);
            }

            return (int)InterfaceError.NoError;
         }
         catch (Exception Ex)
         {
            return (int)ASIErrorCheck.CheckError(Ex);
         }
         finally
         {
            Trace.WriteLineIf(AppConfig.tsStat.Enabled | AppConfig.tsMain.TraceInfo,
               CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Info,
               "GetActivityReportForFleet finished."));
         }
      }


      [WebMethod]
      public int GetViolationReportForFleetWithScore(int userId, string SID, int fleetId,
                                         int maskViolations, DateTime dtFrom, DateTime dtTo, string ViolationPoints, ref string xml)
      {
         try
         {
            Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceVerbose,
                     CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                     string.Format("GetViolationReportForFleetWithScore( userId = {0}, fleetId = {1}, maskViolations={2}, from = {3}, to = {4} )", userId, fleetId, maskViolations, dtFrom, dtTo)));

            Trace.WriteLineIf(AppConfig.tsStat.Enabled | AppConfig.tsMain.TraceInfo,
                     CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Info,
                     "GetViolationReportForFleetWithScore: started."));

            // Authenticate 
            LoginManager.GetInstance().SecurityCheck(userId, SID);

            xml = "";
            using (ReportGenerator dbReport = new ReportGenerator(LoginManager.GetConnnectionString(userId)))
            {
               xml = dbReport.GetViolationReportForFleetWithScore(fleetId, userId, maskViolations, dtFrom, dtTo, ViolationPoints);
            }

            return (int)InterfaceError.NoError;
         }
         catch (Exception Ex)
         {
            return (int)ASIErrorCheck.CheckError(Ex);
         }
         finally
         {
            Trace.WriteLineIf(AppConfig.tsStat.Enabled | AppConfig.tsMain.TraceInfo,
               CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Info,
               "GetViolationReportForFleet finished."));
         }
      }


       [WebMethod]
       public int GetTripsSummaryDataByLicensePlate(int userId, string SID, string licensePlate,
                                          Int16 sensorNum, DateTime dtFrom, DateTime dtTo, ref string xml)
       {
           try
           {
               Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceVerbose,
                        CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                        string.Format("GetTripsSummaryDataByLicensePlate( userId = {0}, licensePlate = {1},  from = {2}, to = {3} )", userId, licensePlate, dtFrom, dtTo)));

               Trace.WriteLineIf(AppConfig.tsStat.Enabled | AppConfig.tsMain.TraceInfo,
                        CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Info,
                        "GetTripsSummaryDataByLicensePlate: started."));

               // Authenticate 
               LoginManager.GetInstance().SecurityCheck(userId, SID);
               DataSet ds = new DataSet();
               xml = "";


               string ConnnectionString = GetConnectionStringByDateRange(userId, dtFrom, dtTo);

               using (VLF.DAS.Logic.Report rpt = new VLF.DAS.Logic.Report(ConnnectionString))
               {
                   ds = rpt.GetTripsSummaryReport(userId, licensePlate, sensorNum, dtFrom, dtTo);
                   ds.DataSetName = "dstTripSummaryPerVehicle";
               }

               xml = ds.GetXml(); 

               return (int)InterfaceError.NoError;
           }
           catch (Exception Ex)
           {
               return (int)ASIErrorCheck.CheckError(Ex);
           }
           finally
           {
               Trace.WriteLineIf(AppConfig.tsStat.Enabled | AppConfig.tsMain.TraceInfo,
                  CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Info,
                  "GetTripsSummaryDataByLicensePlate finished."));
           }
       }

       private string GetConnectionStringByDateRange(int userId, DateTime from, DateTime to)
       {
           string ConnnectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DBReportConnectionString"].ConnectionString;

           if ((from.Day == DateTime.Now.Day) || (to.Day == DateTime.Now.Day) || (to > DateTime.Now))
               ConnnectionString = LoginManager.GetConnnectionString(userId);

           return ConnnectionString;
       }
       

      /*
            [WebMethod]
            public int GetSensorReportForVehicle(int userId, string SID, int vehicleId, int sensorId,
                                               DateTime dtFrom, DateTime dtTo, ref string xml)
            {
               try
               {
                  Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceVerbose, 
                           CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                           string.Format("GetSensorReportForVehicle( userId = {0}, VehiclId = {1}, sensorId = {2}, from = {3}, to = {4} )", userId, vehicleId, sensorId, dtFrom, dtTo)));

                  Trace.WriteLineIf(AppConfig.tsStat.Enabled | AppConfig.tsMain.TraceInfo, 
                           CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Info,
                           "GetSensorReportForVehicle: started."));

                  // Authenticate 
                  LoginManager.GetInstance().SecurityCheck(userId, SID);

                  xml = "";
                  ReportGenerator dbReport = null;
                  try
                  {
                     dbReport = new ReportGenerator(LoginManager.GetConnnectionString(userId));
                     xml = dbReport.GetSensorReportForFleet(fleetId, sensorId, dtFrom, dtTo);
                  }
                  finally
                  {
                     if (dbReport != null)
                        dbReport.Dispose();
                  }

                  return (int)InterfaceError.NoError;
               }
               catch (Exception Ex)
               {
                  return (int)ASIErrorCheck.CheckError(Ex);
               }
               finally
               {
                  Trace.WriteLineIf(AppConfig.tsStat.Enabled | AppConfig.tsMain.TraceInfo, 
                     CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Info,
                     "GetSensorReportForVehicle finished."));
               }
            }
       */

      # region Scheduled reports
      /*
      /// <summary>
      /// Add new scheduled report
      /// </summary>
      /// <param name="userId">User Id</param>
      /// <param name="SID">Security Id</param>
      /// <param name="from">Date from</param>
      /// <param name="to">Date to</param>
      /// <param name="isfleet">True if the report is for fleet, false if for for vehicle</param>
      /// <param name="fleetid">Fleet Id</param>
      /// <param name="parameters">Report parameters</param>
      /// <param name="email">Email to send report</param>
      /// <param name="guiid">Report guid</param>
      /// <param name="status">Report status</param>
      /// <param name="statusdate">Status date</param>
      /// <param name="frequency">Report freq. Id</param>
      /// <param name="freqparam">Report freq. parameter</param>
      /// <param name="startdate">Date to start processing reports</param>
      /// <param name="enddate">Date report is no valid</param>
      /// <param name="deliveryMethod">Method Id for sending</param>
      /// <returns>Error code</returns>
      [WebMethod]
      [Obsolete("Replaced by AddReportSchedule(int userId, string SID, string xmlSchedule)")]
      public int AddReportSchedule(int userId, string SID, DateTime from, DateTime to, bool isfleet, int fleetid,
         string parameters, string email, int guiid, string status, DateTime statusdate, short frequency, short freqparam,
         DateTime startdate, DateTime enddate, short deliveryMethod)
      {
         try
         {
            Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceVerbose,
                     CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                     string.Format("AddReportSchedule( userId = {0}, from = {1}, to = {2} )", userId, from, to)));

            Trace.WriteLineIf(AppConfig.tsStat.Enabled | AppConfig.tsMain.TraceInfo,
                     CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Info,
                     "AddReportSchedule: started."));

            // Authenticate 
            LoginManager.GetInstance().SecurityCheck(userId, SID);

            using (ReportScheduler scheduler = new ReportScheduler(LoginManager.GetConnnectionString(userId)))
            {
               scheduler.AddSchedule(from, to, isfleet, fleetid, parameters, email, userId,
                  guiid, status, statusdate, frequency, freqparam, startdate, enddate, deliveryMethod);
            }

            return (int)InterfaceError.NoError;
         }
         catch (Exception Ex)
         {
            return (int)ASIErrorCheck.CheckError(Ex);
         }
         finally
         {
            Trace.WriteLineIf(AppConfig.tsStat.Enabled | AppConfig.tsMain.TraceInfo,
               CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Info,
               "AddReportSchedule finished."));
         }
      }
*/
      /// <summary>
      /// Add new scheduled report
      /// </summary>
      /// <param name="userId">User Id</param>
      /// <param name="SID">Security Id</param>
      /// <param name="xmlSchedule">Schedule data</param>
      /// <returns>ASI Error code</returns>
      [WebMethod]
      public int AddReportSchedule(int userId, string SID, string xmlSchedule)
      {
         try
         {
            Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceVerbose,
                     CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                     string.Format("AddReportSchedule( userId = {0})", userId)));

            // Authenticate 
            LoginManager.GetInstance().SecurityCheck(userId, SID);

            if (String.IsNullOrEmpty(xmlSchedule))
            {
               return (int)InterfaceError.InvalidParameter;
            }

            using (ReportScheduler scheduler = new ReportScheduler(LoginManager.GetConnnectionString(userId)))
            {
               XmlUtil xDoc = new XmlUtil(xmlSchedule);

               scheduler.AddSchedule(
                  Convert.ToDateTime(xDoc.GetNodeValue("FromDate"), new System.Globalization.CultureInfo("en-US")),
                  Convert.ToDateTime(xDoc.GetNodeValue("ToDate"), new System.Globalization.CultureInfo("en-US")),
                  Convert.ToBoolean(xDoc.GetNodeValue("IsFleet")),
                  Convert.ToInt32(xDoc.GetNodeValue("FleetId")),
                  xDoc.GetNodeValue("XmlParams"),
                  xDoc.GetNodeValue("Email"),
                  userId,
                  Convert.ToInt32(xDoc.GetNodeValue("GuiId")),
                  xDoc.GetNodeValue("Status"),
                  Convert.ToDateTime(xDoc.GetNodeValue("StatusDate"), new System.Globalization.CultureInfo("en-US")),
                  Convert.ToInt16(xDoc.GetNodeValue("Frequency")),
                  Convert.ToInt16(xDoc.GetNodeValue("FrequencyParam")),
                  Convert.ToDateTime(xDoc.GetNodeValue("StartDate"), new System.Globalization.CultureInfo("en-US")),
                  Convert.ToDateTime(xDoc.GetNodeValue("EndDate"), new System.Globalization.CultureInfo("en-US")),
                  Convert.ToInt16(xDoc.GetNodeValue("DeliveryMethod")),
                  xDoc.GetNodeValue("ReportLanguage"),
                  Convert.ToInt16(xDoc.GetNodeValue("ReportFormat")) // new 2008-05-05
                  );
            }

            return (int)InterfaceError.NoError;
         }
         catch (Exception ex)
         {
            Trace.WriteLineIf(AppConfig.tsMain.TraceError, "AddReportSchedule::" + ex.Message);
            return (int)ASIErrorCheck.CheckError(ex);
         }
         finally
         {
            Trace.WriteLineIf(AppConfig.tsStat.Enabled | AppConfig.tsMain.TraceInfo,
               CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Info,
               "AddReportSchedule finished."));
         }
      }

      /// <summary>
      /// Get all scheduled reports for user
      /// </summary>
      /// <param name="userid">User Id</param>
      /// <param name="sid">Security Id</param>
      /// <param name="xml">Referenced string for storing DataSet</param>
      /// <returns>Error code</returns>
      [WebMethod]
      public int GetScheduledReportsByUserID(int userid, string sid, ref string xml)
      {
         try
         {
            Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceVerbose,
                              CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                              String.Format("GetScheduledReportsByUserID( userId = {0} )", userid)));

            // Authenticate
            LoginManager.GetInstance().SecurityCheck(userid, sid);
            xml = "";

            using (ReportScheduler scheduler = new ReportScheduler(LoginManager.GetConnnectionString(userid)))
            {
               xml = (scheduler.GetScheduledReportsByUserID(userid)).GetXml();
            }

            if (String.IsNullOrEmpty(xml))
               return (int)InterfaceError.NotFound;

         }
         catch (Exception ex)
         {
            Trace.WriteLineIf(AppConfig.tsMain.TraceError, "GetScheduledReportsByUserID::" + ex.Message);
            return (int)ASIErrorCheck.CheckError(ex);
         }
         finally
         {
            Trace.WriteLineIf(AppConfig.tsStat.Enabled | AppConfig.tsMain.TraceInfo,
                              CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Info,
                              "GetScheduledReportsByUserID finished."));
         }
         return (int)InterfaceError.NoError;
      }

      /// <summary>
      /// Delete report by id
      /// </summary>
      /// <param name="userid">User Id</param>
      /// <param name="sid">Security Id</param>
      /// <param name="reportid">Report Id</param>
      /// <returns>Error code</returns>
      [WebMethod]
      public int DeleteScheduledReportByReportID(int userid, string sid, int reportid)
      {
         try
         {
            Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceVerbose,
                              CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                              String.Format("DeleteScheduledReportByReportID( userId = {0} reportid = {1})", userid, reportid)));

            // Authenticate
            LoginManager.GetInstance().SecurityCheck(userid, sid);

            using (ReportScheduler scheduler = new ReportScheduler(LoginManager.GetConnnectionString(userid)))
            {
               scheduler.DeleteScheduledReportByReportID(reportid);
            }

            return (int)InterfaceError.NoError;
         }

         catch (Exception ex)
         {
            return (int)ASIErrorCheck.CheckError(ex);
         }

         finally
         {
            Trace.WriteLineIf(AppConfig.tsStat.Enabled | AppConfig.tsMain.TraceInfo,
                              CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Info,
                              "DeleteScheduledReportByReportID finished."));
         }
      }

      /// <summary>
      /// Get all files available for user download
      /// </summary>
      /// <param name="userid">User Id</param>
      /// <param name="sid">Security Id</param>
      /// <param name="xml">Referenced string for storing DataSet xml</param>
      /// DataSet table columns: [RowID, ReportFileName, DateCreated, GuiName]
      /// <returns>Error code</returns>
      [WebMethod]
      public int GetReportFilesByUserID(int userid, string sid, ref string xml)
      {
         try
         {
            Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceVerbose,
                              CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                              String.Format("GetReportFilesByUserID( userId = {0} )", userid)));

            // Authenticate
            LoginManager.GetInstance().SecurityCheck(userid, sid);
            xml = "";

            using (ReportScheduler scheduler = new ReportScheduler(LoginManager.GetConnnectionString(userid)))
            {
               xml = (scheduler.GetReportFilesByUserId(userid)).GetXml();
            }
            if (String.IsNullOrEmpty(xml))
               return (int)InterfaceError.NotFound;

         }
         catch (Exception ex)
         {
            return (int)ASIErrorCheck.CheckError(ex);
         }
         finally
         {
            Trace.WriteLineIf(AppConfig.tsStat.Enabled | AppConfig.tsMain.TraceInfo,
                              CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Info,
                              "GetReportFilesByUserID finished."));
         }
         return (int)InterfaceError.NoError;
      }

      /// <summary>
      /// Get all report files available for user download
      /// </summary>
      /// <param name="userid">User Id</param>
      /// <param name="sid">Security Id</param>
      /// <param name="xml">Referenced string for storing DataSet xml</param>
      /// DataSet table columns: [RowID, ReportFileName, DateCreated, GuiName]
      /// <returns>Error code</returns>
      [WebMethod]
      public int GetReportFilesByReportID(int userId, int reportId, string sid, ref string xml)
      {
         try
         {
            Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceVerbose,
                              CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                              String.Format("GetReportFilesByReportID( userId = {0} reportId = {1})", userId, reportId)));

            // Authenticate
            LoginManager.GetInstance().SecurityCheck(userId, sid);
            xml = "";

            using (ReportScheduler scheduler = new ReportScheduler(LoginManager.GetConnnectionString(userId)))
            {
               xml = (scheduler.GetReportFilesByReportId(reportId)).GetXml();
            }

            if (String.IsNullOrEmpty(xml))
               return (int)InterfaceError.NotFound;
         }
         catch (Exception ex)
         {
            return (int)ASIErrorCheck.CheckError(ex);
         }
         finally
         {
            Trace.WriteLineIf(AppConfig.tsStat.Enabled | AppConfig.tsMain.TraceInfo,
                              CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Info,
                              "GetReportFilesByReportID finished."));
         }
         return (int)InterfaceError.NoError;
      }

      /// <summary>
      /// Delete download file row
      /// </summary>
      /// <param name="userid">User Id</param>
      /// <param name="sid">Security Id</param>
      /// <param name="rowid">Row Id</param>
      /// <returns>Error code</returns>
      [WebMethod]
      public int DeleteReportFileByRowID(int userid, string sid, int rowid)
      {
         try
         {
            Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceVerbose,
                              CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                              String.Format("DeleteReportFileByRowID( userId = {0} rowid = {1} )", userid, rowid)));

            // Authenticate
            LoginManager.GetInstance().SecurityCheck(userid, sid);

            using (ReportScheduler scheduler = new ReportScheduler(LoginManager.GetConnnectionString(userid)))
            {
               scheduler.DeleteReportFile(rowid);
            }

            return (int)InterfaceError.NoError;
         }

         catch (Exception ex)
         {
            return (int)ASIErrorCheck.CheckError(ex);
         }

         finally
         {
            Trace.WriteLineIf(AppConfig.tsStat.Enabled | AppConfig.tsMain.TraceInfo,
                              CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Info,
                              "DeleteReportFileByRowID finished."));
         }
      }

      # endregion

   
  }
}
