using System;
using System.Diagnostics;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;
using System.Collections ;
using System.Reflection ;
using System.Data.SqlClient ;	///< for SqlException
using System.Net;             ///< for IPAddress
using System.Runtime.InteropServices;  ///< for Marshal class                              
using System.Configuration;                                       
using VLF.CLS.Def;
using VLF.ERR ;
using System.Xml;

namespace VLF.CLS
{
	/// <summary>
	/// Common utilities methods
	/// </summary>
	public class Util
	{
		private Util()
		{
			// can't create an instance
            Trace.AutoFlush = true;
		}

#if OLD_LOGS
      public static string TraceFormatLong(Def.Enums.TraceSeverity severity, string msg)
      {
         foreach (TraceListener tl in Trace.Listeners)
         {
            if (tl.Name != "Default" && (tl is TextWriterTraceListener))
            {
               // get short current date
               string newData = DateTime.Now.ToShortDateString().Replace(@"/", @"-").Replace(@"\", @"-");

               // find date section in full log file name
               int before = tl.Name.IndexOf('_');
               int after = tl.Name.LastIndexOf('.');
               string oldData = tl.Name.Substring(before + 1, after - before - 1);

               // haven't changed - use current log file
               if (newData == oldData)
                  break;

               // close old log file and open new one
               tl.Flush();
               Trace.Listeners.Remove(tl.Name);
               tl.Close();

               // open new Trace Listener
               string newFileName = tl.Name.Substring(0, before + 1) + newData + tl.Name.Substring(after);
               TextWriterTraceListener logFile = new TextWriterTraceListener(newFileName, newFileName);
               Trace.Listeners.Add(logFile);
               // one for a time beeing
               break;
            }
         }
         string strSeverity = "";
         switch (severity)
         {
            case Def.Enums.TraceSeverity.Error:
               strSeverity = "ERRO";
               break;
            case Def.Enums.TraceSeverity.Warning:
               strSeverity = "WARN";
               break;
            case Def.Enums.TraceSeverity.Info:
               strSeverity = "INFO";
               break;
            case Def.Enums.TraceSeverity.Verbose:
               strSeverity = "VERB";
               break;
            case Def.Enums.TraceSeverity.Ping:
               strSeverity = "PING";
               break;
            case Def.Enums.TraceSeverity.Statistics:
               strSeverity = "STAT";
               break;
            case Def.Enums.TraceSeverity.WebInterfaces:
               strSeverity = "INTF";
               break;
            case Def.Enums.TraceSeverity.Map:
               strSeverity = "MAPS";
               break;
            case Def.Enums.TraceSeverity.Das:
               strSeverity = "DAS_";
               break;
            case Def.Enums.TraceSeverity.Ota:
               strSeverity = "OTA_";
               break;
         }

         return DateTime.Now.ToString("M/d/yyyy HH:mm:ss.fff") + "> " + strSeverity + ":: " + msg; 
      }

		public static string TraceFormat(string msg)
		{
            foreach( TraceListener tl in Trace.Listeners)
            {
               if (tl.Name != "Default" && (tl is TextWriterTraceListener))
                {
                    // get short current date
                    string newData = DateTime.Now.ToShortDateString().Replace(@"/",@"-").Replace(@"\",@"-");

                    // find date section in full log file name
                    int before = tl.Name.IndexOf('_');
                    int after = tl.Name.LastIndexOf('.');
                    string oldData = tl.Name.Substring(before+1,after-before-1);

                    // haven't changed - use current log file
                    if( newData == oldData )
                        break;

                    // close old log file and open new one
                    tl.Flush();
                    Trace.Listeners.Remove(tl.Name);
                    tl.Close();

                    // open new Trace Listener
                    string newFileName = tl.Name.Substring(0,before+1) + newData +  tl.Name.Substring(after);
                    TextWriterTraceListener logFile = new TextWriterTraceListener(newFileName,newFileName) ;
                    Trace.Listeners.Add(logFile) ;
                    // one for a time beeing
                    break;
                }
            }

            return DateTime.Now.ToString() + "> " + msg; 
        }

        public static string TraceFormat(Def.Enums.TraceSeverity severity, string msg)
        {
            foreach( TraceListener tl in Trace.Listeners)
            {              
               if (tl.Name != "Default" && (tl is TextWriterTraceListener))
                {
                    // get short current date
                    string newData = DateTime.Now.ToShortDateString().Replace(@"/",@"-").Replace(@"\",@"-");

                    // find date section in full log file name
                    int before = tl.Name.IndexOf('_');
                    int after = tl.Name.LastIndexOf('.');
                    string oldData = tl.Name.Substring(before+1,after-before-1);

                    // haven't changed - use current log file
                    if( newData == oldData )
                        break;

                    // close old log file and open new one
                    tl.Flush();
                    Trace.Listeners.Remove(tl.Name);
                    tl.Close();

                    // open new Trace Listener
                    string newFileName = tl.Name.Substring(0,before+1) + newData +  tl.Name.Substring(after);
                    TextWriterTraceListener logFile = new TextWriterTraceListener(newFileName,newFileName) ;
                    Trace.Listeners.Add(logFile) ;
                    // one for a time beeing
                    break;
                }
            }
			string strSeverity = "";
			switch(severity)
			{
				case Def.Enums.TraceSeverity.Error:
					strSeverity = "ERRO";
					break;
				case Def.Enums.TraceSeverity.Warning:
					strSeverity = "WARN";
					break;
				case Def.Enums.TraceSeverity.Info:
					strSeverity = "INFO";
					break;
				case Def.Enums.TraceSeverity.Verbose:
					strSeverity = "VERB";
					break;
				case Def.Enums.TraceSeverity.Ping:
					strSeverity = "PING";
					break;
				case Def.Enums.TraceSeverity.Statistics:
					strSeverity = "STAT";
					break;
				case Def.Enums.TraceSeverity.WebInterfaces:
					strSeverity = "INTF";
					break;
				case Def.Enums.TraceSeverity.Map:
					strSeverity = "MAPS";
					break;
				case Def.Enums.TraceSeverity.Das:
					strSeverity = "DAS_";
					break;
				case Def.Enums.TraceSeverity.Ota:
					strSeverity = "OTA_";
					break;
			}

			return DateTime.Now.ToString() + "> " + strSeverity + ":: " + msg; 
		}
#else

      public static string TraceFormatLong(Def.Enums.TraceSeverity severity, string msg)
      {        
         return DateTime.Now.ToString("M/d/yyyy HH:mm:ss.fff") + "> " + Enums.TraceSeverityMessage[(int)severity] + ":: " + msg;
      }

      public static string TraceFormat(string msg)
      {       
         return DateTime.Now.ToString() + "> " + msg;
      }

      public static string TraceFormat(Def.Enums.TraceSeverity severity, string msg)
      {        
         return DateTime.Now.ToString() + "> " + Enums.TraceSeverityMessage[(int)severity] + ":: " + msg;
      }

#endif
		/// <summary>
		/// Searches for the occurance on one array inside another
		/// </summary>
		/// <param name="arrWhere"></param>
		/// <param name="arrWhat"></param>
		/// <returns></returns>
		public static bool ArrayContains( byte[] arrWhere, byte[] arrWhat )
		{
			if( arrWhere.Length < arrWhat.Length )
				return false ;

			int indWhat = 0 ;
			
			for(int indWhere = 0 ; indWhere < arrWhere.Length ; indWhere++ )
			{
				while( (indWhat < arrWhat.Length) && (arrWhere[indWhere] == arrWhat[indWhat]) )
				{
					indWhat++ ;
					indWhere++ ;
					if(indWhere >= arrWhere.Length)
						break ;
				}
				
				if( indWhat == arrWhat.Length )
					return true ;
				else
					indWhat = 0 ;
			}
			return false ;
		}

		public static bool ArraysEqual(  byte[] arr1, byte[] arr2 )
		{
			for( int i=0 ; i < arr1.Length ; i++ ) 
			{
				if(arr1[i] != arr2[i]) 
					return false ;
			}
			return true;		
		}
		public static byte[] StringToByteArray( string str )
		{
			byte[] arr = new byte[str.Length] ;
			for( int i = 0 ; i < str.Length ; i++ )
				arr[i] = Convert.ToByte( str[i]) ;
			return arr ;
		}

      public static string ByteArrayToString(byte[] arr, int from, int len)
      {
         if (arr == null || len == 0 || arr.Length < from  || arr.Length < (from + len) )
            return "";

         StringBuilder str = new StringBuilder();

         for (int i = from; i < from  + len; i++)
            str.Append(Convert.ToChar(arr[i]));

         return str.ToString();
      }

      public static bool IsPrintableCharacter(byte candidate)
      {
         return !(candidate < 0x20 || candidate > 127);
      }


      public static string ByteArrayToPrintableString(byte[] arr)
      {
         if (arr == null)
            return "";

         StringBuilder str = new StringBuilder();

         for (int i = 0; i < arr.Length; i++)
            if (IsPrintableCharacter(arr[i]))
               str.Append(Convert.ToChar(arr[i]));

         return str.ToString();
      }


		public static string ByteArrayToString( byte[] arr )
		{			
			if(arr == null)
				return "" ;

         StringBuilder str = new StringBuilder();

			for( int i = 0 ; i < arr.Length ; i++ )
				str.Append(Convert.ToChar(arr[i]));

			return str.ToString() ;		
		}

		public static string ByteArrayWithLengthToString( byte[] arr, int length )
		{
			if(arr == null)
				return "" ;

         StringBuilder str = new StringBuilder();			

			for( int i = 0 ; i < length ; i++ )
				str.Append(Convert.ToChar(arr[i]));

			return str.ToString() ;		
		}
/*
		public static string ByteArrayAsHexDumpToString( byte[] arr )
		{
			string str = " < " ;
			
			if(arr == null)
				return "" ;

			for( int i = 0 ; i < arr.Length ; i++ )
				str += String.Format("{0:X2} ",arr[i]);
			return str + ">" ;		
		}
*/

      public static byte[] GStringArrayAsHexDumpToChar(string dump, int length)
      {
         if (string.IsNullOrEmpty(dump) || length < 0 || length*2 > dump.Length)
            return null;

         dump = dump.Trim();

         byte[] bBuff = new byte[length];

         for (int i = 0; i < length; i++)
         {
            int bt = (dump[i * 2] * 256) | dump[i * 2 + 1];
            bBuff[i] = (byte)bt;// Convert.ToByte(dump.Substring(i*2, 2), 16);
         }

         return bBuff;
      }
      public static byte[] GStringArrayAsHexDumpToChar(string dump)
      {
         return StringArrayAsHexDumpToChar(dump, dump.Length / 2);
      }

		/// <summary>
		/// converts string representation of hex byte to byte array
		/// from: "07 5F A0" - string representation
		/// to:   {7,5F,A0} - byte[] array
		/// </summary>
		/// <param name="dump"></param>
		/// <param name="length"></param>
		/// <returns></returns>      
		public static byte[] StringArrayAsHexDumpToChar( string dump, int length )
		{
			byte[] bBuff = new byte[length];

			for( int i = 0 ; i < length ; i++ )
			{
				dump=dump.Trim();
				string val = dump.Substring(0,2);
				dump=dump.Substring(2,dump.Length-2);
				bBuff[i] = Convert.ToByte(val,16);
			}
			return bBuff ;		
		}

		public static byte[] StringArrayAsHexDumpToChar( string dump )
		{
			return StringArrayAsHexDumpToChar( dump, dump.Length / 2) ;		
		}

      static public uint IPAddressToLong(string IPAddr)
      {
         System.Net.IPAddress oIP = System.Net.IPAddress.Parse(IPAddr);
         byte[] byteIP = oIP.GetAddressBytes();


         uint ip = (uint)byteIP[3] << 24;
         ip += (uint)byteIP[2] << 16;
         ip += (uint)byteIP[1] << 8;
         ip += (uint)byteIP[0];

         return ip;
      }

      static public uint IPAddressToLong(IPAddress oIP)
      {
         byte[] byteIP = oIP.GetAddressBytes();
         uint ip = (uint)byteIP[3] << 24;
         ip += (uint)byteIP[2] << 16;
         ip += (uint)byteIP[1] << 8;
         ip += (uint)byteIP[0];

         return ip;
      }

      static public string LongToIPAddress(uint IPAddr)
      {
         return new System.Net.IPAddress(IPAddr).ToString();
      }

      static public class _INTERNAL
      {
         /** \brief these are the headers added for different protocols 
          */
         public const short INTERNAL_IP = 1;         
         public const short INTERNAL_MICROBURST = 2;
         public const short INTERNAL_SMS = 3;
         public const short INTERNAL_CMFIN = 4;       ///< serialized version of CMFIn
         public const short INTERNAL_CMFOUT = 5;      ///< serialized version of CMFOut


         /** 
          *  \brief these are the headers added for internal communication 
          */
         public const short INTERNAL_CUSTOMMESSAGE = 11;
         public const short INTERNAL_SIP = 12;
         public const short INTERNAL_DCLMESSAGE = 13;
      }

      public static byte[] PackUDP(byte[] array, int boxId, uint comm1, int comm2)
      {
         byte[] arrDest = new byte[14 + array.Length];
         array.CopyTo(arrDest, 14 ) ;
         BitConverter.GetBytes(_INTERNAL.INTERNAL_IP).CopyTo(arrDest, 0);
         BitConverter.GetBytes(boxId).CopyTo(arrDest, 2) ;
         BitConverter.GetBytes(comm1).CopyTo(arrDest, 6) ;
         BitConverter.GetBytes(comm2).CopyTo(arrDest, 10) ;
         return arrDest;
      }

      public static Byte[] CutByteArray(byte[] header, byte[] arrSource, int length)
      {
         byte[] arrDest = new byte[length + header.Length];
         header.CopyTo(arrDest, 0);
         for (int i = header.Length; i < (header.Length + length); i++)
            arrDest[i] = arrSource[i];
         return arrDest;

      }

		public static Byte [] CutByteArray(byte[] arrSource, int length)
		{
			byte[] arrDest = new byte[length] ;
			for( int i = 0 ; i < length ; i++ )
				arrDest[i] = arrSource[i];
			return arrDest ;
		}

		public static Byte [] CutByteArray(byte[] arrSource, int start, int end )
		{
			byte[] arrDest = new byte[end - start + 1] ;
			for( int i = start ; i <= end ; i++ )
				arrDest[i-start] = arrSource[i];
			return arrDest ;
		}
		/// <summary>
		/// Convert int to byte presentation in NETWORK ORDER ONLY !!!
		/// </summary>
		/// <param name="bDest">destination byte array</param>
		/// <param name="idx">index of byte array to put int value</param>
		/// <param name="num">int to convert</param>
		/// <param name="length">length of int (1,2,3,4) </param>
		public static void IntToByteConvertion(byte[] bDest,int idx,uint num,int length)
		{
			for( int i=0;i<length;i++)
			{
				bDest[idx+length-i-1] = (byte)(num&0xff);
				num>>=8;
			}
		}
		/// <summary>
		/// Convert string to byte presentation
		/// </summary>
		/// <param name="bDest">destination byte array</param>
		/// <param name="idx">index of byte array to put string value</param>
		/// <param name="s">string</param>
		/// <param name="length">length of string</param>
		public static void StringToByteConvertion(byte[] bDest,int idx,string s,int length)
		{
			if(s.Length > length)
				s=s.Substring(0,length);
			byte[] bTmp = System.Text.Encoding.ASCII.GetBytes(s);
			int i;
			for(i=0;i<length;i++)
			{
				if(i<bTmp.Length)
					bDest[idx+i]=bTmp[i];
				else
					bDest[idx+i]=0;
			}
		}

		/// <summary>
		/// Take ulong value from byte array. NetworkOrderToHostOrder convertion.
		/// </summary>
		/// <param name="srcArray"></param>
		/// <param name="srcIndex"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public static ulong UlongFromByteArray(byte[] srcArray, int srcIndex, int length)
		{
			if(length>4||srcIndex>=srcArray.Length)
				return 0;

			ulong result = 0;
			// starting from the end
			int idx = srcIndex + length - 1;
			for(int j=0;j<length;j++)
			{
				result<<=8;
				result += srcArray[idx-j];
			}
			return result;
		}

		/// <summary>
		/// Take int value from byte array. NetworkOrderToHostOrder convertion.
		/// </summary>
		/// <param name="srcArray"></param>
		/// <param name="srcIndex"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public static int IntFromByteArray(byte[] srcArray, int srcIndex, int length)
		{
			if(length>4||srcIndex>=srcArray.Length)
				return 0;

			int result = 0;
			// starting from the end
			int idx = srcIndex + length - 1;
			for(int j=0;j<length;j++)
			{
				result<<=8;
				result += srcArray[idx-j];
			}
			return result;
		}
		/// <summary>
		/// Take ushort value from byte array. NetworkOrderToHostOrder convertion.
		/// </summary>
		/// <param name="srcArray"></param>
		/// <param name="srcIndex"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public static ushort UshortFromByteArray(byte[] srcArray, int srcIndex, int length)
		{
			if(length>2||srcIndex>=srcArray.Length)
				return 0;

			ushort result = 0;
			// starting from the end
			int idx = srcIndex + length - 1;
			for(int j=0;j<length;j++)
			{
				result<<=8;
				result += srcArray[idx-j];
			}
			return result;
		}

		/// <summary>
		/// Convert num value into byte[] array ( Little-endian order - Less Significant Byte ) 
		/// </summary>
		/// <param name="val"></param>
		/// <param name="buff"></param>
		/// <param name="startIdx"></param>
		/// <param name="number"></param>
		public static void PackIntToByteArrayLSB(ulong val, byte[] buff, int startIdx, int number )
		{
			for(int i=0;i<number;i++)
			{
				buff[i+startIdx] = (byte)(val&0xff);
				val>>=8;
			}
		}

		public static void PackIntToByteArrayLSB(ulong val, byte[] buff, int number )
		{
			for(int i=0;i<number;i++)
			{
				buff[i] = (byte)(val&0xff);
				val>>=8;
			}
		}

		/// <summary>
		/// Convert num value into byte[] array ( Big-endian order - Most Significant Byte ) 
		/// </summary>
		/// <param name="val"></param>
		/// <param name="buff"></param>
		/// <param name="startIdx"></param>
		/// <param name="number"></param>
		public static void PackIntToByteArrayMSB(ulong val, byte[] buff, int startIdx, int number )
		{
			for(int i=0;i<number;i++)
			{
				buff[number-1-i+startIdx] = (byte)(val&0xff);
				val>>=8;
			}
		}

		public static void PackIntToByteArrayMSB(ulong val, byte[] buff, int number )
		{
			for(int i=0;i<number;i++)
			{
				buff[number-1-i] = (byte)(val&0xff);
				val>>=8;
			}
		}

        public static void PackShortToByteArrayMSB(ushort val, byte[] buff, int startIdx)
        {
            for (int i = 1; i >= 0; i--)
            {
                buff[i + startIdx] = (byte)(val & 0xff);
                val >>= 8;
            }
        }
        
        public static short ShortFromByteArrayMSB(byte[] srcArray, int srcIndex)
        {
            short result = 0;
            for (int j = 0; j < 2; j++)
            {
                result <<= 8;
                result += srcArray[srcIndex + j];
            }
            return result;
        }
        
        public static ushort UshortFromByteArrayNetworkOrder(byte[] srcArray, int srcIndex)
        {
            ushort result = 0;
            for (int j = 0; j < 2; j++)
            {
                result <<= 8;
                result += srcArray[srcIndex + j];
            }
            return result;
        }
        
        public static uint UintFromByteArrayNetworkOrder(byte[] srcArray, int srcIndex)
        {
            uint result = 0;
            for (int j = 0; j < 4; j++)
            {
                result <<= 8;
                result += srcArray[srcIndex + j];
            }
            return result;
        }

		/// <summary>
		///      Finds value of specified key in the given string
		///      string may contain other records as well;
		///      string format should be:
		///      [key]=[value];
		/// </summary>
      /// <comment>
      ///      added the case when the last <pair, value> is not followed by ;
      /// </comment>
		/// <param name="key">key to look for</param>
		/// <param name="src"></param>
		/// <returns></returns>
		public static string PairFindValue( string key, string src )
		{
         try
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
                  else
                     return src.Substring(val_start + 1, src.Length - val_start - 1);
               }
            }
         }
         catch (Exception exc)
         {
            Util.BTrace(Util.ERR0, "PairfindValue -> error for string {0}", src);
         }
			return "" ;
		}
		public static string MakePair( string key, string val )
		{
			return key + "=" + val + ";" ;
		}
		public static string ReplacePair( string src, string key, string newValue )
		{
			int start = src.IndexOf( key );
			int end = src.Substring( start ).IndexOf(";") + 1;
			string oldPair = src.Substring( start, end );
			return src.Replace( oldPair, key + "=" + newValue + ";" );
		}
		/// <summary>
		/// Calculates XOR checksum
		/// </summary>
		/// <param name="arrSource">source array</param>
		/// <returns>checksum</returns>
		public static byte CalcChecksum(byte[] arrSource)
		{
			byte checksum = 0;
         if (null != arrSource)
         {
            for (int i = 0; i < arrSource.Length; i++)
               checksum ^= arrSource[i];
            if (checksum == '<' || checksum == '>' || checksum == 0)
               checksum = 0x40;
         }
			return checksum ;
		}
		/// <summary>
		/// Calculates XOR checksum given start and stop indexes
		/// </summary>
		/// <param name="arrSource">source array</param>
		/// <param name="indexFrom">index to start</param>
		/// <param name="indexTo">index to stop calculation</param>
		/// <returns>checksum</returns>
		public static byte CalcChecksum(byte[] arrSource, int indexFrom, int indexTo)
		{
			byte checksum = 0;
			for(int i = indexFrom ; (i < arrSource.Length - indexFrom + 1) && (i <= indexTo); i++ )
				checksum ^= arrSource[i] ;
			if( checksum == '<' || checksum == '>' || checksum == 0 )
				checksum = 0x40;
			return checksum ;		
		}

      public static byte CalcChecksum2(byte[] arrSource, int indexFrom, int indexTo)
      {
         byte checksum = 0 ;
         if (null != arrSource && indexFrom <= indexTo && arrSource.Length > indexTo)
         {
            for (int i = indexFrom; i <= indexTo; checksum ^= arrSource[i++]) ;
            
            if (checksum == '<' || checksum == '>' || checksum == 0)
               checksum = 0x40;
         }
         else
            Util.BTrace(Util.ERR1, "Util.CalckChecksum2 -> wrong params !");

         return checksum;
      }
      public static byte CalcChecksum3(byte[] arrSource, int indexFrom, int indexTo)
      {
          byte checksum = 0;
          if (null != arrSource && indexFrom <= indexTo && arrSource.Length > indexTo)
          {
              for (int i = indexFrom; i <= indexTo; checksum ^= arrSource[i++]) ;
          }
          else
              Util.BTrace(Util.ERR1, "Util.CalckChecksum3 -> wrong params !");

          return checksum;
      }
      /// <summary>
		///      it converts the latitude/longitude from decimal
		/// </summary>
		/// <param name="pos"></param>
		/// <returns></returns>
		public static double ConvertGPSPosition( int pos )
		{
			double[] coeff = { 1000.0, 100.0, 10.0, 1.0, 1.0/6.0, 1.0/60.0,
								 1.0/600.0, 1.0/6000.0, 1.0/60000.0, 1.0/600000.0 };

			//double C = coeff[8] ;
			double  AngleDeg = 0.0 ;
			double  Signum = (pos<0)?-1.0:+1.0;
			pos = Math.Abs(pos);

			int dig = 9;
			while (pos!=0 && dig != 0)   
			{
				int digit = pos%10;
				AngleDeg += coeff[dig--] * (double)digit;
				pos /= 10;
			}
			return Signum * AngleDeg;
		}

		public static double ConvertLatLonFromPayload( long coord )
		{
			int degree = Convert.ToInt32(coord/3600);
			int remainder = Convert.ToInt32((double)coord - degree*3600);
			int seconds = Convert.ToInt32((double)remainder/60*10000);
			return Util.ConvertGPSPosition( degree*1000000 + seconds);
		}
		/// <summary>
		/// Conversion GPS coordinate from Server presentation to Box presentation 
		/// </summary>
		/// <param name="coor">GPS coordinate - should be always positive</param>
		/// <returns></returns>
		public static uint ConvertGPSFromServerToBox(double coor)
		{
			int degree = (int)(coor);  
			coor -= degree;  
			coor *= 60.0;  
			int minutes  = (int)(coor);
			coor -= minutes;   
			coor *= 10000; 
			int seconds  = (int)(coor);
			return (uint)(1000000*degree + 10000*minutes + seconds);
		}

		/// <summary>
		/// Instantiates class given full assembly path and Class Name
		/// Throws:
		/// - System.IO.FileNotFoundException
		/// - VLFInstantiationFailedException
		/// </summary>
		/// <param name="assemblyPath">full assembly path</param>
		/// <param name="typeName">class name including namespaces</param>
		/// <returns>reference to instantiated object</returns>
		/// <example>this.pas = (IPAS)PASFactory.CreateInstance("c:/SRC/HGI VLF/VLF 1.0/Bin/Debug/HGIV10PAS.dll","VLF.PAS.HGIV10PAS")</example>
		/// <exception cref="System.IO.FileNotFoundException">Thrown when assembly file is not found</exception>
		/// <exception cref="VLFInstantiationFailedException">Thrown when class is not found in the assembly</exception>
		public static object CreateInstance( string assemblyPath, string typeName )
		{
			Assembly assm = Assembly.LoadFrom( assemblyPath ) ;		
			Object o = assm.CreateInstance( typeName ) ;
			if( o != null )
			{
				return o ;
			}
			else
                throw new VLFInstantiationFailedException("Class of type " + typeName + " is not found in the assembly " + assemblyPath);
        }

		/// <summary>
		/// Instantiates class given full assembly path, Class Name and dcl Name
		/// Throws:
		/// - System.IO.FileNotFoundException
		/// - VLFInstantiationFailedException
		/// </summary>
		/// <param name="assemblyPath">full assembly path</param>
		/// <param name="typeName">class name including namespaces</param>
		/// <param name="dclName">dcl name taken from config.xml</param>
		/// <returns>reference to instantiated object</returns>
		/// <example>this.dcl = DCLFactory.CreateInstance("c:/SRC/HGI VLF/VLF 1.0/Bin/Debug/DCL.dll","VLF.DCL.DCLBase","IPDCL")</example>
		/// <exception cref="System.IO.FileNotFoundException">Thrown when assembly file is not found</exception>
		/// <exception cref="VLFInstantiationFailedException">Thrown when class is not found in the assembly</exception>
		public static object CreateInstance( string assemblyPath, string typeName, string dclName )
		{
			Assembly assm = Assembly.LoadFrom( assemblyPath ) ;	
			string[] constructorParam = new string[ 1 ] {dclName};
			Object o = assm.CreateInstance( typeName, true, BindingFlags.Default, null, constructorParam, null, null ) ;
			if( o != null )
			{
				return o ;
			}
			else
                throw new VLFInstantiationFailedException("Class " + dclName + " of type " + typeName + " is not found in the assembly " + assemblyPath );
		}

		/// <summary>
		/// Encodes Sensor Mask
		/// </summary>
		/// <param name="sensorNum"></param>
		/// <returns>Sensor Mask</returns>
		public static long EncodeSensorMask(short sensorNum)
		{
			return 0x1 << (sensorNum - 1) ;
		}
		/// <summary>
		/// Decodes Sensor Mask
		/// </summary>
		/// <param name="sensorMask"></param>
		/// <returns>Sensor Number encoded or MaxValue if not found</returns>
		public static ushort DecodeSensorMask(long sensorMask)
		{
			ushort snsNum = ushort.MaxValue ;
			while( sensorMask != 0 )
			{
				sensorMask >>= 0x1 ;
				snsNum++ ;
			}
			snsNum++ ;
			return snsNum ;
		}

		/// <summary>
		/// Debugging HRESULTS from SqlDB and throwing DAS related Exception.
		/// Some DB error messages, see more info in master.sysmessages table
		/// This list was compiled from the Oledberr.h file, 
		/// which ships with the OLE-DB SDK:
		/// </summary>
		/// <param name="errCode"></param>
		public static void ProcessDbException(string prefixMsg, SqlException excptObj)
		{
			// TODO: add detailed error message (for the log file)
			string errMsg = excptObj.Message;
			switch(excptObj.Number)
			{
				case 1786:
				case 2627:
				case 10055:
				case 10065:
				case 11011:
				case 11040:
					throw new DASAppViolatedIntegrityConstraintsException(prefixMsg + " " + errMsg);
				case 1014:
				case 2756:
				case 3217:
				case 8967:
				case 20601:
				case 21267:
				case 21344:
					throw new DASAppInvalidValueException(prefixMsg + " " + errMsg);
				case 1001:
				case 1007:
				case 2750:
				case 2751:
				case 15086:
					throw new DASAppInvalidPrecisionException(prefixMsg + " " + errMsg);
				case 183:
				case 192:
				case 1002:
				case 2749:
				case 15087:
					throw new DASAppInvalidScaleException(prefixMsg + " " + errMsg);
				default:
					throw new DASDbException(prefixMsg + " " + errMsg);
			}
			throw new DASAppException(prefixMsg + " " + errMsg);
		}

		/// <summary>
		/// Converts 5 byte array to DateTime
		/// </summary>
		/// <param name="packet">array where date is located</param>
		/// <param name="startIdx">start index</param>
		/// <returns></returns>
		public static DateTime ByteArrayToDateTime( byte[] packet, int startIdx )
		{
			System.DateTime dt = VLF.CLS.Def.Const.unassignedDateTime;
			ushort date = 0;
			for(int i=0;i<2;i++)
			{
				date<<=8;
				date += packet[startIdx + 4 - i];
			}

			// year from 2000 - 2127 ( 7 bits )
			ushort year = (ushort)((date&0x7f) + 2000);
			date >>=7;
			ushort month = (ushort)(date&0xf);
			date >>=4;
			int hour = Convert.ToInt16(packet[startIdx]);
			int minute = Convert.ToInt16(packet[startIdx + 1]);
			int second = Convert.ToInt16(packet[startIdx + 2]);

			if( 
				( date > 0 && date < 32 ) &&
				( month >0 && month < 13 ) &&
				( hour >= 0 && hour < 24 )&&
				( minute >= 0 && minute < 60 ) &&
				( second >= 0 && second < 60 ) )
				dt = new DateTime( year, month, date, hour, minute, second );

			return dt;
		}
		/// <summary>
		/// Converts byte array to Binary Coded Decimal (BCD).
		/// </summary>
		/// <param name="packet">Array of bytes to convert.</param>
		/// <param name="startIndex">Starting index.</param>
		/// <param name="length">Length of requested packet.</param>
		/// <returns>String with converted decimals.</returns>
		public static string ByteArrayToBCD( byte[] packet, int startIndex, int length )
		{
			// converts Binary Coded Decimals
			string bcdString = "";
			for( int i=0;i<length;i++)
				bcdString+= Convert.ToString(packet[startIndex+i],16).PadLeft(2,'0');
			return bcdString;
		}
		/// <summary>
		/// Converts decimal value to byte array as Binary Coded Decimal (BCD).
		/// </summary>
		/// <param name="bcd">String for convertion.</param>
		/// <returns>Byte array as BCD.</returns>
		public static byte[] BCDToByteArray( string bcd )
		{
			// if it's not even than add leading '0'
			if( (bcd.Length % 2) != 0 )
				bcd = "0" + bcd;
			byte[] conv = new byte[bcd.Length/2];
			for( int i=0;i<bcd.Length/2;i++)
				conv[i] = Convert.ToByte(bcd.Substring(i*2,2),16);
			return conv;
		}
		/// <summary>
		/// Converts integer to byte array.
		/// </summary>
		/// <param name="value">Integer to convert.</param>
		/// <param name="number">Desired array size.</param>
		/// <returns>Array of bytes with encoded integer.</returns>
		public static byte[] ConvertIntToByteArray(ulong value, int number )
		{
			byte[] packet = new byte[number];
			for(int i=0;i<number;i++)
			{
				packet[number-1-i] = (byte)(value&0xff);
				value>>=8;
			}
			return packet;
		}
		/// <summary>
		/// Converts IP Address from Long to String.
		/// </summary>
		/// <param name="ip">IP Address as long value.</param>
		/// <returns>IP Address as String value.</returns>
		public static string LongToStringIPAddress ( long ip )
		{
			byte[] bIP = ConvertIntToByteArray( (ulong)ip, 4 );
			return bIP[3].ToString()+"."+ bIP[2].ToString()+"."+bIP[1].ToString()+"."+bIP[0].ToString();
		}
		public static byte[] HexStringToByteArray( string str )
		{
			str = str.Replace(" ","");
			byte[] data = new byte[str.Length/2];
			try
			{
				for(int i=0;i<data.Length;i++)
				{
					string hex = str.Substring(i*2,2);
					data[i]=Convert.ToByte(hex,16);
				}
			}
			catch {}
			return data;
		}
		public static byte[] HexStringToByteArrayNetworkOrder( string str )
		{
			str = str.Replace(" ","");
			byte[] data = new byte[str.Length/2];
			try
			{
				for(int i=0;i<data.Length;i++)
				{
					string hex = str.Substring( ( data.Length - 1 - i)*2,2);
					data[i]=Convert.ToByte(hex,16);
				}
			}
			catch {}
			return data;
		}
		public static bool MFCMCheckSumOK( byte[] recv )
		{
			ushort checkSum = recv[recv.Length-2];
			checkSum<<=8;
			checkSum |= recv[recv.Length-1];
			ushort contCheck = 0;
			for(int i=0;i<recv.Length-2;i++)
			{
				contCheck+=recv[i];
			}
			return ( contCheck == checkSum );
		}
		public static byte[] MFCMCalcCheckSum( byte[] packet, int length )
		{
			ulong checkSum = 0;
			for(int i=0;i<length;i++)
				checkSum+=packet[i];
			return ConvertIntToByteArray( checkSum, 2 );
		}

      public const int   INF0 = 0 ;
      public const int   INF1 = 1 ;
      public const int   INF2 = 2 ; 
      public const int   WARN0= 3 ;
      public const int   WARN1= 4 ;
      public const int   WARN2= 5 ;
      public const int   ERR0 = 6 ;
      public const int   ERR1 = 7 ;
      public const int   ERR2 = 8 ;

      public enum TRACEFLAGS:long
      {
         EMPTY    = 0,
         DATETIME = 0x00000001,
         PROCID   = 0x00000002,
         THREADID = 0x00000004,
      }
      
      public static  int verbosity = INF0 ;
      public static  TRACEFLAGS flags = TRACEFLAGS.DATETIME ;

      public static void DumpStack(Exception ex)
      {

         try
         {
            Trace.WriteLine(TraceFormat(Def.Enums.TraceSeverity.Error, "EXCEPTION: " + ex.Message));
            StringBuilder str = new StringBuilder(1024);
            StackTrace st = new StackTrace(true); // true means get line numbers.   
            foreach (StackFrame f in st.GetFrames())
               str.AppendFormat("{0}, {1} at {2}:{3}", f.GetFileName(), f.GetMethod(), f.GetFileLineNumber(), f.GetFileColumnNumber()).Append("\r\n");
            Trace.WriteLine(TraceFormat(Def.Enums.TraceSeverity.Error, "STACKTRACE: " + str.ToString()));
         }
         catch (Exception exc)
         {
            Trace.WriteLine("--Util.DumpStack -> EXCEPTION ---" + exc.Message);
         }

      }

      public static void DumpStack(Exception ex, int maxLevels)
      {

         try
         {
            Trace.WriteLine(TraceFormat(Def.Enums.TraceSeverity.Error, "EXCEPTION: " + ex.Message));
            StringBuilder str = new StringBuilder(1024);
            StackTrace st = new StackTrace(true); // true means get line numbers.   
            int cnt = 0 ;
            foreach (StackFrame f in st.GetFrames())
            {
               if (cnt++ < maxLevels)
                  str.AppendFormat("{4} {0}, {1} at {2}:{3}", f.GetFileName(), f.GetMethod(), f.GetFileLineNumber(), f.GetFileColumnNumber(), cnt).Append("\r\n");
               else
                  break;
            }

            Trace.WriteLine(TraceFormat(Def.Enums.TraceSeverity.Error, "STACKTRACE: " + str.ToString()));
         }
         catch (Exception exc)
         {
            Trace.WriteLine("--Util.DumpStack -> EXCEPTION ---" + exc.Message);
         }

      }
      public static void ReadFromListParameters(SortedList listParameters, string varName, out int varValue, int defaultValue)
      {
            varValue = defaultValue;
            try
            {
                varValue = Convert.ToInt32(listParameters[varName]);
                if (varValue == 0)
                    varValue = defaultValue;
            }
            catch { }
      }

      public static void ReadFromListParameters(SortedList listParameters, string varName, out string varValue, string defaultValue)
      {         
         try
         {
            varValue = Convert.ToString(listParameters[varName]);
            if (string.IsNullOrEmpty(varValue))
               varValue = defaultValue;
         }
         catch 
         {
            varValue = defaultValue;
         }
      }

      public static string Concat(string separator, params string[] strings)
      {
         StringBuilder result = new StringBuilder("");
         if (null != strings)
         {
            for (int i = 0; i < strings.Length; i++)
            {
               if (i > 0)
                  result.Append(separator);
               result.Append(strings[i]);
            }
         }
         
         return result.ToString();
      }
      
      /**
       *  \brief     emulates sscanf from C/C++
       */ 
      static void sscanf(string input, string format, params object[] paramlist)
      {
         string pattern = @"\b\S+\b";
         MatchCollection matches1 = Regex.Matches(input, pattern);
         MatchCollection matches2 = Regex.Matches(format, @"\b\S+\b");
         for(int i = 0; i < matches1.Count; i++)
         {
            if (matches2[i].Value == "s")
               paramlist[i] = matches1[i].Value;
            else if (matches2[i].Value == "d")
            {
               try
               {
                  int ii = int.Parse(matches1[i].Value);
                  paramlist[i] = ii;
               }
               catch (Exception exc)
               {
                  Console.WriteLine(exc.Message);
               }
            }
            else if (matches2[i].Value == "f" || matches2[i].Value == "lf")
            {
               try
               {
                  double dd = double.Parse(matches1[i].Value);
                  paramlist[i] = dd;
               }
               catch (Exception exc)
               {
                  Console.WriteLine(exc.Message);
               }
            }
         }
      }      
      
      public static string ByteArrayAsHexDumpToString( byte[] arr )
      {
         if(arr == null)
            return "" ;

         StringBuilder str = new StringBuilder("<") ;
         try
         {
            for (int i = 0; i < arr.Length; i++)
               str.Append(String.Format("{0:X2} ", arr[i]));
            str.Append(">");
         }
         catch (Exception exc)
         {
            Trace.WriteLine("-- Util.ByteArrayAsHexDumpToString -> WRONG FORMATTED DATA ---" + exc.Message);
         }
         return str.ToString() ;
      }

      public static string ByteArrayAsHexDumpToString(byte[] arr, int from, int len)
      {
         if (arr == null && (from + len) <= arr.Length)
            return "";

         StringBuilder str = new StringBuilder("<");
         try
         {
            for (int i = from; i < from + len; i++)
               str.Append(String.Format("{0:X2} ", arr[i]));
            str.Append(">");
         }
         catch (Exception exc)
         {
            Trace.WriteLine("-- Util.ByteArrayAsHexDumpToString -> WRONG FORMATTED DATA ---" + exc.Message);
         }
         return str.ToString();
      }

      /** \fn     static void BTrace(int verbosity, string msg, paramlist )
       *  \brief  this functions accepts messags with multiple arguments and it never throws
       */
      public static void BTrace(int verbosity_, string format, params object[] objects )
      {
         try
         {
            if (verbosity <= verbosity_)
            {
               StringBuilder strDynamic = new StringBuilder();
               strDynamic.AppendFormat(format, objects);               
               //            Trace.WriteLine(strDynamic.ToString()) ;
               Def.Enums.TraceSeverity sev = Enums.TraceSeverity.Info;
               if (verbosity_ == Util.ERR0 || verbosity_ == Util.ERR1 || verbosity_ == Util.ERR2) 
                  Trace.WriteLine(TraceFormat(Def.Enums.TraceSeverity.Error, strDynamic.ToString()));
               else if (verbosity_ == Util.WARN0 || verbosity_ == Util.WARN1 || verbosity_ == Util.WARN2)
                  Trace.WriteLine(TraceFormat(Def.Enums.TraceSeverity.Warning, strDynamic.ToString()));
               else Trace.WriteLine(TraceFormat(Def.Enums.TraceSeverity.Info, strDynamic.ToString()));
            }
         }
         catch(Exception exc)
         {
            Trace.WriteLine("-- Util.BTrace -> WRONG FORMATTED DATA ---" + exc.Message);
         }
      }

      public static void BTraceLong(int verbosity_, string format, params object[] objects)
      {
         try
         {
            if (verbosity <= verbosity_)
            {
               StringBuilder strDynamic = new StringBuilder();
               strDynamic.AppendFormat(format, objects);
               //            Trace.WriteLine(strDynamic.ToString()) ;
               Def.Enums.TraceSeverity sev = Enums.TraceSeverity.Info;
               if (verbosity_ == Util.ERR0 || verbosity_ == Util.ERR1 || verbosity_ == Util.ERR2)
                  Trace.WriteLine(TraceFormatLong(Def.Enums.TraceSeverity.Error, strDynamic.ToString()));
               else if (verbosity_ == Util.WARN0 || verbosity_ == Util.WARN1 || verbosity_ == Util.WARN2)
                  Trace.WriteLine(TraceFormatLong(Def.Enums.TraceSeverity.Warning, strDynamic.ToString()));
               else Trace.WriteLine(TraceFormatLong(Def.Enums.TraceSeverity.Info, strDynamic.ToString()));
            }
         }
         catch (Exception exc)
         {
            Trace.WriteLine("-- Util.BTrace -> WRONG FORMATTED DATA ---" + exc.Message);
         }
      }

      public static bool IsTable(System.Data.DataSet ds)
      {
         // dsResult != null && dsResult.Tables.Count > 0 && dsResult.Tables[0].Rows.Count > 0
         return (null != ds && 0 < ds.Tables.Count && 0 < ds.Tables[0].Rows.Count);
      }

      public static bool IsEmptyQuery(System.Data.DataSet ds)
      {
         return (ds == null ||
                 ds.Tables.Count == 0 ||
                 ds.Tables[0].Rows.Count == 0);
      }

      public static bool EmptyDataSet(DataSet dsVehicInfo)
      {
         return (dsVehicInfo == null || dsVehicInfo.Tables.Count == 0 || dsVehicInfo.Tables[0].Rows.Count == 0);
      }

      public static bool  Field2Bool(DataRow ittr, string name, bool default_)
      {
         bool ret = default_ ;
         try
         {
            ret = Convert.ToBoolean(ittr[name]);
         }
         catch
         {
         }

         return ret;

         
      }
      public static short Field2Int16(DataRow ittr, string name, short default_)
      {
         short ret = default_;
         try
         {
            ret = Convert.ToInt16(ittr[name]);
         }
         catch
         {
         }

         return ret;
      }

      public static void Field2String(DataRow ittr, string name, ref string str, string default_)
      {
         try
         {
            str = ittr[name].ToString().TrimEnd();
         }
         catch
         {
            str = default_;
         }
      }
      

      public static bool IsLandmark(object obj)
      {
         //     if (ittr["NearestLandmark"].ToString() != "" && ittr["NearestLandmark"].ToString().StartsWith(VLF.CLS.Def.Const.addressNA) == false)
         return ("" != obj.ToString() && false == obj.ToString().StartsWith(VLF.CLS.Def.Const.addressNA));
      }

      public static long DiffInMinutes(DateTime t1, DateTime t2)
      {
         if (t1 <= t2)
         {
            TimeSpan ts = t2 - t1;
            return ts.Minutes + ts.Hours * 60 + ts.Days * 60 * 24;
         }

         return 0L;

      }

      /// <summary>
      ///      this is code taken from http://konsulent.sandelien.no/VB_help/Week/
      ///      because  the code below seems to be wrong 
      /// </summary>
      public static int WeekNumber(DateTime date)
      {
      /// 
      ///         // Using builtin .NET culture features and C#
      ///         // Set a DateTime called 'date' to be December 31st 2003.
       //       Create an instance of current culture
               System.Globalization.CultureInfo currCulture = 
               System.Globalization.CultureInfo.CurrentCulture ; 
               // Get the Norwegian calendar from the culture object
               System.Globalization.Calendar cal = currCulture.Calendar;
               // Use the GetWeekOfYear method on the calendar object to 
               // get the correct week of the year
               return cal.GetWeekOfYear(date,
                                     currCulture.DateTimeFormat.CalendarWeekRule,
                                     currCulture.DateTimeFormat.FirstDayOfWeek);
      }
      /// 
      /// </summary>
      /// <param name="date"></param>
      /// <returns></returns>
      public static int WeekNumber_Entire4DayWeekRule(DateTime date)
      {
         // Updated 2004.09.27. Cleaned the code and fixed a bug. Compared the algorithm with
         // code published here . Tested code successfully against the other algorithm 
         // for all dates in all years between 1900 and 2100.
         // Thanks to Marcus Dahlberg for pointing out the deficient logic.

         // Calculates the ISO 8601 Week Number
         // In this scenario the first day of the week is monday, 
         // and the week rule states that:
         // [...] the first calendar week of a year is the one 
         // that includes the first Thursday of that year and 
         // [...] the last calendar week of a calendar year is 
         // the week immediately preceding the first 
         // calendar week of the next year.
         // The first week of the year may thus start in the 
         // preceding year

         const int JAN = 1;
         const int DEC = 12;
         const int LASTDAYOFDEC = 31;
         const int FIRSTDAYOFJAN = 1;
         const int THURSDAY = 4;
         bool ThursdayFlag = false;

         // Get the day number since the beginning of the year
         int DayOfYear = date.DayOfYear;

         // Get the numeric weekday of the first day of the 
         // year (using sunday as FirstDay)
         int StartWeekDayOfYear =
              (int)(new DateTime(date.Year, JAN, FIRSTDAYOFJAN)).DayOfWeek;
         int EndWeekDayOfYear =
              (int)(new DateTime(date.Year, DEC, LASTDAYOFDEC)).DayOfWeek;

         // Compensate for the fact that we are using monday
         // as the first day of the week
         if (StartWeekDayOfYear == 0)
            StartWeekDayOfYear = 7;
         if (EndWeekDayOfYear == 0)
            EndWeekDayOfYear = 7;

         // Calculate the number of days in the first and last week
         int DaysInFirstWeek = 8 - (StartWeekDayOfYear);
         int DaysInLastWeek = 8 - (EndWeekDayOfYear);

         // If the year either starts or ends on a thursday it will have a 53rd week
         if (StartWeekDayOfYear == THURSDAY || EndWeekDayOfYear == THURSDAY)
            ThursdayFlag = true;

         // We begin by calculating the number of FULL weeks between the start of the year and
         // our date. The number is rounded up, so the smallest possible value is 0.
         int FullWeeks = (int)Math.Ceiling((DayOfYear - (DaysInFirstWeek)) / 7.0);

         int WeekNumber = FullWeeks;

         // If the first week of the year has at least four days, then the actual week number for our date
         // can be incremented by one.
         if (DaysInFirstWeek >= THURSDAY)
            WeekNumber = WeekNumber + 1;

         // If week number is larger than week 52 (and the year doesn't either start or end on a thursday)
         // then the correct week number is 1.
         if (WeekNumber > 52 && !ThursdayFlag)
            WeekNumber = 1;

         // If week number is still 0, it means that we are trying to evaluate the week number for a
         // week that belongs in the previous year (since that week has 3 days or less in our date's year).
         // We therefore make a recursive call using the last day of the previous year.
         if (WeekNumber == 0)
            WeekNumber = WeekNumber_Entire4DayWeekRule(
                 new DateTime(date.Year - 1, DEC, LASTDAYOFDEC));
         return WeekNumber;
      }

        /// <summary>
        /// Check if a dataset is not null and its table[0] is not empty
        /// </summary>
        /// <param name="dataset"></param>
        /// <returns>true or false</returns>
        public static bool IsDataSetValid(DataSet dataset)
        {
            if (dataset != null && dataset.Tables.Count > 0 && dataset.Tables[0].Rows.Count > 0)
                return true;
            else return false;
        }

        /// <summary>
        /// Check if a dataset is not null and its table[i] is not empty
        /// </summary>
        /// <param name="dataset"></param>
        /// <returns>true or false</returns>
        public static bool IsDataSetValid(DataSet dataset, int indexTable)
        {
            if (dataset != null && dataset.Tables.Count > 0 && dataset.Tables[indexTable].Rows.Count > 0)
                return true;
            else return false;
        }

      public static bool IsIPAddress(string strIPAddress, out IPAddress ipAddress)
      {
         if (null != strIPAddress)
            return IPAddress.TryParse(strIPAddress, out ipAddress);

         ipAddress = null;
         return false;
      }
#if false
      public static byte[] SerializeExact(SpecialCMFIn obj, int structsize)
      {
         structsize = Marshal.SizeOf(obj);
         obj.size = structsize;
         IntPtr buffer = Marshal.AllocHGlobal(structsize);
         Marshal.StructureToPtr(obj, buffer, false);
         byte[] streamdatas = new byte[structsize];
         Marshal.Copy(buffer, streamdatas, 0, structsize);
         Marshal.FreeHGlobal(buffer);
         return streamdatas;
      }

      public static CMFIn DeserializeExact(byte[] data, int size)
      {
         if (null != data && data.Length > 0 && data.Length >= size)
         {
            SpecialCMFIn cmfIn = new SpecialCMFIn();

            IntPtr buffer = Marshal.AllocHGlobal(size);
            Marshal.Copy(data, 0, buffer, size);
            cmfIn = (SpecialCMFIn)Marshal.PtrToStructure(buffer, typeof(SpecialCMFIn));
            Marshal.FreeHGlobal(buffer);
            return new CMFIn(cmfIn);

            /*
                        IntPtr pData = GlobalLock(buffer);
                        int offset = 0;

                        int size = (Int32)Marshal.ReadInt32(pData);
                        offset = Marshal.SizeOf(Type.GetType("System.Int32"));

                        cmfIn.sequenceNum = (Int32)Marshal.ReadInt32(pData, offset);
                        offset += Marshal.SizeOf(Type.GetType("System.Int32"));
            
                        cmfIn.protocolTypeID = (Int32)Marshal.ReadInt32(pData, offset);
                        offset += Marshal.SizeOf(Type.GetType("System.Int32"));

                        cmfIn.boxID = (Int32)Marshal.ReadInt32(pData, offset);
                        offset += Marshal.SizeOf(Type.GetType("System.Int32"));

                        Marshal.PtrToStructure(pData, cmfIn.commInfo1);
                        offset += Marshal.SizeOf(Type.GetType("System.DateTime"));

                        Marshal.PtrToStructure(pData, cmfIn.receivedDateTime);
                        offset += Marshal.SizeOf(Type.GetType("System.DateTime"));



                        cmfIn.commInfo1 = Marshal.PtrToStringAnsi(pData, offset);
                        offset += 64;

                        cmfIn.commInfo1 = Marshal.PtrToStringAnsi(pData, offset);
                        offset += 64;

                        cmfIn.customProperties = Marshal.PtrToStringAnsi(pData, offset);


                        cmfIn.messageTypeID = Util.RandomInt32();
                        cmfIn.commInfo1 = Util.RandomString(64, false);
                        cmfIn.commInfo2 = Util.RandomString(64, false);

                        cmfIn.latitude = Util.RandomDouble(20, 70);
                        cmfIn.longitude = Util.RandomDouble(20, 70);
                        cmfIn.validGPS = 1;
                        cmfIn.speed = Util.RandomShort();
                        cmfIn.heading = Util.RandomShort();
                        cmfIn.sensorMask = Util.RandomLong(0, 64);
                        cmfIn.dclID = Util.RandomInt32(0, 10);
                        cmfIn.commMode = Util.RandomShort();
                        cmfIn.blobSize = Util.RandomInt32(0, 256);
                        cmfIn.customProperties = Util.RandomString(256, false);
                        cmfIn.isDuplicatedMsg = false;
                        cmfIn.isArmed = 0;
                        cmfIn.blobData = null; 
 
                        GlobalUnlock(pData);
                        Marshal.FreeHGlobal(buffer);

                        return cmfIn;
             */
         }
         else
            throw new ApplicationException("DeserializeExact -> null arguments");
      }

#endif
      public static long ExtractDuration(string keyWord, string customProp)
      {
         if (null != customProp && customProp.Length > 0)
         {
            string strDuration = VLF.CLS.Util.PairFindValue(keyWord, customProp.TrimEnd());                     

            long duration = 0L;
            try
            {
               return Convert.ToInt32(strDuration);
            }
            catch
            {
            }
         }

         return 0L;
      }


      public static string LatLongArrayToString(double[] lats)
      {
         if (null != lats && lats.Length > 0)
         {
            // build _condition strng used for store procedures
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < lats.Length; ++i)
            {
               str.Append(lats[i].ToString("###.######"));
               if (i != lats.Length - 1)
                  str.Append(",");
            }

            return str.ToString();
         }

         return "";
      }
      /// <summary>
      ///         returns item1, item2 .... itemN
      /// </summary>
      /// <param name="arr"></param>
      /// <returns></returns>
      public static string ArrayToString(int[] organizations)
      {
         if (null != organizations && organizations.Length > 0)
         {
            // build _condition strng used for store procedures
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < organizations.Length; ++i)
            {
               str.Append(organizations[i].ToString());
               if (i != organizations.Length - 1)
                  str.Append(",");
            }

            return str.ToString();
         }

         return "";
      }

      public static string Array2Base64(byte[] data)
      {
         char[] base64data = new char[(int)(Math.Ceiling((double)data.Length / 3) * 4)];
         Convert.ToBase64CharArray(data, 0, data.Length, base64data, 0);
         return new String(base64data);
      }


      public static byte[] StrToByteArray(string str)
      {
         System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
         return encoding.GetBytes(str);
      }

      public static string DumpSchema(DataTable dt)
      {
         if (null != dt && dt.Columns.Count > 0)
         {
            StringBuilder str = new StringBuilder(256);
            str.AppendFormat("Table {0} \n", dt.TableName );
            DataColumnCollection columns = dt.Columns ;
            foreach (DataColumn column in columns)
            {
             str.AppendFormat("[{0}]-[{1}]\t",column.ColumnName, column.DataType);
            }

            return str.ToString();
         }

         return "";         
      }

      public static void DumpTable(DataTable dt)
      {
         if (null != dt && dt.Rows.Count > 0 )
         {
            // dump the schema first 
            Util.BTrace(Util.INF0, DumpSchema(dt));

            foreach (DataRow dr in dt.Rows)
               if (dr.RowState != DataRowState.Deleted)
                  Util.BTrace(Util.INF0, DumpRow(dr, false));
          
         }
      }

      public static string DumpRow (DataRow dataRow, bool isSchema)
      {
         if (null != dataRow)
         {
            StringBuilder str = new StringBuilder(256);
            if (isSchema)
            {               
               str.AppendFormat("Table {0}", dataRow.Table.TableName).AppendLine();
               // Print the ColumnName and DataType for each column.
               // Use a DataTable object's DataColumnCollection.
               foreach (DataColumn column in dataRow.Table.Columns)
                  str.AppendFormat("Column[{0}]\tType[{1}]\tValue[{2}]",
                                 column.ColumnName, column.DataType, dataRow[column].ToString()).AppendLine();
            }
            else
            {
               foreach (DataColumn column in dataRow.Table.Columns)
                  str.AppendFormat("{0}\t", dataRow[column].ToString());
            }

            return str.ToString();
         }

         return "";
      }

      public static long ReadSettings(string name, long value)
      {
         long ret = value;
         if (null != ConfigurationSettings.AppSettings[name])
         {
            try
            {
               ret = Convert.ToInt64(ConfigurationSettings.AppSettings[name]);
            }
            catch
            {
            }
         }

         return ret;
      }


      public static int ReadSettings(string name, int value)
      {
         int ret = value;
         if (null != ConfigurationSettings.AppSettings[name])
         {
            try
            {
               ret = Convert.ToInt32(ConfigurationSettings.AppSettings[name]);
            }
            catch
            {
            }
         }

         return ret;
      }
      /** \fn     public static ushort ReadSettings(string name, ushort value) 
            *  \brief  reads from config.xml or fills in with a default value
            */
      public static ushort ReadSettings(string name, ushort value)
      {
         ushort ret = value;
         if (null != ConfigurationSettings.AppSettings[name])
         {
            try
            {
               ret = Convert.ToUInt16(ConfigurationSettings.AppSettings[name]);
            }
            catch
            {
            }
         }

         return ret;
      }

      /** \fn     public static ushort ReadSettings(string name, ushort value) 
       *  \brief  reads from config.xml or fills in with a default value
       */
      public static string ReadSettings(string name, string value)
      {
         string ret = value;
         if (null != ConfigurationSettings.AppSettings[name])
         {
            try
            {
               ret = ConfigurationSettings.AppSettings[name];
            }
            catch
            {
            }
         }

         return ret;
      }


      /** \fn     public static int[] ReadInt32List(string key)
       *  \brief  reads a string of integers from settings and extract the numbers
       */ 
      public static int[] ReadInt32List(string name)
      {
         int[] ret = null;
         if (null != ConfigurationSettings.AppSettings[name])
         {
            try
            {
               string strRet = ConfigurationSettings.AppSettings[name];
               if (false == string.IsNullOrEmpty(strRet))
               {
                  string[] arr = strRet.Split(',');
                  if (null != arr && arr.Length > 0)
                  {
                     ret = new int[arr.Length];
                     for (int i = 0; i < arr.Length; ++i)
                        ret[i] = Convert.ToInt32(arr[i]);
                  }
               }
            }
            catch
            {

            }
         }

         return ret;
      }

      public static void VerifyNotNULL( object obj, string msg) 
      {
         if (null != obj)
            return;
         else
            throw new ApplicationException(msg);
      }

      public static void CopyArray(ref ArrayList list, Array array)
      {
         if (null != list && null != array && 0 < array.Length)
            for (int i = 0; i < array.Length; ++i)
               list.Add(array.GetValue(i));
      }

      /// <summary>
      /// Get bit value 1 or 0
      /// </summary>
      /// <param name="byteValue">Byte value</param>
      /// <param name="bitNumber">Number of bit to check</param>
      /// <returns>1 or 0</returns>
      public static int GetBit(byte byteValue, byte bitNumber)
      {
         if (bitNumber > 7)
            throw new ArgumentOutOfRangeException("Bit number value must be in range 0 - 7");
         return ((byteValue >>= bitNumber) & 1);
      }

      /// <summary>
      /// Set bit = 1
      /// </summary>
      /// <param name="byteValue"></param>
      /// <param name="bitNumber"></param>
      public static void SetBit(ref byte byteValue, byte bitNumber)
      {
         if (bitNumber > 7)
            throw new ArgumentOutOfRangeException("Bit number value must be in range 0 - 7");
         byteValue |= (byte)(1 << bitNumber);
      }

      /// <summary>
      /// Set bit = 0
      /// </summary>
      /// <param name="byteValue"></param>
      /// <param name="bitNumber"></param>
      public static void ClearBit(ref byte byteValue, byte bitNumber)
      {
         if (bitNumber > 7)
            throw new ArgumentOutOfRangeException("Bit number value must be in range 0 - 7");
         byteValue ^= (byte)(1 << bitNumber);
      }

      /// <summary>
      /// Convert DataTable to xml string
      /// </summary>
      /// <param name="table">DataTable</param>
      /// <param name="includeDeclaration">True if include first xml line: '<?xml version="1.0"?>'</param>
      /// <param name="elementName">Name of the element representing table row (if empty - name is 'Row')</param>
      /// <returns>XML string</returns>
      public static string Table2Xml(DataTable table, bool includeDeclaration, string elementName)
      {
         if (table == null)
            return "";
         XmlDocument xdoc = new XmlDocument();
         if (includeDeclaration)
            xdoc.CreateXmlDeclaration("1.0", null, null);
         XmlElement root = xdoc.CreateElement(table.TableName);
         xdoc.AppendChild(root);
         if (String.IsNullOrEmpty(elementName))
            elementName = "Row";
         foreach (DataRow row in table.Rows)
         {
            XmlElement xrow = xdoc.CreateElement(elementName);
            foreach (DataColumn clm in table.Columns)
            {
               XmlNode xnode = xdoc.CreateNode(XmlNodeType.Element, clm.ColumnName, "");
               if (row[clm] != null)
                  xnode.InnerText = row[clm].ToString();
               else
                  xnode.InnerText = "";
               xrow.AppendChild(xnode);
            }
            root.AppendChild(xrow);
         }
         return xdoc.InnerXml;
      }

      /// <summary>
      ///      this functions corrects some of the weird cases when the values received are 
      ///      corrupted
      ///      case 1: if GPS is stored then speed is 0      
      /// </summary>
      /// <param name="cmfin"></param>
      public static void DoubleCheck (ref CMFIn cmfIn )
      {
         // case 1 - storaged message has speed 0, always
         if (0L != (cmfIn.sensorMask & 0x80000000))
         {
            cmfIn.speed = 0;
         }

         // case 2 , fixed DateTime 
         if (cmfIn.originatedDateTime == VLF.CLS.Def.Const.unassignedDateTime)
            cmfIn.originatedDateTime = cmfIn.receivedDateTime;
         if (cmfIn.originatedDateTime < DateTime.Now.AddYears(-2))
         {
            cmfIn.originatedDateTime = cmfIn.receivedDateTime; // remove packets older than three years
            Util.BTrace(Util.ERR0, "-- HGIxxxPAS.ToCMF -> packet older than two years {0}", cmfIn.ToString());
         }

         // case 3, maximum speed 1 Km/h - to protect from GPS bounce
         cmfIn.speed = (cmfIn.speed < 0) ? (short)0 :
            (cmfIn.speed > 150 ? (short)1 : cmfIn.speed);

         if (cmfIn.latitude == double.NaN)
         {
            cmfIn.latitude = .0;
            cmfIn.validGPS = 1;
         }

         if (cmfIn.longitude == double.NaN)
         {
            cmfIn.longitude = .0;
            cmfIn.validGPS = 1;
         }

         // case 4, check for latitude and longitude and adjust them
         if (cmfIn.latitude < -90) 
            cmfIn.latitude = -90;
         if (cmfIn.latitude > 90)
            cmfIn.latitude = 90;
         if (cmfIn.longitude < -180)
            cmfIn.longitude = -180;
         if (cmfIn.longitude > 180)
            cmfIn.longitude = 180;
      }

      public static string ArrayToString<T>(T[] objs)
      {
         if (null != objs && objs.Length > 1)
         {
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < objs.Length; ++i)
               if (i != 0)
                  str.AppendFormat(",{0}", objs[i].ToString());
               else
                  str.Append(objs[i].ToString());

            return str.ToString();
         }

         return "";
      }

      /** FMI 
       *       0 - Data valid but above normal operational range
       *       1 - 
       * 
       *    byte1       -  MID
       *    byte2       -  PID or SID
       *    byte3       -  diagnostic code
       *    byte4       -  occurence count
       *    MIDx=34;FAULTx=0;PIDx=67;FMIx=67;CNTx=3;
       *    MIDx=34;FAULTx=1;SIDx=77;FMIx=67; (no counter)
       */
      public static string DecodeMID(int idx, byte byte1, byte byte2, byte byte3, byte byte4)
      {
         StringBuilder str = new StringBuilder(4);
         str.AppendFormat("{0}{1}={2};", Const.keyJ1708_MID, idx, byte1);
       
         // Bit 8: Occurrence Count included
         //    1 = count is included
         //    0 = count not included
         if ((byte3 & 0x80) != (byte)0)    // bit 8 count not included
         {
            str.AppendFormat("{0}{1}={2};", Const.keyJ1708_CNT, idx, byte4);
         }

         // Current Status of fault
         //    1 = fault is inactive
         //    0 = fault is active
         if ((byte3 & 0x40) != (byte)0)    // bit 7 fault active
             str.AppendFormat("{0}{1}={2};",Const.keyJ1708_FAULT, idx, 1);
         else
             str.AppendFormat("{0}{1}={2};",Const.keyJ1708_FAULT, idx, 0);

         int PidOrSid = (int)byte2;

         // bit 6 Type of diagnostic code
         //       1 = standard diagnostic code
         //       0 = expansion diagnostic code PID (PID from page 2)
         if ((byte3 & 0x20) == (byte)0)
            PidOrSid += 256;

         // bit 5 Low character identifier for a standard diagnostic code
         // 1 = low character is subsystem identifier (SID)
         // 0 = low character is parameter identifier (PID)
         if ((byte3 & 0x10) != (byte)0)    // bit 5 indicates PID or SID
            str.AppendFormat("{0}{1}={2};", Const.keyJ1708_SID, idx, PidOrSid);
         else
            str.AppendFormat("{0}{1}={2};", Const.keyJ1708_PID, idx, PidOrSid);

         str.AppendFormat("{0}{1}={2};", Const.keyJ1708_FMI, idx, byte3 & 0xF);

        
         return str.ToString();
      }

      public static string DecodeDTC(byte byte1, byte byte2)
      {
         StringBuilder str = new StringBuilder(4);
         byte firstDTC = (byte)((byte1 & 0xC0) >> 6);
         switch (firstDTC)
         {
            case (byte)0:
               str.Append('P');
               break;
            case (byte)1:
               str.Append('C');
               break;
            case (byte)2:
               str.Append('B');
               break;
            case (byte)3:
               str.Append('U');
               break;
         }

         byte secondDTC = (byte)((byte1 & 0x30) >> 4);
         str.Append(secondDTC.ToString());

         byte thirdDTC = (byte)(byte1 & 0xF);
         thirdDTC = thirdDTC > 9 ? (byte)9 : thirdDTC;
         str.Append(thirdDTC.ToString());

         byte fourthDTC = (byte)((byte2 & 0xF0) >> 4);
         fourthDTC = fourthDTC > 9 ? (byte)9 : fourthDTC;
         str.Append(fourthDTC.ToString());

         byte fifthDTC = (byte)(byte2 & 0x0F);
         fifthDTC = fifthDTC > 9 ? (byte)9 : fifthDTC;
         str.Append(fifthDTC.ToString());

         return str.ToString();
      }

      // this should depend on the source, which is found in Const.keyDTCSource
      // for OBD2 it returns DTC0=PT234,DTC1=CT234
      // for J1708 it returns MID0=23;PID0=32;FMI0=22;CNT0=1, MID1=
      public static string GetDTCCodes(string customProperties)
      {
         int cntDTCs = 0 ;
         string res = PairFindValue(Const.keyDTCInPacket, customProperties) ;
         if (string.IsNullOrEmpty(res) || false == int.TryParse(res, out cntDTCs) || 0 == cntDTCs)
            return string.Empty;
         string dtcSource = PairFindValue(Const.keyDTCSource, customProperties);
         if (string.IsNullOrEmpty(dtcSource) ||
               (dtcSource != Const.keyDTCSrc_J1708a &&
                 dtcSource != Const.keyDTCSrc_J1708  &&
                 dtcSource != Const.keyDTCSrc_OBD2 ) 
            )
            return string.Empty;

         switch (dtcSource)
         {
            case Const.keyDTCSrc_OBD2:
               {
                  // CODE 0 : PT345
                  // CODE 1 : DT345
                  // CODE 2 : RT235
                  StringBuilder str = new StringBuilder(5 * cntDTCs + 2);
                  for (int i = 0; i < cntDTCs; ++i)
                  {
                     if (i == 0)
                        str.Append(PairFindValue(Const.keyDTC + i.ToString(), customProperties));
                     else
                        str.Append(",").Append(PairFindValue(Const.keyDTC + i.ToString(), customProperties));
                  }
                  return str.ToString();
               }

            case Const.keyDTCSrc_J1708:
               // copy everything and format the code like MID=..;PID/SID=..;FMI=...;
               {
                  // CODE 0 : MID=, PID=, FMI=1, Counter=
                  StringBuilder str = new StringBuilder(5 * cntDTCs + 2);
                  for (int i = 0; i < cntDTCs; ++i)
                  {
                    if (customProperties.Contains(Const.keyJ1708_PID + i.ToString()))
                       str.AppendFormat("MID={0}, PID={1}, FMI={2}",
                          PairFindValue(Const.keyJ1708_MID + i.ToString(), customProperties),
                          PairFindValue(Const.keyJ1708_PID + i.ToString(), customProperties),
                          PairFindValue(Const.keyJ1708_FMI + i.ToString(), customProperties)).AppendLine() ;
                    else if (customProperties.Contains(Const.keyJ1708_SID + i.ToString()))
                       str.AppendFormat("MID={0}, SID={1}, FMI={2}",
                          PairFindValue(Const.keyJ1708_MID + i.ToString(), customProperties),
                          PairFindValue(Const.keyJ1708_SID + i.ToString(), customProperties),
                          PairFindValue(Const.keyJ1708_FMI + i.ToString(), customProperties)).AppendLine();
                  }
                  return str.ToString();
               }

            default:
            case Const.keyDTCSrc_J1708a:
               // return PairFindValue(Const.keyDTC+ "0", customProperties);
               return "ON";
         }
      }
   }
}
