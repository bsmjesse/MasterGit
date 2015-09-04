using System;
using System.Data;
 
namespace SentinelFM
{
	/// <summary>
	/// Summary description for clsMapViewer.
	/// </summary>
	public class clsMapViewer
	{

		private string streetAddress="";
		private string vehicleName="";
		private string speed="";
		private string time="";
		private DataSet dsFleets = null;
		private DataSet dsVehicles = null;


		public DataSet DsFleets
		{
			get{return dsFleets;}
			set{dsFleets = value;}
		}


		public DataSet DsVehicles
		{
			get{return dsVehicles;}
			set{dsVehicles = value;}
		}

		public string StreetAddress
		{
			get{return streetAddress;}
			set{streetAddress = value;}
		}

		public string VehicleName
		{
			get{return vehicleName;}
			set{vehicleName = value;}
		}


		public string Speed
		{
			get{return speed;}
			set{speed = value;}
		}

		public string Time
		{
			get{return time;}
			set{time = value;}
		}

		

		public clsMapViewer()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}
}
