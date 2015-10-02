using System;
using System.Data; 
namespace SentinelFM
{
	/// <summary>
	/// Summary description for clsAdmin.
	/// </summary>
	public class clsAdmin
	{

		private DataSet dsConfigFirmaware;
		public DataSet DsConfigFirmaware
		{
			get{return dsConfigFirmaware;}
			set{dsConfigFirmaware = value;}
		}


		

		public clsAdmin()
		{
		}
	}
}
