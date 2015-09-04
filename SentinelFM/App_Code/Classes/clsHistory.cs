using System;
using System.Data ;
using VLF.MAP;
using VLF.CLS.Interfaces;
using System.Collections;  
namespace SentinelFM
{
	/// <summary>
	/// Summary description for clsHistory.
	/// </summary>
	public class clsHistory
	{


		
		private Int64 vehicleId=0;
		public Int64 VehicleId
		{
			get{return vehicleId;}
			set{vehicleId = value;}
		}

		private Int32 reportStopDuration=0;
		public Int32 ReportstopDuration
		{
			get{return reportStopDuration;}
			set{reportStopDuration = value;}
		}

		private string imgPath="";
		public string ImgPath
		{
			get{return imgPath;}
			set{imgPath = value;}
		}

		private Int64 fleetId=0;
		public Int64 FleetId
		{
			get{return fleetId;}
			set{fleetId = value;}
		}

        private String multiFleetIDs = string.Empty;
        public String MultiFleetIDs
        {
            get { return multiFleetIDs; }
            set {multiFleetIDs = value;}
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


      private string toDate = "";
      public string ToDate
      {
         get { return toDate; }
         set { toDate = value; }
      }



      private string fromDateTime = "";
      public string FromDateTime
      {
         get { return fromDateTime; }
         set { fromDateTime = value; }
      }

		private string toDateTime="";
		public string  ToDateTime
		{
         get { return toDateTime; }
         set { toDateTime = value; }
		}



        private string sqlTopMsg = "";
        public string SqlTopMsg
        {
            get { return sqlTopMsg; }
            set { sqlTopMsg = value; }
        }


        private string msgList = "";
        public string MsgList
        {
            get { return msgList; }
            set { msgList = value; }
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


		private bool showBreadCrumb=false;
		public bool  ShowBreadCrumb
		{
			get{return showBreadCrumb;}
			set{showBreadCrumb = value;}
		}


		private bool showStopSqNum=false;
		public bool  ShowStopSqNum
		{
			get{return showStopSqNum;}
			set{showStopSqNum = value;}
		}

		private DataSet dsHistoryInfo;
		public DataSet DsHistoryInfo
		{
			get{return dsHistoryInfo;}
			set{dsHistoryInfo = value;}
		}

        private DataSet dsReeferPretrip;
        public DataSet DsReeferPretrip
        {
            get { return dsReeferPretrip; }
            set { dsReeferPretrip = value; }
        }

        private DataSet dsReeferImpact;
        public DataSet DsReeferImpact
        {
            get { return dsReeferImpact; }
            set { dsReeferImpact = value; }
        }


		private string carLatitude="";
		public string  CarLatitude
		{
			get{return carLatitude;}
			set{carLatitude = value;}
		}


		private string carLongitude="";
		public string  CarLongitude
		{
			get{return carLongitude;}
			set{carLongitude = value;}
		}


		private string originDateTime="";
		public string  OriginDateTime
		{
			get{return originDateTime;}
			set{originDateTime = value;}
		}
		


		private string mapCenterLongitude="";
		public string  MapCenterLongitude
		{
			get{return mapCenterLongitude;}
			set{mapCenterLongitude = value;}
		}


		private string mapCenterLatitude="";
		public string  MapCenterLatitude
		{
			get{return mapCenterLatitude;}
			set{mapCenterLatitude = value;}
		}

		private string mapScale="";
		public string  MapScale
		{
			get{return mapScale;}
			set{mapScale = value;}
		}

		private string imageDistance = "";
		public string  ImageDistance
		{
			get{return imageDistance;}
			set{imageDistance = value;}
		}

		private string drawAllVehicles="";
		public string  DrawAllVehicles
		{
			get{return drawAllVehicles;}
			set{drawAllVehicles = value;}
		}

		private string carSpeed="";
		public string  CarSpeed
		{
			get{return carSpeed;}
			set{carSpeed = value;}
		}

		private string heading="";
		public string  Heading
		{
			get{return heading;}
			set{heading = value;}
		}
		


		private bool showToolTip=true;
		public bool  ShowToolTip
		{
			get{return showToolTip;}
			set{showToolTip = value;}
		}


		private bool showStops=false;
		public bool  ShowStops
		{
			get{return showStops;}
			set{showStops = value;}
		}



        private bool showIdle = false;
        public bool ShowIdle
        {
            get { return showIdle; }
            set { showIdle = value; }
        }


        private bool showStopsAndIdle = false;
        public bool ShowStopsAndIdle
        {
            get { return showStopsAndIdle; }
            set { showStopsAndIdle = value; }
        }


        private bool showTrips = false;
        public bool ShowTrips
        {
            get { return showTrips; }
            set { showTrips = value; }
        }

		private double stopDurationVal=0;
		public double  StopDurationVal
		{
			get{return stopDurationVal;}
			set{stopDurationVal = value;}
		}


		private int stopIndex=0;
		public int  StopIndex
		{
			get{return stopIndex;}
			set{stopIndex = value;}
		}


		private string stopStatus="";
		public string  StopStatus
		{
			get{return stopStatus;}
			set{stopStatus = value;}
		}


		private string stopDate="";
		public string  StopDate
		{
			get{return stopDate;}
			set{stopDate = value;}
		}


		private string stopAddress="";
		public string  StopAddress
		{
			get{return stopAddress;}
			set{stopAddress = value;}
		}


		private string stopDuration="";
		public string  StopDuration
		{
			get{return stopDuration;}
			set{stopDuration = value;}
		}

        private Int16 dclId = 0;
        public Int16 DclId
        {
            get { return dclId; }
            set { dclId = value; }
        }

		private string carHistoryDate="";
		public string  CarHistoryDate
		{
			get{return carHistoryDate;}
			set{carHistoryDate = value;}
		}


		private string carMessageType="";
		public string  CarMessageType
		{
			get{return carMessageType;}
			set{carMessageType = value;}
		}


		private string carAddress="";
		public string  CarAddress
		{
			get{return carAddress;}
			set{carAddress = value;}
		}


		

		private string iconTypeName="";
		public string  IconTypeName
		{
			get{return iconTypeName;}
			set{iconTypeName = value;}
		}


		
		private DataSet dsUnAssVehicle=null;
		public DataSet DsUnAssVehicle
		{
			get{return dsUnAssVehicle;}
			set{dsUnAssVehicle = value;}
		}

        private DataSet dsUnAssOHVehicle = null;
        public DataSet DsUnAssOHVehicle
        {
            get { return dsUnAssOHVehicle; }
            set { dsUnAssOHVehicle = value; }
        }


		private DataSet dsAssVehicle=null;
		public DataSet DsAssVehicle
		{
			get{return dsAssVehicle;}
			set{dsAssVehicle = value;}
		}

        private DataSet dsAssOHVehicle = null;
        public DataSet DsAssOHVehicle
        {
            get { return dsAssOHVehicle; }
            set { dsAssOHVehicle = value; }
        }


        private DataSet dsUnAssUsers = null;
        public DataSet DsUnAssUsers
        {
            get { return dsUnAssUsers; }
            set { dsUnAssUsers = value; }
        }

        private DataSet dsUnAssOHUsers = null;
        public DataSet DsUnAssOHUsers
        {
            get { return dsUnAssOHUsers; }
            set { dsUnAssOHUsers = value; }
        }


        private DataSet dsAssUsers = null;
        public DataSet DsAssUsers
        {
            get { return dsAssUsers; }
            set { dsAssUsers = value; }
        }

        private DataSet dsAssOHUsers = null;
        public DataSet DsAssOHUsers
        {
            get { return dsAssOHUsers; }
            set { dsAssOHUsers = value; }
        }


         private DataSet dsHistoryVehicles = null;
         public DataSet DsHistoryVehicles
           {
              get { return dsHistoryVehicles; }
              set { dsHistoryVehicles = value; }
           }

         private string dsAssSensors = null;
         public string DsAssSensors
         {
             get { return dsAssSensors; }
             set { dsAssSensors = value; }
         }

     
         private string dsAssEvents = null;
         public string DsAssEvents
         {
             get { return dsAssEvents; }
             set { dsAssEvents = value; }
         }

       
         private string dsAssViolations = null;
         public string DsAssViolations
         {
             get { return dsAssViolations; }
             set { dsAssViolations = value; }
         }
        

        private string mapSouthWestCornerLongitude = "";
        public string MapSouthWestCornerLongitude
        {
            get { return mapSouthWestCornerLongitude; }
            set { mapSouthWestCornerLongitude = value; }
        }

        private string mapSouthWestCornerLatitude = "";
        public string MapSouthWestCornerLatitude
        {
            get { return mapSouthWestCornerLatitude; }
            set { mapSouthWestCornerLatitude = value; }
        }

        private string mapNorthEastCornerLongitude = "";
        public string MapNorthEastCornerLongitude
        {
            get { return mapNorthEastCornerLongitude; }
            set { mapNorthEastCornerLongitude = value; }
        }

        private string mapNorthEastCornerLatitude = "";
        public string MapNorthEastCornerLatitude
        {
            get { return mapNorthEastCornerLatitude; }
            set { mapNorthEastCornerLatitude = value; }
        }


        private bool redirectFromMapScreen = false;
       public bool RedirectFromMapScreen
        {
           get { return redirectFromMapScreen; }
           set { redirectFromMapScreen = value; }
        }



        private int screenWidth=0;
        public int ScreenWidth
        {
           get { return screenWidth; }
           set { screenWidth = value; }
        }

        private Int32 dgVisibleRows = 5;
        public Int32 DgVisibleRows
        {
           get { return dgVisibleRows; }
           set { dgVisibleRows = value; }
        }


        private Int32 dgItemsPerPage = 9999;
        public Int32 DgItemsPerPage
        {
           get { return dgItemsPerPage; }
           set { dgItemsPerPage = value; }
        }

      private bool mapResize = false;
      public bool MapResize
      {
         get { return mapResize; }
         set { mapResize = value; }
      }


      private DataSet dsSelectedData = null;
      public DataSet DsSelectedData
      {
         get { return dsSelectedData; }
         set { dsSelectedData = value; }
      }



      private bool animated = false;
      public bool Animated
      {
         get { return animated; }
         set { animated = value; }
      }



      private Int32 mapAnimationSpeed = 0;
      public Int32 MapAnimationSpeed
      {
         get { return mapAnimationSpeed; }
         set { mapAnimationSpeed = value; }
      }


      private Int32 mapAnimationHistoryInterval = 0;
      public Int32 MapAnimationHistoryInterval
      {
         get { return mapAnimationHistoryInterval; }
         set { mapAnimationHistoryInterval = value; }
      }


      private string licensePlate = "";
      public string LicensePlate
      {
         get { return licensePlate; }
         set { licensePlate = value; }
      }

      private bool mapSearch = false;
      public bool MapSearch
      {
          get { return mapSearch; }
          set { mapSearch = value; }
      }


      private DataSet dsMaintenance = null;
      public DataSet DsMaintenance
      {
          get { return dsMaintenance; }
          set { dsMaintenance = value; }
      }



      private DataSet dsDueServices = null;
      public DataSet DsDueServices
      {
          get { return dsDueServices; }
          set { dsDueServices = value; }
      }

      private DataSet dsVehicleDueServices = null;
      public DataSet DsVehicleDueServices
      {
          get { return dsVehicleDueServices; }
          set { dsVehicleDueServices = value; }
      }


      private DataSet dsMaintenanceHistory = null;
      public DataSet DsMaintenanceHistory
      {
          get { return dsMaintenanceHistory; }
          set { dsMaintenanceHistory = value; }
      }

      private string address = "";
      public string Address
      {
          get { return address; }
          set { address = value; }
      }


      private DataSet dsAlarms = null;
      public DataSet DsAlarms
      {
          get { return dsAlarms; }
          set { dsAlarms = value; }
      }

      private DataSet dsMessages = null;
      public DataSet DsMessages
      {
          get { return dsMessages; }
          set { dsMessages = value; }
      }


      private DataSet dsActivitySummary = null;
      public DataSet DsActivitySummary
      {
          get { return dsActivitySummary; }
          set { dsActivitySummary = value; }
      }


      private DataSet dsActivitySummaryPerVehicle = null;
      public DataSet DsActivitySummaryPerVehicle
      {
          get { return dsActivitySummaryPerVehicle; }
          set { dsActivitySummaryPerVehicle = value; }
      }

     private DataSet dsNotifications = null;
     public DataSet DsNotifications
      {
          get { return dsNotifications; }
          set { dsNotifications = value; }
      }


     private Int16 tripSensor = 3;
     public Int16 TripSensor
     {
         get { return tripSensor; }
         set { tripSensor = value; }
     }

		public clsHistory()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}
}
