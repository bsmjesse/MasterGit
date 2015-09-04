using System;
using VLF.CLS.Interfaces;
using VLF.ERR;
using VLF.DAS.Logic;
using System.Web.Services;
using System.Xml;
using System.Collections.Specialized;
using System.Web;
using System.Data;
using System.IO;
using System.Net;
using System;
using System.Diagnostics;

namespace VLF.ASI
{
	/// <summary>
	/// Responsible for remote communication with "Session Logic Server"
	/// </summary>
	public class CommSLS: ISLSComm
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public CommSLS()
		{
		}

        private string GetRemoteSLS()
        {

            string remoteIP = "";
            string remotePort = "";
            string remoteSLS = "";

            using (SystemConfig dbSystem = new SystemConfig(AppConfig.GetInstance().ConnectionString))
            {

                //GetSLSConn(boxID, ref remoteIP, ref remotePort);
                // if (remoteIP == "")
                // {
                //string remoteURL = "tcp://localhost:8080/SLSCommunication" ;
                //short moduleId = dbSystem.GetConfigurationModuleTypeId("HGISLS");
                short moduleId = dbSystem.GetConfigurationModuleTypeId("HGISLSOut");
                if (moduleId == VLF.CLS.Def.Const.unassignedShortValue)
                    throw new DASAppResultNotFoundException("Cannot find 'ASI' in DB.");
                remoteIP = dbSystem.GetConfigurationValue(moduleId, (short)CLS.Def.Enums.ConfigurationGroups.Common, "Remote IP");
                if (remoteIP == "")
                    throw new ASIDataNotFoundException("Unable to retrieve remote IP for communication with SLS.");
                remotePort = dbSystem.GetConfigurationValue(moduleId, (short)CLS.Def.Enums.ConfigurationGroups.Common, "Remote Port");
                if (remotePort == "")
                    throw new ASIDataNotFoundException("Unable to retrieve remote port for communication with SLS.");
                // }

                remoteSLS=string.Format(@"tcp://{0}:{1}/SLSCommunication", remoteIP, remotePort);
                //Trace.WriteLine( VLF.ASI.AppConfig.tsMain.TraceInfo," GetRemoteSLS -->"+rem//oteSLS);

                return remoteSLS;
            }
        }
		/// <summary>
		/// Sending new command to "Session Logic Server"
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="time"></param>
		/// <param name="boxID"></param>
		/// <param name="commandID"></param>
		/// <param name="paramList"></param>
		/// <param name="scheduled"></param>
		/// <param name="protocolType"></param>
		/// <param name="commMode"></param>
		/// <param name="cmdSent"></param>
		/// <param name="sessionTimeOut"></param>
        public VLF.ERRSecurity.InterfaceError SendCommand(int userId, DateTime time, int boxID, short commandID, string paramList, bool scheduled, ref short protocolType, ref short commMode, ref bool cmdSent, ref Int64 sessionTimeOut)
		{
            string remoteURL = GetRemoteSLS();
			
			ISLSComm slsComm = (ISLSComm)Activator.GetObject( typeof( ISLSComm ), remoteURL ) ;
			return slsComm.SendCommand(userId, time, boxID, commandID, paramList,scheduled, ref protocolType,ref commMode, ref cmdSent,ref sessionTimeOut) ;
		}
		/// <summary>
		/// Get command status from "Session Logic Server"
		/// </summary>
		/// <param name="boxID"></param>
		/// <param name="protocolType"></param>
		/// <param name="userID"></param>
		/// <param name="cmdStatus"></param>
        public VLF.ERRSecurity.InterfaceError GetCommandStatus(int boxID, short protocolType, int userID, ref int cmdStatus)
		{
            string remoteURL = GetRemoteSLS();
/*
			SystemConfig dbSystem = null;
			try
			{
                string remoteIP="";
                string remotePort="";

                //GetSLSConn(boxID, ref remoteIP, ref remotePort);
                //if (remoteIP == "")
                //{
                    dbSystem = new SystemConfig(AppConfig.GetInstance().ConnectionString);
                    //string remoteURL = "tcp://localhost:8080/SLSCommunication" ;
                    short moduleId = dbSystem.GetConfigurationModuleTypeId("HGISLS");
                    if (moduleId == VLF.CLS.Def.Const.unassignedShortValue)
                        throw new DASAppResultNotFoundException("Cannot find 'ASI' in DB.");
                    remoteIP = dbSystem.GetConfigurationValue(moduleId, (short)CLS.Def.Enums.ConfigurationGroups.Common, "Remote IP");
                    if (remoteIP == "")
                        throw new ASIDataNotFoundException("Unable to retrieve remote IP for communication with SLS.");
                    remotePort = dbSystem.GetConfigurationValue(moduleId, (short)CLS.Def.Enums.ConfigurationGroups.Common, "Remote Port");
                    if (remotePort == "")
                        throw new ASIDataNotFoundException("Unable to retrieve remote port for communication with SLS.");
                //}
				remoteURL = @"tcp://" + remoteIP + ":" + remotePort + "/SLSCommunication" ;
			}
			finally
			{
				if(dbSystem != null)
					dbSystem.Dispose();
			}
 */ 
			ISLSComm slsComm = (ISLSComm)Activator.GetObject( typeof( ISLSComm ), remoteURL ) ;
			return slsComm.GetCommandStatus(boxID, protocolType, userID,ref cmdStatus) ;
		}
		/// <summary>
		/// Cancel command located on "Session Logic Server"
		/// </summary>
		/// <param name="boxID"></param>
		/// <param name="protocolType"></param>
		/// <param name="userID"></param>
		/// <param name="cmdCanceled"></param>
        public VLF.ERRSecurity.InterfaceError CancelCommand(int boxID, short protocolType, int userID, ref bool cmdCanceled)
		{
            string remoteURL = GetRemoteSLS();
/*
			SystemConfig dbSystem = null;
			try
			{

                string remoteIP = "";
                string remotePort = "";

                //GetSLSConn(boxID, ref remoteIP, ref remotePort);
                // if (remoteIP == "")
                // {
                    dbSystem = new SystemConfig(AppConfig.GetInstance().ConnectionString);
                    //string remoteURL = "tcp://localhost:8080/SLSCommunication" ;
                    short moduleId = dbSystem.GetConfigurationModuleTypeId("HGISLS");
                    if (moduleId == VLF.CLS.Def.Const.unassignedShortValue)
                        throw new DASAppResultNotFoundException("Cannot find 'ASI' in DB.");
                     remoteIP = dbSystem.GetConfigurationValue(moduleId, (short)CLS.Def.Enums.ConfigurationGroups.Common, "Remote IP");
                    if (remoteIP == "")
                        throw new ASIDataNotFoundException("Unable to retrieve remote IP for communication with SLS.");
                     remotePort = dbSystem.GetConfigurationValue(moduleId, (short)CLS.Def.Enums.ConfigurationGroups.Common, "Remote Port");
                    if (remotePort == "")
                        throw new ASIDataNotFoundException("Unable to retrieve remote port for communication with SLS.");
                //}

				remoteURL = @"tcp://" + remoteIP + ":" + remotePort + "/SLSCommunication" ;
			}
			finally
			{
				if(dbSystem != null)
					dbSystem.Dispose();
			}
 */ 
			ISLSComm slsComm = (ISLSComm)Activator.GetObject( typeof( ISLSComm ), remoteURL ) ;
			return slsComm.CancelCommand(boxID, protocolType, userID, ref cmdCanceled) ;
		}


      /// <summary>
      /// Delete Session "Session Logic Server"
      /// </summary>
      /// <param name="boxID"></param>
      /// <param name="protocolType"></param>
      public VLF.ERRSecurity.InterfaceError DeleteSession(int boxID, short protocolType)
      {
          string remoteURL = GetRemoteSLS();
/*
         SystemConfig dbSystem = null;
         try
         {

            string remoteIP = "";
            string remotePort = "";

            //GetSLSConn(boxID, ref remoteIP, ref remotePort);
            // if (remoteIP == "")
            // {
            dbSystem = new SystemConfig(AppConfig.GetInstance().ConnectionString);
            //string remoteURL = "tcp://localhost:8080/SLSCommunication" ;
            short moduleId = dbSystem.GetConfigurationModuleTypeId("HGISLS");
            if (moduleId == VLF.CLS.Def.Const.unassignedShortValue)
               throw new DASAppResultNotFoundException("Cannot find 'ASI' in DB.");
            remoteIP = dbSystem.GetConfigurationValue(moduleId, (short)CLS.Def.Enums.ConfigurationGroups.Common, "Remote IP");
            if (remoteIP == "")
               throw new ASIDataNotFoundException("Unable to retrieve remote IP for communication with SLS.");
            remotePort = dbSystem.GetConfigurationValue(moduleId, (short)CLS.Def.Enums.ConfigurationGroups.Common, "Remote Port");
            if (remotePort == "")
               throw new ASIDataNotFoundException("Unable to retrieve remote port for communication with SLS.");
            //}

            remoteURL = @"tcp://" + remoteIP + ":" + remotePort + "/SLSCommunication";
         }
         finally
         {
            if (dbSystem != null)
               dbSystem.Dispose();
         }
 */ 
         ISLSComm slsComm = (ISLSComm)Activator.GetObject(typeof(ISLSComm), remoteURL);
         return slsComm.DeleteSession (boxID, protocolType);
      }

        private void  GetSLSConn(int boxID, ref string IP, ref string Port )
        {
            try
            {
                int FromBoxId = 0;
                int ToBoxId = 0;
                Port = "";
                IP = "";
                  

                XmlDocument wConfig = new XmlDocument();
                string strXMLPath =HttpContext.Current.Server.MapPath("~/web.config");
                wConfig.Load(strXMLPath);
                DataSet  ds = new DataSet();
                StringReader strrXML = new StringReader(wConfig.InnerXml.ToString());

                ds.ReadXml(strrXML);

                if (ds.Tables.IndexOf("SLS") != -1 && ds.Tables["SLS"].Rows.Count > 0)  
                {
                    foreach (DataRow dr in ds.Tables["SLS"].Rows)
                    {
                            FromBoxId = Convert.ToInt32(dr["FromBoxId"]);
                            ToBoxId = Convert.ToInt32(dr["ToBoxId"]);
                            if (boxID <=ToBoxId && boxID >= FromBoxId)
                            {
                                IP = dr["ip"].ToString() ;
                                Port = dr["port"].ToString() ;
                                return;
                            }
                        
                    }
                }
            }
            catch (Exception ex)  
            {
                Port = "";
                IP = "";
            }

        }
        /// <summary>
        /// Send multiple commands
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="time"></param>
        /// <param name="boxID"></param>
        /// <param name="commandID"></param>
        /// <param name="paramList"></param>
        /// <param name="scheduled"></param>
        /// <param name="protocolType"></param>
        /// <param name="commMode"></param>
        /// <param name="cmdSent"></param>
        /// <param name="sessionTimeOut"></param>
        /// <param name="results"></param>
        public void SendCommand(int userId,
                             DateTime time,
                             int[] boxID,
                             short commandID,
                             string paramList,
                             bool scheduled,
                             ref short[] protocolType,
                             ref short[] commMode,
                             ref bool[] cmdSent,
                             ref Int64[] sessionTimeOut,
                             ref short[] results)
        {
            string remoteURL = GetRemoteSLS();
            ISLSComm slsComm = (ISLSComm)Activator.GetObject(typeof(ISLSComm), remoteURL);
            slsComm.SendCommand(userId, time, boxID, commandID, paramList, scheduled, ref protocolType, ref commMode, ref cmdSent, ref sessionTimeOut, ref results);
        }

        /// <summary>
        /// Get command status for multiple commands
        /// </summary>
        /// <param name="boxIDs"></param>
        /// <param name="protocolTypes"></param>
        /// <param name="userID"></param>
        /// <param name="cmdStatus"></param>
        public void  GetCommandStatus(int[] boxIDs, short[] protocolTypes, int userID, ref int[] cmdStatus)
        {
            string remoteURL = GetRemoteSLS();
            ISLSComm slsComm = (ISLSComm)Activator.GetObject(typeof(ISLSComm), remoteURL);
            slsComm.GetCommandStatus(boxIDs, protocolTypes, userID, ref cmdStatus);
        }
	}
}
