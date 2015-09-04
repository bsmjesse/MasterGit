using System;
using System.Collections;
using System.Text;
using VLF.CLS.Def;
using VLF.ERR;
using System.Collections.Generic;

namespace VLF.CLS
{
   public static class Definitions
   {
      public static ushort[] PayloadLength = 
      {     0,             //  None                = 0,
            1,             // Ack                  = 1,
           16,             // BoxModemPhoneNumber  = 2,
            3,             // NewBoxID             = 3,
            4,             // ServerIPAddress      = 4,
            2,             // ServerPort           = 5,
            4,             // BoxIPAddress         = 6,
            2,             // BoxPort              = 7,
            1,             // CommunicationMode    = 8,
          255,             // SimEsn               = 9,
            1,             // IPUpdateReason       =10,
            1,             // IPUpdateParameter    =11,
            2,             // AppName              =12,
            2,             // AppType              =13,
            8,             // AppFeatureMask       =14,
            9,             // VirtualSensorCfg     =15,
            2,             // EnabledSensorsMask   =16,
            2,             // AllStatusSensorsMask =17,
            1,             // SensorStatus         =18,
            1,             // SensorName           =19,
            2,             // TracePeriod          =20,
            2,             // TraceInterval        =21,
            4,             // TraceEnabledSensorsMask =
            4,             // MainBattery          =23,
            4,             // BackupBattery        =24,
            1,             // AnalogSensorName     =25,
            1,             // AnalogSensorValue    =26,
            1,             // PhysOutputNumber     =27,
            1,             // OutputFunction       =28,
            1,             // VirtualOutputID      =29,
            2,             // VirtualOutputCfg     =30,
            4,             // Odometer             =31,
            1,             // Speed                =32,
            1,             // ArmStatus            =33,
            1,             // PowerStatus          =34,
          255,             // SerialNumber         =35,
            4,             // IdlingTime           =36,
            1,             // SpeedThreshold       =37,
            1,             // RemoteControlSettings=38,
            1,             // RemoteControlStatus  =39,
            2,             // GeoFenceRadius       =40,
            3,             // GPSReportingInterval =41,
            1,             // GPSStatus            =42,
            1,             // GPSAntennaStatus     =43,
            4,             // Latitude             =44,
            4,             // Longitude            =45,
            1,             // Heading              =46,
            1,             // Direction            =47,
            3,             // DateTime             =48,
            2,             // GeoZoneID            =49,
            1,             // GeoZoneType          =50,
            1,             // GeoZoneCounter       =51,
            1,             // VCRStatus            =52,
            1,             // VCROffDelay          =53
            1,             // BatteryStatusNotification =54
          244              // EEPROMData           =55
      };

      public enum PayloadID : ushort
      {
         None = 0,
         AckStatus = 1,
         BoxModemPhoneNumber = 2,
         NewBoxID = 3,
         ServerIPAddress = 4,
         ServerPort = 5,
         BoxIPAddress = 6,
         BoxPort = 7,
         CommunicationMode = 8,
         SimEsn = 9,
         IPUpdateReason = 10,
         IPUpdateParameter = 11,
         AppName = 12,
         AppType = 13,
         AppFeatureMask = 14,
         VirtualSensorCfg = 15,
         EnabledSensorsMask = 16,
         AllStatusSensorsMask = 17,
         SensorStatus = 18,
         SensorName = 19,
         TracePeriod = 20,
         TraceInterval = 21,
         TraceEnabledSensorsMask = 22,
         MainBattery = 23,
         BackupBattery = 24,
         AnalogSensorName = 25,
         AnalogSensorValue = 26,
         PhysOutputNumber = 27,
         OutputFunction = 28,
         VirtualOutputID = 29,
         VirtualOutputCfg = 30,
         Odometer = 31,
         Speed = 32,
         ArmStatus = 33,
         PowerStatus = 34,
         SerialNumber = 35,
         IdlingTime = 36,
         SpeedThreshold = 37,
         RemoteControlSettings = 38,
         RemoteControlStatus = 39,
         GeoFenceRadius = 40,
         GPSReportingInterval = 41,
         GPSStatus = 42,
         GPSAntennaStatus = 43,
         Latitude = 44,
         Longitude = 45,
         Heading = 46,
         Direction = 47,
         DateTime = 48,
         GeoZoneID = 49,
         GeoZoneType = 50,
         GeoZoneCounter = 51,
         Date = 52,
         GeoZoneBrokenType	 = 53,
         //InternalConfig = 54,
         BatteryStatusNotification = 54,
         EEPROMData = 55,
         SpeedDuration = 56,
         HarshAcceleration = 57,
         HarshBraking = 58,
         ExtremeAcceleration = 59,
         ExtremeBraking = 60,
         StandardMessage = 61,
         FirmwareVersion = 62,
         StoredMessage = 63,
         GeoZoneIDList = 64,
         GeoZoneCoordinates = 65
     }
     public enum ReeferPayloadID : ushort
     {
         None                       = 0,
         AckStatus                  = 1,
         Wrapper                    = 2,
         ReportingInterval          = 3,
         TemperatureThresholdSetup  = 4,
         FuelThresholdSetup         = 5,
         SensorsEnableMask          = 6,
         SensorsStatus              = 7,
         TemperatureZoneStatus      = 8,
         FuelStatus                 = 9,
         SensorAlarm                = 10,
         TemperatureZoneAlarm       = 11,
         FuelAlarm                  = 12,
         FeatureMask                = 13,
         FirmwareVersion            = 14,
         TemperatureZoneCheckingTime= 15,
         MainBatteryVoltage         = 16,
         FuelLevelRiseDropAlarm     = 17,
         FuelLevelRiseDropSetup     = 18
     }

     // Message Ids from Server to BOX:
      public enum CommandType : byte
      {
         // Message name	Message ID
         Empty = 0xff,
         ACK = 0,
         SetBoxSetup = 1,
         GetBoxSetup = 2,
         GetBoxStatus = 3,
         ChangeBoxID = 4,
         SetReportInterval = 5,
         SetSpeedThreshold = 6,
         SetGeoFence = 7,
         SetEnabledSensor = 8,
         SetOutput = 9,
         UpdateGPSPosition = 10,
         Arm = 11,
         Disarm = 12,
         ClearMemory = 13,
         AddGeoZone = 14,
         DeleteGeoZones = 15,
         GetGeoZoneIDs = 16,
         GetGeoZoneSetup = 17,
         Reset = 18,
         SetOdometer = 19,
         KeyFobSetup = 20,
         StartOTAProcess = 21,
         StartOTAPlusProcess    = 22,
         WriteEEPROMData        = 23,
         ReadEEPROMData         = 24,
         KeyFobGetStatus        = 25
      }

      // Message Ids from BOX to Server:
      public enum MessageType : byte
      {
         // Message name	Message ID
         Empty                  = 0xff,
         ACK                    = 0,
         IPAddressUpdate        = 1,
         SendGPSPosition        = 2,
         ScheduledGPSPosition   = 3,
         SensorStatus           = 4,
         Alarm	                = 5,
         PowerStatus            = 6,
         Speeding               = 7,
         Idling                 = 8,
         KeyFobArm              = 9,
         KeyFobDisarm           = 10,
         KeyFobPanic            = 11,
         SendKeyfobStatus       = 12,
         GPSAntennaStatus       = 13,
         SendBoxStatus          = 14,
         SendBoxSetup           = 15,
         GeoZoneBroken          = 16,
         SendGeoZoneIDs         = 17,
         SendGeoZoneSetup       = 18,
         SendEEPROMData         = 19
      }

      public static byte[] StringArrayAsHexDumpToChar(string dump, int length)
      {
         byte[] bBuff = new byte[length];
         string[] split = dump.Split(new Char[] { ' ' });

         for (int i = 0; i < length; i++)
            bBuff[i] = Convert.ToByte(split[i], 16);

         return bBuff;
      }

   }


   // Declare the Tokens class:
   public class Tokens : IEnumerable
   {
      private string[] elements;

      Tokens(string source, char[] delimiters)
      {
         // Parse the string into tokens:
         elements = source.Split(delimiters);
      }

      // IEnumerable Interface Implementation:
      //   Declaration of the GetEnumerator() method 
      //   required by IEnumerable
      public IEnumerator GetEnumerator()
      {
         return new TokenEnumerator(this);
      }


      // Inner class implements IEnumerator interface:
      private class TokenEnumerator : IEnumerator
      {
         private int position = -1;
         private Tokens t;

         public TokenEnumerator(Tokens t)
         {
            this.t = t;
         }

         // Declare the MoveNext method required by IEnumerator:
         public bool MoveNext()
         {
            if (position < t.elements.Length - 1)
            {
               position++;
               return true;
            }
            else
            {
               return false;
            }
         }

         // Declare the Reset method required by IEnumerator:
         public void Reset()
         {
            position = -1;
         }

         // Declare the Current property required by IEnumerator:
         public object Current
         {
            get
            {
               return t.elements[position];
            }
         }
      }
   }

    public class PayloadItem
    {
        #region private variables.
        private ushort id;
        private byte dataLength;
        private byte[] data;
        #endregion
        #region private variables.
        public ushort Id
        {
            get { return id; }
        }
        public byte DataLength
        {
            get { return dataLength; }
        }
        public byte[] Data
        {
            get { return data; }
        }
        #endregion
        public PayloadItem(byte[] packet, ref int startIndex)
        {
            // minimum size check.
            if (packet.Length - startIndex < BuildPacket.EMPTY_PAYLOAD_SIZE)
            {
                throw new PASProtocolException(string.Format("Invalid payload at {0}, minimum {1} byte(s) required", startIndex, BuildPacket.EMPTY_PAYLOAD_SIZE));
            }
            // id
            ushort tempId = BitConverter.ToUInt16(packet, startIndex);
            // do not discard unknown PID as requested.
            /*
            if (!Enum.IsDefined(typeof(Definitions.PayloadID), tempId))
            {
                throw new PASProtocolException(string.Format("Payload Id {0} at {1} not defined yet", tempId, startIndex));
            }
            */ 
            id = tempId;
            startIndex += 2;
            // data length            
            dataLength = packet[startIndex];
            startIndex++;
            // data
            if (dataLength > 0)
            {
                if (packet.Length - startIndex < dataLength)
                {
                    throw new PASProtocolException(string.Format("Insufficient payload data at {0}, {1} byte(s) required", startIndex, dataLength));
                }
                data = new byte[dataLength];
                Array.Copy(packet, startIndex, data, 0, dataLength);
                startIndex += dataLength;
            }
        }
    }
    public class EEPROMData
    {
        #region const
        public const int EEPROM_SIZE = 4096; // 4K bytes, 0x000 - 0xfff.
        public const int MIN_LENGTH = 1;
        public const int MAX_LENGTH = 128;
        public const char SEPARATOR = ' ';
        #endregion
        #region private variables.
        private ushort offset;
        private byte length;
        private byte[] data;
        private char[] Seperator = { SEPARATOR };
        #endregion
        #region properties.
        public ushort Offset
        {
            get { return offset; }
            set { offset = value; }
        }
        public byte Length
        {
            get { return length; }
            set { length = value; }
        }
        public byte[] Data
        {
            get { return data; }
            set { data = value; }
        }
        #endregion
        public EEPROMData()
        {
        }
        public void Decode(string customProperties, Enums.CommandType commandType)
        {
            bool decoded = false;
            string key;
            string value;
            // offset
            key = Const.keyEEPROMOffset;
            value = Util.PairFindValue(key, customProperties);
            if (string.IsNullOrEmpty(value))
            {
                throw new PASProtocolException(key + " not found");
            }
            try
            {
                offset = Convert.ToUInt16(value, 16);
            }
            catch
            {
                throw new PASProtocolException(string.Format("Invalid {0}: {1}", key, value));
            }
            // length
            key = Const.keyEEPROMLength;
            value = Util.PairFindValue(key, customProperties);
            if (string.IsNullOrEmpty(value))
            {
                throw new PASProtocolException(key + " not found");
            }
            try
            {
                length = Convert.ToByte(value);
            }
            catch
            {
                throw new PASProtocolException(string.Format("Invalid {0}: {1}", key, value));
            }
            if (length < MIN_LENGTH || length > MAX_LENGTH)
            {
                throw new PASProtocolException(string.Format("Invalid {0}: {1} out of range({2} - {3})", key, value, MIN_LENGTH, MAX_LENGTH));
            }
            // in range test
            if (offset + length > EEPROM_SIZE)
            {
                throw new PASProtocolException(string.Format("Out of range: {0} + {1} > {2}", offset, length, EEPROM_SIZE));
            }
            // data, not present for read EEPROM.
            if (commandType == Enums.CommandType.WriteEEPROMData)
            {
                key = Const.keyEEPROMData;
                value = Util.PairFindValue(key, customProperties);
                if (string.IsNullOrEmpty(value))
                {
                    throw new PASProtocolException(key + " not found");
                }
                string[] itemList = value.Split(Seperator, StringSplitOptions.RemoveEmptyEntries);
                if (itemList.Length != length)
                {
                    throw new PASProtocolException(string.Format("Invalid {0}: not in length of {1}", key, length));
                }
                data = new byte[length];
                for (int i = 0; i < itemList.Length; i++)
                {
                    try
                    {
                        data[i] = Convert.ToByte(itemList[i], 16);
                    }
                    catch (Exception ex)
                    {
                        throw new PASProtocolException(string.Format("Invalid {0} at position {1}: {2}", key, itemList[i], ex.Message));
                    }
                }
            }
        }
        public byte[] ToPayloadData()
        {
            byte[] payloadData = null;
            int bufferSize = 3;
            if (data != null)
            {
                bufferSize += length;
            }
            payloadData = new byte[bufferSize];
            byte[] temp;
            int i;
            // offset
            i = 0;
            temp = BitConverter.GetBytes(offset);
            temp.CopyTo(payloadData, i);
            i += temp.Length;
            // length            
            payloadData[i] = length;
            i++;
            // data, not present for read EEPROM.
            if (data != null)
            {
                data.CopyTo(payloadData, i);
            }
            return payloadData;
        }
        // to customProperties.
        public string ToCustomProperties()
        {
            string customProperties = null;
            customProperties = Util.MakePair(Const.keyEEPROMOffset, string.Format("0X{0:X3}", offset));
            customProperties += Util.MakePair(Const.keyEEPROMLength, length.ToString());            
            StringBuilder sb = new StringBuilder(length * 3);
            foreach (byte b in data)
            {
                sb.AppendFormat("{0:X2}{1}", b, SEPARATOR);
            }
            customProperties += Util.MakePair(Const.keyEEPROMData, sb.ToString(0, sb.Length - 1));
            return customProperties;
        }
        public void Decode(byte[] packet, int startIndex)
        {
            try
            {
                // offset
                offset = BitConverter.ToUInt16(packet, startIndex);
                startIndex += 2;
                // length
                length = packet[startIndex];
                startIndex++;
                // data
                data = new byte[length];
                Array.Copy(packet, startIndex, data, 0, length);
            }
            catch (Exception ex)
            {
                throw new PASProtocolException(string.Format("Failed to decode EEPROM data from packet: {0}", ex.Message));
            }
        }
    }
    /*
     * below is the general format of the packet
     * Start	Packet Len	Packet Check Sum	Packet Sequence Number	Box ID	        Msg ID	    PID	        PID Data Len	PID Data	...	End
     * “(”	0x0 -0xFFFF	0x0 -0xFF	        0x0 -0xFF	            0x0 -0xFFFFFF	0x0 -0xFF	0x0 -0xFFFF	0x0 -0xFF			            “)”
     * B0	B1-2	    B3	                B4	                    B5-7	        B8	        B9-10	    B11	            B12	        ...	...
     * */
   public class BuildPacket
   {
      public const byte  SOP = 0x28 ;
      public const byte  EOP = 0x29 ;
      public const int MIN_XS_PACKET_SIZE = (int)FieldOffset.PayloadId + EMPTY_PAYLOAD_SIZE + TAIL_SIZE;
      public const int EMPTY_PAYLOAD_SIZE = 3;      // the size of an empty payload
      public enum FieldOffset
      {
          Start             = 0,
          Length            = 1,
          Checksum          = 3,
          SequenceNumber    = 4,
          SecurityToken     = 5,
          BoxId             = 7,
          DeviceType        = 10,
          DeviceId          = 11,
          MessageId         = 12,
          PayloadId         = 13,
          PayloadDataLength = 15,
      }

      const int   MIN_PACKET_SIZE = 64;        // this should be an average of the packets received in the system
      const int   MAX_PACKET_SIZE = 256;        // max length of the packet 
      const int HEADER_FOOTER_SIZE = MIN_XS_PACKET_SIZE;     // size of the wrapper
      static byte[] EMPTY_PAYLOAD = new byte[] { 0, 0, 0 };      
      const int   TAIL_SIZE = 1;               // the wrapper at the end of the packet ; 
      const int   FIRST_PID_POSITION = (int)FieldOffset.PayloadId;      // where the first PID starts
      const int   MAX_PIDS = 32;               // maximum number of PIDs in a packet

      private int _boxId = 0;
      Enums.DeviceType _deviceType;
      byte _deviceId = 0;
      private ushort _msgId;
      private byte[] _payload = null;
      private int _index = 0;            // keep score of the space used from the payload
      private byte _sequenceNumber = 0;
      private ushort _securityToken = 0;
      private List<PayloadItem> payloadList;
      #region properties
      public int BoxId
      {
          get { return _boxId; }
      }
      public Enums.DeviceType DeviceType
      {
           get { return _deviceType; }
      }
      public byte DeviceId
      {
          get { return _deviceId; }
      }
      public ushort MessageId
      {
          get { return _msgId; }
      }
      public byte SequenceNumber
      {
          get { return _sequenceNumber; }      
      }
      public ushort SecurityToken
      {
          get { return _securityToken; }
      }
      public List<PayloadItem> PayloadList
      {
          get { return payloadList; }
      }      
      #endregion
      // encoder, construct packet buffer.
      public BuildPacket(byte sequenceNumber, int boxId, ushort msgId)
      {
         _sequenceNumber = sequenceNumber;
         _boxId = boxId;
         _msgId = msgId;
         _payload = new byte[MIN_PACKET_SIZE];
         _payload[0] = SOP;
         _payload[(int)FieldOffset.SequenceNumber] = _sequenceNumber;
         BitConverter.GetBytes(_securityToken).CopyTo(_payload, (int)FieldOffset.SecurityToken);
         Util.PackIntToByteArrayLSB((ulong)_boxId, _payload, (int)FieldOffset.BoxId, 3);
         _payload[(int)FieldOffset.MessageId] = (byte)_msgId;
         _index = FIRST_PID_POSITION;
      }
       public BuildPacket(byte sequenceNumber, int boxId, ushort msgId, Enums.DeviceType deviceType) : this(sequenceNumber, boxId, msgId, deviceType, 0)
       {
       }
       public BuildPacket(byte sequenceNumber, int boxId, ushort msgId, Enums.DeviceType deviceType, byte deviceId)
           : this(sequenceNumber, boxId, msgId)
       {
           _deviceType = deviceType;
           _deviceId = deviceId;
           _payload[(int)FieldOffset.DeviceType] = (byte)_deviceType;
           _payload[(int)FieldOffset.DeviceId] = _deviceId;
       }

      // decoder, construct packet structure.
      public BuildPacket(byte[] packet)
      {
          int index = 0;
          // parse packet
          int length = packet.Length;

          if (length < MIN_XS_PACKET_SIZE)
              throw new PASProtocolException("Protocol error: length " + length);

          // Check SOP/EOP
          if ((packet[index] != (byte)SOP) || (packet[length - 1] != (byte)EOP))
              throw new PASSOPEOPException("Either SOP or EOP are not found.");
          index++;
          // packet length
          int packetLength = BitConverter.ToUInt16(packet, index);
          if (packetLength != length)
          {
              throw new PASProtocolException("Packet length mismatch");
          }
          index += 2;
          // Calculate and verify checksum
          byte packetChSum = packet[index];
          byte actChSum = Util.CalcChecksum3(packet, index + 1, length - 2);
          if (packetChSum != actChSum)
              throw new PASProtocolChecksumException("Checksum error.");
          index++;
          // sequence number
          _sequenceNumber = packet[index];
          index++;
          // security token
          _securityToken = BitConverter.ToUInt16(packet, index);
          index += 2;
          // box id
          _boxId = 0;
          for (int i = 0; i < 3; i++)
          {
              _boxId <<= 8;
              _boxId += packet[index + 2 - i];
          }
          index += 3;
          // device type
          _deviceType = (Enums.DeviceType)packet[index];
          index++;
          // device id
          _deviceId = packet[index];
          index++;
          // message id
          _msgId = packet[index];
          index++;
          // Get payloads
          payloadList = new List<PayloadItem>();
          while (index < length - TAIL_SIZE)
          {
              PayloadItem payloadItem = new PayloadItem(packet, ref index);
              payloadList.Add(payloadItem);
          }
      }
       public override string ToString()
       {
           StringBuilder sb = new StringBuilder();
           sb.AppendFormat("Sequence Number {0}, Security Token {1}, Box Id {2}, Message Id {3}{4}", _sequenceNumber, _securityToken, _boxId, _msgId, Environment.NewLine);
           foreach (PayloadItem item in payloadList)
           {
               sb.AppendFormat("Payload Id {0}, Data Length {1}, Data {2}{3}", item.Id, item.DataLength, Util.ByteArrayAsHexDumpToString(item.Data), Environment.NewLine);
           }
           return sb.ToString();
       }
      /// <summary>
      ///      add the header: ( PacketLength PacketCheckSum _sequenceNumber _boxId _msgId _payload )
      ///      and returns the array of bytes
      ///      don't check on anything because for every 
      /// </summary>
      /// <returns></returns>
      public byte[] GetPacket()
      {
         byte[] temp = new byte[TAIL_SIZE + _index];
         Array.Copy(_payload, temp, temp.Length);
         // EOP, last byte.
         temp[_index] = EOP;
         _index++;
         // packet length, 2 bytes.
         Util.PackIntToByteArrayLSB((ulong)_index, temp, (int)FieldOffset.Length, 2);  // size of the packet
         // checksum, 1 byte.
         temp[(int)FieldOffset.Checksum] = Util.CalcChecksum3(temp, (int)FieldOffset.SequenceNumber, _index - 2);
         return temp;
      }

      private void SafeIncrease(int size)
      {
         int newSize = (null == _payload) ? size : size + _payload.Length;
         byte[] temp = new byte[newSize];
         Array.Copy(_payload, temp, _payload.Length);
         _payload = temp;
      }

      private void IncreaseBuffer(int size)
      {
         if (size + _index > MAX_PACKET_SIZE - TAIL_SIZE)
            throw new ApplicationException("BuildPacket.IncreaseBuffer: size larger than 256");

         // try to add MIN_PACKET_SIZE, more than you need 
         if (MIN_PACKET_SIZE > size)
         {
            if ((_index + MIN_PACKET_SIZE) < (MAX_PACKET_SIZE - TAIL_SIZE))
            {
               SafeIncrease(MIN_PACKET_SIZE);
               return;
            }
         }

         SafeIncrease(size);
      }


      /// <summary>
      ///      appending to the existing payload 
      /// </summary>
      /// <comment> 
      ///   // this could throw an error if the payload is larger than MAX_PACKET_SIZE - TAIL_SIZE
      ///   Payload format: Id(2) Length(1) Data(varible)
      /// </comment>
      /// <param name="id"></param>
      /// <param name="payloadData"></param>
      private void AddPayload(ushort id, byte[] payloadData)
      {
          int payloadDataLength = 0;
          if (!(id == 0 || payloadData == null))
          {
              payloadDataLength = payloadData.Length;
          }
          int increaseBy = EMPTY_PAYLOAD_SIZE + payloadDataLength;
          byte[] temp;// = (null != payloadData && payloadData.Length > 0) ? payloadData : EMPTY_PAYLOAD;
          if (increaseBy + _index > _payload.Length)
              IncreaseBuffer(increaseBy);      // increase the _payload to accomodate the new payload
          // you have enough space to copy data
          // payload id
          temp = BitConverter.GetBytes(id);
          temp.CopyTo(_payload, _index);
          _index += temp.Length;
          // payload data length
          _payload[_index] = (byte)payloadDataLength;
          _index++;
          // payload data         
          //Array.Copy(temp, 0, _payload, _index, increaseBy);
          if (payloadDataLength > 0)
          {
              payloadData.CopyTo(_payload, _index);
              _index += payloadData.Length;
          }
      }        
      public void AddPayload(Definitions.PayloadID id, byte[] payloadData)
      {
          AddPayload((ushort)id, payloadData);
      }
      public void AddPayload(Definitions.ReeferPayloadID id, byte[] payloadData)
      {
          AddPayload((ushort)id, payloadData);
      }
      public void AddPayload(Definitions.ReeferPayloadID id, CMFOut cmfOut)
      {
          byte[] payloadData = BuildReeferPayloadData(id, cmfOut);
          AddPayload((ushort)id, payloadData);
      }
      public void AddPayload(Definitions.ReeferPayloadID[] idList, CMFOut cmfOut)
      {
          foreach (Definitions.ReeferPayloadID id in idList)
          { 
              AddPayload(id, cmfOut);
          }
      }
      /// <summary>
      ///         just a way to build packets with 0 payloads
      /// </summary>
      /// <param name="id"></param>
      /// <comment> 
      ///   this was only used by HGIV80PAS, before the sequence number is introduced.
      ///   untouched to not break existing module used in production.
      /// </comment>
      public static byte[] SimplePayload(int boxid, Definitions.CommandType msgid)
      {
         byte[] temp = new byte[] { SOP, 0xC, 0, 0, 0, 0, 0, 0, 0, 0, 0, EOP };
         Util.PackIntToByteArrayLSB((ulong)boxid, temp, 4, 3);  // boxid 
         temp[7] = (byte)msgid;
         temp[3] = Util.CalcChecksum2(temp, 4, 10);
         return temp;
      }

      // this is called after all 
      public static void GetPIDs(out ushort[] PIDs, out ushort[] startFrom, byte[] packet)
      {
         PIDs = null;
         startFrom = null;
         if (null != packet && packet.Length >= HEADER_FOOTER_SIZE)
         {
            ushort[] listPIDs = new ushort[MAX_PIDS];
            ushort[] listPositions = new ushort[MAX_PIDS];
            int idx = FIRST_PID_POSITION,
                cnt = 0;
            while (idx < packet.Length - 1)
            {
               listPIDs[cnt] = Util.UshortFromByteArray(packet, idx, 2);
               listPositions[cnt] = (ushort)idx;
               idx += packet[idx + 2] + 1;
               if (++cnt > MAX_PIDS)
                  throw new IndexOutOfRangeException(" BuildPacket.GetPIDs -> cnt > MAX_PIDS " + MAX_PIDS);
            }
            Array.Resize(ref listPIDs, cnt);
            Array.Resize(ref listPositions, cnt);
            PIDs = listPIDs;
            startFrom = listPositions;
         }
      }


      /////////////////////////////////////////////////////////////////////////////////////////////////
      //
      //    here are the functions which translates from one protocol to another
      //
      public static byte[] GetIPAddress(CMFOut cmfOut) // take IP address from custom properties
      {
         byte[] bData = new byte[4];
         uint uiData = 0;
         string tmpStr = Util.PairFindValue(Const.setupIPAddress, cmfOut.customProperties);
         if (tmpStr == "")
            throw new PASProtocolException("Check configuration in DB. For setup command Server IP address should be in DB.");
         try { uiData = (uint)System.Net.IPAddress.Parse(tmpStr).Address; }
         catch { throw new PASProtocolException("Check configuration in DB. Server IP address is not valid: " + tmpStr); }
         Util.PackIntToByteArrayMSB(uiData, bData, 4);
         return bData;
      }

      public static byte[] GetIPPort(CMFOut cmfOut)
      {
         byte[] bData = new byte[2];
         // take Port number from custom properties
         string tmpStr = Util.PairFindValue(Const.setupPort, cmfOut.customProperties);
         if (tmpStr == "")
            throw new PASProtocolException("Check configuration in DB. For setup command Server port should be in DB.");
         ushort usData = 0;
         try { usData = Convert.ToUInt16(tmpStr); }
         catch { throw new PASProtocolException("Check configuration in DB. Server port is not valid: " + tmpStr); }
         Util.PackIntToByteArrayLSB(usData, bData, 2);
         return bData;
      }
      public class CommunicationMode
      {
          [Flags]
          private enum ModeMask : byte
          {
              Primary = 0x01,
              Secondary = 0x02
          }
          private const string DUAL_MODE_STRING = "DUAL";
          private const string GPRS_MODE_STRING = "GPRS";
          private byte mode;
          private const string KEY = Const.setupCommMode;
          public byte Mode
          {
              get { return mode; }
          }
          public void Decode(string customProperties)
          {
              string value = Util.PairFindValue(KEY, customProperties);
              mode = (byte)ModeMask.Primary; 
              if (value == DUAL_MODE_STRING)
              {
                  mode = (byte)ModeMask.Primary | (byte)ModeMask.Secondary;
              }
              else if (value == Enums.CommMode.SAT.ToString())
              {
                  mode = (byte)ModeMask.Secondary;
              }
          }
          public void Decode(byte[] packet, int startIndex)
          {
              mode = packet[startIndex];
          }
          public string ToCustomProperties()
          {
              string customProperties;
              string value = "";
              bool primaryEnalbed = (mode & (byte)ModeMask.Primary) != 0;
              bool secondaryEnabled = (mode & (byte)ModeMask.Secondary) != 0;
              if (primaryEnalbed && secondaryEnabled)
              {
                  value = DUAL_MODE_STRING;
              }
              else if (primaryEnalbed)
              {
                  value = GPRS_MODE_STRING;
              }
              else if (secondaryEnabled)
              {
                  value = Enums.CommMode.SAT.ToString();
              }
              customProperties = Util.MakePair(KEY, value);
              return customProperties;
          }
          public byte[] ToPayloadData()
          {
              byte[] payloadData = new byte[1] { mode };
              return payloadData;
          }
      }
      public static byte[] GetCommunicationMode(CMFOut cmfOut)
      {
          byte[] payloadData = null;
          CommunicationMode communicationMode = new CommunicationMode();
          communicationMode.Decode(cmfOut.customProperties);
          payloadData = communicationMode.ToPayloadData();
          return payloadData;
      }

      public class PhoneNumber
      {
          private const int MAX_LENGTH = 16;
          private const char PADDING_CHAR = '@';
          private string phoneNumber;
          private const string KEY = Const.setupPhoneNumber;
          public string Number
          {
              get { return phoneNumber; }
          }
          public void Decode(string customProperties)
          {
              string value = Util.PairFindValue(KEY, customProperties);
              if (value != "")
              {
                  value = value.Replace(".", "").Replace("-", "");
                  if (value.Length > MAX_LENGTH)
                  {
                      string before = value;
                      value = value.Substring(0, MAX_LENGTH);
                      System.Diagnostics.Trace.WriteLine("WARN:: Phone number cannot be more than 16 characters. Cutting '" + before + "' to '" + value + "'.");
                  }
                  try { Convert.ToInt64(value); }
                  catch
                  {
                      throw new PASProtocolException("Check configuration in DB. Phone number should only consist of digits.");
                  }
                  phoneNumber = value;
              }
          }
          public void Decode(byte[] packet, int startIndex)
          {
              phoneNumber = Encoding.ASCII.GetString(packet).TrimEnd(PADDING_CHAR);
          }
          public string ToCustomProperties()
          {
              string customProperties;
              customProperties = Util.MakePair(KEY, phoneNumber);
              return customProperties;
          }
          public byte[] ToPayloadData()
          {
              byte[] payloadData = Encoding.ASCII.GetBytes(phoneNumber.PadRight(MAX_LENGTH, PADDING_CHAR));
              return payloadData;
          }
      }
      public static byte[] GetPhoneNumber(CMFOut cmfOut)
      {
          byte[] payloadData = null;
          PhoneNumber phoneNumber = new PhoneNumber();
          phoneNumber.Decode(cmfOut.customProperties);
          payloadData = phoneNumber.ToPayloadData();
          return payloadData;
      }

      public class GPSReportingInterval : ReportingInterval
      {
          public GPSReportingInterval(string key)
              : base(key)
          { 
          }
      }
      public class ReportingInterval : IPayloadDataBuilder, ICustomPropertiesBuilder
      {
          private uint interval;
          private string key;
          public uint Interval
          {
              get { return interval; }
          }
          public ReportingInterval(string key)
          {
              this.key = key;
          }
          public void Decode(string customProperties)
          {
              string value = Util.PairFindValue(key, customProperties);
              if (string.IsNullOrEmpty(value))
              {
                  throw new PASProtocolException(key + " not found");
              }
              try
              {
                  interval = Convert.ToUInt32(value);
              }
              catch
              {
                  throw new PASProtocolException(string.Format("Invalid {0}: {1}", key, value));
              }
              DisableIfOutOfRange(ref interval);
          }
          public void Decode(byte[] packet, int startIndex)
          {
              interval = (uint)Util.UlongFromByteArray(packet, startIndex, 3);
              DisableIfOutOfRange(ref interval);
          }
          public string ToCustomProperties()
          {
              string customProperties;
              string value = (interval == 0) ? Const.keyDisabled : interval.ToString();
              customProperties = Util.MakePair(key, value);
              return customProperties;
          }
          public byte[] ToPayloadData()
          {
              byte[] payloadData = new byte[3];
              Util.PackIntToByteArrayLSB(interval, payloadData, 3);
              return payloadData;
          }
          private static void DisableIfOutOfRange(ref uint interval)
          {
              // if GPS Frequency is out of range than make it as disable 
              if ((interval > 0 && interval < 30) || interval > 86400)
              {
                  interval = 0;
              }
          }
      }
      public static byte[] GetGPSReportingInterval(CMFOut cmfOut, string key)
      {
          byte[] payloadData = null;
          GPSReportingInterval gpsReportingInterval = new GPSReportingInterval(key);
          gpsReportingInterval.Decode(cmfOut.customProperties);
          payloadData = gpsReportingInterval.ToPayloadData();
          return payloadData;
      }

      public class TracePeriod
      {
          private const ushort TRACE_FOREVER = 65500;
          private ushort period;
          private const string KEY = Const.setupTracePeriod;
          public uint Period
          {
              get { return period; }
          }
          public void Decode(string customProperties)
          {
              string value = Util.PairFindValue(KEY, customProperties);
              if (string.IsNullOrEmpty(value))
              {
                  throw new PASProtocolException(KEY + " not found");
              }
              try
              {
                  period = Convert.ToUInt16(value);
              }
              catch
              {
                  throw new PASProtocolException(string.Format("Invalid {0}: {1}", KEY, value));
              }
              DisableIfOutOfRange(ref period);
          }
          public void Decode(byte[] packet, int startIndex)
          {
              period = BitConverter.ToUInt16(packet, startIndex);
              DisableIfOutOfRange(ref period);
          }
          public string ToCustomProperties()
          {
              string customProperties;
              string value = (period == 0) ? Const.keyDisabled : ((period == TRACE_FOREVER) ? Const.keyConstantly : period.ToString());
              customProperties = Util.MakePair(KEY, value);
              return customProperties;
          }
          public byte[] ToPayloadData()
          {
              byte[] payloadData = BitConverter.GetBytes(period);
              return payloadData;
          }
          private static void DisableIfOutOfRange(ref ushort period)
          {
              // Trace Period should be either 0 (Disabled) or between 60 and 64800(18 hours)
              // 65500 - forever
              if ((period != 0 && period < 60) || (period > 64800 && period != TRACE_FOREVER))
              {
                  period = 0;
              }
          }
      }
      public static byte[] GetTracePeriod(CMFOut cmfOut)
      {
          byte[] payloadData = null;
          TracePeriod tracePeriod = new TracePeriod();
          tracePeriod.Decode(cmfOut.customProperties);
          payloadData = tracePeriod.ToPayloadData();
          return payloadData;
      }

      public class TraceInterval
      {
          private ushort interval;
          private const string KEY = Const.setupTraceInterval;
          public uint Interval
          {
              get { return interval; }
          }
          public void Decode(string customProperties)
          {
              string value = Util.PairFindValue(KEY, customProperties);
              if (string.IsNullOrEmpty(value))
              {
                  throw new PASProtocolException(KEY + " not found");
              }
              try
              {
                  interval = Convert.ToUInt16(value);
              }
              catch
              {
                  throw new PASProtocolException(string.Format("Invalid {0}: {1}", KEY, value));
              }
              DisableIfOutOfRange(ref interval);
          }
          public void Decode(byte[] packet, int startIndex)
          {
              interval = BitConverter.ToUInt16(packet, startIndex);
              DisableIfOutOfRange(ref interval);
          }
          public string ToCustomProperties()
          {
              string customProperties;
              string value = (interval == 0) ? Const.keyDisabled : interval.ToString();
              customProperties = Util.MakePair(KEY, value);
              return customProperties;
          }
          public byte[] ToPayloadData()
          {
              byte[] payloadData = BitConverter.GetBytes(interval);
              return payloadData;
          }
          private static void DisableIfOutOfRange(ref ushort interval)
          {
              // Trace Interval should be either 0 (Disabled) or between 15 and 57600(16 hours)
              if ((interval > 0 && interval < 15) || interval > 57600)
              {
                  interval = 0;
              }
          }
      }
      public static byte[] GetTraceInterval(CMFOut cmfOut)
      {
          byte[] payloadData = null;
          TraceInterval traceInterval = new TraceInterval();
          traceInterval.Decode(cmfOut.customProperties);
          payloadData = traceInterval.ToPayloadData();
          return payloadData;
      }
      public class TraceEnabledSensor
      {
          private long mask;
          private const string KEY = Const.setupTraceStates;
          public long Mask
          {
              get { return mask; }
          }
          public void Decode(string customProperties)
          {
              string value = Util.PairFindValue(KEY, customProperties);
              mask = 0;
              int length = (value.Length >= 32) ? 32 : value.Length;
              for (int i = 0; i < length; i++)
              {
                  long traceState = ((byte)value[i] - 0x30) & 0x03;
                  mask |= traceState << (i * 2);
              }
          }
          public void Decode(byte[] packet, int startIndex)
          {
              mask = BitConverter.ToInt64(packet, startIndex);
          }
          public string ToCustomProperties()
          {
              string customProperties;
              StringBuilder sb = new StringBuilder(32);
              for (int i = 0; i < 32; i++)
              {
                  byte traceState = (byte)((mask >> (i * 2)) & 0x03);
                  sb.Append(traceState);
              }               
              customProperties = Util.MakePair(KEY, sb.ToString());
              return customProperties;
          }
          public byte[] ToPayloadData()
          {
              byte[] payloadData = BitConverter.GetBytes(mask);
              return payloadData;
          }
      }
      public static byte[] GetTraceEnabledSensor(CMFOut cmfOut)
      {
          byte[] payloadData = null;
          TraceEnabledSensor traceEnabledSensor = new TraceEnabledSensor();
          traceEnabledSensor.Decode(cmfOut.customProperties);
          payloadData = traceEnabledSensor.ToPayloadData();
          return payloadData;
      }
      public class SpeedThreshold
      {
          private byte threshold;
          private const string KEY = Const.setupSpeedThreshold;
          public byte Threshold
          {
              get { return threshold; }
          }
          public void Decode(string customProperties)
          {
              string value = Util.PairFindValue(KEY, customProperties);
              if (string.IsNullOrEmpty(value))
              {
                  throw new PASProtocolException(KEY + " not found");
              }
              try
              {
                  threshold = Convert.ToByte(value);
              }
              catch
              {
                  throw new PASProtocolException(string.Format("Invalid {0}: {1}", KEY, value));
              }
              DisableIfOutOfRange(ref threshold);
          }
          public void Decode(byte[] packet, int startIndex)
          {
              threshold = packet[startIndex];
              DisableIfOutOfRange(ref threshold);
          }
          public string ToCustomProperties()
          {
              string customProperties;
              string value = (threshold == 0) ? Const.keyDisabled : threshold.ToString();
              customProperties = Util.MakePair(KEY, value);
              return customProperties;
          }
          public byte[] ToPayloadData()
          {
              byte[] payloadData = new byte[1] { threshold };
              return payloadData;
          }
          private static void DisableIfOutOfRange(ref byte threshold)
          {
              if (threshold > 250)
              {
                  threshold = 0;
              }
          }
      }
      public static byte[] GetSpeedThreshold(CMFOut cmfOut)
      {
          byte[] payloadData = null;
          SpeedThreshold speedThreshold = new SpeedThreshold();
          speedThreshold.Decode(cmfOut.customProperties);
          payloadData = speedThreshold.ToPayloadData();
          return payloadData;
      }
      public class GeoFenceRadius
      {
          private ushort radius;
          private const string KEY = Const.setupGeoFenceRadius;
          public ushort Radius
          {
              get { return radius; }
          }
          public void Decode(string customProperties)
          {
              string value = Util.PairFindValue(KEY, customProperties);
              if (string.IsNullOrEmpty(value))
              {
                  throw new PASProtocolException(KEY + " not found");
              }
              try
              {
                  radius = Convert.ToUInt16(value);
              }
              catch
              {
                  throw new PASProtocolException(string.Format("Invalid {0}: {1}", KEY, value));
              }
              DisableIfOutOfRange(ref radius);
          }
          public void Decode(byte[] packet, int startIndex)
          {
              radius = BitConverter.ToUInt16(packet, startIndex);
              DisableIfOutOfRange(ref radius);
          }
          public string ToCustomProperties()
          {
              string customProperties;
              string value = (radius == 0) ? Const.keyDisabled : radius.ToString();
              customProperties = Util.MakePair(KEY, value);
              return customProperties;
          }
          public byte[] ToPayloadData()
          {
              byte[] payloadData = BitConverter.GetBytes(radius);
              return payloadData;
          }
          private static void DisableIfOutOfRange(ref ushort radius)
          {
              if ((radius > 0 && radius < 100) || radius > 65500)
              {
                  radius = 0;
              }
          }
      }
      public static byte[] GetGeoFenceRadius(CMFOut cmfOut)
      {
          byte[] payloadData = null;
          GeoFenceRadius geoFenceRadius = new GeoFenceRadius();
          geoFenceRadius.Decode(cmfOut.customProperties);
          payloadData = geoFenceRadius.ToPayloadData();
          return payloadData;
      }
      public class EnabledSensor
      {
          private uint mask;
          private const string KEY = Const.setupSensorsMask;
          public uint Mask
          {
              get { return mask; }
          }
          public void Decode(string customProperties)
          {
              string value = Util.PairFindValue(KEY, customProperties);
              mask = 0;
              int n = 4;
              if (value.Length != n * 2)
              {
                  throw new PASProtocolException(string.Format("Invalid {0} {1}, must be {2} byte(s)", KEY, value, n));
              }
              for (int i = 0; i < n; i++)
              {
                  byte lo = Convert.ToByte(value[i * 2]);
                  lo -= 0x40;
                  byte hi = Convert.ToByte(value[i * 2 + 1]);
                  hi -= 0x40;
                  uint m = (uint)((byte)(hi << 4) | lo);
                  mask |= (uint)m << (i * 8);
              }
          }
          public void Decode(byte[] packet, int startIndex)
          {
              mask = BitConverter.ToUInt32(packet, startIndex);
          }
          public byte[] ToPayloadData()
          {
              byte[] payloadData = BitConverter.GetBytes(mask);
              return payloadData;
          }
      }
      public static byte[] GetEnabledSensor(CMFOut cmfOut)
      {
          byte[] payloadData = null;
          EnabledSensor enabledSensor = new EnabledSensor();
          enabledSensor.Decode(cmfOut.customProperties);
          payloadData = enabledSensor.ToPayloadData();
          return payloadData;
      }
      public class AccelerationThreshold
      {
          private byte threshold;
          private string key;
          public AccelerationThreshold(string key)
          {
              this.key = key;
          }
          public byte Threshold
          {
              get { return threshold; }
          }
          public void Decode(string customProperties)
          {
              string value = Util.PairFindValue(key, customProperties);
              if (string.IsNullOrEmpty(value))
              {
                  throw new PASProtocolException(key + " not found");
              }
              try
              {
                  threshold = Convert.ToByte(value);
              }
              catch
              {
                  throw new PASProtocolException(string.Format("Invalid {0}: {1}", key, value));
              }
              DisableIfOutOfRange(ref threshold);
          }
          public void Decode(byte[] packet, int startIndex)
          {
              threshold = packet[startIndex];
              DisableIfOutOfRange(ref threshold);
          }
          public string ToCustomProperties()
          {
              string customProperties;
              string value = (threshold == 0) ? Const.keyDisabled : threshold.ToString();
              customProperties = Util.MakePair(key, value);
              return customProperties;
          }
          public byte[] ToPayloadData()
          {
              byte[] payloadData = new byte[1] { threshold };
              return payloadData;
          }
          private static void DisableIfOutOfRange(ref byte threshold)
          {
              if (threshold > 100)
              {
                  threshold = 0;
              }
          }
      }
      public static byte[] GetHarshAcceleration(CMFOut cmfOut)
      {
          byte[] payloadData = null;
          AccelerationThreshold accelerationThreshold = new AccelerationThreshold(Const.setupHarshAcceleration);
          accelerationThreshold.Decode(cmfOut.customProperties);
          payloadData = accelerationThreshold.ToPayloadData();
          return payloadData;
      }
      public static byte[] GetHarshBraking(CMFOut cmfOut)
      {
          byte[] payloadData = null;
          AccelerationThreshold accelerationThreshold = new AccelerationThreshold(Const.setupHarshBraking);
          accelerationThreshold.Decode(cmfOut.customProperties);
          payloadData = accelerationThreshold.ToPayloadData();
          return payloadData;
      }
      public static byte[] GetExtremeAcceleration(CMFOut cmfOut)
      {
          byte[] payloadData = null;
          AccelerationThreshold accelerationThreshold = new AccelerationThreshold(Const.setupExtremeAcceleration);
          accelerationThreshold.Decode(cmfOut.customProperties);
          payloadData = accelerationThreshold.ToPayloadData();
          return payloadData;
      }
      public static byte[] GetExtremeBraking(CMFOut cmfOut)
      {
          byte[] payloadData = null;
          AccelerationThreshold accelerationThreshold = new AccelerationThreshold(Const.setupExtremeBraking);
          accelerationThreshold.Decode(cmfOut.customProperties);
          payloadData = accelerationThreshold.ToPayloadData();
          return payloadData;
      }
       public static byte[] GetNewBoxId(CMFOut cmfOut)
       {
           byte[] payloadData = null;
           string strBoxID = Util.PairFindValue(Const.keyBoxID, cmfOut.customProperties).Trim();
           if (strBoxID == "")
               throw new PASCommandNotSupportedException("BoxID not specified.");           
           int boxId = int.Parse(strBoxID);
           payloadData = new byte[3];
           Util.PackIntToByteArrayLSB((ulong)boxId, payloadData, 3);        
           return payloadData;
       }
       public static byte[] GetSimEsn(CMFOut cmfOut)
       {
           string sim = Util.PairFindValue(Const.keySIM, cmfOut.customProperties).Trim();
           if (sim == "")
               throw new PASCommandNotSupportedException("Sim/Esn not specified.");
           return Encoding.ASCII.GetBytes(sim);
       }
       public static int GetServerOutputID(CMFOut cmfOut)
       {
           return int.Parse(Util.PairFindValue(Const.keySensorNum, cmfOut.customProperties));
       }
       public static byte[] GetOutputFunction(CMFOut cmfOut)
       {
           int outputFunction = Util.PairFindValue(Const.keySensorStatus, cmfOut.customProperties) == Const.valON ? 1 : 0;
           return new byte[1] { (byte)outputFunction };
       }
       public static byte[] GetGeoZoneID(CMFOut cmfOut)
       {
           string strGeoID = Util.PairFindValue(Const.keyGeozoneId, cmfOut.customProperties);
           ushort geoID = 0;
           if (strGeoID != "")
           {
               try { geoID = Convert.ToUInt16(strGeoID); }
               catch { }
           }

           // write GeoZone ID
           byte[] bGeo = new Byte[2];
           Util.PackIntToByteArrayLSB(geoID, bGeo, 2);
           return bGeo;
       }
       public static byte[] GeoZoneType(CMFOut cmfOut)
       {
           byte bType = 2;
           Enums.GeozoneType type = Enums.GeozoneType.Rectangle;
           try
           {
               type = (Enums.GeozoneType)Convert.ToInt32(Util.PairFindValue(Const.keyGeozoneType, cmfOut.customProperties));
           }
           catch { }
           // extract GeoZone Direction
           byte bDir = 0;
           try
           {
               bDir = (byte)Convert.ToChar(Util.PairFindValue(Const.keyGeozoneDir, cmfOut.customProperties));
               bDir -= 0x30;
           }
           catch { }

           bDir |= (byte)(bType << 2);
           return new byte[1] { bDir };
       }
       public class GeoZoneIDList
       {
           private const string KEY = Const.keyGeozoneId;
           private List<ushort> geoZoneIDList = new List<ushort>();
           public void Decode(string customProperties)
           {
               geoZoneIDList = new List<ushort>();
               string value = "";
               int geoZoneIndex = 0;
               ushort geoZoneID = 0;
               while ((value = Util.PairFindValue(KEY + (geoZoneIndex + 1), customProperties)) != "")
               {
                   try
                   {
                       geoZoneID = Convert.ToUInt16(value);
                       geoZoneIDList.Add(geoZoneID);
                   }
                   catch { }
                   geoZoneIndex++;
               }
           }
           public void Decode(byte[] packet, int startIndex)
           {
               int numberOfGeoZones = packet[startIndex];
               startIndex++;
               geoZoneIDList = new List<ushort>(numberOfGeoZones);
               for (int i = 0; i < numberOfGeoZones; i++)
               {
                   ushort geoZoneID = BitConverter.ToUInt16(packet, startIndex);
                   geoZoneIDList.Add(geoZoneID);
                   startIndex += 2;
               }
           }
           public string ToCustomProperties()
           {
               string customProperties = "";
               for (int i = 0; i < geoZoneIDList.Count; i++)
               {
                   string key = KEY + (i + 1);
                   string value = geoZoneIDList[i].ToString();
                   customProperties += Util.MakePair(key, value);
               }
               return customProperties;
           }
           public byte[] ToPayloadData()
           {
               byte[] payloadData = new byte[1 + geoZoneIDList.Count * 2];
               int i = 0;
               payloadData[i] = (byte)geoZoneIDList.Count;
               i++;
               foreach (ushort geoZoneId in geoZoneIDList)
               {
                   BitConverter.GetBytes(geoZoneId).CopyTo(payloadData, i);
                   i += 2;
               }
               return payloadData;
           }
       }
       public static byte[] GetGeoZoneIDList(CMFOut cmfOut)
       {
           byte[] payloadData = null;
           GeoZoneIDList geoZoneIDList = new GeoZoneIDList();
           geoZoneIDList.Decode(cmfOut.customProperties);
           payloadData = geoZoneIDList.ToPayloadData();
           return payloadData;
       }
       public struct GeoPoint
       {      
           public uint Latitude;
           public uint Longitude;
           public GeoPoint(uint latitude, uint longitude)
           {
               Latitude = latitude;
               Longitude = longitude;
           }
       }
       public class GeoZone
       {
           const int MAX_NUMBER_OF_POINTS = 15;
           #region private variable.
           private ushort id;
           private Enums.GeozoneType type;
           private Enums.GeoZoneDirection direction;
           private List<GeoPoint> pointList;
           #endregion 
           #region properties
           public ushort ID
           {
               get { return id; }
               set { id = value; }
           }
           public Enums.GeozoneType Type
           {
               get { return type; }
               set { type = value; }
           }
           public Enums.GeoZoneDirection Direction
           {
               get { return direction; }
               set { direction = value; }
           }
           public List<GeoPoint> PointList
           {
               get { return pointList; }
               set { pointList = value; }
           }
           #endregion
           public GeoZone()
           {           
           }
           public void Decode(string customProperties)
           {
               string key;
               string value;               
               // id
               key = Const.keyGeozoneId;
               value = Util.PairFindValue(key, customProperties);
               if (!ushort.TryParse(value, out id))
               {
                   throw new PASProtocolException(string.Format("Invalid {0}: {1}", key, value));
               }
               // type
               key = Const.keyGeozoneType;
               value = Util.PairFindValue(key, customProperties);
               int i;
               if (int.TryParse(value, out i) && Enum.IsDefined(typeof(Enums.GeozoneType), i))
               {
                   type = (Enums.GeozoneType)i;
               }
               else
               {
                   throw new PASProtocolException(string.Format("Invalid {0}: {1}", key, value));
               }
               // direction
               key = Const.keyGeozoneDir;
               value = Util.PairFindValue(key, customProperties);
               if (int.TryParse(value, out i) && Enum.IsDefined(typeof(Enums.GeoZoneDirection), i))
               {
                   direction = (Enums.GeoZoneDirection)i;
               }
               else
               {
                   throw new PASProtocolException(string.Format("Invalid {0}: {1}", key, value));
               }
               // points
               pointList = new List<GeoPoint>();
               if (type == Enums.GeozoneType.Polygon)
               {
                   for (i = 0; i < MAX_NUMBER_OF_POINTS; i++)
                   {
                       GeoPoint point = new GeoPoint();
                       // latitude
                       key = Const.keyGeozoneLat + (i + 1);
                       value = Util.PairFindValue(key, customProperties);
                       if (value == "")
                           break;

                       double dblLatitude = 0;
                       // convert to double
                       try { dblLatitude = Convert.ToDouble(value); }
                       catch { }

                       // choose north/south side
                       uint ns = 0;
                       if (dblLatitude > 0)
                       {
                           dblLatitude = Math.Abs(dblLatitude);
                           ns = 0x80000000;
                       }

                       // convert to byte-order value
                       point.Latitude = Util.ConvertGPSFromServerToBox(dblLatitude);
                       point.Latitude |= ns;

                       // longitude
                       key = Const.keyGeozoneLon + (i + 1);
                       value = Util.PairFindValue(key, customProperties);
                       if (value == "")
                           break;

                       double dblLongitude = 0;
                       try { dblLongitude = Convert.ToDouble(value); }
                       catch { }

                       // in older byte in older bit put 1 for west
                       uint we = 0;
                       if (dblLongitude < 0)
                       {
                           dblLongitude = Math.Abs(dblLongitude);
                           we = 0x80000000;
                       }
                       point.Longitude = Util.ConvertGPSFromServerToBox(dblLongitude);
                       point.Longitude |= we;
                       
                       pointList.Add(point);
                   }
               }
               else if (type == Enums.GeozoneType.Rectangle)
               {
                   // take GeoZoneLatTop and GeoZoneLatBottom
                   string strLatNorth = Util.PairFindValue(Const.keyGeozoneLatTop, customProperties);
                   string strLatSouth = Util.PairFindValue(Const.keyGeozoneLatBottom, customProperties);
                   double latNorth = 0;
                   double latSouth = 0;
                   try
                   {
                       latNorth = Convert.ToDouble(strLatNorth);
                       latSouth = Convert.ToDouble(strLatSouth);
                   }
                   catch { }
                   // replace if latTop < latBottom.
                   // latTop should be always greater than latBottom
                   if (Math.Abs(latNorth) < Math.Abs(latSouth))
                   {
                       double temp = latNorth;
                       latNorth = latSouth;
                       latSouth = temp;

                   }
                   // in older byte in older bit put 1 for north
                   uint ns = 0;
                   if (latNorth > 0)
                   {
                       latNorth = Math.Abs(latNorth);
                       ns = 0x80000000;
                   }
                   uint boxLatNorth = Util.ConvertGPSFromServerToBox(latNorth);
                   boxLatNorth |= ns;
                   ns = 0;
                   if (latSouth > 0)
                   {
                       latSouth = Math.Abs(latSouth);
                       ns = 0x80000000;
                   }
                   uint boxLatSouth = Util.ConvertGPSFromServerToBox(latSouth);
                   boxLatSouth |= ns;

                   // take GeoZoneLonTop and GeoZoneLonBottom
                   string strLonWest = Util.PairFindValue(Const.keyGeozoneLonTop, customProperties);
                   string strLonEast = Util.PairFindValue(Const.keyGeozoneLonBottom, customProperties);
                   double lonEast = 0;
                   double lonWest = 0;
                   try
                   {
                       lonEast = Convert.ToDouble(strLonEast);
                       lonWest = Convert.ToDouble(strLonWest);
                   }
                   catch { }
                   // replace if lonTop < lonBottom.
                   // lonTop should be always greater than latBottom
                   if (Math.Abs(lonWest) < Math.Abs(lonEast))
                   {
                       double temp = lonEast;
                       lonEast = lonWest;
                       lonWest = temp;

                   }
                   // in older byte in older bit put 1 for west
                   uint we = 0;
                   if (lonEast < 0)
                   {
                       lonEast = Math.Abs(lonEast);
                       we = 0x80000000;
                   }
                   uint boxLonEast = Util.ConvertGPSFromServerToBox(lonEast);
                   boxLonEast |= we;
                   we = 0;
                   if (lonWest < 0)
                   {
                       lonWest = Math.Abs(lonWest);
                       we = 0x80000000;
                   }
                   uint boxLonWest = Util.ConvertGPSFromServerToBox(lonWest);
                   boxLonWest |= we;

                   pointList.Add(new GeoPoint(boxLatSouth, boxLonEast));
                   pointList.Add(new GeoPoint(boxLatNorth, boxLonWest));
               }
               else
               {
                   throw new PASException("Unrecognizable geozone type " + type);
               }
           }
           public void AddToPacket(ref BuildPacket packet)
           {
               packet.AddPayload(Definitions.PayloadID.GeoZoneID, BitConverter.GetBytes(id));
               byte type = (byte)direction;
               packet.AddPayload(Definitions.PayloadID.GeoZoneType, new byte[1] { type });
               byte[] geoZoneCoordinatesBuffer = new byte[1 + pointList.Count * 8];
               int i = 0;
               geoZoneCoordinatesBuffer[i] = (byte)pointList.Count;
               i++;
               byte[] temp;
               foreach (GeoPoint point in pointList)
               {
                   temp = BitConverter.GetBytes(point.Latitude);
                   temp.CopyTo(geoZoneCoordinatesBuffer, i);
                   i += temp.Length;
                   temp = BitConverter.GetBytes(point.Longitude);
                   temp.CopyTo(geoZoneCoordinatesBuffer, i);
                   i += temp.Length;
               }
               packet.AddPayload(Definitions.PayloadID.GeoZoneCoordinates, geoZoneCoordinatesBuffer);
           }
       }
       public static byte[] GetOdoMeter(CMFOut cmfOut)
       {
           string kmOdometer = Util.PairFindValue(Const.keyOdometerValue, cmfOut.customProperties).Trim();
           if (kmOdometer == "")
               throw new PASCommandNotSupportedException(Const.keyOdometerValue + " not specified");
           int kmValue = Int32.Parse(kmOdometer);
           byte[] data = new byte[3];
           Util.PackIntToByteArrayLSB((ulong)kmValue, data, 3);
           return data;
       }
       public static byte[] GetRemoteControlSettings(CMFOut cmfOut)
       {
           string key = Const.keyRemoteControlSettings;
           string value = Util.PairFindValue(key, cmfOut.customProperties).Trim();
           int setting;
           if (int.TryParse(value, out setting))
           {
                if (Enum.IsDefined(typeof(Enums.RemoteControlSettings), setting))
                {
                    return new byte[1] { (byte)setting};
                }
           }
           throw new PASProtocolException(string.Format("Invalid {0}: {1}", key, value));
       }
       /*
                      // extract parameter
        */

       /** \fn     public static byte[] ThirdPartyPacket(CMFOut cmfOut)
        *  \brief  
        */ 
      public static byte[] ThirdPartyPacket(CMFOut cmfOut)
      {
         if (null == cmfOut)
            return null;
         int utilSize = cmfOut.customProperties.Length;
         if (utilSize + 11 > 255 )
         {
            Util.BTrace(Util.ERR0, "ThirdPartyPacket -> size {0} > 255", utilSize );
            return null;
         }

         byte[] tmpBuf = new byte[utilSize + 11] ;
         tmpBuf[utilSize + 10] = (byte)'>'; 
         int idx = 0 ;
         if (null != tmpBuf)
         {
            tmpBuf[idx++] = (byte)'<';
            tmpBuf[idx++] = (byte)utilSize;
            tmpBuf[idx++] = (byte)'W';
            // add boxId to protocol
            string strBoxID = cmfOut.boxID.ToString("d" + 6); 
            for (int i = 0; i < strBoxID.Length; i++)
               tmpBuf[idx++] = (byte)strBoxID[i];
            tmpBuf[idx++] = (byte)utilSize;
            Array.Copy(cmfOut.customProperties.ToCharArray(), 0, tmpBuf, idx, utilSize);
            tmpBuf[utilSize + 9] = Util.CalcChecksum(tmpBuf, 1, utilSize + 8);
            return tmpBuf;
         }

         return null;
      }
/*      
      #region SetOdometer
      static byte[] SetOdometer(CMFOut cmfOut)
      {
      }
      #endregion SetOdometer
*/
       public static byte[] GetEEPROMData(CMFOut cmfOut)
       {
           byte[] payloadData = null;
           EEPROMData eepromData = new EEPROMData();
           eepromData.Decode(cmfOut.customProperties, (Enums.CommandType)cmfOut.commandTypeID);
           payloadData = eepromData.ToPayloadData();
           return payloadData;
       }
       public static string CreateEEPROMDataProperties(byte[] packet, int startIndex)
       {
           string customProperties = null;
           EEPROMData eepromData = new EEPROMData();
           eepromData.Decode(packet, startIndex);
           customProperties = eepromData.ToCustomProperties();
           return customProperties;
       }
       public class StandardMessage
       {
           #region private variables.
           private GeoPoint geoPoint;
           private short heading;
           private byte direction;
           private byte speed;
           private byte gpsStatus;
           private byte armStatus;
           private uint sensorsStatusMask;
           private byte sensorNumber;
           private byte sensorStatus;
           private uint odometer;
           private DateTime dateTime;
           private bool noSensorAlarm = false;
           [Flags]
           private enum DirectionMask
           {
               West = 1,
               North = 2
           }
           #endregion
           #region properties
           public GeoPoint GeoPoint
           {
               get { return geoPoint; }
           }
           public short Heading
           {
               get { return heading; }
           }
           public byte Direction
           {
               get { return direction; }
           }
           public byte Speed
           {
               get { return speed; }
           }
           public byte GpsStatus
           {
               get { return gpsStatus; }
           }
           public byte ArmStatus
           {
               get { return armStatus; }
           }
           public uint SensorsStatusMask
           {
               get { return sensorsStatusMask; }
           }
           public byte SensorNumber
           {
               get { return sensorNumber; }
           }
           public byte SensorStatus
           {
               get { return sensorStatus; }
           }
           public uint Odometer
           {
               get { return odometer; }
           }
           public DateTime DateTime
           {
               get { return dateTime; }
           }
           public bool NoSensorAlarm
           {
               get { return noSensorAlarm; }
               set { noSensorAlarm = value; }
           }
           #endregion
           public void Decode(byte[] packet, int startIndex)
           {
               int i = startIndex;
               geoPoint = new GeoPoint();
               geoPoint.Latitude = BitConverter.ToUInt32(packet, i);
               i += 4;
               geoPoint.Longitude = BitConverter.ToUInt32(packet, i);
               i += 4;
               heading = BitConverter.ToInt16(packet, i); ;
               i += 2;
               direction = packet[i];
               i++;
               speed = packet[i];
               i++;
               gpsStatus = packet[i];
               i++;
               armStatus = packet[i];
               i++;
               sensorsStatusMask = BitConverter.ToUInt32(packet, i);
               i += 4;
               if (!noSensorAlarm)
               {
                   sensorNumber = packet[i];
                   i++;
                   sensorStatus = packet[i];
                   i++;
               }
               odometer = BitConverter.ToUInt32(packet, i);
               i += 4;
               int hour = packet[i++];
               int minute = packet[i++];
               int second = packet[i++];
               int day = packet[i++];
               int month = packet[i++];
               int year = packet[i++] + 2000;
               try
               {
                   dateTime = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc);
               }
               catch
               {
                   dateTime = Const.unassignedDateTime;
               }
           }
           public void FillCmfIn(CMFIn cmfIn)
           {
               cmfIn.latitude = Util.ConvertGPSPosition((int)geoPoint.Latitude);
               // South part of the Earth
               if ((direction & (byte)DirectionMask.North) == 0)
               {
                   cmfIn.latitude = -cmfIn.latitude;
               }
               cmfIn.longitude = Util.ConvertGPSPosition((int)geoPoint.Longitude);
               // West part of the Earth
               if ((direction & (byte)DirectionMask.West) != 0)
               {
                   cmfIn.longitude = -cmfIn.longitude;
               }
               // heading
               cmfIn.heading = heading;
               // speed
               cmfIn.speed = speed;
               // check if GPS is valid
               cmfIn.validGPS = gpsStatus;
               // Arm/Disarm status of the box
               cmfIn.isArmed = armStatus == 0 ? (short)Enums.ArmedStatus.False : (short)Enums.ArmedStatus.True;
               // Sensor mask
               cmfIn.sensorMask = sensorsStatusMask;
               //Note: If Value Sensor NONE. Ignore Byte 17 and 18
               if (!noSensorAlarm)
               {
                   if (sensorNumber != 0)
                   {
                       cmfIn.customProperties += Util.MakePair(Const.keySensorNum, sensorNumber.ToString());
                       cmfIn.customProperties += Util.MakePair(Const.keySensorStatus, sensorStatus == 0 ? Const.valOFF : Const.valON);
                   }
               }
               // Odometer value
               cmfIn.customProperties += Util.MakePair(Const.keyOdometerValue, odometer.ToString());
               // date and time
               cmfIn.originatedDateTime = dateTime;
           }
       }
       public static void SetAckStatus(CMFIn cmfIn, Enums.AckStatus ackStatus)
       {
           cmfIn.blobSize = 1;
           cmfIn.blobData = new byte[cmfIn.blobSize];
           cmfIn.blobData[0] = (byte)ackStatus;
       }
       public static Enums.AckStatus GetAckStatus(CMFIn cmfIn)
       {
           Enums.AckStatus ackStatus = Enums.AckStatus.ReceivedAndExecuted;
           if (cmfIn.blobData != null)
           {
               ackStatus = (Enums.AckStatus)cmfIn.blobData[0];
           }
           return ackStatus;
       }
       public static bool IsAckRequired(Enums.MessageType messageType)
       {
           bool isAckRequired = false;
           if (messageType == Enums.MessageType.Coordinate
               || messageType == Enums.MessageType.Sensor
               || messageType == Enums.MessageType.Speed
               || messageType == Enums.MessageType.Speeding
               || messageType == Enums.MessageType.GeoFence
               || messageType == Enums.MessageType.KeyFobArm
               || messageType == Enums.MessageType.KeyFobDisarm
               || messageType == Enums.MessageType.IPUpdate
               || messageType == Enums.MessageType.KeyFobPanic
               || messageType == Enums.MessageType.Alarm
               || messageType == Enums.MessageType.Idling
               || messageType == Enums.MessageType.ExtendedIdling
               || messageType == Enums.MessageType.GeoZone
               || messageType == Enums.MessageType.GPSAntenna
               || messageType == Enums.MessageType.Speeding
               || messageType == Enums.MessageType.AlivePacket
               || messageType == Enums.MessageType.GPSAntennaShort
               || messageType == Enums.MessageType.GPSAntennaOpen
               || messageType == Enums.MessageType.BreakIn
               || messageType == Enums.MessageType.PanicButton
               || messageType == Enums.MessageType.GPSAntennaOK
               || messageType == Enums.MessageType.BadSensor
               || messageType == Enums.MessageType.HarshBraking
               || messageType == Enums.MessageType.ExtremeBraking
               || messageType == Enums.MessageType.HarshAcceleration
               || messageType == Enums.MessageType.ExtremeAcceleration)
           {
               isAckRequired = true;
           }
           return isAckRequired;
       }

       #region Reefer
       public interface IPayloadDataBuilder
       {
           void Decode(string customProperties);
           byte[] ToPayloadData();
       }
       public interface ICustomPropertiesBuilder
       {
           void Decode(byte[] packet, int startIndex);
           string ToCustomProperties();
       }
       public abstract class ReeferThreshold : IPayloadDataBuilder, ICustomPropertiesBuilder
       {
           #region private variables.
           protected static sbyte minValue;
           protected static sbyte maxValue;
           protected static sbyte disabledValue;
           private string lowerKey;
           private string upperKey;
           private sbyte lowerValue;
           private sbyte upperValue;
           #endregion
           #region properties
           public static sbyte MinValue 
           {
               get {return minValue;}
           }
           public static sbyte MaxValue
           {
               get { return maxValue; }
           }
           public static sbyte DisabledValue
           {
               get { return disabledValue; }
           }
           public string LowerKey
           {
               get { return lowerKey; }
               set { lowerKey = value; }
           }
           public string UpperKey
           {
               get { return upperKey; }
               set { upperKey = value; }
           }
           public sbyte LowerValue
           {
               get { return lowerValue; }
           }
           public sbyte UpperValue
           {
               get { return upperValue; }
           }
           #endregion
           public ReeferThreshold(string lowerKey, string upperKey)
           {
               this.lowerKey = lowerKey;
               this.upperKey = upperKey;
               lowerValue = upperValue = disabledValue;
           }
           public void Decode(string customProperties)
           {
               lowerValue = Decode(customProperties, lowerKey);
               upperValue = Decode(customProperties, upperKey);
           }
           private static sbyte Decode(string customProperties, string key)
           {
               bool decoded = false;
               string value;
               sbyte result;
               value = Util.PairFindValue(key, customProperties);
               if (sbyte.TryParse(value, out result))
               {
                   decoded = (result >= minValue && result <= maxValue);
               }
               if (!decoded)
               {
                   throw new PASProtocolException(string.Format("Invalid {0}: {1}, value must be between {2} to {3}", key, value, minValue, maxValue));
               }
               return result;
           }
           public void Decode(byte[] packet, int startIndex)
           {
               lowerValue = (sbyte)packet[startIndex];
               upperValue = (sbyte)packet[startIndex + 1];
           }
           public string ToCustomProperties()
           {
               StringBuilder sb = new StringBuilder();
               sb.Append(Util.MakePair(lowerKey, lowerValue.ToString()));
               sb.Append(Util.MakePair(upperKey, upperValue.ToString()));
               return sb.ToString();
           }
           public byte[] ToPayloadData()
           {
               return new byte[2] { (byte)lowerValue, (byte)upperValue };
           }
       }
       public class ReeferTemperatureThreshold : ReeferThreshold
       {
           static ReeferTemperatureThreshold()
           {
               minValue = -128;
               maxValue = 127;
               disabledValue = -128;
           }
           public ReeferTemperatureThreshold(string lowerKey, string upperKey)
               : base(lowerKey, upperKey)
           { 
           }
       }
       public class ReeferFuelThreshold : ReeferThreshold
       {
           static ReeferFuelThreshold()
           {
               minValue = 0;
               maxValue = 100;
               disabledValue = 0;
           }
           public ReeferFuelThreshold(string lowerKey, string upperKey)
               : base(lowerKey, upperKey)
           {
           }
       }
       public class TemperatureThresholdSetup : IPayloadDataBuilder, ICustomPropertiesBuilder
       {
           public const int NUMBER_OF_ZONE = 5;
           private byte mask;
           private ReeferTemperatureThreshold[] thresholds;
           public TemperatureThresholdSetup()
           {
               thresholds = new ReeferTemperatureThreshold[NUMBER_OF_ZONE];
           }
           public void Decode(string customProperties)
           {
               string key;
               string value;
               key = Const.keyReeferTempZoneEnableMask;
               value = Util.PairFindValue(key, customProperties);
               try
               {
                   mask = Convert.ToByte(value, 16);
               }
               catch
               {
                   throw new PASProtocolException(string.Format("Invalid {0}: {1}", key, value));
               }               
               for (int i = 0; i < NUMBER_OF_ZONE; i++)
               {
                   string suffix = (i + 1).ToString();
                   string lowerKey = Const.keyReeferLowerThresholdOfTempZone + suffix;
                   string upperKey = Const.keyReeferUpperThresholdOfTempZone + suffix;
                   thresholds[i] = new ReeferTemperatureThreshold(lowerKey, upperKey);
                   if (((mask >> i) & 0x01) == 0x01)
                   {
                       thresholds[i].Decode(customProperties);
                   }
               }
           }
           public byte[] ToPayloadData()
           {
               byte[] payloadData = new byte[1 + NUMBER_OF_ZONE * 2];
               int i = 0;
               payloadData[i] = mask;
               i++;
               foreach (ReeferTemperatureThreshold threshold in thresholds)
               {
                   threshold.ToPayloadData().CopyTo(payloadData, i);
                   i += 2;
               }
               return payloadData;
           }

           #region ICustomPropertiesBuilder Members

           public void Decode(byte[] packet, int startIndex)
           {
               int i = startIndex;
               mask = packet[i];
               i++;
               for (int j = 0; j < NUMBER_OF_ZONE; j++)
               {
                   string suffix = (j + 1).ToString();
                   string lowerKey = Const.keyReeferLowerThresholdOfTempZone + suffix;
                   string upperKey = Const.keyReeferUpperThresholdOfTempZone + suffix;
                   thresholds[j] = new ReeferTemperatureThreshold(lowerKey, upperKey);
                   thresholds[j].Decode(packet, i);
                   i += 2;
               }
           }

           public string ToCustomProperties()
           {
               StringBuilder sb = new StringBuilder();
               sb.Append(Util.MakePair(Const.keyReeferTempZoneEnableMask, string.Format("{0:X2}", mask)));
               foreach (ReeferTemperatureThreshold threshold in thresholds)
               {
                   sb.Append(threshold.ToCustomProperties());
               }
               return sb.ToString();               
           }
           #endregion
       }
       public class FuelThresholdSetup : IPayloadDataBuilder, ICustomPropertiesBuilder
       {
           private byte mask;
           private ReeferFuelThreshold threshold;
           public FuelThresholdSetup()
           {
           }
           public void Decode(string customProperties)
           {
               string key;
               string value;
               key = Const.keyReeferFuelEnableMask;
               value = Util.PairFindValue(key, customProperties);
               try
               {
                   mask = Convert.ToByte(value, 16);
               }
               catch
               {
                   throw new PASProtocolException(string.Format("Invalid {0}: {1}", key, value));
               }
               string lowerKey = Const.keyReeferLowerThresholdOfFuel;
               string upperKey = Const.keyReeferUpperThresholdOfFuel;
               threshold = new ReeferFuelThreshold(lowerKey, upperKey);
               if ((mask & 0x01) != 0)
               {
                   threshold.Decode(customProperties);
               }
           }
           public byte[] ToPayloadData()
           {
               byte[] payloadData = new byte[1 + 1 * 2];
               int i = 0;
               payloadData[i] = mask;
               i++;
               threshold.ToPayloadData().CopyTo(payloadData, i);
               i += 2;
               return payloadData;
           }

           #region ICustomPropertiesBuilder Members

           public void Decode(byte[] packet, int startIndex)
           {
               int i = startIndex;
               mask = packet[i];
               i++;
               threshold = new ReeferFuelThreshold(Const.keyReeferLowerThresholdOfFuel, Const.keyReeferUpperThresholdOfFuel);
               threshold.Decode(packet, i);
           }

           public string ToCustomProperties()
           {
               StringBuilder sb = new StringBuilder();
               sb.Append(Util.MakePair(Const.keyReeferFuelEnableMask, string.Format("{0:X2}", mask)));
               sb.Append(threshold.ToCustomProperties());
               return sb.ToString();                              
           }

           #endregion
       }
       public class ReeferMask : IPayloadDataBuilder, ICustomPropertiesBuilder
       {
           private byte[] mask;
           private string key;
           private int numberOfByte;
           public ReeferMask(string key, int numberOfByte)
           {
               this.key = key;
               this.numberOfByte = numberOfByte;
               mask = new byte[numberOfByte];
           }
           public byte[] Mask
           {
               get { return mask; }
           }
           public void Decode(string customProperties)
           {
               string value = Util.PairFindValue(key, customProperties);
               if (string.IsNullOrEmpty(value))
               {
                   throw new PASProtocolException(key + " not found");
               }
               try
               {
                   string[] items = value.Split(new char[]{' '}, numberOfByte, StringSplitOptions.RemoveEmptyEntries);
                   for (int i = 0; i < items.Length; i++)
                   {
                       mask[i] = Convert.ToByte(items[i], 16);
                   }
               }
               catch
               {
                   throw new PASProtocolException(string.Format("Invalid {0}: {1}", key, value));
               }
           }
           public void Decode(byte[] packet, int startIndex)
           {
               Array.Copy(packet, startIndex, mask, 0, numberOfByte);
           }
           public byte[] ToPayloadData()
           {
               return mask;
           }
           public string ToCustomProperties()
           {
               string customProperties = null;
               StringBuilder sb = new StringBuilder(numberOfByte * 3);
               for (int i = 0; i < numberOfByte; i++)
               {
                   sb.AppendFormat("{0:X2} ", mask[i]);                   
               }
               customProperties += Util.MakePair(key, sb.ToString(0, sb.Length - 1));
               return customProperties;
           }
       }
       public static byte[] BuildReeferPayloadData(Definitions.ReeferPayloadID payloadID, CMFOut cmfOut)
       {
           byte[] payloadData = null;
           IPayloadDataBuilder pdb = null;
           switch (payloadID)
           {
               case Definitions.ReeferPayloadID.TemperatureThresholdSetup:
                   pdb = new TemperatureThresholdSetup();
                   break;
               case Definitions.ReeferPayloadID.FuelThresholdSetup:
                   pdb = new FuelThresholdSetup();
                   break;
               case Definitions.ReeferPayloadID.SensorsEnableMask:
                   pdb = new ReeferMask(Const.keyReeferSensorsEnableMask, 2);
                   break;
               case Definitions.ReeferPayloadID.ReportingInterval:
                   pdb = new ReportingInterval(Const.keyReeferReportingInterval);
                   break;
               case Definitions.ReeferPayloadID.FeatureMask:
                   pdb = new ReeferMask(Const.keyReeferFeatureMask, 2);
                   break;
               case Definitions.ReeferPayloadID.TemperatureZoneCheckingTime:
                   pdb = new TemperatureZoneCheckingTime();
                   break;
               case Definitions.ReeferPayloadID.FuelLevelRiseDropSetup:
                   pdb = new FuelLevelRiseDropSetup();
                   break;
               default:
                   throw new PASProtocolException(string.Format("Payload {0} not supported", payloadID));
           }
           pdb.Decode(cmfOut.customProperties);
           payloadData = pdb.ToPayloadData();
           return payloadData;
       }
       public abstract class ReeferStatus : ICustomPropertiesBuilder
       {
           #region private variables.
           protected static sbyte minValue;
           protected static sbyte maxValue;
           protected static sbyte disabledValue;
           private string key;
           private sbyte currentValue;
           #endregion
           #region properties
           public static sbyte MinValue
           {
               get { return minValue; }
           }
           public static sbyte MaxValue
           {
               get { return MinValue; }
           }
           public static sbyte DisabledValue
           {
               get { return disabledValue; }
           }
           public string Key
           {
               get { return key; }
               set { key = value; }
           }
           public sbyte CurrentValue
           {
               get { return currentValue; }
           }
           #endregion
           public ReeferStatus(string key)
           {
               this.key = key;
               currentValue = disabledValue;
           }
           public void Decode(byte[] packet, int startIndex)
           {
               currentValue = (sbyte)packet[startIndex];
           }
           public string ToCustomProperties()
           {
               return Util.MakePair(key, currentValue.ToString());
           }
       }
       public class ReeferTemperatureStatus : ReeferStatus
       {
           static ReeferTemperatureStatus()
           {
               minValue = -128;
               maxValue = 127;
               disabledValue = -128;
           }
           public ReeferTemperatureStatus(string key)
               : base(key)
           { 
           }
       }
       public class ReeferFuelStatus : ReeferStatus
       {
           static ReeferFuelStatus()
           {
               minValue = 0;
               maxValue = 100;
               disabledValue = 0;
           }
           public ReeferFuelStatus(string key)
               : base(key)
           { 
           }
       }
       public class TemperatureZoneStatus : ICustomPropertiesBuilder
       {
           public const int NUMBER_OF_ZONE = 5;
           private ReeferTemperatureStatus[] status;
           public TemperatureZoneStatus()
           {
               status = new ReeferTemperatureStatus[NUMBER_OF_ZONE];
           }
           public void Decode(byte[] packet, int startIndex)
           {
               int i = startIndex;
               for (int j = 0; j < NUMBER_OF_ZONE; j++)
               {
                   string suffix = (j + 1).ToString();
                   string key = Const.keyReeferTempOfZone + suffix;
                   status[j] = new ReeferTemperatureStatus(key);
                   status[j].Decode(packet, i);
                   i++;
               }
           }

           public string ToCustomProperties()
           {
               StringBuilder sb = new StringBuilder();
               foreach (ReeferTemperatureStatus s in status)
               {
                   sb.Append(s.ToCustomProperties());
               }
               return sb.ToString();
           }
       }
       public enum ReeferAlarmType : byte
       {
           InRange = 0,
           OutOfRange = 1       
       }
       public class TemperatureZoneAlarm : ICustomPropertiesBuilder
       {
           public const short TempZoneSensorIndex = 16;
           private byte zoneNumber;
           private ReeferAlarmType alarmType;
           private ReeferTemperatureStatus status;
           private ReeferTemperatureThreshold threshold;
           public TemperatureZoneAlarm()
           {               
           }
           public void Decode(byte[] packet, int startIndex)
           {
               int i = startIndex;
               zoneNumber = packet[i];
               i++;
               alarmType = (ReeferAlarmType)packet[i];
               i++;
               status = new ReeferTemperatureStatus(Const.keyReeferTempOfZone);
               status.Decode(packet, i);
               i++;
               threshold = new ReeferTemperatureThreshold(Const.keyReeferLowerThresholdOfTempZone, Const.keyReeferUpperThresholdOfTempZone);
               threshold.Decode(packet, i);
           }
           public string ToCustomProperties()
           {
               StringBuilder sb = new StringBuilder();
               // simulating a sensor.
               SensorAlarm sensorAlarm = new SensorAlarm(Const.keyReeferSensorNumber, Const.keyReeferSensorStatus);
               sensorAlarm.SensorNumber = (byte)(TempZoneSensorIndex + zoneNumber);
               sensorAlarm.SensorStatus = (byte)alarmType;
               sb.Append(sensorAlarm.ToCustomProperties());
               sb.Append(Util.MakePair(Const.keyReeferTempZoneNumber, zoneNumber.ToString()));
               sb.Append(Util.MakePair(Const.keyReeferTempZoneAlarmType, alarmType.ToString()));
               sb.Append(status.ToCustomProperties());
               sb.Append(threshold.ToCustomProperties());
               return sb.ToString();
           }
       }
       public class FuelAlarm : ICustomPropertiesBuilder
       {
           public const short FuelSensorIndex = 24;
           private ReeferAlarmType alarmType;
           private ReeferFuelStatus status;
           private ReeferFuelThreshold threshold;
           public FuelAlarm()
           {
           }
           public void Decode(byte[] packet, int startIndex)
           {
               int i = startIndex;
               alarmType = (ReeferAlarmType)packet[i];
               i++;
               status = new ReeferFuelStatus(Const.keyReeferFuelStatus);
               status.Decode(packet, i);
               i++;
               threshold = new ReeferFuelThreshold(Const.keyReeferLowerThresholdOfFuel, Const.keyReeferUpperThresholdOfFuel);
               threshold.Decode(packet, i);               
           }
           public string ToCustomProperties()
           {
               StringBuilder sb = new StringBuilder();
               // simulating a sensor.
               SensorAlarm sensorAlarm = new SensorAlarm(Const.keyReeferSensorNumber, Const.keyReeferSensorStatus);
               sensorAlarm.SensorNumber = FuelSensorIndex + 1;
               sensorAlarm.SensorStatus = (byte)alarmType;
               sb.Append(sensorAlarm.ToCustomProperties());
               sb.Append(Util.MakePair(Const.keyReeferFuelAlarmType, alarmType.ToString()));
               sb.Append(status.ToCustomProperties());
               sb.Append(threshold.ToCustomProperties());
               return sb.ToString();
           }
       }
       public class SensorAlarm : ICustomPropertiesBuilder
       {
           private byte sensorNumber; 
           private byte sensorStatus;
           string sensorNumberKey;
           string sensorStatusKey;
           public byte SensorNumber
           {
               get { return sensorNumber; }
               set { sensorNumber = value; }
           }
           public byte SensorStatus
           {
               get { return sensorStatus; }
               set { sensorStatus = value; }           
           }
           public SensorAlarm(string sensorNumberKey, string sensorStatusKey)
           {
               this.sensorNumberKey = sensorNumberKey;
               this.sensorStatusKey = sensorStatusKey;
           }
           public void Decode(byte[] packet, int startIndex)
           {
               int i = startIndex;
               sensorNumber = packet[i];
               i++;
               sensorStatus = packet[i];
               i++;
           }
           public string ToCustomProperties()
           {
               StringBuilder sb = new StringBuilder();
               sb.Append(Util.MakePair(sensorNumberKey, sensorNumber.ToString()));
               sb.Append(Util.MakePair(sensorStatusKey, sensorStatus == 0 ? Const.valOFF : Const.valON));
               return sb.ToString();
           }
       }
       public class FirmwareVersion : ICustomPropertiesBuilder
       {
           private string key;
           private string versionString;
           public FirmwareVersion(string key)
           {
               this.key = key;
           }
           #region ICustomPropertiesBuilder Members

           public void Decode(byte[] packet, int startIndex)
           {
               versionString = Encoding.ASCII.GetString(packet, startIndex, packet.Length - startIndex);
           }

           public string ToCustomProperties()
           {
               return Util.MakePair(key, versionString);
           }

           #endregion
       }
       public class TemperatureZoneCheckingTime : IPayloadDataBuilder, ICustomPropertiesBuilder
       {
           public const int NUMBER_OF_ZONE = 5;
           private byte mask;
           private ushort[] checkingTimes;
           public TemperatureZoneCheckingTime()
           {
               checkingTimes = new ushort[NUMBER_OF_ZONE];
           }
           public void Decode(string customProperties)
           {
               string key;
               string value;
               key = Const.keyReeferTempZoneTimeMask;
               value = Util.PairFindValue(key, customProperties);
               try
               {
                   mask = Convert.ToByte(value, 16);
               }
               catch
               {
                   throw new PASProtocolException(string.Format("Invalid {0}: {1}", key, value));
               }
               for (int i = 0; i < NUMBER_OF_ZONE; i++)
               {
                   if (((mask >> i) & 0x01) == 0x01)
                   {
                       string suffix = (i + 1).ToString();
                       key = Const.keyReeferTempZoneTime + suffix;
                       value = Util.PairFindValue(key, customProperties);
                       checkingTimes[i] = ushort.Parse(value);
                   }
               }
           }
           public byte[] ToPayloadData()
           {
               byte[] payloadData = new byte[1 + NUMBER_OF_ZONE * 2];
               int i = 0;
               payloadData[i] = mask;
               i++;
               foreach (ushort checkingTime in checkingTimes)
               {
                   BitConverter.GetBytes(checkingTime).CopyTo(payloadData, i);
                   i += 2;
               }
               return payloadData;
           }

           #region ICustomPropertiesBuilder Members

           public void Decode(byte[] packet, int startIndex)
           {
               int i = startIndex;
               mask = packet[i];
               i++;
               for (int j = 0; j < NUMBER_OF_ZONE; j++)
               {
                   checkingTimes[j] = (ushort)(packet[i] + ((ushort)packet[i + 1] << 8));
                   i += 2;
               }
           }

           public string ToCustomProperties()
           {
               StringBuilder sb = new StringBuilder();
               sb.Append(Util.MakePair(Const.keyReeferTempZoneTimeMask, string.Format("{0:X2}", mask)));
               for (int i = 0; i < NUMBER_OF_ZONE; i++)
               {
                   string key = Const.keyReeferTempZoneTime + (i + 1).ToString();
                   string value = checkingTimes[i].ToString();
                   sb.Append(Util.MakePair(key, value));
               }
               return sb.ToString();
           }
           #endregion
       }
       public class BatteryVoltage : ICustomPropertiesBuilder
       {
           float voltage;
           private string key;
           public BatteryVoltage(string key)
           {
               this.key = key;
           }
           #region ICustomPropertiesBuilder Members

           public void Decode(byte[] packet, int startIndex)
           {
               voltage  = (float)(int.Parse(Encoding.ASCII.GetString(packet, startIndex, 4))) / 100;
           }

           public string ToCustomProperties()
           {
               return Util.MakePair(key, voltage.ToString());
           }

           #endregion
       }
       internal class FuelLevelChange : IPayloadDataBuilder, ICustomPropertiesBuilder
       {
           private byte change;
           private string key = Const.keyReeferFuelLevelChange;
           #region IPayloadDataBuilder Members

           public void Decode(string customProperties)
           {
               string value = Util.PairFindValue(key, customProperties);
               if (string.IsNullOrEmpty(value))
               {
                   throw new PASProtocolException(key + " not found");
               }
               try
               {
                   change = byte.Parse(value);
               }
               catch
               {
                   throw new PASProtocolException(string.Format("Invalid {0}: {1}", key, value));
               }               
           }

           public byte[] ToPayloadData()
           {
               return new byte[1] { change };
           }

           #endregion

           #region ICustomPropertiesBuilder Members

           public void Decode(byte[] packet, int startIndex)
           {
               change = packet[startIndex];
           }

           public string ToCustomProperties()
           {
               return Util.MakePair(key, change.ToString());
           }

           #endregion
       }
       internal class FuelLevelDurationOfChange : IPayloadDataBuilder, ICustomPropertiesBuilder
       {
           private ushort duration;
           private string key = Const.keyReeferFuelLevelDurationOfChange;
           #region IPayloadDataBuilder Members

           public void Decode(string customProperties)
           {
               string value = Util.PairFindValue(key, customProperties);
               if (string.IsNullOrEmpty(value))
               {
                   throw new PASProtocolException(key + " not found");
               }
               try
               {
                   duration = ushort.Parse(value);
               }
               catch
               {
                   throw new PASProtocolException(string.Format("Invalid {0}: {1}", key, value));
               }                              
           }

           public byte[] ToPayloadData()
           {
               return BitConverter.GetBytes(duration);
           }

           #endregion

           #region ICustomPropertiesBuilder Members

           public void Decode(byte[] packet, int startIndex)
           {
               duration = (ushort)(packet[startIndex] | ((ushort)packet[startIndex + 1] << 8));
           }

           public string ToCustomProperties()
           {
               return Util.MakePair(key, duration.ToString());
           }

           #endregion
       }
       public class FuelLevelRiseDropSetup : IPayloadDataBuilder, ICustomPropertiesBuilder
       {
           #region private variables
           private ReeferMask mask;
           private FuelLevelChange change;
           private FuelLevelDurationOfChange duration;
           #endregion
           #region IPayloadDataBuilder Members

           public void Decode(string customProperties)
           {
               mask = new ReeferMask(Const.keyReeferFuelLevelRiseDropMask, 1);
               mask.Decode(customProperties);
               if ((mask.Mask[0] & 0x01) == 0x01)
               {
                   change = new FuelLevelChange();
                   change.Decode(customProperties);
                   duration = new FuelLevelDurationOfChange();
                   duration.Decode(customProperties);
               }
           }

           public byte[] ToPayloadData()
           {
               byte[] payloadData = new byte[4];
               int i = 0;
               mask.ToPayloadData().CopyTo(payloadData, i);
               i++;
               change.ToPayloadData().CopyTo(payloadData, i);
               i++;
               duration.ToPayloadData().CopyTo(payloadData, i);
               return payloadData;               
           }

           #endregion

           #region ICustomPropertiesBuilder Members

           public void Decode(byte[] packet, int startIndex)
           {
               mask = new ReeferMask(Const.keyReeferFuelLevelRiseDropMask, 1);
               mask.Decode(packet, startIndex);
               startIndex++;
               change = new FuelLevelChange();
               change.Decode(packet, startIndex);
               startIndex++;
               duration = new FuelLevelDurationOfChange();
               duration.Decode(packet, startIndex);
           }

           public string ToCustomProperties()
           {
               return (mask.ToCustomProperties() + change.ToCustomProperties() + duration.ToCustomProperties());
           }

           #endregion

           
       }
       public enum ReeferRiseDropAlarmType : byte
       {
           Drop = 0,
           Rise = 1
       }
       public class FuelLevelRiseDropAlarm : ICustomPropertiesBuilder
       {
           #region private variables
           private ReeferRiseDropAlarmType type;
           private ReeferFuelStatus current;
           private FuelLevelChange change;
           private FuelLevelDurationOfChange duration;
           #endregion           

           #region ICustomPropertiesBuilder Members

           public void Decode(byte[] packet, int startIndex)
           {
               type = (ReeferRiseDropAlarmType)packet[startIndex];
               startIndex++;
               current = new ReeferFuelStatus(Const.keyReeferFuelStatus);
               current.Decode(packet, startIndex);
               startIndex++;
               change = new FuelLevelChange();
               change.Decode(packet, startIndex);
               startIndex++;
               duration = new FuelLevelDurationOfChange();
               duration.Decode(packet, startIndex);
           }

           public string ToCustomProperties()
           {
               StringBuilder sb = new StringBuilder();
               // simulating a sensor.
               SensorAlarm sensorAlarm = new SensorAlarm(Const.keyReeferSensorNumber, Const.keyReeferSensorStatus);
               sensorAlarm.SensorNumber = (byte)(FuelAlarm.FuelSensorIndex + 2);
               sensorAlarm.SensorStatus = (byte)type;
               sb.Append(sensorAlarm.ToCustomProperties());
               sb.Append(Util.MakePair(Const.keyReeferFuelLevelRiseDropAlarmType, type.ToString()));
               sb.Append(current.ToCustomProperties());
               sb.Append(change.ToCustomProperties());
               sb.Append(duration.ToCustomProperties());
               return (sb.ToString());
           }

           #endregion


       }
       public class UnknownPayload : ICustomPropertiesBuilder
       {
           private string key;
           private byte[] payload;
           public UnknownPayload(string key)
           {
               this.key = key;
           }
           #region ICustomPropertiesBuilder Members

           public void Decode(byte[] packet, int startIndex)
           {
               payload = new byte[packet.Length - startIndex];
               Array.Copy(packet, startIndex, payload, 0, payload.Length);
           }

           public string ToCustomProperties()
           {
               StringBuilder sb = new StringBuilder(payload.Length * 3);
               foreach (byte b in payload)
               {
                   sb.AppendFormat("{0:X2} ", b);
               }
               return Util.MakePair(key, sb.ToString(0, sb.Length - 1));
           }

           #endregion
       }

       public static string BuildReeferCustomProperties(Definitions.ReeferPayloadID payloadID, byte[] payloadData, int startIndex)
       {
           string customProperties = null;
           ICustomPropertiesBuilder builder = null;
           switch (payloadID)
           {
               case Definitions.ReeferPayloadID.None:
                   break;
               case Definitions.ReeferPayloadID.AckStatus:
                   customProperties = Util.MakePair(Const.keyAckStatus, payloadData[0].ToString());
                   break;
               case Definitions.ReeferPayloadID.TemperatureThresholdSetup:
                   builder = new TemperatureThresholdSetup();
                   break;
               case Definitions.ReeferPayloadID.FuelThresholdSetup:
                   builder = new FuelThresholdSetup();
                   break;
               case Definitions.ReeferPayloadID.SensorsEnableMask:
                   builder = new ReeferMask(Const.keyReeferSensorsEnableMask, 2);
                   break;
               case Definitions.ReeferPayloadID.ReportingInterval:
                   builder = new ReportingInterval(Const.keyReeferReportingInterval);
                   break;
               case Definitions.ReeferPayloadID.FeatureMask:
                   builder = new ReeferMask(Const.keyReeferFeatureMask, 2);
                   break;
               case Definitions.ReeferPayloadID.SensorsStatus:
                   builder = new ReeferMask(Const.keyReeferSensorsStatus, 2);
                   break;
               case Definitions.ReeferPayloadID.TemperatureZoneStatus:
                   builder = new TemperatureZoneStatus();
                   break;
               case Definitions.ReeferPayloadID.FuelStatus:
                   builder = new ReeferFuelStatus(Const.keyReeferFuelStatus);
                   break;
               case Definitions.ReeferPayloadID.TemperatureZoneAlarm:
                   builder = new TemperatureZoneAlarm();
                   break;
               case Definitions.ReeferPayloadID.FuelAlarm:
                   builder = new FuelAlarm();
                   break;
               case Definitions.ReeferPayloadID.SensorAlarm:
                   builder = new SensorAlarm(Const.keyReeferSensorNumber, Const.keyReeferSensorStatus);
                   break;
               case Definitions.ReeferPayloadID.FirmwareVersion:
                   builder = new FirmwareVersion(Const.keyReeferFirmwareVersion);
                   break;
               case Definitions.ReeferPayloadID.TemperatureZoneCheckingTime:
                   builder = new TemperatureZoneCheckingTime();
                   break;
               case Definitions.ReeferPayloadID.MainBatteryVoltage:
                   builder = new BatteryVoltage(Const.keyReeferMainBatteryVoltage);
                   break;
               case Definitions.ReeferPayloadID.FuelLevelRiseDropAlarm:
                   builder = new FuelLevelRiseDropAlarm();
                   break;
               case Definitions.ReeferPayloadID.FuelLevelRiseDropSetup:
                   builder = new FuelLevelRiseDropSetup();
                   break;
               //default:
                   //throw new PASProtocolException(string.Format("Payload {0} not supported", payloadID));
               // unknown or unhandled PID
               default:
                   string key = Const.keyPayloadID + payloadID.ToString();
                   builder = new UnknownPayload(key);
                   break;

           }
           if (builder != null)
           {
               builder.Decode(payloadData, startIndex);
               customProperties = builder.ToCustomProperties();
           }
           return customProperties;
       }
       public enum XSWrapperID : byte
       {
           None     = 0,
           Standard = 1
       }
       public static void FillWrapperCmfIn(byte[] payloadData, int startIndex, CMFIn cmfIn)
       {
           XSWrapperID id = (XSWrapperID)payloadData[startIndex];
           if (id == XSWrapperID.Standard)
           {
               StandardMessage standardMessage = new StandardMessage();
               standardMessage.NoSensorAlarm = true;
               standardMessage.Decode(payloadData, startIndex + 1);
               standardMessage.FillCmfIn(cmfIn);
           }
           else if (id != XSWrapperID.None)
           {
               // unknown or unhandled PID
               string key = Const.keyReeferWrapperID + id.ToString();
               UnknownPayload unknownPayload = new UnknownPayload(key);
               unknownPayload.Decode(payloadData, startIndex + 1);
               cmfIn.customProperties += unknownPayload.ToCustomProperties();
               //throw new PASProtocolException(string.Format("Wrapper {0} not supported", id));
           }
       }
       
       #endregion
   }
    public static class CmfMessageId
    { 
        private enum Mask
        { 
            DeviceMessageId = 0x00ff,
            DeviceType      = 0x0f00,
            DeviceId        = 0xf000
        }
        private enum Offset
        {
            DeviceMessageId = 0,            
            DeviceType      = 8,
            DeviceId        = 12
        }
        public static byte GetDeviceId(int cmfMessageId)
        {
            return (byte)((cmfMessageId & (int)Mask.DeviceId) >> (int)Offset.DeviceId);
        }
        public static Enums.DeviceType GetDeviceType(int cmfMessageId)
        {
            return (Enums.DeviceType)((cmfMessageId & (int)Mask.DeviceType) >> (int)Offset.DeviceType);
        }
        public static byte GetDeviceMessageId(int cmfMessageId)
        {
            return (byte)((cmfMessageId & (int)Mask.DeviceMessageId) >> (int)Offset.DeviceMessageId);
        }
        public static int GetFullCmfMessageId(byte deviceMessageId, Enums.DeviceType deviceType, byte deviceId)
        {
            return (int)deviceMessageId | ((int)deviceType << (int)Offset.DeviceType) | ((int)deviceId << (int)Offset.DeviceId);
        }
        public static int GetCmfMessageId(int cmfMessageId)
        {
            return cmfMessageId & ((int)Mask.DeviceMessageId | (int)Mask.DeviceType);
        }        
    }
    public static class CmfSensorNumber
    {
        private enum Mask
        {
            DeviceSensorIndex = 0x00ff,
            DeviceType = 0x0f00,
            DeviceId = 0xf000
        }
        private enum Offset
        {
            DeviceSensorIndex = 0,
            DeviceType = 8,
            DeviceId = 12
        }
        public static int GetFullCmfSensorNumber(byte deviceSensorNumber, Enums.DeviceType deviceType, byte deviceId)
        {
            return ((int)deviceSensorNumber | ((int)deviceType << (int)Offset.DeviceType) | ((int)deviceId << (int)Offset.DeviceId));
        }
    }
}
