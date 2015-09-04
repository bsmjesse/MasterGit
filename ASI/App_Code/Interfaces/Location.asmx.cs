using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services;
using System.IO;
using VLF.CLS.Interfaces ;
using VLF.ERRSecurity ;
using VLF.CLS.Def ;
using VLF.DAS.Logic;
using VLF.MAP;
using System.Xml;
using System.Reflection;

namespace VLF.ASI.Interfaces
{
	
	/// <summary>
	/// This interface provide basic vehicle location services and sending commands/sensors to the vehicle.
	/// </summary>
	[WebService(Namespace="http://www.sentinelfm.com")]
	public class Location : System.Web.Services.WebService
	{
		public Location()
		{
			//CODEGEN: This call is required by the ASP.NET Web Services Designer
			InitializeComponent();
		}

      /// <summary>
      ///      by replacing the log calls we can add a UDP sender for logs
      ///      or dynamic filtering 
      /// </summary>
      /// <param name="strFormat"></param>
      /// <param name="objects"></param>
      private void Log(string strFormat, params object[] objects)
      {
         try
         {
            Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceVerbose,
                 CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                                   string.Format(strFormat, objects)));
         }
         catch (Exception exc)
         {

         }
      }


      /// <summary>
      ///      the exception should be saved in a separate file or in the Event log of the computer
      /// </summary>
      /// <param name="strFormat"></param>
      /// <param name="objects"></param>
      private void LogException(string strFormat, params object[] objects)
      {
         try
         {
            Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceError,
                 CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Error,
                                   string.Format(strFormat, objects)));
         }
         catch (Exception exc)
         {

         }
      }

      private bool ValidateUserBox(int userId, string securityId, int boxId)
      {
         LoginManager.GetInstance().SecurityCheck(userId, securityId);

         //Authorization
         using (User dbUser = new User(LoginManager.GetConnnectionString(userId)))
         {
            return dbUser.ValidateUserBox(userId, boxId);
         }
      }

      private bool ValidateSuperUser(int userId, string SID)
      {
         // Authenticate
         LoginManager.GetInstance().SecurityCheck(userId, SID);

         //Authorization
         using (User dbUser = new User(LoginManager.GetConnnectionString(userId)))
         {
            return dbUser.ValidateSuperUser(userId);
         }
      }

      private bool ValidateUserLicensePlate(int userId, string SID, string licensePlate)
      {
         // Authenticate & Authorize
         LoginManager.GetInstance().SecurityCheck(userId, SID);

         //Authorization
         using (VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId)))
         {
            return dbUser.ValidateUserLicensePlate(userId, licensePlate);
         }
      }

		#region Component Designer generated code
		
		//Required by the Web Services Designer 
		private IContainer components = null;
				
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if(disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);		
		}
		
		#endregion

		#region Command Management
		[WebMethod(Description="This method is used to clarify current execution status of the previously sent SendCommand () .Returned cmdStatus can be one of the following:Idle (0) – No command has been sentSent (1) – Command sent, no result yetAck  (2) – Command executed successfullyTimeout  (3) – Communication timed outCanceled  (4) - Command canceledQueued (5) – Command Queued")]
		public int GetCommandStatus( int userId, string SID, int boxId, short protocolType, ref int cmdStatus )
		{
			try
			{
				Log(">> GetCommandStatus(uId={0}, boxId={1}, protocolType={2})", userId, boxId, protocolType );


				//CheckBoxUserAuthorization
				VLF.DAS.Logic.SystemConfig  dbSystem=new SystemConfig( LoginManager.GetConnnectionString(userId)); 
				if (!dbSystem.CheckBoxUserAuthorization(boxId,userId))
					 return Convert.ToInt32(InterfaceError.AuthorizationFailed);;

             if (!ValidateUserBox(userId, SID, boxId))
					return Convert.ToInt32(InterfaceError.AuthorizationFailed);

				
				CommSLS commSLS = new CommSLS() ;
				return Convert.ToInt32(ASIErrorCheck.CheckError(commSLS.GetCommandStatus(boxId, protocolType, userId, ref cmdStatus),"Error occured in SLS module","SLS","See SLS log StackTrace")) ;
			}
			catch( Exception Ex )
			{
            LogException("<< GetCommandStatus : uId={0}, EXC={1}", userId, Ex.Message);
				return (int)ASIErrorCheck.CheckError( Ex ) ;
			}			
		}


        [WebMethod(Description = "This method is used to clarify current execution status of the previously sent SendCommand () .Returned cmdStatus can be one of the following:Idle (0) – No command has been sentSent (1) – Command sent, no result yetAck  (2) – Command executed successfullyTimeout  (3) – Communication timed outCanceled  (4) - Command canceledQueued (5) – Command Queued")]
        public int GetBoxCommandStatus(int userId, string SID,DateTime commDateTime, int boxId, ref int cmdStatus)
        {
            try
            {
                Log(">> GetBoxCommandStatus(uId={0}, boxId={1})", userId, boxId);


                //CheckBoxUserAuthorization
                VLF.DAS.Logic.SystemConfig dbSystem = new SystemConfig(LoginManager.GetConnnectionString(userId));
                if (!dbSystem.CheckBoxUserAuthorization(boxId, userId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed); ;

                if (!ValidateUserBox(userId, SID, boxId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);



                DataSet ds = new DataSet();


                using (VLF.DAS.Logic.Box box = new VLF.DAS.Logic.Box(LoginManager.GetConnnectionString(userId)))
                {
                    ds = box.GetLastCommunicatedDateTimeFromHistory(commDateTime, boxId);
                }

                return 0;
               
            }
            catch (Exception Ex)
            {
                LogException("<< GetBoxCommandStatus : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }



		[WebMethod(Description="Sends a command to the Vehicle. This method call should be followed by consequent calls of GetCommandStatus() to determine current execution status of the command")]
		public int SendCommand( int userId, string SID, DateTime time, int boxID, short commandID, string paramList, 
                        ref short protocolType,ref short commMode, ref bool cmdSent,ref Int64 sessionTimeOut)
		{
			try
			{
				Log(">> SendCommand(uId={0}, time={1}, boxID={2}, commandID={3}, paramList={4}, commMode={5}, protocolType={6})", 
                           userId,time,boxID,commandID,paramList,commMode,protocolType );

            if (!ValidateUserBox(userId, SID, boxID))
               return Convert.ToInt32(InterfaceError.AuthorizationFailed);
			
				
				// Authorize
				if(commandID == Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.Output))
					LoginManager.GetInstance().AuthorizeOperation(userId,VLF.CLS.Def.Enums.OperationType.Output,Convert.ToInt32(commandID));
				else
					LoginManager.GetInstance().AuthorizeOperation(userId,VLF.CLS.Def.Enums.OperationType.Command,Convert.ToInt32(commandID));

                //Outputs-----
                if ((commandID == Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.Output_2_Single_Tap)) || (commandID == Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.Output_2_Double_Tap)))
                    commandID = 4;

				CommSLS commSLS = new CommSLS() ;

            return Convert.ToInt32(ASIErrorCheck.CheckError(commSLS.SendCommand(userId, time, boxID, commandID, paramList, false, ref protocolType, ref commMode, ref cmdSent, ref sessionTimeOut), "Error occured in SLS module", "SLS", "See SLS log StackTrace"));
			}
			catch( Exception Ex )
			{
            LogException("<< SendCommand : uId={0}, CMD={2}, boxId={3} EXC={1}", userId, Ex.Message, commandID, boxID);
				return (int)ASIErrorCheck.CheckError( Ex ) ;
			}			
		}

        [WebMethod(Description = "Sends output to the Vehicle. This method call should be followed by consequent calls of GetCommandStatus() to determine current execution status of the command")]
        public int SendOutput(int userId, string SID, DateTime time, int boxID, short commandID, string paramList,
                        ref short protocolType, ref short commMode, ref bool cmdSent, ref Int64 sessionTimeOut)
        {
            try
            {
                Log(">> SendOutput(uId={0}, time={1}, boxID={2}, commandID={3}, paramList={4}, commMode={5}, protocolType={6})",
                           userId, time, boxID, commandID, paramList, commMode, protocolType);

                if (!ValidateUserBox(userId, SID, boxID))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                // Authorize
                LoginManager.GetInstance().AuthorizeOperation(userId, VLF.CLS.Def.Enums.OperationType.Output, Convert.ToInt32(commandID));

                //Outputs-----
                //if ((commandID == Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.Output_2_Single_Tap)) || (commandID == Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.Output_2_Double_Tap)))
                commandID = 4;

                CommSLS commSLS = new CommSLS();

                return Convert.ToInt32(ASIErrorCheck.CheckError(commSLS.SendCommand(userId, time, boxID, commandID, paramList, false, ref protocolType, ref commMode, ref cmdSent, ref sessionTimeOut), "Error occured in SLS module", "SLS", "See SLS log StackTrace"));
            }
            catch (Exception Ex)
            {
                LogException("<< SendOutput : uId={0}, CMD={2}, boxId={3} EXC={1}", userId, Ex.Message, commandID, boxID);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Cancel to send output to the Vehicle. This method call should be followed by consequent calls of SendOutput() to cancel current execution of the command.")]
		public int CancelCommand( int userId, string SID, int boxID, short protocolType )
		{
			try
			{
				Log(">> CancelCommand( uId={0}, boxID={1}, protocolType={2} )", userId,boxID, protocolType );

            if (!ValidateUserBox(userId, SID, boxID))
               return Convert.ToInt32(InterfaceError.AuthorizationFailed);


				bool cmdCanceled = false;
				CommSLS commSLS = new CommSLS() ;
            return Convert.ToInt32(ASIErrorCheck.CheckError(commSLS.CancelCommand(boxID, protocolType, userId, ref cmdCanceled), "Error occured in SLS module", "SLS", "See SLS log StackTrace"));
			}
			catch( Exception Ex )
			{
            LogException("<< CancelCommand : uId={0}, boxId={2} EXC={1}", userId, Ex.Message, boxID);
				return (int)ASIErrorCheck.CheckError( Ex ) ;
			}			
		}

		#endregion

		#region Vehicle Location Information
		[WebMethod(Description="Retrieves last-known vehicle location information from the Server by Box ID.")]
		public int GetVehicleLocationInfoXmlByBoxId( int userId, string SID, int boxID, ref string xml )
		{
			try
			{
				Log(">> GetVehicleLocationInfoXmlByBoxId( uId={0}, boxID={1} )", userId,boxID );

            if (!ValidateUserBox(userId, SID, boxID))
               return Convert.ToInt32(InterfaceError.AuthorizationFailed);

				DataSet dsBoxStatus = null;
				
				using( Box dbBox = new Box(LoginManager.GetConnnectionString(userId)))
            {
					dsBoxStatus = dbBox.GetBoxLastInfo(userId,boxID); 
				}

				if(ASIErrorCheck.IsAnyRecord(dsBoxStatus))
					xml = dsBoxStatus.GetXml() ;

				return (int)InterfaceError.NoError ;
			}
			catch( Exception Ex )
			{
            LogException("<< GetVehicleLocationInfoXmlByBoxId : uId={0}, boxId={2} EXC={1}", userId, Ex.Message, boxID);
				return (int)ASIErrorCheck.CheckError( Ex ) ;
			}			
		}

		[WebMethod(Description="Retrieves last-known vehicle location information from the Server by Vehicle ID.")]
		public int GetVehicleLocationInfoXmlByVehicleId( int userId, string SID, Int64 vehicleId, ref string xml )
		{
			try
			{
				Log(">> GetVehicleLocationInfoXmlByVehicleId (uId={0}, vehicleId={1})", userId,vehicleId );

				// Authenticate & Authorize
				LoginManager.GetInstance().SecurityCheck( userId, SID ) ;
				
				//Authorization
				VLF.DAS.Logic.User   dbUser=new User (LoginManager.GetConnnectionString(userId)); 
				if (!dbUser.ValidateUserVehicle(userId,vehicleId))
					return Convert.ToInt32(InterfaceError.AuthorizationFailed);


				int boxId = VLF.CLS.Def.Const.unassignedIntValue;
				DataSet dsBoxStatus = null;
				
				
				using( Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
				{
					boxId = dbVehicle.GetBoxIDByVehicleId(vehicleId);
               using ( Box dbBox = new Box(LoginManager.GetConnnectionString(userId)))
               {
					   dsBoxStatus = dbBox.GetBoxLastInfo(userId,boxId);
               }
				}
				
				if(ASIErrorCheck.IsAnyRecord(dsBoxStatus))
					xml = dsBoxStatus.GetXml() ;

				return (int)InterfaceError.NoError ;
			}
			catch( Exception Ex )
			{
            LogException("<< GetVehicleLocationInfoXmlByVehicleId : uId={0}, EXC={1}", userId, Ex.Message);
				return (int)ASIErrorCheck.CheckError( Ex ) ;
			}			
		}

		[WebMethod(Description="Retrieves last-known vehicles location information from the Server by BoxIds XML string.")]
		public int GetVehiclesLocationInfoXml(int userId, string SID, string boxIdsXml, ref string xml)
		{
         try
         {
           Log(">> GetVehiclesLocationInfoXml( uId={0}, boxIdsXml={1} )", userId, boxIdsXml);

            if (String.IsNullOrEmpty(boxIdsXml)) return (int)InterfaceError.InvalidParameter;

            // Authenticate & Authorize
            LoginManager.GetInstance().SecurityCheck(userId, SID);

            DataSet dsBoxIds = new DataSet();

            using (StringReader strrXML = new StringReader(boxIdsXml))
            {
               dsBoxIds.ReadXml(strrXML);
            }

            //Authorization
            VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId));
            foreach (DataRow rowItem in dsBoxIds.Tables[0].Rows)
            {
               if (!dbUser.ValidateUserBox(userId, Convert.ToInt32(rowItem[0])))
                  return Convert.ToInt32(InterfaceError.AuthorizationFailed);
            }

            xml = "";

            if (dsBoxIds != null && dsBoxIds.Tables.Count > 0)
            {
               DataSet dsBoxLocation = null;
               DataSet dsResult = new DataSet("VehiclesLocationInfo");

               using (Box dbBox = new Box(LoginManager.GetConnnectionString(userId)))
               {
                  foreach (DataRow ittr in dsBoxIds.Tables[0].Rows)
                  {
                     dsBoxLocation = dbBox.GetBoxLastInfo(userId, Convert.ToInt32(ittr[0]));
                     if (dsBoxLocation != null && dsBoxLocation.Tables.Count > 0)
                     {
                        dsResult.Merge(dsBoxLocation);
                     }
                  }
               }
               xml = dsResult.GetXml();
            }

            return (int)InterfaceError.NoError;
         }
         catch (Exception Ex)
         {
            LogException("<< GetVehiclesLocationInfoXml : uId={0}, EXC={1}", userId, Ex.Message);
            return (int)ASIErrorCheck.CheckError(Ex);
         }			
		}

		[WebMethod(Description="Retrieves last-known vehicles location information from the Server by License Plate.")]
		public int GetVehicleLocationInfoXmlByLicensePlate( int userId, string SID, string licensePlate, ref string xml )
		{
			try
			{
				Log(">> GetVehicleLocationInfoXmlByLicensePlate( uId={0}, licensePlate={1} )", userId,licensePlate );

				if (!ValidateUserLicensePlate(userId, SID, licensePlate))
					return Convert.ToInt32(InterfaceError.AuthorizationFailed);


				int boxId = VLF.CLS.Def.Const.unassignedIntValue;
				using( Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
            {
					boxId = dbVehicle.GetBoxIdByLicensePlate(licensePlate);
				}
				
				return GetVehicleLocationInfoXmlByBoxId(userId, SID, boxId, ref xml );
			}
			catch( Exception Ex )
			{
            LogException("<< GetVehicleLocationInfoXmlByLicensePlate : uId={0}, EXC={1}", userId, Ex.Message);
				return (int)ASIErrorCheck.CheckError( Ex ) ;
			}			
		}

		#endregion

		#region Automatic Messaging Service


      /// <summary>
      ///         this is the main function to send a command, used by most of the web functions
      /// </summary>
      /// <param name="userId"></param>
      /// <param name="boxID"></param>
      /// <param name="commandID"></param>
      /// <param name="cmdScheduledDateTime"></param>
      /// <param name="paramList"></param>
      /// <param name="protocolType"></param>
      /// <param name="commMode"></param>
      /// <param name="transmissionPeriod"></param>
      /// <param name="transmissionInterval"></param>
      /// <param name="usingDualMode"></param>
      /// <param name="taskId"></param>
      private void SendCommand(int userId, int boxID, short commandID,
                                DateTime cmdScheduledDateTime, string paramList, ref short protocolType,
                                ref short commMode, Int64 transmissionPeriod, int transmissionInterval,
                                bool usingDualMode, ref Int64 taskId)
      {
         using (Box dbBox = new Box(LoginManager.GetConnnectionString(userId)))
         {
            taskId = VLF.CLS.Def.Const.unassignedIntValue;

            if (commandID != Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.UploadFirmwareStatus))
            {
               DataSet dsCmdCommInfo = dbBox.GetPrimaryCommInfo(boxID, userId, commandID);
               // According to MichaeS requirements (Feb 7, 2006) we do not send task via secondary communication mode
               if (dsCmdCommInfo == null || dsCmdCommInfo.Tables.Count == 0 || dsCmdCommInfo.Tables[0].Rows.Count != 1)
                  throw new VLF.ERR.AMSOperationNotSupported("Command ID=" + commandID + " doesn't have primary communication for boxID=" + boxID);

               short primeProtocolPriority = Convert.ToInt16(dsCmdCommInfo.Tables[0].Rows[0]["BoxProtocolTypeId"]);
               short primeCommMode = Convert.ToInt16(dsCmdCommInfo.Tables[0].Rows[0]["CommModeId"]);

               if (primeProtocolPriority != protocolType || primeCommMode != commMode)
               {
                  string errMsg = "Command settings (currProtocolPriority=" + protocolType + " commMode=" + commMode + ") are different from datase (protocolPriority=" + primeProtocolPriority + " commMode=" + primeCommMode + ").";
                  protocolType = VLF.CLS.Def.Const.unassignedShortValue;
                  commMode = VLF.CLS.Def.Const.unassignedShortValue;
                  throw new VLF.ERR.AMSVehicleConfigurationErrorException(errMsg);
               }
            }
         }

         using (SystemConfig dbSystem = new SystemConfig(LoginManager.GetConnnectionString(userId)))
         {
            taskId = dbSystem.AddTask(userId, cmdScheduledDateTime, boxID, commandID, paramList, protocolType, 
                                          commMode, transmissionPeriod, transmissionInterval, usingDualMode);
            //taskId = dbSystem.AddTask(userId,DateTime.Now,boxID,commandID,paramList,protocolType,commMode,transmissionPeriod,transmissionInterval,usingDualMode);
         }
      }

     [WebMethod(Description = "Send a scheduled task to box with start time,re-sendind interval and period.")]
      public int SendAutomaticCommand(int userId, string SID, int boxID, short commandID, DateTime cmdScheduledDateTime, string paramList, ref short protocolType, ref short commMode, Int64 transmissionPeriod, int transmissionInterval, bool usingDualMode, ref Int64 taskId)
      {
          try
          {
              taskId = VLF.CLS.Def.Const.unassignedIntValue;
              Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceVerbose, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces, string.Format("SendAutomaticCommand( userId = {0}, boxID = {1}, commandID = {2}, paramList = {3}, commMode = {4}, protocolType = {5}, transmissionPeriod = {6}, transmissionInterval = {7}, usingDualMode = {8}, cmdScheduledDateTime = '{9}'  )", userId, boxID, commandID, paramList, commMode, protocolType, transmissionPeriod, transmissionInterval, usingDualMode, cmdScheduledDateTime.ToString())));

              if (!ValidateUserBox(userId, SID, boxID))
                  return Convert.ToInt32(InterfaceError.AuthorizationFailed);
			


              // Authorize
              if (commandID == Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.Output))
                  throw new VLF.ERR.ASIAuthorizationFailedException("SecurityCheck::AuthorizeOperation for user=" + userId + " operationType=Output operationId=" + Convert.ToInt32(commandID) + " failed.");
              else
                  LoginManager.GetInstance().AuthorizeOperation(userId, VLF.CLS.Def.Enums.OperationType.Command, Convert.ToInt32(commandID));




              VLF.DAS.Logic.SystemConfig dbSystem = null;

              Box dbBox = null;
              try
              {
                  dbBox = new Box(LoginManager.GetConnnectionString(userId));
                  dbSystem = new SystemConfig(LoginManager.GetConnnectionString(userId));
                  taskId = VLF.CLS.Def.Const.unassignedIntValue;

                  if (commandID != Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.UploadFirmwareStatus))
                  {
                      DataSet dsCmdCommInfo = dbBox.GetPrimaryCommInfo(boxID, userId, commandID);
                      // According to MichaeS requirements (Feb 7, 2006) we do not send task via secondary communication mode
                      if (dsCmdCommInfo == null || dsCmdCommInfo.Tables.Count == 0 || dsCmdCommInfo.Tables[0].Rows.Count != 1)
                          throw new VLF.ERR.AMSOperationNotSupported("Command ID=" + commandID + " doesn't have primary communication for boxID=" + boxID);

                      short primeProtocolPriority = Convert.ToInt16(dsCmdCommInfo.Tables[0].Rows[0]["BoxProtocolTypeId"]);
                      short primeCommMode = Convert.ToInt16(dsCmdCommInfo.Tables[0].Rows[0]["CommModeId"]);

                      if (primeProtocolPriority != protocolType || primeCommMode != commMode)
                      {
                          string errMsg = "Command settings (currProtocolPriority=" + protocolType + " commMode=" + commMode + ") are different from datase (protocolPriority=" + primeProtocolPriority + " commMode=" + primeCommMode + ").";
                          protocolType = VLF.CLS.Def.Const.unassignedShortValue;
                          commMode = VLF.CLS.Def.Const.unassignedShortValue;
                          throw new VLF.ERR.AMSVehicleConfigurationErrorException(errMsg);
                      }
                  }

                  taskId = dbSystem.AddTask(userId, cmdScheduledDateTime, boxID, commandID, paramList, protocolType, commMode, transmissionPeriod, transmissionInterval, usingDualMode);
                  //taskId = dbSystem.AddTask(userId,DateTime.Now,boxID,commandID,paramList,protocolType,commMode,transmissionPeriod,transmissionInterval,usingDualMode);
              }
              finally
              {
                  if (dbSystem != null)
                      dbSystem.Dispose();
                  if (dbBox != null)
                      dbBox.Dispose();
              }
              return (int)InterfaceError.NoError;
          }
          catch (Exception Ex)
          {
              taskId = VLF.CLS.Def.Const.unassignedIntValue;

              return (int)ASIErrorCheck.CheckError(Ex);
          }
      }


      [WebMethod(Description = "Send a scheduled task to box with start time,re-sendind interval and period.")]
      public int SendAutomaticCommandWithoutCommParams(int userId, string SID, int boxID, short commandID, DateTime cmdScheduledDateTime, string paramList, Int64 transmissionPeriod, int transmissionInterval, bool usingDualMode, ref Int64 taskId)
      {
          try
          {
              taskId = VLF.CLS.Def.Const.unassignedIntValue;
              Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceVerbose, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces, string.Format("SendAutomaticCommand( userId = {0}, boxID = {1}, commandID = {2}, paramList = {3}, commMode = {4}, protocolType = {5}, transmissionPeriod = {6}, transmissionInterval = {7}, usingDualMode = {8}, cmdScheduledDateTime = '{9}'  )", userId, boxID, commandID, paramList, -1, -1, transmissionPeriod, transmissionInterval, usingDualMode, cmdScheduledDateTime.ToString())));

              if (!ValidateUserBox(userId, SID, boxID))
                  return Convert.ToInt32(InterfaceError.AuthorizationFailed);
			

              // Authorize
              if (commandID == Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.Output))
                  throw new VLF.ERR.ASIAuthorizationFailedException("SecurityCheck::AuthorizeOperation for user=" + userId + " operationType=Output operationId=" + Convert.ToInt32(commandID) + " failed.");
              else
                  LoginManager.GetInstance().AuthorizeOperation(userId, VLF.CLS.Def.Enums.OperationType.Command, Convert.ToInt32(commandID));



              VLF.DAS.Logic.SystemConfig dbSystem = null;

              Box dbBox = null;
              try
              {
                  dbBox = new Box(LoginManager.GetConnnectionString(userId));
                  dbSystem = new SystemConfig(LoginManager.GetConnnectionString(userId));

                  taskId = VLF.CLS.Def.Const.unassignedIntValue;
                  DataSet dsCmdCommInfo = dbBox.GetPrimaryCommInfo(boxID, userId, commandID);
                  // According to MichaeS requirements (Feb 7, 2006) we do not send task via secondary communication mode
                  if (dsCmdCommInfo == null || dsCmdCommInfo.Tables.Count == 0 || dsCmdCommInfo.Tables[0].Rows.Count != 1)
                      throw new VLF.ERR.AMSOperationNotSupported("Command ID=" + commandID + " doesn't have primary communication for boxID=" + boxID);

                  short primeProtocolPriority = Convert.ToInt16(dsCmdCommInfo.Tables[0].Rows[0]["BoxProtocolTypeId"]);
                  short primeCommMode = Convert.ToInt16(dsCmdCommInfo.Tables[0].Rows[0]["CommModeId"]);

                  //if (primeProtocolPriority != protocolType || primeCommMode != commMode)
                  //{
                  //   string errMsg = "Command settings (currProtocolPriority=" + protocolType + " commMode=" + commMode + ") are different from datase (protocolPriority=" + primeProtocolPriority + " commMode=" + primeCommMode + ").";
                  //   protocolType = VLF.CLS.Def.Const.unassignedShortValue;
                  //   commMode = VLF.CLS.Def.Const.unassignedShortValue;
                  //   throw new VLF.ERR.AMSVehicleConfigurationErrorException(errMsg);
                  //}

                  taskId = dbSystem.AddTask(userId, DateTime.Now, boxID, commandID, paramList, primeProtocolPriority, primeCommMode, transmissionPeriod, transmissionInterval, usingDualMode);
              }
              finally
              {
                  if (dbSystem != null)
                      dbSystem.Dispose();
                  if (dbBox != null)
                      dbBox.Dispose();
              }
              return (int)InterfaceError.NoError;
          }
          catch (Exception Ex)
          {
              taskId = VLF.CLS.Def.Const.unassignedIntValue;

              return (int)ASIErrorCheck.CheckError(Ex);
          }
      }

      [WebMethod(Description = "Send a scheduled task to box by secondary channel with start time,re-sendind interval and period.")]
      public int SendAutomaticCommandWithoutCommParamsOnSecondaryMode(int userId, string SID, int boxID, short commandID, DateTime cmdScheduledDateTime, string paramList, Int64 transmissionPeriod, int transmissionInterval, bool usingDualMode, ref Int64 taskId)
      {
          try
          {
              taskId = VLF.CLS.Def.Const.unassignedIntValue;
              Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceVerbose, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces, string.Format("SendAutomaticCommand( userId = {0}, boxID = {1}, commandID = {2}, paramList = {3}, commMode = {4}, protocolType = {5}, transmissionPeriod = {6}, transmissionInterval = {7}, usingDualMode = {8}, cmdScheduledDateTime = '{9}'  )", userId, boxID, commandID, paramList, -1, -1, transmissionPeriod, transmissionInterval, usingDualMode, cmdScheduledDateTime.ToString())));

              if (!ValidateUserBox(userId, SID, boxID))
                  return Convert.ToInt32(InterfaceError.AuthorizationFailed);
			

              // Authorize
              if (commandID == Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.Output))
                  throw new VLF.ERR.ASIAuthorizationFailedException("SecurityCheck::AuthorizeOperation for user=" + userId + " operationType=Output operationId=" + Convert.ToInt32(commandID) + " failed.");
              else
                  LoginManager.GetInstance().AuthorizeOperation(userId, VLF.CLS.Def.Enums.OperationType.Command, Convert.ToInt32(commandID));



              VLF.DAS.Logic.SystemConfig dbSystem = null;

              Box dbBox = null;
              try
              {
                  dbBox = new Box(LoginManager.GetConnnectionString(userId));
                  dbSystem = new SystemConfig(LoginManager.GetConnnectionString(userId));

                  taskId = VLF.CLS.Def.Const.unassignedIntValue;
                  DataSet dsCmdCommInfo = dbBox.GetSecondaryCommInfo(boxID, userId, commandID);
                  if (dsCmdCommInfo == null || dsCmdCommInfo.Tables.Count == 0 || dsCmdCommInfo.Tables[0].Rows.Count != 1)
                      throw new VLF.ERR.AMSOperationNotSupported("Command ID=" + commandID + " doesn't have secondary communication for boxID=" + boxID);

                  short secondProtocolPriority = Convert.ToInt16(dsCmdCommInfo.Tables[0].Rows[0]["BoxProtocolTypeId"]);
                  short secondCommMode = Convert.ToInt16(dsCmdCommInfo.Tables[0].Rows[0]["CommModeId"]);

                  //if (primeProtocolPriority != protocolType || primeCommMode != commMode)
                  //{
                  //   string errMsg = "Command settings (currProtocolPriority=" + protocolType + " commMode=" + commMode + ") are different from datase (protocolPriority=" + primeProtocolPriority + " commMode=" + primeCommMode + ").";
                  //   protocolType = VLF.CLS.Def.Const.unassignedShortValue;
                  //   commMode = VLF.CLS.Def.Const.unassignedShortValue;
                  //   throw new VLF.ERR.AMSVehicleConfigurationErrorException(errMsg);
                  //}

                  taskId = dbSystem.AddTask(userId, DateTime.Now, boxID, commandID, paramList, secondProtocolPriority, secondCommMode, transmissionPeriod, transmissionInterval, usingDualMode);
              }
              finally
              {
                  if (dbSystem != null)
                      dbSystem.Dispose();
                  if (dbBox != null)
                      dbBox.Dispose();
              }
              return (int)InterfaceError.NoError;
          }
          catch (Exception Ex)
          {
              taskId = VLF.CLS.Def.Const.unassignedIntValue;

              return (int)ASIErrorCheck.CheckError(Ex);
          }
      }

        [WebMethod(Description = "Delete scheduled tasks.")]
		public int DeleteTask( int userId, string SID, Int64[] tasks,ref  bool[] taskDeleted)
		{
			try
			{
                Log(">> DeleteTask( uId={0}, tasksCount={1} )", userId, tasks.Length);

				// Authenticate 
				LoginManager.GetInstance().SecurityCheck( userId, SID ) ;
				
				// Authorize
				//TODO:LoginManager.GetInstance().AuthorizeOperation(userId,VLF.CLS.Def.Enums.OperationType.Command,Convert.ToInt32(commandID));

				
				using( SystemConfig dbSystem = new SystemConfig(LoginManager.GetConnnectionString(userId)))
                {
                    for(int i=0;i<tasks.Length;i++)
                        taskDeleted[i] = (dbSystem.DeleteTask(tasks[i]) != 0);
				}

				return (int)InterfaceError.NoError ;
			}
			catch( Exception Ex )
			{
                LogException("<< DeleteTask : uId={0}, tasksCount={1}, EXC={2} )", userId, tasks.Length , Ex.Message);
				return (int)ASIErrorCheck.CheckError( Ex ) ;
			}			
		}
		

		public int ReScheduledTask( int userId, string SID, Int64 taskId,Int64 transmissionPeriod,int transmissionInterval,bool usingDualMode,ref bool rescheduleTask)
		{
			try
			{
				Log(">> ReScheduledTask( uId={0}, taskId={1}, transmissionPeriod={2}, transmissionInterval={3}, usingDualMode={4} )", 
                           userId,taskId,transmissionPeriod,transmissionInterval,usingDualMode);

				// Authenticate 
				LoginManager.GetInstance().SecurityCheck( userId, SID ) ;
				
				// Authorize
				//TODO:LoginManager.GetInstance().AuthorizeOperation(userId,VLF.CLS.Def.Enums.OperationType.Command,Convert.ToInt32(commandID));

				rescheduleTask = false;
				using( SystemConfig dbSystem = new SystemConfig(LoginManager.GetConnnectionString(userId)))
            {
					rescheduleTask = (dbSystem.ReScheduledTask(taskId,transmissionPeriod,transmissionInterval,usingDualMode) != 0) ;
				}

				return (int)InterfaceError.NoError ;
			}
			catch( Exception Ex )
			{
            LogException("<< ReScheduledTask : uId={0}, taskId={1}, EXC={2} )", userId, taskId, Ex.Message);
				return (int)ASIErrorCheck.CheckError( Ex ) ;
			}			
		}

		[WebMethod(Description="Get user scheduled tasks.")]
		public int GetUserTasks( int userId, string SID,ref string xml)
		{
			try
			{
				Log(">> GetUserTasks( uId={0} )", userId );

				// Authenticate 
				LoginManager.GetInstance().SecurityCheck( userId, SID ) ;
				
				// Authorize
				//TODO:LoginManager.GetInstance().AuthorizeOperation(userId,VLF.CLS.Def.Enums.OperationType.Command,Convert.ToInt32(commandID));

				DataSet dsResult = null;
				using( SystemConfig dbSystem = new SystemConfig(LoginManager.GetConnnectionString(userId)))
				{					
					dsResult = dbSystem.GetUserTasks(userId);
               
					if(ASIErrorCheck.IsAnyRecord(dsResult))
						xml = dsResult.GetXml() ;
				}

				return (int)InterfaceError.NoError ;
			}
			catch( Exception Ex )
			{
            LogException("<< GetUserTasks: uId={0}, EXC={1} )", userId, Ex.Message);
				return (int)ASIErrorCheck.CheckError( Ex ) ;
			}			
		}
		#endregion

      # region Map info

      // added 26/02/2007 - Max
      /// <summary>
      ///            Resolve street address based on lat - lon
      /// </summary>
      /// <param name="userId">User Id</param>
      /// <param name="SID">Security Id</param>
      /// <param name="organizationId">Organization Id</param>
      /// <param name="latitude">Latitude</param>
      /// <param name="longitude">Longitude</param>
      /// <param name="streetAddress">Street address</param>
      /// <returns>ASI Error code</returns>
      [WebMethod(Description = "Resolve street address based on latitude and longitude")]
      public int GetStreetAddress(int userId, string SID, int organizationId, 
                                  double latitude, double longitude, ref string streetAddress)
      {
         try
         {
            Log(">> GetStreetAddress: uId={0} ({1},{2})", userId, latitude, longitude);
            
            if (!ValidateSuperUser(userId, SID))
               return Convert.ToInt32(InterfaceError.AuthorizationFailed);

            // Authorization
            //LoginManager.GetInstance().AuthorizeWebMethod(userId, MethodInfo.GetCurrentMethod().Name, MethodInfo.GetCurrentMethod().ReflectedType.Name);

            // get map engine info
            SystemConfig sysConfig = new SystemConfig(LoginManager.GetConnnectionString(userId));
            DataSet dsMapEngineInfo = sysConfig.GetUserGeoCodeEngineInfo(userId);

            ClientMapProxy geoMap = new ClientMapProxy(
               new MapEngine<GeoCodeType>[] { new MapEngine<GeoCodeType>(
                  (GeoCodeType)dsMapEngineInfo.Tables[0].Rows[0]["GeoCodeId"], //GeoCodeType.MapPoint, 
                  dsMapEngineInfo.Tables[0].Rows[0]["Path"].ToString()) });

            streetAddress = geoMap.GetStreetAddress(new GeoPoint(latitude, longitude));

            return (int)InterfaceError.NoError;
         }
         catch (Exception Ex)
         {
            LogException("<< GetStreetAddress: uId={0}, EXC={1}", userId, Ex.Message);
            return (int)ASIErrorCheck.CheckError(Ex);
         }
      }

      /// <summary>
      ///      Resolve street addresses in batch based on lat - lon arrays - LSD Proxy
      /// </summary>
      /// <param name="userId">User Id</param>
      /// <param name="SID">Security Id</param>
      /// <param name="organizationId">Organization Id</param>
      /// <param name="latitude">Latitude</param>
      /// <param name="longitude">Longitude</param>
      /// <param name="streetAddress">Street address</param>
      /// <returns>ASI Error code</returns>
      [WebMethod(Description = "Resolve street address based on latitude and longitude -  LSD Proxy")]
      public int GetStreetAddressesLSD(int userId, string SID, int organizationId, 
                                       double[] lat, double[] lot, ref string[] streetAddresses)
      {
          try
          {
             Log(">> GetStreetAddressesLSD: uId={0} ({1},{2})", userId, lat, lot);

             if (!ValidateSuperUser(userId, SID))
                return Convert.ToInt32(InterfaceError.AuthorizationFailed);


              // Authorization
              //LoginManager.GetInstance().AuthorizeWebMethod(userId, MethodInfo.GetCurrentMethod().Name, MethodInfo.GetCurrentMethod().ReflectedType.Name);

              // get map engine info
              SystemConfig sysConfig = new SystemConfig(LoginManager.GetConnnectionString(userId));
              DataSet dsMapEngineInfo = sysConfig.GetUserGeoCodeEngineInfo(userId);

              ClientMapProxy geoMap = new ClientMapProxy(
                 new MapEngine<GeoCodeType>[] { new MapEngine<GeoCodeType>(
                  GeoCodeType.LSD, 
                  dsMapEngineInfo.Tables[0].Rows[0]["Path"].ToString()) });

              streetAddresses = geoMap.GetStreetAddresses(lat, lot);

              return (int)InterfaceError.NoError;
          }
          catch (Exception Ex)
          {
             LogException("<< GetStreetAddressesLSD: uId={0}, EXC={1}", userId, Ex.Message);
             return (int)ASIErrorCheck.CheckError(Ex);
          }
      }



      /// Resolve street addresses in batch based on lat - lon arrays
      /// </summary>
      /// <param name="userId">User Id</param>
      /// <param name="SID">Security Id</param>
      /// <param name="organizationId">Organization Id</param>
      /// <param name="latitude">Latitude</param>
      /// <param name="longitude">Longitude</param>
      /// <param name="streetAddress">Street address</param>
      /// <returns>ASI Error code</returns>
      [WebMethod(Description = "Resolve street address based on latitude and longitude")]
      public int GetStreetAddresses(int userId, string SID, int organizationId, 
                                    double[] lat, double[] lot, ref string[] streetAddresses)
      {
          try
          {
             Log(">> GetStreetAddresses: uId={0}, orgId={1}", organizationId);

             if (!ValidateSuperUser(userId, SID))
                return Convert.ToInt32(InterfaceError.AuthorizationFailed);

              // Authorization
              //LoginManager.GetInstance().AuthorizeWebMethod(userId, MethodInfo.GetCurrentMethod().Name, MethodInfo.GetCurrentMethod().ReflectedType.Name);

              // get map engine info
              SystemConfig sysConfig = new SystemConfig(LoginManager.GetConnnectionString(userId));
              DataSet dsMapEngineInfo = sysConfig.GetUserGeoCodeEngineInfo(userId);

              ClientMapProxy geoMap = new ClientMapProxy(
                                      new MapEngine<GeoCodeType>[] { new MapEngine<GeoCodeType>(
                                       (GeoCodeType)dsMapEngineInfo.Tables[0].Rows[0]["GeoCodeId"], //GeoCodeType.MapPoint, 
                                       dsMapEngineInfo.Tables[0].Rows[0]["Path"].ToString()) });

              streetAddresses = geoMap.GetStreetAddresses(lat, lot);

              return (int)InterfaceError.NoError;
          }
          catch (Exception Ex)
          {
             LogException("<< GetStreetAddresses: uId={0}, EXC={1}", userId, Ex.Message);
             return (int)ASIErrorCheck.CheckError(Ex);
          }
      }


      /// Resolve street addresses in batch based on lat - lon arrays
      /// </summary>
      /// <param name="userId">User Id</param>
      /// <param name="SID">Security Id</param>
            /// <param name="latitude">Latitude</param>
      /// <param name="longitude">Longitude</param>
      /// <param name="streetAddress">Street address</param>
      /// <returns>ASI Error code</returns>
      [WebMethod(Description = "Resolve latitude and longitude by address")]
      public int GetLatLongByAddress(int userId, string SID,
                                    string streetAddresses, ref string xmlAddressMatches)
      {
          try
          {
              Log(">> GetLatLongByAddress: uId={0}, streetAddresses={1}", streetAddresses);

              if (!ValidateSuperUser(userId, SID))
                  return Convert.ToInt32(InterfaceError.AuthorizationFailed);

              // Authorization
              //LoginManager.GetInstance().AuthorizeWebMethod(userId, MethodInfo.GetCurrentMethod().Name, MethodInfo.GetCurrentMethod().ReflectedType.Name);

              // get map engine info
              SystemConfig sysConfig = new SystemConfig(LoginManager.GetConnnectionString(userId));
              DataSet dsMapEngineInfo = sysConfig.GetUserGeoCodeEngineInfo(userId);

              ClientMapProxy geoMap = new ClientMapProxy(
                                      new MapEngine<GeoCodeType>[] { new MapEngine<GeoCodeType>(
                                       (GeoCodeType)dsMapEngineInfo.Tables[0].Rows[0]["GeoCodeId"], //GeoCodeType.MapPoint, 
                                       dsMapEngineInfo.Tables[0].Rows[0]["Path"].ToString()) });

              xmlAddressMatches = geoMap.GetAddressMatches(streetAddresses);

              return (int)InterfaceError.NoError;
          }
          catch (Exception Ex)
          {
              LogException("<< GetLatLongByAddress: uId={0}, EXC={1}", userId, Ex.Message);
              return (int)ASIErrorCheck.CheckError(Ex);
          }
      }

      // added 1/03/2007 - Max
      /// <summary>
      /// Resolve street address based on lat - lon
      /// </summary>
      /// <param name="userId">User Id</param>
      /// <param name="SID">Security Id</param>
      /// <param name="organizationId">Organization Id</param>
      /// <param name="xmlData">XML data string containing positions</param>
      /// Example: <Positions><Pos id=\"1\"><Lat>40.0</Lat><Lon>-74.3</Lon></Pos><Pos id=\"2\"><Lat>40.2</Lat><Lon>-74.6</Lon></Pos></Positions>
      /// <param name="xmlResult">XML string with address node added</param>
      /// Example: <Positions><Pos id=\"1\"><Lat>40.0</Lat><Lon>-74.3</Lon><Addr>Street address 1</Addr></Pos><Pos id=\"2\"><Lat>40.2</Lat><Lon>-74.6</Lon><Addr>Street address 2</Addr></Pos></Positions>
      /// <returns>ASI Error code</returns>
      [WebMethod(Description = "Resolve a set of street addresses based on latitude and longitude")]
      public int GetStreetAddressesXML(int userId, string SID, int organizationId, string xmlData, ref string xmlResult)
      {
         double latitude = 0.0, longitude = 0.0;
         string streetAddress = "", positionId = "";
         try
         {
            Log(">> GetStreetAddressesXML: uId={0}, data={1}", userId, xmlData);

            if (!ValidateSuperUser(userId, SID))
               return Convert.ToInt32(InterfaceError.AuthorizationFailed);


            // Authorization
            //LoginManager.GetInstance().AuthorizeWebMethod(userId, MethodInfo.GetCurrentMethod().Name, MethodInfo.GetCurrentMethod().ReflectedType.Name);

            // get map engine info
            SystemConfig sysConfig = new SystemConfig(LoginManager.GetConnnectionString(userId));
            DataSet dsMapEngineInfo = sysConfig.GetUserGeoCodeEngineInfo(userId);

            ClientMapProxy geoMap = new ClientMapProxy(
               new MapEngine<GeoCodeType>[] { new MapEngine<GeoCodeType>(
                  (GeoCodeType)dsMapEngineInfo.Tables[0].Rows[0]["GeoCodeId"], //GeoCodeType.MapPoint, 
                  dsMapEngineInfo.Tables[0].Rows[0]["Path"].ToString()) });

            // data xml
            XmlDocument xData = new XmlDocument();
            xData.LoadXml(xmlData);
            // get positions list
            XmlNodeList positionList = xData.DocumentElement.SelectNodes("Pos");

            // result xml
            XmlDocument xResult = new XmlDocument();
            // add the root
            XmlNode resultRoot = xResult.CreateNode("element", "Positions", "");
            xResult.AppendChild(resultRoot);

            for (int i = 0; i < positionList.Count; i++)
            {
               // get position node
               XmlNode position = positionList.Item(i);
               // get position id node
               positionId = position.Attributes.GetNamedItem("id").Value;
               // get lat value
               latitude = Convert.ToDouble(position.FirstChild.InnerText);
               // get long value
               longitude = Convert.ToDouble(position.LastChild.InnerText);
               // get address
               streetAddress = geoMap.GetStreetAddress(new GeoPoint(latitude, longitude));

               // result - create position node
               XmlNode posResult = xResult.CreateNode("element", "Pos", "");
               // add the same id attr.
               XmlAttribute posIdAttr = xResult.CreateAttribute("id");
               posIdAttr.InnerText = positionId;
               posResult.Attributes.Prepend(posIdAttr);
               // create address node
               XmlNode addressNode = xResult.CreateNode("element", "Addr", "");
               addressNode.InnerText = streetAddress;
               // add address node
               posResult.AppendChild(addressNode);
               // add position to the root
               resultRoot.AppendChild(posResult);
            }

            // save the xml doc to string
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            xResult.Save(new StringWriter(sb));
            xmlResult = sb.ToString();

            return (int)InterfaceError.NoError;
         }
         catch (Exception Ex)
         {
            LogException("<< GetStreetAddressesXML: uId={0}, EXC={1}", userId, Ex.Message);
            return (int)ASIErrorCheck.CheckError(Ex);
         }
      }



      /// Resolve street addresses in batch based on lat - lon arrays - LSD Proxy
      /// </summary>
      /// <param name="userId">User Id</param>
      /// <param name="SID">Security Id</param>
      /// <param name="organizationId">Organization Id</param>
      /// <param name="latitude">Latitude</param>
      /// <param name="longitude">Longitude</param>
      /// <param name="streetAddress">Street address</param>
      /// <returns>ASI Error code</returns>
      [WebMethod(Description = "Resolve street address based on latitude and longitude -  LSD Proxy")]
      public int GetSpecialAddressLSD(int userId, string SID, int organizationId, double lat, double lot, ref string streetAddresses)
      {
          try
          {
              Log(">> GetSpecialAddressLSD: uId={0} ({1},{2})", userId, lat, lot);

              // Authenticate 
              LoginManager.GetInstance().SecurityCheck(userId, SID);
              VLF.MAP.LSDProxy geomap = new VLF.MAP.LSDProxy("");
              streetAddresses = geomap.GetSpecialAddress(lat,lot);

              return (int)InterfaceError.NoError;
          }
          catch (Exception Ex)
          {
             LogException("<< GetSpecialAddressLSD: uId={0}, EXC={1}", userId, Ex.Message);
             return (int)ASIErrorCheck.CheckError(Ex);
          }
      }



      [WebMethod(Description = "Delete Session")]
      public int DeleteSession(int userId, string SID, int boxID,  short protocolType)
      {
         try
         {
            Log(">> DeleteSession( uId={0}, boxID={1}, protocolType={2})", userId, boxID,  protocolType);

            if (!ValidateUserBox(userId, SID, boxID))
               return Convert.ToInt32(InterfaceError.AuthorizationFailed);

            // Authorize
            // LoginManager.GetInstance().AuthorizeOperation(userId, VLF.CLS.Def.Enums.OperationType.Command, Convert.ToInt32(commandID));

            CommSLS commSLS = new CommSLS();
            return Convert.ToInt32(ASIErrorCheck.CheckError(commSLS.DeleteSession(boxID,  protocolType), "Error occured in SLS module", "SLS", "See SLS log StackTrace"));
         }
         catch (Exception Ex)
         {
            LogException("<< DeleteSession: uId={0}, EXC={1}", userId, Ex.Message);
            return (int)ASIErrorCheck.CheckError(Ex);
         }
      }

      # endregion
      #region Command to multiple boxes

       [WebMethod(Description = "Sends a command to the Multiple Vehicles. This method call should be followed by consequent calls of GetCommandStatus() to determine current execution status of the command")]
       public int SendCommandToMultipleVehicles(int userId, string SID, DateTime time, int[] boxID, short commandID, string paramList,
                       bool scheduled, ref short[] protocolType, ref short[] commMode, ref bool[] cmdSent, ref Int64[] sessionTimeOut, ref short[] results)
        {
            try
            {
                Log(">> SendCommandToMultipleVehicles(uId={0}, time={1}, boxID#={2}, commandID={3}, paramList={4}, commMode={5}, protocolType={6})",
                           userId, time, boxID.Length  , commandID, paramList, commMode.ToString(), protocolType.ToString());

                // Authenticate 
                LoginManager.GetInstance().SecurityCheck(userId, SID);


                // Authorize
                if (commandID == Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.Output))
                    LoginManager.GetInstance().AuthorizeOperation(userId, VLF.CLS.Def.Enums.OperationType.Output, Convert.ToInt32(commandID));
                else
                    LoginManager.GetInstance().AuthorizeOperation(userId, VLF.CLS.Def.Enums.OperationType.Command, Convert.ToInt32(commandID));

                CommSLS commSLS = new CommSLS();

                commSLS.SendCommand(userId, time, boxID, commandID, paramList, false, ref protocolType, ref commMode, ref cmdSent, ref sessionTimeOut,ref results);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< SendCommandToMultipleVehicles : uId={0}, CMD={2}, boxId={3} EXC={1}", userId, Ex.Message, commandID, boxID.ToString());
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        [WebMethod(Description = "Sends a command to the Multiple Vehicles. This method call should be followed by consequent calls of GetCommandStatus() to determine current execution status of the command")]
        public int GetCommandStatusFromMultipleVehicles(int userId, string SID, int[] boxId, short[] protocolType, ref int[] cmdStatus)
        {
            try
            {
                Log(">> GetCommandStatusFromMultipleVehicles(uId={0}, boxId#={1}, protocolType#={2})", userId, boxId.Length, protocolType.Length);

                // Authenticate 
                LoginManager.GetInstance().SecurityCheck(userId, SID);


                // Authorize
                CommSLS commSLS = new CommSLS();

                commSLS.GetCommandStatus(boxId, protocolType, userId, ref cmdStatus);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetCommandStatusFromMultipleVehicles : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }



      #endregion

  }

   /// <summary>
   /// Method Id for authorization - table vlfWebMethods
   /// </summary>
   //enum LocationWebMethod
   //{
   //   GetStreetAddress = 217,
   //   GetStreetAddressesXML = 218
   //}
}
