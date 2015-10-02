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

      private DataSet dsFirmware;
      public DataSet DsFirmware
      {
         get { return dsFirmware; }
         set { dsFirmware = value; }
      }


      private DataSet dsLog;
      public DataSet DsLog
      {
          get { return dsLog; }
          set { dsLog = value; }
      }


		public clsAdmin()
		{
		}
	}
}
