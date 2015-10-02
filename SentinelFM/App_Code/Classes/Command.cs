using System;
using System.Data; 
using VLF.CLS.Interfaces;
using System.Collections.Generic;
  
namespace SentinelFM
{

//	public enum CommandStatus
//	{
//		Idle = 0,
//		Sent = 1,
//		Ack = 2,
//		Timeout = 3,
//		Canceled = 4,
//		Queued = 5
//	}
	
	/// <summary>
	/// Summary description for Command.
	/// </summary>
	public class Command
	{
		private DateTime timeSent ;
		private CommandStatus status ;
		private string commandName ;
		private int progress ;
		private bool updatePositionSend=false;
		private bool geoZoneSync=false;
		private short protocolTypeId=0;
		private int boxId=0;
		private DataSet dsSensorsTraceStates=new DataSet();
		private DataSet dsProtocolTypes=new DataSet();
		private DataTable  dtUpdatePositionFails =new DataTable() ;
		private string selectedVehicleLicensePlate="";
		private bool dualComm=false;
		private string commandParams="";
		private short commandId=0;
		private short commModeId=0;
		private Int64 schPeriod=0;
		private Int32 schInterval=0;
		private bool schCommand=false;
		private string sendCommandMessage="";


		private Int64 getCommandStatusRefreshFreq=1000;
		public Int64 GetCommandStatusRefreshFreq
		{
			get{return getCommandStatusRefreshFreq;}
			set{getCommandStatusRefreshFreq = value;}
		}


		public string SendCommandMessage
		{
			get{return sendCommandMessage;}
			set{sendCommandMessage = value;}
		}


		public bool SchCommand
		{
			get{return schCommand;}
			set{schCommand = value;}
		}

		public Int64 SchPeriod
		{
			get{return schPeriod;}
			set{schPeriod = value;}
		}


		public Int32 SchInterval
		{
			get{return schInterval;}
			set{schInterval = value;}
		}


		public short CommModeId
		{
			get{return commModeId;}
			set{commModeId = value;}
		}


		public short CommandId
		{
			get{return commandId;}
			set{commandId = value;}
		}

		public string CommandParams
		{
			get{return commandParams;}
			set{commandParams = value;}
		}

		public bool DualComm
		{
			get{return dualComm;}
			set{dualComm = value;}
		}

		public string SelectedVehicleLicensePlate
		{
			get{return selectedVehicleLicensePlate;}
			set{selectedVehicleLicensePlate = value;}
		}

		public DataSet DsSensorsTraceStates
		{
			get{return dsSensorsTraceStates;}
			set{dsSensorsTraceStates = value;}
		}


		public DataSet DsProtocolTypes
		{
			get{return dsProtocolTypes;}
			set{dsProtocolTypes = value;}
		}

		public int BoxId
		{
			get{return boxId;}
			set{boxId = value;}
		}

		public short ProtocolTypeId
		{
			get{return protocolTypeId;}
			set{protocolTypeId = value;}
		}

		public int Progress
		{
			set
			{ 

				if( value < 0 )
					progress = 0 ;
				else if( value > 100 )
					progress = 0 ; // restart
				else
					progress = value; 
			}
			get{ return progress ; }
		}
		
		public bool GeoZoneSync
		{
			get{return geoZoneSync;}
			set{geoZoneSync = value;}
		}

		public CommandStatus    Status
		{
			get{return status;}
			set{status = value;}
		}
		public string CommandName
		{
			get{return commandName;}
			set{commandName = value;}
		}
		
		public DateTime TimeSent
		{
			get{return timeSent;}
			set{timeSent = value;}
		}


		public DataTable DtUpdatePositionFails
		{
			get{return dtUpdatePositionFails;}
			set{dtUpdatePositionFails = value;}
		}

      


        private List<int> arrBoxId=new List<int>() ;
        public List<int> ArrBoxId
        {
            get { return arrBoxId; }
            set { arrBoxId = value; }
        }


        private List<short> arrProtocolType=new List<short>() ;
        public List<short> ArrProtocolType
        {
            get { return arrProtocolType; }
            set { arrProtocolType = value; }
        }


        private List<int> arrCmdStatus=new List<int>() ;
        public List<int> ArrCmdStatus
        {
            get { return arrCmdStatus; }
            set { arrCmdStatus = value; }
        }


        private List<string> arrVehicle=new List<string>();
        public List<string> ArrVehicle
        {
            get { return arrVehicle; }
            set { arrVehicle = value; }
        }

		public Command()
		{
			status = CommandStatus.Idle ;	

			//Create SensorsTrace DataSet
			dsSensorsTraceStates.Tables.Add();  
			DataColumn SensorId = new DataColumn("SensorId",Type.GetType("System.Int32"));
			dsSensorsTraceStates.Tables[0].Columns.Add(SensorId);
			DataColumn SensorName = new DataColumn("SensorName",Type.GetType("System.String"));
			dsSensorsTraceStates.Tables[0].Columns.Add(SensorName);
			DataColumn TraceStateId = new DataColumn("TraceStateId",Type.GetType("System.Int16"));
			TraceStateId.DefaultValue=VLF.CLS.Def.Enums.SensorsTraceState.Disable;    
			dsSensorsTraceStates.Tables[0].Columns.Add(TraceStateId);
			DataColumn TraceStateName = new DataColumn("TraceStateName",Type.GetType("System.String"));
			TraceStateName.DefaultValue=VLF.CLS.Def.Enums.SensorsTraceState.Disable   ;
			dsSensorsTraceStates.Tables[0].Columns.Add(TraceStateName);


			//Create Columns for 
			//Vehicle with TimeOut Update Position
			DataColumn colVehicleDesc = new DataColumn("VehicleDesc",Type.GetType("System.String"));
			dtUpdatePositionFails.Columns.Add(colVehicleDesc);
			DataColumn colStatus = new DataColumn("Status",Type.GetType("System.String"));
			dtUpdatePositionFails.Columns.Add(colStatus);
		}

			public bool UpdatePositionSend
		{
			get{return updatePositionSend;}
			set{updatePositionSend = value;}
		}

	}
}

