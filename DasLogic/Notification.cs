using System;
using System.Collections.Generic;
using System.Text;
using System.Data;			// for DataSet
using System.Data.SqlClient ;	// for SqlException
using VLF.CLS.Def;
using VLF.CLS;

namespace VLF.DAS.Logic
{

   /**   \class      Notification
    *    \brief      this is one of the classes which will work both with the current events and with the history of them
    */ 
   public class Notification : Das
	{
      /// <summary>
      ///         BE VERY CAREFUL, the string notifications are in the same order as the Enums.NotificationType 
      /// </summary>
      /// <comment>
      ///         public enum NotificationType : short
      ///         {
      ///            No = 0,
      ///            DTC_Codes = 1,
      ///            MIL_Light_On = 2,
      ///            Service_Maintenance = 3,
      ///            Route_adherence = 4,
      ///            Abnormal_GPS = 5,           /// this is detected if the GPS bounce too far from the previous location
      ///            Abnormal_GPS_time = 6,      /// too much in the future or in the history
      ///            Fuel_transaction = 7,
      ///            Off_working_hours = 8,
      ///            Wireless_signal_lost = 9,
      ///            J1708_Codes = 10
      ///         }
      ///   please see table vlfNotificationType in production
      /// </comment>
      /// 
      ///          
      private static string[] DataFormatForNotifications = new string[]  
      {
         "",
         ///< <d><ex>DTCSource="OBD2" DTCCnt="2" DTCInVehicle=20</ex><v>PT230</v>....<v>CT340</v></d>                                             
         "<d><ex DTCSource=\"{0}\" DTCCnt=\"{1}\" DTCInVehicle=\"{2}\" />{3}</d>",    ///< DTC codes   
         
         //////< MIL light    --> just 1, indicating the light was on                                                
         "<d>{0}</d>",                                                                  
         
         ///< Service Notification rule ...... vehicleId, boxId, description, serviceId, serviceName, status                                                
         "<d vid=\"{0}\" boxid=\"{1}\" description=\"{2}\" serviceId=\"{3}\" serviceName=\"{4}\" status=\"{5}\" />",        
         
         ///< Route adherence rule ......
         "<d route=\"{0}\" segment=\"{1}\" dt=\"{2}\" lat=\"{3}\" long=\"{4}\" />",                     
         
         ///< Abnormal_GPS, it is the speed, by default the abnormal speed is anything over 160                                                                                        
         "<d>{0}</d>",                                                                                   
         
         ///< Abnormal_GPS_time, it is the difference in time compared with current datetime, by default is 100 days
         "<d>{0}</d>",                                                                                   
         
         ///< Fuel Transaction ... NOT IMPLEMENTED YET
         "",                                          
         
         ///< the datetime of the first packet outside working hours                                                   
         "<d>{0}</d>",
         
         ///< Wireless_signal_lost
         "",

         ///< <d><ex>DTCSource="J1708" DTCCnt="2" DTCInVehicle=20</ex><v>MID|PID|FMI|COUNT</v>....<v>MID|SID|FMI|COUNT</v></d>                                             
          "<d><ex DTCSource=\"{0}\" DTCCnt=\"{1}\" DTCInVehicle=\"{2}\" />{3}</d>"    
      };

      public static string BuildXMLValues(string strFormat, int cnt, string key, string customProp)
      {
         if (customProp.Contains(key))
         {
            if (key == Const.keyDTC)
            {
               StringBuilder res = new StringBuilder(64);
               int i = 0;
               string str = VLF.CLS.Util.PairFindValue(key + i.ToString(), customProp);
               do
               {
                  res.AppendFormat(strFormat, str);
                  str = VLF.CLS.Util.PairFindValue(key + (++i).ToString(), customProp);
               } while (!string.IsNullOrEmpty(str));


               return res.ToString();
            }
            else if (key == Const.keyJ1708_MID)
            {
               StringBuilder res = new StringBuilder(64);
               string str;
               int i = 0;
               // I extract everything between MIDs and remove the index from the name
               // stick what is left in the 
               do
               {
                  if (customProp.Contains(Const.keyJ1708_PID + i.ToString()))
                   res.AppendFormat(strFormat, 
                     string.Format("MID=\"{0}\" PID=\"{1}\" FMI=\"{2}\"",
                        Util.PairFindValue(Const.keyJ1708_MID + i.ToString(), customProp),
                        Util.PairFindValue(Const.keyJ1708_PID + i.ToString(), customProp),
                        Util.PairFindValue(Const.keyJ1708_FMI + i.ToString(), customProp)));
                else if (customProp.Contains(Const.keyJ1708_SID + i.ToString()))
                   res.AppendFormat(strFormat,
                     string.Format("MID=\"{0}\" SID=\"{1}\" FMI=\"{2}\"",
                        Util.PairFindValue(Const.keyJ1708_MID + i.ToString(), customProp),
                        Util.PairFindValue(Const.keyJ1708_SID + i.ToString(), customProp),
                        Util.PairFindValue(Const.keyJ1708_FMI + i.ToString(), customProp)));

                  str = VLF.CLS.Util.PairFindValue(key + (++i).ToString(), customProp);
               } while (!string.IsNullOrEmpty(str));

               return res.ToString();
            }
         }

         return string.Empty;
      }
      /// <summary>
      ///         that is the string which is saved in vlfNotification
      /// </summary>
      /// <param name="type"></param>
      /// <param name="obj"></param>
      /// <returns></returns>
      public static string GetData(Enums.NotificationType type, object obj)
      {
         if (null != obj)
         {
            Util.BTrace(Util.INF1, ">> Notification.GetData -> Type={0} : data = {1}", type.ToString(), obj.ToString());

            string str, 
                   customProp;

            switch (type)
            {
               case Enums.NotificationType.DTC_Codes:
                  {
                     customProp = obj.ToString().TrimEnd();    // Custom Properties
                     int cnt = Convert.ToInt32(VLF.CLS.Util.PairFindValue(Const.keyDTCInPacket, customProp));
                     str = string.Format(DataFormatForNotifications[(int)type],
                                    VLF.CLS.Util.PairFindValue(Const.keyDTCSource, customProp),
                                    VLF.CLS.Util.PairFindValue(Const.keyDTCInPacket, customProp),
                                    VLF.CLS.Util.PairFindValue(Const.keyDTCInVehicle, customProp),
                                    BuildXMLValues("<v>{0}</v>", cnt, Const.keyDTC, customProp));
                     break;
                  }
               // what is the content : 
               //       MID=34;PIDorSID=67;FMI=67
               case Enums.NotificationType.J1708_Codes:
                  {
                     customProp = obj.ToString().TrimEnd();    // Custom Properties
                     int cnt = Convert.ToInt32(VLF.CLS.Util.PairFindValue(Const.keyDTCInPacket, customProp));
                     str = string.Format(DataFormatForNotifications[(int)type],
                                     VLF.CLS.Util.PairFindValue(Const.keyDTCSource, customProp),
                                     VLF.CLS.Util.PairFindValue(Const.keyDTCInPacket, customProp),
                                     VLF.CLS.Util.PairFindValue(Const.keyDTCInVehicle, customProp),
                                     BuildXMLValues("<v>{0}</v>",
                                                    cnt,
                                                    Const.keyJ1708_MID,
                                                    customProp)); 
                     break;
                  }
               case Enums.NotificationType.MIL_Light_On:
                  customProp = obj.ToString().TrimEnd();    // Custom Properties
                  str = string.Format(DataFormatForNotifications[(int)type], Const.valON);
                  break;
               case Enums.NotificationType.Service_Maintenance:      // this is build by EngineHours program 
                  str = obj.ToString().TrimEnd();
                  break;

               default: // <d>{0}</d>
                  str = string.Format(DataFormatForNotifications[(int)type],obj.ToString());
                  break;
            }

            Util.BTrace(Util.INF1, "<< Notification.GetData -> Type={0} : ret = {1}", type.ToString(), str);

            return str;
         }


         return null; 
      }

		private VLF.DAS.DB.Notification _notification = null;
      private VLF.DAS.DB.NotificationHistory _notificationHistory = null;
		#region General Interfaces
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="connectionString"></param>
		public Notification(string connectionString, bool forHistory) : base (connectionString)
		{
         if (forHistory)
            _notificationHistory = new VLF.DAS.DB.NotificationHistory(sqlExec) ;
         else
			   _notification = new VLF.DAS.DB.Notification(sqlExec);
		}
		/// <summary>
		/// Destructor
		/// </summary>
		public new void Dispose()
		{
			base.Dispose();
		}
		#endregion

      #region public interface
      public int AddNotification(Enums.NotificationType type,
                                  DateTime dt,
                                  int boxID,
                                  double latitude,
                                  double longitude,
                                  string data)
      {

         if (( type == Enums.NotificationType.MIL_Light_On        ||
               type == Enums.NotificationType.Off_working_hours   ||
               type == Enums.NotificationType.Wireless_signal_lost)     &&
               true == _notification.IsNotificationPresent(boxID, type))
         {
            Util.BTrace(Util.INF1, "at={0}, notification {1} boxid={2} already present", dt.ToString(), type.ToString(), boxID);
            return 0;
         }

         return _notification.AddNotification(type, dt, boxID, latitude, longitude, data);
      }

      // Changes for TimeZone Feature start
      public DataSet GetAllNotificationsForUserId_NewTZ(int userId,
                                                  DateTime dtFrom,
                                                  DateTime dtTo,
                                                  ulong flags)
      {
          return _notification.GetAllNotificationsForUserId_NewTZ(userId, dtFrom, dtTo);
      }
      // Changes for TimeZone Feature end


      public DataSet GetAllNotificationsForUserId(int userId,
                                                   DateTime dtFrom,
                                                   DateTime dtTo,
                                                   ulong flags)
      {
          return _notification.GetAllNotificationsForUserId(userId, dtFrom, dtTo);
      }

      /// <summary>
      ///         updates the notification and moves it in history
      ///         maybe you don't allow them to close notifications
      /// </summary>
      /// <param name="notificationId"></param>
      /// <param name="userId"></param>
      /// <param name="when"></param>
      /// <param name="reason"></param>
      /// <returns></returns>
      public bool AcknowledgeNotification(long notificationId,
                                          int userId,
                                          DateTime when,
                                          string reason)
      {
         return _notification.AcknowledgeNotification(notificationId, userId, when, reason);
      }


      /// <summary>
      ///         updates the notification and moves it in history
      ///         maybe you don't allow them to close notifications
      /// </summary>
      /// <param name="notificationId"></param>
      /// <param name="userId"></param>
      /// <param name="when"></param>
      /// <param name="reason"></param>
      /// <returns></returns>
      public bool AckNotificationMaintenance(long notificationId, Int16 typeId,
                                          int userId,
                                          DateTime when,
                                          string reason)
      {
          return _notification.AckNotificationMaintenance (notificationId,typeId, userId, when, reason);
      }
                     
                                          
      public int AddNotificationInHist(Enums.NotificationType type,
                                        DateTime dt,
                                        int boxID,
                                        double latitude,
                                        double longitude,
                                        string data,
                                        int userId,
                                        string reason)
      {
         return _notificationHistory.AddNotification(type, dt, boxID, latitude, longitude, data, userId, reason);
      }

      #endregion public interface
   }
}
