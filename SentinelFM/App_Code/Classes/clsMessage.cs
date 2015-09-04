using System;
using System.Data ;

namespace SentinelFM
{
	/// <summary>
	/// Summary description for clsMessage.
	/// </summary>
	public class clsMessage
	{

		private Int16 folderTypeId=0;
		public Int16 FolderTypeId
		{
			get{return folderTypeId;}
			set{folderTypeId = value;}
		}


		private Int64 vehicleId=0;
		public Int64 VehicleId
		{
			get{return vehicleId;}
			set{vehicleId = value;}
		}



		private Int32 fleetId=0;
		public Int32 FleetId
		{
			get{return fleetId;}
			set{fleetId = value;}
		}


		private string message="";
		public string  Message
		{
			get{return message;}
			set{message = value;}
		}


		private string response="";
		public string  Response
		{
			get{return response;}
			set{response = value;}
		}

		private string vehicleName="";
		public string  VehicleName
		{
			get{return vehicleName;}
			set{vehicleName = value;}
		}


		private string fleetName="";
		public string  FleetName
		{
			get{return fleetName;}
			set{fleetName = value;}
		}

		private string fromHours="";
		public string FromHours
		{
			get{return fromHours;}
			set{fromHours = value;}
		}



		private string toHours="";
		public string ToHours
		{
			get{return toHours;}
			set{toHours = value;}
		}


		private string fromDate="";
		public string  FromDate
		{
			get{return fromDate;}
			set{fromDate = value;}
		}


		private string toDate="";
		public string  ToDate
		{
			get{return toDate;}
			set{toDate = value;}
		}


		private DataSet dsMessages;
		public DataSet DsMessages
		{
			get{return dsMessages;}
			set{dsMessages = value;}
		}


      private DataSet dsHistoryMessages;
      public DataSet DsHistoryMessages
      {
         get { return dsHistoryMessages; }
         set { dsHistoryMessages = value; }
      }

      private DataSet dsHistoryAlarms;
      public DataSet DsHistoryAlarms
      {
         get { return dsHistoryAlarms; }
         set { dsHistoryAlarms = value; }
      }

		private DataSet dsVehicles;
		public DataSet DsVehicles
		{
			get{return dsVehicles;}
			set{dsVehicles = value;}
		}

		private bool timerStatus=false;
		public bool TimerStatus
		{
			get{return timerStatus;}
			set{timerStatus = value;}
		}


		private bool messageSent=false;
		public bool MessageSent
		{
			get{return messageSent;}
			set{messageSent = value;}
		}

        private bool messageQueued = false;
        public bool MessageQueued
        {
            get { return messageQueued; }
            set { messageQueued = value; }
        }


		private int boxId;

		public int BoxId
		{
			get{return boxId;}
			set{boxId = value;}
		}


		private int messageDirectionId;
		public int MessageDirectionId
		{
			get{return messageDirectionId;}
			set{messageDirectionId = value;}
		}


		private string messageDirection="";
		public string  MessageDirection
		{
			get{return messageDirection;}
			set{messageDirection = value;}
		}


		private DataTable  dtSendMessageFails =new DataTable() ;
		public DataTable DtSendMessageFails
		{
			get{return dtSendMessageFails;}
			set{dtSendMessageFails = value;}
		}



		private DataTable  dtSendMessageBoxes =new DataTable() ;
		public DataTable DtSendMessageBoxes
		{
			get{return dtSendMessageBoxes;}
			set{dtSendMessageBoxes = value;}
		}

      private DataSet dsScheduledTasks = null;
      public DataSet DsScheduledTasks
      {
         get { return dsScheduledTasks; }
         set { dsScheduledTasks = value; }
      }

      private DataSet dsDriverMessages = null;
      public DataSet DsDriverMessages
      {
          get { return dsDriverMessages; }
          set { dsDriverMessages = value; }
      }


      private DataSet dsGarminMessages = null;
      public DataSet DsGarminMessages
      {
          get { return dsGarminMessages; }
          set { dsGarminMessages = value; }
      }


      private DataSet dsGarminLocations = null;
      public DataSet DsGarminLocations
      {
          get { return dsGarminLocations; }
          set { dsGarminLocations = value; }
      }


      private DataSet dsGarminVehicles = null;
      public DataSet DsGarminVehicles
      {
          get { return dsGarminVehicles; }
          set { dsGarminVehicles = value; }
      }

      private string msgsCheckSum = "";
      public string MsgsCheckSum
      {
          get { return msgsCheckSum; }
          set { msgsCheckSum = value; }
      }

      private string message_data = "";
      public string MESSAGE_DATA
      {
          get { return message_data; }
          set { message_data = value; }
      }

      public clsMessage()
		{
			
			
			//------------Create Columns for ..
			//------------Vehicles send Message
			DataColumn colBoxId = new DataColumn("BoxId",Type.GetType("System.Int64"));
			dtSendMessageBoxes.Columns.Add(colBoxId);
			DataColumn colVehicleDesc = new DataColumn("VehicleDesc",Type.GetType("System.String"));
			dtSendMessageBoxes.Columns.Add(colVehicleDesc);
			DataColumn colUpdated = new DataColumn("Updated",Type.GetType("System.Int32"));
			dtSendMessageBoxes.Columns.Add(colUpdated);
			DataColumn colProtocolId = new DataColumn("ProtocolId",Type.GetType("System.Int16"));
			dtSendMessageBoxes.Columns.Add(colProtocolId);

            DataColumn colComModeId = new DataColumn("ComModeId", Type.GetType("System.Int16"));
            dtSendMessageBoxes.Columns.Add(colComModeId);
          
			//-------------------------

			
			//Create Columns for 
			//Vehicle send Message TimeOuts
			DataColumn colVehicleDesc1 = new DataColumn("VehicleDesc",Type.GetType("System.String"));
			dtSendMessageFails.Columns.Add(colVehicleDesc1);
		}
	}
}
