using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Drawing;
using VLF.ERR ;
using VLF.CLS ;
using VLF.CLS.Def ;

namespace VLF.CLS.Interfaces
{
   /// <summary>
   /// Delegate for event of the service which is called on incoming message from the DAS.
   /// </summary>
   public delegate void OnDASMsgInEventHandler(byte[] pBuff);
	/// <summary>
	/// Delegate for event of the service which is called on incoming message from the DCS.
	/// </summary>
	public delegate void OnChannelDataEvent( byte[] pBuff, string boxID, string commInfo1, string commInfo2 ) ;
	/// <summary>
	/// Delegate for event of the service which is called on incoming internal message of the DCS.
	/// </summary>
	public delegate void OnChannelEvent( byte[] pBuff, string commInfo1, string commInfo2, string commParam ) ;
	/// <summary>
	/// Delegate for event of the service which is called on outgoing message from the DAS.
	/// </summary>
	public delegate void OnDASMsgOutEventHandler( CMFOut cmfOut ) ;
   /// <summary>
   /// 
   /// </summary>
   /// <param name="pkt"></param>
   public delegate void OnDASPktOutEventHandler( byte[] pkt);
	/// <summary>
	/// Device Communication Service (DCS) provides with common interface for all of SentinelFM 
	/// communication services such as UDP GPRS service, TCP Microburst service, satellite 
	/// email service, satellite Inmarsat Standard D+ service and so on.
	/// </summary>
	public interface IDCS
	{
		/// <summary>
		/// Initializes communication service.
		/// </summary>
		/// <param name="ips">Collection of IP addresses on which communication service operates on.</param>
		/// <param name="ports">Collection of server's ports on which communication service operates on.</param>
		/// <param name="evtOnData">Event on the communication service which will be called on incoming message.</param>
		/// <param name="evtOnChannel">Event on the communication service which will be called in case of problem.</param>
		/// <param name="evtOnQueueOut">Event on the communication service which is called on outgoing message.</param>
		/// <param name="protocolSOP">Start of Packet identificator accepted by DCS.</param>
		/// <param name="protocolEOP">End of Packet identificator accepted by DCS.</param>
		/// <param name="specificProperties">Specific for the given communication service set of parameters.</param>
		/// <param name="userName">Name of the dedicated user to get access to the external server.</param>
		/// <param name="password">Password of the dedicated user to get access to the external server.</param>
		void Initialize( IList ips, IList ports, OnChannelDataEvent evtOnData, OnChannelEvent evtOnChannel, OnDASMsgOutEventHandler evtOnQueueOut, object protocolSOP, object protocolEOP, string specificProperties, string userName, string password) ;
		/// <summary>
		/// Sends out messages using dedicated communication channel.
		/// </summary>
		/// <param name="boxID">SentinelFM unique identificator of the device.</param>
		/// <param name="commInfo1">First value of communication info. It's usually IP address 
		/// (UDP or TCP messages), unique MIN number (Microburst message), email address (Satellite
		/// message), terminal ID (SkyWave message) and so on.</param>
		/// <param name="commInfo2">Second value of communication info. It could be port number (UDP or TCP messages).</param>
		/// <param name="data">Data to send out.</param>
		/// <returns>Returns true if operation was successful.</returns>
		bool SendData( string boxID, string commInfo1, string commInfo2, byte[] data  ) ;	
		/// <summary>
		/// Starts communication service.
		/// </summary>
		void Start() ;
		/// <summary>
		/// Stops communication service.
		/// </summary>
		void Stop() ;
		/// <summary>
		/// Returns array of working threads which communication service consists of.
		/// </summary>
		/// <returns>Returns array of working threads.</returns>
		ThreadBase[] GetThreadInstance() ;
		/// <summary>
		/// Multilevel <see cref="System.Diagnostics.TraceSwitch"/> object to control tracing and debug output.
		/// </summary>
		TraceSwitch TSMain
		{
			get;
			set;
		}
		/// <summary>
		/// Array of counters for performing usage calculations.
		/// </summary>
		PerformanceCounters PerformCounters
		{
			get;
			set;
		}
	}

   public interface IPAS2
   {
      CMFIn ToCMF(byte[] packet, string commInfo1, string commInfo2, int timeShift, ref bool multiRecord);
   }

	/// <summary>
	/// Protocol Adapter Service (PAS) provides with common interface for all of SentinelFM 
	/// protocol parsers such as GPRS parser (HGIv..), Microburst parser (MBv..) , satellite 
	/// parser (SATv..), D+ Access Protocol parser (DAPv..) and so on.
	/// </summary>
	public interface IPAS
	{
		/// <summary>
		/// Start of Packet identificator accepted by given PAS.
		/// </summary>
		object SOP
		{
			get ;
		}
		/// <summary>
		/// End of Packet identificator accepted by given PAS.
		/// </summary>
		object EOP
		{
			get ;
		}
		/// <summary>
		/// Unique protocol ID accepted by given PAS.
		/// </summary>
		short ProtocolTypeID
		{
			get;
		}
		/// <summary>
		/// Gets list of supported commands.
		/// </summary>
		/// <returns>List of supported commands.</returns>			
		Enums.CommandType[] GetSupportedCommands() ; 
		/// <summary>
		/// Gets list of supported messages.
		/// </summary>
		/// <returns>List of supported messages.</returns>
		Enums.MessageType[] GetSupportedMessages() ;
		/// <summary>
		/// Provides a routine to convert row incoming data to CMF structure.
		/// </summary>
		/// <remarks>
		/// This method takes incoming message from DCS and translate it to 
		/// CMF structure which later will be process by DAS.
		/// </remarks>
		/// <param name="mess">Message from the queue (MQS).</param>
		/// <param name="timeShift">Time shift to GMT time.</param>
		/// <param name="multiRecord">Indicates if packet consists of multiple records.</param>
		/// <returns>Returns filled out CMF structure.</returns>
		CMFIn ToCMF( MQSMessage mess, int timeShift, ref bool multiRecord ) ;
      
		/// <summary>
		/// Provides a routine to convert CMF structure to low-level communication
		/// channel specific message.
		/// </summary>
		/// <param name="cmfOut"></param>
		/// <returns></returns>      
		byte[] FromCMF( CMFOut cmfOut ) ;	
	}

	// SLS
	public enum CommandStatus
	{
		Idle = 0,
		Sent = 1,
		Ack = 2,
		CommTimeout = 3,
		Canceled = 4,
		Pending = 5,
      SessionNotFound = 6
	}
	public enum SessionType
	{
		Normal,
		Atomic,
		Ping,
	}
	public interface IVehicleInfo
	{
		int BoxID
		{get;}

		DateTime LastValidDateTime
		{get;set;}			
		double Latitude
		{get;set;}
		
		double Longitude
		{get;set;}
		
		int Heading
		{get;set;}
		
		short Speed
		{get;set;}		
		
		DateTime LastCommunicatedDateTime
		{get;set;}			
		
		long SensorMask
		{get;set;}		
		
		bool BoxArmed
		{get;set;}		
		bool GeoFenceEnabled
		{get;set;}		
		

		string GetVehicleInfoXml(int userId) ;
		bool GetSensorValue( int sensorID );
		void SetSensorValue( int sensorID, bool val );
	}

	/// <summary>
	/// CommSession implementation notes:
	/// 
	/// </summary>
	public interface ICommSession
	{
		Int64 CommandCommTimeout
		{
			get;
		}

		TimeSpan PollTimeout
		{
			get;
			set;
		}

		int BoxID
		{get;}

		/// <summary>
		/// ID of last command sent
		/// </summary>
		Enums.CommandType CommandTypeID
		{get;}
		/// <summary>
		/// If Session has been created as atomic or ping, user other than creator can't work with it.
		/// It will die upon receiving of Ack or timeout.
		/// </summary>
		Enums.ProtocolTypes ProtocolType
		{get;}
		/// <summary>
		/// UserID of the Session's creator
		/// </summary>
		int CreatorUserID
		{get;}
		/// <summary>
		/// Status of last command sent
		/// OperatorID must match for security
		/// If command has been acknowledged, returns status and sets command as Null.
		/// </summary>
		/// <param name="operatorID"></param>
		/// <returns>CommandStatus.Idle if operatorID doesn't match</returns>
		CommandStatus GetCommandStatus();


      /// <summary>
      ///  Status after the command expired
      /// </summary>
      /// <returns></returns>
      CommandStatus GetCommandStatusAfter();

		/// <summary>
		/// Sends command to the box. Starts command timeout timer.
		/// Resets Session timeout timer.
		/// </summary>
		/// <param name="timeSent"></param>
		/// <param name="commandID"></param>
		/// <param name="operatorID"></param>
		/// <param name="paramList"></param>
		/// <param name="enforce">if true sends command even if busy removing the last command</param>
		/// <returns>false if server is busy</returns>
		bool SendCommand( DateTime timeSent, 
                        Enums.CommandType commandType, 
                        Enums.CommMode commMode,
                        int operatorID, 
                        string paramList,
                        bool scheduled);

      bool SendCommand(DateTime timeSent,
                        Enums.CommandType commandType,
                        Enums.CommMode commMode,
                        int operatorID,
                        string paramList,
                        bool scheduled,
                        ref int sequenceNum);

		/// <summary>
		/// Terminates last sent command
		/// </summary>
		bool CancelCommand();

   

		/// <summary>
		/// Is called from MEssage distributor.
		/// Resets Sesion timeout timer.
		/// </summary>
		/// <param name="cmfIn"></param>
		void ProcessMessage( CMFIn cmfIn );
		
		/// <summary>
		/// Update dormant status
		/// </summary>
		/// <param name="cmfIn"></param>
		void UpdateDormantStatus(CMFIn cmfIn);
        /// <summary>
        /// Reset box commands scheduled DateTime
        /// </summary>
        /// <param name="cmfIn"></param>
      void AdvancedProcessing(CMFInEx cmfIn);

      /// <summary>
      ///      deletes from vlfMsgIn
      /// </summary>
      /// <param name="cmfIn"></param>
//      void ClearMessage(CMFIn cmfIn);

		/// <summary>
		/// Reference count
		/// </summary>
		/// <returns></returns>
		int AddRef();
		/// <summary>
		/// Reference count implementation.
		/// Closes Session if there are no more users.
		/// </summary>
		/// <returns></returns>
		int Release();

		/// <summary>
		/// Message Distributor ref count
		/// </summary>
		/// <returns></returns>
		int SysAddRef();

		/// <summary>
		/// Message Distributor ref count
		/// </summary>
		/// <returns></returns>
		int SysRelease();
		/// <summary>
		/// Reference count
		/// </summary>
		int RefCount
		{get;}

		/// <summary>
		/// Resets Poll timeout. interval specifies new timeout value.
		/// </summary>
		/// <param name="interval"></param>
		void ResetPollTimeout() ;
		/// <summary>
		/// Ends the session sending EndCall and removing session from the Session List
		/// </summary>
		void EndSession();
	}
	/// <summary>
	/// Base class for all Message Queue Service (MQS) message types.
	/// </summary>
	public abstract class MQSMessage
	{
		/// <summary>
		/// Data for keeping in queue.
		/// </summary>
		public object data;
		/// <summary>
        /// Unit unique identificator ( box ID, Primary MIN, Email address, 
		/// terminal ID and etc. ).
		/// </summary>
		public string boxID;
		/// <summary>
		///     First part of unit's communication info. 
        ///     <remarks> 1) It's usually IP address (UDP or TCP messages), 
        ///               2) unique MIN number (Microburst message), 
        ///               3) email address (Satellite message), 
        ///               4) terminal ID (SkyWave message) and so on.
        ///     </remarks>
		/// </summary>
		public string commInfo1;
		/// <summary>
		/// Second part of unit's communication info. It could be port number (UDP or TCP messages).
		/// </summary>
		public string commInfo2;
		/// <summary>
		/// Base constructor for a new instance of the class, derived from MQSMessage.
		/// </summary>
		/// <param name="data">Data for keeping.</param>
		/// <param name="boxID">Unique unit ID.</param>
		/// <param name="commInfo1">First value of communicaton info.</param>
		/// <param name="commInfo2">Second value of communicaton info.</param>
		public MQSMessage(object data, string boxID, string commInfo1, string commInfo2)
		{
			this.data = data;
			this.boxID = boxID;
			this.commInfo1 = commInfo1;
			this.commInfo2 = commInfo2;
		}
	}
	/// <summary>
	/// Remoting interface between SLS and ASI
	/// </summary>
	public interface ISLSComm
	{
		/// <summary>
		/// Sends command to the box
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="time"></param>
		/// <param name="boxID"></param>
		/// <param name="paramList"></param>
		/// <param name="commandID"></param>
		/// <param name="protocolType"></param>
		/// <param name="cmdSent"></param>
		/// <param name="sessionTimeOut"></param>
		/// <remarks>Sets cmdSent true if command been sent successfully,othrwise sets false</remarks>
        VLF.ERRSecurity.InterfaceError SendCommand(int userId, DateTime time, int boxID, short commandID, string paramList, bool scheduled, ref short protocolType, ref short commMode, ref bool cmdSent, ref Int64 sessionTimeOut);
		/// <summary>
		/// Cancel command
		/// </summary>
		/// <param name="boxID"></param>
		/// <param name="protocolType"></param>
		/// <param name="userID"></param>
		/// <remarks>Sets cmdCanceled true if command been canceled successfully,othrwise sets false</remarks>
        VLF.ERRSecurity.InterfaceError CancelCommand(int boxID, short protocolType, int userID, ref bool cmdCanceled);
		/// <summary>
		/// Retrives command status
		/// </summary>
		/// <param name="boxID"></param>
		/// <param name="protocolType"></param>
		/// <param name="userID"></param>
        VLF.ERRSecurity.InterfaceError GetCommandStatus(int boxID, short protocolType, int userID, ref int cmdStatus);

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
      void SendCommand(int userId,
                        DateTime time,
                        int[] boxID,
                        short commandID,
                        string paramList,
                        bool scheduled,
                        ref short[] protocolType,
                        ref short[] commMode,
                        ref bool[] cmdSent,
                        ref Int64[] sessionTimeOut,
                        ref short[] results);
        /// <summary>
        /// Get command status for multiple commands
        /// </summary>
        /// <param name="boxIDs"></param>
        /// <param name="protocolTypes"></param>
        /// <param name="userID"></param>
        /// <param name="cmdStatus"></param>
      void GetCommandStatus(int[] boxIDs, short[] protocolTypes, int userID, ref int[] cmdStatus);

      /// <summary>
      ///      deletes a session - useful for OTA process
      /// </summary>
      /// <param name="boxID"></param>
      /// <param name="protocolType"></param>
      /// <returns></returns>
      VLF.ERRSecurity.InterfaceError DeleteSession(int boxID, short protocolType);
	}
	/// <summary>
	/// Remoting interface between Application Control Manager (ACM) and 
	/// Control Panael Utility (CPU).
	/// </summary>
	public interface IACMManager
	{
		/// <summary>
		/// Gets and sets controlling status of ACM.
		/// </summary>
		bool ACMStatus
		{
			get ;
			set ;
		}
		/// <summary>
		/// Turns ACM on to enable control of all dependent modules.
		/// </summary>
		void ACM_ON();
		/// <summary>
		/// Turns ACM off to disable control of all dependent modules.
		/// </summary>
		void ACM_OFF();
	}
    /// <summary>
    /// Remoting interface to get alarms
    /// </summary>
    public interface IALarms
    {
      System.Data.DataTable GetAlarms(Int32 userId);
    }
}
