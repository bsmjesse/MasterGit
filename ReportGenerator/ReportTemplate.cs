using System;

namespace VLF.Reports
{
   /// <summary>
   /// Defines parameters for all reports
   /// </summary>
   public class ReportTemplate
   {
      #region Waypoint Report Parameters
      public const string RpWaypointsTableName = "vlfMsgInHst";
      public const string RpWaypointsFirstParamName = "LicensePlate";
      public const string RpWaypointsSecondParamName = "FromDateTime";
      public const string RpWaypointsThirdParamName = "ToDateTime";
      #endregion

      #region Detailed Trip Report Parameters
      public const string RpDetailedTripFirstParamName = "LicensePlate";
      public const string RpDetailedTripSecondParamName = "FromDateTime";
      public const string RpDetailedTripThirdParamName = "ToDateTime";
      public const string RpDetailedTripFourthParamName = "IncludeStreetAddress";
      public const string RpDetailedTripFifthParamName = "IncludeSensors";
      public const string RpDetailedTripSixthParamName = "IncludePosition";
      public const string RpDetailedTripSeventhParamName = "IncludeIdleTime";
      public const string RpDetailedTripEighthParamName = "IncludeSummary";
      public const string RpDetailedTripNinthParamName = "ShowStoredPosition";
      public const string RpDetailedTripTenthParamName = "EndTripSensor";
      #endregion

      #region Fleet Detailed Trip Report Parameters
      public const string RpFleetDetailedTripFirstParamName = RpDetailedTripFirstParamName;
      public const string RpFleetDetailedTripSecondParamName = RpDetailedTripSecondParamName;
      public const string RpFleetDetailedTripThirdParamName = RpDetailedTripThirdParamName;
      public const string RpFleetDetailedTripFourthParamName = RpDetailedTripFourthParamName;
      public const string RpFleetDetailedTripFifthParamName = RpDetailedTripFifthParamName;
      public const string RpFleetDetailedTripSixthParamName = RpDetailedTripSixthParamName;
      public const string RpFleetDetailedTripSeventhParamName = RpDetailedTripSeventhParamName;
      public const string RpFleetDetailedTripEighthParamName = RpDetailedTripEighthParamName;
      public const string RpFleetDetailedTripNinthParamName = RpDetailedTripNinthParamName;
      public const string RpFleetDetailedTripTenthParamName = RpDetailedTripTenthParamName;
      #endregion

      #region Trip Report Parameters
      public const string RpTripFirstParamName = RpDetailedTripFirstParamName;
      public const string RpTripSecondParamName = RpDetailedTripSecondParamName;
      public const string RpTripThirdParamName = RpDetailedTripThirdParamName;
      public const string RpTripFourthParamName = RpDetailedTripNinthParamName;
      public const string RpTripFifthParamName = RpDetailedTripTenthParamName;
      #endregion

      #region Fleet Trip Report Parameters
      public const string RpFleetTripFirstParamName = "FleetId";
      public const string RpFleetTripSecondParamName = RpDetailedTripSecondParamName;
      public const string RpFleetTripThirdParamName = RpDetailedTripThirdParamName;
      public const string RpFleetTripFourthParamName = RpDetailedTripNinthParamName;
      public const string RpFleetTripFifthParamName = RpDetailedTripTenthParamName;
      #endregion

      #region Fleet Stop Report Parameters
      public const string RpFleetStopFirstParamName = "FleetId";
      public const string RpFleetStopSecondParamName = RpDetailedTripSecondParamName;
      public const string RpFleetStopThirdParamName = RpDetailedTripThirdParamName;
      public const string RpFleetStopFourthParamName = RpDetailedTripNinthParamName;
      public const string RpFleetStopFifthParamName = RpStopFifthParamName;
      public const string RpFleetStopSixthParamName = RpStopSixthParamName;
      public const string RpFleetStopSeventhParamName = RpStopSeventhParamName;
      public const string RpFleetStopEighthParamName = RpDetailedTripTenthParamName;
      #endregion

      #region Alarm Report Parameters
      public const string RpAlarmFirstParamName = RpDetailedTripFirstParamName;
      public const string RpAlarmSecondParamName = RpDetailedTripSecondParamName;
      public const string RpAlarmThirdParamName = RpDetailedTripThirdParamName;
      #endregion

      #region Fleet Alarms Report Parameters
      public const string RpFleetAlarmsFirstParamName = "FleetId";
      public const string RpFleetAlarmsSecondParamName = RpDetailedTripSecondParamName;
      public const string RpFleetAlarmsThirdParamName = RpDetailedTripThirdParamName;
      #endregion

      #region Organization System Usage Report Parameters
      public const string RpOrganizationSystemUsageFirstParamName = "OrganizationId";
      public const string RpOrganizationSystemUsageSecondParamName = RpDetailedTripSecondParamName;
      public const string RpOrganizationSystemUsageThirdParamName = RpDetailedTripThirdParamName;
      public const string RpOrganizationSystemUsageFourthParamName = "ShowExceptionOnly";
      #endregion

      #region System Usage Exception Report For All Organizations Parameters
      public const string RpSystemUsageExceptionReportForAllOrganizationsFirstParamName = RpDetailedTripSecondParamName;
      public const string RpSystemUsageExceptionReportForAllOrganizationsSecondParamName = RpDetailedTripThirdParamName;
      #endregion

      #region Box System Usage Report Parameters
      public const string RpBoxSystemUsageFirstParamName = "BoxId";
      public const string RpBoxSystemUsageSecondParamName = RpDetailedTripSecondParamName;
      public const string RpBoxSystemUsageThirdParamName = RpDetailedTripThirdParamName;
      public const string RpBoxSystemUsageFourthParamName = "ShowExceptionOnly";
      #endregion

      #region Stop Report Parameters
      public const string RpStopFirstParamName = RpDetailedTripFirstParamName;
      public const string RpStopSecondParamName = RpDetailedTripSecondParamName;
      public const string RpStopThirdParamName = RpDetailedTripThirdParamName;
      public const string RpStopFourthParamName = RpDetailedTripNinthParamName;
      public const string RpStopFifthParamName = "MinStopDuration";
      public const string RpStopSixthParamName = "IncludeStop";
      public const string RpStopSeventhParamName = "IncludeIdle";
      public const string RpStopEighthParamName = RpDetailedTripTenthParamName;
      #endregion

      #region Exception Report Parameters
      public const string RpExceptionFirstParamName = RpDetailedTripFirstParamName;
      public const string RpExceptionSecondParamName = RpDetailedTripSecondParamName;
      public const string RpExceptionThirdParamName = RpDetailedTripThirdParamName;
      public const string RpExceptionFourthParamName = "SosLimit";
      public const string RpExceptionFifthParamName = "NoDoorSnsHrs";
      public const string RpExceptionSixthParamName = "IncludeTar";
      public const string RpExceptionSeventhParamName = "IncludeMobilize";
      public const string RpExceptionEightParamName = "15SecDoorSns";
      public const string RpExceptionNineParamName = "50ofLeash";
      public const string RpExceptionTenParamName = "MainAndBackupBatterySns";
      public const string RpExceptionElevenParamName = "TamperSns";
      public const string RpExceptionTwelveParamName = "AnyPanicSns";
      public const string RpExceptionThirteenParamName = "3KeypadAttemptsSns";
      public const string RpExceptionFourteenParamName = "AltGPSAntennaSns";
      public const string RpExceptionFifteenParamName = "ControllerStatus";
      public const string RpExceptionSixteenParamName = "LeashBrokenSns";
      public const string RpExceptionSeventeenParamName = "DriverDoor";
      public const string RpExceptionEighteenParamName = "PassengerDoor";
      public const string RpExceptionNineteenParamName = "SideHopperDoor";
      public const string RpExceptionTwentyParamName = "RearHopperDoor";
      public const string RpExceptionTwentyFirstParamName = "IncludeCurrentTar";

      public const string RpExceptionTwentySecondParamName = "Locker1";
      public const string RpExceptionTwentyThirdParamName = "Locker2";
      public const string RpExceptionTwentyFourthParamName = "Locker3";
      public const string RpExceptionTwentyFifthParamName = "Locker4";
      public const string RpExceptionTwentySixthParamName = "Locker5";
      public const string RpExceptionTwentySeventParamName = "Locker6";
      public const string RpExceptionTwentyEightParamName = "Locker7";
      public const string RpExceptionTwentyNineParamName = "Locker8";
      public const string RpExceptionThirtyParamName = "Locker9";
      #endregion

      #region Fleet Exception Report Parameters
      public const string RpFleetExceptionFirstParamName = "FleetId";
      public const string RpFleetExceptionSecondParamName = RpDetailedTripSecondParamName;
      public const string RpFleetExceptionThirdParamName = RpDetailedTripThirdParamName;
      public const string RpFleetExceptionFourthParamName = RpExceptionFourthParamName;
      public const string RpFleetExceptionFifthParamName = RpExceptionFifthParamName;
      public const string RpFleetExceptionSixthParamName = RpExceptionSixthParamName;
      public const string RpFleetExceptionSeventhParamName = RpExceptionSeventhParamName;
      public const string RpFleetExceptionEightParamName = RpExceptionEightParamName;
      public const string RpFleetExceptionNineParamName = RpExceptionNineParamName;
      public const string RpFleetExceptionTenParamName = "MainAndBackupBatterySns";
      public const string RpFleetExceptionElevenParamName = "TamperSns";
      public const string RpFleetExceptionTwelveParamName = "AnyPanicSns";
      public const string RpFleetExceptionThirteenParamName = "3KeypadAttemptsSns";
      public const string RpFleetExceptionFourteenParamName = "AltGPSAntennaSns";
      public const string RpFleetExceptionFifteenParamName = "ControllerStatus";
      public const string RpFleetExceptionSixteenParamName = "LeashBrokenSns";
      public const string RpFleetExceptionSeventeenParamName = "DriverDoor";
      public const string RpFleetExceptionEighteenParamName = "PassengerDoor";
      public const string RpFleetExceptionNineteenParamName = "SideHopperDoor";
      public const string RpFleetExceptionTwentyParamName = "RearHopperDoor";
      public const string RpFleetExceptionTwentyFirstParamName = RpExceptionTwentyFirstParamName;

      public const string RpFleetExceptionTwentySecondParamName = "Locker1";
      public const string RpFleetExceptionTwentyThirdParamName = "Locker2";
      public const string RpFleetExceptionTwentyFourthParamName = "Locker3";
      public const string RpFleetExceptionTwentyFifthParamName = "Locker4";
      public const string RpFleetExceptionTwentySixthParamName = "Locker5";
      public const string RpFleetExceptionTwentySeventParamName = "Locker6";
      public const string RpFleetExceptionTwentyEightParamName = "Locker7";
      public const string RpFleetExceptionTwentyNineParamName = "Locker8";
      public const string RpFleetExceptionThirtyParamName = "Locker9";
      #endregion

      #region Vehicle Latency Report Parameters
      public const string RpLatencyFirstParamName = "OrganizationId";
      public const string RpLatencySecondParamName = "FleetId";
      public const string RpLatencyThirdParamName = "VehicleId";
      public const string RpLatencyFourthParamName = RpDetailedTripSecondParamName;
      public const string RpLatencyFifthParamName = RpDetailedTripThirdParamName;
      public const string RpLatencySixthParamName = "CommModes";
      #endregion

      #region Off Hour Report Parameters
      public const string RpOffHourFirstParamName = RpDetailedTripFirstParamName;
      public const string RpOffHourSecondParamName = RpDetailedTripSecondParamName;
      public const string RpOffHourThirdParamName = RpDetailedTripThirdParamName;
      public const string RpOffHourFourthParamName = RpDetailedTripNinthParamName;
      public const string RpOffHourFifthParamName = "DayFromHour";
      public const string RpOffHourSixthParamName = "DayFromMin";
      public const string RpOffHourSeventhParamName = "DayToHour";
      public const string RpOffHourEightParamName = "DayToMin";
      public const string RpOffHourNineParamName = "WeekendFromHour";
      public const string RpOffHourTenParamName = "WeekendFromMin";
      public const string RpOffHourElevenParamName = "WeekendToHour";
      public const string RpOffHourTwelveParamName = "WeekendToMin";
      #endregion

      #region Fleet Off Hour Report Parameters
      public const string RpFleetOffHourFirstParamName = "FleetId";
      #endregion

      #region Fleet Maintenance Report Parameters
      public const string RpFleetMaintenanceFirstParamName = "FleetId";
      #endregion

      #region Messages Report Parameters
      public const string RpMessagesFirstParamName = "FromDateTime";
      public const string RpMessagesSecondParamName = "ToDateTime";
      public const string RpMessagesThirdParamName = "FleetID";
      public const string RpMessagesFourthParamName = "FleetName";
      public const string RpMessagesFifthParamName = "BoxId";
      public const string RpMessagesSixthParamName = "VehicleName ";

      #endregion

      # region Common Report Parameters

      public const string RptParamFleetId = "FleetId";
      public const string RptParamFromDateTime = "FromDateTime";
      public const string RptParamToDateTime = "ToDateTime";
      public const string RptParamLicensePlate = "LicensePlate";
      public const string RptParamLandmark = "LM";
      public const string RptParamGeozone = "GZ";
      public const string RptParamSensorId = "SENSOR_NUM";
      public const string RptParamDriverId = "DRV";
      public const string RptParamIncludeSummary = "Summary";
      public const string RptParamCost = "Cost";
      public const string RptParamVehicleId = "VID";
       

      # endregion

      public enum ReportTypes
      {
         WaypointsReport = 0,
         DetailedTrip = 1,
         Trip = 2,
         TripActivity = 3,
         FleetDetailedTrip = 4,
         FleetTrip = 5,
         Alarm = 6,
         FleetAlarms = 7,
         OrganizationSystemUsage = 8,
         Stop = 9,
         FleetStop = 10,
         BoxSystemUsage = 11,
         Exception = 12,
         HistoryReport = 13,
         MessagesReport = 14,
         FleetException = 15,
         SystemUsageExceptionReportForAllOrganizations = 16,
         Latency = 17,
         StopByLandmark = 18,
         FleetStopByLandmark = 19,
         OffHoursReport = 20,
         FleetOffHoursReport = 21,
         FleetMaintenanceReport = 22,
         SensorForVehicleReport = 23,
         SensorForFleetReport = 24,
      }

      public static string MakePair(string key, string val)
      {
         return key + "=" + val + ";";
      }

      /// <summary>
      /// Finds value of specified key in the given string
      /// string may contain other records as well;
      /// string format should be:
      /// [key]=[value];
      /// </summary>
      /// <param name="key">key to look for</param>
      /// <param name="src"></param>
      /// <returns></returns>
      public static string PairFindValue(string key, string src)
      {
         int val_start, val_end;
         int key_pos = src.IndexOf(key);

         if (key_pos != -1)
         {
            val_start = src.IndexOf("=", key_pos);
            if (key_pos != -1)
            {
               val_end = src.IndexOf(";", val_start);

               if (val_end != -1)
                  return src.Substring(val_start + 1, val_end - val_start - 1);
            }
         }
         return "";
      }

      public enum ExceptionReportGroup
      {
         SosMode = 0,
         NoDoorOpenedClosed,
         TarMode,
         Immobilization
      }

   }
}
