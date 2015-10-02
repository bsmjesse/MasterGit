using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.IO;

namespace VLF.CLS
{
/*
   public class UdpLogLine
   {
      int _moduleId;
      int _verbosity;
      DateTime _when;
      string _msg;
   }
*/
   /** \class  UDPLogListener
    *  \brief  sends automatically all messages to an UDP server which saves them periodically in a database
    * 
    */
   public class UDPLogListener : TraceListener
	{
      int _id; 
      IPEndPoint _epHost;
      Socket _socket;
      bool   _hasRepository;

      public enum enumKeywords : int
      {
         KEY_ModuleId = 0,
         KEY_BoxId,
         KEY_UserId,
         KEY_PageId,
         KEY_PageName,
         KEY_WebMethodName,
         KEY_ReportName,
         Key_ThirdPartySoftware
      };

      public static string[] LogKeywords = 
      {
         "ModuleId",
         "boxId",
         "userId",
         "pageId",
         "pageName",
         "webFunction",
         "reportName",
         "ThirdPartySoftware"
      };

      public UDPLogListener(string specificParam) : base()
      {
         _id = Convert.ToInt32(XmlConfig.ReadValue(specificParam, "UniqueId"));

         _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
         Util.VerifyNotNULL(_socket, "null socket");

         string serverIPAddress = XmlConfig.ReadValue(specificParam, "ServerIPAddress");
         ushort port = Convert.ToUInt16(XmlConfig.ReadValue(specificParam, "ServerIPPort"));

         _epHost = new IPEndPoint(IPAddress.Parse(serverIPAddress), port);
      }

      public UDPLogListener(int id, string serverIPAddress, ushort port)
         : base()
      {
         _id = id;

         _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
         Util.VerifyNotNULL(_socket, "null socket");
         _epHost = new IPEndPoint(IPAddress.Parse(serverIPAddress), port);
      }

      public void SendLog(int moduleId, int verbosity, string msg)
      {
         if (!string.IsNullOrEmpty(msg) && null != _socket)
         {
            using (MemoryStream ms = new MemoryStream(8 + msg.Length))
            {
               byte[] ret;

               ret = BitConverter.GetBytes(moduleId);     // moduleId
               ms.Write(ret, 0, ret.Length);

               ret = BitConverter.GetBytes(verbosity);     // verbosity
               ms.Write(ret, 0, ret.Length);

               ret = BitConverter.GetBytes(msg.Length);     // length of the string
               ms.Write(ret, 0, ret.Length);

               ret = Encoding.ASCII.GetBytes(msg);     // message
               ms.Write(ret, 0, ret.Length);


               try
               {
                  _socket.SendTo(ms.ToArray(), _epHost);
               }
               catch (Exception exc)
               {
                  Trace.WriteLine("SendLog -> EXC " + exc.Message);
               }
            }
         }
      }

      public void SendLog(int verbosity, string msg)
      {
         if (!string.IsNullOrEmpty(msg) && null != _socket)
         {
            using (MemoryStream ms = new MemoryStream(8 + msg.Length))
            {
               byte[] ret;

               ret = BitConverter.GetBytes(_id);     // moduleId
               ms.Write(ret, 0, ret.Length);

               ret = BitConverter.GetBytes(verbosity);     // verbosity
               ms.Write(ret, 0, ret.Length);

               ret = BitConverter.GetBytes(msg.Length);     // length of the string
               ms.Write(ret, 0, ret.Length);

               ret = Encoding.ASCII.GetBytes(msg);     // message
               ms.Write(ret, 0, ret.Length);


               try
               {
                  _socket.SendTo(ms.ToArray(), _epHost);
               }
               catch (Exception exc)
               {
                  Trace.WriteLine("SendLog -> EXC " + exc.Message);
               }
            }
         }
      }

      public void Log(int moduleId, int verbosity_, string message)
      {
         SendLog(moduleId, verbosity_, message);
      }

      public void Log(int moduleId, int verbosity_, string format, params object[] objects)
      {
         StringBuilder strDynamic = new StringBuilder();
         strDynamic.AppendFormat(format, objects);
         Log(moduleId, verbosity_, strDynamic.ToString());
      }

      public void LogDevice(int moduleId, int deviceId, int verbosity_, string format, params object[] objects)
      {
         StringBuilder strDynamic = new StringBuilder();
         strDynamic.AppendFormat("{0}={1} ", LogKeywords[(int)enumKeywords.KEY_BoxId], deviceId);
         strDynamic.AppendFormat(format, objects);
         Log(moduleId, verbosity_, strDynamic.ToString());
      }

      public void LogUser(int moduleId, int userId, int verbosity_, string format, params object[] objects)
      {
         StringBuilder strDynamic = new StringBuilder();
         strDynamic.AppendFormat("{0}={1} ", LogKeywords[(int)enumKeywords.KEY_UserId], userId);
         strDynamic.AppendFormat(format, objects);
         Log(moduleId, verbosity_, strDynamic.ToString());
      }

      public void LogWebPage(int moduleId, int pageId, int verbosity_, string format, params object[] objects)
      {
         StringBuilder strDynamic = new StringBuilder();
         strDynamic.AppendFormat("{0}={1} ", LogKeywords[(int)enumKeywords.KEY_PageId], pageId);
         strDynamic.AppendFormat(format, objects);
         Log(moduleId, verbosity_, strDynamic.ToString());
      }

      public void LogWebPage(int moduleId, string pageName, int verbosity_, string format, params object[] objects)
      {
         StringBuilder strDynamic = new StringBuilder();
         strDynamic.AppendFormat("{0}={1} ", LogKeywords[(int)enumKeywords.KEY_PageName], pageName);
         strDynamic.AppendFormat(format, objects);
         Log(moduleId, verbosity_, strDynamic.ToString());
      }

      public void LogWebMethod(int moduleId, string methodName, int verbosity_, string format, params object[] objects)
      {
         StringBuilder strDynamic = new StringBuilder();
         strDynamic.AppendFormat("{0}={1} ", LogKeywords[(int)enumKeywords.KEY_WebMethodName], methodName);
         strDynamic.AppendFormat(format, objects);
         Log(moduleId, verbosity_, strDynamic.ToString());
      }

      public void LogReport(int moduleId, string reportName, int verbosity_, string format, params object[] objects)
      {
         StringBuilder strDynamic = new StringBuilder();
         strDynamic.AppendFormat("{0}={1} ", LogKeywords[(int)enumKeywords.KEY_ReportName], reportName);
         strDynamic.AppendFormat(format, objects);
         Log(moduleId, verbosity_, strDynamic.ToString());
      }

      public void LogThirdParty(int moduleId, string reportName, int verbosity_, string format, params object[] objects)
      {
         StringBuilder strDynamic = new StringBuilder();
         strDynamic.AppendFormat("{0}={1} ", LogKeywords[(int)enumKeywords.KEY_ReportName], reportName);
         strDynamic.AppendFormat(format, objects);
         Log(moduleId, verbosity_, strDynamic.ToString());
      }


      public override void Write(string message)
      {
         SendLog(_id, 0, message);
      }

      public override void WriteLine(string message)
      {
         SendLog(_id, 0, message);
      }

      public override void Write(object o)
      {
         Write(o.ToString());
      }

      public override void  Write(object o, string category)
      {
 	       Write(o.ToString(), category);
      }

      public override void Write(string message, string category)
      {
         Write(message + "|" + category);
      }

      public override void WriteLine(object o)
      {
         WriteLine(o.ToString());
      }

      public override void WriteLine(object o, string category)
      {
         WriteLine(o.ToString()  + "|" + category);
      }

      public override void WriteLine(string message, string category)
      {
         WriteLine(message, category);
      }
   }
}
