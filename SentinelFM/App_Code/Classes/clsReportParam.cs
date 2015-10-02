using System;
using System.Data; 
namespace SentinelFM
{
	/// <summary>
	/// Summary description for clsReportParam.
	/// </summary>
	public class clsReportParam
	{
	  private DataSet dsTripReport;
	  private string xmlParams="";
      private string deliveryPeriod = "";
      private string deliveryDate = "";
	  private string fromDate="";
	  private string toDate="";
      private DataSet userReportsDataSet;
      private DataSet userExtendedReportsDataSet;
      private Int32 reportFormat = 1;
	  private string fleetReport="";
	  private string licensePlate="";
	  private string fleetName="";
	  private Int32 fleetId=0;
      private Int32 vehicleId = 0;
      private Int32 reportTypeID ;
      private bool isFleet = false;
      private string reportURL="";
      private Int16 guiId = 0;
      private string reportType = "";
      private string reportAddType = "";
      private Int32  reportActiveTab = 0;
      private Int32 driverId = 0;
      private string landmarkName = "";
      private string organizationHierarchyNodeCode = "";
      private bool organizationHierarchySelected = false;
      private string organizationHierarchyPath = "";
      private string tmpData = "";

      //By devin for report Begin
      private string xmlDOC = "";

      private string myReportDate = "";

      public string MyReportDate
      {
          get { return myReportDate; }
          set { myReportDate = value; }
      }

      public string XmlDOC
      {
          get { return xmlDOC; }
          set { xmlDOC = value; }
      }

      public string TmpData
      {
          get { return tmpData; }
          set { tmpData = value; }
      }

      //End


      public Int32 ReportActiveTab
        {
            get { return reportActiveTab; }
            set { reportActiveTab = value; }
        }


      public string ReportAddType
      {
         get { return reportAddType; }
         set { reportAddType = value; }
      }


      public string ReportType
      {
         get { return reportType; }
         set { reportType = value; }
      }


      public Int16 GuiId
        {
            get { return guiId; }
            set { guiId = value; }
        }
 	
		public string ReportURL
		{
			get{ return reportURL ; }
			set{ reportURL = value ; }
		}

		public DataSet DsTripReport
		{
			get{return dsTripReport;}
			set{dsTripReport = value;}
		}
        public string DeliveryDate
        {
            get
            {
                return deliveryDate;
            }
            set
            {
                deliveryDate = value;
            }
        }
        public string DeliveryPeriod
        {
            get
            {
                return deliveryPeriod;
            }
            set
            {
                deliveryPeriod=value;
            }
        }

		public string XmlParams
		{
			get{return xmlParams;}
			set{xmlParams = value;}
		}

		public string FromDate
		{
			get{return fromDate;}
			set{fromDate = value;}
		}


		public string ToDate
		{
			get{return toDate;}
			set{toDate = value;}
		}


		public string LicensePlate
		{
			get{return licensePlate;}
			set{licensePlate = value;}
		}


		public string FleetName
		{
			get{return fleetName;}
			set{fleetName = value;}
		}


		public Int32 FleetId
		{
			get{return fleetId;}
			set{fleetId = value;}
		}


        public Int32 VehicleId
        {
            get { return vehicleId; }
            set { vehicleId = value; }
        }


        public Int32 DriverId
        {
            get { return driverId; }
            set { driverId = value; }
        }


        public string LandmarkName
        {
            get { return landmarkName; }
            set { landmarkName = value; }
        }

        public Int32 ReportFormat
		{
			get{return reportFormat;}
			set{reportFormat = value;}
		}
        public bool IsFleet
        {
            get
            {
                return isFleet; 
            }
            set
            {
                isFleet = value;
            }
        }

        public DataSet UserReportsDataSet
        {
            get
            {
                return userReportsDataSet;
            }
            set
            {
                userReportsDataSet = value;
            }
        }

        public DataSet UserExtendedReportsDataSet
        {
            get
            {
                return userExtendedReportsDataSet;
            }
            set
            {
                userExtendedReportsDataSet = value;
            }
        }
        public Int32 ReportTypeID
        {
            get
            {
                return reportTypeID;
            }
            set
            {
                reportTypeID = value;
            }
        }

		public string FleetReport
		{
			get{return fleetReport;}
			set{fleetReport = value;}
		}


        public string OrganizationHierarchyNodeCode
        {
            get { return organizationHierarchyNodeCode; }
            set { organizationHierarchyNodeCode = value; }
        }

        public bool OrganizationHierarchySelected
        {
            get { return organizationHierarchySelected; }
            set { organizationHierarchySelected = value; }
        }

        public string OrganizationHierarchyPath
        {
            get { return organizationHierarchyPath; }
            set { organizationHierarchyPath = value; }
        }
 	 }
 }

