using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace VLF.CLS.Def
{
    /// <summary>
    /// !!! IMPORTANT !!!
    /// Any new record in enums should be added TO THE END OF ENUM !!!
    /// DON'T EVER DELETE RECORDS FROM ENUM !!!
    /// </summary>
    public class Enums
    {
        public Enums()
        {
        }

        public enum ValidationItem : short
        {
            Validate_Organization = 1,
            Validate_Box = 2,
            Validate_Vehicle = 3,
            Validate_Driver = 4,
            Validate_Fleet = 5
        }

        public enum ConfigurationGroups
        {
            None = 0,
            Common = 1,
            LOG = 2,
            PAS = 3,
            DCS = 4,
            DAS = 5,
            SLS = 6,
        }
        public enum VehicleType : short
        {
            NotKnown = -1,
            Car = 0,
            Truck = 1,
            ArmoredVehicle = 2,
            Trailer = 3,
            Motorcycle = 4,
            OneTonTruck = 5,
            Distributors = 6,
            Loader = 7,
            Lowboy = 8,
            ServiceTruck = 9,
            Tanker = 10,
            Triaxles = 11,
            Van = 12,
            CarSecurity = 13,
            XS_Truck = 101,
            XS_Trailer = 102
        }

        public enum VehicleStatus : short
        {
            Null = 0,              ///< coresponds to ""
            NA = 1,
            Idling = 2,
            Ignition_ON = 3,
            Moving = 4,
            Parked = 5,
            PowerSave = 6,
            Tethered = 7,
            Untethered = 8,
            NotInCoverage = 9,              ///< this is the case where ScheduledUpdate didn't arrive every X minutes
            NA_ = NA + 100,
            Idling_ = Idling + 100,   ///< the packets are coming after one of the alarms came first !!!
            Ignition_ON_ = Ignition_ON + 100,
            Moving_ = Moving + 100,
            Parked_ = Parked + 100,
            PowerSave_ = PowerSave + 100,
            Tethered_ = Tethered + 100,
            Untethered_ = Untethered + 100,
            NotInCoverage_ = NotInCoverage + 100
        }

        public enum GeozoneType
        {
            Unknown = -1,
            None = 0,
            Rectangle = 1,
            Polygon = 2,
        }

        /** \comment [gb, 2006/06/03] all unsuported protocols shouls be commented out
         *           there is a document BoxConfiguration.doc which shows the relationship
         *           between <firmware, Communication Services [DCLs], protocols>
         */
        public enum ProtocolTypes
        {
            Unknown = -1,
            DCL = 0,            // internal messages between modules 
            ASI = 1,            // internal messages between modules 
            HGIv10 = 2,
            HGIv20 = 3,
            HGIv30 = 4,
            MBv10 = 5,
            SATv10 = 6,
            SATv20 = 7,
            HGIv40 = 8,            // SentinelFM
            SATv40 = 9,
            DAPv10 = 10,
            HGIv50 = 11,           // Bantek v 2.x
            MBv20 = 12,
            MBv21 = 13,
            MBv22 = 14,
            HGIv60 = 15,
            HGIv70 = 16,           // Bantek v 2.x
            MBv30 = 17,
            TCPv10 = 18,
            RAPv10 = 19,
            OTAv10 = 20,
            DAPv20 = 21,
            MDSv10 = 22,
            HGIv80 = 23,           // SentinelFM 
            BEPv10 = 24,
            XSv10 = 25,            // XS   
            Iridiumv10 = 26,             // Iridium
            RAPv11 = 27,
            IAMSv10 = 28,
            CALv10 = 29,
            IAMSv10P = 30,
        }
        public enum ArmedStatus
        {
            NotSet = 0,
            False = 1,
            True = 2,
        }
        public enum GPSValid
        {
            True,
            False,
            NotSet,
        }

        // key fob setup
        public enum RemoteControlSettings
        {
            Disable = 0,
            Enable = 1,
            Learn = 2,
            Erase = 3
        }

        public enum DeviceType : byte
        {
            AVLXS = 0,
            ReeferAdapter = 1,
            ReadWriteEpromBasedOnLocation = 2
        }
        public enum XSMessageIdMask
        {
            MessageId = 0x00ff,
            DeviceType = 0x0f00,
            DeviceId = 0xf000
        }

        public const short ReeferBase = (short)DeviceType.ReeferAdapter << 8;     // 256
        public const short ReadWriteEpromBasedOnLocationBase = (short)DeviceType.ReeferAdapter << 9;     // it allows 256 commands for Reefer

        // Command Types for all protocols in the system. Can be common.
        public enum CommandType : short
        {
            Unknown = -1,
            Ack = 0,
            FindVehicle = 1,
            Arm = 2,
            Disarm = 3,
            Output = 4,
            SetFence = 5,
            ClearFences = 6,
            Setup = 7,
            DAck = 8,
            AcceptCall = 9,
            BoxReset = 10,
            IdnAck = 11,
            EndCall = 12,
            UpdatePosition = 13,
            GetWaypoints = 14,
            ClearMemory = 15,
            GetBoxStatus = 16,
            GetBoxSetup = 17,
            ClearFence = 18,
            SecurityCode = 19,
            VoiceMessage = 20,
            Trace = 21,
            MDTTextMessage = 22,
            MDTAck = 23,
            AddGeoZone = 24,
            DeleteGeoZone = 25,
            GetGeoZoneIDs = 26,
            GetGeoZoneSetup = 27,
            SetReportInterval = 28,
            SetSpeedThreshold = 29,
            SetEnabledSensor = 30,
            SetTrace = 31,
            StartTraceMode = 32,
            StopTraceMode = 33,
            GeneralSet = 34,
            GeneralRead = 35,
            DeleteAllGeoZones = 36,
            RadioEnable = 37,
            RadioDisable = 38,
            MobilizeVehicle = 39,
            ImmobilizeVehicle = 40,
            ProxCards = 41,
            ControllerReset = 42,
            SetVCROffDelay = 43,
            GetVCROffDelay = 44,
            KeyFobEnable = 45,
            KeyFobDisable = 46,
            GetKeyFobStatus = 47,
            SetTARMode = 48,
            GetTARMode = 49,
            GetControllerStatus = 50,
            SetPowerDown = 51,
            BoxHardReset = 52,
            WakeUpByte = 53,
            ChangeBoxID = 54,
            //			TARModeFor7Days = 55,
            SetServiceRequired = 56,
            SetIdle = 57,
            SetRecordsCount = 58,
            SetOdometer = 59,
            GetOdometer = 60,
            UploadFirmwareStatus = 61,
            UploadFirmware = 62,
            LockDoor = 63,
            UnlockDoor = 64,
            IgnitionEnable = 65,
            IgnitionDisable = 66,
            UpdateTelusPRL = 67,
            SetPowerOffDelay = 68,
            GetPowerOffDelay = 69,
            RemoteControlLearn = 70,
            RemoteControlErase = 71,
            UpdatePositionSatellite = 72,
            StartOTAforXS = 73,            // gb - this packet is sent from OTA session to a normal session
            ThirdPartyPacket = 74,          // gb - message for 3Rd Party libraries
            WriteEEPROMData = 75,        // AVLXS Protocol V1.4, SentinelFM Protocol V2.11, 
            ReadEEPROMData = 76,
            ExtendedSetup = 77,        // SentinelFM Protocol V2.11
            GetExtendedSetup = 78,
            GetExtendedStatus = 79,
            SetExtendedEnabledSensor = 80,
            KeyFobSetup = 81,
            StartOTAPlus = 82,
            SwapActiveModem = 83,             // introduced for Bantek and added to IPDCL 8.0 
            ChannelSetup = 84,                // see Bantek Protocol v3.5 
            MDTNAck = 85,
            SetBoxSleepTime = 86,
            SetIridiumFilter = 87,
            GetIridiumFilter = 88,
            SetPowerSaveConfig = 89,
            GetPowerSaveConfig = 90,
            SynchronizeGeoZone = 91,         // is a command intended only for CommandScheduler
            // and is reading the geo zones and send multiple commands using the command scheduler
            DeleteGeoZoneFromBox = 92,
            SetExtendedReportInterval = 93,  // used for messages stored in the box and dumped at once
            KeepAliveAck = 94,               // gb contains the sequence number
            GetSMC = 95,
            GetDTC = 96,
            ClearDTC = 97,
            MDT_OTA_Firmware = 98,
            MDT_OTA_Forms = 99,
            SetFuelConfiguration = 100,
            StartDeviceTest = 101,
            WriteSMC = 102,
            SetTrailerWakeSleepTimers = 103,
            LimitExceeded = 104,
            MovementAlert = 105,
            Heartbeat = 106,
            Reefer_Send_Message=110,
            Reefer_Initiate_Defrost = 111,
            Reefer_Change_Set_Point = 112,
            Reefer_Clear_Alarm = 113,
            Reefer_Initial_Pre_Trip = 114,
            Reefer_Change_Operating_Mode = 115,
            Reefer_Open_Fresh_Air_Door = 116,
            Reefer_Close_Fresh_Air_Door = 117,
            Reefer_Turn_Power_Off = 118,
            Reefer_Turn_Power_On= 119,
            Reefer_Multiple_Alarm_Read = 120,
            Reefer_Software_Identification = 121,
            WiFiUpgrade=123,
            

            DebugInfo = 254,
            Output_2_Single_Tap = 124,
            Output_2_Double_Tap = 125,

            #region Reefer, AVLXS Protocol 9 - V1.5
            ReeferAck = ReeferBase + 0,   // 256
            SetReeferSetup = ReeferBase + 1,   // 257
            GetReeferSetup = ReeferBase + 2,   // 258
            ReeferReset = ReeferBase + 3,    // 259, AVLXS Protocol 9 - V1.6
            #endregion Reefer, AVLXS Protocol 9 - V1.5

            #region Read Write Eprom
            RWEPROMFuelConfiguration = ReadWriteEpromBasedOnLocationBase + 0, // offset 0xA2
            RWEPROMFeatureConfiguration = ReadWriteEpromBasedOnLocationBase + 1, // offset 0x2F
            #endregion Read Write Eprom

            #region Third Party Messages
            FuelTransaction=900,
            #endregion

            #region Spreader Messages
            Spreader = 700,
            #endregion
        }

        public enum MapTypes
        {
            Map = 1,
            StreetAddress = 2,
            GetDistance = 3,
        }

        // Message Types for all protocols in the system. Can be common.
        public enum MessageType
        {
            Ack = 0,
            Coordinate = 1,
            Sensor = 2,
            Speed = 3,
            GeoFence = 4,
            KeyFobArm = 5,
            KeyFobDisarm = 6,
            Identification = 7,
            PictureDownloadComplete = 8,
            PositionUpdate = 9,
            IPUpdate = 10,
            WaypointData = 11,
            WaypointDownloadEnd = 12,
            BoxStatus = 13,
            BoxSetup = 14,
            KeyFobPanic = 15,
            ExternalData = 16,
            NAck = 17,
            Alarm = 18,
            MBAlarm = 19,
            MDTResponse = 20,
            MDTSpecialMessage = 21,
            MDTTextMessage = 22,
            MDTAck = 23,
            StoredPosition = 24,
            Idling = 25,
            GeoZone = 26,
            GPSAntenna = 27,
            GeoZoneIDs = 28,
            GeoZoneSetup = 29,
            VCROffDelay = 30,
            KeyFobStatus = 31,
            TARMode = 32,
            ControllerStatus = 33,
            Status = 34,
            Speeding = 35,              // speed with duration for XS
            AlivePacket = 36,
            ServiceRequired = 37,
            BoxReset = 38,
            LowBattery = 39,
            AuxInputOneOn = 40,
            AuxInputTwoOn = 41,
            Odometer = 42,
            GPSAntennaShort = 43,
            GPSAntennaOpen = 44,
            BreakIn = 45,
            TowAwayAlert = 46,
            Collision = 47,
            PanicButton = 48,
            SleepModeFailure = 49,
            AckWithPosition = 50,
            GPSAntennaOK = 51,
            RFIDCode = 52,
            UpdateTelusPRLDone = 53,
            MainBatteryDisconnected = 54,
            SleepModeProblem = 55,           // gb 2006/06/20 
            PowerOffDelay = 56,
            BadSensor = 57,                   // gb 2006/08/17
            HarshBraking = 58,                // gb 2006/08/22
            ExtremeBraking = 59,
            HarshAcceleration = 60,
            ExtremeAcceleration = 61,
            SeatBelt = 62,
            ThirdPartyPacket = 63,         // gb 2006/11/15
            // this is a message which is transparent for our system  
            SendEEPROMData = 64,           // AVLXS Protocol V1.4, SentinelFM Protocol V2.11, 
            SendExtendedSetup = 65,        // SentinelFM Protocol V2.11
            SendExtendedStatus = 66,
            IPUpdateBantek = 67,
            SendIridiumFilter = 68,
            SendPowerSaveConfiguration = 69,
            MultipleStandardMessages = 70,      // gb, for Gen 6
            MultipleStandardMessagesXS = 71,    // gb, for XS
            ExtendedIdling = 72,                // gb, used for alarms
            SensorExtended = 73,                // gb, IGNITION ON/OFF with duration
            KeepAliveDual = 74,                 // gb, 2008/03/27
            SendSMC = 75,                       // gb, 2008/04/02
            TetheredState = 76,                 // gb, 2008/05/26
            SendDTC = 77,                       // gb, 2008/06/15
            MDT_OTA_Firmware_ACK = 78,
            MDT_OTA_Forms_ACK = 79,
            DeviceTestResult = 80,
            SMCWriteDone = 81,                  // standard message
            ////////////////////////////////  DATACOM   /////////////////////////////
            TripStart = 82,                     // Webtech box - 'r'
            TripEnd = 83,                       // Webtech box - 'r'
            SpeedStart = 84,                    // Webtech box - 'p'
            SpeedEnd = 85,                    // Webtech box   - '^'
            DistanceGPS = 86,                 // Webtech box   - '~'
            TurnPacketGPS = 87,               // Webtech box   - '%'
            TracerGPS = 88,                   // Webtech box   - '-'
            ////////////////////////////////  DATACOM   /////////////////////////////
            GeoZoneSpeedStart = 89,          // '('
            GeoZoneSpeedEnd = 90,          // ')'
            DallasKey = 91,            // is like driver sign in - '+'
            /////////////////////////////////////////////
            DoSStarted = 100,
            DoSFinished = 101,
            HighVolumeTraffic = 102,
            SuspiciousSilence = 103,
            LimitExceeded = 104,
            MovementAlert = 105,
            Heartbeat = 106,
            ReverseExcessSpeed = 107,
            ReverseExcessDistance = 108,
            Landmark = 109,
            VirtualLandmark = 110,
            HighRailSpeed = 111,
            ReverseHyRailExcessSpeed = 113,
            DebugInfo = 254,
            ReeferInitialPreTrip = 114,             //0x30
            Turn=116,
            VIN_Changed = 117,
            Force_America = 118,
            Harsh_Drive = 119,
            BatteryTrending=120,
            GPS_Module_Initialized_OK = 122,


            #region Reefer, AVLXS Protocol 9 - V1.5
            ReeferAck = ReeferBase + 0,   // 256
            SendReeferSetup = ReeferBase + 1,   // 257
            ReeferStatusReport = ReeferBase + 2,   // 258
            ReeferTemperatureAlarm = ReeferBase + 3,   // 259
            ReeferFuelAlarm = ReeferBase + 4,   // 260
            ReeferSensorAlarm = ReeferBase + 5,   // 261
            ReeferFuelLevelRiseDropAlarm = ReeferBase + 6,    // 262, AVLXS Protocol 9 - V1.7
            #endregion

            #region Read Write Eprom
            RWEPROMFuelConfiguration = ReadWriteEpromBasedOnLocationBase + 0, // offset 0xA2
            RWEPROMFeatureConfiguration = ReadWriteEpromBasedOnLocationBase + 1, // offset 0x2F
            #endregion Read Write Eprom

            #region Third Party Messages
            FuelTransaction=900,
            #endregion

            #region Spreader
            Spreader = 700,
            #endregion
        }

        // XS ack status
        public enum AckStatus : byte
        {
            ReceivedAndExecuted = 0,
            ReceivedAndNotExecuted = 1,
            ChecksumIncorrect = 2,
            BoxIdIncorrect = 3,
            MessageNotSupported = 4,
            SequenceNumberIncorrect = 5,
            ServerBusy = 6,
            InvalidParameter = 7
        }

        // Message Types for all protocols in the system. Can be common.
        public enum TxtMsgType
        {
            MdtAck = 0,
            MdtText = 1,
            MdtSpecial = 2,
            MdtResponse = 3,
            ClientText = 4,
            ClientAck = 5,
        }
        // Message Types for all protocols in the system. Can be common.
        public enum TxtMsgDirectionType
        {
            In = 0,
            Out = 1,
            Both = 2,
        }
        // Message Types for all protocols in the system. Can be common.
        public enum SystemUpdateType
        {
            All = 0,
            NewFeature = 1,
            SystemStatus = 2,
        }
        // DCL Communication Info Types for all DCLs.
        public enum DCLCommInfo
        {
            ChannelOperation = 0,
            ChannelInitSuccess = 1,
            Connect = 2,
            Ring = 3,
            Drop = 4,
            Busy = 5,
            Error = 6,
            FatalError = 7,
            Diagnostic = 8,
        }

        // DCL Command Types for all DCLs.
        public enum DCLCommand
        {
            Connect = 0,
            Disconnect = 1,
            Reset = 2,
        }

        // All communication address types in the system. Can be common.
        public enum CommAddressType
        {
            PhoneNum = 0,
            IP = 1,
            Port = 2,
            PMin = 5,	// MicroBurst only
            Min1 = 6,	// MicroBurst only
            Min2 = 7,	// MicroBurst only
            Min3 = 8,	// MicroBurst only
            Min4 = 9,	// MicroBurst only
            Email = 10,	// Satellite devices
            ESN = 11,	// MicroBurst only
            DeviceID = 12,	// SkyWave (TerminalID), Matco (AssetID), AirLink 
            Min5 = 13,	// MicroBurst only
            Min6 = 14,	// MicroBurst only
            Min7 = 15,	// MicroBurst only
            Min8 = 16,	// MicroBurst only
            Min9 = 17,	// MicroBurst only
            RadioID = 18,	// ??
            MobileId = 19,	// SIM from GPRS, ESN from CDMA
            IMEI = 20    // International Mobile Equipment Identity
        }
        // All communication modes in the system. Can be common.
        public enum CommMode
        {
            Unknown = -1,
            DCL = 0,
            AMPS = 1,
            CELL_UDP = 2,
            MB = 3,
            SAT = 4,
            GPRS_ROGERS = 5,
            GPRS_KORE = 6,
            Inmarsat = 7,
            CELL_TCP = 8,
            CDMA_AIRLINK = 9,
            MSAT = 10,
            GPRS_BELL_VPN = 11,
            Satamatics = 12,
            SecTrack_Socket = 13,
            SBD_Iridium = 14,
            IAMS_v10 = 17,
            CDMA_SASKTEL_VPN = 18,
            IAMS_v10P = 19,
        }

        // All permissions in the system. Can be common.
        public enum Permission
        {
            AddUser = 0,
            DeleteUser = 1,
            ModifyUser = 2,
            AddVehicle = 3,
            DeleteVehicle = 4,
            ModifyVehicle = 5,
        }

        // All user gGroups in the system. Can be common.
        public enum UserGroup
        {
            Administrator = 0,
            Salesman = 1,
            RcClient = 2,
            WebClient = 3,
        }

        // All permissions in the system. Can be common.
        public enum OperationType
        {
            System = 0,
            Output = 1,
            Command = 2,
            Gui = 3,
            Repors = 4,
            N_A = 5,
            WebMethod = 6,
        }

        // All event severities in the system. Can be common.
        public enum AlarmSeverity
        {
            NoAlarm = 0,
            Notify = 1,
            Warning = 2,
            Critical = 3,
        }
        // All event types in the system. Can be common.
        public enum Preference
        {
            MeasurementUnits = 0,
            TimeZone = 1,
            DefaultFleet = 2,
            DefaultLatitude = 3,
            DefaultLongitude = 4,
            DefaultZoomLevel = 5,
            ResolveLandmark = 6,
            DayLightSaving = 7,
            MapOptShowVehicleName = 8,
            MapOptShowLandmark = 9,
            MapOptShowLandmarkName = 10,
            MapOptShowBreadCrumbTrail = 11,
            MapOptShowStopSequenceNo = 12,
            ShowReadMessages = 13,
            AlarmRefreshFrequency = 14,
            GeneralRefreshFrequency = 15,
            PositionExpiredTime = 16,
            AutoAdjustDayLightSaving = 17,
            MapGridDefaultRows = 18,
            HistoryGridDefaultRows = 19,
            ViewMDTMessagesScrolling = 20,
            ShowMapGridFilter = 21,
            DefaultMapId = 22,
            MapOptShowGeoZone = 23,
            MapOptShowLandmarkRadius = 24,
            VolumeUnits = 25,
            ViewAlarmMessagesScrolling=26,
            MapAssets = 29,
            MaxVehiclesOnMap = 30,
            DefaultMapView=31,
            VehicleClustering=32,
            VehicleClusteringDistance=33,
            VehicleClusteringThreshold=34,
            DefaultOrganizationHierarchyNodeCode=35,
            DefaultLanguage = 37,
            LoadLandmarkByDefault = 40,
            RememberLastPage = 46, 
            // Changes for TimeZone Feature start
            TimeZoneNew= 47,
            // Changes for TimeZone Feature end
            VehicleNotReported = 48,
            EventColumns = 49,
            ViolationColumns =50,
            RecordsToFetch = 51,
            ShowRetiredVehicles = 52,
            TemperatureType = 53


        }

        public enum OTAReturnCode
        {
            Success,
            NoCommunication,
            FWUpdateFailed,
            DBUpdateFailed,
            SetupFailed,
            FirmwareProblem,
        }

        // Micro Burst messages
        public enum MBMessageTypeRecc
        {
            UNKNOWN = 0,
            SEND_DS = 1,
            SEND_DS_ACK = 2,
            RESEND_DS = 3,
            PING_DS = 13,
            REJECT_DS_CONNECTION = 14,
            PING_DS_ACK = 15,
            RESEND_DS_ACK = 16,
            UNKNOWN_DS_ERROR = 17,
            PASSWORD_DS = 18,
            PASSWORD_DS_ACK = 19,
            PASSWORD_DS_REJ = 20,
            HEALTH_DS = 21,
            HEALTH_DS_RESULT = 22,
            STATUS_DS = 23,
            STATUS_DS_ACK = 24,
            DISCONNECT_DS = 25,
            DISCONNECT_DS_ACK = 26,
            VERSION_DS = 27,
            VERSION_DS_RESULT = 28,
        }

        public enum MBMessageTypeFocc
        {
            PASSWORD_AS = 103,
            PASSWORD_AS_ACK = 104,
            PASSWORD_AS_REJECT = 105,
            PAGE_AS = 106,
            PAGE_AS_ACK = 107,
            PAGE_AS_REJECT = 108,
            PAGE_AS_RESULT = 109,
            CONFIG_AS = 110,
            CONFIG_AS_ACK = 111,
            CONFIG_AS_REJECT = 112,
            CONFIG_AS_RESULT = 113,
            QUERY_AS = 114,
            QUERY_AS_ACK = 115,
            QUERY_AS_REJECT = 116,
            QUERY_AS_RESULT = 117,
            PING_AS = 119,
            PING_AS_ACK = 120,
            UNKNOWN_AS_ERROR = 122,
            MULTI_AS = 123,
            MULTI_AS_ACK = 124,
            MULTI_AS_REJECT = 125,
            MULTI_AS_RESULT = 126,
            HEALTH_AS = 127,
            HEALTH_AS_RESULT = 128,
            STATUS_AS = 129,
            STATUS_AS_ACK = 130,
            DISCONNECT_AS = 131,
            DISCONNECT_AS_ACK = 132,
            VERSION_AS = 133,
            VERSION_AS_RESULT = 134,
            TIMED_AS = 135,
            TIMED_AS_ACK = 136,
            TIMED_AS_REJECT = 137,
            TIMED_AS_RESULT = 138,
            PAGE_UID_AS = 139,
            PAGE_UID_AS_ACK = 140,
            PAGE_UID_AS_REJECT = 141,
            CONFIG_UID_AS = 142,
            CONFIG_UID_AS_ACK = 143,
            CONFIG_UID_AS_REJECT = 144,
            QUERY_UID_AS = 145,
            QUERY_UID_AS_ACK = 146,
            QUERY_UID_AS_REJECT = 147,
            MULTI_UID_AS = 148,
            MULTI_UID_AS_ACK = 149,
            MULTI_UID_AS_REJECT = 150,
            TIMED_UID_AS = 151,
            TIMED_UID_AS_ACK = 152,
            TIMED_UID_AS_REJECT = 153,
        }

        public enum MBRejectReasons
        {
            UNUSED = 0,
            RESERVED_OFF_CODE,
            INV_PAGE_TYPE,
            INV_MIN,
            UNEQ_NPA_CODE,
            UNUSED1,
            TIMEOUT,
            CONNECT_FAIL,
            NOT_PASSWORD_MESSAGE,
            INVALID_CREDENTIALS,
            NOT_IN_DB,
            INVALID_MSG_TYPE,
            INVALID_MSG_LENGTH,
            TOO_FREQ_HEALTH_REQ,
            TOO_FREQ_VERSION_REQ,
            INVALID_CONFIG_CMD,
            RESERVED_CONFIG_CMD,
            INVALID_CONFIG_DATA,
            RESERVED_CONFIG_DATA,
            NOT_MIN_OWNER,
            MULTI_PAGE_MINS_EQUAL,
            INVALID_NUM_OF_PAGES,
            INVALID_TIME_TO_LIVE,
            EXTRANEOUS_MESSAGE,
            MIN_NOT_ACTIVE,
            DEVICE_LOC_UNKNOWN,
            CONN_LIMIT_EXCEEDED,
            MIN_EVEN_ODD_VIOLATED,
            UNUSED2,
            UNUSED3,
            RESEND_ALREADY_RCVD,
            INVALID_FILTER_COUNT,
            TOO_FREQUENT_PING,
            INVALID_DATA_MSG_LEN,
            DB_ERROR,
            OUTBOX_IS_EMPTY,
            OUTBOX_IS_NOT_EMPTY,
            INVALID_DLVRY_METHOD,
            INVALID_FTP_INFO,
            INVALID_FTP_COMP,
            INVALID_FTP_INTERVAL,
            TOO_FREQUENT_RESEND,
            PRIM_SEC_MIN_MIX,
            TOO_MANY_PRIM_MINS,
            TOO_MANY_ACTIVE_PAGES,
            TOO_FREQUENT_PAGE,
            MIN_NOT_UNIQUE,
            UNEXPECTED_HLR_ERROR,
            INVALID_DATA_LENGTH,
            DATA_LENGTH_MISMATCH,
            INVALID_DATA_CONTENT,
            FUNC_NOT_ALLOWED,
            FUNC_NOT_SUPPORTED,
            INV_POINTCODE_MSC_COMB,
        }

        public enum MBMessageResults
        {
            NO_ERROR = 0,
            SD_ERROR = 3,
            INV_PRIMARY_MIN = 4,
            MIN_NOT_IN_DB = 5,
            MIN_NOT_ACTIVE = 6,
            HLR_CONNECT_ERR = 9,
            TTL_EXPIRED = 17,
            CRITICAL_DOWNTIME = 19,
            MAINT_DOWNTIME = 20,
            DEVICE_LOC_UNKNOWN = 21
        }

        // Bitwise diag mask
        public enum BitWiseDiagMask
        {
            None = 0,
            Statistics = 0x1,
            Info = 0x2,
            Trace = 0x4,
            Ping = 0x8,
        }
        // don't change ordering because of AddGeoZone command from HGI Sentinel FM v40 protocol
        public enum GeoZoneDirection
        {
            Disable = 0,
            Out = 1,
            In = 2,
            InOut = 3,
        }
        public enum SensorsTraceState
        {
            Disable = 0,
            OnOnly = 1,
            OffOnly = 2,
            Both = 3
        }
        public enum TraceSeverity
        {
            Error,
            Warning,
            Info,
            Verbose,
            Ping,
            Statistics,
            WebInterfaces,
            Map,
            Das,
            Ota,
        }
        public static string[] TraceSeverityMessage =
       {
               "ERRO",
               "WARN",
               "INFO",
               "VERB",
               "PING",
               "STAT",
               "INTF",
               "MAPS",
               "DAS_",
               "OTA_"
       };

        public enum GPSAntennaCutReason
        {
            ActiveOn,			// No Alarm: Antenna Ok 
            OpenCircuit,		// Alarm: Antenna Cut
            ShortCircuit,		// Alarm: Antenna Short
            ActiveOff,			// Alarm: Antenna not configured properly
            PassiveOn			// Alarm: Leo and Frank are not sure
        }

        public enum PowerReason
        {
            SleepMode,			// Alarm: No Power 
            PowerOK,			// Power OK
            Tethered,			// Power On
            Untethered,			// Alarm: Power down
            WakeUp,				// HeartBeat
            PowerDisconnected   // Power Disconnected
        }

        public enum ConfigurationModuleTypes
        {
            Computer = 1,
            CommService = 2,
            Process = 3,
            IIS = 4,
            WinService = 5,
            Manager = 6,
        }

        public enum ConfigurationModuleState
        {
            Disabled = 0,
            Enabled = 1,
            Both = 2
        }
        public enum AlarmState
        {
            New,
            Accepted,
            Closed,
            Unknown
        }
        /// <summary>
        /// Current state of the base service. 
        /// </summary>
        public enum ServiceState
        {
            Stopped = 0,
            StartPending = 1,
            StopPending = 2,
            Running = 3,
            ContinuePending = 4,
            PausePending = 5,
            Paused = 6,
            Unknown = 7,
        }
        // All firmware types in the system.
        public enum FirmwareType
        {
            AllTypes = 0,
            SentinelFM = 1,
            Bantek = 2,
            SkyWave = 3,
            MDT = 4,
            Matco = 5,
            AirLinkPinPoint = 6,
            PDTMsat = 7,
            Reefer = 8,
            BlueTree = 9,
            Satamatics = 10,
            Iridium = 11
        }

        // Reasons for IP Update
        public enum IPUpdateReason
        {
            Unknown = 0,
            PowerUp = 1,
            ModemReset = 2,
            WakeUp = 3,
            ScheduledWakeUp = 4,
            ModemLock = 5,
            TaskOverloaded = 6,
            LatsPointerOverloaded = 7,
            StackTrashed = 8,
            GPSModuleStop = 9,
            EventQueueFull = 10,
            FailSafe1Hour = 11,
            FailSafe12Hours = 12,
            MaxIPUpdate = 13,
            TaskFailure = 14
        }
        // output id by server definition.
        public enum OutputID
        {
            //OutputName	OutputId	OutputAction
            None = 0,
            Starter = 1, //Disable/Enable                                    
            Horn = 2, // On/Off                                            
            Piezo = 3, // On/Off                                            
            DoorLock = 4, // */Lock                                            
            DoorUnlock = 5, // Unlock/*                                          
            UserDef3 = 6, // On/Off                                            
            UserDef1 = 7, // On/Off                                            
            VCR = 8, // On/Off                                            
            Ignition2Disable = 9, // Disable/*                                         
            Ignition2Enable = 10, // */Enable                                          
            Siren = 11, // On/Off                                            
            Lights = 12, // On/Off                                            
            FuelPumpDisable = 13, // Disable/*                                         
            FuelPumpEnable = 14, // */Enable                                          
            UnlockBox = 15, // Yes/No                                            
            SpareOutput2 = 16, // On/Off                                            
            SpareOutput3 = 17, // On/Off                                            
            SpareOutput4 = 18, // On/Off                                            
            DefOut19 = 19, // On/Off                                            
            DefOut20 = 20, // On/Off                                            
            DefOut21 = 21, // On/Off                                            
            DefOut22 = 22, // On/Off                                            
            DefOut23 = 23, // On/Off                                            
            DefOut24 = 24, // On/Off                                            
            DefOut25 = 25, // On/Off                                            
            DefOut26 = 26, // On/Off                                            
            DefOut27 = 27, // On/Off                                            
            DefOut28 = 28, // On/Off                                            
            DefOut29 = 29, // On/Off                                            
            DefOut30 = 30, // On/Off                                            
            DefOut31 = 31, // On/Off                                            
            DefOut32 = 32, // On/Off                                            
            DefOut33 = 33, // On/Off                                            
            TransportAndRepair = 34  // On/Off                                            
        }

        /// <summary>
        /// Firmware attributes, inherited by a box
        /// DB: vlfFirmware->FwAttributes1, vlfBox->FwAttributes1
        /// </summary>
        [Flags]
        public enum FwAttributes : long
        {
            GEN6_TRUCK = 0x01,
            GEN6_TRAILER = 0x02,
            XS_TRUCK = 0x04,
            XS_TRAILER = 0x08,
            PTO = 0x20,
            TP = 0x40,
            REEFER = 0x80,
            OUTPUTS_SECONDARY_COMMUNICATION_MODE = 0x100
        }

        /// <summary>
        /// HosRule for Hours of Service Calculations
        /// </summary>
        public enum HosRule
        {
            Undefined,
            CDN,
            CDN_Sleeper,
            CDN_Team,
            US_CMV,
            US_CMV_Sleeper,
        }

        /// <summary>
        /// HosDutyCycle for Hours of Service Calculations
        /// </summary>
        public enum HosDutyCycle
        {
            Canadian7,
            Canadian14,
            US,
            USsleeper,
        }

        /// <summary>
        /// HosDutyState for Hours of Service Calculations
        /// </summary>       
        [Flags]
        public enum HosDutyState
        {
            Unknown = 0x00,
            OffDuty = 0x01,
            Sleep = 0x02,
            Driving = 0x04,
            OnDuty = 0x08,
        }

        /// <summary>
        /// HosException codes
        /// </summary>
        public enum HosException
        {
            NoExceptions,
            DrivingHoursExceeded,                   //Driver has driven after exceeding the allowed hours of driving time in a day
            DrivingOnDutyHoursExceeded,             //Driver has driven after exceeding the allowed hours of on-duty time in a day
            DrivingOffDutyHoursExceeded,            //Driver has driven after exceeding the allowed hours between the mandatory off-duty periods
            DrivingCycleHoursExceeded,              //Driver has driven after exceeding the allowed hours in current cycle
        }

        /// <summary>
        /// HosNotificationSeverity for Hours of Service notifications
        /// </summary>
        public enum HosNotificationSeverity
        {
            None,
            Warning,
            Critical,
            Exception,
        }

        /// <summary>
        ///     this is an internal notification from different process or from rules enabled by the user
        /// </summary>
        /// <comment>
        ///     the same values are in vlfNotificationType 
        /// </comment>
        public enum NotificationType : short
        {
            No = 0,
            DTC_Codes = 1,              ///< OBD2 codes or J1708 codes
            MIL_Light_On = 2,
            Service_Maintenance = 3,
            Route_adherence = 4,
            Abnormal_GPS = 5,           ///< this is detected if the GPS bounce too far from the previous location
            Abnormal_GPS_time = 6,      ///< too much in the future or in the history
            Fuel_transaction = 7,
            Off_working_hours = 8,
            Wireless_signal_lost = 9,
            J1708_Codes = 10            ///< J1708 codes
        }

        public enum PeripheralTypes
        {
            NA = 1,
            Reefer = 2,
            Netistix = 3,
            MDT = 4,
            Garmin = 5,
        }

        public enum InterfacePrefrence
        {
            SentinelFM = 0,
            Lite = 1,
            Both = 2,
        }


        public enum OrganizationPreference
        {
            HeaderColor = 1,
            ConfTabBackGround = 2,
            HomePagePicture = 3,
            InterfacePreference = 4,
            ResovleLSDAddress = 5,
            SMSsupport = 6,
            SpeedThresholdDriverReport=7,
            IdlingThresholdDriverReport = 8,
            ReportFrequencyEmailDriver = 9,
            TempZone =10,

        }

        public enum HistoryTables
        {
            vlfMsgInHst=0,
            AppData=1,
            vlfMsgInHstFailures = 100,
            AppDataFailures = 101,
        }

        public enum ApplicationType : ushort
        {
            MdtOta = 2,
            ThirdParty = 5,
            DriverSignIn = 6,
            ThirdPartyGarmin = 7,
            Garmin = 8,
            TrynEx = 9,
            CalAmpMower = 10,
            Dev = 99,
        }
        public enum AuthenticationMDT : short
        {
            ExceptionInSQL = -2,
            CredentialsFailed = -1,
            NoAction,
            NewDriverAssignment
        }
    }

    public class DAPEnums
    {
        public enum POLL_TYPE
        {
            POLL_TYPE_NONE = -1,
            POLL_TYPE_7 = 0x7,
            POLL_TYPE_8 = 0x8,
            POLL_TYPE_9 = 0x9,
            POLL_TYPE_A = 0xa,
            POLL_TYPE_B = 0xb,
            POLL_TYPE_C = 0xc,
            POLL_TYPE_D = 0xd,
            POLL_TYPE_10 = 0x10,
            POLL_TYPE_11 = 0x11,
            POLL_TYPE_12 = 0x12,
            POLL_TYPE_13 = 0x13,
            POLL_TYPE_14 = 0x14,
            // custom poll types
            POLL_TYPE_20 = 0x20,
            POLL_TYPE_21 = 0x21,
            POLL_TYPE_22 = 0x22,
            POLL_TYPE_23 = 0x23,
            POLL_TYPE_24 = 0x24,
            POLL_TYPE_25 = 0x25,
            POLL_TYPE_26 = 0x26,
            POLL_TYPE_27 = 0x27,
            POLL_TYPE_28 = 0x28,
            POLL_TYPE_29 = 0x29,
            POLL_TYPE_2A = 0x2A,
            POLL_TYPE_2B = 0x2B,
            POLL_TYPE_2C = 0x2C,
            POLL_TYPE_2D = 0x2D,
            POLL_TYPE_2E = 0x2E,
            POLL_TYPE_2F = 0x2F,
            POLL_TYPE_30 = 0x30,
            POLL_TYPE_31 = 0x31,
            POLL_TYPE_32 = 0x32,
            POLL_TYPE_33 = 0x33,
            POLL_TYPE_34 = 0x34,
            POLL_TYPE_35 = 0x35,
            POLL_TYPE_36 = 0x36,
            POLL_TYPE_37 = 0x37,
            POLL_TYPE_38 = 0x38,
            POLL_TYPE_39 = 0x39,
            POLL_TYPE_3A = 0x3A,
            POLL_TYPE_3B = 0x3B,
            POLL_TYPE_3C = 0x3C,
            POLL_TYPE_3D = 0x3D,
            POLL_TYPE_3E = 0x3E,
            POLL_TYPE_3F = 0x3F,
        }
        public enum DAP_RC
        {
            DAP_ERR_ALREADY_OPEN = -53, //	The session must be closed before reopening. 
            DAP_ERR_MESSAGE_STATUS = -52, //	Message status missing from response. 
            DAP_ERR_MESSAGES_MISSING = -51, //	Messages requested were not received. 
            DAP_ERR_XML_RESPONSE = -50, //	Host response contains illegal XML. 
            DAP_ERR_XML_FAULT = -49, //	Request rejected by host, no reason given. 
            DAP_ERR_HTTP_INIT = -48, //	Error initializing the HTTP transport. 
            DAP_ERR_UNSUPPORTED = -47, //	Operation not supported with specified transport. 
            DAP_ERR_REGION = -46, //	Bad ocean region in request. 
            DAP_ERR_INTERNAL = -45, //	Bad parameters passed or internal API error. 
            DAP_ERR_NOT_PROCESSED = -44, //	The associated request has not been sent to the gateway. 
            DAP_ERR_MESSAGE_ID = -43, //	Message ID missing from submit response. 
            DAP_ERR_RESPONSE_CODE = -42, //	Response Code missing from response. 
            DAP_ERR_REPEAT = -41, //	Invalid repeat interval in submit request. 
            DAP_ERR_INCOMPLETE_SERVER = -40, //	The gateway server specification is bad or incomplete. 
            DAP_ERR_SESSION = -39, //	The session handle is bad. 
            DAP_ERR_SOCKET = -38, //	Low level socket error. 
            DAP_ERR_RX_OVFL = -37, //	Gateway message too long. 
            DAP_ERR_INVALID_R_TERM = -36, //	A reserved field was not terminated. 
            DAP_ERR_INVALID_R = -35, //	Invalid reserved field character. 
            DAP_ERR_INVALID_TYEAR = -34, //	Invalid year in datestamp field. 
            DAP_ERR_INVALID_TMONTH = -33, //	Invalid month in datestamp field. 
            DAP_ERR_INVALID_TDAY = -32, //	Invalid day in datestamp field. 
            DAP_ERR_INVALID_THOUR = -31, //	Invalid hour in datestamp field. 
            DAP_ERR_INVALID_TMINUTE = -30, //	Invalid minute in datestamp field. 
            DAP_ERR_INVALID_TSECOND = -29, //	Invalid second in datestamp field. 
            DAP_ERR_INVALID_T_TERM = -28, //	A timestamp field was not terminated. 
            DAP_ERR_INVALID_T = -27, //	Invalid timestamp field characters. 
            DAP_ERR_CONNECT_CLOSED = -26, //	Socket connection closed. 
            DAP_ERR_WINSOCK = -25, //	Windows socket library failed to initialize. 
            DAP_ERR_TCP_PORT = -24, //	Bad TCP port number given for gateway server. 
            DAP_ERR_UNKNOWN_HOST = -23, //	Unknown host specified for gateway server. 
            DAP_ERR_MESSAGE_CATEGORY = -22, //	Illegal message category. 
            DAP_ERR_CONNECT_FAILED = -21, //	Connection to gateway server failed. 
            DAP_ERR_DEFERRED = -20, //	Illegal deferred time in submit request. 
            DAP_ERR_REJECTED = -19, //	Request rejected by gateway, see response code. 
            DAP_ERR_INVALID_N = -18, //	Invalid characters in numeric field. 
            DAP_ERR_INVALID_N_TERM = -17, //	Numeric field badly terminated. 
            DAP_ERR_INVALID_A_TERM = -16, //	Alphanumeric field badly terminated. 
            DAP_ERR_INVALID_A = -15, //	Invalid characters in alphanumeric field. 
            DAP_ERR_RX_TIMEOUT = -14, //	Timeout waiting for gateway message. 
            DAP_ERR_RX_ERR = -13, //	Socket error receiving gateway message. 
            DAP_ERR_TX_TIMEOUT = -12, //	Timeout waiting for message transmission. 
            DAP_ERR_TX_ERR = -11, //	Socket error transmitting message. 
            DAP_ERR_INVALID_CHECKSUM = -10, //	Bad message checksum. 
            DAP_ERR_INVALID_TRAILER = -9, //	Message trailer contains bad characters. 
            DAP_ERR_RESPONSE_FLAG = -8, //	Illegal character in response flag. 
            DAP_ERR_OP_CODE = -7, //	Illegal operation code in header or operation code mismatch in response. 
            DAP_ERR_OP_TYPE = -6, //	Illegal operation type in header or operation type mismatch in response. 
            DAP_ERR_TXN_REF = -5, //	Transaction reference number mismatch in header. 
            DAP_ERR_INVALID_HEADER = -4, //	The message header is contains bad characters. 
            DAP_ERR_SPARE = -3, //	Spare error message. 
            DAP_ERR_PAYLOAD_ILLEGAL = -2, //	Message payload contains bad characters. 
            DAP_ERR_PAYLOAD_TOO_BIG = -1, //	Too many characters in payload. 
            DAP_SUCCESS = 0, //	All OK. 
            DAP_WARN_ALERT_CODE = 1, //	Alert code illegal, ignored. 
            DAP_WARN_INFO_TYPE = 2, //	Information type invalid, ignored. 
            DAP_WARN_PRIORITY = 3, //	Priority invalid, ignored. 
            DAP_WARN_NOTIFICATION = 4, //	Notification request invalid or not supported for current transport, ignored. 
            DAP_WARN_SUBMESSAGE_COUNTER = 5, //	Submsg out of range or not supported for current transport, ignored. 
            DAP_WARN_REPEATS = 6, //	Repeat count bad or not supported for current transport, ignored. 
            DAP_WARN_REPEAT_INTERVAL = 7, //	Repeat interval illegal, ignored. 
            DAP_WARN_TERMINAL_ACK = 8, //	Terminal acknowledge request value invalid, ignored. 
            DAP_WARN_PAYLOAD = 9, //	Payload in submit tone-only, ignored. 
            DAP_WARN_NO_SERVER = 10, //	No gateway server specified for session. 
            DAP_WARN_NO_PORT = 11, //	No gateway server TCP port specified. 
            DAP_WARN_MESSAGE_ID = 12, //	A message identifier was specified for a negative acknowledgement. 
            DAP_WARN_TXN_REF = 13, //	Operation transaction reference is out of sequence. 
            DAP_WARN_LENGTH = 14, //	Message header length incorrect. 
            DAP_WARN_MESSAGE_NUMBER = 15, //	Msgnum out of range or not supported for current transport, ignored. 
            DAP_WARN_LIFETIME = 16, //	Lifetime out of range or not supported for current transport, ignored. 
            DAP_WARN_VALIDITY_TIME = 17, //	Validity time not supported for current transport, ignored. 
            DAP_WARN_XML_RESPONSE = 18, //	Host response contains additional XML, ignored. 
            DAP_WARN_CONCURRENT_ACCESS = 19, //	Session handle accessed from multiple threads. 
            DAP_WARN_INVALID_T = 20, //	Invalid time in response. 
        }

        public enum DAP_CONNECT
        {
            DAP_TCP = 0,
            DAP_SSL = 1,
            DAP_XML = 16,
        }

        public enum RegistrationRequestType
        {
            RegistrationRequestNone = 0,
            RegistrationRequestAuthen = 1,
            RegistrationRequestDelivery = 2,
        }

        public enum MessageType
        {
            MessageTypeNone = 0,
            MessageTypeTone = 1,
            MessageTypeNumeric = 2,
            MessageTypeAlphanumeric = 3,
            MessageTypeTransparent = 4,
        }

        public enum CommandCode
        {
            CommandCodeNone = -1,
            CommandCodeDefault = 0,
            CommandCodeZero = 0,
            CommandCodeOne = 1,
            CommandCodeTwo = 2,
            CommandCodeThree = 3,
        }

        public enum InformationType
        {
            InformationTypeNone = -1,
            InformationTypeDefault = 0,
            InformationTypeExternal = 0,
            InformationTypeZero = 0,
            InformationTypeOne = 1,
            InformationTypeTwo = 2,
            InformationTypeThree = 3,
            InformationTypeFour = 4,
            InformationTypeFive = 5,
            InformationTypeSix = 6,
            InformationTypeSeven = 7,
            InformationTypeEight = 8,
            InformationTypeNine = 9,
            InformationTypeTen = 10,
            InformationTypeEleven = 11,
            InformationTypeTwelve = 12,
            InformationTypeThirteen = 13,
            InformationTypeFourteen = 14,
            InformationTypeFifteen = 15,
            InformationTypeInternal = 15
        }

        public enum Priority
        {
            PriorityNone = -1,
            PriorityLow = 1,
            PriorityDefault = 2,
            PriorityNormal = 2,
            PriorityHigh = 3,
        }

        public enum DapBoolean
        {
            DapBooleanNone = -1,
            DapBooleanFalse = 0,
            DapBooleanTrue = 1,
        }

        public enum MessageStatus
        {
            MessageStatusProcessing = 1,
            MessageStatusStored = 2, // No longer used
            MessageStatusDeferred = 3,
            MessageStatusTransmitting = 4, // No longer used
            MessageStatusTransmitted = 5,
            MessageStatusAcknowledged = 6,
            MessageStatusNotAcknowledged = 7,
            MessageStatusExpired = 8,
            MessageStatusCancelled = 9,
            MessageStatusCancelledXmit = 10, // No longer used
            MessageStatusRejected = 11,
            MessageStatusExpiredXmit = 12, // No longer used
            MessageStatusCancelledRecipient = 13, // No longer used
            MessageStatusRetrievedRecipient = 14, // No longer used
            StatusQueued = 101, // DAP-XML - MessageStatusProcessing
            StatusDeferred = 103, // MessageStatusDeferred
            StatusCompleted = 105, // MessageStatusTransmitted
            StatusAcknowledged = 106, // MessageStatusAcknowledged
            StatusNotAcknowledged = 107, // MessageStatusNotAcknowledged
            StatusExpired = 108, // MessageStatusExpired
            StatusCancelled = 109, // MessageStatusCancelled
            StatusRejected = 111, // MessageStatusRejected
        }

        public enum ResponseCode
        {
            ResponseCodeNone = -1,
            ResponseCodeOK = 0,
            ResponseCodeChecksum = 1,
            ResponseCodeFormat = 2,
            ResponseCodeOperationNotSupported = 3,
            ResponseCodeNotAllowed = 4,
            ResponseCodeCallBarred = 5,
            ResponseCodeDestinationInvalid = 6,
            ResponseCodeRegionInvalid = 9,
            ResponseCodeTimeInvalid = 22,
            ResponseCodeMsgTypeInvalid = 23,
            ResponseCodeMsgTooLong = 24,
            ResponseCodeMsgNotFound = 27,
            ResponseCodeMsgAlreadyDone = 28,
            ResponseCodeTransidInvalid = 30,
            ResponseCodeParameterMissing = 31,
            ResponseCodeAccessDenied = 32,
            ResponseCodeAuthorizationFailed = 33,
            ResponseCodeReplyIDInvalid = 34,
            ResponseCodeRepetitionDenied = 40,
            ResponseCodePriorityDenied = 41,
            ResponseCodeInfoTypeDenied = 42,
            ResponseCodeDeferredDenied = 43,
            ResponseCodeTerminalAckDenied = 44,
            ResponseCodeNotifyIDInvalid = 45,
            ResponseCodeRegionDenied = 46,
            ResponseCodeMsgTypeDenied = 47,
            ResponseCodeRegionRequired = 48,
            ResponseCodeMaxMsgsExceeded = 49,
            ResponseCodeServerBusy = 50,
            CodeOK = 100, // DAP-XML
            CodeQueued = 101,
            CodeDeferred = 103,
            CodeCompleted = 105,
            CodeAckReceived = 106,
            CodeNoAckReceived = 107,
            CodeExpired = 108,
            CodeCancelled = 109,
            CodeRejected = 111,
            CodeUndefined = 200,
            CodeBadXML = 201,
            CodeBadXMLResponse = 202,
            CodeAccessDenied = 203,
            CodeDatabaseError = 204,
            CodeSyntaxError = 205,
            CodeOperationNotSupported = 206,
            CodeServerBusy = 207,
            CodeMaxMsgsExceeded = 208,
            CodeBadAdC = 210,
            CodeMsgNotFound = 211,
            CodeRegionInvalid = 212,
            CodeInvalidMsgData = 213,
            CodeInvalidMsgCategory = 214,
            CodeAlertCodeInvalid = 215,
            CodeInfoTypeInvalid = 217,
            CodeMsgnumInvalid = 218,
            CodeSubmsgInvalid = 219,
            CodeMsgTooLong = 220,
            CodePriorityInvalid = 221,
            CodeCancelFailed = 222,
            CodeTimeInvalid = 223,
            CodePriorityDenied = 250,
            CodeRegionDenied = 251,
        }

        public enum BurstType
        {
            BurstTypeNone = -1,
            BurstTypeShort = 0,
            BurstTypeLong = 1,
        }
        public DAPEnums()
        {
        }

        public enum DAP_Kind
        {
            Tone_Default,
            Poll_Default,
            Other,
        }


        



    }


}
