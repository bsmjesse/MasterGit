using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Data;
using System.Runtime.InteropServices;  ///< for Marshal class
using System.Text;

namespace VLF.CLS.Def
{
    public interface ICMFOutReceiver
    {
        bool Send(CMFOut obj, int userid);
        bool[] SendM(CMFOut[] obj, int[] userids);
        int BufferedPacketsCounter { get; }
        string GetBufferedPackets();
        void Dump();
    }


    /// <summary>
    /// Summary description for CMF.
    /// </summary>
    [Serializable]
    public class CMFOut
    {
        public int sequenceNum;
        public int protocolTypeID;
        public int boxID;
        public int dclID;
        public int commandTypeID;
        public string commInfo1;              // max 64 bytes
        public string commInfo2;              // max 64 bytes
        public string customProperties;
        public DateTime timeSent;
        public string ack;                     // max 32 bytes
        public bool scheduled;

        public CMFOut()
        {
            customProperties = string.Empty;
        }

        public string ToShortString()
        {
            return string.Format("seqN:{0}, boxId:{1}, protocolTypeId:{2}, cmd:{3}, DT:{4}",
                           sequenceNum,
                           boxID,
                           protocolTypeID,
                           ((Enums.CommandType)commandTypeID).ToString(),
                           timeSent.ToString());
        }

        public override string ToString()
        {
            return string.Format("seqN:{0}, boxId:{1}, protocolTypeId:{2}, cmd:{3}, DT:{4}, comm1:{5}, comm2:{6}, Param:{7}",
                        sequenceNum,
                        boxID,
                        protocolTypeID,
                        ((Enums.CommandType)commandTypeID).ToString(),
                        timeSent.ToString(),
                        (commInfo1 != null ? commInfo1 : string.Empty),
                        (commInfo2 != null ? commInfo2 : string.Empty),
                        (customProperties != null && customProperties != "") ? customProperties.TrimEnd() : "");

        }

        public int StreamSize
        {
            get
            {
                return HEADER_CMFOUT +
                        CONST_SIZE_CMFOUT +
                        (null != commInfo1 ? commInfo1.Length : 0) +
                        (null != commInfo2 ? commInfo2.Length : 0) +
                        (null != customProperties ? customProperties.Length : 0) +
                        (null != ack ? ack.Length : 0);
            }
        }

        static readonly int HEADER_CMFOUT = 24;
        static readonly int CONST_SIZE_CMFOUT = 29;

        public byte[] ToBytes()
        {
            /*  // this is the header of the packet - length 24
               int   allSize ;                  4
               int   pktType ;                  4        // used as a switch for a dispatcher app
               int   sizeCommInfo1 ;            4        // L1
               int   sizeCommInfo2 ;            4        // L2
               int   sizeCustomProps ;          4        // L3
               int   sizeAck ;                  4        // L4 
               *****************************************************          length 29
               public int dclID ;                 4
               public int boxID ;                 4
               public int sequenceNum ;           4
               public int protocolTypeID ;        4
               public int commandTypeID ;         4

               public DateTime timeSent ;         8           
               public bool scheduled;             1
               ************************************************* THIS IS THE VARIABLE PART IN THE PACKET
               public string commInfo1 ;          L1        
               public string commInfo2 ;          L2
               public string customProperties ;   L3
               public string ack ;                L4

                */

            int commInfo1Length = (null != commInfo1 ? commInfo1.Length : 0);
            int commInfo2Length = (null != commInfo2 ? commInfo2.Length : 0);
            int customPropertiesLength = (null != customProperties ? customProperties.Length : 0);
            int ackLength = (null != ack ? ack.Length : 0);
            int size = HEADER_CMFOUT +
                       CONST_SIZE_CMFOUT +
                       commInfo1Length +
                       commInfo2Length +
                       customPropertiesLength +
                       ackLength;

            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                byte[] ret;

                ret = BitConverter.GetBytes(size);     // size of the packet
                ms.Write(ret, 0, ret.Length);

                ret = BitConverter.GetBytes(Const.PKT_CMFOUT);   // type of the packet
                ms.Write(ret, 0, ret.Length);

                ret = BitConverter.GetBytes(commInfo1Length);
                ms.Write(ret, 0, ret.Length);

                ret = BitConverter.GetBytes(commInfo2Length);
                ms.Write(ret, 0, ret.Length);

                ret = BitConverter.GetBytes(customPropertiesLength);
                ms.Write(ret, 0, ret.Length);

                ret = BitConverter.GetBytes(ackLength);
                ms.Write(ret, 0, ret.Length);

                /// end of the header, begin of the real payload
                ret = BitConverter.GetBytes(dclID);
                ms.Write(ret, 0, ret.Length);

                ret = BitConverter.GetBytes(boxID);
                ms.Write(ret, 0, ret.Length);

                ret = BitConverter.GetBytes(sequenceNum);
                ms.Write(ret, 0, ret.Length);

                ret = BitConverter.GetBytes(protocolTypeID);
                ms.Write(ret, 0, ret.Length);

                ret = BitConverter.GetBytes(commandTypeID);
                ms.Write(ret, 0, ret.Length);

                ret = BitConverter.GetBytes(timeSent.ToBinary());
                ms.Write(ret, 0, ret.Length);

                ret = BitConverter.GetBytes(scheduled);
                ms.Write(ret, 0, ret.Length);

                // THIS IS THE VARIABLE PART IN THE PACKET
                if (null != commInfo1)
                {
                    ret = Encoding.ASCII.GetBytes(commInfo1);
                    ms.Write(ret, 0, ret.Length);
                }

                if (null != commInfo2)
                {
                    ret = Encoding.ASCII.GetBytes(commInfo2);
                    ms.Write(ret, 0, ret.Length);
                }

                if (null != customProperties)
                {
                    ret = Encoding.ASCII.GetBytes(customProperties);
                    ms.Write(ret, 0, ret.Length);
                }
                if (null != ack)
                {
                    ret = Encoding.ASCII.GetBytes(ack);
                    ms.Write(ret, 0, ret.Length);
                }

                return ms.ToArray();
            }

        }

        public static CMFOut FromBytes(byte[] stream, int index)
        {
            if (null != stream && index >= 0 && stream.Length > index)
            {
                try
                {
                    int size = BitConverter.ToInt32(stream, index);
                    int idx = index + 8;

                    if (size >= (HEADER_CMFOUT + CONST_SIZE_CMFOUT))
                    {
                        CMFOut cmfOut = new CMFOut();

                        int sizeCommInfo1 = BitConverter.ToInt32(stream, idx); idx += 4;
                        int sizeCommInfo2 = BitConverter.ToInt32(stream, idx); idx += 4;
                        int sizeCustomProps = BitConverter.ToInt32(stream, idx); idx += 4;
                        int sizeAck = BitConverter.ToInt32(stream, idx); idx += 4;

                        // fill the fixed fields
                        cmfOut.dclID = BitConverter.ToInt32(stream, idx); idx += 4;
                        cmfOut.boxID = BitConverter.ToInt32(stream, idx); idx += 4;
                        cmfOut.sequenceNum = BitConverter.ToInt32(stream, idx); idx += 4;
                        cmfOut.protocolTypeID = BitConverter.ToInt32(stream, idx); idx += 4;
                        cmfOut.commandTypeID = BitConverter.ToInt32(stream, idx); idx += 4;
                        cmfOut.timeSent = DateTime.FromBinary(BitConverter.ToInt64(stream, idx)); idx += 8;
                        cmfOut.scheduled = BitConverter.ToBoolean(stream, idx); idx += 1;

                        // fill the variable fields
                        cmfOut.commInfo1 = (0 < sizeCommInfo1) ? Encoding.ASCII.GetString(stream, idx, sizeCommInfo1) : "";
                        idx += sizeCommInfo1;

                        cmfOut.commInfo2 = (0 < sizeCommInfo2) ? Encoding.ASCII.GetString(stream, idx, sizeCommInfo2) : "";
                        idx += sizeCommInfo2;

                        cmfOut.customProperties = (0 < sizeCustomProps) ? Encoding.ASCII.GetString(stream, idx, sizeCustomProps) : "";
                        idx += sizeCustomProps;

                        cmfOut.customProperties = (0 < sizeAck) ? Encoding.ASCII.GetString(stream, idx, sizeAck) : "";
                        idx += sizeAck;

                        return cmfOut;
                    }
                }
                catch (Exception exc)
                {
                    Util.BTrace(Util.ERR1, "-- CMFOut.FromBytes -> failed {0} ", stream);
                }
            }

            return null;
        }


        // received on SLS, reconstruct CMFOut from ToBytes serialization
        public static CMFOut FromBytes(byte[] stream)
        {
            return FromBytes(stream, 0);
        }

    }

    /** \class     LocationInfo
     *  \brief     used to compress multiple standard messages in one
     *  \comment   you have to copy 
     *             - Sensors Mask
     *             - Odometer
     *             - Analog Sensor 1
     *             - Analog Sensor 2
     *             - 
     */
    public class LocationInfo
    {
        public static int LOCATION_SIZE = 10;

        public DateTime originatedDateTime;
        public double latitude;
        public double longitude;
        public short speed;
        public int validGPS;
        public short heading;
        public short isArmed;
        public int messageTypeID;     ///< could be alarm or standard message
        public bool currentGPS;

        public LocationInfo()
        {
            messageTypeID = -1;
            latitude = longitude = .0;
            validGPS = (int)Enums.GPSValid.NotSet;
            isArmed = (short)Enums.ArmedStatus.NotSet;
            currentGPS = false;
        }

        public override string ToString()
        {
            return string.Format("origTime:{0}, msg:{1}, ({2},{3}), okGPS:{4}, speed:{5}, heading:{6}, isArmed:{7}, currentGPS:{8}",
                                        originatedDateTime,
                                        ((Enums.MessageType)messageTypeID).ToString(),
                                        latitude.ToString("##.######"),
                                        longitude.ToString("###.######"),
                                        validGPS,
                                        speed,
                                        heading,
                                        isArmed,
                                        currentGPS);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="from"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        public static void ExtractLatLong(byte[] packet, int from, ref double latitude, ref double longitude)
        {
            latitude = longitude = .0;

            if (packet != null && from >= 0 && packet.Length >= (from + 7))
            {
                ulong lon_long = 0;
                for (int i = 0; i < 7; i++)
                {
                    lon_long <<= 8;
                    lon_long += packet[from + 6 - i];
                }
                uint lat = (uint)lon_long & 0xfffffff;
                lon_long >>= 28;
                uint lon = (uint)lon_long;
                longitude = Util.ConvertGPSPosition((int)lon);

                if (longitude < -180)
                    longitude = -180;
                if (longitude > 180)
                    longitude = 180;


                // West part of the Earth
                if ((packet[from + 9] & 0x2) != 0)
                    longitude = -longitude;
                latitude = Util.ConvertGPSPosition((int)lat);

                // South part of the Earth
                if ((packet[from + 9] & 0x4) == 0)
                    latitude = -latitude;

                if (latitude < -90)
                    latitude = -90;
                if (latitude > 90)
                    latitude = 90;
            }
        }

        /// <summary>
        ///         decoder for 10 bytes of data
        /// </summary>
        /// <param name="packet"> the byte array</param>
        /// <param name="from">start of the packet</param>
        /// <returns></returns>
        /// <comment>
        ///         add the check when the lat/long are not within limits
        /// </comment>
        public static LocationInfo FromBytes(byte[] packet, int from)
        {
            if (packet != null && from >= 0 && packet.Length >= (from + LOCATION_SIZE))
            {
                LocationInfo loc = new LocationInfo();

                ExtractLatLong(packet, from, ref loc.latitude, ref loc.longitude);

                // Arm/Disarm status of the box
                loc.isArmed = (packet[from + 9] & 0x8) == 0 ? (short)Enums.ArmedStatus.False : (short)Enums.ArmedStatus.True;

                // speed
                loc.speed = Convert.ToInt16(CLS.Def.Const.kmPerMiles * packet[from + 7]);

                // heading
                ushort hdg = 0;
                for (int i = 0; i < 2; i++)
                {
                    hdg <<= 8;
                    hdg += packet[from + 9 - i];
                }
                loc.heading = (short)(hdg & 0x1ff);

                // check if GPS is valid packet[18] or packet[23] & 0x10
                loc.validGPS = (packet[from + 9] & 0x10) == 0 ? (int)(Enums.GPSValid.True) : (int)(Enums.GPSValid.False);

                loc.currentGPS = ((packet[from + 9] & 0x20) == 0);

                return loc;
            }
            return null;
        }

        /// <summary>
        ///      the problem is that the order is setting a different originDateTime
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static CMFIn Expand2CMFIn(CMFIn origin, int position)
        {
            if (null != origin && null != origin.trailInfo && position >= 0 && origin.trailInfo.Length > position)
            {
                CMFIn cmfIn = new CMFIn(origin);
                LocationInfo loc = (LocationInfo)(origin.trailInfo[position]);
                if (null != loc)
                {
                    cmfIn.originatedDateTime = loc.originatedDateTime;
                    if (cmfIn.originatedDateTime == VLF.CLS.Def.Const.unassignedDateTime)
                        cmfIn.originatedDateTime = cmfIn.receivedDateTime;
                    cmfIn.latitude = loc.latitude;
                    cmfIn.longitude = loc.longitude;
                    cmfIn.speed = loc.speed;
                    cmfIn.validGPS = loc.validGPS;
                    cmfIn.heading = loc.heading;
                    cmfIn.isArmed = loc.isArmed;
                    cmfIn.priority = origin.priority;
                    cmfIn.messageTypeID = (int)Enums.MessageType.Coordinate;
                    cmfIn.sensorMask |= (loc.currentGPS ? 0 : (1 << 31));
                    return cmfIn;
                }
            }

            return null;
        }
    }

    /** \class    CMFIn
     *  \comment  In fact, you must use that attribute when the field's type is not serializable, 
     *            otherwise you'll get errors when attempting to serialize or deserialize 
     *            the class or structure that contains it. You can even provide custom serialization 
     *            by also implementing the ISerializable interface.
     *
     */
    [Serializable]
    public class CMFIn
    {
        public static byte STD_NOTACKNOWLEDGED_REGULAR = 0;
        public static byte STD_NOTACKNOWLEDGED_BANTEK = 1;
        //      public static byte STD_NOTACKNOWLEDGED_IPCHANGE_REGULAR = 2;
        //      public static byte STD_NOTACKNOWLEDGED_IPCHANGE_BANTEK = 3;
        public static byte STD_NOTACKNOWLEDGED = 2;
        public static byte NON_STD_IPCHANGE_BANTEK = 7;       // ACK with sequence number
        public static byte NON_STD_IPCHANGE = 8;              // ACK wo sequence number
        public static byte NON_STD = 9;
        public static byte DUPLICATE = 100;


        public int sequenceNum;
        public int protocolTypeID;
        public int boxID;

        public DateTime originatedDateTime;
        public DateTime receivedDateTime;

        public int messageTypeID;
        public string commInfo1;
        public string commInfo2;

        public string streetAddress;
        public double latitude;
        public double longitude;
        public int validGPS;
        public short speed;
        public short heading;

        public Int64 sensorMask;
        public int dclID;
        public short commMode;
        public int blobSize;
        public byte[] blobData;
        public string customProperties;
        public bool isDuplicatedMsg;
        public short isArmed;
        public byte priority;                     ///< this can be used in the database and also to signal anomalies
        
        ///< like - duplicates packets

        //      [NonSerialized]
        public object[] trailInfo;   // 2008/02/24 (gb) - for Scheduled UpdateEnhancement

        public CMFIn()
        {
            messageTypeID = -1;
            isDuplicatedMsg = false;
            blobSize = 0;
            blobData = null; // new byte[1024];	  // why on earth you allocate this kind on memory when is not needed  (gb) 
            validGPS = (int)Enums.GPSValid.NotSet;
            isArmed = (short)Enums.ArmedStatus.NotSet;
            priority = NON_STD;
            trailInfo = null;
            latitude = longitude = .0;
        }

        // copy constructor 
        public CMFIn(CMFIn cmfFrom)
        {

            sequenceNum = cmfFrom.sequenceNum;
            protocolTypeID = cmfFrom.protocolTypeID;
            boxID = cmfFrom.boxID;
            originatedDateTime = cmfFrom.originatedDateTime;
            receivedDateTime = cmfFrom.receivedDateTime;
            messageTypeID = cmfFrom.messageTypeID;
            commInfo1 = cmfFrom.commInfo1;
            commInfo2 = cmfFrom.commInfo2;
            latitude = cmfFrom.latitude;
            longitude = cmfFrom.longitude;
            validGPS = cmfFrom.validGPS;
            speed = cmfFrom.speed;
            heading = cmfFrom.heading;
            sensorMask = cmfFrom.sensorMask;
            dclID = cmfFrom.dclID;
            commMode = cmfFrom.commMode;
            blobSize = cmfFrom.blobSize;
            blobData = null;
            customProperties = cmfFrom.customProperties;
            isDuplicatedMsg = cmfFrom.isDuplicatedMsg;
            isArmed = cmfFrom.isArmed;
            priority = cmfFrom.priority;
        }

        // copy constructor 
        public CMFIn(DataRow row)
        {
            if (row == null) return;

            //            BoxId	OriginDateTime	DateTimeReceived	DclId	BoxMsgInTypeId	BoxProtocolTypeId	CommInfo1	CommInfo2	ValidGps	Latitude	Longitude	Speed	Heading	SensorMask	CustomProp	BlobDataSize	StreetAddress	SequenceNum	IsArmed	NearestLandmark	LSD	ProcessedHistory	ProcessedReport	ProcessedApp	Processed	SlsDateTime
            //4663	2011-01-11 16:31:07.000	2011-01-11 16:34:58.000	10	2	16	10.111.237.75	4030	0	35.613165	-77.37412	0	0	4294967316	SENSOR_NUM=24;SENSOR_STATUS=OFF;	0	NULL	164	1	NULL	NULL	NULL	NULL	NULL	1	2011-01-11 17:00:15.020

            string fieldName = "SequenceNum";
            sequenceNum = 0;
            if (row[fieldName] != DBNull.Value)
                sequenceNum = Convert.ToInt32(row[fieldName]);

            fieldName = "BoxProtocolTypeId";
            protocolTypeID = 0;
            if (row[fieldName] != DBNull.Value)
                protocolTypeID = Convert.ToInt32(row[fieldName]);

            fieldName = "BoxId";
            boxID = 0;
            if (row[fieldName] != DBNull.Value)
                boxID = Convert.ToInt32(row[fieldName]);

            fieldName = "OriginDateTime";
            originatedDateTime = new DateTime();
            if (row[fieldName] != DBNull.Value)
                originatedDateTime = Convert.ToDateTime(row[fieldName]);

            fieldName = "DateTimeReceived";
            receivedDateTime = new DateTime();
            if (row[fieldName] != DBNull.Value)
            {
                if (row[fieldName].GetType().Name != "DateTime")
                {
                    long ticks = Convert.ToInt64(row[fieldName]);
                    receivedDateTime = new DateTime(ticks);
                }
                else
                {
                    receivedDateTime = Convert.ToDateTime(row[fieldName]);
                }

            }

            fieldName = "BoxMsgInTypeId";
            messageTypeID = 0;
            if (row[fieldName] != DBNull.Value)
                messageTypeID = Convert.ToInt32(row[fieldName]);

            fieldName = "CommInfo1";
            commInfo1 = string.Empty;
            if (row[fieldName] != DBNull.Value)
                commInfo1 = Convert.ToString(row[fieldName]);

            fieldName = "CommInfo2";
            commInfo2 = string.Empty;
            if (row[fieldName] != DBNull.Value)
                commInfo2 = Convert.ToString(row[fieldName]);

            fieldName = "Latitude";
            latitude = 0;
            if (row[fieldName] != DBNull.Value)
                latitude = Convert.ToDouble(row[fieldName]);

            fieldName = "Longitude";
            longitude = 0;
            if (row[fieldName] != DBNull.Value)
                longitude = Convert.ToDouble(row[fieldName]);

            fieldName = "ValidGps";
            validGPS = 0;
            if (row[fieldName] != DBNull.Value)
                validGPS = Convert.ToInt32(row[fieldName]);

            fieldName = "Speed";
            speed = 0;
            if (row[fieldName] != DBNull.Value)
                speed = Convert.ToInt16(row[fieldName]);

            fieldName = "Heading";
            heading = 0;
            if (row[fieldName] != DBNull.Value)
                heading = Convert.ToInt16(row[fieldName]);

            fieldName = "SensorMask";
            sensorMask = 0;
            if (row[fieldName] != DBNull.Value)
                sensorMask = Convert.ToInt64(row[fieldName]);

            fieldName = "DclId";
            dclID = 0;
            if (row[fieldName] != DBNull.Value)
                dclID = Convert.ToInt32(row[fieldName]);


            commMode = 0;


            fieldName = "BlobDataSize";
            blobSize = 0;
            if (row[fieldName] != DBNull.Value)
                blobSize = Convert.ToInt32(row[fieldName]);


            //fieldName = "BlobData";
            blobData = null;
            //if (row[fieldName] != DBNull.Value)
            //    blobData = (byte[])row[fieldName];

            fieldName = "CustomProp";
            customProperties = string.Empty;
            if (row[fieldName] != DBNull.Value)
                customProperties = Convert.ToString(row[fieldName]);

            //fieldName = "CustomProp";
            isDuplicatedMsg = false;
            //if (row[fieldName] != DBNull.Value)
            //    isDuplicatedMsg = Convert.ToBoolean(row[fieldName]);

            fieldName = "IsArmed";
            isArmed = 0;
            if (row[fieldName] != DBNull.Value)
                isArmed = Convert.ToInt16(row[fieldName]);

            //fieldName = "CustomProp";
            priority = 0;
            //if (row[fieldName] != DBNull.Value)
            //    priority = Convert.ToByte(row[fieldName]);


        }

        public static byte CalculateChecksum(CMFIn cmfIn)
        {
            byte cs = 0x00;
            try
            {
                cmfIn.receivedDateTime = cmfIn.originatedDateTime;
                cmfIn.sequenceNum = 0;
                byte[] buffer = null;
                using (MemoryStream ms = new MemoryStream())
                {
                    new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter().Serialize(ms, cmfIn);
                    buffer = ms.GetBuffer();
                }
                for (int a = 0; a < buffer.Length; cs ^= buffer[a++]) ;
            }
            catch { }
            return cs;
        }


        public static ushort CalculateHash(CMFIn cmfIn)
        {
            try
            {
                if (cmfIn.messageTypeID == (int)Enums.MessageType.SendDTC)
                    cmfIn.originatedDateTime = new DateTime();
                cmfIn.receivedDateTime = cmfIn.originatedDateTime;
                cmfIn.sequenceNum = 0;

                byte[] buffer;

                using (MemoryStream ms = new MemoryStream())
                {
                    new BinaryFormatter().Serialize(ms, cmfIn);
                    buffer = ms.GetBuffer();
                }
                CRC.StartCRCCalc();
                ushort cs = CRC.CRCCalc(buffer, buffer.Length);
                return cs;
            }
            catch { }
            return 0;
        }


        public void Clear()
        {
            sequenceNum = 0;
            protocolTypeID = 0;
            boxID = 0;

            originatedDateTime = CLS.Def.Const.unassignedDateTime;
            receivedDateTime = CLS.Def.Const.unassignedDateTime; ;

            messageTypeID = 0;
            commInfo1 = "";
            commInfo2 = "";

            latitude = 0;
            longitude = 0;
            validGPS = 2;
            speed = 0;
            heading = 0;
            sensorMask = 0;
            dclID = 0;
            commMode = 0;
            blobSize = 0;
            customProperties = "";
            isDuplicatedMsg = false;
            isArmed = 0;
            blobData = null;
            priority = 10;
            trailInfo = null;
        }

        public override string ToString()
        {
            //         return string.Format("seqN:{0}, boxID:{1}, orgT:{2:MM/dd/yyy hh:mm:ss.fff}, rcvT:{3:MM/dd/yyy hh:mm:ss.fff}, msg:{4}, comm1:{5}, comm2:{6}, protoType:{7}, commMode:{8}, ({9}, {10}), okGPS:{11}, v:{12}, dir:{13}, snsMask:{14}, isArmed:{15}, priority:{16}, Param:{17}",
            return string.Format("seqN:{0}, boxID:{1}, orgT:{2:MM/dd/yyy hh:mm:ss.fff}, rcvT:{3}, msg:{4}, comm1:{5}, comm2:{6}, protoType:{7}, commMode:{8}, ({9}, {10}), okGPS:{11}, v:{12}, dir:{13}, snsMask:{14}, isArmed:{15}, priority:{16}, Param:{17}",
                                          sequenceNum,
                                           boxID,
                                           originatedDateTime,
                                           receivedDateTime,
                                           ((Enums.MessageType)messageTypeID).ToString(),
                                           (commInfo1 != null ? commInfo1.TrimEnd() : ""),
                                           (commInfo2 != null ? commInfo2.TrimEnd() : ""),
                                       ((Enums.ProtocolTypes)protocolTypeID).ToString(),
                                           ((Enums.CommMode)commMode).ToString(),
                                       latitude.ToString("##.######"),
                                       longitude.ToString("###.######"),
                                           validGPS,
                                           speed,
                                           heading,
                                           sensorMask,
                                           isArmed,
                                       priority,
                                           (customProperties != null ? customProperties.TrimEnd() : ""));
        }


        public int StreamSize
        {
            get
            {
                return HEADER_CMFIN +
                       CONST_SIZE_CMFIN +
                     (null != commInfo1 ? commInfo1.Length : 0) +
                     (null != commInfo2 ? commInfo2.Length : 0) +
                     (null != customProperties ? customProperties.Length : 0) +
                     (null != blobData ? blobData.Length : 0);
            }
        }
        static readonly int HEADER_CMFIN = 20;
        static readonly int CONST_SIZE_CMFIN = 73;
        /// <summary>
        ///      return an array of binary values used to pack the fields in the class
        /// </summary>
        /// <returns></returns>
        public byte[] ToBytes()
        {
            /*  // this is the header of the packet - length 20
               int   allSize ;                           4
               int   pktType ;                           4        // used as a switch for a dispatcher app
               int   sizeCommInfo1 ;                     4
               int   sizeCommInfo2 ;                     4
               int   sizeCustomProps ;                   4
               *****************************************************          
                     
               public int blobSize ;                     4
               public int sequenceNum ;                  4
               public int protocolTypeID ;               4
               public int boxID ;                        4

               public DateTime originatedDateTime ;      8
               public DateTime receivedDateTime ;        8

               public int messageTypeID ;                4

               public double latitude ;                  8
               public double longitude ;                 8
               public int validGPS ;                     4
               public short speed ;                      2
               public short heading ;                    2
               public Int64 sensorMask ;                 8
               public int dclID ;                        4
               public short commMode ;                   2
               public bool isDuplicatedMsg;              1
               public short isArmed;                     2
               ************************************************** THIS IS THE VARIABLE PART IN THE PACKET
               public string commInfo1 ;                 L1        
               public string commInfo2 ;                 L2
               public string customProperties ;          L3
               public byte[] blobData ;                  L4

                */

            int commInfo1Length = (null != commInfo1 ? commInfo1.Length : 0);
            int commInfo2Length = (null != commInfo2 ? commInfo2.Length : 0);
            int customPropertiesLength = (null != customProperties ? customProperties.Length : 0);

            int size = HEADER_CMFIN +
                       CONST_SIZE_CMFIN +
                       commInfo1Length +
                       commInfo2Length +
                       customPropertiesLength +
                       (null != blobData ? blobData.Length : 0);

            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                byte[] ret;

                ret = BitConverter.GetBytes(size);              // size of the packet
                ms.Write(ret, 0, ret.Length);

                ret = BitConverter.GetBytes(Const.PKT_CMFIN);   // type of the packet
                ms.Write(ret, 0, ret.Length);

                ret = BitConverter.GetBytes(commInfo1Length);
                ms.Write(ret, 0, ret.Length);

                ret = BitConverter.GetBytes(commInfo2Length);
                ms.Write(ret, 0, ret.Length);

                ret = BitConverter.GetBytes(customPropertiesLength);
                ms.Write(ret, 0, ret.Length);

                ret = BitConverter.GetBytes(blobSize);
                ms.Write(ret, 0, ret.Length);

                /// end of the header, begin of the real payload
                ret = BitConverter.GetBytes(originatedDateTime.ToBinary());
                ms.Write(ret, 0, ret.Length);

                ret = BitConverter.GetBytes(receivedDateTime.ToBinary());
                ms.Write(ret, 0, ret.Length);

                ret = BitConverter.GetBytes(messageTypeID);
                ms.Write(ret, 0, ret.Length);

                ret = BitConverter.GetBytes(latitude);
                ms.Write(ret, 0, ret.Length);

                ret = BitConverter.GetBytes(longitude);
                ms.Write(ret, 0, ret.Length);

                ret = BitConverter.GetBytes(validGPS);
                ms.Write(ret, 0, ret.Length);

                ret = BitConverter.GetBytes(speed);
                ms.Write(ret, 0, ret.Length);

                ret = BitConverter.GetBytes(heading);
                ms.Write(ret, 0, ret.Length);

                ret = BitConverter.GetBytes(sensorMask);
                ms.Write(ret, 0, ret.Length);

                ret = BitConverter.GetBytes(dclID);
                ms.Write(ret, 0, ret.Length);

                ret = BitConverter.GetBytes(commMode);
                ms.Write(ret, 0, ret.Length);

                ret = BitConverter.GetBytes(isDuplicatedMsg);
                ms.Write(ret, 0, ret.Length);

                ret = BitConverter.GetBytes(isArmed);
                ms.Write(ret, 0, ret.Length);

                // THIS IS THE VARIABLE PART IN THE PACKET
                if (null != commInfo1)
                {
                    ret = Encoding.ASCII.GetBytes(commInfo1);
                    ms.Write(ret, 0, ret.Length);
                }

                if (null != commInfo2)
                {
                    ret = Encoding.ASCII.GetBytes(commInfo2);
                    ms.Write(ret, 0, ret.Length);
                }

                if (null != customProperties)
                {
                    ret = Encoding.ASCII.GetBytes(customProperties);
                    ms.Write(ret, 0, ret.Length);
                }

                if (0 != blobSize && null != blobData)
                {
                    ms.Write(blobData, 0, blobSize);
                }

                return ms.ToArray();
            }

        }

        public static CMFIn FromBytes(byte[] stream, int index)
        {
            if (null != stream && index >= 0 && stream.Length > index)
            {
                try
                {
                    int size = BitConverter.ToInt32(stream, index);
                    int idx = 8 + index;

                    if (size >= (HEADER_CMFIN + CONST_SIZE_CMFIN))
                    {
                        CMFIn cmfIn = new CMFIn();

                        int sizeCommInfo1 = BitConverter.ToInt32(stream, idx); idx += 4;
                        int sizeCommInfo2 = BitConverter.ToInt32(stream, idx); idx += 4;
                        int sizeCustomProps = BitConverter.ToInt32(stream, idx); idx += 4;
                        cmfIn.blobSize = BitConverter.ToInt32(stream, idx); idx += 4;

                        // 
                        cmfIn.originatedDateTime = DateTime.FromBinary(BitConverter.ToInt64(stream, idx)); idx += 8;
                        cmfIn.receivedDateTime = DateTime.FromBinary(BitConverter.ToInt64(stream, idx)); idx += 8;
                        cmfIn.messageTypeID = BitConverter.ToInt32(stream, idx); idx += 4;
                        cmfIn.latitude = BitConverter.ToDouble(stream, idx); idx += 8;
                        cmfIn.longitude = BitConverter.ToDouble(stream, idx); idx += 8;
                        cmfIn.validGPS = BitConverter.ToInt32(stream, idx); idx += 4;
                        cmfIn.speed = BitConverter.ToInt16(stream, idx); idx += 2;
                        cmfIn.heading = BitConverter.ToInt16(stream, idx); idx += 2;
                        cmfIn.sensorMask = BitConverter.ToInt64(stream, idx); idx += 8;
                        cmfIn.dclID = BitConverter.ToInt32(stream, idx); idx += 4;
                        cmfIn.commMode = BitConverter.ToInt16(stream, idx); idx += 2;
                        cmfIn.isDuplicatedMsg = BitConverter.ToBoolean(stream, idx); idx += 1;
                        cmfIn.isArmed = BitConverter.ToInt16(stream, idx); idx += 2;

                        // 
                        cmfIn.commInfo1 = (0 < sizeCommInfo1) ? Encoding.ASCII.GetString(stream, idx, sizeCommInfo1) : "";
                        idx += sizeCommInfo1;

                        cmfIn.commInfo2 = (0 < sizeCommInfo2) ? Encoding.ASCII.GetString(stream, idx, sizeCommInfo2) : "";
                        idx += sizeCommInfo2;

                        cmfIn.customProperties = (0 < sizeCustomProps) ? Encoding.ASCII.GetString(stream, idx, sizeCustomProps) : "";
                        idx += sizeCustomProps;

                        if (0 != cmfIn.blobSize)
                        {
                            cmfIn.blobData = new byte[cmfIn.blobSize];
                            Array.Copy(stream, cmfIn.blobData, cmfIn.blobSize);
                        }
                        else
                            cmfIn.blobData = null;

                        return cmfIn;
                    }
                }
                catch (Exception exc)
                {
                    Util.BTrace(Util.ERR1, "-- CMFIn.FromBytes -> failed {0} ", stream);
                }
            }

            return null;

        }
        // received on SLS, reconstruct CMFIn from ToBytes serialization
        public static CMFIn FromBytes(byte[] stream)
        {
            return FromBytes(stream, 0);
        }


        /// <summary>
        ///      it doesn't count the unique bytes
        /// </summary>
        /// <returns></returns>
        public byte[] ToUniqueBytes()
        {
            /*  // this is the header of the packet - length 20
               int   allSize ;                           4
               int   pktType ;                           4        // used as a switch for a dispatcher app
               int   sizeCommInfo1 ;                     4
               int   sizeCommInfo2 ;                     4
               int   sizeCustomProps ;                   4
               *****************************************************          
                     
               public int blobSize ;                     4
               public int sequenceNum ;                  4
               public int protocolTypeID ;               4
               public int boxID ;                        4

               public DateTime originatedDateTime ;      8
   ///         public DateTime receivedDateTime ;        8        ///< eliminated

               public int messageTypeID ;                4

               public double latitude ;                  8
               public double longitude ;                 8
               public int validGPS ;                     4
               public short speed ;                      2
               public short heading ;                    2
               public Int64 sensorMask ;                 8
               public int dclID ;                        4
               public short commMode ;                   2
               public bool isDuplicatedMsg;              1
               public short isArmed;                     2
               ************************************************** THIS IS THE VARIABLE PART IN THE PACKET
               public string commInfo1 ;                 L1        
               public string commInfo2 ;                 L2
               public string customProperties ;          L3
               public byte[] blobData ;                  L4

                */

            int commInfo1Length = (null != commInfo1 ? commInfo1.Length : 0);
            int commInfo2Length = (null != commInfo2 ? commInfo2.Length : 0);
            int customPropertiesLength = (null != customProperties ? customProperties.Length : 0);

            int size = HEADER_CMFIN +
                       (CONST_SIZE_CMFIN - 8) +
                       commInfo1Length +
                       commInfo2Length +
                       customPropertiesLength +
                       (null != blobData ? blobData.Length : 0);

            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                byte[] ret;

                ret = BitConverter.GetBytes(size);              // size of the packet
                ms.Write(ret, 0, ret.Length);

                ret = BitConverter.GetBytes(Const.PKT_CMFIN);   // type of the packet
                ms.Write(ret, 0, ret.Length);

                ret = BitConverter.GetBytes(commInfo1Length);
                ms.Write(ret, 0, ret.Length);

                ret = BitConverter.GetBytes(commInfo2Length);
                ms.Write(ret, 0, ret.Length);

                ret = BitConverter.GetBytes(customPropertiesLength);
                ms.Write(ret, 0, ret.Length);

                ret = BitConverter.GetBytes(blobSize);
                ms.Write(ret, 0, ret.Length);

                /// end of the header, begin of the real payload
                ret = BitConverter.GetBytes(originatedDateTime.ToBinary());
                ms.Write(ret, 0, ret.Length);
                /*
                            ret = BitConverter.GetBytes(receivedDateTime.ToBinary());
                            ms.Write(ret, 0, ret.Length);
                */
                ret = BitConverter.GetBytes(messageTypeID);
                ms.Write(ret, 0, ret.Length);

                ret = BitConverter.GetBytes(latitude);
                ms.Write(ret, 0, ret.Length);

                ret = BitConverter.GetBytes(longitude);
                ms.Write(ret, 0, ret.Length);

                ret = BitConverter.GetBytes(validGPS);
                ms.Write(ret, 0, ret.Length);

                ret = BitConverter.GetBytes(speed);
                ms.Write(ret, 0, ret.Length);

                ret = BitConverter.GetBytes(heading);
                ms.Write(ret, 0, ret.Length);

                ret = BitConverter.GetBytes(sensorMask);
                ms.Write(ret, 0, ret.Length);

                ret = BitConverter.GetBytes(dclID);
                ms.Write(ret, 0, ret.Length);

                ret = BitConverter.GetBytes(commMode);
                ms.Write(ret, 0, ret.Length);

                ret = BitConverter.GetBytes(isDuplicatedMsg);
                ms.Write(ret, 0, ret.Length);

                ret = BitConverter.GetBytes(isArmed);
                ms.Write(ret, 0, ret.Length);

                // THIS IS THE VARIABLE PART IN THE PACKET
                if (null != commInfo1)
                {
                    ret = Encoding.ASCII.GetBytes(commInfo1);
                    ms.Write(ret, 0, ret.Length);
                }

                if (null != commInfo2)
                {
                    ret = Encoding.ASCII.GetBytes(commInfo2);
                    ms.Write(ret, 0, ret.Length);
                }

                if (null != customProperties)
                {
                    ret = Encoding.ASCII.GetBytes(customProperties);
                    ms.Write(ret, 0, ret.Length);
                }

                if (0 != blobSize && null != blobData)
                {
                    ms.Write(blobData, 0, blobSize);
                }

                return ms.ToArray();
            }

        }
    }

    /// <summary>
    ///      this came along because the origindatetime is changed in the code running in the server and 
    ///      the original message in vlfMsgIn cannot be referenced anymore
    /// </summary>
    [Serializable]
    public class CMFInEx : CMFIn
    {
        public long ID;
        public DateTime realOriginDateTime;

        public CMFInEx(CMFIn cmf) :
            base(cmf)
        {
            ID = 0;
            realOriginDateTime = base.originatedDateTime;
        }

        public CMFInEx(DataRow row) :
            base(row)
        {

            realOriginDateTime = base.originatedDateTime;

            if (row == null) return;


            string fieldName = "ID";
            ID = 0;
            if (row[fieldName] != DBNull.Value)
                ID = Convert.ToInt64(row[fieldName]);
        }


        public CMFInEx()
            : base()
        {
            ID = 0;
            realOriginDateTime = base.originatedDateTime;
        }

        public CMFIn Base
        {
            get { return new CMFIn(this); }
        }
    }
}
