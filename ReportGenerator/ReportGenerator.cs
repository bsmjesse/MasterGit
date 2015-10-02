using System;
using System.Data;
using System.Collections;
using VLF.Reports;
using VLF.CLS;
using VLF.ERR;
using System.IO;


namespace VLF.Reports
{
    /// <summary>
    /// Generates strong-type dataset for report
    /// </summary>
    public class ReportGenerator : IDisposable
    {
        private string connectionString = "";
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString"></param>
        public ReportGenerator(string connectionString)
        {
            this.connectionString = connectionString;
        }
        /// <summary>
        /// Destructor
        /// </summary>
        public void Dispose()
        {
        }

        // Changes for TimeZone Feature start
        /// <summary>
        /// Get strong-type dataset as XML.
        /// </summary>
        /// <param name="repType"></param>
        /// <param name="xmlParams"></param>
        /// <param name="userId"></param>
        /// <param name="requestOverflowed"></param>
        /// <param name="totalSqlRecords"></param>
        /// <param name="outMaxOverflowed"></param>
        /// <param name="outMaxRecords"></param>
        /// <returns>XML</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public string GetXml_NewTZ(ReportTemplate.ReportTypes repType, string xmlParams, int userId, string lang,
                       ref bool requestOverflowed, ref int totalSqlRecords,
                       ref bool outMaxOverflowed, ref int outMaxRecords)
        {
            VLF.DAS.Logic.User dbUser = null;
            VLF.DAS.Logic.Fleet dbFleet = null;
            string xmlDataSet = "";
            try
            {
                dbUser = new VLF.DAS.Logic.User(connectionString);
                bool showExceptionOnly = false;
                string prm1 = "";
                string prm2 = "";
                string prm3 = "";
                string prm4 = "";
                string prm5 = "";
                string prm6 = "";
                string prm7 = "";
                string prm8 = "";
                string prm9 = "";
                string prm10 = "";
                string prm11 = "";
                string prm12 = "";
                string prm13 = "";
                string prm14 = "";
                string prm15 = "";
                string prm16 = "";
                string prm17 = "";
                string prm18 = "";
                string prm19 = "";
                string prm20 = "";
                string prm21 = "";
                string prm22 = "";
                string prm23 = "";
                string prm24 = "";
                string prm25 = "";
                string prm26 = "";
                string prm27 = "";
                string prm28 = "";
                string prm29 = "";
                string prm30 = "";

                DataSet dsReport = null;
                requestOverflowed = false;
                totalSqlRecords = 0;
                outMaxOverflowed = false;
                outMaxRecords = 0;

                switch (repType)
                {
                    case ReportTemplate.ReportTypes.DetailedTrip:
                        #region Prepares Detailed Trip Report
                        VLF.DAS.Logic.Report detailedTrip = new VLF.DAS.Logic.Report(connectionString);
                        prm1 = Util.PairFindValue(ReportTemplate.RpDetailedTripFirstParamName, xmlParams);
                        prm2 = Util.PairFindValue(ReportTemplate.RpDetailedTripSecondParamName, xmlParams);
                        prm3 = Util.PairFindValue(ReportTemplate.RpDetailedTripThirdParamName, xmlParams);
                        prm4 = Util.PairFindValue(ReportTemplate.RpDetailedTripFourthParamName, xmlParams);
                        prm5 = Util.PairFindValue(ReportTemplate.RpDetailedTripFifthParamName, xmlParams);
                        prm6 = Util.PairFindValue(ReportTemplate.RpDetailedTripSixthParamName, xmlParams);
                        prm7 = Util.PairFindValue(ReportTemplate.RpDetailedTripSeventhParamName, xmlParams);
                        prm8 = Util.PairFindValue(ReportTemplate.RpDetailedTripEighthParamName, xmlParams);
                        prm9 = Util.PairFindValue(ReportTemplate.RpDetailedTripNinthParamName, xmlParams);
                        prm10 = Util.PairFindValue(ReportTemplate.RpDetailedTripTenthParamName, xmlParams);

                        if (xmlParams == null || prm1 == null || prm2 == null || prm3 == null || prm4 == null ||
                           prm5 == null || prm6 == null || prm7 == null || prm8 == null || prm9 == null || prm10 == null)
                        {
                            // empty result
                            return xmlDataSet;
                        }
                        //Authorization
                        if (!dbUser.ValidateUserLicensePlateOne(userId, prm1))
                            throw new ASIAuthorizationFailedException("User: " + userId + " LicensePlate: " + prm1);
                        dsReport = detailedTrip.GetDetailedTripReport_NewTZ(prm1, prm2, prm3,
                           Convert.ToBoolean(prm4),
                           Convert.ToBoolean(prm5),
                           Convert.ToBoolean(prm6),
                           Convert.ToBoolean(prm7),
                           Convert.ToBoolean(prm8),
                           Convert.ToBoolean(prm9),
                           userId, Convert.ToInt32(prm10), lang,
                           ref requestOverflowed,
                           ref totalSqlRecords,
                           ref outMaxOverflowed,
                           ref outMaxRecords);
                        if (dsReport != null)
                            xmlDataSet = dsReport.GetXml();
                        #endregion
                        break;
                    case ReportTemplate.ReportTypes.Trip:
                        #region Prepares Single Vehicle Trip Report
                        VLF.DAS.Logic.Report tripReport = new VLF.DAS.Logic.Report(connectionString);
                        prm1 = Util.PairFindValue(ReportTemplate.RpTripFirstParamName, xmlParams);
                        prm2 = Util.PairFindValue(ReportTemplate.RpTripSecondParamName, xmlParams);
                        prm3 = Util.PairFindValue(ReportTemplate.RpTripThirdParamName, xmlParams);
                        prm4 = Util.PairFindValue(ReportTemplate.RpTripFourthParamName, xmlParams);
                        prm5 = Util.PairFindValue(ReportTemplate.RpTripFifthParamName, xmlParams);
                        if ((xmlParams == null) || (prm1 == null) || (prm2 == null) || (prm3 == null) || (prm4 == null) || (prm5 == null))
                        {
                            // empty result
                            return xmlDataSet;
                        }
                        //Authorization
                        if (!dbUser.ValidateUserLicensePlateOne(userId, prm1))
                            throw new ASIAuthorizationFailedException("User: " + userId + " LicensePlate: " + prm1);
                        dsReport = tripReport.GetTripReport_NewTZ(prm1, prm2, prm3, userId, Convert.ToBoolean(prm4), Convert.ToInt32(prm5), lang,
                           ref requestOverflowed, ref totalSqlRecords,
                           ref outMaxOverflowed, ref outMaxRecords);
                        if (dsReport != null)
                            xmlDataSet = dsReport.GetXml();
                        #endregion
                        break;
                    case ReportTemplate.ReportTypes.Stop:
                        #region Prepares Single Vehicle Stop Report
                        VLF.DAS.Logic.Report stopReport = new VLF.DAS.Logic.Report(connectionString);
                        prm1 = Util.PairFindValue(ReportTemplate.RpStopFirstParamName, xmlParams);
                        prm2 = Util.PairFindValue(ReportTemplate.RpStopSecondParamName, xmlParams);
                        prm3 = Util.PairFindValue(ReportTemplate.RpStopThirdParamName, xmlParams);
                        prm4 = Util.PairFindValue(ReportTemplate.RpStopFourthParamName, xmlParams);
                        prm5 = Util.PairFindValue(ReportTemplate.RpStopFifthParamName, xmlParams);
                        prm6 = Util.PairFindValue(ReportTemplate.RpStopSixthParamName, xmlParams);
                        prm7 = Util.PairFindValue(ReportTemplate.RpStopSeventhParamName, xmlParams);
                        prm8 = Util.PairFindValue(ReportTemplate.RpStopEighthParamName, xmlParams);

                        if (xmlParams == null || prm1 == null || prm2 == null || prm3 == null || prm4 == null || prm5 == null || prm6 == null || prm7 == null || prm8 == null)
                        {
                            // empty result
                            return xmlDataSet;
                        }
                        //Authorization
                        if (!dbUser.ValidateUserLicensePlateOne(userId, prm1))
                            throw new ASIAuthorizationFailedException("User: " + userId + " LicensePlate: " + prm1);
                        if (Convert.ToBoolean(prm6) == false && Convert.ToBoolean(prm7) == false)
                            throw new ASIDataNotFoundException("Wrong filter values. Include at least one of following options either Stop or Idling.");
                        dsReport = stopReport.GetStopReport_NewTZ(prm1, prm2, prm3, userId, Convert.ToBoolean(prm4), Convert.ToInt32(prm5), Convert.ToBoolean(prm6), Convert.ToBoolean(prm7), Convert.ToInt32(prm8), lang, ref requestOverflowed, ref totalSqlRecords);
                        if (dsReport != null)
                            xmlDataSet = dsReport.GetXml();
                        #endregion
                        break;
                    case ReportTemplate.ReportTypes.TripActivity:
                        #region Prepares Vehicle Trip Activity Report
                        VLF.DAS.Logic.Report tripActivityReport = new VLF.DAS.Logic.Report(connectionString);
                        prm1 = Util.PairFindValue(ReportTemplate.RpTripFirstParamName, xmlParams);
                        prm2 = Util.PairFindValue(ReportTemplate.RpTripSecondParamName, xmlParams);
                        prm3 = Util.PairFindValue(ReportTemplate.RpTripThirdParamName, xmlParams);
                        prm4 = Util.PairFindValue(ReportTemplate.RpTripFourthParamName, xmlParams);
                        prm5 = Util.PairFindValue(ReportTemplate.RpTripFifthParamName, xmlParams);
                        if ((xmlParams == null) || (prm1 == null) || (prm2 == null) || (prm3 == null) || (prm4 == null) || (prm5 == null))
                        {
                            // empty result
                            return xmlDataSet;
                        }
                        //Authorization
                        if (!dbUser.ValidateUserLicensePlateOne(userId, prm1))
                            throw new ASIAuthorizationFailedException("User: " + userId + " LicensePlate: " + prm1);
                        dsReport = tripActivityReport.GetTripActivityReport_NewTZ(prm1, prm2, prm3, userId, Convert.ToBoolean(prm4), Convert.ToInt32(prm5), lang, ref requestOverflowed, ref totalSqlRecords);
                        if (dsReport != null)
                            xmlDataSet = dsReport.GetXml();
                        #endregion
                        break;
                    case ReportTemplate.ReportTypes.FleetDetailedTrip:
                        #region Prepares Fleet Detailed Trip Report
                        VLF.DAS.Logic.Report fleetDetailedTrip = new VLF.DAS.Logic.Report(connectionString);
                        prm1 = Util.PairFindValue(ReportTemplate.RpFleetDetailedTripFirstParamName, xmlParams);
                        prm2 = Util.PairFindValue(ReportTemplate.RpFleetDetailedTripSecondParamName, xmlParams);
                        prm3 = Util.PairFindValue(ReportTemplate.RpFleetDetailedTripThirdParamName, xmlParams);
                        prm4 = Util.PairFindValue(ReportTemplate.RpFleetDetailedTripFourthParamName, xmlParams);
                        prm5 = Util.PairFindValue(ReportTemplate.RpFleetDetailedTripFifthParamName, xmlParams);
                        prm6 = Util.PairFindValue(ReportTemplate.RpFleetDetailedTripSixthParamName, xmlParams);
                        prm7 = Util.PairFindValue(ReportTemplate.RpFleetDetailedTripSeventhParamName, xmlParams);
                        prm8 = Util.PairFindValue(ReportTemplate.RpFleetDetailedTripEighthParamName, xmlParams);
                        prm9 = Util.PairFindValue(ReportTemplate.RpFleetDetailedTripNinthParamName, xmlParams);
                        prm10 = Util.PairFindValue(ReportTemplate.RpFleetDetailedTripTenthParamName, xmlParams);

                        if ((xmlParams == null) || (prm1 == null) || (prm2 == null) || (prm3 == null) ||
                           (prm4 == null) || (prm5 == null) || (prm6 == null) || (prm7 == null) || (prm8 == null) || (prm9 == null) || (prm10 == null))
                        {
                            // empty result
                            return xmlDataSet;
                        }
                        //Authorization
                        //if (!dbUser.ValidateUserFleetOne(userId, Convert.ToInt32(prm1)))
                        //    throw new ASIAuthorizationFailedException("User: " + userId + " Fleet: " + prm1);
                        dsReport = fleetDetailedTrip.GetFleetDetailedTripReport_NewTZ(
                           Convert.ToInt16(prm1),
                           prm2,
                           prm3,
                           Convert.ToBoolean(prm4),
                           Convert.ToBoolean(prm5),
                           Convert.ToBoolean(prm6),
                           Convert.ToBoolean(prm7),
                           Convert.ToBoolean(prm8),
                           Convert.ToBoolean(prm9),
                           userId, Convert.ToInt32(prm10), lang,
                           ref requestOverflowed,
                           ref totalSqlRecords,
                           ref outMaxOverflowed,
                           ref outMaxRecords);
                        if (dsReport != null)
                            xmlDataSet = dsReport.GetXml();
                        #endregion
                        break;
                    case ReportTemplate.ReportTypes.FleetTrip:
                        #region Prepares Fleet Trip Report
                        VLF.DAS.Logic.Report fleetTripReport = new VLF.DAS.Logic.Report(connectionString);
                        prm1 = Util.PairFindValue(ReportTemplate.RpFleetTripFirstParamName, xmlParams);
                        prm2 = Util.PairFindValue(ReportTemplate.RpFleetTripSecondParamName, xmlParams);
                        prm3 = Util.PairFindValue(ReportTemplate.RpFleetTripThirdParamName, xmlParams);
                        prm4 = Util.PairFindValue(ReportTemplate.RpFleetTripFourthParamName, xmlParams);
                        prm5 = Util.PairFindValue(ReportTemplate.RpFleetTripFifthParamName, xmlParams);
                        if ((xmlParams == null) || (prm1 == null) || (prm2 == null) || (prm3 == null) || (prm4 == null) || (prm5 == null))
                        {
                            // empty result
                            return xmlDataSet;
                        }
                        //Authorization
                        //if (!dbUser.ValidateUserFleetOne(userId, Convert.ToInt32(prm1)))
                        //    throw new ASIAuthorizationFailedException("User: " + userId + " Fleet: " + prm1);
                        dsReport = fleetTripReport.GetFleetTripReport_NewTZ(Convert.ToInt32(prm1),
                           prm2,
                           prm3,
                           userId,
                           Convert.ToBoolean(prm4), Convert.ToInt32(prm5), lang,
                           ref requestOverflowed,
                           ref totalSqlRecords,
                           ref outMaxOverflowed,
                           ref outMaxRecords);
                        if (dsReport != null)
                            xmlDataSet = dsReport.GetXml();
                        #endregion
                        break;
                    case ReportTemplate.ReportTypes.FleetStop:
                        #region Prepares Fleet Stop Report
                        VLF.DAS.Logic.Report fleetStopReport = new VLF.DAS.Logic.Report(connectionString);
                        prm1 = Util.PairFindValue(ReportTemplate.RpFleetStopFirstParamName, xmlParams);
                        prm2 = Util.PairFindValue(ReportTemplate.RpFleetStopSecondParamName, xmlParams);
                        prm3 = Util.PairFindValue(ReportTemplate.RpFleetStopThirdParamName, xmlParams);
                        prm4 = Util.PairFindValue(ReportTemplate.RpFleetStopFourthParamName, xmlParams);
                        prm5 = Util.PairFindValue(ReportTemplate.RpFleetStopFifthParamName, xmlParams);
                        prm6 = Util.PairFindValue(ReportTemplate.RpFleetStopSixthParamName, xmlParams);
                        prm7 = Util.PairFindValue(ReportTemplate.RpFleetStopSeventhParamName, xmlParams);
                        prm8 = Util.PairFindValue(ReportTemplate.RpFleetStopEighthParamName, xmlParams);
                        if (xmlParams == null || prm1 == null || prm2 == null || prm3 == null || prm4 == null || prm5 == null || prm6 == null || prm7 == null || prm8 == null)
                        {
                            // empty result
                            return xmlDataSet;
                        }
                        //Authorization
                        //if (!dbUser.ValidateUserFleetOne(userId, Convert.ToInt32(prm1)))
                        //    throw new ASIAuthorizationFailedException("User: " + userId + " Fleet: " + prm1);
                        if (Convert.ToBoolean(prm6) == false && Convert.ToBoolean(prm7) == false)
                            throw new ASIDataNotFoundException("Wrong filter values. Include at least one of following options either Stop or Idling.");
                        dsReport = fleetStopReport.GetFleetStopReport_NewTZ(Convert.ToInt32(prm1),
                     prm2,
                     prm3,
                     userId,
                     Convert.ToBoolean(prm4),
                     Convert.ToInt32(prm5),
                            Convert.ToBoolean(prm6),
                            Convert.ToBoolean(prm7), Convert.ToInt32(prm8), lang,
                     ref requestOverflowed,
                     ref totalSqlRecords);
                        if (dsReport != null)
                            xmlDataSet = dsReport.GetXml();
                        #endregion
                        break;
                    case ReportTemplate.ReportTypes.Alarm:
                        #region Prepares Alarm Report
                        VLF.DAS.Logic.Report alarmReport = new VLF.DAS.Logic.Report(connectionString);
                        prm1 = Util.PairFindValue(ReportTemplate.RpAlarmFirstParamName, xmlParams);
                        prm2 = Util.PairFindValue(ReportTemplate.RpAlarmSecondParamName, xmlParams);
                        prm3 = Util.PairFindValue(ReportTemplate.RpAlarmThirdParamName, xmlParams);
                        if ((xmlParams == null) || (prm1 == null) || (prm2 == null) || (prm3 == null))
                        {
                            // empty result
                            return xmlDataSet;
                        }
                        //Authorization
                        if (!dbUser.ValidateUserLicensePlateOne(userId, prm1))
                            throw new ASIAuthorizationFailedException("User: " + userId + " LicensePlate: " + prm1);
                        dsReport = alarmReport.GetAlarmReport(userId, prm1, prm2, prm3, ref requestOverflowed, ref totalSqlRecords);

                        if (dsReport != null)
                        {
                            if (lang != "en" && lang != null)
                            {
                                LocalizationLayer.ServerLocalizationLayer dbl = new LocalizationLayer.ServerLocalizationLayer(connectionString);
                                dbl.LocalizationData(lang, "AlarmSeverity", "AlarmSeverityName", "AlarmSeverity", ref dsReport);
                            }

                            xmlDataSet = dsReport.GetXml();
                        }

                        #endregion
                        break;
                    case ReportTemplate.ReportTypes.FleetAlarms:
                        #region Prepares Fleet Alarms Report
                        VLF.DAS.Logic.Report fleetAlamsReport = new VLF.DAS.Logic.Report(connectionString);
                        prm1 = Util.PairFindValue(ReportTemplate.RpFleetAlarmsFirstParamName, xmlParams);
                        prm2 = Util.PairFindValue(ReportTemplate.RpFleetAlarmsSecondParamName, xmlParams);
                        prm3 = Util.PairFindValue(ReportTemplate.RpFleetAlarmsThirdParamName, xmlParams);
                        if ((xmlParams == null) || (prm1 == null) || (prm2 == null) || (prm3 == null))
                        {
                            // empty result
                            return xmlDataSet;
                        }
                        //Authorization
                        //if (!dbUser.ValidateUserFleetOne(userId, Convert.ToInt32(prm1)))
                        //    throw new ASIAuthorizationFailedException("User: " + userId + " Fleet: " + prm1);
                        dsReport = fleetAlamsReport.GetFleetAlarmsReport(userId, Convert.ToInt32(prm1), prm2, prm3, ref requestOverflowed, ref totalSqlRecords);




                        if (dsReport != null)
                        {
                            if (lang != "en" && lang != null)
                            {
                                LocalizationLayer.ServerLocalizationLayer dbl = new LocalizationLayer.ServerLocalizationLayer(connectionString);
                                dbl.LocalizationData(lang, "AlarmSeverity", "AlarmSeverityName", "AlarmSeverity", ref dsReport);
                            }

                            xmlDataSet = dsReport.GetXml();
                        }
                        #endregion
                        break;
                    case ReportTemplate.ReportTypes.SystemUsageExceptionReportForAllOrganizations:
                        #region Prepares System Usage Exception Report For All Organizations
                        VLF.DAS.Logic.Report systemUsageExceptionReport = new VLF.DAS.Logic.Report(connectionString);
                        prm1 = Util.PairFindValue(ReportTemplate.RpSystemUsageExceptionReportForAllOrganizationsFirstParamName, xmlParams);
                        prm2 = Util.PairFindValue(ReportTemplate.RpSystemUsageExceptionReportForAllOrganizationsSecondParamName, xmlParams);
                        if (xmlParams == null || prm1 == null || prm2 == null)
                        {
                            // empty result
                            return xmlDataSet;
                        }
                        //Authorization
                        if (!dbUser.ValidateHGISuperUser(userId))
                            throw new ASIAuthorizationFailedException("User: " + userId);
                        dsReport = systemUsageExceptionReport.GetSystemUsageExceptionReportForAllOrganizations(prm1, prm2);
                        if (dsReport != null)
                            xmlDataSet = dsReport.GetXml();
                        #endregion
                        break;
                    case ReportTemplate.ReportTypes.OrganizationSystemUsage:
                        #region Prepares Organization System Usage Report
                        VLF.DAS.Logic.Report orgSystemUsageReport = new VLF.DAS.Logic.Report(connectionString);
                        prm1 = Util.PairFindValue(ReportTemplate.RpOrganizationSystemUsageFirstParamName, xmlParams);
                        prm2 = Util.PairFindValue(ReportTemplate.RpOrganizationSystemUsageSecondParamName, xmlParams);
                        prm3 = Util.PairFindValue(ReportTemplate.RpOrganizationSystemUsageThirdParamName, xmlParams);
                        if ((xmlParams == null) || (prm1 == null) || (prm2 == null) || (prm3 == null))
                        {
                            // empty result
                            return xmlDataSet;
                        }
                        prm4 = Util.PairFindValue(ReportTemplate.RpOrganizationSystemUsageFourthParamName, xmlParams);
                        if (prm4 != null)
                            showExceptionOnly = Convert.ToBoolean(prm4);

                        //Authorization
                        if (!dbUser.ValidateHGISuperUser(userId))
                            throw new ASIAuthorizationFailedException("User: " + userId);
                        dsReport = orgSystemUsageReport.GetSystemUsageReportByOrganization(Convert.ToInt32(prm1), prm2, prm3, showExceptionOnly);
                        if (dsReport != null)
                            xmlDataSet = dsReport.GetXml();
                        #endregion
                        break;
                    case ReportTemplate.ReportTypes.BoxSystemUsage:
                        #region Prepares Box System Usage Report
                        VLF.DAS.Logic.Report boxSystemUsageReport = new VLF.DAS.Logic.Report(connectionString);
                        prm1 = Util.PairFindValue(ReportTemplate.RpBoxSystemUsageFirstParamName, xmlParams);
                        prm2 = Util.PairFindValue(ReportTemplate.RpBoxSystemUsageSecondParamName, xmlParams);
                        prm3 = Util.PairFindValue(ReportTemplate.RpBoxSystemUsageThirdParamName, xmlParams);
                        if ((xmlParams == null) || (prm1 == null) || (prm2 == null) || (prm3 == null))
                        {
                            // empty result
                            return xmlDataSet;
                        }
                        prm4 = Util.PairFindValue(ReportTemplate.RpBoxSystemUsageFourthParamName, xmlParams);
                        if (prm4 != null)
                            showExceptionOnly = Convert.ToBoolean(prm4);

                        //Authorization
                        //if (!dbUser.ValidateHGISuperUser(userId))
                        //throw new ASIAuthorizationFailedException("User: " + userId);
                        dsReport = boxSystemUsageReport.GetSystemUsageReportByBox(Convert.ToInt32(prm1), prm2, prm3, showExceptionOnly);
                        if (dsReport != null)
                            xmlDataSet = dsReport.GetXml();
                        #endregion
                        break;
                    case ReportTemplate.ReportTypes.Exception:
                        #region Prepares Exception Report
                        VLF.DAS.Logic.Report boxExceptionReport = new VLF.DAS.Logic.Report(connectionString);
                        prm1 = Util.PairFindValue(ReportTemplate.RpExceptionFirstParamName, xmlParams);
                        prm2 = Util.PairFindValue(ReportTemplate.RpExceptionSecondParamName, xmlParams);
                        prm3 = Util.PairFindValue(ReportTemplate.RpExceptionThirdParamName, xmlParams);
                        prm4 = Util.PairFindValue(ReportTemplate.RpExceptionFourthParamName, xmlParams);
                        prm5 = Util.PairFindValue(ReportTemplate.RpExceptionFifthParamName, xmlParams);
                        prm6 = Util.PairFindValue(ReportTemplate.RpExceptionSixthParamName, xmlParams);
                        prm7 = Util.PairFindValue(ReportTemplate.RpExceptionSeventhParamName, xmlParams);
                        prm8 = Util.PairFindValue(ReportTemplate.RpExceptionEightParamName, xmlParams);
                        prm9 = Util.PairFindValue(ReportTemplate.RpExceptionNineParamName, xmlParams);
                        prm10 = Util.PairFindValue(ReportTemplate.RpExceptionTenParamName, xmlParams);
                        prm11 = Util.PairFindValue(ReportTemplate.RpExceptionElevenParamName, xmlParams);
                        prm12 = Util.PairFindValue(ReportTemplate.RpExceptionTwelveParamName, xmlParams);
                        prm13 = Util.PairFindValue(ReportTemplate.RpExceptionThirteenParamName, xmlParams);
                        prm14 = Util.PairFindValue(ReportTemplate.RpExceptionFourteenParamName, xmlParams);
                        prm15 = Util.PairFindValue(ReportTemplate.RpExceptionFifteenParamName, xmlParams);
                        prm16 = Util.PairFindValue(ReportTemplate.RpExceptionSixteenParamName, xmlParams);
                        prm17 = Util.PairFindValue(ReportTemplate.RpExceptionSeventeenParamName, xmlParams);
                        prm18 = Util.PairFindValue(ReportTemplate.RpExceptionEighteenParamName, xmlParams);
                        prm19 = Util.PairFindValue(ReportTemplate.RpExceptionNineteenParamName, xmlParams);
                        prm20 = Util.PairFindValue(ReportTemplate.RpExceptionTwentyParamName, xmlParams);
                        prm21 = Util.PairFindValue(ReportTemplate.RpExceptionTwentyFirstParamName, xmlParams);

                        if (xmlParams == null || prm1 == null || prm2 == null || prm3 == null ||
                           prm4 == null || prm5 == null || prm6 == null || prm7 == null ||
                           prm8 == null || prm9 == null || prm10 == null || prm11 == null ||
                           prm12 == null || prm13 == null || prm14 == null || prm15 == null || prm16 == null ||
                                  prm17 == null || prm18 == null || prm19 == null || prm20 == null || prm21 == null)
                        {
                            // empty result
                            return xmlDataSet;
                        }

                        //Authorization
                        if (!dbUser.ValidateUserLicensePlateOne(userId, prm1))
                            throw new ASIAuthorizationFailedException("User: " + userId + " LicensePlate: " + prm1);
                        dsReport = boxExceptionReport.GetExceptionReport(prm1,
                           prm2,//fromDateTime,
                           prm3,//toDateTime,
                           userId,
                           Convert.ToInt16(prm4),//sosLimit,
                           Convert.ToInt32(prm5),//noDoorSnsHrs,
                           Convert.ToBoolean(prm6),//IncludeTar
                           Convert.ToBoolean(prm7),//IncludeMobilize
                           Convert.ToBoolean(prm8),//15SecDoorSns
                           Convert.ToBoolean(prm9),//50%ofLeash
                           Convert.ToBoolean(prm10),//MainAndBackupBatterySns
                           Convert.ToBoolean(prm11),//TamperSns
                           Convert.ToBoolean(prm12),//AnyPanicSns
                           Convert.ToBoolean(prm13),//ThreeKeypadAttemptsSns
                           Convert.ToBoolean(prm14),//AltGPSAntennaSns
                           Convert.ToBoolean(prm15),//ControllerStatus
                           Convert.ToBoolean(prm16),//LeashBrokenSns
                           Convert.ToBoolean(prm17),//DriverDoor
                           Convert.ToBoolean(prm18),//PassengerDoor
                           Convert.ToBoolean(prm19),//SideHopperDoor
                           Convert.ToBoolean(prm20),//RearHopperDoor
                           Convert.ToBoolean(prm21),//IncludeCurrentTar
                             Convert.ToBoolean(prm22),//Locker1
                             Convert.ToBoolean(prm23),//Locker2
                             Convert.ToBoolean(prm24),//Locker3
                             Convert.ToBoolean(prm25),//Locker4
                             Convert.ToBoolean(prm26),//Locker5
                             Convert.ToBoolean(prm27),//Locker6
                             Convert.ToBoolean(prm28),//Locker7
                             Convert.ToBoolean(prm29),//Locker8
                             Convert.ToBoolean(prm30),//Locker9

                           ref requestOverflowed,
                           ref totalSqlRecords);
                        if (dsReport != null)
                            xmlDataSet = dsReport.GetXml();
                        #endregion
                        break;
                    case ReportTemplate.ReportTypes.Latency:
                        #region Prepares Latency Report
                        VLF.DAS.Logic.Report vehicleLatency = new VLF.DAS.Logic.Report(connectionString);
                        prm1 = Util.PairFindValue(ReportTemplate.RpLatencyFirstParamName, xmlParams);
                        prm2 = Util.PairFindValue(ReportTemplate.RpLatencySecondParamName, xmlParams);
                        prm3 = Util.PairFindValue(ReportTemplate.RpLatencyThirdParamName, xmlParams);
                        prm4 = Util.PairFindValue(ReportTemplate.RpLatencyFourthParamName, xmlParams);
                        prm5 = Util.PairFindValue(ReportTemplate.RpLatencyFifthParamName, xmlParams);
                        prm6 = Util.PairFindValue(ReportTemplate.RpLatencySixthParamName, xmlParams);
                        if (xmlParams == null || prm1 == null || prm2 == null || prm3 == null || prm4 == null || prm5 == null)
                            return xmlDataSet;
                        StringReader strrXML = new StringReader(prm6);
                        DataSet dsParams = new DataSet();
                        dsParams.ReadXml(strrXML);

                        //Authorization
                        if (!dbUser.ValidateHGISuperUser(userId))
                            throw new ASIAuthorizationFailedException("User: " + userId);
                        if (Convert.ToInt64(prm3) != VLF.CLS.Def.Const.unassignedIntValue) // Vehicle Id
                            dsReport = vehicleLatency.GetVehicleLatencyReport(Convert.ToInt64(prm3), prm4, prm5, dsParams);
                        else if (Convert.ToInt32(prm2) != VLF.CLS.Def.Const.unassignedIntValue) // Fleet Id
                            dsReport = vehicleLatency.GetFleetLatencyReport(Convert.ToInt32(prm2), prm4, prm5, dsParams);
                        else // Organization Id
                            dsReport = vehicleLatency.GetOrganizationLatencyReport(Convert.ToInt32(prm1), prm4, prm5, dsParams);

                        if (dsReport != null)
                            xmlDataSet = dsReport.GetXml();
                        #endregion
                        break;
                    case ReportTemplate.ReportTypes.StopByLandmark:
                        #region Prepares Single Vehicle Stop Report By Landmark
                        VLF.DAS.Logic.Report stopReportByLandmark = new VLF.DAS.Logic.Report(connectionString);
                        prm1 = Util.PairFindValue(ReportTemplate.RpStopFirstParamName, xmlParams);
                        prm2 = Util.PairFindValue(ReportTemplate.RpStopSecondParamName, xmlParams);
                        prm3 = Util.PairFindValue(ReportTemplate.RpStopThirdParamName, xmlParams);
                        prm4 = Util.PairFindValue(ReportTemplate.RpStopFourthParamName, xmlParams);
                        prm5 = Util.PairFindValue(ReportTemplate.RpStopFifthParamName, xmlParams);

                        if (xmlParams == null || prm1 == null || prm2 == null || prm3 == null || prm4 == null || prm5 == null)
                        {
                            // empty result
                            return xmlDataSet;
                        }
                        //Authorization
                        if (!dbUser.ValidateUserLicensePlateOne(userId, prm1))
                            throw new ASIAuthorizationFailedException("User: " + userId + " LicensePlate: " + prm1);
                        dsReport = stopReportByLandmark.GetStopReportByLandmark(prm1, prm2, prm3, userId, Convert.ToBoolean(prm4), Convert.ToInt32(prm5), lang, ref requestOverflowed, ref totalSqlRecords);
                        if (dsReport != null)
                            xmlDataSet = dsReport.GetXml();
                        #endregion
                        break;
                    case ReportTemplate.ReportTypes.FleetStopByLandmark:
                        #region Prepares Fleet Stop Report By Landmark
                        VLF.DAS.Logic.Report fleetStopReportByLandmark = new VLF.DAS.Logic.Report(connectionString);
                        prm1 = Util.PairFindValue(ReportTemplate.RpFleetStopFirstParamName, xmlParams);
                        prm2 = Util.PairFindValue(ReportTemplate.RpFleetStopSecondParamName, xmlParams);
                        prm3 = Util.PairFindValue(ReportTemplate.RpFleetStopThirdParamName, xmlParams);
                        prm4 = Util.PairFindValue(ReportTemplate.RpFleetStopFourthParamName, xmlParams);
                        prm5 = Util.PairFindValue(ReportTemplate.RpFleetStopFifthParamName, xmlParams);
                        if (xmlParams == null || prm1 == null || prm2 == null || prm3 == null || prm4 == null || prm5 == null)
                        {
                            // empty result
                            return xmlDataSet;
                        }
                        //Authorization
                        //if (!dbUser.ValidateUserFleetOne(userId, Convert.ToInt32(prm1)))
                        //    throw new ASIAuthorizationFailedException("User: " + userId + " Fleet: " + prm1);
                        dsReport = fleetStopReportByLandmark.GetFleetStopReportByLandmark(Convert.ToInt32(prm1), prm2, prm3, userId, Convert.ToBoolean(prm4), Convert.ToInt32(prm5), lang, ref requestOverflowed, ref totalSqlRecords);
                        if (dsReport != null)
                            xmlDataSet = dsReport.GetXml();
                        #endregion
                        break;
                    case ReportTemplate.ReportTypes.OffHoursReport:
                        #region Prepares Single Vehicle Off Hours Report
                        VLF.DAS.Logic.Report offHoursReport = new VLF.DAS.Logic.Report(connectionString);
                        prm1 = Util.PairFindValue(ReportTemplate.RpOffHourFirstParamName, xmlParams);
                        prm2 = Util.PairFindValue(ReportTemplate.RpOffHourSecondParamName, xmlParams);
                        prm3 = Util.PairFindValue(ReportTemplate.RpOffHourThirdParamName, xmlParams);
                        prm4 = Util.PairFindValue(ReportTemplate.RpOffHourFourthParamName, xmlParams);
                        prm5 = Util.PairFindValue(ReportTemplate.RpOffHourFifthParamName, xmlParams);
                        prm6 = Util.PairFindValue(ReportTemplate.RpOffHourSixthParamName, xmlParams);
                        prm7 = Util.PairFindValue(ReportTemplate.RpOffHourSeventhParamName, xmlParams);
                        prm8 = Util.PairFindValue(ReportTemplate.RpOffHourEightParamName, xmlParams);
                        prm9 = Util.PairFindValue(ReportTemplate.RpOffHourNineParamName, xmlParams);
                        prm10 = Util.PairFindValue(ReportTemplate.RpOffHourTenParamName, xmlParams);
                        prm11 = Util.PairFindValue(ReportTemplate.RpOffHourElevenParamName, xmlParams);
                        prm12 = Util.PairFindValue(ReportTemplate.RpOffHourTwelveParamName, xmlParams);

                        if (xmlParams == null || prm1 == null || prm2 == null || prm3 == null || prm4 == null || prm5 == null || prm6 == null || prm7 == null || prm8 == null || prm9 == null || prm10 == null || prm11 == null || prm12 == null || prm13 == null)
                            return xmlDataSet;

                        //Authorization
                        if (!dbUser.ValidateUserLicensePlateOne(userId, prm1))
                            throw new ASIAuthorizationFailedException("User: " + userId + " LicensePlate: " + prm1);
                        dsReport = offHoursReport.GetOffHourReport(prm1, prm2, prm3, userId, Convert.ToBoolean(prm4), Convert.ToInt16(prm5), Convert.ToInt16(prm6), Convert.ToInt16(prm7), Convert.ToInt16(prm8), Convert.ToInt16(prm9), Convert.ToInt16(prm10), Convert.ToInt16(prm11), Convert.ToInt16(prm12), lang, ref requestOverflowed, ref totalSqlRecords, ref outMaxOverflowed, ref outMaxRecords);
                        if (dsReport != null)
                            xmlDataSet = dsReport.GetXml();
                        #endregion
                        break;
                    case ReportTemplate.ReportTypes.FleetOffHoursReport:
                        #region Prepares Single Vehicle Off Hours Report
                        VLF.DAS.Logic.Report fleetOffHoursReport = new VLF.DAS.Logic.Report(connectionString);
                        prm1 = Util.PairFindValue(ReportTemplate.RpFleetOffHourFirstParamName, xmlParams);
                        prm2 = Util.PairFindValue(ReportTemplate.RpOffHourSecondParamName, xmlParams);
                        prm3 = Util.PairFindValue(ReportTemplate.RpOffHourThirdParamName, xmlParams);
                        prm4 = Util.PairFindValue(ReportTemplate.RpOffHourFourthParamName, xmlParams);
                        prm5 = Util.PairFindValue(ReportTemplate.RpOffHourFifthParamName, xmlParams);
                        prm6 = Util.PairFindValue(ReportTemplate.RpOffHourSixthParamName, xmlParams);
                        prm7 = Util.PairFindValue(ReportTemplate.RpOffHourSeventhParamName, xmlParams);
                        prm8 = Util.PairFindValue(ReportTemplate.RpOffHourEightParamName, xmlParams);
                        prm9 = Util.PairFindValue(ReportTemplate.RpOffHourNineParamName, xmlParams);
                        prm10 = Util.PairFindValue(ReportTemplate.RpOffHourTenParamName, xmlParams);
                        prm11 = Util.PairFindValue(ReportTemplate.RpOffHourElevenParamName, xmlParams);
                        prm12 = Util.PairFindValue(ReportTemplate.RpOffHourTwelveParamName, xmlParams);

                        if (xmlParams == null || prm1 == null || prm2 == null || prm3 == null || prm4 == null || prm5 == null || prm6 == null || prm7 == null || prm8 == null || prm9 == null || prm10 == null || prm11 == null || prm12 == null || prm13 == null)
                            return xmlDataSet;

                        //Authorization
                        //if (!dbUser.ValidateUserFleetOne(userId, Convert.ToInt32(prm1)))
                        //    throw new ASIAuthorizationFailedException("User: " + userId + " Fleet: " + prm1);
                        dsReport = fleetOffHoursReport.GetFleetOffHourReport(Convert.ToInt32(prm1), prm2, prm3, userId, Convert.ToBoolean(prm4), Convert.ToInt16(prm5), Convert.ToInt16(prm6), Convert.ToInt16(prm7), Convert.ToInt16(prm8), Convert.ToInt16(prm9), Convert.ToInt16(prm10), Convert.ToInt16(prm11), Convert.ToInt16(prm12), lang, ref requestOverflowed, ref totalSqlRecords, ref outMaxOverflowed, ref outMaxRecords);
                        if (dsReport != null)
                            xmlDataSet = dsReport.GetXml();
                        #endregion
                        break;
                    case ReportTemplate.ReportTypes.FleetException:
                        #region Prepares Exception Report
                        boxExceptionReport = new VLF.DAS.Logic.Report(connectionString);
                        prm1 = Util.PairFindValue(ReportTemplate.RpFleetTripFirstParamName, xmlParams);
                        prm2 = Util.PairFindValue(ReportTemplate.RpExceptionSecondParamName, xmlParams);
                        prm3 = Util.PairFindValue(ReportTemplate.RpExceptionThirdParamName, xmlParams);
                        prm4 = Util.PairFindValue(ReportTemplate.RpExceptionFourthParamName, xmlParams);
                        prm5 = Util.PairFindValue(ReportTemplate.RpExceptionFifthParamName, xmlParams);
                        prm6 = Util.PairFindValue(ReportTemplate.RpExceptionSixthParamName, xmlParams);
                        prm7 = Util.PairFindValue(ReportTemplate.RpExceptionSeventhParamName, xmlParams);
                        prm8 = Util.PairFindValue(ReportTemplate.RpExceptionEightParamName, xmlParams);
                        prm9 = Util.PairFindValue(ReportTemplate.RpExceptionNineParamName, xmlParams);
                        prm10 = Util.PairFindValue(ReportTemplate.RpExceptionTenParamName, xmlParams);
                        prm11 = Util.PairFindValue(ReportTemplate.RpExceptionElevenParamName, xmlParams);
                        prm12 = Util.PairFindValue(ReportTemplate.RpExceptionTwelveParamName, xmlParams);
                        prm13 = Util.PairFindValue(ReportTemplate.RpExceptionThirteenParamName, xmlParams);
                        prm14 = Util.PairFindValue(ReportTemplate.RpExceptionFourteenParamName, xmlParams);
                        prm15 = Util.PairFindValue(ReportTemplate.RpExceptionFifteenParamName, xmlParams);
                        prm16 = Util.PairFindValue(ReportTemplate.RpExceptionSixteenParamName, xmlParams);
                        prm17 = Util.PairFindValue(ReportTemplate.RpExceptionSeventeenParamName, xmlParams);
                        prm18 = Util.PairFindValue(ReportTemplate.RpExceptionEighteenParamName, xmlParams);
                        prm19 = Util.PairFindValue(ReportTemplate.RpExceptionNineteenParamName, xmlParams);
                        prm20 = Util.PairFindValue(ReportTemplate.RpExceptionTwentyParamName, xmlParams);
                        prm21 = Util.PairFindValue(ReportTemplate.RpExceptionTwentyFirstParamName, xmlParams);
                        if (xmlParams == null || prm1 == null || prm2 == null || prm3 == null ||
                     prm4 == null || prm5 == null || prm6 == null || prm7 == null ||
                     prm8 == null || prm9 == null || prm10 == null || prm11 == null ||
                     prm12 == null || prm13 == null || prm14 == null || prm15 == null || prm16 == null ||
                     prm17 == null || prm18 == null || prm19 == null || prm20 == null || prm21 == null)
                        {
                            // empty result
                            return xmlDataSet;
                        }

                        //Authorization
                        //if (!dbUser.ValidateUserFleetOne(userId, Convert.ToInt32(prm1)))
                        //    throw new ASIAuthorizationFailedException("User: " + userId + " Fleet: " + prm1);
                        dsReport = boxExceptionReport.GetFleetExceptionReport(Convert.ToInt32(prm1),
                           prm2,//fromDateTime,
                           prm3,//toDateTime,
                           userId,
                           Convert.ToInt16(prm4),//sosLimit,
                           Convert.ToInt32(prm5),//noDoorSnsHrs,
                           Convert.ToBoolean(prm6),//IncludeTar
                           Convert.ToBoolean(prm7),//IncludeMobilize
                           Convert.ToBoolean(prm8),//15SecDoorSns
                           Convert.ToBoolean(prm9),//50%ofLeash
                           Convert.ToBoolean(prm10),//MainAndBackupBatterySns
                           Convert.ToBoolean(prm11),//TamperSns
                           Convert.ToBoolean(prm12),//AnyPanicSns
                           Convert.ToBoolean(prm13),//ThreeKeypadAttemptsSns
                           Convert.ToBoolean(prm14),//AltGPSAntennaSns
                           Convert.ToBoolean(prm15),//ControllerStatus
                           Convert.ToBoolean(prm16),//LeashBrokenSns
                           Convert.ToBoolean(prm17),//DriverDoor
                           Convert.ToBoolean(prm18),//PassengerDoor
                           Convert.ToBoolean(prm19),//SideHopperDoor
                           Convert.ToBoolean(prm20),//RearHopperDoor
                           Convert.ToBoolean(prm21),//IncludeCurrentTar
                           Convert.ToBoolean(prm22),//Locker1
                         Convert.ToBoolean(prm23),//Locker2
                         Convert.ToBoolean(prm24),//Locker3
                         Convert.ToBoolean(prm25),//Locker4
                         Convert.ToBoolean(prm26),//Locker5
                         Convert.ToBoolean(prm27),//Locker6
                         Convert.ToBoolean(prm28),//Locker7
                         Convert.ToBoolean(prm29),//Locker8
                         Convert.ToBoolean(prm30),//Locker9

                                  ref requestOverflowed,
                           ref totalSqlRecords);
                        if (dsReport != null)
                            xmlDataSet = dsReport.GetXml();
                        #endregion
                        break;
                    case ReportTemplate.ReportTypes.FleetMaintenanceReport:
                        #region Prepares Fleet Maintenance Report
                        boxExceptionReport = new VLF.DAS.Logic.Report(connectionString);
                        prm1 = Util.PairFindValue(ReportTemplate.RpFleetMaintenanceFirstParamName, xmlParams);
                        if (xmlParams == null || prm1 == null)
                        {
                            // empty result
                            return xmlDataSet;
                        }

                        //Authorization
                        //if (!dbUser.ValidateUserFleetOne(userId, Convert.ToInt32(prm1)))
                        //    throw new ASIAuthorizationFailedException("User: " + userId + " Fleet: " + prm1);
                        dbFleet = new VLF.DAS.Logic.Fleet(connectionString);

                        dsReport = dbFleet.GetFleetMaintenanceHistory(userId, Convert.ToInt32(prm1));
                        if (dsReport != null)
                            xmlDataSet = dsReport.GetXml();
                        #endregion
                        break;
                }
            }
            finally
            {
                if (dbUser != null)
                    dbUser.Dispose();
                if (dbFleet != null)
                    dbFleet.Dispose();
            }
            return xmlDataSet;
        }
        // Changes for TimeZone Feature end
        /// <summary>
        /// Get strong-type dataset as XML.
        /// </summary>
        /// <param name="repType"></param>
        /// <param name="xmlParams"></param>
        /// <param name="userId"></param>
        /// <param name="requestOverflowed"></param>
        /// <param name="totalSqlRecords"></param>
        /// <param name="outMaxOverflowed"></param>
        /// <param name="outMaxRecords"></param>
        /// <returns>XML</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public string GetXml(ReportTemplate.ReportTypes repType, string xmlParams, int userId, string lang,
                       ref bool requestOverflowed, ref int totalSqlRecords,
                       ref bool outMaxOverflowed, ref int outMaxRecords)
        {
            VLF.DAS.Logic.User dbUser = null;
            VLF.DAS.Logic.Fleet dbFleet = null;
            string xmlDataSet = "";
            try
            {
                dbUser = new VLF.DAS.Logic.User(connectionString);
                bool showExceptionOnly = false;
                string prm1 = "";
                string prm2 = "";
                string prm3 = "";
                string prm4 = "";
                string prm5 = "";
                string prm6 = "";
                string prm7 = "";
                string prm8 = "";
                string prm9 = "";
                string prm10 = "";
                string prm11 = "";
                string prm12 = "";
                string prm13 = "";
                string prm14 = "";
                string prm15 = "";
                string prm16 = "";
                string prm17 = "";
                string prm18 = "";
                string prm19 = "";
                string prm20 = "";
                string prm21 = "";
                string prm22 = "";
                string prm23 = "";
                string prm24 = "";
                string prm25 = "";
                string prm26 = "";
                string prm27 = "";
                string prm28 = "";
                string prm29 = "";
                string prm30 = "";

                DataSet dsReport = null;
                requestOverflowed = false;
                totalSqlRecords = 0;
                outMaxOverflowed = false;
                outMaxRecords = 0;

                switch (repType)
                {
                    case ReportTemplate.ReportTypes.DetailedTrip:
                        #region Prepares Detailed Trip Report
                        VLF.DAS.Logic.Report detailedTrip = new VLF.DAS.Logic.Report(connectionString);
                        prm1 = Util.PairFindValue(ReportTemplate.RpDetailedTripFirstParamName, xmlParams);
                        prm2 = Util.PairFindValue(ReportTemplate.RpDetailedTripSecondParamName, xmlParams);
                        prm3 = Util.PairFindValue(ReportTemplate.RpDetailedTripThirdParamName, xmlParams);
                        prm4 = Util.PairFindValue(ReportTemplate.RpDetailedTripFourthParamName, xmlParams);
                        prm5 = Util.PairFindValue(ReportTemplate.RpDetailedTripFifthParamName, xmlParams);
                        prm6 = Util.PairFindValue(ReportTemplate.RpDetailedTripSixthParamName, xmlParams);
                        prm7 = Util.PairFindValue(ReportTemplate.RpDetailedTripSeventhParamName, xmlParams);
                        prm8 = Util.PairFindValue(ReportTemplate.RpDetailedTripEighthParamName, xmlParams);
                        prm9 = Util.PairFindValue(ReportTemplate.RpDetailedTripNinthParamName, xmlParams);
                        prm10 = Util.PairFindValue(ReportTemplate.RpDetailedTripTenthParamName, xmlParams);

                        if (xmlParams == null || prm1 == null || prm2 == null || prm3 == null || prm4 == null ||
                           prm5 == null || prm6 == null || prm7 == null || prm8 == null || prm9 == null || prm10 == null)
                        {
                            // empty result
                            return xmlDataSet;
                        }
                        //Authorization
                        if (!dbUser.ValidateUserLicensePlateOne(userId, prm1))
                            throw new ASIAuthorizationFailedException("User: " + userId + " LicensePlate: " + prm1);
                        dsReport = detailedTrip.GetDetailedTripReport(prm1, prm2, prm3,
                           Convert.ToBoolean(prm4),
                           Convert.ToBoolean(prm5),
                           Convert.ToBoolean(prm6),
                           Convert.ToBoolean(prm7),
                           Convert.ToBoolean(prm8),
                           Convert.ToBoolean(prm9),
                           userId, Convert.ToInt32(prm10), lang,
                           ref requestOverflowed,
                           ref totalSqlRecords,
                           ref outMaxOverflowed,
                           ref outMaxRecords);
                        if (dsReport != null)
                            xmlDataSet = dsReport.GetXml();
                        #endregion
                        break;
                    case ReportTemplate.ReportTypes.Trip:
                        #region Prepares Single Vehicle Trip Report
                        VLF.DAS.Logic.Report tripReport = new VLF.DAS.Logic.Report(connectionString);
                        prm1 = Util.PairFindValue(ReportTemplate.RpTripFirstParamName, xmlParams);
                        prm2 = Util.PairFindValue(ReportTemplate.RpTripSecondParamName, xmlParams);
                        prm3 = Util.PairFindValue(ReportTemplate.RpTripThirdParamName, xmlParams);
                        prm4 = Util.PairFindValue(ReportTemplate.RpTripFourthParamName, xmlParams);
                        prm5 = Util.PairFindValue(ReportTemplate.RpTripFifthParamName, xmlParams);
                        if ((xmlParams == null) || (prm1 == null) || (prm2 == null) || (prm3 == null) || (prm4 == null) || (prm5 == null))
                        {
                            // empty result
                            return xmlDataSet;
                        }
                        //Authorization
                        if (!dbUser.ValidateUserLicensePlateOne(userId, prm1))
                            throw new ASIAuthorizationFailedException("User: " + userId + " LicensePlate: " + prm1);
                        dsReport = tripReport.GetTripReport(prm1, prm2, prm3, userId, Convert.ToBoolean(prm4), Convert.ToInt32(prm5), lang,
                           ref requestOverflowed, ref totalSqlRecords,
                           ref outMaxOverflowed, ref outMaxRecords);
                        if (dsReport != null)
                            xmlDataSet = dsReport.GetXml();
                        #endregion
                        break;
                    case ReportTemplate.ReportTypes.Stop:
                        #region Prepares Single Vehicle Stop Report
                        VLF.DAS.Logic.Report stopReport = new VLF.DAS.Logic.Report(connectionString);
                        prm1 = Util.PairFindValue(ReportTemplate.RpStopFirstParamName, xmlParams);
                        prm2 = Util.PairFindValue(ReportTemplate.RpStopSecondParamName, xmlParams);
                        prm3 = Util.PairFindValue(ReportTemplate.RpStopThirdParamName, xmlParams);
                        prm4 = Util.PairFindValue(ReportTemplate.RpStopFourthParamName, xmlParams);
                        prm5 = Util.PairFindValue(ReportTemplate.RpStopFifthParamName, xmlParams);
                        prm6 = Util.PairFindValue(ReportTemplate.RpStopSixthParamName, xmlParams);
                        prm7 = Util.PairFindValue(ReportTemplate.RpStopSeventhParamName, xmlParams);
                        prm8 = Util.PairFindValue(ReportTemplate.RpStopEighthParamName, xmlParams);

                        if (xmlParams == null || prm1 == null || prm2 == null || prm3 == null || prm4 == null || prm5 == null || prm6 == null || prm7 == null || prm8 == null)
                        {
                            // empty result
                            return xmlDataSet;
                        }
                        //Authorization
                        if (!dbUser.ValidateUserLicensePlateOne(userId, prm1))
                            throw new ASIAuthorizationFailedException("User: " + userId + " LicensePlate: " + prm1);
                        if (Convert.ToBoolean(prm6) == false && Convert.ToBoolean(prm7) == false)
                            throw new ASIDataNotFoundException("Wrong filter values. Include at least one of following options either Stop or Idling.");
                        dsReport = stopReport.GetStopReport(prm1, prm2, prm3, userId, Convert.ToBoolean(prm4), Convert.ToInt32(prm5), Convert.ToBoolean(prm6), Convert.ToBoolean(prm7), Convert.ToInt32(prm8), lang, ref requestOverflowed, ref totalSqlRecords);
                        if (dsReport != null)
                            xmlDataSet = dsReport.GetXml();
                        #endregion
                        break;
                    case ReportTemplate.ReportTypes.TripActivity:
                        #region Prepares Vehicle Trip Activity Report
                        VLF.DAS.Logic.Report tripActivityReport = new VLF.DAS.Logic.Report(connectionString);
                        prm1 = Util.PairFindValue(ReportTemplate.RpTripFirstParamName, xmlParams);
                        prm2 = Util.PairFindValue(ReportTemplate.RpTripSecondParamName, xmlParams);
                        prm3 = Util.PairFindValue(ReportTemplate.RpTripThirdParamName, xmlParams);
                        prm4 = Util.PairFindValue(ReportTemplate.RpTripFourthParamName, xmlParams);
                        prm5 = Util.PairFindValue(ReportTemplate.RpTripFifthParamName, xmlParams);
                        if ((xmlParams == null) || (prm1 == null) || (prm2 == null) || (prm3 == null) || (prm4 == null) || (prm5 == null))
                        {
                            // empty result
                            return xmlDataSet;
                        }
                        //Authorization
                        if (!dbUser.ValidateUserLicensePlateOne(userId, prm1))
                            throw new ASIAuthorizationFailedException("User: " + userId + " LicensePlate: " + prm1);
                        dsReport = tripActivityReport.GetTripActivityReport(prm1, prm2, prm3, userId, Convert.ToBoolean(prm4), Convert.ToInt32(prm5), lang, ref requestOverflowed, ref totalSqlRecords);
                        if (dsReport != null)
                            xmlDataSet = dsReport.GetXml();
                        #endregion
                        break;
                    case ReportTemplate.ReportTypes.FleetDetailedTrip:
                        #region Prepares Fleet Detailed Trip Report
                        VLF.DAS.Logic.Report fleetDetailedTrip = new VLF.DAS.Logic.Report(connectionString);
                        prm1 = Util.PairFindValue(ReportTemplate.RpFleetDetailedTripFirstParamName, xmlParams);
                        prm2 = Util.PairFindValue(ReportTemplate.RpFleetDetailedTripSecondParamName, xmlParams);
                        prm3 = Util.PairFindValue(ReportTemplate.RpFleetDetailedTripThirdParamName, xmlParams);
                        prm4 = Util.PairFindValue(ReportTemplate.RpFleetDetailedTripFourthParamName, xmlParams);
                        prm5 = Util.PairFindValue(ReportTemplate.RpFleetDetailedTripFifthParamName, xmlParams);
                        prm6 = Util.PairFindValue(ReportTemplate.RpFleetDetailedTripSixthParamName, xmlParams);
                        prm7 = Util.PairFindValue(ReportTemplate.RpFleetDetailedTripSeventhParamName, xmlParams);
                        prm8 = Util.PairFindValue(ReportTemplate.RpFleetDetailedTripEighthParamName, xmlParams);
                        prm9 = Util.PairFindValue(ReportTemplate.RpFleetDetailedTripNinthParamName, xmlParams);
                        prm10 = Util.PairFindValue(ReportTemplate.RpFleetDetailedTripTenthParamName, xmlParams);

                        if ((xmlParams == null) || (prm1 == null) || (prm2 == null) || (prm3 == null) ||
                           (prm4 == null) || (prm5 == null) || (prm6 == null) || (prm7 == null) || (prm8 == null) || (prm9 == null) || (prm10 == null))
                        {
                            // empty result
                            return xmlDataSet;
                        }
                        //Authorization
                        //if (!dbUser.ValidateUserFleetOne(userId, Convert.ToInt32(prm1)))
                        //    throw new ASIAuthorizationFailedException("User: " + userId + " Fleet: " + prm1);
                        dsReport = fleetDetailedTrip.GetFleetDetailedTripReport(
                           Convert.ToInt16(prm1),
                           prm2,
                           prm3,
                           Convert.ToBoolean(prm4),
                           Convert.ToBoolean(prm5),
                           Convert.ToBoolean(prm6),
                           Convert.ToBoolean(prm7),
                           Convert.ToBoolean(prm8),
                           Convert.ToBoolean(prm9),
                           userId, Convert.ToInt32(prm10), lang,
                           ref requestOverflowed,
                           ref totalSqlRecords,
                           ref outMaxOverflowed,
                           ref outMaxRecords);
                        if (dsReport != null)
                            xmlDataSet = dsReport.GetXml();
                        #endregion
                        break;
                    case ReportTemplate.ReportTypes.FleetTrip:
                        #region Prepares Fleet Trip Report
                        VLF.DAS.Logic.Report fleetTripReport = new VLF.DAS.Logic.Report(connectionString);
                        prm1 = Util.PairFindValue(ReportTemplate.RpFleetTripFirstParamName, xmlParams);
                        prm2 = Util.PairFindValue(ReportTemplate.RpFleetTripSecondParamName, xmlParams);
                        prm3 = Util.PairFindValue(ReportTemplate.RpFleetTripThirdParamName, xmlParams);
                        prm4 = Util.PairFindValue(ReportTemplate.RpFleetTripFourthParamName, xmlParams);
                        prm5 = Util.PairFindValue(ReportTemplate.RpFleetTripFifthParamName, xmlParams);
                        if ((xmlParams == null) || (prm1 == null) || (prm2 == null) || (prm3 == null) || (prm4 == null) || (prm5 == null))
                        {
                            // empty result
                            return xmlDataSet;
                        }
                        //Authorization
                        //if (!dbUser.ValidateUserFleetOne(userId, Convert.ToInt32(prm1)))
                        //    throw new ASIAuthorizationFailedException("User: " + userId + " Fleet: " + prm1);
                        dsReport = fleetTripReport.GetFleetTripReport(Convert.ToInt32(prm1),
                           prm2,
                           prm3,
                           userId,
                           Convert.ToBoolean(prm4), Convert.ToInt32(prm5), lang,
                           ref requestOverflowed,
                           ref totalSqlRecords,
                           ref outMaxOverflowed,
                           ref outMaxRecords);
                        if (dsReport != null)
                            xmlDataSet = dsReport.GetXml();
                        #endregion
                        break;
                    case ReportTemplate.ReportTypes.FleetStop:
                        #region Prepares Fleet Stop Report
                        VLF.DAS.Logic.Report fleetStopReport = new VLF.DAS.Logic.Report(connectionString);
                        prm1 = Util.PairFindValue(ReportTemplate.RpFleetStopFirstParamName, xmlParams);
                        prm2 = Util.PairFindValue(ReportTemplate.RpFleetStopSecondParamName, xmlParams);
                        prm3 = Util.PairFindValue(ReportTemplate.RpFleetStopThirdParamName, xmlParams);
                        prm4 = Util.PairFindValue(ReportTemplate.RpFleetStopFourthParamName, xmlParams);
                        prm5 = Util.PairFindValue(ReportTemplate.RpFleetStopFifthParamName, xmlParams);
                        prm6 = Util.PairFindValue(ReportTemplate.RpFleetStopSixthParamName, xmlParams);
                        prm7 = Util.PairFindValue(ReportTemplate.RpFleetStopSeventhParamName, xmlParams);
                        prm8 = Util.PairFindValue(ReportTemplate.RpFleetStopEighthParamName, xmlParams);
                        if (xmlParams == null || prm1 == null || prm2 == null || prm3 == null || prm4 == null || prm5 == null || prm6 == null || prm7 == null || prm8 == null)
                        {
                            // empty result
                            return xmlDataSet;
                        }
                        //Authorization
                        //if (!dbUser.ValidateUserFleetOne(userId, Convert.ToInt32(prm1)))
                        //    throw new ASIAuthorizationFailedException("User: " + userId + " Fleet: " + prm1);
                        if (Convert.ToBoolean(prm6) == false && Convert.ToBoolean(prm7) == false)
                            throw new ASIDataNotFoundException("Wrong filter values. Include at least one of following options either Stop or Idling.");
                        dsReport = fleetStopReport.GetFleetStopReport(Convert.ToInt32(prm1),
                     prm2,
                     prm3,
                     userId,
                     Convert.ToBoolean(prm4),
                     Convert.ToInt32(prm5),
                            Convert.ToBoolean(prm6),
                            Convert.ToBoolean(prm7), Convert.ToInt32(prm8), lang,
                     ref requestOverflowed,
                     ref totalSqlRecords);
                        if (dsReport != null)
                            xmlDataSet = dsReport.GetXml();
                        #endregion
                        break;
                    case ReportTemplate.ReportTypes.Alarm:
                        #region Prepares Alarm Report
                        VLF.DAS.Logic.Report alarmReport = new VLF.DAS.Logic.Report(connectionString);
                        prm1 = Util.PairFindValue(ReportTemplate.RpAlarmFirstParamName, xmlParams);
                        prm2 = Util.PairFindValue(ReportTemplate.RpAlarmSecondParamName, xmlParams);
                        prm3 = Util.PairFindValue(ReportTemplate.RpAlarmThirdParamName, xmlParams);
                        if ((xmlParams == null) || (prm1 == null) || (prm2 == null) || (prm3 == null))
                        {
                            // empty result
                            return xmlDataSet;
                        }
                        //Authorization
                        if (!dbUser.ValidateUserLicensePlateOne(userId, prm1))
                            throw new ASIAuthorizationFailedException("User: " + userId + " LicensePlate: " + prm1);
                        dsReport = alarmReport.GetAlarmReport(userId, prm1, prm2, prm3, ref requestOverflowed, ref totalSqlRecords);

                        if (dsReport != null)
                        {
                            if (lang != "en" && lang != null)
                            {
                                LocalizationLayer.ServerLocalizationLayer dbl = new LocalizationLayer.ServerLocalizationLayer(connectionString);
                                dbl.LocalizationData(lang, "AlarmSeverity", "AlarmSeverityName", "AlarmSeverity", ref dsReport);
                            }

                            xmlDataSet = dsReport.GetXml();
                        }

                        #endregion
                        break;
                    case ReportTemplate.ReportTypes.FleetAlarms:
                        #region Prepares Fleet Alarms Report
                        VLF.DAS.Logic.Report fleetAlamsReport = new VLF.DAS.Logic.Report(connectionString);
                        prm1 = Util.PairFindValue(ReportTemplate.RpFleetAlarmsFirstParamName, xmlParams);
                        prm2 = Util.PairFindValue(ReportTemplate.RpFleetAlarmsSecondParamName, xmlParams);
                        prm3 = Util.PairFindValue(ReportTemplate.RpFleetAlarmsThirdParamName, xmlParams);
                        if ((xmlParams == null) || (prm1 == null) || (prm2 == null) || (prm3 == null))
                        {
                            // empty result
                            return xmlDataSet;
                        }
                        //Authorization
                        //if (!dbUser.ValidateUserFleetOne(userId, Convert.ToInt32(prm1)))
                        //    throw new ASIAuthorizationFailedException("User: " + userId + " Fleet: " + prm1);
                        dsReport = fleetAlamsReport.GetFleetAlarmsReport(userId, Convert.ToInt32(prm1), prm2, prm3, ref requestOverflowed, ref totalSqlRecords);




                        if (dsReport != null)
                        {
                            if (lang != "en" && lang != null)
                            {
                                LocalizationLayer.ServerLocalizationLayer dbl = new LocalizationLayer.ServerLocalizationLayer(connectionString);
                                dbl.LocalizationData(lang, "AlarmSeverity", "AlarmSeverityName", "AlarmSeverity", ref dsReport);
                            }

                            xmlDataSet = dsReport.GetXml();
                        }
                        #endregion
                        break;
                    case ReportTemplate.ReportTypes.SystemUsageExceptionReportForAllOrganizations:
                        #region Prepares System Usage Exception Report For All Organizations
                        VLF.DAS.Logic.Report systemUsageExceptionReport = new VLF.DAS.Logic.Report(connectionString);
                        prm1 = Util.PairFindValue(ReportTemplate.RpSystemUsageExceptionReportForAllOrganizationsFirstParamName, xmlParams);
                        prm2 = Util.PairFindValue(ReportTemplate.RpSystemUsageExceptionReportForAllOrganizationsSecondParamName, xmlParams);
                        if (xmlParams == null || prm1 == null || prm2 == null)
                        {
                            // empty result
                            return xmlDataSet;
                        }
                        //Authorization
                        if (!dbUser.ValidateHGISuperUser(userId))
                            throw new ASIAuthorizationFailedException("User: " + userId);
                        dsReport = systemUsageExceptionReport.GetSystemUsageExceptionReportForAllOrganizations(prm1, prm2);
                        if (dsReport != null)
                            xmlDataSet = dsReport.GetXml();
                        #endregion
                        break;
                    case ReportTemplate.ReportTypes.OrganizationSystemUsage:
                        #region Prepares Organization System Usage Report
                        VLF.DAS.Logic.Report orgSystemUsageReport = new VLF.DAS.Logic.Report(connectionString);
                        prm1 = Util.PairFindValue(ReportTemplate.RpOrganizationSystemUsageFirstParamName, xmlParams);
                        prm2 = Util.PairFindValue(ReportTemplate.RpOrganizationSystemUsageSecondParamName, xmlParams);
                        prm3 = Util.PairFindValue(ReportTemplate.RpOrganizationSystemUsageThirdParamName, xmlParams);
                        if ((xmlParams == null) || (prm1 == null) || (prm2 == null) || (prm3 == null))
                        {
                            // empty result
                            return xmlDataSet;
                        }
                        prm4 = Util.PairFindValue(ReportTemplate.RpOrganizationSystemUsageFourthParamName, xmlParams);
                        if (prm4 != null)
                            showExceptionOnly = Convert.ToBoolean(prm4);

                        //Authorization
                        if (!dbUser.ValidateHGISuperUser(userId))
                            throw new ASIAuthorizationFailedException("User: " + userId);
                        dsReport = orgSystemUsageReport.GetSystemUsageReportByOrganization(Convert.ToInt32(prm1), prm2, prm3, showExceptionOnly);
                        if (dsReport != null)
                            xmlDataSet = dsReport.GetXml();
                        #endregion
                        break;
                    case ReportTemplate.ReportTypes.BoxSystemUsage:
                        #region Prepares Box System Usage Report
                        VLF.DAS.Logic.Report boxSystemUsageReport = new VLF.DAS.Logic.Report(connectionString);
                        prm1 = Util.PairFindValue(ReportTemplate.RpBoxSystemUsageFirstParamName, xmlParams);
                        prm2 = Util.PairFindValue(ReportTemplate.RpBoxSystemUsageSecondParamName, xmlParams);
                        prm3 = Util.PairFindValue(ReportTemplate.RpBoxSystemUsageThirdParamName, xmlParams);
                        if ((xmlParams == null) || (prm1 == null) || (prm2 == null) || (prm3 == null))
                        {
                            // empty result
                            return xmlDataSet;
                        }
                        prm4 = Util.PairFindValue(ReportTemplate.RpBoxSystemUsageFourthParamName, xmlParams);
                        if (prm4 != null)
                            showExceptionOnly = Convert.ToBoolean(prm4);

                        //Authorization
                        //if (!dbUser.ValidateHGISuperUser(userId))
                        //throw new ASIAuthorizationFailedException("User: " + userId);
                        dsReport = boxSystemUsageReport.GetSystemUsageReportByBox(Convert.ToInt32(prm1), prm2, prm3, showExceptionOnly);
                        if (dsReport != null)
                            xmlDataSet = dsReport.GetXml();
                        #endregion
                        break;
                    case ReportTemplate.ReportTypes.Exception:
                        #region Prepares Exception Report
                        VLF.DAS.Logic.Report boxExceptionReport = new VLF.DAS.Logic.Report(connectionString);
                        prm1 = Util.PairFindValue(ReportTemplate.RpExceptionFirstParamName, xmlParams);
                        prm2 = Util.PairFindValue(ReportTemplate.RpExceptionSecondParamName, xmlParams);
                        prm3 = Util.PairFindValue(ReportTemplate.RpExceptionThirdParamName, xmlParams);
                        prm4 = Util.PairFindValue(ReportTemplate.RpExceptionFourthParamName, xmlParams);
                        prm5 = Util.PairFindValue(ReportTemplate.RpExceptionFifthParamName, xmlParams);
                        prm6 = Util.PairFindValue(ReportTemplate.RpExceptionSixthParamName, xmlParams);
                        prm7 = Util.PairFindValue(ReportTemplate.RpExceptionSeventhParamName, xmlParams);
                        prm8 = Util.PairFindValue(ReportTemplate.RpExceptionEightParamName, xmlParams);
                        prm9 = Util.PairFindValue(ReportTemplate.RpExceptionNineParamName, xmlParams);
                        prm10 = Util.PairFindValue(ReportTemplate.RpExceptionTenParamName, xmlParams);
                        prm11 = Util.PairFindValue(ReportTemplate.RpExceptionElevenParamName, xmlParams);
                        prm12 = Util.PairFindValue(ReportTemplate.RpExceptionTwelveParamName, xmlParams);
                        prm13 = Util.PairFindValue(ReportTemplate.RpExceptionThirteenParamName, xmlParams);
                        prm14 = Util.PairFindValue(ReportTemplate.RpExceptionFourteenParamName, xmlParams);
                        prm15 = Util.PairFindValue(ReportTemplate.RpExceptionFifteenParamName, xmlParams);
                        prm16 = Util.PairFindValue(ReportTemplate.RpExceptionSixteenParamName, xmlParams);
                        prm17 = Util.PairFindValue(ReportTemplate.RpExceptionSeventeenParamName, xmlParams);
                        prm18 = Util.PairFindValue(ReportTemplate.RpExceptionEighteenParamName, xmlParams);
                        prm19 = Util.PairFindValue(ReportTemplate.RpExceptionNineteenParamName, xmlParams);
                        prm20 = Util.PairFindValue(ReportTemplate.RpExceptionTwentyParamName, xmlParams);
                        prm21 = Util.PairFindValue(ReportTemplate.RpExceptionTwentyFirstParamName, xmlParams);

                        if (xmlParams == null || prm1 == null || prm2 == null || prm3 == null ||
                           prm4 == null || prm5 == null || prm6 == null || prm7 == null ||
                           prm8 == null || prm9 == null || prm10 == null || prm11 == null ||
                           prm12 == null || prm13 == null || prm14 == null || prm15 == null || prm16 == null ||
                                  prm17 == null || prm18 == null || prm19 == null || prm20 == null || prm21 == null)
                        {
                            // empty result
                            return xmlDataSet;
                        }

                        //Authorization
                        if (!dbUser.ValidateUserLicensePlateOne(userId, prm1))
                            throw new ASIAuthorizationFailedException("User: " + userId + " LicensePlate: " + prm1);
                        dsReport = boxExceptionReport.GetExceptionReport(prm1,
                           prm2,//fromDateTime,
                           prm3,//toDateTime,
                           userId,
                           Convert.ToInt16(prm4),//sosLimit,
                           Convert.ToInt32(prm5),//noDoorSnsHrs,
                           Convert.ToBoolean(prm6),//IncludeTar
                           Convert.ToBoolean(prm7),//IncludeMobilize
                           Convert.ToBoolean(prm8),//15SecDoorSns
                           Convert.ToBoolean(prm9),//50%ofLeash
                           Convert.ToBoolean(prm10),//MainAndBackupBatterySns
                           Convert.ToBoolean(prm11),//TamperSns
                           Convert.ToBoolean(prm12),//AnyPanicSns
                           Convert.ToBoolean(prm13),//ThreeKeypadAttemptsSns
                           Convert.ToBoolean(prm14),//AltGPSAntennaSns
                           Convert.ToBoolean(prm15),//ControllerStatus
                           Convert.ToBoolean(prm16),//LeashBrokenSns
                           Convert.ToBoolean(prm17),//DriverDoor
                           Convert.ToBoolean(prm18),//PassengerDoor
                           Convert.ToBoolean(prm19),//SideHopperDoor
                           Convert.ToBoolean(prm20),//RearHopperDoor
                           Convert.ToBoolean(prm21),//IncludeCurrentTar
                             Convert.ToBoolean(prm22),//Locker1
                             Convert.ToBoolean(prm23),//Locker2
                             Convert.ToBoolean(prm24),//Locker3
                             Convert.ToBoolean(prm25),//Locker4
                             Convert.ToBoolean(prm26),//Locker5
                             Convert.ToBoolean(prm27),//Locker6
                             Convert.ToBoolean(prm28),//Locker7
                             Convert.ToBoolean(prm29),//Locker8
                             Convert.ToBoolean(prm30),//Locker9

                           ref requestOverflowed,
                           ref totalSqlRecords);
                        if (dsReport != null)
                            xmlDataSet = dsReport.GetXml();
                        #endregion
                        break;
                    case ReportTemplate.ReportTypes.Latency:
                        #region Prepares Latency Report
                        VLF.DAS.Logic.Report vehicleLatency = new VLF.DAS.Logic.Report(connectionString);
                        prm1 = Util.PairFindValue(ReportTemplate.RpLatencyFirstParamName, xmlParams);
                        prm2 = Util.PairFindValue(ReportTemplate.RpLatencySecondParamName, xmlParams);
                        prm3 = Util.PairFindValue(ReportTemplate.RpLatencyThirdParamName, xmlParams);
                        prm4 = Util.PairFindValue(ReportTemplate.RpLatencyFourthParamName, xmlParams);
                        prm5 = Util.PairFindValue(ReportTemplate.RpLatencyFifthParamName, xmlParams);
                        prm6 = Util.PairFindValue(ReportTemplate.RpLatencySixthParamName, xmlParams);
                        if (xmlParams == null || prm1 == null || prm2 == null || prm3 == null || prm4 == null || prm5 == null)
                            return xmlDataSet;
                        StringReader strrXML = new StringReader(prm6);
                        DataSet dsParams = new DataSet();
                        dsParams.ReadXml(strrXML);

                        //Authorization
                        if (!dbUser.ValidateHGISuperUser(userId))
                            throw new ASIAuthorizationFailedException("User: " + userId);
                        if (Convert.ToInt64(prm3) != VLF.CLS.Def.Const.unassignedIntValue) // Vehicle Id
                            dsReport = vehicleLatency.GetVehicleLatencyReport(Convert.ToInt64(prm3), prm4, prm5, dsParams);
                        else if (Convert.ToInt32(prm2) != VLF.CLS.Def.Const.unassignedIntValue) // Fleet Id
                            dsReport = vehicleLatency.GetFleetLatencyReport(Convert.ToInt32(prm2), prm4, prm5, dsParams);
                        else // Organization Id
                            dsReport = vehicleLatency.GetOrganizationLatencyReport(Convert.ToInt32(prm1), prm4, prm5, dsParams);

                        if (dsReport != null)
                            xmlDataSet = dsReport.GetXml();
                        #endregion
                        break;
                    case ReportTemplate.ReportTypes.StopByLandmark:
                        #region Prepares Single Vehicle Stop Report By Landmark
                        VLF.DAS.Logic.Report stopReportByLandmark = new VLF.DAS.Logic.Report(connectionString);
                        prm1 = Util.PairFindValue(ReportTemplate.RpStopFirstParamName, xmlParams);
                        prm2 = Util.PairFindValue(ReportTemplate.RpStopSecondParamName, xmlParams);
                        prm3 = Util.PairFindValue(ReportTemplate.RpStopThirdParamName, xmlParams);
                        prm4 = Util.PairFindValue(ReportTemplate.RpStopFourthParamName, xmlParams);
                        prm5 = Util.PairFindValue(ReportTemplate.RpStopFifthParamName, xmlParams);

                        if (xmlParams == null || prm1 == null || prm2 == null || prm3 == null || prm4 == null || prm5 == null)
                        {
                            // empty result
                            return xmlDataSet;
                        }
                        //Authorization
                        if (!dbUser.ValidateUserLicensePlateOne(userId, prm1))
                            throw new ASIAuthorizationFailedException("User: " + userId + " LicensePlate: " + prm1);
                        dsReport = stopReportByLandmark.GetStopReportByLandmark(prm1, prm2, prm3, userId, Convert.ToBoolean(prm4), Convert.ToInt32(prm5), lang, ref requestOverflowed, ref totalSqlRecords);
                        if (dsReport != null)
                            xmlDataSet = dsReport.GetXml();
                        #endregion
                        break;
                    case ReportTemplate.ReportTypes.FleetStopByLandmark:
                        #region Prepares Fleet Stop Report By Landmark
                        VLF.DAS.Logic.Report fleetStopReportByLandmark = new VLF.DAS.Logic.Report(connectionString);
                        prm1 = Util.PairFindValue(ReportTemplate.RpFleetStopFirstParamName, xmlParams);
                        prm2 = Util.PairFindValue(ReportTemplate.RpFleetStopSecondParamName, xmlParams);
                        prm3 = Util.PairFindValue(ReportTemplate.RpFleetStopThirdParamName, xmlParams);
                        prm4 = Util.PairFindValue(ReportTemplate.RpFleetStopFourthParamName, xmlParams);
                        prm5 = Util.PairFindValue(ReportTemplate.RpFleetStopFifthParamName, xmlParams);
                        if (xmlParams == null || prm1 == null || prm2 == null || prm3 == null || prm4 == null || prm5 == null)
                        {
                            // empty result
                            return xmlDataSet;
                        }
                        //Authorization
                        //if (!dbUser.ValidateUserFleetOne(userId, Convert.ToInt32(prm1)))
                        //    throw new ASIAuthorizationFailedException("User: " + userId + " Fleet: " + prm1);
                        dsReport = fleetStopReportByLandmark.GetFleetStopReportByLandmark(Convert.ToInt32(prm1), prm2, prm3, userId, Convert.ToBoolean(prm4), Convert.ToInt32(prm5), lang, ref requestOverflowed, ref totalSqlRecords);
                        if (dsReport != null)
                            xmlDataSet = dsReport.GetXml();
                        #endregion
                        break;
                    case ReportTemplate.ReportTypes.OffHoursReport:
                        #region Prepares Single Vehicle Off Hours Report
                        VLF.DAS.Logic.Report offHoursReport = new VLF.DAS.Logic.Report(connectionString);
                        prm1 = Util.PairFindValue(ReportTemplate.RpOffHourFirstParamName, xmlParams);
                        prm2 = Util.PairFindValue(ReportTemplate.RpOffHourSecondParamName, xmlParams);
                        prm3 = Util.PairFindValue(ReportTemplate.RpOffHourThirdParamName, xmlParams);
                        prm4 = Util.PairFindValue(ReportTemplate.RpOffHourFourthParamName, xmlParams);
                        prm5 = Util.PairFindValue(ReportTemplate.RpOffHourFifthParamName, xmlParams);
                        prm6 = Util.PairFindValue(ReportTemplate.RpOffHourSixthParamName, xmlParams);
                        prm7 = Util.PairFindValue(ReportTemplate.RpOffHourSeventhParamName, xmlParams);
                        prm8 = Util.PairFindValue(ReportTemplate.RpOffHourEightParamName, xmlParams);
                        prm9 = Util.PairFindValue(ReportTemplate.RpOffHourNineParamName, xmlParams);
                        prm10 = Util.PairFindValue(ReportTemplate.RpOffHourTenParamName, xmlParams);
                        prm11 = Util.PairFindValue(ReportTemplate.RpOffHourElevenParamName, xmlParams);
                        prm12 = Util.PairFindValue(ReportTemplate.RpOffHourTwelveParamName, xmlParams);

                        if (xmlParams == null || prm1 == null || prm2 == null || prm3 == null || prm4 == null || prm5 == null || prm6 == null || prm7 == null || prm8 == null || prm9 == null || prm10 == null || prm11 == null || prm12 == null || prm13 == null)
                            return xmlDataSet;

                        //Authorization
                        if (!dbUser.ValidateUserLicensePlateOne(userId, prm1))
                            throw new ASIAuthorizationFailedException("User: " + userId + " LicensePlate: " + prm1);
                        dsReport = offHoursReport.GetOffHourReport(prm1, prm2, prm3, userId, Convert.ToBoolean(prm4), Convert.ToInt16(prm5), Convert.ToInt16(prm6), Convert.ToInt16(prm7), Convert.ToInt16(prm8), Convert.ToInt16(prm9), Convert.ToInt16(prm10), Convert.ToInt16(prm11), Convert.ToInt16(prm12), lang, ref requestOverflowed, ref totalSqlRecords, ref outMaxOverflowed, ref outMaxRecords);
                        if (dsReport != null)
                            xmlDataSet = dsReport.GetXml();
                        #endregion
                        break;
                    case ReportTemplate.ReportTypes.FleetOffHoursReport:
                        #region Prepares Single Vehicle Off Hours Report
                        VLF.DAS.Logic.Report fleetOffHoursReport = new VLF.DAS.Logic.Report(connectionString);
                        prm1 = Util.PairFindValue(ReportTemplate.RpFleetOffHourFirstParamName, xmlParams);
                        prm2 = Util.PairFindValue(ReportTemplate.RpOffHourSecondParamName, xmlParams);
                        prm3 = Util.PairFindValue(ReportTemplate.RpOffHourThirdParamName, xmlParams);
                        prm4 = Util.PairFindValue(ReportTemplate.RpOffHourFourthParamName, xmlParams);
                        prm5 = Util.PairFindValue(ReportTemplate.RpOffHourFifthParamName, xmlParams);
                        prm6 = Util.PairFindValue(ReportTemplate.RpOffHourSixthParamName, xmlParams);
                        prm7 = Util.PairFindValue(ReportTemplate.RpOffHourSeventhParamName, xmlParams);
                        prm8 = Util.PairFindValue(ReportTemplate.RpOffHourEightParamName, xmlParams);
                        prm9 = Util.PairFindValue(ReportTemplate.RpOffHourNineParamName, xmlParams);
                        prm10 = Util.PairFindValue(ReportTemplate.RpOffHourTenParamName, xmlParams);
                        prm11 = Util.PairFindValue(ReportTemplate.RpOffHourElevenParamName, xmlParams);
                        prm12 = Util.PairFindValue(ReportTemplate.RpOffHourTwelveParamName, xmlParams);

                        if (xmlParams == null || prm1 == null || prm2 == null || prm3 == null || prm4 == null || prm5 == null || prm6 == null || prm7 == null || prm8 == null || prm9 == null || prm10 == null || prm11 == null || prm12 == null || prm13 == null)
                            return xmlDataSet;

                        //Authorization
                        //if (!dbUser.ValidateUserFleetOne(userId, Convert.ToInt32(prm1)))
                        //    throw new ASIAuthorizationFailedException("User: " + userId + " Fleet: " + prm1);
                        dsReport = fleetOffHoursReport.GetFleetOffHourReport(Convert.ToInt32(prm1), prm2, prm3, userId, Convert.ToBoolean(prm4), Convert.ToInt16(prm5), Convert.ToInt16(prm6), Convert.ToInt16(prm7), Convert.ToInt16(prm8), Convert.ToInt16(prm9), Convert.ToInt16(prm10), Convert.ToInt16(prm11), Convert.ToInt16(prm12), lang, ref requestOverflowed, ref totalSqlRecords, ref outMaxOverflowed, ref outMaxRecords);
                        if (dsReport != null)
                            xmlDataSet = dsReport.GetXml();
                        #endregion
                        break;
                    case ReportTemplate.ReportTypes.FleetException:
                        #region Prepares Exception Report
                        boxExceptionReport = new VLF.DAS.Logic.Report(connectionString);
                        prm1 = Util.PairFindValue(ReportTemplate.RpFleetTripFirstParamName, xmlParams);
                        prm2 = Util.PairFindValue(ReportTemplate.RpExceptionSecondParamName, xmlParams);
                        prm3 = Util.PairFindValue(ReportTemplate.RpExceptionThirdParamName, xmlParams);
                        prm4 = Util.PairFindValue(ReportTemplate.RpExceptionFourthParamName, xmlParams);
                        prm5 = Util.PairFindValue(ReportTemplate.RpExceptionFifthParamName, xmlParams);
                        prm6 = Util.PairFindValue(ReportTemplate.RpExceptionSixthParamName, xmlParams);
                        prm7 = Util.PairFindValue(ReportTemplate.RpExceptionSeventhParamName, xmlParams);
                        prm8 = Util.PairFindValue(ReportTemplate.RpExceptionEightParamName, xmlParams);
                        prm9 = Util.PairFindValue(ReportTemplate.RpExceptionNineParamName, xmlParams);
                        prm10 = Util.PairFindValue(ReportTemplate.RpExceptionTenParamName, xmlParams);
                        prm11 = Util.PairFindValue(ReportTemplate.RpExceptionElevenParamName, xmlParams);
                        prm12 = Util.PairFindValue(ReportTemplate.RpExceptionTwelveParamName, xmlParams);
                        prm13 = Util.PairFindValue(ReportTemplate.RpExceptionThirteenParamName, xmlParams);
                        prm14 = Util.PairFindValue(ReportTemplate.RpExceptionFourteenParamName, xmlParams);
                        prm15 = Util.PairFindValue(ReportTemplate.RpExceptionFifteenParamName, xmlParams);
                        prm16 = Util.PairFindValue(ReportTemplate.RpExceptionSixteenParamName, xmlParams);
                        prm17 = Util.PairFindValue(ReportTemplate.RpExceptionSeventeenParamName, xmlParams);
                        prm18 = Util.PairFindValue(ReportTemplate.RpExceptionEighteenParamName, xmlParams);
                        prm19 = Util.PairFindValue(ReportTemplate.RpExceptionNineteenParamName, xmlParams);
                        prm20 = Util.PairFindValue(ReportTemplate.RpExceptionTwentyParamName, xmlParams);
                        prm21 = Util.PairFindValue(ReportTemplate.RpExceptionTwentyFirstParamName, xmlParams);
                        if (xmlParams == null || prm1 == null || prm2 == null || prm3 == null ||
                     prm4 == null || prm5 == null || prm6 == null || prm7 == null ||
                     prm8 == null || prm9 == null || prm10 == null || prm11 == null ||
                     prm12 == null || prm13 == null || prm14 == null || prm15 == null || prm16 == null ||
                     prm17 == null || prm18 == null || prm19 == null || prm20 == null || prm21 == null)
                        {
                            // empty result
                            return xmlDataSet;
                        }

                        //Authorization
                        //if (!dbUser.ValidateUserFleetOne(userId, Convert.ToInt32(prm1)))
                        //    throw new ASIAuthorizationFailedException("User: " + userId + " Fleet: " + prm1);
                        dsReport = boxExceptionReport.GetFleetExceptionReport(Convert.ToInt32(prm1),
                           prm2,//fromDateTime,
                           prm3,//toDateTime,
                           userId,
                           Convert.ToInt16(prm4),//sosLimit,
                           Convert.ToInt32(prm5),//noDoorSnsHrs,
                           Convert.ToBoolean(prm6),//IncludeTar
                           Convert.ToBoolean(prm7),//IncludeMobilize
                           Convert.ToBoolean(prm8),//15SecDoorSns
                           Convert.ToBoolean(prm9),//50%ofLeash
                           Convert.ToBoolean(prm10),//MainAndBackupBatterySns
                           Convert.ToBoolean(prm11),//TamperSns
                           Convert.ToBoolean(prm12),//AnyPanicSns
                           Convert.ToBoolean(prm13),//ThreeKeypadAttemptsSns
                           Convert.ToBoolean(prm14),//AltGPSAntennaSns
                           Convert.ToBoolean(prm15),//ControllerStatus
                           Convert.ToBoolean(prm16),//LeashBrokenSns
                           Convert.ToBoolean(prm17),//DriverDoor
                           Convert.ToBoolean(prm18),//PassengerDoor
                           Convert.ToBoolean(prm19),//SideHopperDoor
                           Convert.ToBoolean(prm20),//RearHopperDoor
                           Convert.ToBoolean(prm21),//IncludeCurrentTar
                           Convert.ToBoolean(prm22),//Locker1
                         Convert.ToBoolean(prm23),//Locker2
                         Convert.ToBoolean(prm24),//Locker3
                         Convert.ToBoolean(prm25),//Locker4
                         Convert.ToBoolean(prm26),//Locker5
                         Convert.ToBoolean(prm27),//Locker6
                         Convert.ToBoolean(prm28),//Locker7
                         Convert.ToBoolean(prm29),//Locker8
                         Convert.ToBoolean(prm30),//Locker9

                                  ref requestOverflowed,
                           ref totalSqlRecords);
                        if (dsReport != null)
                            xmlDataSet = dsReport.GetXml();
                        #endregion
                        break;
                    case ReportTemplate.ReportTypes.FleetMaintenanceReport:
                        #region Prepares Fleet Maintenance Report
                        boxExceptionReport = new VLF.DAS.Logic.Report(connectionString);
                        prm1 = Util.PairFindValue(ReportTemplate.RpFleetMaintenanceFirstParamName, xmlParams);
                        if (xmlParams == null || prm1 == null)
                        {
                            // empty result
                            return xmlDataSet;
                        }

                        //Authorization
                        //if (!dbUser.ValidateUserFleetOne(userId, Convert.ToInt32(prm1)))
                        //    throw new ASIAuthorizationFailedException("User: " + userId + " Fleet: " + prm1);
                        dbFleet = new VLF.DAS.Logic.Fleet(connectionString);

                        dsReport = dbFleet.GetFleetMaintenanceHistory(userId, Convert.ToInt32(prm1));
                        if (dsReport != null)
                            xmlDataSet = dsReport.GetXml();
                        #endregion
                        break;
                }
            }
            finally
            {
                if (dbUser != null)
                    dbUser.Dispose();
                if (dbFleet != null)
                    dbFleet.Dispose();
            }
            return xmlDataSet;
        }

        /// <summary>
        /// Get strong-type dataset as XML.
        /// </summary>
        /// <param name="repType"></param>
        /// <param name="xmlParams"></param>
        /// <param name="userId"></param>
        /// <returns>XML</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public string GetSystemReport(ReportTemplate.ReportTypes repType, string xmlParams, int userId)
        {
            bool showExceptionOnly = false;
            string xmlDataSet = "";
            string prm1 = "";
            string prm2 = "";
            string prm3 = "";
            string prm4 = "";
            string prm5 = "";
            string prm6 = "";
            DataSet dsReport = null;

            switch (repType)
            {
                case ReportTemplate.ReportTypes.Latency:
                    #region Prepares Latency Report
                    VLF.DAS.Logic.Report vehicleLatency = new VLF.DAS.Logic.Report(connectionString);
                    prm1 = Util.PairFindValue(ReportTemplate.RpLatencyFirstParamName, xmlParams);
                    prm2 = Util.PairFindValue(ReportTemplate.RpLatencySecondParamName, xmlParams);
                    prm3 = Util.PairFindValue(ReportTemplate.RpLatencyThirdParamName, xmlParams);
                    prm4 = Util.PairFindValue(ReportTemplate.RpLatencyFourthParamName, xmlParams);
                    prm5 = Util.PairFindValue(ReportTemplate.RpLatencyFifthParamName, xmlParams);
                    prm6 = Util.PairFindValue(ReportTemplate.RpLatencySixthParamName, xmlParams);
                    if (xmlParams == null || prm1 == null || prm2 == null || prm3 == null || prm4 == null || prm5 == null)
                        return xmlDataSet;
                    StringReader strrXML = new StringReader(prm6);
                    DataSet dsParams = new DataSet();
                    dsParams.ReadXml(strrXML);

                    if (Convert.ToInt64(prm3) != VLF.CLS.Def.Const.unassignedIntValue) // Vehicle Id
                        dsReport = vehicleLatency.GetVehicleLatencyReport(Convert.ToInt64(prm3), prm4, prm5, dsParams);
                    else if (Convert.ToInt32(prm2) != VLF.CLS.Def.Const.unassignedIntValue) // Fleet Id
                        dsReport = vehicleLatency.GetFleetLatencyReport(Convert.ToInt32(prm2), prm4, prm5, dsParams);
                    else // Organization Id
                        dsReport = vehicleLatency.GetOrganizationLatencyReport(Convert.ToInt32(prm1), prm4, prm5, dsParams);
                    if (dsReport != null)
                        xmlDataSet = dsReport.GetXml();
                    #endregion
                    break;

            }
            return xmlDataSet;
        }

        /// <summary>
        ///         extract the usage for a specific sensor for all vehicles in a fleet during two dates
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="userId"></param>
        /// <param name="sensorId"></param>
        /// <param name="dtFrom"></param>
        /// <param name="dtTo"></param>
        /// <returns></returns>
        public string GetSensorReportForFleet(int fleetId, int userId, int sensorId,
                                              DateTime dtFrom, DateTime dtTo)
        {
            #region Prepare Sensors for Fleet Report
            string xmlDataSet = "";
            VLF.DAS.DB.SensorReport sensReport = new VLF.DAS.DB.SensorReport(connectionString);
            DataSet dsResult1 = sensReport.Exec_GetSensorsPerFleet(fleetId, userId, sensorId, dtFrom, dtTo);
            if (null != dsResult1)
            {
                DataSet dsResult2 = sensReport.Exec_GetIdlingDurationForFleet(fleetId, userId, dtFrom, dtTo);
                sensReport.FillIdlingDuration(dtFrom, dtTo, dsResult2);

                sensReport.FillFleetUtilization(sensorId, dtFrom, dtTo, dsResult1);
                xmlDataSet = sensReport.result.GetXml();
            }
            else
                Util.BTrace(Util.ERR1, "-- ReportGenerator.GetSensorReportForFleet -> ERROR for sp_GetSensorsPerFleet for fleetId={0}, sensorId={1}, from={2}, to={3}",
                            fleetId, sensorId, dtFrom, dtTo);

            return xmlDataSet;
            #endregion
        }




        /// <summary>
        ///         extract the usage for a specific sensor for all vehicles in a fleet during two dates
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="userId"></param>
        /// <param name="sensorId"></param>
        /// <param name="dtFrom"></param>
        /// <param name="dtTo"></param>
        /// <returns></returns>
        public string GetSensorIdlingReportForFleet(int fleetId, int userId, int sensorId,
                                              DateTime dtFrom, DateTime dtTo)
        {
            #region Prepare Sensors for Fleet Report
            string xmlDataSet = "";
            VLF.DAS.DB.SensorReport sensReport = new VLF.DAS.DB.SensorReport(connectionString);
            DataSet dsResult1 = sensReport.Exec_GetSensorsPerFleet(fleetId, userId, sensorId, dtFrom, dtTo);
            if (null != dsResult1)
            {
                DataSet dsResult2 = sensReport.Exec_GetIdlingMessagesForFleet(fleetId, userId, dtFrom, dtTo);
                sensReport.FillFleetIdlingMessages(dtFrom, dtTo, dsResult2);

                sensReport.FillFleetUtilization(sensorId, dtFrom, dtTo, dsResult1);
                xmlDataSet = sensReport.result.GetXml();
            }
            else
                Util.BTrace(Util.ERR1, "-- ReportGenerator.GetSensorReportForFleet -> ERROR for sp_GetSensorsPerFleet for fleetId={0}, sensorId={1}, from={2}, to={3}",
                            fleetId, sensorId, dtFrom, dtTo);

            return xmlDataSet;
            #endregion
        }

        /// <summary>
        ///         extract the usage for a specific sensor for all vehicles in a fleet during two dates
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="userId"></param>
        /// <param name="sensorId"></param>
        /// <param name="dtFrom"></param>
        /// <param name="dtTo"></param>
        /// <returns></returns>
        public string GetSensorReportForFleetWithIdling(int fleetId, int userId, int sensorId,
                                              DateTime dtFrom, DateTime dtTo)
        {
            #region Prepare Sensors for Fleet Report
            string xmlDataSet = "";
            VLF.DAS.DB.SensorReport sensReport = new VLF.DAS.DB.SensorReport(connectionString);
            DataSet dsResult1 = sensReport.Exec_GetSensorsPerFleet(fleetId, userId, sensorId, dtFrom, dtTo);
            if (null != dsResult1)
            {
                DataSet dsResult2 = sensReport.Exec_GetIdlingDurationForFleet(fleetId, userId, dtFrom, dtTo);
                sensReport.FillIdlingDuration(dtFrom, dtTo, dsResult2);

                sensReport.FillFleetUtilization(sensorId, dtFrom, dtTo, dsResult1);
                xmlDataSet = sensReport.result.GetXml();
            }
            else
                Util.BTrace(Util.ERR1, "-- ReportGenerator.GetSensorReportForFleet -> ERROR for sp_GetSensorsPerFleet for fleetId={0}, sensorId={1}, from={2}, to={3}",
                            fleetId, sensorId, dtFrom, dtTo);

            return xmlDataSet;
            #endregion
        }

        /// <summary>
        ///         extract the usage for all vehicles from a fleet during two dates
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="userId"></param>
        /// <param name="dtFrom"></param>
        /// <param name="dtTo"></param>
        /// <returns>
        ///      [BoxId]           boxid
        ///      [Week]            week in the year
        ///      [Date]            first ignition on
        ///      [WorkingMinutes]  last ignition off - first ignition on
        ///      [OnMinutes]       sum among all consecutive (ignition off - ignition on)
        /// </returns>
        public string GetActivityReportForFleet(int sensorId, int fleetId, int userId, DateTime dtFrom, DateTime dtTo)
        {
            #region Prepare Activity Report for Fleet
            string xmlDataSet = "";
            VLF.DAS.DB.AverageFleetReport report = new VLF.DAS.DB.AverageFleetReport(connectionString);
            DataSet dsResult1 = report.Exec_GetActivityReportForFleet(sensorId, fleetId, userId, dtFrom, dtTo);
            if (null != dsResult1)
            {
                DataSet dsResult2 = report.Exec_GetIdlingDurationForFleet(fleetId, userId, dtFrom, dtTo);
                report.FillIdlingDuration(dtFrom, dtTo, dsResult2);

                report.FillFleetUtilization(1, dtFrom, dtTo, dsResult1);
                xmlDataSet = report.result.GetXml();
            }
            else
                Util.BTrace(Util.ERR1, "-- ReportGenerator.GetActivityReportForFleet -> ERROR for sp_GetSensorsPerFleet for fleetId={0}, sensorId={1}, from={2}, to={3}",
                            fleetId, sensorId, dtFrom, dtTo);

            return xmlDataSet;
            #endregion
        }

        /// <summary>
        ///            run sp_GetActiveVehiclesPerDay which takes into consideration the user preferences
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="userId"></param>
        /// <param name="dtFrom"></param>
        /// <param name="dtTo"></param>
        /// <returns>
        ///      [OriginDateTime]     - date only 
        ///      [ActiveVehicles]     - the number of active vehicles on that day
        /// </returns>
        public string GetActiveVehiclesPerDay(int fleetId, int userId, DateTime dtFrom, DateTime dtTo)
        {

            VLF.DAS.DB.AverageFleetReport report = new VLF.DAS.DB.AverageFleetReport(connectionString);
            DataSet dsResult1 = report.Exec_GetActiveVehiclesPerDay(fleetId, userId, dtFrom, dtTo);
            return dsResult1.GetXml();

        }

        /*
                    case Enums.MessageType.HarshBraking:
                    case Enums.MessageType.ExtremeBraking:
                    case Enums.MessageType.HarshAcceleration:
                    case Enums.MessageType.ExtremeAcceleration:
                    case Enums.MessageType.SeatBelt:
      
         */
        public static int CT_VIOLATION_HARSHBRAKING = 0x0001;
        public static int CT_VIOLATION_HARSHACCELERATION = 0x0002;
        public static int CT_VIOLATION_EXTREMEACCELERATION = 0x0004;
        public static int CT_VIOLATION_EXTREMEBRAKING = 0x0008;
        public static int CT_VIOLATION_SEATBELT = 0x0010;
        public static int CT_VIOLATION_SPEED = 0x0020;
        public static int CT_VIOLATION_REVERSESPEED = 0x0040;
        public static int CT_VIOLATION_REVERSEDISTANCE = 0x0080;
        public static int CT_VIOLATION_HIGHRAIL = 0x0100;
        public static int CT_ALL_VIOLATIONS = CT_VIOLATION_HARSHBRAKING |
                                                             CT_VIOLATION_HARSHACCELERATION |
                                                             CT_VIOLATION_EXTREMEACCELERATION |
                                                             CT_VIOLATION_EXTREMEBRAKING |
                                                             CT_VIOLATION_SEATBELT |
                                                             CT_VIOLATION_SPEED |
                                                             CT_VIOLATION_REVERSESPEED |
                                                             CT_VIOLATION_REVERSEDISTANCE |
                                                             CT_VIOLATION_HIGHRAIL;

        /// <summary>
        ///         all violations defined in the mask
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="userId"></param>
        /// <param name="maskViolations">
        ///       combination of CT_VIOLATION_SEATBELT | CT_VIOLATION_SPEED 
        /// </param>
        /// <param name="dtFrom"></param>
        /// <param name="dtTo"></param>
        /// <returns>
        /// is the XML description for 
        ///    [BoxId], [VehicleDescription], [DateTime], 
        ///    [Duration],         -- for speed Duration is the actual speed value
        ///    [Description], [DescriptionId], [StreetAddress], [Driver] 
        ///
        ///    where description will be BoxMsgInTypeId for
        ///                HARSHBRAKING                                    
        ///                HARSHACCELERATION   
        ///                EXTREMEACCELERATION 
        ///                EXTREMEBRAKING      
        ///                SEATBELT            
        ///                SPEED               
        /// </returns>
        public string GetViolationReportForFleet(int fleetId, int userId, int maskViolations,
                                                 DateTime dtFrom, DateTime dtTo, int speed)
        {
            #region Violations Activity Report for Fleet
            string xmlDataSet = "";
            VLF.DAS.DB.ViolationsFleetReport report = new VLF.DAS.DB.ViolationsFleetReport(connectionString);
            DataSet dsResult = report.Exec_GetViolationsReportForFleet(fleetId, userId, maskViolations, dtFrom, dtTo, speed);
            if (null != dsResult)
            {
                //report.FillReport(dtFrom, dtTo, dsResult);
                //xmlDataSet = report.result.GetXml();
                xmlDataSet = dsResult.GetXml();
            }
            else
                Util.BTrace(Util.ERR1, "-- ReportGenerator.GetViolationReportForFleet -> ERROR for sp_GetViolationsReportForFleet for fleetId={0}, from={1}, to={2}",
                            fleetId, dtFrom, dtTo);

            return xmlDataSet;
            #endregion

        }


        public string GetViolationReportForFleet_Main(int fleetId, int userId, int maskViolations,
                                               DateTime dtFrom, DateTime dtTo, int speed)
        {
            #region Violations Activity Report for Fleet
            string xmlDataSet = "";
            VLF.DAS.DB.ViolationsFleetReport report = new VLF.DAS.DB.ViolationsFleetReport(connectionString);
            DataSet dsResult = report.Exec_GetViolationsReportForFleet_Main(fleetId, userId, maskViolations, dtFrom, dtTo, speed);
            if (null != dsResult)
            {
                //report.FillReport(dtFrom, dtTo, dsResult);
                //xmlDataSet = report.result.GetXml();
                xmlDataSet = dsResult.GetXml();
            }
            else
                Util.BTrace(Util.ERR1, "-- ReportGenerator.GetViolationReportForFleet -> ERROR for sp_GetViolationsReportForFleet for fleetId={0}, from={1}, to={2}",
                            fleetId, dtFrom, dtTo);

            return xmlDataSet;
            #endregion

        }

        // Changes for TimeZone Feature start

        /// <summary>
        /// Get Violation Report For Fleet By Lang
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="userId"></param>
        /// <param name="maskViolations"></param>
        /// <param name="dtFrom"></param>
        /// <param name="dtTo"></param>
        /// <param name="lang"></param>
        /// <param name="speed"></param>
        /// <returns></returns>
        public string GetViolationReportForFleetByLang_NewTZ(int fleetId, int userId, int maskViolations,
                                                       DateTime dtFrom, DateTime dtTo, string lang, int speed)
        {
            #region Violations Activity Report for Fleet
            string xmlDataSet = "";
            VLF.DAS.DB.ViolationsFleetReport report = new VLF.DAS.DB.ViolationsFleetReport(connectionString);
            DataSet dsResult = report.Exec_GetViolationsReportForFleet_NewTZ(fleetId, userId, maskViolations, dtFrom, dtTo, speed);
            if (null != dsResult)
            {
                if (lang != "en" && lang != null)
                {
                    LocalizationLayer.ServerLocalizationLayer dbl = new LocalizationLayer.ServerLocalizationLayer(connectionString);
                    dbl.LocalizationData(lang, "DescriptionId", "Description", "Violations", ref dsResult);
                }
                //report.FillReport(dtFrom, dtTo, dsResult);
                //xmlDataSet = report.result.GetXml();
                xmlDataSet = dsResult.GetXml();
            }
            else
                Util.BTrace(Util.ERR1, "-- ReportGenerator.GetViolationReportForFleet -> ERROR for sp_GetViolationsReportForFleet for fleetId={0}, from={1}, to={2}",
                            fleetId, dtFrom, dtTo);

            return xmlDataSet;
            #endregion

        }

        // Changes for TimeZone Feature end

        /// <summary>
        /// Get Violation Report For Fleet By Lang
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="userId"></param>
        /// <param name="maskViolations"></param>
        /// <param name="dtFrom"></param>
        /// <param name="dtTo"></param>
        /// <param name="lang"></param>
        /// <param name="speed"></param>
        /// <returns></returns>
        public string GetViolationReportForFleetByLang(int fleetId, int userId, int maskViolations,
                                                       DateTime dtFrom, DateTime dtTo, string lang, int speed)
        {
            #region Violations Activity Report for Fleet
            string xmlDataSet = "";
            VLF.DAS.DB.ViolationsFleetReport report = new VLF.DAS.DB.ViolationsFleetReport(connectionString);
            DataSet dsResult = report.Exec_GetViolationsReportForFleet(fleetId, userId, maskViolations, dtFrom, dtTo, speed);
            if (null != dsResult)
            {
                if (lang != "en" && lang != null)
                {
                    LocalizationLayer.ServerLocalizationLayer dbl = new LocalizationLayer.ServerLocalizationLayer(connectionString);
                    dbl.LocalizationData(lang, "DescriptionId", "Description", "Violations", ref dsResult);
                }
                //report.FillReport(dtFrom, dtTo, dsResult);
                //xmlDataSet = report.result.GetXml();
                xmlDataSet = dsResult.GetXml();
            }
            else
                Util.BTrace(Util.ERR1, "-- ReportGenerator.GetViolationReportForFleet -> ERROR for sp_GetViolationsReportForFleet for fleetId={0}, from={1}, to={2}",
                            fleetId, dtFrom, dtTo);

            return xmlDataSet;
            #endregion

        }




        /// <summary>
        /// Get Violation Report For Fleet By Lang
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="userId"></param>
        /// <param name="maskViolations"></param>
        /// <param name="dtFrom"></param>
        /// <param name="dtTo"></param>
        /// <param name="lang"></param>
        /// <param name="speed"></param>
        /// <returns></returns>
        public string GetViolationReportForFleetByLang_Exteneded(int fleetId, int userId, int maskViolations,
                                                       DateTime dtFrom, DateTime dtTo, string lang, int speed)
        {
            #region Violations Activity Report for Fleet
            string xmlDataSet = "";
            VLF.DAS.DB.ViolationsFleetReport report = new VLF.DAS.DB.ViolationsFleetReport(connectionString);
            DataSet dsResult = report.Exec_GetViolationsReportForFleet_Exteneded(fleetId, userId, maskViolations, dtFrom, dtTo, speed);
            if (null != dsResult)
            {
                if (lang != "en" && lang != null)
                {
                    LocalizationLayer.ServerLocalizationLayer dbl = new LocalizationLayer.ServerLocalizationLayer(connectionString);
                    dbl.LocalizationData(lang, "DescriptionId", "Description", "Violations", ref dsResult);
                }
                //report.FillReport(dtFrom, dtTo, dsResult);
                //xmlDataSet = report.result.GetXml();
                xmlDataSet = dsResult.GetXml();
            }
            else
                Util.BTrace(Util.ERR1, "-- ReportGenerator.Exec_GetViolationsReportForFleet_Exteneded -> ERROR for sp_GetViolationsReportForFleet for fleetId={0}, from={1}, to={2}",
                            fleetId, dtFrom, dtTo);

            return xmlDataSet;
            #endregion

        }




        public string GetViolationReportForFleetByLang_NewSafetyMatrix(int fleetId, int userId, int maskViolations,
                                              DateTime dtFrom, DateTime dtTo, string lang, int speed, string extraParams)
        {
            #region Violations Activity Report for Fleet
            string xmlDataSet = "";
            VLF.DAS.DB.ViolationsFleetReport report = new VLF.DAS.DB.ViolationsFleetReport(connectionString);
            DataSet dsResult = report.Exec_GetViolationsReportForFleet_NewSafetyMatrix(fleetId, userId, maskViolations, dtFrom, dtTo, speed, extraParams);
            if (null != dsResult)
            {
                if (lang != "en" && lang != null)
                {
                    LocalizationLayer.ServerLocalizationLayer dbl = new LocalizationLayer.ServerLocalizationLayer(connectionString);
                    dbl.LocalizationData(lang, "DescriptionId", "Description", "Violations", ref dsResult);
                }
                //report.FillReport(dtFrom, dtTo, dsResult);
                //xmlDataSet = report.result.GetXml();
                xmlDataSet = dsResult.GetXml();
            }
            else
                Util.BTrace(Util.ERR1, "-- ReportGenerator.GetViolationReportForFleetByLang_NewSafetyMatrix -> ERROR for sp_GetViolationsReportForFleet for fleetId={0}, from={1}, to={2}",
                            fleetId, dtFrom, dtTo);

            return xmlDataSet;
            #endregion


        }



        public string GetViolationReportForFleetByLang_Exteneded_NewSafetyMatrix(int fleetId, int userId, int maskViolations,
                                              DateTime dtFrom, DateTime dtTo, string lang, int speed, string extraParams)
        {
            #region Violations Activity Report for Fleet
            string xmlDataSet = "";
            VLF.DAS.DB.ViolationsFleetReport report = new VLF.DAS.DB.ViolationsFleetReport(connectionString);
            DataSet dsResult = report.Exec_GetViolationsReportForFleet_Exteneded_NewSafetyMatrix(fleetId, userId, maskViolations, dtFrom, dtTo, speed, extraParams);
            if (null != dsResult)
            {
                if (lang != "en" && lang != null)
                {
                    LocalizationLayer.ServerLocalizationLayer dbl = new LocalizationLayer.ServerLocalizationLayer(connectionString);
                    dbl.LocalizationData(lang, "DescriptionId", "Description", "Violations", ref dsResult);
                }
                //report.FillReport(dtFrom, dtTo, dsResult);
                //xmlDataSet = report.result.GetXml();
                xmlDataSet = dsResult.GetXml();
            }
            else
                Util.BTrace(Util.ERR1, "-- ReportGenerator.Exec_GetViolationsReportForFleet_Exteneded -> ERROR for sp_GetViolationsReportForFleet for fleetId={0}, from={1}, to={2}",
                            fleetId, dtFrom, dtTo);

            return xmlDataSet;
            #endregion

        }

        public string GetViolationReportForFleetByLang_Main(int fleetId, int userId, int maskViolations,
                                                     DateTime dtFrom, DateTime dtTo, string lang, int speed)
        {
            #region Violations Activity Report for Fleet
            string xmlDataSet = "";
            VLF.DAS.DB.ViolationsFleetReport report = new VLF.DAS.DB.ViolationsFleetReport(connectionString);
            DataSet dsResult = report.Exec_GetViolationsReportForFleet_Main(fleetId, userId, maskViolations, dtFrom, dtTo, speed);
            if (null != dsResult)
            {
                if (lang != "en" && lang != null)
                {
                    LocalizationLayer.ServerLocalizationLayer dbl = new LocalizationLayer.ServerLocalizationLayer(connectionString);
                    dbl.LocalizationData(lang, "DescriptionId", "Description", "Violations", ref dsResult);
                }
                //report.FillReport(dtFrom, dtTo, dsResult);
                //xmlDataSet = report.result.GetXml();
                xmlDataSet = dsResult.GetXml();
            }
            else
                Util.BTrace(Util.ERR1, "-- ReportGenerator.GetViolationReportForFleet -> ERROR for sp_GetViolationsReportForFleet for fleetId={0}, from={1}, to={2}",
                            fleetId, dtFrom, dtTo);

            return xmlDataSet;
            #endregion

        }


        /// <summary>
        /// Get Violation Report For Fleet By Lang
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="userId"></param>
        /// <param name="maskViolations"></param>
        /// <param name="dtFrom"></param>
        /// <param name="dtTo"></param>
        /// <param name="lang"></param>
        /// <param name="speed"></param>
        /// <returns></returns>
        public string GetViolationReportForVehicleByLang(int boxId, int userId, int maskViolations,
                                                       DateTime dtFrom, DateTime dtTo, string lang, int speed)
        {
            #region Violations Activity Report for Fleet
            string xmlDataSet = "";
            VLF.DAS.DB.ViolationsFleetReport report = new VLF.DAS.DB.ViolationsFleetReport(connectionString);
            //DataSet dsResult = report.Exec_GetViolationsReportForVehicle(boxId, userId, maskViolations, dtFrom, dtTo, speed);
            //DataSet dsResult = report.Exec_GetViolationsReportForVehicle_Exteneded(boxId, userId, maskViolations, dtFrom, dtTo, speed);
            DataSet dsResult = null;

            if (null != dsResult)
            {
                if (lang != "en" && lang != null)
                {
                    LocalizationLayer.ServerLocalizationLayer dbl = new LocalizationLayer.ServerLocalizationLayer(connectionString);
                    dbl.LocalizationData(lang, "DescriptionId", "Description", "Violations", ref dsResult);
                }
                //report.FillReport(dtFrom, dtTo, dsResult);
                //xmlDataSet = report.result.GetXml();
                xmlDataSet = dsResult.GetXml();
            }
            else
                Util.BTrace(Util.ERR1, "-- ReportGenerator.GetViolationReportForFleet -> ERROR for sp_GetViolationsReportForFleet for boxId={0}, from={1}, to={2}",
                            boxId, dtFrom, dtTo);

            return xmlDataSet;
            #endregion

        }

        /// <summary>
        /// Get Violation Report For Fleet With Score
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="userId"></param>
        /// <param name="maskViolations"></param>
        /// <param name="dtFrom"></param>
        /// <param name="dtTo"></param>
        /// <param name="ViolationPoints"></param>
        /// <returns></returns>
        public string GetViolationReportForFleetWithScore(int fleetId, int userId, int maskViolations,
                                         DateTime dtFrom, DateTime dtTo, string ViolationPoints)
        {
            #region Violations Activity Report for Fleet
            string xmlDataSet = "";
            VLF.DAS.DB.ViolationsFleetReport report = new VLF.DAS.DB.ViolationsFleetReport(connectionString);
            DataSet dsResult = report.Exec_GetViolationsReportForFleetWithScore(fleetId, userId, maskViolations, dtFrom, dtTo, ViolationPoints);
            if (null != dsResult)
            {
                //report.FillReport(dtFrom, dtTo, dsResult);
                //xmlDataSet = report.result.GetXml();
                xmlDataSet = dsResult.GetXml();
            }
            else
                Util.BTrace(Util.ERR1, "-- ReportGenerator.GetViolationReportForFleetWithScore -> ERROR for sp_GetViolationsReportForFleetWithScore for fleetId={0}, from={1}, to={2}",
                            fleetId, dtFrom, dtTo);

            return xmlDataSet;
            #endregion

        }



        public string GetViolationReportForFleetWithScore_Main(int fleetId, int userId, int maskViolations,
                                       DateTime dtFrom, DateTime dtTo, string ViolationPoints)
        {
            #region Violations Activity Report for Fleet
            string xmlDataSet = "";
            VLF.DAS.DB.ViolationsFleetReport report = new VLF.DAS.DB.ViolationsFleetReport(connectionString);
            DataSet dsResult = report.Exec_GetViolationsReportForFleetWithScore_Main(fleetId, userId, maskViolations, dtFrom, dtTo, ViolationPoints);
            if (null != dsResult)
            {
                //report.FillReport(dtFrom, dtTo, dsResult);
                //xmlDataSet = report.result.GetXml();
                xmlDataSet = dsResult.GetXml();
            }
            else
                Util.BTrace(Util.ERR1, "-- ReportGenerator.GetViolationReportForFleetWithScore -> ERROR for sp_GetViolationsReportForFleetWithScore for fleetId={0}, from={1}, to={2}",
                            fleetId, dtFrom, dtTo);

            return xmlDataSet;
            #endregion

        }




        public string GetViolationReportForFleetWithScore_Extended(int fleetId, int userId, int maskViolations,
                                       DateTime dtFrom, DateTime dtTo, string ViolationPoints)
        {
            #region Violations Activity Report for Fleet
            string xmlDataSet = "";
            VLF.DAS.DB.ViolationsFleetReport report = new VLF.DAS.DB.ViolationsFleetReport(connectionString);
            DataSet dsResult = report.Exec_GetViolationsReportForFleetWithScore_Extended(fleetId, userId, maskViolations, dtFrom, dtTo, ViolationPoints);
            if (null != dsResult)
            {
                //report.FillReport(dtFrom, dtTo, dsResult);
                //xmlDataSet = report.result.GetXml();
                xmlDataSet = dsResult.GetXml();
            }
            else
                Util.BTrace(Util.ERR1, "-- ReportGenerator.GetViolationReportForFleetWithScore -> ERROR for sp_GetViolationsReportForFleetWithScore for fleetId={0}, from={1}, to={2}",
                            fleetId, dtFrom, dtTo);

            return xmlDataSet;
            #endregion

        }



        public string GetViolationReportForFleetWithScore_NewSafetyMatrix(int fleetId, int userId, int maskViolations,
                              DateTime dtFrom, DateTime dtTo, string ViolationPoints, string extraParams)
        {
            #region Violations Activity Report for Fleet
            string xmlDataSet = "";
            VLF.DAS.DB.ViolationsFleetReport report = new VLF.DAS.DB.ViolationsFleetReport(connectionString);
            DataSet dsResult = report.Exec_GetViolationsReportForFleetWithScore_NewSafetyMatrix(fleetId, userId, maskViolations, dtFrom, dtTo, ViolationPoints, extraParams);
            if (null != dsResult)
            {
                //report.FillReport(dtFrom, dtTo, dsResult);
                //xmlDataSet = report.result.GetXml();
                xmlDataSet = dsResult.GetXml();
            }
            else
                Util.BTrace(Util.ERR1, "-- ReportGenerator.GetViolationReportForFleetWithScore_NewSafetyMatrix -> ERROR for sp_GetViolationsReportForFleetWithScore for fleetId={0}, from={1}, to={2}",
                            fleetId, dtFrom, dtTo);

            return xmlDataSet;
            #endregion

        }



        public string GetViolationReportForFleetWithScore_Extended_NewSafetyMatrix(int fleetId, int userId, int maskViolations,
                              DateTime dtFrom, DateTime dtTo, string ViolationPoints,string extraParams)
        {
            #region Violations Activity Report for Fleet
            string xmlDataSet = "";
            VLF.DAS.DB.ViolationsFleetReport report = new VLF.DAS.DB.ViolationsFleetReport(connectionString);
            DataSet dsResult = report.Exec_GetViolationsReportForFleetWithScore_Extended_NewSafetyMatrix(fleetId, userId, maskViolations, dtFrom, dtTo, ViolationPoints, extraParams);
            if (null != dsResult)
            {
                //report.FillReport(dtFrom, dtTo, dsResult);
                //xmlDataSet = report.result.GetXml();
                xmlDataSet = dsResult.GetXml();
            }
            else
                Util.BTrace(Util.ERR1, "-- ReportGenerator.GetViolationReportForFleetWithScore -> ERROR for sp_GetViolationsReportForFleetWithScore for fleetId={0}, from={1}, to={2}",
                            fleetId, dtFrom, dtTo);

            return xmlDataSet;
            #endregion

        }

        /// <summary>
        /// Get Violation Report For Fleet With Score
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="userId"></param>
        /// <param name="maskViolations"></param>
        /// <param name="dtFrom"></param>
        /// <param name="dtTo"></param>
        /// <param name="ViolationPoints"></param>
        /// <returns></returns>
        public string GetViolationReportForVehicleWithScore(int boxId, int userId, int maskViolations,
                                         DateTime dtFrom, DateTime dtTo, string ViolationPoints)
        {
            #region Violations Activity Report for Fleet
            string xmlDataSet = "";
            VLF.DAS.DB.ViolationsFleetReport report = new VLF.DAS.DB.ViolationsFleetReport(connectionString);
            //DataSet dsResult = report.Exec_GetViolationsReportForVehicleWithScore(boxId, userId, maskViolations, dtFrom, dtTo, ViolationPoints);
            DataSet dsResult = report.Exec_GetViolationsReportForVehicleWithScore_Extended(boxId, userId, maskViolations, dtFrom, dtTo, ViolationPoints);

            if (null != dsResult)
            {
                //report.FillReport(dtFrom, dtTo, dsResult);
                //xmlDataSet = report.result.GetXml();
                xmlDataSet = dsResult.GetXml();
            }
            else
                //Util.BTrace(Util.ERR1, "-- ReportGenerator.GetViolationReportForVehicleWithScore -> ERROR for Exec_GetViolationsReportForVehicleWithScor for fleetId={0}, from={1}, to={2}",
                //            boxId, dtFrom, dtTo);

                Util.BTrace(Util.ERR1, "-- ReportGenerator.GetViolationReportForVehicleWithScore -> ERROR for Exec_GetViolationsReportForVehicleWithScore_Extended for fleetId={0}, from={1}, to={2}",
                            boxId, dtFrom, dtTo);

            return xmlDataSet;
            #endregion

        }

        /// <summary>
        /// GetViolationReportForFleetWithScore_Hierarchy
        /// </summary>
        /// <param name="nodeCode"></param>
        /// <param name="userId"></param>
        /// <param name="maskViolations"></param>
        /// <param name="dtFrom"></param>
        /// <param name="dtTo"></param>
        /// <param name="ViolationPoints"></param>
        /// <returns></returns>
        public string GetViolationReportForFleetWithScore_Hierarchy(string nodeCode, int userId, int maskViolations,
                                    DateTime dtFrom, DateTime dtTo, string ViolationPoints)
        {
            #region Violations Activity Report for Fleet
            string xmlDataSet = "";
            VLF.DAS.DB.ViolationsFleetReport report = new VLF.DAS.DB.ViolationsFleetReport(connectionString);
            DataSet dsResult = report.Exec_GetViolationsReportForFleetWithScore_Hierarchy(nodeCode, userId, maskViolations, dtFrom, dtTo, ViolationPoints);
            if (null != dsResult)
            {
                //report.FillReport(dtFrom, dtTo, dsResult);
                //xmlDataSet = report.result.GetXml();
                xmlDataSet = dsResult.GetXml();
            }
            else
                Util.BTrace(Util.ERR1, "-- ReportGenerator.GetViolationReportForFleetWithScore_Hierarchy -> ERROR for sp_GetViolationsReportForFleetWithScore for nodeCode={0}, from={1}, to={2}",
                            nodeCode, dtFrom, dtTo);

            return xmlDataSet;
            #endregion

        }



        /// <summary>
        /// Get Violation Report For Fleet With Score
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="userId"></param>
        /// <param name="maskViolations"></param>
        /// <param name="dtFrom"></param>
        /// <param name="dtTo"></param>
        /// <param name="ViolationPoints"></param>
        /// <returns></returns>
        public string GetViolationMonthlyData(int fleetId, int userId, int maskViolations,
                                         DateTime dtFrom, DateTime dtTo, string ViolationPoints)
        {
            #region Violations Activity Report for Fleet
            string xmlDataSet = "";
            VLF.DAS.DB.ViolationsFleetReport report = new VLF.DAS.DB.ViolationsFleetReport(connectionString);
            DataSet dsResult = report.ViolationsMonthlyData(fleetId, userId, maskViolations, dtFrom, dtTo, ViolationPoints);
            if (null != dsResult)
            {
                //report.FillReport(dtFrom, dtTo, dsResult);
                //xmlDataSet = report.result.GetXml();
                xmlDataSet = dsResult.GetXml();
            }
            else
                Util.BTrace(Util.ERR1, "-- ReportGenerator.GetViolationMonthlyData -> ERROR for GetViolationMonthlyData for fleetId={0}, from={1}, to={2}",
                            fleetId, dtFrom, dtTo);

            return xmlDataSet;
            #endregion

        }

        /// <summary>
        /// Get Violation Report For Fleet With Score
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="userId"></param>
        /// <param name="maskViolations"></param>
        /// <param name="dtFrom"></param>
        /// <param name="dtTo"></param>
        /// <param name="ViolationPoints"></param>
        /// <returns></returns>
        public string GetViolationReportForFleetWithScore_Special(int fleetId, int userId, int maskViolations,
                                         DateTime dtFrom, DateTime dtTo, string ViolationPoints)
        {
            #region Violations Activity Report for Fleet
            string xmlDataSet = "";
            VLF.DAS.DB.ViolationsFleetReport report = new VLF.DAS.DB.ViolationsFleetReport(connectionString);
            DataSet dsResult = report.Exec_GetViolationsReportForFleetWithScore_Special(fleetId, userId, maskViolations, dtFrom, dtTo, ViolationPoints);
            if (null != dsResult)
            {
                //report.FillReport(dtFrom, dtTo, dsResult);
                //xmlDataSet = report.result.GetXml();
                xmlDataSet = dsResult.GetXml();
            }
            else
                Util.BTrace(Util.ERR1, "-- ReportGenerator.Exec_GetViolationsReportForFleetWithScore_Special -> ERROR for Exec_GetViolationsReportForFleetWithScore_Special for fleetId={0}, from={1}, to={2}",
                            fleetId, dtFrom, dtTo);

            return xmlDataSet;
            #endregion

        }

        /// <summary>
        /// Get driver violation report and localize it
        /// </summary>
        /// <param name="driverId"></param>
        /// <param name="userId"></param>
        /// <param name="maskViolations"></param>
        /// <param name="dtFrom"></param>
        /// <param name="dtTo"></param>
        /// <param name="lang"></param>
        /// <param name="speed"></param>
        /// <returns>xml string dataset</returns>
        public DataSet GetViolationReportForDriverByLang(
           int driverId, int userId, int maskViolations, DateTime dtFrom, DateTime dtTo, string lang, int speed)
        {
            VLF.DAS.DB.ViolationsFleetReport report = new VLF.DAS.DB.ViolationsFleetReport(connectionString);
            DataSet dsResult = report.Exec_GetViolationsReportForDriver(driverId, userId, maskViolations, dtFrom, dtTo, speed);
            if (Util.IsDataSetValid(dsResult))
            {
                if (lang != "en" && lang != null)
                {
                    LocalizationLayer.ServerLocalizationLayer dbl = new LocalizationLayer.ServerLocalizationLayer(connectionString);
                    dbl.LocalizationData(lang, "DescriptionId", "Description", "Violations", ref dsResult);
                }
            }
            else
            {
                Util.BTrace(Util.ERR1, "-- ReportGenerator.GetViolationsReportForDriver -> ERROR for ReportDriverViolations for fleetId={0}, from={1}, to={2}",
                            driverId, dtFrom, dtTo);
                return null;
            }
            return dsResult;
        }


        /// <summary>
        /// Get driver violation report and localize it
        /// </summary>
        /// <param name="driverId"></param>
        /// <param name="userId"></param>
        /// <param name="maskViolations"></param>
        /// <param name="dtFrom"></param>
        /// <param name="dtTo"></param>
        /// <param name="lang"></param>
        /// <param name="speed"></param>
        /// <returns>xml string dataset</returns>
        public DataSet GetViolationReportForDriverByLang_Main(
           int driverId, int userId, int maskViolations, DateTime dtFrom, DateTime dtTo, string lang, int speed)
        {
            VLF.DAS.DB.ViolationsFleetReport report = new VLF.DAS.DB.ViolationsFleetReport(connectionString);
            DataSet dsResult = report.Exec_GetViolationsReportForDriver_Main(driverId, userId, maskViolations, dtFrom, dtTo, speed);
            if (Util.IsDataSetValid(dsResult))
            {
                if (lang != "en" && lang != null)
                {
                    LocalizationLayer.ServerLocalizationLayer dbl = new LocalizationLayer.ServerLocalizationLayer(connectionString);
                    dbl.LocalizationData(lang, "DescriptionId", "Description", "Violations", ref dsResult);
                }
            }
            else
            {
                Util.BTrace(Util.ERR1, "-- ReportGenerator.GetViolationsReportForDriver -> ERROR for ReportDriverViolations for fleetId={0}, from={1}, to={2}",
                            driverId, dtFrom, dtTo);
                return null;
            }
            return dsResult;
        }

        /// <summary>
        /// Get Driver Violation Report With Score
        /// </summary>
        /// <param name="driverId"></param>
        /// <param name="userId"></param>
        /// <param name="maskViolations"></param>
        /// <param name="dtFrom"></param>
        /// <param name="dtTo"></param>
        /// <param name="ViolationPoints"></param>
        /// <returns></returns>
        public DataSet GetViolationReportForDriverWithScore(int driverId, int userId, int maskViolations,
           DateTime dtFrom, DateTime dtTo, string[] ViolationPoints)
        {
            VLF.DAS.DB.ViolationsFleetReport report = new VLF.DAS.DB.ViolationsFleetReport(connectionString);
            DataSet dsResult = report.Exec_GetViolationsReportForDriverWithScore(driverId, userId, maskViolations, dtFrom, dtTo, ViolationPoints);
            if (!Util.IsDataSetValid(dsResult))
            {
                Util.BTrace(Util.ERR1, "-- ReportGenerator.GetViolationReportForDriverWithScore -> ERROR for ReportDriverViolationsSummary for driverId={0}, from={1}, to={2}",
                            driverId, dtFrom, dtTo);
                return null;
            }
            return dsResult;
        }



        /// <summary>
        /// Get Driver Violation Report With Score
        /// </summary>
        /// <param name="driverId"></param>
        /// <param name="userId"></param>
        /// <param name="maskViolations"></param>
        /// <param name="dtFrom"></param>
        /// <param name="dtTo"></param>
        /// <param name="ViolationPoints"></param>
        /// <returns></returns>
        public DataSet GetViolationReportForDriverWithScore_Main(int driverId, int userId, int maskViolations,
           DateTime dtFrom, DateTime dtTo, string[] ViolationPoints)
        {
            VLF.DAS.DB.ViolationsFleetReport report = new VLF.DAS.DB.ViolationsFleetReport(connectionString);
            DataSet dsResult = report.Exec_GetViolationsReportForDriverWithScore_Main(driverId, userId, maskViolations, dtFrom, dtTo, ViolationPoints);
            if (!Util.IsDataSetValid(dsResult))
            {
                Util.BTrace(Util.ERR1, "-- ReportGenerator.GetViolationReportForDriverWithScore -> ERROR for ReportDriverViolationsSummary for driverId={0}, from={1}, to={2}",
                            driverId, dtFrom, dtTo);
                return null;
            }
            return dsResult;
        }

        /// <summary>
        /// GetFuelConsumption
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="dtFrom"></param>
        /// <param name="dtTo"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public DataSet GetFuelConsumption(int fleetId, DateTime dtFrom, DateTime dtTo, int userId)
        {
            VLF.DAS.DB.SensorReport report = new VLF.DAS.DB.SensorReport(connectionString);
            DataSet dsResult = report.GetFuelConsumption(fleetId, dtFrom, dtTo, userId);
            if (!Util.IsDataSetValid(dsResult))
            {
                Util.BTrace(Util.ERR1, "-- ReportGenerator.GetFuelConsumption -> ERROR for GetFuelConsumption for userId={0}, from={1}, to={2}",
                            userId, dtFrom, dtTo);
                return null;
            }
            return dsResult;
        }
        /// <summary>
        /// GetFuelConsumptionPerOrganization
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="dtFrom"></param>
        /// <param name="dtTo"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public DataSet GetFuelConsumptionPerOrganization(int orgId, DateTime dtFrom, DateTime dtTo, int userId)
        {
            VLF.DAS.DB.SensorReport report = new VLF.DAS.DB.SensorReport(connectionString);
            DataSet dsResult = report.GetFuelConsumptionPerOrganization(orgId, dtFrom, dtTo, userId);
            if (!Util.IsDataSetValid(dsResult))
            {
                Util.BTrace(Util.ERR1, "-- ReportGenerator.GetFuelConsumptionPerOrganization -> ERROR for GetFuelConsumptionPerOrganization for userId={0}, from={1}, to={2}",
                            userId, dtFrom, dtTo);
                return null;
            }
            return dsResult;
        }


        /// <summary>
        /// Get Violation Report For Fleet By Lang
        /// </summary>
        /// <param name="nodeCode"></param>
        /// <param name="userId"></param>
        /// <param name="maskViolations"></param>
        /// <param name="dtFrom"></param>
        /// <param name="dtTo"></param>
        /// <param name="lang"></param>
        /// <param name="speed"></param>
        /// <returns></returns>
        public string GetViolationReportForFleetByLang_Hierarchy(string nodeCode, int userId, int maskViolations,
                                                       DateTime dtFrom, DateTime dtTo, string lang, int speed)
        {
            #region Violations Activity Report for Fleet
            string xmlDataSet = "";
            VLF.DAS.DB.ViolationsFleetReport report = new VLF.DAS.DB.ViolationsFleetReport(connectionString);
            DataSet dsResult = report.Exec_GetViolationsReportForFleet_Hierarchy(nodeCode, userId, maskViolations, dtFrom, dtTo, speed);
            if (null != dsResult)
            {
                if (lang != "en" && lang != null)
                {
                    LocalizationLayer.ServerLocalizationLayer dbl = new LocalizationLayer.ServerLocalizationLayer(connectionString);
                    dbl.LocalizationData(lang, "DescriptionId", "Description", "Violations", ref dsResult);
                }
                //report.FillReport(dtFrom, dtTo, dsResult);
                //xmlDataSet = report.result.GetXml();
                xmlDataSet = dsResult.GetXml();
            }
            else
                Util.BTrace(Util.ERR1, "-- ReportGenerator.GetViolationReportForFleetByLang_Hierarchy -> ERROR for sp_GetViolationsReportForFleet_Hierarchy for fleetId={0}, from={1}, to={2}",
                            nodeCode, dtFrom, dtTo);

            return xmlDataSet;
            #endregion

        }



    }
}
