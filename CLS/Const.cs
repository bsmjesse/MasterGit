using System;

namespace VLF.CLS.Def
{
	public class Const
	{
		#region E-MAIL Section
		// SMTP server for sending outgoing e-mail messages
		public const string defaultPOP3Server = "205.205.121.105" ;
		public const string defaultSMTPServer = "mail.ca.mci.com" ;
		public const int defaultPOP3Port = 110;
		public const int defaultSMTPPort = 25;
		#endregion

		#region DAS Section
		/// The value of this constant is equivalent to 00:00:00.0000000, January 1, 0001
		public static readonly DateTime unassignedDateTime = DateTime.MinValue;
		public const int unassignedIntValue		= -1 ;
		public const short unassignedShortValue = -1 ;
		public const string unassignedStrValue	= null;
		
		public const int invalidBoxID = unassignedIntValue;
		public const string invalidLicensePlate = unassignedStrValue;
		public const int violatedIntegrityMaxRetries	= 10 ; // maximal number of attemts to insert/update information
		public const int nextDateTimeMillisecInterval	= 10; // milliseconds interval for next datetime key itteration.
		public const string addressNA			    = "Address resolution in progress";
        public const string noGPSData = "-----";
        public const string noValidAddress = "-------";
		public const string blankValue			 = "-----";
		public const short defaultMsgInTypeLen	 = 0;
		public const short defaultCmdOutTypeLen = 0;
		public const short udpHeaderLen			 = 0;
		
		public const short MsgInDirectionValue	 = 0;
		public const string MsgInDirectionSign	 = "<<";
		public const short MsgOutDirectionValue = 1;
		public const string MsgOutDirectionSign = ">>";

		public const string cmdAck				= "Yes";
		public const string cmdNotAck			= "No";
		public const string cmdNotApplicable	= " ";

		public const short resolveLandmarksYes	= 1;
		public const short resolveLandmarksNo	= 0;
		public const string defaultFleetName	= "All Vehicles";
		public const double kilometersInMile	= 1.609344 ;
		public const double milesInKilometer	= 0.6214 ;
		
		public const string txtMsgAcknowledged	= "Yes";
		public const string txtMsgTimedOut		= "No";
		public const string txtMsgAckNA			= "N/A";
		public const string defNA			      = "N/A";
		
		public const string unknownGeozoneName	= "Unknown";
		
		#endregion DAS Section

		#region Custom Properties and Command Params

		public const string unknownPrefix   = "UNKNOWN" ;
		public const string freePrefix      = "FREE" ;
		public const string errorPrefix     = "ERROR" ;

		// All devices
        public const string keyDeviceID = "DeviceID";
        public const string keyDeviceStatus = "DeviceStatus";

		// MFCM
		public const string keyServiceRequired = "ServiceRequired";
		public const string keyIdleTime = "IdleTime";
		public const string keyRecordsCount = "RecordsCount";

		// AirLink PinPoint
		public const string keyNackReason = "NackReason";
		public const string keyOldOdometerValue = "OldOdomValue";
		public const string keyOdometerValue = "Odometer";
		public const string setupDistanceInterval = "DistInt";
		public const string setupGPSReportType = "ReportType";
		public const string setupGPSFrequencyStationary = "GPSFreqStat";
		public const string setupStoreAndForwardBehavior = "S&FBehavior";
		public const string setupStoreAndForwardGroupingThreshold = "S&FGroupThreshold";
		public const string setupStoreAndForwardDeliveryMethod = "S&FDelivMethod";
		public const string setupStoreAndForward = "S&F";
		public const string setupOdometer = "Odom";
		public const string setupDataBits = "DataBits";

		// SkyWave modems
		public const string keyPayload = "PayLoad";
		public const string keyMessageDescriptor="MessageDescriptor";
		public const string keyDestinationAddress = "DestinationAddress";

		public const string keyFirmwareIdOld = "FW_Old" ;
		public const string keyFirmwareIdNew = "FW_New" ;
		public const string keyFirmwareUpdateStatus = "FWUpdateStatus";
		public const string keyFirmwareFileName = "FWFileName";
		public const string keyFenceNo = "FENCE_NO" ;
		public const string keyFenceDir = "FENCE_DIR" ;
		public const string keyFenceLatN = "FENCE_LATN" ;
		public const string keyFenceLatS = "FENCE_LATS" ;
		public const string keyFenceLonE = "FENCE_LONE" ;
		public const string keyFenceLonW = "FENCE_LONW" ;
		public const string keyImageIndex = "ImageIndex";
		public const string keySensorNum = "SENSOR_NUM" ;
		public const string keySensorStatus = "SENSOR_STATUS" ;
		public const string valFenceDirIn  = "FENCE_IN" ;
		public const string valFenceDirOut = "FENCE_OUT" ;
		public const string keyArmed = "ARMED" ;
		public const string keyCamera = "CAMERA" ;
		public const string keyMainBattery = "Main Battery" ;
		public const string keyBackupBattery = "Backup Battery" ;
		public const string keySerialNumber = "Box Serial Number" ;
		public const string keyFirmwareVersion = "Firmware Version" ;
		public const string keyWaypointsUsage = "Waypoints Usage" ;
      public const string keyMDTNumMessages = "MDT messages"; 
		public const string keySIM = "SIM";
		public const string keyBoxID = "BoxID";
		public const string keyCell = "Cell";
		public const string keyAlarmMsg = "ALARM_MSG" ;
		public const string keyAlarmNum = "SENSOR_NUM" ;
		public const string keyBadSensorNum = "BAD_SENSOR_NUM" ;
		public const string keyBattery = "BATTERY" ;
		public const string keyBatteryStatus = "BATTERY_STATUS" ;
		public const string keyTemperature = "TEMPERATURE" ;
		public const string keyExternalPower = "EXTERNAL_POWER" ;
		public const string keyReportInterval = "REPORT_INTERVAL" ;
      public const string keyNumberReports = "NUMBER_REPORTS";
		public const string keyIdleDuration = "IDLE_DURATION" ;
      public const string keyDuration = "DURATION" ;              // gb used for braking/accelration/seat belt
      public const string keySensorDuration = "SENSOR_DURATION";  // (gb) used for engine hours
		public const string keySpeedDuration = "Speed_DURATION" ;
		public const string keyGPSAntenna = "GPSAntenna";
		public const string keyVCRDelayTime = "VCRDelay";
		public const string keyKeyFobStatus = "KeyFobStatus";
		public const string keyTARMode = "TARMode";
		public const string keyServerIP = "ServerIP";
		public const string keyServerPort = "ServerPort";
		public const string keyPowerDownMode = "PowerDownMode";
		public const string keySleepModeReportingInterval = "SMRI";
		public const string keyWakeUpInterval = "WUI";
		public const string keySleepModeDelay = "SMD";
      public const string keyAnalogSensor1Value = "Analog1";
      public const string keyAnalogSensor2Value = "Analog2";
      public const string keyAnalogSensor3Value = "Analog3";
      public const string keyIPUpdateReason = "IPReason";
      public const string keyParamValue = "ParamValue";

    //Report Params
      public const string keyLicensePlate = "LicensePlate";          // license plate
      public const string keySummary = "Summary";                    // indicates if is summary or details for a report
      public const string keyVehicleId = "VID";                      // vehicle ID, used in scheduler report
      public const string keyCost = "Cost";                          // numeric value used in some reports, corelated with another colum Total cost = Cost * value_column_X
      public const string keyLandmarkName = "LM";                    //Landmark Name



      // Blue Tree Device
      public const string keyPowerOffDelay = "PowerOff";
       public const string keyGeneralParameter = "GeneralParam";
       public const string keyEventID = "EventID";

		// 
		public const string keyNumIgnoredMessages = "NumIgnoredMess";
		public const string keyIgnoredMessagesLength = "IgnoredMessLength";
		public const string keyStartedTime = "StartedTime";
		public const string keyMessageType = "MessageType";
		public const string keySilenceTime = "SilenceTime";
		public const string keyPower = "Power";
		public const string keyNumAllowedMessages = "NumAllowedMess";
		public const string keyNumDaysCheck = "NumDaysCheck";

		// Message ID for MicroNet
		public const string keyMID = "MID" ;
		public const string keyMessage = "M" ;
		public const string keyAnswer = "A" ;
      public const string keyMDTMajorVersion = "mdtHigh";
      public const string keyMDTMinorVersion = "mdtLow";
      public const string keyPeripheralType = "pertype" ;
      public const string keyMDTType = "mdttype";
      public const string keyPeripheralId = "pid" ;

		public const string valON = "ON" ;
		public const string valOFF = "OFF" ;
		public const string keyDisabled = "Disabled";
		public const string keyEnabled = "Enabled";
		public const string keyConstantly = "Constantly";

		public const string keyGeozoneId = "GZ" ;
		public const string keyGeozoneDir = "GZ_DIR" ;
		public const string keyGeozoneType = "GZ_TYPE" ;
		public const string keyGeozoneLatCenter = "GZ_LAT_CENTER" ;
		public const string keyGeozoneLonCenter = "GZ_LON_CENTER" ;
		public const string keyGeozoneWidth = "GZ_WIDTH" ;
		public const string keyGeozoneHeight = "GZ_HEIGHT" ;
		public const string keyGeozoneSeverity = "GZ_SEVERITY" ;
		public const string keyGeozoneLatTop = "GZ_LAT_TOP" ;
		public const string keyGeozoneLonTop = "GZ_LON_TOP" ;
		public const string keyGeozoneLatBottom = "GZ_LAT_BOTTOM" ;
		public const string keyGeozoneLonBottom = "GZ_LON_BOTTOM" ;
		public const string keyGeozoneLat = "GZ_LAT_" ;
		public const string keyGeozoneLon = "GZ_LON_" ;
		public const ushort allGeozones = 0xFFFF;
		public const short maxGeozones = 50;
        public const string keyGeozoneSpeed = "GZ_SPEED";  // km/h
        public const string keyGeozoneSpeedingDuration = "GZ_SPEEDING_DURATION";  // seconds

        public const string keyDallasKey = "Dallas";
		// For Box Setup
		public const string setupPhoneNumber = "PhoneNumber";
		public const string setupIPAddress = "IPAddress";
		public const string setupPort = "Port";
		public const string setupGPSFrequency = "GPSFrequency";
		public const string setupCommMode = "CommMode";
		public const string setupSpeedThreshold = "Speed";
		public const string setupTracePeriod = "TracePeriod";
		public const string setupTraceInterval = "TraceInterval";
		public const string setupTraceStates = "TraceStates";
		public const string setupGeoFenceRadius = "GeoFence";
		public const string setupSensorsMask = "SensorsMask";
		public const string setupExtendedSensorsMask = "ExtendedSensorsMask";
      public const string setupChannelSetup = "ChannelSetup";
      public const string setupChannelID = "ChannelID";
      
      // for Bantek, ChannelSetup
      public const string setupBoxChannel = "BoxSetup";

      // for Iridium new commands - gb 2007/12/14
      public const string setupIridiumMessageMask  = "IrdMsgMask";
      public const string setupIridiumSensorMask   = "IrdSnsMask";
      public const string setupIridiumSensorAlarmMask = "IrdSnsAlarmMask";
      public const string setupIridiumInitDelay = "IrdInitDelay";
      public const string setupUpdateMultiplier = "IrdMultiplier";

      // for SetPowerSave configuration new commands - gb 2007/12/14
      public const string setPowerCfgSleepTime = "PowerCfgSleepTime";
      public const string setMDTSleepTime = "PowerCfgMDTTime";
      public const string setModemTurnOffTime = "PowerCfgModemTime";
      public const string setPeriodicWakeupTime = "PowerCfgPeriodicWakeupTime";

        // extended setup fields.
        public const string setupHarshAcceleration  = "HarshAcceleration";
        public const string setupHarshBraking       = "HarshBraking";
        public const string setupExtremeAcceleration= "ExtremeAcceleration";
        public const string setupExtremeBraking     = "ExtremeBraking";
        
        // EEPROM
        public const string keyEEPROMOffset       = "EEPROMOffset";   // XS write / read EEPROM
        public const string keyEEPROMLength       = "EEPROMLength";   // XS write / read EEPROM
        public const string keyEEPROMData         = "EEPROMData";     // XS write / read EEPROM

        //WI-FI UPGRADE
        public const string KEY_WIFI_UPGRADE_FILE_TYPE = "FileType";     
        public const string KEY_WIFI_UPGRADE_FILE_PATH= "FileName";

        public const string KEY_WIFI_UPGRADE_COMMAND_CODE = "CODE";
        public const string KEY_WIFI_UPGRADE_COMMAND_OUTPUT = "OUTPUT";
        public const string KEY_WIFI_SETTING_ENABLE = "ENABLE";
        public const string KEY_WIFI_SETTING_ESSID = "ESSID";
        public const string KEY_WIFI_SETTING_CHANNEL = "CHANNEL";
        public const string KEY_WIFI_SETTING_MODE = "MODE";
        public const string KEY_WIFI_SETTING_SECURITY = "SECU";
        public const string KEY_WIFI_SETTING_WIFIKEY = "KEY";
        public const string KEY_WIFI_SETTING_CIPHER = "CIPHER";   
    
        // XS Ack Status
        public const string keyAckStatus            = "AckStatus";

        // Key fob setup
        public const string keyRemoteControlSettings = "RemoteControlSettings";

        // Key fob setup
        public const string keyPayloadID = "PID";

      // External data
      public const string extVoiceMessage = "Voice Message";
		public const string extSecurityCode = "SC";
		public const string extFacilityCode = "FC";
		public const string extIDNumber = "IDN";
		public const string extControllerVersion = "ControllerVersion";

		public const string keyLatitude = "Latitude";
		public const string keyLongitude = "Longitude";
		public const string keyAltitude = "Altitude";
		public const string keyHeading = "Heading";
		public const string keySpeed = "Speed";
		public const string keyOriginTime = "Time";
		public const string keyUnitName = "UnitName";
      public const string keySensorNotInUse = "Unused";
		public const string keyPRLNumber = "PRL";

      public const string keyEngineHours = "EngineHours";
      public const string keyBoxSleepTimeValue = "BoxSleepTimeValue";
      public const string keyActiveChannel = "Channel";
      public const string keyCheckSum = "CK";      // for KeepAliveDual
      public const string keySMC = "KEYSMC";
      public const string keyFuelConsumption = "FUEL";
      public const string keyTetheredState = "Tethered";
      public const string keyDTCSource = "DTCSource";    // 0 - none, 1 - J1708, 2 - OBD2
      public const string keyDTCSrc_J1708a = "J1708a";   // J1708 generic
      public const string keyDTCSrc_J1708 = "J1708";     // J1708
      public const string keyDTCSrc_OBD2  = "OBD2";      // OBD2
      public const string keyDTCInVehicle = "DTCInVehicle";
      public const string keyDTCInPacket = "DTCCnt";
      public const string keyDTC = "DTC_";
      public const string keyMIL = "MIL";
      public const string keyJ1708_MID = "MID";    // Message Identification --> present
      public const string keyJ1708_PID = "PID";    // Parameter Identification --> interchangeable with SID
      public const string keyJ1708_SID = "SID";    // Parameter Identification --> interchangeable with PID
      public const string keyJ1708_FAULT = "FAULT";// present
      public const string keyJ1708_FMI = "FMI";    // Failure Mode Identifier  --> present
      public const string keyJ1708_CNT = "CNT";    // counter                  --> 

      public const string keyTestId = "Test";         // used for StartDeviceTest command
      public const string keyTestParam = "TestParam";
      public const string keySMCCodes = "SmcCodes";
      public const string keyTrailerWakeTime = "WakeTime";
      public const string keyTrailerSleepTime = "SleepTime";


      // OTA for MDTs
      public const string keyMDTUpgradeType = "UPG";            // Upgrade type (REQUIRED)  1=Firmware, 2=Forms
      public const string keyMDTUpgradeFirmwareVersion = "FW";  // Firmware version (i.e. 3.6c) (OPTIONAL – default `CURRENT`)
      public const string keyMDTUpgradeFormsetId = "FSID";      // Formset identifier for form definition folder resolution (REQUIRED)
      public const string keyMDTUpgradeBlankInMotion = "BIM";   // MDT Blank in motion mode (OPTIONAL – default 0)   0=Off, 1=On
      public const string keyMDTUpgradeDaylightSaving = "DLS";  // MDT Daylight savings mode (OPTIONAL – default 0)  0=Off, 1=On
      public const string keyMDTUpgradeTitle = "TITLE";         // MDT Title value (OPTIONAL – default `BSM Wireless`) Company Header i.e. – Koch Transport
      public const string keyMDTUpgradeWipeFormsMemory = "WIPE"; // Clear MDT forms memory (OPTIONAL – default 0)  0=No, 1=Yes
      public const string keyMDTUpgradeOverwriteSettings = "OVR";// Overwrite MDT settings where 0=No, 1=Yes (OPTIONAL – default 0)
                                                                 // 1 will overwrite MDT configuration values with items contained in TITLE, DLS, BIM key-value pairs
      public const string keyMDTUpgradeBytesIn = "BYTESIN";           // Upgrade service process bytes received from MDT
      public const string keyMDTUpgradeBytesOut = "BYTESOUT";         // Upgrade service process bytes sent to MDT
      public const string keyMDTUpgradeResult = "RESULT";             // Upgrade service process result


      // XS application
      public const string keyAppID            = "AppID";
      public const string keyAppTypeID        = "AppTypeID";
      public const string keyAppFeatureMask   = "AppFeatureMask";
      public const string keyVirtualSensorConfig  = "VirtualSensorConfig";
      public const string keyVirtualOutputConfig  = "VirtualOutputConfig";

        #region Reefer, AVLXS Protocol 9 - V1.5
        // Reefer Temperature Threshold  Setup
        public const string keyReeferTempZoneEnableMask = "ReeferTempZoneEnableMask";
        public const string keyReeferLowerThresholdOfTempZone = "ReeferLowerThresholdOfTempZone";
        public const string keyReeferUpperThresholdOfTempZone = "ReeferUpperThresholdOfTempZone";
        // Reefer Fuel Threshold  Setup
        public const string keyReeferFuelEnableMask = "ReeferFuelEnableMask";
        public const string keyReeferLowerThresholdOfFuel = "ReeferLowerThresholdOfFuel";
        public const string keyReeferUpperThresholdOfFuel = "ReeferUpperThresholdOfFuel";
        // Reefer Sensors Enable Mask
        public const string keyReeferSensorsEnableMask = "ReeferSensorsEnableMask";
        // Reefer Reporting Interval
        public const string keyReeferReportingInterval = "ReeferReportingInterval";
        // Feature Mask
        public const string keyReeferFeatureMask = "ReeferFeatureMask";
        // Reefer Sensors Status
        public const string keyReeferSensorsStatus = "ReeferSensorsStatus";
        // Reefer Temperature Zone  Status
        public const string keyReeferTempOfZone = "ReeferTempOfZone";
        // Reefer Fuel Status
        public const string keyReeferFuelStatus = "ReeferFuelStatus";
        // Reefer Temperature Zone  Alarm
        public const string keyReeferTempZoneNumber = "ReeferTempZoneNumber";
        public const string keyReeferTempZoneAlarmType = "ReeferTempZoneAlarmType";
        public const string valInRange = "InRange";
        public const string valOutOfRange = "OutOfRange";
        // Reefer Fuel Alarm
        public const string keyReeferFuelAlarmType = "ReeferFuelAlarmType";
        // Reefer Sensor Alarm
        public const string keyReeferSensorNumber = "ReeferSensorNumber";
        public const string keyReeferSensorStatus = "ReeferSensorStatus";
        // Reefer Wrapper Mask, wrapper id = bit offset + 1, e.g. wrapper id 1 is at bit 0.
        public const string keyReeferWrapperMask = "ReeferWrapperMask";
        public const string keyReeferWrapperID = "ReeferWrapperID";

      // Reefer Temperature Zone Time Interval, followed by temp. zone number: 1,2,3,4,5
      public const string keyReeferTempZoneTime = "ReeferTempZoneTime";
      public const string keyReeferTempZoneTimeMask = "ReeferTempZoneTimeMask";
        public const string keyReeferFirmwareVersion = "ReeferFirmwareVersion";
        public const string keyReeferMainBatteryVoltage = "ReeferMainBatteryVoltage";
        // Reefer Fuel Level Rise/Drop Alarm/Setup
        public const string keyReeferFuelLevelRiseDropAlarmType = "ReeferFuelLevelRiseDropAlarmType";
        public const string valDrop = "Drop";
        public const string valRise = "Rise";
        public const string keyReeferFuelLevelRiseDropMask      = "ReeferFuelLevelRiseDropMask";
        public const string keyReeferFuelLevelChange            = "ReeferFuelLevelChange";
        public const string keyReeferFuelLevelDurationOfChange  = "ReeferFuelLevelDurationOfChange";
      
      public static string[] STATUS_SENSORS = new string[] { "Standby Power", "Auto Start", "Cool", "Defrost", "Heat", "Out Of Range", "Fault Sensor" };

        #endregion

        #endregion  Custom Properties and Command Params

      #region DCL Section
        // timeout for checking new outgoing messages in DB (msec)
		public const int dclCheckDBInterval = 50 ;
		// acm waits heartbeat value seconds to check all controlled DCLs 
		public const int acmHeartBeat = 30 ;
		// Convertion from miles to km
		public const double kmPerMiles = 1.609344;

      public const int NET_REMOTING_BASE_PORT = 9200;

		#endregion

		#region PAS Section
		// max number of stored messages in memory for G5 boxes
		public const int pasG5MaxMemory = 24564 ;
		// max number of stored messages in memory for G4 boxes
		public const int pasG4MaxMemory = 12282 ;
      public const double knotsToKm = 1.85200;
      public const double kmToKnots = 0.539956803;
      public const string atPortBT = "6070";
      #endregion

		#region MQS Section
		public const int mqsThreadDelay = 50 ;
		#endregion MQS Section

		#region DCS Section
		public const int dcsSocketNumber = 8;
		#endregion DCS Section

		#region CPU Section
		public const string Bg_Off_Color = "#93bee2";//"#336699";
		public const string Bg_On_Color = "#DBEAF5";
		public const string Bg_Color = "#104A7B";////"#104a7b";
		#endregion DCS Section

		#region ASI Section
		public const int aslServerUser = 0 ;
		public const int aslNoEvent = -1 ;
		public static readonly string[] GPSFrequency = new string[6] { "30 sec","1 min","2 min","5 min","10 min","30 min" };
		//		public static readonly string[] SpeedTrigger = new string[13] { "10","30","50","60","70","80","90","100","110","115","120","125","130" };
		public static readonly string[] SpeedTrigger = new string[6] { "16","48","80","112","146","160" };
		public static readonly string[] SpeedTriggerMPH = new string[26] { "15","20","25","30", "50","60","61","62","63","64","65","66","67","68","69","70","75","76","78","79","80","82","85","90","101","121" };
		public static readonly string[] SpeedTriggerKMH = new string[18] { "20","30","40", "50", "60","70","80","90","100","105","110","111","115","120","121","125","130","140" };
		public const int DBStreetAddressThreadDelay = 30000 ; // 30 seconds
		public const int VehicleInfoCacheMaxSize = 500 ; 
		public const Int64 PositionExpiredTime = 43200 ; // 60 min * 24 hours * 30 days  
		public const string aslDefaultMapURL = @"http://192.168.0.16/GeoMicroMapService/";
		#endregion ASI Section
	
		#region AMS Section
		public const int SchedulerThreadDelay = 60000; // 1 minute
		public const int ScheduledTaskDelay = 5000; // 5 sec
		#endregion ASI Section

		#region DAP Section
		public const short DAP_API_VERSION = 0x0100 ; // major version high byte, minor version low
		#endregion

		#region Performance Parameters
		// dcs 
		public const string dcsReceiveMessagesRate = "Receive Packets/sec";
		public const string dcsSendMessagesRate = "Send Packets/sec";
		public const string dcsReceivedMessagesTotal = "Received Messages";
		public const string dcsSentMessagesTotal = "Sent Messages";
      public const string dcsActiveThreads = "Active Threads in Pool";
      public const string dcsDuplicateMessagesWithin40Secs = "Duplicate Messages received within 40 Secs";
      public const string dcsIntegrationServiceMessagesTotal = "Integration Service Messages Total";
      public const string dcsIntegrationServiceMessagesRate = "Integration Service Messages Rate";
      public const string dcsCallbacks = "Callback items";

        //mqs
		public const string mqsInMessages = "Messages in QueueIn";
		public const string mqsOutMessages = "Messages in QueueOut";
		//check db
		public const string dbOutMessagesRate = "DB OUT Messages/sec";
		// sls
		public const string slsRetrieveMessagesRate = "Retrieved Messages/sec";
		public const string slsRetrieveMessagesTotal = "Retrieved Messages";
		public const string slsCheckDBRate = "Check DB/sec";
		public const string slsProcessMessagesRate = "Processed Messages/sec";
		public const string slsProcessMessagesTotal = "Processed Messages";
      public const string slsProcessMessagesExceptionTotal = "Processed Messages throwing an exception";
		public const string slsActiveThreads = "Active Threads in Pool";
      public const string slsRetrievedPacketsNullDCL = "Packets with null DCL";
      public const string slsWaitingCallBacks = "Waiting callbacks in Pool";
		#endregion Performance Parameters

      public const int MIN_INTEGRATION_PKT_SIZE = 18;
      public const int MIN_NEW_PROTOCOL_PKT_SIZE = 10;

      public const int PKT_CMFIN = 1;        ///< this is the normal packet received from the box
      public const int PKT_CMFOUT = 2;       ///< this is the normal packet sent to the box
      public const int PKT_EXTERNAL = 3;     ///< this is for ThirdParty applications      
      public const int PKT_HEART_BEAT = 4;   ///< this is the heart beat between upstream dispatcher to downstream dispatcher
      public const int PKT_ID = 5;           ///< this is the ID of the downstream dispatcher to the upstream dispatcher 

      public const string CT_DURATION = "Duration: ";
      public const string CT_UNK_DURATION = "Unknown duration";
      public const string CT_IDLING_DURATION = "Idling Duration: ";

      // constants from reports
      public const string CT_ENGINE_IDLE = "Engine Idle";
      public const string CT_TRIP_START = "Trip Start";
      public const string CT_TRIP_END = "Trip End";
      public const string CT_HARSH_ACC = "Harsh Acceleration";
      public const string CT_HARSH_BRAKING = "Harsh Braking";
      public const string CT_EXTREME_ACC = "Extreme Acceleration";
      public const string CT_EXTREME_BRAKING = "Extreme Braking";
      public const string CT_SEAT_BELT_VIOLATION = "Seat Belt Violation";
      public const string CT_SPEEDING = "Speeding";
      public const string CT_POSITION = "Position";
      public const string CT_STORED_POSITION = "Stored Position";
      public const string CT_GEO_ZONE = "Geo Zone";
      public const string CT_DOLLAR = "$";
      public const string CT_PER_HOUR = "/h";
      public const string CT_MILES = "mi";
      public const string CT_KM = "km";

      #region map
      public const short    mapAssetsNo=0;
      public const short mapAssetsYes = 1;
      public const short VehicleClusteringNo = 0;
      public const short VehicleClusteringYes = 1;

      #endregion
    }
}
